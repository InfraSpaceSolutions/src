package com.unitethiscity.ui;


import android.annotation.TargetApi;
import android.app.AlertDialog;
import android.app.Dialog;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RatingBar;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class ImageViewerDialog extends DialogFragment {

    private String mName = Constants.UTC_IMAGE_VIEWER_DIALOG;

    private final static String BUNDLE_ARGS_DRAWABLE = "Drawable";
    private final static String BUNDLE_ARGS_URL = "URL";

    View mParent;

    public ImageViewerDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static ImageViewerDialog newInstance(Drawable image, String url) {
        Bundle args = new Bundle();
        args.putParcelable(BUNDLE_ARGS_DRAWABLE, ((BitmapDrawable) image).getBitmap());
        args.putString(BUNDLE_ARGS_URL, url);
        ImageViewerDialog frag = new ImageViewerDialog();
        frag.setArguments(args);
        return frag;
    }

    public void show(FragmentManager manager) {
        show(manager, mName);
    }

    @Override
    public void show(FragmentManager manager, String tag) {
        synchronized (ImageViewerDialog.class) {
            if (manager.findFragmentByTag(tag) == null) {
                super.show(manager, tag);
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, mName + " created");

        mParent = inflater.inflate(R.layout.fragment_dialog_image_viewer, container);

        return mParent;
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        Bundle args = getArguments();
        Bitmap image = args.getParcelable(BUNDLE_ARGS_DRAWABLE);
        String url = args.getString(BUNDLE_ARGS_URL);
        ImageView imageViewer = (ImageView) mParent.findViewById(R.id.imageViewImageViewer);

        // no need to set bitmap here anymore but ImageView parameter maintained for potential
        // future usage
        //imageViewer.setImageBitmap(image);
        addImage(url, imageViewer);
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog ad = new AlertDialog.Builder(getActivity())
                .setTitle("")
                .show();
        ad.getWindow().setLayout(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        return ad;
    }

    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
    private void addImage(String url, ImageView iv) {
        AddImageTask ait = new AddImageTask(iv);
        if(Utils.hasHoneycomb()) {
            ait.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, url);
        }
        else {
            ait.execute(url);
        }
    }

    private class AddImageTask extends AsyncTask<String, Void, ImageView> {

        ImageView mIv;

        AddImageTask(ImageView iv) {
            mIv = iv;
        }

        protected ImageView doInBackground(String... url) {
            ImageView img = new ImageView(mParent.getContext());

            if(isAdded()) ((MainActivity) getActivity())
                    .getImageDownloader()
                    .download(url[0], img);

            return img;
        }

        protected void onPostExecute(ImageView img) {
            if(img != null && mIv != null) {
                if(img.getDrawable() != null && ((BitmapDrawable)img.getDrawable()).getBitmap() != null) {
                    mIv.setImageBitmap(((BitmapDrawable)img.getDrawable()).getBitmap());
                }
            }
        }
    }
}
