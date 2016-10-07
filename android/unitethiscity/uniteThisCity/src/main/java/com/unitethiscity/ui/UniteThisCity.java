package com.unitethiscity.ui;

import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.R;
import com.unitethiscity.data.AccountContext;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.app.Activity;
import android.content.DialogInterface;
import android.content.DialogInterface.OnDismissListener;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.widget.Toast;

import java.io.IOException;

public class UniteThisCity extends Activity  
{  
	private String mName = Constants.UNITE_THIS_CITY;
	
	private LoadViewTask task;

    @Override  
    public void onCreate(Bundle savedInstanceState)  
    {  
        super.onCreate(savedInstanceState);

        // determine application version
        String versionText = "";
        PackageInfo pInfo;
        try {
            pInfo = getPackageManager().getPackageInfo(getPackageName(), 0);
            versionText = pInfo.versionName;
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }

        // get stored version
        SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
        String storedVersion = settings.getString(Constants.SHARED_PREFERENCES_VERSION, "");

        if (storedVersion.equals(versionText) == false) {
            // if version is different, clear account context
            Utils.delete(this, Constants.ACCOUNT_CONTEXT);
            LoginManager.getInstance().setAccountContext(new AccountContext(), false);

            // clear shared prefs
            settings.edit().clear().commit();

            // store version
            settings.edit().putString(Constants.SHARED_PREFERENCES_VERSION, versionText).commit();
        }
        else if(Utils.saveExists(this, Constants.ACCOUNT_CONTEXT)) {
            // check if account context has been previously saved, and attempt to
            // deserialize it into our account context within the login manager
            try {
                Object obj = Utils.open(this, Constants.ACCOUNT_CONTEXT);
                AccountContext ac = (AccountContext) obj;
                if(ac != null) {
                    LoginManager.getInstance().setAccountContext((AccountContext) obj, true);
                }
                else {
                    LoginManager.getInstance().setAccountContext(new AccountContext(), false);
                }
            } catch (IOException e) {
                e.printStackTrace();
            } catch (ClassNotFoundException e) {
                e.printStackTrace();
            }
        }
        else {
            LoginManager.getInstance().setAccountContext(new AccountContext(), false);
        }

        if(LoginManager.getInstance().userLoggedIn() &&
                LoginManager.getInstance().getAccountContext().getToken() == null) {
            LoginManager.getInstance().setAccountContext(new AccountContext(), false);
        }

        task = new LoadViewTask();
        if(Utils.hasHoneycomb()) {
            task.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
        }
        else {
            task.execute();
        }
    }
    
    @Override
    public void onStart() {
    	super.onStart();
    	
    	Logger.verbose(mName, "onStart()");
    }
    
    @Override
    public void onStop() {
    	super.onStop();
    	
    	Logger.verbose(mName, "onStop()");
    }
  
    private class LoadViewTask extends AsyncTask<Void, Integer, String>  implements OnDismissListener 
    {
        private boolean running = true;

        @Override
        protected void onCancelled() {
            running = false;
        }
    	
        @Override  
        protected void onPreExecute()  
        {  
        	setContentView(R.layout.activity_splash); 
        }  
  
        @Override  
        protected String doInBackground(Void... params)  
        {
            // we'll be doing any work we need in the background and
            // once we are finished, onPostExecute will start the main
            // activity intent if the splash isn't exited from (like a back
            // button press)

            JSONObject version = UTCWebAPI.getVersion();
            int major = 0;
            int minor = 0;
            int patch = 0;

            if(version != null) {
                boolean hasAllAPI = true;
                // parse version information
                if(version.has(DataManager.APIVersion.MAJOR)) {
                    try {
                        major = version.getInt(DataManager.APIVersion.MAJOR);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
                else {
                    hasAllAPI = false;
                }
                if(version.has(DataManager.APIVersion.MINOR)) {
                    try {
                        minor = version.getInt(DataManager.APIVersion.MINOR);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
                else {
                    hasAllAPI = false;
                }
                if(version.has(DataManager.APIVersion.PATCH)) {
                    try {
                        patch = version.getInt(DataManager.APIVersion.PATCH);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
                else {
                    hasAllAPI = false;
                }

                if(hasAllAPI == false) {
                    return "Unable to validate the Unite This Ctiy API version. Please restart or update if problem persists.";
                }

                Logger.info(mName, String.valueOf(major) + "." + String.valueOf(minor) + "." + String.valueOf(patch));
                DataManager.getInstance().setAPIVersion(major, minor, patch);

                // check for valid version response
                if(DataManager.getInstance().getAPIVersion() != null) {
                    // check major version number
                    if(DataManager.getInstance().getAPIVersion().versionCheck(UTCWebAPI.API_MAJOR_VERSION, UTCWebAPI.API_MAJOR_VERSION)) {
                        return LocationParser.setLocations();
                    }
                    else {
                        return "API incompatibility. Please update the Unite This City application.";
                    }
                }
            }
            else {
                return "Unable to connect to the Unite This City server.";
            }
            
            return "Unknown error when setting up Unite This City";
        }  
   
        @Override  
        protected void onProgressUpdate(Integer... values)  
        {  
        	// not used (for now?)
        }
  
        @Override  
        protected void onPostExecute(String result)  
        {
            if(result == null) {
                if(LoginManager.getInstance().userLoggedIn()) {
                    startMainActivity();
                }
                else {
                    startSignupActivity();
                }
            }
            else {
            	// go ahead and complete launch if locations failed since we were able to 
            	// complete API version call
            	if(result.equals(LocationParser.FAILED)) {
                    if(LoginManager.getInstance().userLoggedIn()) {
                        startMainActivity();
                    }
                    else {
                        startSignupActivity();
                    }
                }
                else {
                    startErrorActivity(result);
                }
            }
            finish();
        }
        
        @Override
        public void onDismiss(DialogInterface dialog) {
            this.cancel(true);
        }
    }
    
    @Override
    public void onBackPressed() {
        super.onBackPressed();
        task.cancel(true);
        finish();
    }
    
    private void startMainActivity() {
    	Intent intent = new Intent(UniteThisCity.this, MainActivity.class);
    	intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
    	UniteThisCity.this.startActivity(intent);
    }

    private void startSignupActivity() {
        Intent intent = new Intent(UniteThisCity.this, SignupActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        UniteThisCity.this.startActivity(intent);
    }

    private void startErrorActivity(String error) {
        Intent intent = new Intent(UniteThisCity.this, ErrorActivity.class);
        intent.putExtra(ErrorActivity.ARG_ERROR, error);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        UniteThisCity.this.startActivity(intent);
    }
}  