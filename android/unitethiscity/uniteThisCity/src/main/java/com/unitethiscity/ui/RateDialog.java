package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.app.Dialog;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.FragmentManager;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.RatingBar;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;


public class RateDialog extends DialogFragment {

    private String mName = Constants.UTC_RATE_DIALOG;

    private final static String BUNDLE_ARGS_BUSINESS_NAME = "BusinessName";
    private final static String BUNDLE_ARGS_RATING = "Rating";

    public final static int RESULT_RATED = 1;

    public RateDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static RateDialog newInstance(String busName, float rating) {
        Bundle args = new Bundle();
        args.putString(BUNDLE_ARGS_BUSINESS_NAME, busName);
        args.putFloat(BUNDLE_ARGS_RATING, rating);
        RateDialog frag = new RateDialog();
        frag.setArguments(args);
        return frag;
    }

    public void show(FragmentManager manager) {
        show(manager, mName);
    }

    @Override
    public void show(FragmentManager manager, String tag) {
        synchronized (RateDialog.class) {
            if (manager.findFragmentByTag(tag) == null) {
                super.show(manager, tag);
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, mName + " created");

        return inflater.inflate(R.layout.fragment_dialog_rate, container);
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        final View vw = view;

        Bundle args = getArguments();

        Button b = (Button) view.findViewById(R.id.btnRateDone);
        final RatingBar rb = (RatingBar) view.findViewById(R.id.ratingBarRateDialog);
        rb.setRating(args.getFloat(BUNDLE_ARGS_RATING, 0.0f));
        b.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // pass back selected rating
                RateDialogListener activity = (RateDialogListener) DataManager.getInstance().getRedeemFragment();
                activity.onFinishRateDialog(RESULT_RATED, rb.getRating());
                RateDialog.this.dismiss();

                return true;
            }
        });

        TextView tv = (TextView) view.findViewById(R.id.rateBusinessName);
        tv.setText(args.getString(BUNDLE_ARGS_BUSINESS_NAME, ""));
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog ad = new AlertDialog.Builder(getActivity())
                .setTitle("")
                .show();
        ad.getWindow().setLayout(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        return ad;
    }

    public interface RateDialogListener {
        void onFinishRateDialog(int result, float rating);
    }
}
