package com.unitethiscity.ui;

import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.share.Sharer;
import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

public class RedeemRewardDialog extends DialogFragment {

    private String mName = Constants.UTC_REDEEM_REWARD_DIALOG;

    View mParent;

    private final static String BUNDLE_ARGS_SHARE_TITLE = "ShareTitle";
    private final static String BUNDLE_ARGS_DEAL_AMOUNT = "DealAmount";
    private final static String BUNDLE_ARGS_DEAL_DESCRIPTION = "DealDescription";
    private final static String BUNDLE_ARGS_BUSINESS_IMAGE = "BusinessImage";
    private final static String BUNDLE_ARGS_LOCATION_ID = "LocationID";
    private final static String BUNDLE_ARGS_BUSINESS_ID = "BusinessID";
    private final static String BUNDLE_ARGS_ERROR_MESSAGE = "ErrorMessage";

    public final static int RESULT_SHARED = 1;
    public final static int RESULT_CANCELED = 2;

    public final static int REQUEST_TWITTER = 10;
    public final static int REQUEST_FACEBOOK = 11;
    public final static int REQUEST_INSTAGRAM = 12;
    public final static int REQUEST_TAKE_PHOTO = 13;

    public final static int DEFAULT_SOCIAL_NONE = 0;
    public final static int DEFAULT_SOCIAL_IGNORE = 1;
    public final static int DEFAULT_SOCIAL_TWITTER = 2;
    public final static int DEFAULT_SOCIAL_FACEBOOK = 3;
    public final static int DEFAULT_SOCIAL_INSTAGRAM = 4;

    public static boolean TwitterShared = false;
    public static boolean FacebookShared = false;
    public static boolean InstagramShared = false;

    private String mBusinessID = "";
    private String mCurrentPhotoPath = "";

    private int mDefaultSocialSetting = 0;
    private int mPendingSocialSetting = 0;
    private View mCurrentSocialView;
    private boolean mIgnoreSocialSetting = false;

    FacebookCallback<Sharer.Result> fbShareCallback = new FacebookCallback<Sharer.Result>() {
        @Override
        public void onSuccess(Sharer.Result result) {
            ((MainActivity)getActivity()).submitFacebookPost(Integer.valueOf(mBusinessID));

            if(shared() == false) {
                // redeem
                DataManager.getInstance().setScanTask(DataManager.ScanTask.REDEEM);
                DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
                        null, Constants.Role.MEMBER.getValue(), -1, true, false);

                FacebookShared = true;
            }
            else {
                FacebookShared = true;

                updateSuccess();
            }
        }

        @Override
        public void onCancel() {
            if(shared() == false) {
                // indicate to user they canceled the share
                Toast.makeText(RedeemRewardDialog.this.getActivity(), "You must share this reward before you can redeem!", Toast.LENGTH_SHORT).show();
            }
        }

        @Override
        public void onError(FacebookException error) {
            // indicate error to user
            Toast.makeText(RedeemRewardDialog.this.getActivity(), "An error occurred while attempting to share", Toast.LENGTH_SHORT).show();
        }
    };

    public RedeemRewardDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static RedeemRewardDialog newInstance(String title, String dealAmount, String dealDesc, String busImage, String locID, String busID, String error) {
        Bundle args = new Bundle();
        args.putString(BUNDLE_ARGS_SHARE_TITLE, title);
        args.putString(BUNDLE_ARGS_DEAL_AMOUNT, dealAmount);
        args.putString(BUNDLE_ARGS_DEAL_DESCRIPTION, dealDesc);
        args.putString(BUNDLE_ARGS_BUSINESS_IMAGE, busImage);
        args.putString(BUNDLE_ARGS_LOCATION_ID, locID);
        args.putString(BUNDLE_ARGS_BUSINESS_ID, busID);
        if(error != null) {
            args.putString(BUNDLE_ARGS_ERROR_MESSAGE, error);
        }
        RedeemRewardDialog frag = new RedeemRewardDialog();
        frag.setArguments(args);
        return frag;
    }

    public void show(FragmentManager manager) {
        show(manager, mName);
    }

    @Override
    public void show(FragmentManager manager, String tag) {
        synchronized (RedeemRewardDialog.class) {
            if (manager.findFragmentByTag(tag) == null) {
                super.show(manager, tag);
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, mName + " created");

        mParent = inflater.inflate(R.layout.fragment_dialog_redeem_reward, container);

        return mParent;
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        Bundle args = getArguments();

        final String title = args.getString(BUNDLE_ARGS_SHARE_TITLE);
        final String dealAmount = args.getString(BUNDLE_ARGS_DEAL_AMOUNT);
        final String dealDescription = args.getString(BUNDLE_ARGS_DEAL_DESCRIPTION);
        final String businessImage = args.getString(BUNDLE_ARGS_BUSINESS_IMAGE);
        final String locationID = args.getString(BUNDLE_ARGS_LOCATION_ID);
        mBusinessID = args.getString(BUNDLE_ARGS_BUSINESS_ID);

        final SharedPreferences settings = getActivity().getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
        mDefaultSocialSetting = settings.getInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, DEFAULT_SOCIAL_NONE);
        mPendingSocialSetting = mDefaultSocialSetting;
        mIgnoreSocialSetting = false;

        TwitterShared = false;
        FacebookShared = false;
        InstagramShared = false;

        final LinearLayout redeemBody = (LinearLayout)mParent.findViewById(R.id.linearLayoutRedeemRewardDialogBody);
        final LinearLayout defaultSocial = (LinearLayout)mParent.findViewById(R.id.linearLayoutRedeemRewardDialogSetting);

        TextView tv = (TextView)mParent.findViewById(R.id.redeemRewardBusinessName);
        tv.setText(title);

        tv = (TextView)mParent.findViewById(R.id.redeemRewardDealAmount);
        tv.setText(dealAmount);

        tv = (TextView)mParent.findViewById(R.id.redeemRewardDescription);
        tv.setText(dealDescription);

        Button twitter = (Button)mParent.findViewById(R.id.btnRedeemRewardShareTwitter);
        Button facebook = (Button)mParent.findViewById(R.id.btnRedeemRewardShareFacebook);
        Button instagram = (Button)mParent.findViewById(R.id.btnRedeemRewardShareInstagram);

        twitter.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(mDefaultSocialSetting != DEFAULT_SOCIAL_TWITTER && mIgnoreSocialSetting == false) {
                    // save view
                    mCurrentSocialView = v;
                    mPendingSocialSetting = DEFAULT_SOCIAL_TWITTER;

                    // show yes/no default social option
                    redeemBody.setVisibility(LinearLayout.GONE);
                    defaultSocial.setVisibility(LinearLayout.VISIBLE);
                }
                else {
                    publishTweet(title + ": " + dealAmount + " Savings, " +
                            Constants.DEFAULT_DESCRIPTION_PREFIX + " " +
                            Constants.BUSINESS_DIRECTORY + locationID +
                            Constants.BUSINESS_DIRECTORY_NO_MAPS);
                }
            }
        });

        facebook.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(mDefaultSocialSetting != DEFAULT_SOCIAL_FACEBOOK && mIgnoreSocialSetting == false) {
                    // save view
                    mCurrentSocialView = v;
                    mPendingSocialSetting = DEFAULT_SOCIAL_FACEBOOK;

                    // show yes/no default social option
                    redeemBody.setVisibility(LinearLayout.GONE);
                    defaultSocial.setVisibility(LinearLayout.VISIBLE);
                }
                else {
                    ((MainActivity) getActivity()).publishFeedDialog(null,
                            title,
                            null,
                            Constants.BUSINESS_DIRECTORY + locationID + Constants.BUSINESS_DIRECTORY_NO_MAPS,
                            businessImage,
                            Constants.DEFAULT_DESCRIPTION_PREFIX,
                            Integer.valueOf(mBusinessID),
                            fbShareCallback);
                }
            }
        });

        instagram.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(mDefaultSocialSetting != DEFAULT_SOCIAL_INSTAGRAM && mIgnoreSocialSetting == false) {
                    // save view
                    mCurrentSocialView = v;
                    mPendingSocialSetting = DEFAULT_SOCIAL_INSTAGRAM;

                    // show yes/no default social option
                    redeemBody.setVisibility(LinearLayout.GONE);
                    defaultSocial.setVisibility(LinearLayout.VISIBLE);
                }
                else {
                    // request camera for taking picture to share with Instagram
                    dispatchTakePictureIntent();
                }
            }
        });

        Button defaultYes = (Button)mParent.findViewById(R.id.btnRedeemRewardDefaultSocialYes);
        Button defaultNo = (Button)mParent.findViewById(R.id.btnRedeemRewardDefaultSocialNo);

        defaultYes.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // save setting
                mDefaultSocialSetting = mPendingSocialSetting;
                settings.edit().putInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, mDefaultSocialSetting).apply();

                defaultSocial.setVisibility(LinearLayout.GONE);
                redeemBody.setVisibility(LinearLayout.VISIBLE);
                mCurrentSocialView.performClick();
            }
        });

        defaultNo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // ignore future requests for this dialog
                mIgnoreSocialSetting = true;

                defaultSocial.setVisibility(LinearLayout.GONE);
                redeemBody.setVisibility(LinearLayout.VISIBLE);
                mCurrentSocialView.performClick();
            }
        });

        if(mDefaultSocialSetting == DEFAULT_SOCIAL_TWITTER) {
            twitter.performClick();
        }
        else if(mDefaultSocialSetting == DEFAULT_SOCIAL_FACEBOOK) {
            facebook.performClick();
        }
        else if(mDefaultSocialSetting == DEFAULT_SOCIAL_INSTAGRAM) {
            instagram.performClick();
        }
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog ad = new AlertDialog.Builder(getActivity())
                .setTitle("")
                .show();
        ad.getWindow().setLayout(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        return ad;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        Logger.verbose(mName, "onActivityResult()");

        if(requestCode == REQUEST_TWITTER) {
            ((MainActivity)getActivity()).submitTweetPost(Integer.valueOf(mBusinessID));

            if(shared() == false) {
                // redeem
                DataManager.getInstance().setScanTask(DataManager.ScanTask.REDEEM);
                DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
                        null, Constants.Role.MEMBER.getValue(), -1, true, false);

                TwitterShared = true;
            }
            else {
                TwitterShared = true;

                updateSuccess();
            }
        }
        else if(requestCode == REQUEST_INSTAGRAM) {
            ((MainActivity)getActivity()).submitInstagramPost(Integer.valueOf(mBusinessID));

            if(shared() == false) {
                // redeem
                DataManager.getInstance().setScanTask(DataManager.ScanTask.REDEEM);
                DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
                        null, Constants.Role.MEMBER.getValue(), -1, true, false);

                InstagramShared = true;
            }
            else {
                InstagramShared = true;

                updateSuccess();
            }
        }
        else if(requestCode == REQUEST_TAKE_PHOTO) {
            if(resultCode == Activity.RESULT_OK) {
                Logger.info(mName, "Photo taken, request share intent (for use with Instagram)");
                publishInstagram(mCurrentPhotoPath);
            }
            else if(resultCode == Activity.RESULT_CANCELED) {
                Logger.info(mName, "Take photo request canceled");
            }
            else {
                Logger.info(mName, "Take photo request error");
                Toast.makeText(getActivity(), "An error occurred while attempting to take a photo",
                        Toast.LENGTH_SHORT).show();
            }
        }
    }

    public void updateSuccess() {
        if(isAdded()) {
            // title green background #7CB342
            LinearLayout ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogTitle);
            ll.setBackgroundColor(Color.parseColor("#7CB342"));

            // change title
            TextView tv = (TextView) mParent.findViewById(R.id.redeemRewardDialogTitle);
            tv.setText(getString(R.string.dialog_redeem_reward_thanks));

            // show savings redeemed subtitle
            tv = (TextView) mParent.findViewById(R.id.redeemRewardSavingsRedeemed);
            tv.setVisibility(TextView.VISIBLE);

            // show thanks for support phrase
            tv = (TextView) mParent.findViewById(R.id.redeemRewardThanksLocal);
            tv.setVisibility(TextView.VISIBLE);

            // update sharing description
            tv = (TextView) mParent.findViewById(R.id.redeemRewardShare);
            tv.setText(getString(R.string.dialog_redeem_reward_share_more));

            if (TwitterShared) {
                // hide twitter
                ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardShareTwitter);
                ll.setVisibility(LinearLayout.GONE);
            }

            if (FacebookShared) {
                // hide facebook
                ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardShareFacebook);
                ll.setVisibility(LinearLayout.GONE);
            }

            if (InstagramShared) {
                // hide instagram
                ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardShareInstagram);
                ll.setVisibility(LinearLayout.GONE);
            }

            // hide UTC logo
            ImageView iv = (ImageView) mParent.findViewById(R.id.imageViewRedeemRewardUTCLogo);
            iv.setVisibility(ImageView.GONE);
        }
    }

    public void updateFailure(String errorMessage) {
        TwitterShared = false;
        FacebookShared = false;
        InstagramShared = false;

        if(isAdded()) {
            // title orange background #FF6D00
            LinearLayout ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogTitle);
            ll.setBackgroundColor(Color.parseColor("#FF6D00"));

            // change title
            TextView tv = (TextView) mParent.findViewById(R.id.redeemRewardDialogTitle);
            tv.setText(getString(R.string.dialog_redeem_reward_not_redeemed));

            // set error message
            tv = (TextView) mParent.findViewById(R.id.redeemRewardErrorMessage);
            tv.setText(errorMessage);

            // hide normal redeem body
            ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogBody);
            ll.setVisibility(LinearLayout.GONE);

            // show failure body
            ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogBodyNotRedeemed);
            ll.setVisibility(LinearLayout.VISIBLE);

            Button retry = (Button) mParent.findViewById(R.id.buttonRedeemRewardRetry);
            retry.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {

                    // hide failure body
                    LinearLayout ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogBodyNotRedeemed);
                    ll.setVisibility(LinearLayout.GONE);

                    // show normal redeem body
                    ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogBody);
                    ll.setVisibility(LinearLayout.VISIBLE);

                    // change title
                    TextView tv = (TextView) mParent.findViewById(R.id.redeemRewardDialogTitle);
                    tv.setText(getString(R.string.dialog_redeem_reward));

                    // title orange background #3B3B3B
                    ll = (LinearLayout) mParent.findViewById(R.id.linearLayoutRedeemRewardDialogTitle);
                    ll.setBackgroundColor(Color.parseColor("#3B3B3B"));
                }
            });
        }
    }

    public boolean shared() {
        return TwitterShared || FacebookShared || InstagramShared;
    }

    private void publishTweet(String message) {
        String url = "https://twitter.com/intent/tweet?source=webclient&text=" + message.replace(" ", "+");
        Intent i = new Intent(Intent.ACTION_VIEW);
        i.setData(Uri.parse(url));
        startActivityForResult(i, REQUEST_TWITTER);
    }

    private void publishInstagram(String mediaPath) {
        Intent share = new Intent(Intent.ACTION_SEND);
        share.setType("image/*");
        File media = new File(mediaPath);
        Uri uri = Uri.fromFile(media);
        share.putExtra(Intent.EXTRA_STREAM, uri);
        share.putExtra(Intent.EXTRA_TITLE, "Test text");
        share.setPackage("com.instagram.android");
        startActivityForResult(Intent.createChooser(share, "Share to"), REQUEST_INSTAGRAM);
    }

    private File createImageFile() throws IOException {
        File image = null;

        if(ContextCompat.checkSelfPermission(getActivity(), android.Manifest.permission.WRITE_EXTERNAL_STORAGE)
                != PackageManager.PERMISSION_GRANTED) {
            if(ActivityCompat.shouldShowRequestPermissionRationale(getActivity(), Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
                Toast.makeText(getActivity(), "UTC needs write access to capture photos you want to share", Toast.LENGTH_SHORT).show();
            }
            else {
                ActivityCompat.requestPermissions(getActivity(), new String[]{android.Manifest.permission.WRITE_EXTERNAL_STORAGE}, Constants.PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE_1);
            }
        }
        else {
            // create an image file name
            String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
            String imageFileName = "UTC_" + timeStamp + "_";
            File storageDir = Environment.getExternalStoragePublicDirectory(
                    Environment.DIRECTORY_PICTURES);
            image = File.createTempFile(
                    imageFileName,  /* prefix */
                    ".jpg",         /* suffix */
                    storageDir      /* directory */
            );

            // save file path
            mCurrentPhotoPath =  image.getAbsolutePath();
        }

        return image;
    }

    public void successPermissionWriteExternalStorage() {
        dispatchTakePictureIntent();
    }

    private void dispatchTakePictureIntent() {
        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        // ensure that there's a camera activity to handle the intent
        if (takePictureIntent.resolveActivity(getActivity().getPackageManager()) != null) {
            // create the File where the photo should go
            File photoFile = null;
            try {
                photoFile = createImageFile();
            } catch (IOException ex) {
                // error occurred while creating the File
                Logger.error(mName, "error creating image file: " + ex.toString());
            }
            // continue only if the File was successfully created
            if (photoFile != null) {
                takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT,
                        Uri.fromFile(photoFile));
                startActivityForResult(takePictureIntent, REQUEST_TAKE_PHOTO);
            }
        }
    }

    public interface RedeemRewardDialogDialogListener {
        void onFinishRedeemRewardDialogDialog(int result, float rating);
    }
}
