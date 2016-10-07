package com.unitethiscity.ui;

import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.RatingBar;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;


public class LoyaltyDialog extends DialogFragment {

    private String mName = Constants.UTC_LOYALTY_DIALOG;

    View mParent;

    private final static String BUNDLE_ARGS_SHARE_TITLE = "ShareTitle";
    private final static String BUNDLE_ARGS_BUSINESS_IMAGE = "BusinessImage";
    private final static String BUNDLE_ARGS_LOCATION_ID = "LocationID";
    private final static String BUNDLE_ARGS_BUSINESS_ID = "BusinessID";
    private final static String BUNDLE_ARGS_ERROR_MESSAGE = "ErrorMessage";

    public final static int RESULT_SHARED = 1;
    public final static int RESULT_CANCELED = 2;

    public final static int REQUEST_TAKE_PHOTO = 13;

    private String mCurrentPhotoPath = "";

    public LoyaltyDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static LoyaltyDialog newInstance(String title, String busImage, String locID, String busID, String error) {
        Bundle args = new Bundle();
        args.putString(BUNDLE_ARGS_SHARE_TITLE, title);
        args.putString(BUNDLE_ARGS_BUSINESS_IMAGE, busImage);
        args.putString(BUNDLE_ARGS_LOCATION_ID, locID);
        args.putString(BUNDLE_ARGS_BUSINESS_ID, busID);
        if(error != null) {
            args.putString(BUNDLE_ARGS_ERROR_MESSAGE, error);
        }
        LoyaltyDialog frag = new LoyaltyDialog();
        frag.setArguments(args);
        return frag;
    }

    public void show(FragmentManager manager) {
        show(manager, mName);
    }

    @Override
    public void show(FragmentManager manager, String tag) {
        synchronized (SignInDialog.class) {
            if (manager.findFragmentByTag(tag) == null) {
                super.show(manager, tag);
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, mName + " created");

        mParent = inflater.inflate(R.layout.fragment_dialog_loyalty, container);

        return mParent;
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        Bundle args = getArguments();

        final String title = args.getString(BUNDLE_ARGS_SHARE_TITLE);
        final String businessImage = args.getString(BUNDLE_ARGS_BUSINESS_IMAGE);
        final String locationID = args.getString(BUNDLE_ARGS_LOCATION_ID);
        final String businessID = args.getString(BUNDLE_ARGS_BUSINESS_ID);
        String error = null;
        if(args.containsKey(BUNDLE_ARGS_ERROR_MESSAGE)) {
            error = args.getString(BUNDLE_ARGS_ERROR_MESSAGE);
        }

        if(error == null) {
            Button twitter = (Button) mParent.findViewById(R.id.btnLoyaltyShareTwitter);
            Button facebook = (Button) mParent.findViewById(R.id.btnLoyaltyShareFacebook);
            Button instagram = (Button) mParent.findViewById(R.id.btnLoyaltyShareInstagram);

            twitter.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {

                    ((MainActivity) getActivity()).publishTweet(title + ", " +
                            Constants.DEFAULT_DESCRIPTION_PREFIX + " " +
                            Constants.BUSINESS_DIRECTORY + locationID +
                            Constants.BUSINESS_DIRECTORY_NO_MAPS, Integer.valueOf(businessID));
                }
            });

            facebook.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {

                    ((MainActivity) getActivity()).publishFeedDialog(null,
                            title,
                            null,
                            Constants.BUSINESS_DIRECTORY + locationID + Constants.BUSINESS_DIRECTORY_NO_MAPS,
                            businessImage,
                            Constants.DEFAULT_DESCRIPTION_PREFIX,
                            Integer.valueOf(businessID),
                            null);
                }
            });

            instagram.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    // request camera for taking picture to share with Instagram
                    dispatchTakePictureIntent();
                }
            });
        }
        else {
            LinearLayout body = (LinearLayout) mParent.findViewById(R.id.linearLayoutLoyaltyDialogBody);
            body.setVisibility(LinearLayout.INVISIBLE);
            TextView errorMessage = (TextView) mParent.findViewById(R.id.loyaltyErrorMessage);
            errorMessage.setVisibility(TextView.VISIBLE);
            errorMessage.setText(error);
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

        if(requestCode == REQUEST_TAKE_PHOTO) {
            if(resultCode == Activity.RESULT_OK) {
                Logger.info(mName, "Photo taken, request share intent (for use with Instagram)");
                ((MainActivity)getActivity()).publishInstagram(mCurrentPhotoPath);
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

    public interface LoyaltyDialogListener {
        void onFinishLoyaltyDialog(int result, float rating);
    }
}
