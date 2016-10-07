package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.net.Uri;
import android.support.v4.app.FragmentManager;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class SignInDialog extends DialogFragment {

    private String mName = Constants.UTC_SIGN_IN_DIALOG;

    public final static int RESULT_TAP_HERE = 1;
    public final static int RESULT_EMAIL_LOGIN = 2;
    public final static int RESULT_FACEBOOK_LOGIN = 3;

    private String mEmail;
    private String mPassword;

    public SignInDialog() {
        // Required empty public constructor
    }

    // dialog constructor
    public static SignInDialog newInstance() {
        SignInDialog frag = new SignInDialog();
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

        return inflater.inflate(R.layout.fragment_dialog_sign_in, container);
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        final View vw = view;

        mEmail = "";
        mPassword = "";

        Button b = (Button) view.findViewById(R.id.btnSignInEmail);
        b.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // show email sign in
                RelativeLayout rl = (RelativeLayout) vw.findViewById(R.id.relativeLayoutSignInButtons);
                rl.setVisibility(RelativeLayout.INVISIBLE);
                rl = (RelativeLayout) vw.findViewById(R.id.relativeLayoutSignInDialogEmail);
                rl.setVisibility(RelativeLayout.VISIBLE);

                return true;
            }
        });

        b = (Button) view.findViewById(R.id.btnSignInFacebook);
        b.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // sign in with Facebook
                SignInDialogListener activity = (SignInDialogListener) getActivity();
                activity.onFinishSignInDialog(RESULT_FACEBOOK_LOGIN, "", "");
                SignInDialog.this.dismiss();

                return true;
            }
        });

        TextView tv = (TextView) view.findViewById(R.id.signInDialogTapHere);
        tv.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // close
                SignInDialogListener activity = (SignInDialogListener) getActivity();
                activity.onFinishSignInDialog(RESULT_TAP_HERE, "", "");
                SignInDialog.this.dismiss();

                return true;
            }
        });

        b = (Button) view.findViewById(R.id.btnSignInCancel);
        b.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // go back to sign in buttons
                RelativeLayout rl = (RelativeLayout) vw.findViewById(R.id.relativeLayoutSignInDialogEmail);
                rl.setVisibility(RelativeLayout.INVISIBLE);
                rl = (RelativeLayout) vw.findViewById(R.id.relativeLayoutSignInButtons);
                rl.setVisibility(RelativeLayout.VISIBLE);

                return true;
            }
        });

        b = (Button) view.findViewById(R.id.btnSignInLogin);
        b.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                // perform email login
                SignInDialogListener activity = (SignInDialogListener) getActivity();
                activity.onFinishSignInDialog(RESULT_EMAIL_LOGIN, mEmail, mPassword);
                SignInDialog.this.dismiss();

                return true;
            }
        });

        tv = (TextView) view.findViewById(R.id.signInForgotPassword);
        tv.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                Uri url = Uri.parse(Constants.UNITE_THIS_CITY_FORGOT_PASSWORD);
                Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
                try {
                    startActivity(launchBrowser);
                }
                catch(ActivityNotFoundException anfe) {
                    anfe.printStackTrace();
                }

                return false;
            }
        });

        EditText et = (EditText) view.findViewById(R.id.editTextSignInDialogEmail);
        et.addTextChangedListener(new TextWatcher() {

            @Override
            public void afterTextChanged(Editable s) {

            }

            @Override
            public void beforeTextChanged(CharSequence s, int start,
                                          int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start,
                                      int before, int count) {
                mEmail = s.toString();
            }

        });

        et = (EditText) view.findViewById(R.id.editTextSignInDialogPassword);
        et.addTextChangedListener(new TextWatcher() {

            @Override
            public void afterTextChanged(Editable s) {

            }

            @Override
            public void beforeTextChanged(CharSequence s, int start,
                                          int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start,
                                      int before, int count) {
                mPassword = s.toString();
            }

        });
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        super.onCreateDialog(savedInstanceState);

        Dialog dialog = new Dialog(getActivity(), R.style.CustomDialogTheme);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        View view = getActivity().getLayoutInflater().inflate(R.layout.fragment_dialog_sign_in, null);
        dialog.setContentView(view);
        return dialog;
    }

    public interface SignInDialogListener {
        void onFinishSignInDialog(int result, String email, String password);
    }
}
