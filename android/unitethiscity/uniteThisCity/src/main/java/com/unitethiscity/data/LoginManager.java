package com.unitethiscity.data;

import java.io.IOException;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.R;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;
import com.unitethiscity.ui.MainActivity;
import com.unitethiscity.ui.SignupActivity;

import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.os.AsyncTask;
import android.os.SystemClock;
import android.widget.ImageView;
import android.widget.Toast;

public class LoginManager {
	
	private static String mName = Constants.LOGIN_MANAGER;
	
	private static LoginManager mInstance;
	
    public static final String 	JSON_TAG_BUSINESS_ROLES 				= "BusinessRoles";
    public static final String 	JSON_TAG_CHARITY_ROLES 					= "CharityRoles";
    public static final String 	JSON_TAG_ASSOCIATE_ROLES 				= "AssociateRoles";
    public static final String 	JSON_TAG_ACCOUNT_GUID 					= "AccGuid";
    public static final String 	JSON_TAG_TOKEN 							= "Token";
    public static final String 	JSON_TAG_ACCOUNT_EMAIL 					= "AccEMail";
    public static final String 	JSON_TAG_ACCOUNT_FIRST_NAME 			= "AccFName";
    public static final String 	JSON_TAG_ACCOUNT_LAST_NAME 				= "AccLName";
    public static final String 	JSON_TAG_IS_ADMIN 						= "IsAdmin";
    public static final String 	JSON_TAG_IS_SALES_REP 					= "IsSalesRep";
    public static final String 	JSON_TAG_IS_MEMBER 						= "IsMember";
    public static final String 	JSON_TAG_CITID 							= "CitId";
    public static final String	JSON_TAG_ACCOUNT_ID						= "AccId";
	
    private MainActivity mMain;
	private SignupActivity mSignup;
    
	private AsyncTask<Void, Void, Boolean> mLoginTask;
	private AsyncTask<Void, Void, Boolean> mLogoutTask;
	private AsyncTask<String, Void, ImageView> mAddImageTask;
	
	private String mLoginError;
	private String mLogoutError;

	private boolean mFacebookLogin;
	private boolean mEmailSigninFromSignup;
	private boolean mFacebookSigninFromSignin;

	private String mUsername;
	private String mPassword;
	private String mUserId;
	private boolean mLoggedIn;
	private AccountContext mAccountContext;
	private FacebookContext mFacebookContext;
	
	private Bitmap mAvatar;
	
	private LoginManager() {
		mLoginError = null;
		mLoggedIn = false;
		mEmailSigninFromSignup = false;
		mFacebookSigninFromSignin = false;
		mAccountContext = new AccountContext();
		mFacebookContext = new FacebookContext();
		mAvatar = null;
		mLoginTask = new LoginTask();
		mLogoutTask = new LogoutTask();
		mAddImageTask = new AddImageTask();
	}
	
	public static LoginManager getInstance() {
		if(mInstance == null) {
			mInstance = new LoginManager();
		}
		return mInstance;
	}
	
	public static void destroyInstance() {
		mInstance = null;
	}
	
	public void cleanup() {
		mLoginError = null;
		mLoggedIn = false;
		mAccountContext = null;
		mFacebookContext = null;
		mAvatar = null;
		mLoginTask = null;
		mLogoutTask = null;
		mAddImageTask = null;
	}
	
	public void setMainContext(MainActivity main) {
		mMain = main;
	}

	public boolean userLoggedIn() {
		return mLoggedIn;
	}
	
	public void setAccountContext(AccountContext ac, boolean assumeLoggedIn) {
		mAccountContext = ac;
		mLoggedIn = assumeLoggedIn;
	}

	public AccountContext getAccountContext() {
		return mAccountContext;
	}

	public void setFacebookContext(FacebookContext fc) {
		mFacebookContext = fc;
	}

	public FacebookContext getFacebookContext() {
		return mFacebookContext;
	}

	public Bitmap getAvatar() {
		return mAvatar;
	}
	
	public void cancelAllTasks() {
		cancelLoginTask();
		cancelLogoutTask();

    	if(mMain != null) {
    		mMain.hideSpinner();
    	}
	}
	
	public void login(MainActivity main, String username, String password) {
		mMain = main;
		
		mUsername = username;
		mPassword = password;
		mFacebookLogin = false;
		mEmailSigninFromSignup = false;

    	if(main != null) {
    		main.showSpinner();
    	}
		
		mLoginTask = new LoginTask();
		if(Utils.hasHoneycomb()) {
			mLoginTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mLoginTask.execute();
		}
	}

	public void login(SignupActivity signup, String username, String password) {
		mSignup = signup;

		mUsername = username;
		mPassword = password;
		mFacebookLogin = false;
		mEmailSigninFromSignup = true;

		LoginManager.getInstance().getAccountContext().setFacebook(false);

		mLoginTask = new LoginTask();
		if(Utils.hasHoneycomb()) {
			mLoginTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mLoginTask.execute();
		}
	}

	public void loginFacebook(SignupActivity signup, String username, String id, boolean fromSignIn) {
		mSignup = signup;

		mUsername = username;
		mUserId = id;
		mFacebookLogin = true;
		mFacebookSigninFromSignin = fromSignIn;

		LoginManager.getInstance().getAccountContext().setFacebook(true);

		mLoginTask = new LoginTask();
		if(Utils.hasHoneycomb()) {
			mLoginTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mLoginTask.execute();
		}
	}

	public void loginFacebook(MainActivity main, String username, String id, boolean fromSignIn) {
		mMain = main;

		mUsername = username;
		mUserId = id;
		mFacebookLogin = true;
		mFacebookSigninFromSignin = fromSignIn;

		mLoginTask = new LoginTask();
		if(Utils.hasHoneycomb()) {
			mLoginTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mLoginTask.execute();
		}
	}

	public void logout(MainActivity main) {
		mMain = main;
		
    	if(main != null) {
    		main.showSpinner();
    	}
		
		mLogoutTask = new LogoutTask();
		if(Utils.hasHoneycomb()) {
			mLogoutTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mLogoutTask.execute();
		}
	}
	
    private class LoginTask extends AsyncTask<Void, Void, Boolean> {
        protected Boolean doInBackground(Void... params) {
        	mLoggedIn = false;
        	
    		JSONObject json = null;

			if(mFacebookLogin) {
				json = UTCWebAPI.loginFacebook(mUsername, mUserId);
			}
			else {
				json = UTCWebAPI.login(mUsername, mPassword);
			}

    		// parse user information here
            // if JSON request didn't work, bail
    		if(json == null) {
    			mLoginError = "Unable to login";
    			return Boolean.valueOf(false);
    		}
    		
    		if(json.has("Message")) {		
            	try {
					mLoginError = json.getString("Message");
				} catch (JSONException e) {
					e.printStackTrace();
				}
    			return Boolean.valueOf(false);
    		}
    		
            if(!json.has(JSON_TAG_ACCOUNT_ID)) {
            	mLoginError = "A login error occurred";
    			return Boolean.valueOf(false);
            }
            
            try {
				mAccountContext.setAccountID(json.getInt(JSON_TAG_ACCOUNT_ID));
				Logger.info(mName, "account ID " + String.valueOf(mAccountContext.getAccountID()));
			} catch (JSONException e) {
				e.printStackTrace();
			}
            
            JSONArray roles = null;
            try {
				roles = json.getJSONArray(JSON_TAG_BUSINESS_ROLES);
			} catch (JSONException e) {
				e.printStackTrace();
			}
            
            mAccountContext.clearBusinessRoles();
            if(roles != null) {
            	for(int i = 0; i < roles.length(); i++) {
            		try {
						mAccountContext.addBusinessRole(roles.getInt(i));
					} catch (JSONException e) {
						e.printStackTrace();
					}
            	}
            }
            
            roles = null;
            try {
				roles = json.getJSONArray(JSON_TAG_CHARITY_ROLES);
			} catch (JSONException e) {
				e.printStackTrace();
			}
            
            mAccountContext.clearCharityRoles();
            if(roles != null) {
            	for(int i = 0; i < roles.length(); i++) {
            		try {
						mAccountContext.addCharityRole(roles.getInt(i));
					} catch (JSONException e) {
						e.printStackTrace();
					}
            	}
            }
            
            roles = null;
            try {
				roles = json.getJSONArray(JSON_TAG_ASSOCIATE_ROLES);
			} catch (JSONException e) {
				e.printStackTrace();
			}
            
            mAccountContext.clearAssociateRoles();
            if(roles != null) {
            	for(int i = 0; i < roles.length(); i++) {
            		try {
						mAccountContext.addAssociateRole(roles.getInt(i));
					} catch (JSONException e) {
						e.printStackTrace();
					}
            	}
            }
            
            if(json.has(JSON_TAG_CITID)) {
                try {
                	mAccountContext.setCityID(json.getInt(JSON_TAG_CITID));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }

            if(json.has(JSON_TAG_ACCOUNT_GUID)) {
                try {
                	mAccountContext.setAccountGUID(json.getString(JSON_TAG_ACCOUNT_GUID));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_TOKEN)) {
                try {
                	mAccountContext.setToken(json.getString(JSON_TAG_TOKEN));
                	Logger.info(mName, "account token " + String.valueOf(mAccountContext.getToken()));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_ACCOUNT_EMAIL)) {
                try {
                	mAccountContext.setAccountEmail(json.getString(JSON_TAG_ACCOUNT_EMAIL));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_ACCOUNT_FIRST_NAME)) {
                try {
                	mAccountContext.setAccountFirstName(json.getString(JSON_TAG_ACCOUNT_FIRST_NAME));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_ACCOUNT_LAST_NAME)) {
                try {
                	mAccountContext.setAccountLastName(json.getString(JSON_TAG_ACCOUNT_LAST_NAME));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_IS_ADMIN)) {
                try {
                	mAccountContext.setAdminStatus(json.getBoolean(JSON_TAG_IS_ADMIN));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_IS_SALES_REP)) {
                try {
                	mAccountContext.setSalesRepStatus(json.getBoolean(JSON_TAG_IS_SALES_REP));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
            if(json.has(JSON_TAG_IS_MEMBER)) {
                try {
                	mAccountContext.setMemberStatus(json.getBoolean(JSON_TAG_IS_MEMBER));
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
            }
            
    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
        	if(success.booleanValue()) {
        		mLoggedIn = true;
				SharedPreferences settings;
				if(mFacebookLogin && mSignup != null) {
					settings = mSignup.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
					settings.edit().putString(Constants.SHARED_PREFERENCES_ACCTOKEN,
							LoginManager.getInstance().getAccountContext().getToken())
							.apply();
					try {
						Utils.save(mSignup, Constants.ACCOUNT_CONTEXT, mAccountContext);
					} catch (IOException e) {
						e.printStackTrace();
					}
				}
				else {
					if (mEmailSigninFromSignup && mSignup != null) {
						settings = mSignup.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
						settings.edit().putString(Constants.SHARED_PREFERENCES_ACCTOKEN,
								LoginManager.getInstance().getAccountContext().getToken())
								.apply();
						try {
							Utils.save(mSignup, Constants.ACCOUNT_CONTEXT, mAccountContext);
						} catch (IOException e) {
							e.printStackTrace();
						}
					}
					else {
						settings = mMain.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
						settings.edit().putString(Constants.SHARED_PREFERENCES_ACCTOKEN,
								LoginManager.getInstance().getAccountContext().getToken())
								.apply();
						try {
							Utils.save(mMain, Constants.ACCOUNT_CONTEXT, mAccountContext);
						} catch (IOException e) {
							e.printStackTrace();
						}
					}
				}

        		// force location update by clearing data revision
        		DataManager.getInstance().clearLocationInfoDataRevision();
        		// force wallet update (we just logged in)
        		DataManager.getInstance().setWalletNeedsUpdate(true);
				if(DataManager.getInstance().getAccountFragment() != null &&
						DataManager.getInstance().getAccountFragment().isFragmentActive()) {
					// execute account fragment update
					DataManager.getInstance().getAccountFragment().loggedIn();
				}
        		// wipe potential previous messages from previous account login
        		DataManager.getInstance().clearMessages();
				// force an inbox update (we just logged in)
				DataManager.getInstance().setMessagesTimestamp(SystemClock.elapsedRealtime() 
						- Constants.MESSAGES_UPDATE_DWELL);

				if(mFacebookLogin) {
					if(mFacebookSigninFromSignin && mSignup != null) {
						mSignup.callbackLoginSuccessFromSignin();
					}
					else if (mSignup != null) {
						mSignup.callbackLoginSuccess();
					}
					else {
						refreshAvatarAsFacebook(LoginManager.getInstance().getFacebookContext().getUserID());

						DataManager.getInstance().getMainFragment().switchAccountImage();

						mMain.registerWithGCM();

						mMain.callbackLoginSuccessFromSignin();
					}
				}
				else {
					if(mEmailSigninFromSignup) {
						mSignup.callbackLoginSuccess();
					}
					else {
						if(LoginManager.getInstance().getAccountContext().getFacebook()) {
							refreshAvatarAsFacebook(LoginManager.getInstance().getFacebookContext().getUserID());
						}
						else {
							refreshAvatar();
						}

						DataManager.getInstance().getMainFragment().switchAccountImage();

						mMain.registerWithGCM();

						if (mFacebookContext.getUserID() != null) {
							mMain.facebookLogin(false, false);
						}
					}
				}
        	}
        	else {
				if(mFacebookLogin && mFacebookSigninFromSignin == false) {
					if(mLoginError.equals("Unable to login") && mSignup != null) {
						Toast.makeText(mSignup, mLoginError, Toast.LENGTH_SHORT).show();
					}
					else if(mSignup != null) {
						mSignup.callbackLoginFailure();
					}
					else if(mLoginError.equals("Unable to login") && mMain != null) {
						Toast.makeText(mMain, mLoginError, Toast.LENGTH_SHORT).show();
					}
				}
				else {
					if (mMain != null) {
						Toast.makeText(mMain, mLoginError, Toast.LENGTH_SHORT).show();
					}
					else if (mSignup != null) {
						Toast.makeText(mSignup, mLoginError, Toast.LENGTH_SHORT).show();
					}
				}
        	}
        	
        	if(mMain != null) {
        		mMain.hideSpinner();
        	}
        }
    }
    
    public boolean cancelLoginTask() {
    	boolean result = false;
    	if(mLoginTask != null) {
    		Logger.info(mName, "LoginTask cancelled");
    		result = mLoginTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoginTask was null when cancelling");
    	}
    	return result;
    }
    
    private class LogoutTask extends AsyncTask<Void, Void, Boolean> {
        protected Boolean doInBackground(Void... params) {
    		JSONObject json = null;

			// don't care about GCM at the moment
    		/* while(DataManager.getInstance().getUnregistering() &&
    				 (SystemClock.elapsedRealtime() - DataManager.getInstance().getUnregisterTimestamp() < Constants.GCM_UNREGISTER_TIMEOUT));
    		
    		if(SystemClock.elapsedRealtime() - DataManager.getInstance().getUnregisterTimestamp() >= Constants.GCM_UNREGISTER_TIMEOUT) {
    			Logger.warn(mName, "GCM unregister timeout");
    		} */
    		
    		json = UTCWebAPI.logout(
    				mAccountContext.getBusinessRoles(),
    				mAccountContext.getCharityRoles(), 
    				mAccountContext.getCharityRoles(), 
    				mAccountContext.getAccountID().intValue(), 
    				mAccountContext.getCityID(), 
    				mAccountContext.getAccountGUID(), 
    				mAccountContext.getToken(), 
    				mAccountContext.getAccountEmail(), 
    				mAccountContext.getAccountFirstName(), 
    				mAccountContext.getAccountLastName(), 
    				mAccountContext.getAdminStatus(), 
    				mAccountContext.getSalesRepStatus(), 
    				mAccountContext.getMemberStatus());
    		
    		if(json != null) {
        		if(json.has("Message")) {		
                	try {
                		mLogoutError = json.getString("Message");
    				} catch (JSONException e) {
    					e.printStackTrace();
    				}
        		}
        		return Boolean.valueOf(false);
    		}
    		
    		SharedPreferences settings = mMain.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
    		settings.edit().remove(Constants.SHARED_PREFERENCES_ACCTOKEN).commit();
            
    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
			mLoggedIn = false;
			SharedPreferences settings = mMain.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
			settings.edit().putString(Constants.SHARED_PREFERENCES_ACCTOKEN,
					"")
					.commit();
			// remove account information from disk
			Utils.delete(mMain, Constants.ACCOUNT_CONTEXT);
			// clear the account context
			mAccountContext = new AccountContext();
			// set update flags and clear information related to user
			DataManager.getInstance().setLocationsNeedUpdated(true);
			DataManager.getInstance().setWalletNeedsUpdate(true);
			DataManager.getInstance().clearFavorites();
			DataManager.getInstance().clearMessages();
			// perform account fragment specific UI updates
			DataManager.getInstance().getAccountFragment().loggedOut();
			// update avatar with mystery man image
			ImageView iv = (ImageView) mMain.findViewById(R.id.imageViewUser);
			iv.setImageDrawable(mMain.getResources().getDrawable(R.drawable.mystery_man));
			iv = (ImageView) mMain.findViewById(R.id.imageViewUserPicture);
			iv.setImageDrawable(mMain.getResources().getDrawable(R.drawable.mystery_man_fancy));

			DataManager.getInstance().getMainFragment().switchAccountImage();

			mMain.facebookLogout();

        	if(!success.booleanValue()) {
        		if(mMain != null) {
        			Toast.makeText(mMain, mLogoutError, Toast.LENGTH_SHORT).show();
        		}
        	}

        	if(mMain != null) {
        		mMain.hideSpinner();
        	}
        }
    }
    
    public boolean cancelLogoutTask() {
    	boolean result = false;
    	if(mLogoutTask != null) {
    		Logger.info(mName, "LogoutTask cancelled");
    		result = mLogoutTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LogoutTask was null when cancelling");
    	}
    	return result;
    }
    
    public void refreshAvatar() {
    	if(mAccountContext != null && mAccountContext.getAccountEmail() != null)
    	{
			String gravatarURL = Constants.GRAVATAR_AVATAR_URL +
					Utils.md5(mAccountContext.getAccountEmail().trim().toLowerCase()) +
					"?d=mm&s=200";
			
			mAddImageTask = new AddImageTask();
			if(Utils.hasHoneycomb()) {
				mAddImageTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, gravatarURL);
			}
			else {
				mAddImageTask.execute(gravatarURL);
			}
    	}
    }
    
    public void refreshAvatarAsFacebook(String facebookID) {
    	String facebookURL = Constants.FACEBOOK_AVARTAR_URL + 
    			facebookID + 
    			"/picture?width=150";
		
		mAddImageTask = new AddImageTask();
		if(Utils.hasHoneycomb()) {
			mAddImageTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, facebookURL);
		}
		else {
			mAddImageTask.execute(facebookURL);
		}
    }
    
	private class AddImageTask extends AsyncTask<String, Void, ImageView> {
        protected ImageView doInBackground(String... url) {
			ImageView img = null;
			if(mMain != null) {
				img = new ImageView(mMain);
				mMain.getImageDownloader().download(url[0], img);
			}
        	
        	return img;
        }
        
        protected void onPostExecute(ImageView img) {
        	if(img != null) {
        		mAvatar = ((BitmapDrawable)img.getDrawable()).getBitmap();
        		ImageView iv = (ImageView) mMain.findViewById(R.id.imageViewUser);
        		if(iv != null) {
        			iv.setImageBitmap(mAvatar);
        		}
        		iv = (ImageView) mMain.findViewById(R.id.imageViewUserPicture);
        		if(iv != null) {
        			iv.setImageBitmap(mAvatar);
        		}
        	}
        }
    }
    
    public boolean cancelAddImageTask() {
    	boolean result = false;
    	if(mAddImageTask != null) {
    		result = mAddImageTask.cancel(true);
    	}
    	return result;
    }
}
