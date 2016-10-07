package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.app.Dialog;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.FragmentManager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class ReviewViewerDialog extends DialogFragment {

    private String mName = Constants.UTC_REVIEW_VIEWER_DIALOG;

    private final static String BUNDLE_ARGS_NAME = "Name";
    private final static String BUNDLE_ARGS_REVIEW = "Review";

    View mParent;

    public ReviewViewerDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static ReviewViewerDialog newInstance(String name, String review) {
        Bundle args = new Bundle();
        args.putString(BUNDLE_ARGS_NAME, name);
        args.putString(BUNDLE_ARGS_REVIEW, review);
        ReviewViewerDialog frag = new ReviewViewerDialog();
        frag.setArguments(args);
        return frag;
    }

    public void show(FragmentManager manager) {
        show(manager, mName);
    }

    @Override
    public void show(FragmentManager manager, String tag) {
        synchronized (ReviewViewerDialog.class) {
            if (manager.findFragmentByTag(tag) == null) {
                super.show(manager, tag);
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, mName + " created");

        mParent = inflater.inflate(R.layout.fragment_dialog_review_viewer, container);

        return mParent;
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        Bundle args = getArguments();
        String name = args.getString(BUNDLE_ARGS_NAME);
        String review = args.getString(BUNDLE_ARGS_REVIEW);

        TextView tv = (TextView) mParent.findViewById(R.id.reviewName);
        tv.setText(name);

        tv = (TextView) mParent.findViewById(R.id.reviewText);
        tv.setText(review);
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog ad = new AlertDialog.Builder(getActivity())
                .setTitle("")
                .show();
        ad.getWindow().setLayout(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        return ad;
    }
}
