package com.unitethiscity.ui;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentActivity;
import android.support.v4.content.ContextCompat;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.style.ForegroundColorSpan;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.facebook.CallbackManager;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.FacebookRequestError;
import com.facebook.FacebookSdk;
import com.facebook.GraphRequest;
import com.facebook.GraphResponse;
import com.facebook.login.LoginResult;
import com.unitethiscity.R;
import com.unitethiscity.data.FacebookContext;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCGeolocationManager;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import org.json.JSONException;
import org.json.JSONObject;

import javax.xml.transform.Result;

public class SignupActivity extends FragmentActivity implements SignInDialog.SignInDialogListener {

    private String mName = Constants.SIGNUP_ACTIVITY;

    private final static int WHITE_TEXT_START = 13;

    // signin-related
    private static final String SIGNIN_ERROR_ALREADY_IN_USE = "Email address already in use.";

    // Facebook-related
    CallbackManager callbackManager;
    private boolean mBlockCallbacks = false;
    private static final String FACEBOOK_ERROR_DIFFERENT_USER = "User logged in as different Facebook user.";
    private static final String FACEBOOK_DEFAULT_GENDER = "?";
    private static final String FACEBOOK_DEFAULT_ZIP = "00000";
    private static final String FACEBOOK_DEFAULT_BIRTHDATE = "01/01/1900";
    private static final String FACEBOOK_DEFAULT_EMAIL_DOMAIN = "@www.unitethiscity.com";
    private String mFirstName = null;
    private String mLastName = null;
    private String mEmail = null;
    private String mGender = null;
    private String mBirthday = null;
    private String mZipCode = null;
    private String mUserID = null;
    private String mPromoCode = "";
    private boolean mAllInfoPresent = true;
    private boolean mLoginAfterSubscribe = false;
    private boolean mFacebookLoginFromSignin = false;

    private String mAccount;
    private String mPassword;

    SubscribeTask mSubscribeTask;

    SignInDialog mSignInDialog;

    private FacebookCallback<LoginResult> fbCallback = new FacebookCallback<LoginResult>() {

        @Override
        public void onSuccess(LoginResult result) {

            final LoginResult thisResult = result;

            GraphRequest request = GraphRequest.newMeRequest(
                    result.getAccessToken(),
                    new GraphRequest.GraphJSONObjectCallback() {
                        @Override
                        public void onCompleted(
                                JSONObject object,
                                GraphResponse response) {

                            FacebookRequestError err = response.getError();
                            if(err == null) {
                                try {
                                    mUserID = thisResult.getAccessToken().getUserId();
                                    mFirstName = "";
                                    mLastName = "";
                                    mEmail = String.valueOf(mUserID) + FACEBOOK_DEFAULT_EMAIL_DOMAIN;
                                    mGender = FACEBOOK_DEFAULT_GENDER;
                                    mBirthday = FACEBOOK_DEFAULT_BIRTHDATE;
                                    // always default the zip code
                                    mZipCode = FACEBOOK_DEFAULT_ZIP;

                                    mAllInfoPresent = true;

                                    if(object.has("first_name")) {
                                        mFirstName = object.getString("first_name");
                                    }

                                    if(object.has("last_name")) {
                                        mLastName = object.getString("last_name");
                                    }

                                    if(object.has("email")) {
                                        mEmail = object.getString("email");
                                    }

                                    if(object.has("gender")) {
                                        mGender = object.getString("gender");
                                    }

                                    if(object.has("birthday")) {
                                        mBirthday = object.getString("birthday");
                                    }

                                    LoginManager.getInstance().setFacebookContext(
                                            new FacebookContext(
                                                    mFirstName,
                                                    mLastName,
                                                    mEmail,
                                                    mGender,
                                                    mBirthday,
                                                    mUserID
                                            ));

                                    // attempt UTC login
                                    LoginManager.getInstance().loginFacebook(SignupActivity.this, mEmail, mUserID, mFacebookLoginFromSignin);
                                } catch (JSONException e) {
                                    e.printStackTrace();
                                }
                            }
                            else {
                                Toast.makeText(SignupActivity.this, "Facebook error: " + err.getErrorMessage(), Toast.LENGTH_SHORT).show();
                            }
                        }
                    });

            Bundle parameters = new Bundle();
            parameters.putString("fields", "first_name,last_name,email,gender,birthday");
            request.setParameters(parameters);
            request.executeAsync();
        }

        @Override
        public void onCancel() {

        }

        @Override
        public void onError(FacebookException error) {
            if(error.toString().equals(FACEBOOK_ERROR_DIFFERENT_USER)) {
                com.facebook.login.LoginManager.getInstance().logOut();
                Toast.makeText(SignupActivity.this, FACEBOOK_ERROR_DIFFERENT_USER + " Logging out, please try again.", Toast.LENGTH_SHORT).show();
            }
            else {
                Toast.makeText(SignupActivity.this, "Failed to login to Facebook", Toast.LENGTH_SHORT).show();
            }
        }

    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        FacebookSdk.sdkInitialize(getApplicationContext());

        setContentView(R.layout.activity_signup);

        callbackManager = CallbackManager.Factory.create();
        com.facebook.login.LoginManager.getInstance().registerCallback(callbackManager, fbCallback);

        // highlight part of the text
        TextView note1 = (TextView)findViewById(R.id.textViewSignupNote1);
        Spannable note1span = new SpannableString(note1.getText());
        note1span.setSpan(new ForegroundColorSpan(Color.WHITE), WHITE_TEXT_START, note1.getText().length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        note1.setText(note1span);

        // buttons and text
        Button signupEmail = (Button)findViewById(R.id.buttonSignupEmail);
        Button signupFacebook = (Button)findViewById(R.id.buttonSignupFacebook);
        TextView signin = (TextView)findViewById(R.id.textViewSignupAlreadyJoined);
        TextView skip = (TextView)findViewById(R.id.textViewSignupSkip);

        // touch handlers
        signupEmail.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                startMainActivity(Constants.MenuID.SUBSCRIBE, false, false);
                return true;
            }
        });
        signupFacebook.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                // start Facebook Login
                com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(SignupActivity.this, MainActivity.FB_READ_PERMISSIONS);
                return true;
            }
        });
        signin.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                mSignInDialog = SignInDialog.newInstance();
                mSignInDialog.show(getSupportFragmentManager());

                return true;
            }
        });
        skip.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                startMainActivity(Constants.MenuID.HOME, false, false);
                return true;
            }
        });

        UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
        lm.initializeLocationManager(this.getApplicationContext());
        if(ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED) {
            if(ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.ACCESS_FINE_LOCATION)) {
                Toast.makeText(this, "UTC needs your location to find businesses and deals near you", Toast.LENGTH_SHORT).show();
            }
            else {
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_COARSE_LOCATION}, Constants.PERMISSION_REQUEST_FINE_LOCATION);
            }
        }
        else {
            lm.setFineAccess(true);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
        UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
        switch (requestCode) {
            case Constants.PERMISSION_REQUEST_FINE_LOCATION: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    lm.setFineAccess(true);
                    if(lm.isCurrentLocationStale()) {
                        lm.locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_BOTH);
                    }
                }
                else {
                    lm.setFineAccess(false);
                    Toast.makeText(this, "UTC will not be able to accurately determine your location", Toast.LENGTH_SHORT).show();
                }

                return;
            }
            case Constants.PERMISSION_REQUEST_COARSE_LOCATION: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    lm.setCoarseAccess(true);
                    if(lm.isCurrentLocationStale()) {
                        lm.locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_BOTH);
                    }
                }
                else {
                    lm.setCoarseAccess(false);
                    Toast.makeText(this, "UTC will not be able to determine your location", Toast.LENGTH_SHORT).show();
                }

                return;
            }
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if(!mBlockCallbacks) {
            mBlockCallbacks = true;
            callbackManager.onActivityResult(requestCode, resultCode, data);
        }
    }

    private void startMainActivity(Constants.MenuID id, boolean signin, boolean facebook) {
        Intent intent = new Intent(SignupActivity.this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        intent.putExtra(MainActivity.EXTRA_NAVIGATION, id.getValue());
        if(signin) {
            intent.putExtra(MainActivity.EXTRA_SIGNIN, true);
        }
        if(facebook) {
            intent.putExtra(MainActivity.EXTRA_FACEBOOK_SIGNIN, true);
        }
        SignupActivity.this.startActivity(intent);
        finish();
    }

    @Override
    public void onFinishSignInDialog(int result, String email, String password) {
        Logger.info(mName, "onFinishSignInDialog: " + String.valueOf(result));

        switch (result) {
            case SignInDialog.RESULT_EMAIL_LOGIN:
                mEmail = email;
                mPassword = password;
                LoginManager.getInstance().login(this, mEmail, mPassword);
                break;
            case SignInDialog.RESULT_FACEBOOK_LOGIN:
                // start Facebook Login
                mFacebookLoginFromSignin = true;
                LoginManager.getInstance().getAccountContext().setFacebook(true);
                com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(SignupActivity.this, MainActivity.FB_READ_PERMISSIONS);
                break;
            case SignInDialog.RESULT_TAP_HERE:
                break;
            default:
                break;
        }
    }

    private class SubscribeTask extends AsyncTask<Void, Void, String> {
        protected String doInBackground(Void... params) {

            JSONObject json = new JSONObject();

            // request free sign up (version 3 - Facebook signup)
            json = UTCWebAPI.freeSignUp3(mFirstName, mLastName, mEmail,
                    mZipCode, mGender, mBirthday, mPromoCode, mUserID);


            // if JSON request didn't work, bail
            if(json == null) {
                return "Sign up request failed (1)";
            }

            if(json.has("Message")) {
                try {
                    return json.getString("Message");
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            if(!json.has("Account") && !json.has("Password")) {
                return "Sign up request failed (2)";
            }

            try {
                mAccount = json.getString("Account");
            } catch (JSONException e) {
                e.printStackTrace();
            }
            try {
                mPassword = json.getString("Password");
            } catch (JSONException e) {
                e.printStackTrace();
            }

            return null;
        }

        protected void onPostExecute(String result) {
            if (result == null || (mUserID != null && result.equals(SIGNIN_ERROR_ALREADY_IN_USE))) {
                Logger.info(mName, "Account - " + mAccount + ", Password " + mPassword);
                FacebookContext fc = LoginManager.getInstance().getFacebookContext();
                fc.setAccount(mAccount);
                fc.setPassword(mPassword);
                LoginManager.getInstance().setFacebookContext(fc);
                // attempt UTC login
                mLoginAfterSubscribe = true;
                LoginManager.getInstance().loginFacebook(SignupActivity.this, mEmail, mUserID, false);
            }
            else {
                Toast.makeText(SignupActivity.this, result, Toast.LENGTH_SHORT).show();
            }
        }
    }

    public boolean cancelSubscribeTask() {
        boolean result = false;
        if(mSubscribeTask != null) {
            result = mSubscribeTask.cancel(true);
        }
        return result;
    }

    public void callbackLoginSuccess() {
        mBlockCallbacks = false;
        startMainActivity(Constants.MenuID.HOME, false, true);
    }

    public void callbackLoginSuccessFromSignin() {
        mBlockCallbacks = false;
        startMainActivity(Constants.MenuID.HOME, false, true);
    }

    public void callbackLoginFailure() {
        mBlockCallbacks = false;
        if(mLoginAfterSubscribe) {
            Toast.makeText(this, "Registration complete but failed to login.", Toast.LENGTH_SHORT).show();
        }
        else {
            if (mAllInfoPresent) {
                // register with Facebook info
                mSubscribeTask = new SubscribeTask();
                if(Utils.hasHoneycomb()) {
                    mSubscribeTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                }
                else {
                    mSubscribeTask.execute();
                }
            } else {
                Toast.makeText(this, "More information required to complete registration.", Toast.LENGTH_SHORT).show();

                // lacking info and need user to manually complete
                startMainActivity(Constants.MenuID.SUBSCRIBE, false, true);
            }
        }
    }
}
