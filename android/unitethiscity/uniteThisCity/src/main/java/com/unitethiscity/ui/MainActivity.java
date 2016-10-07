package com.unitethiscity.ui;

import java.io.File;
import java.io.IOException;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

//import twitter4j.Twitter;
//import twitter4j.TwitterException;
//import twitter4j.TwitterFactory;
//import twitter4j.User;

import android.Manifest;
import android.app.ActivityManager;
import android.app.ActivityManager.RunningServiceInfo;
import android.app.AlertDialog;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.SystemClock;
import android.os.Vibrator;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.TaskStackBuilder;
import android.support.v4.content.ContextCompat;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.Toast;

import com.facebook.*;
import com.facebook.login.LoginResult;
import com.facebook.share.Sharer;
import com.facebook.share.model.ShareLinkContent;
import com.facebook.share.widget.ShareDialog;
import com.google.android.gms.analytics.HitBuilders;
import com.google.android.gms.analytics.Tracker;
import com.unitethiscity.R;
import com.unitethiscity.data.AccountContext;
import com.unitethiscity.data.AnalyticsApplication;
import com.unitethiscity.data.CategoryAntifilters;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.FacebookContext;
import com.unitethiscity.data.ImageDownloader;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.ProximityLocationService;
import com.unitethiscity.data.UTCGeolocationManager;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class MainActivity extends FragmentActivity implements SignInDialog.SignInDialogListener {

	private String mName = Constants.MAIN_ACTIVITY;

	public final static String EXTRA_NAVIGATION = "com.unitethiscity.ui.MainActivity.NAVIGATION";
	public final static String EXTRA_SIGNIN = "com.unitethiscity.ui.MainActivity.SIGNIN";
	public final static String EXTRA_FACEBOOK_SIGNIN = "com.unitethiscity.ui.MainActivity.FACEBOOK_SIGNIN";

	private Tracker mTracker;

	private long mTimestamp;
	
	private boolean mInitialized;
	private boolean mBackPressed;
	private boolean mMenuChanged;

	private Constants.MenuID mCurrentFragmentID = Constants.MenuID.HOME;
	private Constants.MenuID mParentOfCurrentFragment;
	private Constants.MenuID mCurrentMainMenu = Constants.MenuID.HOME;

	private Constants.MenuID mNavigationRequest;
	private boolean mNavigateToSignin;
	private boolean mFacebookSignin;

	private boolean mGoToAccount = false;
	
	private ImageDownloader mImageDownloader;
	
	private UTCDialogFragment mUTCDialog;
	
	// Facebook-related
	CallbackManager callbackManager;
	ShareDialog shareDialog;
	public static final List<String> FB_READ_PERMISSIONS = Arrays.asList("public_profile", "email", "user_friends");
	private static final List<String> PERMISSIONS = Arrays.asList("publish_actions", "publish_stream");
	private static final String PENDING_PUBLISH_KEY = "pendingPublishReauthorization";
	private boolean pendingPublishReauthorization = false;
	private String mFacebookName;
	private String mFacebookCaption;
	private String mFacebookDescription;
	private String mFacebookLink;
	private String mFacebookPicture;
	private String mFacebookMessage;
	private String mFacebookUserID;

	private static final String FACEBOOK_ERROR_DIFFERENT_USER = "User logged in as different Facebook user.";
	private static final String FACEBOOK_DEFAULT_GENDER = "?";
	private static final String FACEBOOK_DEFAULT_ZIP = "00000";
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
	
	private int mSocialBusinessID;
	
	private AsyncTask<Integer, Void, Boolean> mSocialPostTask;
	
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
									mFirstName = null;
									mLastName = null;
									mEmail = null;
									mGender = null;
									mBirthday = null;
									// always default the zip code
									mZipCode = FACEBOOK_DEFAULT_ZIP;
									mUserID = thisResult.getAccessToken().getUserId();
									mAllInfoPresent = true;

									LoginManager.getInstance().refreshAvatarAsFacebook(mUserID);

									if(object.has("first_name")) {
										mFirstName = object.getString("first_name");
									}
									else {
										mAllInfoPresent = false;
									}

									if(object.has("last_name")) {
										mLastName = object.getString("last_name");
									}
									else {
										mAllInfoPresent = false;
									}

									if(object.has("email")) {
										mEmail = object.getString("email");
									}
									else {
										mAllInfoPresent = false;
									}

									if(object.has("gender")) {
										mGender = object.getString("gender");
									}
									else {
										// gender not required, default it
										mGender = FACEBOOK_DEFAULT_GENDER;
									}

									if(object.has("birthday")) {
										mBirthday = object.getString("birthday");
									}
									else {
										mAllInfoPresent = false;
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
									LoginManager.getInstance().loginFacebook(MainActivity.this, mEmail, mUserID, mFacebookLoginFromSignin);
								} catch (JSONException e) {
									e.printStackTrace();
								}
							}
							else {
								Toast.makeText(MainActivity.this, "Facebook error: " + err.getErrorMessage(), Toast.LENGTH_SHORT).show();
							}

							try {
								Utils.save(MainActivity.this, Constants.ACCOUNT_CONTEXT, LoginManager.getInstance().getAccountContext());
							} catch (IOException e) {
								e.printStackTrace();
							}

							LoginManager.getInstance().getAccountContext();
			            }
			});
			
			Bundle parameters = new Bundle();
			parameters.putString("fields", "first_name,last_name");
			request.setParameters(parameters);
			request.executeAsync();
			
			hideSpinner();
		}

		@Override
		public void onCancel() {
			
		}

		@Override
		public void onError(FacebookException error) {
			Toast.makeText(MainActivity.this, "Failed to login to Facebook", Toast.LENGTH_SHORT).show();
			LoginManager.getInstance().getAccountContext().setFacebook(false);
			try {
				Utils.save(MainActivity.this, Constants.ACCOUNT_CONTEXT, LoginManager.getInstance().getAccountContext());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		
	};
	
//	@SuppressLint("NewApi")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Logger.info(mName, "onCreate()");
		
		// print key hash used for Facebook
		/*
		try {
			android.content.pm.PackageInfo info = getPackageManager().getPackageInfo(
					"com.unitethiscity", 
					android.content.pm.PackageManager.GET_SIGNATURES);
			for (android.content.pm.Signature signature : info.signatures) {
				java.security.MessageDigest md = java.security.MessageDigest.getInstance("SHA");
				md.update(signature.toByteArray());
				if(Utils.hasFroyo()) {
					Logger.debug(mName, "KeyHash: " + android.util.Base64.encodeToString(md.digest(), android.util.Base64.DEFAULT));
				}
			}
		} catch (Exception e) {

		}
		*/

		// Obtain the shared Tracker instance.
		AnalyticsApplication application = new AnalyticsApplication();
		mTracker = application.getDefaultTracker(getApplication());

		FacebookSdk.sdkInitialize(getApplicationContext());
		callbackManager = CallbackManager.Factory.create();
		com.facebook.login.LoginManager.getInstance().registerCallback(callbackManager, fbCallback);
		shareDialog = new ShareDialog(this);
		
		getWindow().requestFeature(Window.FEATURE_PROGRESS);
		setContentView(R.layout.activity_main);
		
		DataManager.getInstance().initializeFragments();
		DataManager.getInstance().setMainActivityContext(this);

		Intent intent = getIntent();
		if (intent.hasExtra(EXTRA_NAVIGATION)) {
			mNavigationRequest = Constants.MenuID.fromValue(intent.getIntExtra(EXTRA_NAVIGATION, Constants.MenuID.HOME.getValue()));
		}
		else {
			mNavigationRequest = Constants.MenuID.EMPTY;
		}
		if (intent.hasExtra(EXTRA_SIGNIN)) {
			mNavigateToSignin = true;
		}
		else {
			mNavigateToSignin = false;
		}
		if (intent.hasExtra(EXTRA_FACEBOOK_SIGNIN)) {
			mFacebookSignin = true;
		}
		else {
			mFacebookSignin = false;
		}

        if (findViewById(R.id.relativeLayoutMain) != null) {

        	// Originally, if there was a valid saved instance state this function would 
        	// return here, as it was assumed the had already been running.
        	// This could be the cause of the NPE data manager issues upon navigation 
        	// back to the app. Investigation shows that Android will kill processes when 
        	// needed but will attempt to bring the app back to it's previous state (or 
        	// as close as possible). This creates a valid saved instance state, but 
        	// everything else is trashed.

            if(savedInstanceState == null) {
                getSupportFragmentManager().beginTransaction().add(R.id.frameLayoutMiddle, 
                		DataManager.getInstance().getMainFragment(), 
                		mCurrentFragmentID.toString()).commit();
            }
            else {
            	Logger.info(mName, "resuming from kill");
            	
            	int savedFragmentID = savedInstanceState.getInt(Constants.SAVED_INSTANCE_FRAGMENT, 
            			Constants.MenuID.HOME.getValue()) + 2;
            	int savedParentFragmentID = savedInstanceState.getInt(Constants.SAVED_INSTANCE_FRAGMENT_PARENT, 
            			Constants.MenuID.HOME.getValue());
            	
            	if(savedFragmentID < Constants.MenuID.values().length && savedFragmentID >= 0) {
            		mCurrentFragmentID = Constants.MenuID.values()[savedFragmentID];
            	}
            	else {
            		mCurrentFragmentID = Constants.MenuID.HOME;
            	}
            	
            	// ensure we have a valid location manager again
            	UTCGeolocationManager.getInstance().initializeLocationManager(this.getApplicationContext());
            	
            	if(Utils.menuType(mCurrentFragmentID) == Constants.MenuType.MAIN) {
            		Logger.info(mName, "main menu type");
            		replaceFragment(mCurrentFragmentID, true);
            	}
            	else {
            		Logger.info(mName, "sub menu type - " + mCurrentFragmentID.toString());
            		
            		getSupportFragmentManager()
            			.beginTransaction()
            			.remove(getSupportFragmentManager()
            					.findFragmentByTag(mCurrentFragmentID.toString()))
            					.commit();
            		
            		switch (mCurrentFragmentID) {
            		case SEARCH_BUSINESS:
            			replaceFragment(mCurrentFragmentID, true);
            			setActiveMenuImage(Constants.MenuID.SEARCH.getValue());
            			break;
            		case SEARCH_EVENT:
            			replaceFragment(mCurrentFragmentID, true);
            			setActiveMenuImage(Constants.MenuID.SEARCH.getValue());
            			break;
            		case SEARCH_CATEGORIES:
            			replaceFragment(mCurrentFragmentID, true);
            			setActiveMenuImage(Constants.MenuID.SEARCH.getValue());
            			break;
            		case REDEEM:
            			replaceFragment(Constants.MenuID.fromValue(savedParentFragmentID), true);
            			if(Constants.MenuID.fromValue(savedParentFragmentID) == Constants.MenuID.SEARCH_BUSINESS) {
            				setActiveMenuImage(Constants.MenuID.SEARCH.getValue());
            			}
            			break;
            		case EVENT:
            			replaceFragment(Constants.MenuID.SEARCH_EVENT, true);
            			setActiveMenuImage(Constants.MenuID.SEARCH.getValue());
            			break;
            		case WEB:
            			if(Constants.MenuID.fromValue(savedParentFragmentID) == Constants.MenuID.REDEEM) {
                    		getSupportFragmentManager()
                			.beginTransaction()
                			.remove(getSupportFragmentManager()
                					.findFragmentByTag(Constants.MenuID.REDEEM.toString()))
                					.commit();
            				replaceFragment(Constants.MenuID.UTC, true);
            			}
            			else {
            				replaceFragment(Constants.MenuID.fromValue(savedParentFragmentID), true);
            			}
            			break;
            		case BUSINESS:
            			replaceFragment(mCurrentFragmentID, true);
            			break;
            		case SUBSCRIBE:
            			replaceFragment(mCurrentFragmentID, true);
            			break;
					case STATISTICS_SUMMARY:
						replaceFragment(Constants.MenuID.BUSINESS, true);
						break;
					case STATISTICS_LIST:
						replaceFragment(Constants.MenuID.BUSINESS, true);
						break;
					case ANALYTICS_SUMMARY:
						replaceFragment(Constants.MenuID.BUSINESS, true);
						break;
					case ANALYTICS_BUSINESS:
						getSupportFragmentManager()
								.beginTransaction()
								.remove(getSupportFragmentManager()
										.findFragmentByTag(Constants.MenuID.STATISTICS_SUMMARY.toString()))
								.commit();
						replaceFragment(Constants.MenuID.BUSINESS, true);
						break;
            		}
            	}
            }
        }
        
        mTimestamp = SystemClock.elapsedRealtime() - Constants.FRAGMENT_SWITCH_DWELL;

        mInitialized = false;
        mBackPressed = false;
        mMenuChanged = false;
        
        mImageDownloader = new ImageDownloader();

        ImageView imageView = (ImageView) findViewById(R.id.imageViewWallet);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.WALLET, false);
				}
				
				return false;
			}
		});
        
        imageView = (ImageView) findViewById(R.id.imageViewFavorite);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.FAVORITE, false);
				}
				
				return false;
			}
		});
        
        imageView = (ImageView) findViewById(R.id.imageViewUTC);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.UTC, false);
					DataManager.getInstance().setFromBigButton(false);
				}
				
				return false;
			}
		});
		
        imageView = (ImageView) findViewById(R.id.imageViewInbox);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.INBOX, false);
				}
				
				return false;
			}
		});
        
        imageView = (ImageView) findViewById(R.id.imageViewSearch);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.SEARCH, false);
				}
				
				return false;
			}
		});
        
        imageView = (ImageView) findViewById(R.id.imageViewUser);
        imageView.setOnTouchListener(new View.OnTouchListener() {
			
			@Override
			public boolean onTouch(View v, MotionEvent event) {
				if(event.getAction() == (MotionEvent.ACTION_UP)) {
					replaceFragment(Constants.MenuID.ACCOUNT, false);
				}
				
				return false;
			}
		});
        
        // check if account context has been previously saved, and attempt to 
        // deserialize it into our account context within the login manager
        if(Utils.saveExists(this, Constants.ACCOUNT_CONTEXT)) {
        	try {
				Object obj = Utils.open(this, Constants.ACCOUNT_CONTEXT);
				AccountContext ac = (AccountContext) obj;
				if(ac != null) {
					LoginManager.getInstance().setAccountContext((AccountContext) obj, true);
				}
				else {
					LoginManager.getInstance().setAccountContext(new AccountContext(), false);
					mGoToAccount = true;
				}
			} catch (IOException e) {
				e.printStackTrace();
			} catch (ClassNotFoundException e) {
				e.printStackTrace();
			}
        	
        	// basic check for valid data from account context restoration
        	if(LoginManager.getInstance().getAccountContext().getAccountFirstName() == null || 
        			LoginManager.getInstance().getAccountContext().getAccountLastName() == null) {
        		// reset account context if we have any null names
        		LoginManager.getInstance().setAccountContext(new AccountContext(), false);
        		mGoToAccount = true;
        	}
        	else {
        		/* use this to inspect account and push tokens
                SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
                String accountToken = settings.getString(Constants.SHARED_PREFERENCES_ACCTOKEN, "");
                String registrationID = settings.getString(Constants.GCM_REGISTRATION_ID, "");
        		Logger.info(mName, "account token: " + 
        				LoginManager.getInstance().getAccountContext().getToken() + 
        				", push token: " + registrationID);
        		*/
        		
        		// start the proximity service if restored account context successfully
        		// and it is not already running and is enabled
                SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
                boolean notificationsEnabled = settings.getBoolean(Constants.SHARED_PREFERENCES_NOTIFICATIONS, true);
        		if(isMyServiceRunning(ProximityLocationService.class) == false && notificationsEnabled) {
        			Intent serviceIntent = new Intent(this, ProximityLocationService.class);
    				startService(serviceIntent);
        		}
        	}
        }
        else {
        	mGoToAccount = true;
        }
        
		if(LoginManager.getInstance().getAccountContext().getFacebook() && savedInstanceState == null) {
			facebookLogin(false, false);
		}
        
        if(LoginManager.getInstance().getAccountContext().getAccountEmail() != null) {
        	LoginManager.getInstance().setMainContext((MainActivity) this);
        	LoginManager.getInstance().refreshAvatar();
        }
        
        // check if category antifilters have been previously saved, and attempt to 
        // deserialize it into our category context within the data manager
//        if(Utils.saveExists(this, Constants.CATEGORY_ANTIFILTERS)) {
//        	try {
//				Object obj = Utils.open(this, Constants.CATEGORY_ANTIFILTERS);
//				DataManager.getInstance().setCategoryAntifilters(
//						(CategoryAntifilters) obj);
//			} catch (IOException e) {
//				e.printStackTrace();
//			} catch (ClassNotFoundException e) {
//				e.printStackTrace();
//			}
//        }
        // disabled for issue 135
        
        DataManager.getInstance().getSearchCategoriesFragment().executeCategoriesTask();

		// show the tutorial if not previously shown
		SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
		boolean tutorialShown = settings.getBoolean(Constants.SHARED_PREFERENCES_TUTORIAL_SHOWN, false);
		if (tutorialShown == false) {
			Intent tutorialIntent = new Intent(MainActivity.this, TutorialActivity.class);
			tutorialIntent.setFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
			MainActivity.this.startActivity(tutorialIntent);
		}
	}
	
	@Override
	public void onRestart() {
		super.onRestart();
		
		// on activity restart, make sure data manager is initialized 
		// by retrieving an instance (forces a new instance if it is 
		// null) and checking the validity of all fragments
		Logger.info(mName, "onRestart - checking fragment validity");
		if(!DataManager.getInstance().fragmentsValid()) {
			Logger.warn(mName, "fragments destroyed, reinitializing");
			// if any fragment is null, reinitialize them
			DataManager.getInstance().initializeFragments();
		}
	}
	
	@Override
	public void onResume() {
		super.onResume();
		Logger.verbose(mName, "onResume()");

		DataManager.getInstance().setMainActivityActive(true);

		if(DataManager.getInstance().getAnalyticsState()) {
			mTracker.setScreenName(mName);
			mTracker.send(new HitBuilders.ScreenViewBuilder().build());
		}
		
		UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
		
		if(mInitialized == false) {
			lm.initializeLocationManager(this.getApplicationContext());
		}

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

		mInitialized = true;

		switch (mCurrentFragmentID) {
		case WALLET:
			DataManager.getInstance().getWalletFragment().fragmentActive(true);
			break;
		case FAVORITE:
			DataManager.getInstance().getFavoriteFragment().fragmentActive(true);
			break;
		case UTC:
			DataManager.getInstance().getUTCFragment().fragmentActive(true);
			break;
		case INBOX:
			DataManager.getInstance().getInboxFragment().fragmentActive(true);
			break;
		case SEARCH:
			DataManager.getInstance().getSearchFragment().fragmentActive(true);
			break;
		case SEARCH_BUSINESS:
			DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
			break;
		case SEARCH_EVENT:
			DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
			break;
		case REDEEM:
			DataManager.getInstance().getRedeemFragment().fragmentActive(true);
			break;
		case EVENT:
			DataManager.getInstance().getEventFragment().fragmentActive(true);
			break;
		case ACCOUNT:
			DataManager.getInstance().getAccountFragment().fragmentActive(true);
			break;
		case WEB:
			DataManager.getInstance().getWebFragment().fragmentActive(true);
			break;
		case BUSINESS:
			DataManager.getInstance().getBusinessFragment().fragmentActive(true);
			break;
		case SUBSCRIBE:
			DataManager.getInstance().getSubscribeFragment().fragmentActive(true);
			break;
		case STATISTICS_SUMMARY:
			DataManager.getInstance().getStatisticsSummaryFragment().fragmentActive(true);
			break;
		case STATISTICS_LIST:
			DataManager.getInstance().getStatisticsListFragment().fragmentActive(true);
			break;
		case ANALYTICS_SUMMARY:
			DataManager.getInstance().getAnalyticsSummaryFragment().fragmentActive(true);
			break;
		case ANALYTICS_BUSINESS:
			DataManager.getInstance().getAnalyticsBusinessFragment().fragmentActive(true);
			break;
		case HOME:
		default:
			DataManager.getInstance().getMainFragment().fragmentActive(true);
			break;
		}
		
    	if(DataManager.getInstance().getAnalyticsState()) {
    		Logger.verbose(mName, "sending view for " + mCurrentFragmentID.getName());
    		sendView(mCurrentFragmentID.getName());
    	}
    	
		int destFromNotify = getIntent().getIntExtra("destFromNotify", -2);
		if(destFromNotify != -2) {
			getIntent().removeExtra("destFromNotify");
			Logger.info(mName, "destFromNotify: " + String.valueOf(destFromNotify));
			Logger.info(mName, "going to " + Constants.MenuID.fromValue(destFromNotify).toString());
			replaceFragment(Constants.MenuID.fromValue(destFromNotify), false);
			DataManager.getInstance().setMenuFromNotification(destFromNotify);
		}
		else {
			Logger.info(mName, "destFromNotify not found");
		}
		
		if(mNavigationRequest != Constants.MenuID.EMPTY) {
			replaceFragment(mNavigationRequest, false);
			mNavigationRequest = Constants.MenuID.EMPTY;
		}
		else if(mGoToAccount) {
			replaceFragment(Constants.MenuID.ACCOUNT, false);
			mGoToAccount = false;
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
					if(lm.isCurrentLocationStale() || mInitialized == false) {
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
					if(lm.isCurrentLocationStale() || mInitialized == false) {
						lm.locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_BOTH);
					}
				}
				else {
					lm.setCoarseAccess(false);
					Toast.makeText(this, "UTC will not be able to determine your location", Toast.LENGTH_SHORT).show();
				}

				return;
			}
			case Constants.PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE_1: {
				// If request is cancelled, the result arrays are empty.
				if (grantResults.length > 0
						&& grantResults[0] == PackageManager.PERMISSION_GRANTED) {
					DataManager.getInstance().getRedeemFragment().permissionResultRedeem(grantResults[0]);
				}
				else {
					Toast.makeText(this, "UTC will not be able to take a picture to share to Instagram", Toast.LENGTH_SHORT).show();
				}
				return;
			}
			case Constants.PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE_2: {
				// If request is cancelled, the result arrays are empty.
				if (grantResults.length > 0
						&& grantResults[0] == PackageManager.PERMISSION_GRANTED) {
					DataManager.getInstance().getRedeemFragment().permissionResultLoyalty(grantResults[0]);
				}
				else {
					Toast.makeText(this, "UTC will not be able to take a picture to share to Instagram", Toast.LENGTH_SHORT).show();
				}
				return;
			}
		}
	}
	
	@Override
	public void onPause() {
		super.onPause();
		Logger.verbose(mName, "onPause()");
		
		DataManager.getInstance().setMainActivityActive(false);
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
		
		UTCGeolocationManager.getInstance().removeLocationUpdates();
		setInactiveAndCancel();
	}
	
	@Override
	public void onDestroy() {
	    super.onDestroy();
	    Logger.verbose(mName, "onDestroy()");
	    
	    DataManager.getInstance().cleanup();
	    DataManager.destroyInstance();
	    
	    LoginManager.getInstance().cleanup();
	    LoginManager.destroyInstance();
	    
	    UTCGeolocationManager.getInstance().cleanup();
	    UTCGeolocationManager.destroyInstance();
	    
		// stop the proximity service
		Intent intent = new Intent(this, ProximityLocationService.class);
		stopService(intent);
	}
	
	@Override
	public void onSaveInstanceState(Bundle savedInstanceState) {
		super.onSaveInstanceState(savedInstanceState);
		Logger.verbose(mName, "onSaveInstanceState()");
		
		savedInstanceState.putInt(Constants.SAVED_INSTANCE_FRAGMENT, mCurrentFragmentID.getValue());
		if(mParentOfCurrentFragment != null) {
			savedInstanceState.putInt(Constants.SAVED_INSTANCE_FRAGMENT_PARENT, mParentOfCurrentFragment.getValue());
		}
		savedInstanceState.putBoolean(PENDING_PUBLISH_KEY, pendingPublishReauthorization);
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		Logger.verbose(mName, "onActivityResult()");

		callbackManager.onActivityResult(requestCode, resultCode, data);
	}

	@Override
	public void onBackPressed()
	{
	    mBackPressed = true;
	    
	    if(DataManager.getInstance().getWebFragment().isFragmentActive() && 
	    		DataManager.getInstance().getWebFragment().canGoBack()) {
	    	DataManager.getInstance().getWebFragment().goBack();
	    	return;
		}

	    FragmentManager fm = getSupportFragmentManager();
	    
	    setInactiveAndCancel();
		
	    // prompt for application exit if we're currently on main screen
	    // otherwise, step back from current screen to main screen
	    if(Constants.MenuID.valueOf(fm.findFragmentById(R.id.frameLayoutMiddle).getTag())
	    		== Constants.MenuID.HOME) {//if(fm.getBackStackEntryCount() == 0) {//mFragmentsReplaced == 0) {
	        new AlertDialog.Builder(this)
	        .setTitle("Quit")
	        .setMessage("Are you sure you want to quit?")
	        .setNegativeButton(android.R.string.no, null)
	        .setPositiveButton(android.R.string.yes, new OnClickListener() {

	            public void onClick(DialogInterface arg0, int arg1) {
	                MainActivity.super.onBackPressed();
	            }
	        }).create().show();
	    }
	    else {
	    	// for now, replace with main - want to handle submenus replaced from 
	    	// main menus to use back stack
	    	if(DataManager.getInstance().menuStackEmpty() == false) {
	    		Logger.info(mName, "onBackPressed, back stack count - " +
						String.valueOf(DataManager.getInstance().menuStackSize()));
	    		
				FragmentTransaction transaction = fm.beginTransaction();
			    Constants.MenuID stackMenuId;
			    DataManager.getInstance().popFromMenuStack();
		    	stackMenuId = DataManager.getInstance().lastMenuPopped();
		    	
		    	Logger.info(mName, "onBackPressed, fID - " + stackMenuId.toString());
			    switch (stackMenuId) {
			    case REDEEM:
			    	DataManager.getInstance().getRedeemFragment().fragmentActive(false);
					transaction.remove(DataManager.getInstance().getRedeemFragment()).commit();
					DataManager.getInstance().getRedeemFragment().clearContents();
					DataManager.getInstance().setLocationsNeedUpdated(true);
					if(DataManager.getInstance().getRedeemFragment().getParent() == Constants.MenuID.UTC) {
						Logger.info(mName, "redeem parent UTC");
						DataManager.getInstance().getUTCFragment().fragmentActive(true);
						DataManager.getInstance().getUTCFragment().show();
				    	if(DataManager.getInstance().needLocalUpdate()) {
				    		DataManager.getInstance().getUTCFragment().removeAllLocationResults();
				    		DataManager.getInstance().getUTCFragment().loadLocations(true);
							DataManager.getInstance().setLocalUpdateNeeded(false);
				    	}
					}
					else if(DataManager.getInstance().getRedeemFragment().getParent() == Constants.MenuID.FAVORITE) {
						Logger.info(mName, "redeem parent Favorite");
						DataManager.getInstance().getFavoriteFragment().fragmentActive(true);
						DataManager.getInstance().getFavoriteFragment().show();
				    	if(DataManager.getInstance().needLocalUpdate()) {
				    		DataManager.getInstance().getFavoriteFragment().removeAllLocationResults();
				    		DataManager.getInstance().getFavoriteFragment().loadLocations();
				    		DataManager.getInstance().setLocalUpdateNeeded(false);
				    	}
					}
					else if(DataManager.getInstance().getRedeemFragment().getParent() == Constants.MenuID.SEARCH_BUSINESS) {
						Logger.info(mName, "redeem parent SearchBusiness");
						DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
						DataManager.getInstance().getSearchBusinessFragment().show();
				    	if(DataManager.getInstance().needLocalUpdate()) {
				    		DataManager.getInstance().getSearchBusinessFragment().removeAllLocationResults();
				    		DataManager.getInstance().getSearchBusinessFragment().loadLocations(true);
				    		DataManager.getInstance().setLocalUpdateNeeded(false);
				    	}
					}
					break;
			    case EVENT:
			    	DataManager.getInstance().getEventFragment().fragmentActive(false);
					if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.REDEEM) {
						DataManager.getInstance().getRedeemFragment().fragmentActive(true);
						transaction.remove(DataManager.getInstance().getEventFragment()).commit();
					}
					else {
						transaction.remove(DataManager.getInstance().getEventFragment()).commit();
						DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
						DataManager.getInstance().getSearchEventFragment().show();
					}
			    	break;
			    case WEB:
			    	DataManager.getInstance().getWebFragment().fragmentActive(false);
			    	if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.INBOX) {
				    	DataManager.getInstance().getInboxFragment().fragmentActive(true);
				    	transaction.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getInboxFragment(), Constants.MenuID.INBOX.toString()).commit();
			    	}
			    	else if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.REDEEM) {
						DataManager.getInstance().getRedeemFragment().fragmentActive(true);
						transaction.remove(DataManager.getInstance().getWebFragment()).commit();
					}
					else if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.UTC) {
						DataManager.getInstance().getUTCFragment().fragmentActive(true);
						transaction.remove(DataManager.getInstance().getWebFragment()).commit();
						DataManager.getInstance().getUTCFragment().show();
					}
					else if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.FAVORITE) {
						DataManager.getInstance().getFavoriteFragment().fragmentActive(true);
						transaction.remove(DataManager.getInstance().getWebFragment()).commit();
						DataManager.getInstance().getFavoriteFragment().show();
					}
					else if(DataManager.getInstance().getWebFragment().getParent() == Constants.MenuID.SEARCH_BUSINESS) {
						DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
						transaction.remove(DataManager.getInstance().getWebFragment()).commit();
						DataManager.getInstance().getSearchBusinessFragment().show();
					}
			    	break;
			    case BUSINESS:
			    	DataManager.getInstance().getBusinessFragment().fragmentActive(false);
			    	DataManager.getInstance().getAccountFragment().fragmentActive(true);
				    transaction.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getAccountFragment(), Constants.MenuID.ACCOUNT.toString()).commit();
			    	break;
			    case SEARCH_BUSINESS:
			    	DataManager.getInstance().getSearchBusinessFragment().fragmentActive(false);
			    	DataManager.getInstance().getSearchFragment().fragmentActive(true);
				    transaction.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchFragment(), Constants.MenuID.SEARCH.toString()).commit();
			    	break;
			    case SEARCH_EVENT:
			    	DataManager.getInstance().getSearchEventFragment().fragmentActive(false);
			    	DataManager.getInstance().getSearchFragment().fragmentActive(true);
				    transaction.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchFragment(), Constants.MenuID.SEARCH.toString()).commit();
			    	break;
			    case SEARCH_CATEGORIES:
					transaction.remove(DataManager.getInstance().getSearchCategoriesFragment()).commit();
					if(DataManager.getInstance().getSearchCategoriesFragment().getParent() == Constants.MenuID.SEARCH_BUSINESS) {
						Logger.info(mName, "SearchCategories parent SearchBusiness");
						DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
				    	if(DataManager.getInstance().doesCategoryFilteringNeedUpdating()) {
				    		DataManager.getInstance().getSearchBusinessFragment().removeAllLocationResults();
				    		DataManager.getInstance().getSearchBusinessFragment().loadLocations(true);
				    		DataManager.getInstance().setCategoryFilterUpdate(false);
				    	}
				    	DataManager.getInstance().getSearchBusinessFragment().show();
					}
					else if(DataManager.getInstance().getSearchCategoriesFragment().getParent() == Constants.MenuID.SEARCH_EVENT) {
						Logger.info(mName, "SearchCategories parent SearchEvent");
						DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
				    	if(DataManager.getInstance().doesCategoryFilteringNeedUpdating()) {
				    		DataManager.getInstance().getSearchEventFragment().removeAllEventResults();
				    		DataManager.getInstance().getSearchEventFragment().loadEvents();
				    		DataManager.getInstance().setCategoryFilterUpdate(false);
				    	}
				    	DataManager.getInstance().getSearchEventFragment().show();
					}
			    	break;
				case STATISTICS_SUMMARY:
					transaction.remove(DataManager.getInstance().getStatisticsSummaryFragment()).commit();
					DataManager.getInstance().getBusinessFragment().fragmentActive(true);
					DataManager.getInstance().getBusinessFragment().show();
					break;
				case STATISTICS_LIST:
					transaction.remove(DataManager.getInstance().getStatisticsListFragment()).commit();
					DataManager.getInstance().getStatisticsSummaryFragment().fragmentActive(true);
					DataManager.getInstance().getStatisticsSummaryFragment().show();
					break;
				case ANALYTICS_SUMMARY:
					transaction.remove(DataManager.getInstance().getAnalyticsSummaryFragment()).commit();
					DataManager.getInstance().getBusinessFragment().fragmentActive(true);
					DataManager.getInstance().getBusinessFragment().show();
					break;
				case ANALYTICS_BUSINESS:
					Constants.MenuID parent = DataManager.getInstance().getAnalyticsBusinessFragment().getParent();
					transaction.remove(DataManager.getInstance().getAnalyticsBusinessFragment()).commit();
					if(parent == Constants.MenuID.STATISTICS_SUMMARY) {
						// without business statistics permission, go back to business fragment
						if(DataManager.getInstance().hasBusinessAnalyticsPermission() &&
								!DataManager.getInstance().hasBusinessStatisticsPermission()) {
							FragmentTransaction transaction2 = fm.beginTransaction();
							transaction2.remove(DataManager.getInstance().getStatisticsSummaryFragment()).commit();
							DataManager.getInstance().getBusinessFragment().fragmentActive(true);
							DataManager.getInstance().getBusinessFragment().show();
						}
						else {
							DataManager.getInstance().getStatisticsSummaryFragment().fragmentActive(true);
							DataManager.getInstance().getStatisticsSummaryFragment().show();
						}
					}
					else if(parent == Constants.MenuID.STATISTICS_LIST) {
						DataManager.getInstance().getStatisticsListFragment().fragmentActive(true);
						DataManager.getInstance().getStatisticsListFragment().show();
					}
					break;
			    default:
			    	break;
			    }
	    	}
		    else {
		    	Logger.verbose(mName, "onBackPressed, replace with HOME");
		    	replaceFragment(Constants.MenuID.HOME, false);
		    }
	    }
	}
	
	public void clearBackPressed() {
		mBackPressed = false;
	}
	
	public void clearMenuChanged() {
		mMenuChanged = false;
	}
	
	public boolean wasBackPressed() {
		return mBackPressed;
	}
	
	public boolean wasMenuChanged() {
		return mMenuChanged;
	}
	
	public void setFragmentID(Constants.MenuID id) {
		mCurrentFragmentID = id;
	}
	
	public void setParentFragmentID(Constants.MenuID id) {
		mParentOfCurrentFragment = id;
	}
	
	public void replaceFragment(Constants.MenuID fID, boolean fromSavedInstance)
	{
		if(fID == Constants.MenuID.FAVORITE && LoginManager.getInstance().userLoggedIn() == false) {
			Logger.info(mName, "show guest favorites dialog");
			showGuestFavoritesDialog();
			return;
		}
		
		if(fID == Constants.MenuID.INBOX && LoginManager.getInstance().userLoggedIn() == false) {
			Logger.info(mName, "show guest notifications dialog");
			showGuestNotificationsDialog();
			return;
		}

		// don't replace a fragment with itself
		if(mCurrentFragmentID == fID && !fromSavedInstance ) {
			Logger.info(mName, "current FID equals requested FID");
			
			if(DataManager.getInstance().fromBigButton()) {
				setActiveMenuImage(fID.getValue());
				DataManager.getInstance().setFromBigButton(false);
			}
			else {
				Logger.verbose(mName, "replaceFragment, already on " + fID.toString());
			}
			
			return;
		}
		
		if(!fromSavedInstance && fID == mCurrentMainMenu && 
				DataManager.getInstance().currentMenu() == Constants.MenuID.REDEEM) {
			Logger.info(mName, "requested FID is current main menu and is REDEEM");
			return;
		}
		
		// only replace fragments after the appropriate dwell time has occurred
		if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.FRAGMENT_SWITCH_DWELL) {
			Logger.info(mName, "DWELL not elapsed");
			return;
		}
		mTimestamp = SystemClock.elapsedRealtime();
		
		// hide any progress indication from a previous fragment
		hideSpinner();
		
		setInactiveAndCancel();

		// destroy web view if not being used
		if(mCurrentFragmentID == Constants.MenuID.WEB) {
			DataManager.getInstance().getWebFragment().destroyWebView();
		}
		
		// request a location update if our current location is old
		UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
		if(lm.isCurrentLocationStale()) {
			lm.locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_BOTH);
		}
		
		// haptic feedback
		if(fID != Constants.MenuID.HOME) {
			final Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
			vib.vibrate(Constants.VIBRATE_LENGTH);
		}

		mCurrentFragmentID = fID;
		
		final FragmentManager fm = getSupportFragmentManager();
		FragmentTransaction transaction = fm.beginTransaction();
	    
	    // clear back stack since it should be cleared when navigating
	    // back to a main menu
	    Constants.MenuID stackMenuId;
	    while(DataManager.getInstance().menuStackEmpty() == false) {
	    	DataManager.getInstance().popFromMenuStack();
	    	stackMenuId = DataManager.getInstance().lastMenuPopped();
		    switch (stackMenuId) {
		    case REDEEM:
		    	transaction.remove(DataManager.getInstance().getRedeemFragment());
		    	break;
		    case EVENT:
		    	transaction.remove(DataManager.getInstance().getEventFragment());
		    	break;
		    case SEARCH_CATEGORIES:
		    	transaction.remove(DataManager.getInstance().getSearchCategoriesFragment());
		    	break;
		    case WEB:
		    	transaction.remove(DataManager.getInstance().getWebFragment());
		    	break;
			case STATISTICS_SUMMARY:
				transaction.remove(DataManager.getInstance().getStatisticsSummaryFragment());
				break;
			case STATISTICS_LIST:
				transaction.remove(DataManager.getInstance().getStatisticsListFragment());
				break;
			case ANALYTICS_SUMMARY:
				transaction.remove(DataManager.getInstance().getAnalyticsSummaryFragment());
				break;
			case ANALYTICS_BUSINESS:
				transaction.remove(DataManager.getInstance().getAnalyticsBusinessFragment());
				break;
		    default:
		    	break;
		    }
	    }
		
	    Logger.info(mName, "replacing with fragment ID " + fID);
	    
	    if(fID != Constants.MenuID.SEARCH && Utils.saveExists(this, Constants.CATEGORY_ANTIFILTERS)) {
	    	Utils.delete(this, Constants.CATEGORY_ANTIFILTERS);
	    	DataManager.getInstance().setCategoryAntifilters(new CategoryAntifilters());
	    }
	    
		// Replace whatever is in the frameLayout view with this fragment  
	    switch (fID) {
	    case WALLET:
	    	DataManager.getInstance().getWalletFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getWalletFragment(), fID.toString());
	    	break;
	    case FAVORITE:
		    DataManager.getInstance().getFavoriteFragment().fragmentActive(true);
		    transaction.replace(R.id.frameLayoutMiddle, 
		    		DataManager.getInstance().getFavoriteFragment(), fID.toString());
	    	break;
	    case UTC:
		    DataManager.getInstance().getRedeemFragment().setParent(fID);
		    DataManager.getInstance().getUTCFragment().fragmentActive(true);
		    transaction.replace(R.id.frameLayoutMiddle, 
		    		DataManager.getInstance().getUTCFragment(), fID.toString());
	    	break;
	    case INBOX:
	    	DataManager.getInstance().getInboxFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getInboxFragment(), fID.toString());
	    	break;
	    case SEARCH:
	    	DataManager.getInstance().getSearchFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getSearchFragment(), fID.toString());
	    	break;
	    case ACCOUNT:
	    	DataManager.getInstance().getAccountFragment().fragmentActive(true);
			if(mNavigateToSignin) {
				mNavigateToSignin = false;
				DataManager.getInstance().getAccountFragment().navigateToSignin();
			}
			else if(mFacebookSignin) {
				mFacebookSignin = false;
				DataManager.getInstance().getAccountFragment().facebookSignin();
			}
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getAccountFragment(), fID.toString());
	    	break;
	    case SEARCH_BUSINESS:
	    	DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getSearchBusinessFragment(), fID.toString());
	    	break;
	    case SEARCH_EVENT:
	    	DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getSearchEventFragment(), fID.toString());
	    	break;
	    case BUSINESS:
	    	DataManager.getInstance().getBusinessFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getBusinessFragment(), fID.toString());
	    	break;
		case STATISTICS_SUMMARY:
			DataManager.getInstance().getStatisticsSummaryFragment().fragmentActive(true);
			transaction.replace(R.id.frameLayoutMiddle,
					DataManager.getInstance().getStatisticsSummaryFragment(), fID.toString());
			break;
		case STATISTICS_LIST:
			DataManager.getInstance().getStatisticsListFragment().fragmentActive(true);
			transaction.replace(R.id.frameLayoutMiddle,
					DataManager.getInstance().getStatisticsListFragment(), fID.toString());
			break;
		case ANALYTICS_SUMMARY:
			DataManager.getInstance().getAnalyticsSummaryFragment().fragmentActive(true);
			transaction.replace(R.id.frameLayoutMiddle,
					DataManager.getInstance().getAnalyticsSummaryFragment(), fID.toString());
			break;
		case ANALYTICS_BUSINESS:
			DataManager.getInstance().getAnalyticsBusinessFragment().fragmentActive(true);
			transaction.replace(R.id.frameLayoutMiddle,
					DataManager.getInstance().getAnalyticsBusinessFragment(), fID.toString());
			break;
	    case SUBSCRIBE:
	    	DataManager.getInstance().getSubscribeFragment().fragmentActive(true);
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getSubscribeFragment(), fID.toString());
	    	break;
	    case HOME:
	    default:
	    	DataManager.getInstance().getMainFragment().fragmentActive(true);
	    	mCurrentFragmentID = Constants.MenuID.HOME;
	    	transaction.replace(R.id.frameLayoutMiddle, 
	    			DataManager.getInstance().getMainFragment(), mCurrentFragmentID.toString());
	    	break;
	    }
	    
	    // fading transition between fragments has been removed,
	    // without it transitions feel snappier
	    //transaction.setTransition(FragmentTransaction.TRANSIT_FRAGMENT_OPEN);
	
		// Commit the transaction
		transaction.commit();

		mMenuChanged = true;
		
		// update our menu image
		setActiveMenuImage(fID.getValue());
	}
	
	private void setActiveMenuImage(int menuOn)
	{
		// set menu item found by menuOn to on, all others off
		for(int i = 0; i < Constants.NUM_MAIN_MENU_ITEMS; i++) {
			if(i == menuOn) {
				((ImageView) findViewById(Constants.menuImageViews[i])).setImageResource(Constants.menuOnImageResouces[i]);
				mCurrentMainMenu = Constants.MenuID.fromValue(menuOn);
			}
			else {
				((ImageView) findViewById(Constants.menuImageViews[i])).setImageResource(Constants.menuImageResouces[i]);
			}
		}
	}
	
	public void clearMenuImages()
	{
		// set menu item found by menuOn to on, all others off
		for(int i = 0; i < Constants.NUM_MAIN_MENU_ITEMS; i++) {
			((ImageView) findViewById(Constants.menuImageViews[i])).setImageResource(Constants.menuImageResouces[i]);
		}
	}
	
	public void setAccountFragment() {
		// set all menu items off
		for(int i = 0; i < Constants.NUM_MAIN_MENU_ITEMS; i++) {
			((ImageView) findViewById(Constants.menuImageViews[i])).setImageResource(Constants.menuImageResouces[i]);
		}
		
		replaceFragment(Constants.MenuID.ACCOUNT, false);
	}
	
	public void setSubscribeFragment() {
		// set all menu items off
		for(int i = 0; i < Constants.NUM_MAIN_MENU_ITEMS; i++) {
			((ImageView) findViewById(Constants.menuImageViews[i])).setImageResource(Constants.menuImageResouces[i]);
		}
		
		replaceFragment(Constants.MenuID.SUBSCRIBE, false);
	}
	
	public ImageDownloader getImageDownloader() {
		return mImageDownloader;
	}
	
	public boolean isNetworkAvailable() {
	    ConnectivityManager connectivityManager 
	          = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
	    NetworkInfo activeNetworkInfo = connectivityManager.getActiveNetworkInfo();
	    return activeNetworkInfo != null && activeNetworkInfo.isConnected();
	}
	
	public void showSpinner() {
		this.findViewById(R.id.progressBarMain).setVisibility(View.VISIBLE);
	}
	
	public void hideSpinner() {
		this.findViewById(R.id.progressBarMain).setVisibility(View.INVISIBLE);
	}
	
	private void setInactiveAndCancel() {
		// clear scan data and requesting PIN state if it was being used 
		// for a previous redeem or check-in operation
		DataManager.getInstance().setScanData(null);
		DataManager.getInstance().setRequestingPIN(false);
		
		// make sure we set all fragments as inactive so we don't run into 
		// any issues where parent fragments of submenus execute their 
		// lifecycle methods when we don't want them to - fragments should 
		// check their active state before performing certain operations	
		DataManager.getInstance().getMainFragment().fragmentActive(false);
		DataManager.getInstance().getWalletFragment().fragmentActive(false);
		DataManager.getInstance().getFavoriteFragment().fragmentActive(false);
		DataManager.getInstance().getUTCFragment().fragmentActive(false);
		DataManager.getInstance().getInboxFragment().fragmentActive(false);
		DataManager.getInstance().getSearchFragment().fragmentActive(false);
		DataManager.getInstance().getSearchBusinessFragment().fragmentActive(false);
		DataManager.getInstance().getSearchEventFragment().fragmentActive(false);
		DataManager.getInstance().getSearchCategoriesFragment().fragmentActive(false);
		DataManager.getInstance().getRedeemFragment().fragmentActive(false);
		DataManager.getInstance().getWebFragment().fragmentActive(false);
		DataManager.getInstance().getAccountFragment().fragmentActive(false);
		DataManager.getInstance().getBusinessFragment().fragmentActive(false);
		DataManager.getInstance().getSubscribeFragment().fragmentActive(false);
		
		// cancel any async tasks being executed on fragments and this activity
		DataManager.getInstance().getMainFragment().cancelImageSwitcherTask();
		DataManager.getInstance().getUTCFragment().cancelAllTasks();
		DataManager.getInstance().getRedeemFragment().cancelAllTasks();
		DataManager.getInstance().getWalletFragment().cancelWalletTask();
		DataManager.getInstance().getSearchCategoriesFragment().cancelCategoriesTask();
		DataManager.getInstance().getSearchBusinessFragment().cancelAllTasks();
		DataManager.getInstance().getSearchEventFragment().cancelAllTasks();
		DataManager.getInstance().getBusinessFragment().cancelAllTasks();
		DataManager.getInstance().getSubscribeFragment().cancelSubscribeTask();
		LoginManager.getInstance().cancelAllTasks();
		cancelSocialPostTask();
		
		// hide horizontal progress bar
		findViewById(R.id.progressBarHorizontal).setVisibility(ProgressBar.GONE);
	}
	
    private void showGuestFavoritesDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestFavoriteDialog");
    	mUTCDialog.show(getSupportFragmentManager(), "GuestFavoriteDialog");
    }
    
    private void showGuestNotificationsDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestNotificationsDialog");
    	mUTCDialog.show(getSupportFragmentManager(), "GuestNotificationsDialog");
    }

	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	
	/*
	 * Notification and GCM-related functionality
	 */
	
	public void sendView(String viewName) {
		if(DataManager.getInstance().getAnalyticsState()) {
			mTracker.setScreenName(viewName);
			mTracker.send(new HitBuilders.ScreenViewBuilder().build());
		}
	}
	
	public void generateNotification(String title, String content) {
        SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
        boolean notificationsEnabled = settings.getBoolean(Constants.SHARED_PREFERENCES_NOTIFICATIONS, true);
        
        if(notificationsEnabled) {
			Logger.info(mName, "generating notification");
			
			NotificationCompat.Builder mBuilder =
			        new NotificationCompat.Builder(this)
			        .setSmallIcon(R.drawable.ic_launcher)
			        .setContentTitle(title)
			        .setContentText(content);
			// Creates an explicit intent for an Activity in your app
			Intent resultIntent = new Intent(this, MainActivity.class);
	
			// The stack builder object will contain an artificial back stack for the
			// started Activity.
			// This ensures that navigating backward from the Activity leads out of
			// your application to the Home screen.
			TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
			// Adds the back stack for the Intent (but not the Intent itself)
			stackBuilder.addParentStack(MainActivity.class);
			// Adds the Intent that starts the Activity to the top of the stack
			stackBuilder.addNextIntent(resultIntent);
			PendingIntent resultPendingIntent =
			        stackBuilder.getPendingIntent(
			            0,
			            PendingIntent.FLAG_UPDATE_CURRENT
			        );
			mBuilder.setContentIntent(resultPendingIntent);
			NotificationManager mNotificationManager =
			    (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
			// mId allows you to update the notification later on.
			mNotificationManager.notify(0, mBuilder.build());
        }
	}
	
	public void registerWithGCM() {
    	SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
        if (!settings.contains(Constants.GCM_REGISTRATION_ID)) {
        	Logger.info(mName, "registration ID does not exist, requesting");
    		Intent registrationIntent = new Intent("com.google.android.c2dm.intent.REGISTER");
			registrationIntent = Utils.createExplicitFromImplicitIntent(this, registrationIntent);
    		// sets the app name in the intent
    		registrationIntent.putExtra("app", PendingIntent.getBroadcast(this, 0, new Intent(), 0));
    		registrationIntent.putExtra("sender", Constants.GOOGLE_API_PROJECT_NUMBER);
			startService(registrationIntent);
        }
        else {
        	Logger.info(mName, "registration ID already exists");
        }
	}
	
	public void unregisterFromGCM() {
		Logger.info(mName, "requesting to unregister");
		DataManager.getInstance().setUnregistering(true);
		DataManager.getInstance().setUnregisterTimestamp(SystemClock.elapsedRealtime());
		Intent unregIntent = new Intent("com.google.android.c2dm.intent.UNREGISTER");
		unregIntent = Utils.createExplicitFromImplicitIntent(this, unregIntent);
		unregIntent.putExtra("app", PendingIntent.getBroadcast(this, 0, new Intent(), 0));
		startService(unregIntent);
	}
	
	public boolean isMyServiceRunning(Class<?> serviceClass) {
	    ActivityManager manager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
	    for (RunningServiceInfo service : manager.getRunningServices(Integer.MAX_VALUE)) {
	        if (serviceClass.getName().equals(service.service.getClassName())) {
	            return true;
	        }
	    }
	    return false;
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	
	/*
	 *  Facebook-related functionality using the Facebook LoginManager
	 */
	
	public void facebookLogin(boolean setButton, boolean showProgress) {
		// start Facebook Login
		com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(this, FB_READ_PERMISSIONS);
	}
	
	public void facebookLogout() {
		// logout and clear
		com.facebook.login.LoginManager.getInstance().logOut();
		LoginManager.getInstance().getAccountContext().setFacebook(false);
		LoginManager.getInstance().refreshAvatar();
		try {
			Utils.save(MainActivity.this, Constants.ACCOUNT_CONTEXT, LoginManager.getInstance().getAccountContext());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void callbackLoginSuccessFromSignin() {
		if(DataManager.getInstance().getAccountFragment().isFragmentActive()) {
			DataManager.getInstance().getAccountFragment().loggedIn();
		}
	}
	
	public void facebookLogoutAndClear() {
		facebookLogout();
	}
	
	public boolean facebookSessionOpen() {
		return AccessToken.getCurrentAccessToken() != null;
	}
	
	public void publishStory(String name, String caption, String description, String link, String picture, 
			String message, int businessID) {
		// Cannot publish in the background without user aware of message being posted
	}
	
	public void publishFeedDialog(String name, String caption, String description, String link, String picture, String message, int businessID, FacebookCallback<Sharer.Result> fbCallback) {
		mFacebookName = name;
		mFacebookCaption = caption;
		mFacebookDescription = description;
		mFacebookLink = link;
		mFacebookPicture = picture;
		mFacebookMessage = message;
		mSocialBusinessID = businessID;
		
		ShareLinkContent content;
		
		Logger.info(mName, "LINK: " + link);
		Logger.info(mName, "CAPTION: " + caption);
		Logger.info(mName, "MESSAGE: " + message);
		
		if(picture != null)
		{
			Logger.info(mName, "PICTURE: " + picture);
			content = new ShareLinkContent.Builder()
				.setContentUrl(Uri.parse(link))
				.setContentTitle(caption)
				.setContentDescription(message)
				.setImageUrl(Uri.parse(picture))
				.build();
		}
		else
		{
			content = new ShareLinkContent.Builder()
				.setContentUrl(Uri.parse(link))
				.setContentTitle(caption)
				.setContentDescription(message)
				.build();
		}

		if(fbCallback != null) {
			shareDialog.registerCallback(callbackManager, fbCallback);
		}
		else {
			mSocialPostTask = new SocialPostTask();
			if(Utils.hasHoneycomb()) {
				mSocialPostTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, LoginManager.getInstance().getAccountContext().getAccountID(),
						Constants.SocialPostType.FACEBOOK.getValue(),
						mSocialBusinessID);
			}
			else {
				mSocialPostTask.execute(LoginManager.getInstance().getAccountContext().getAccountID(),
						Constants.SocialPostType.FACEBOOK.getValue(),
						mSocialBusinessID);
			}
		}
		shareDialog.show(content);
	}
	
	private boolean isSubsetOf(Collection<String> subset, Collection<String> superset) {
	    for (String string : subset) {
	        if (!superset.contains(string)) {
	            return false;
	        }
	    }
	    return true;
	}
	
	public void twitterLogin() {
		LoginManager.getInstance().getAccountContext().setTwitter(true);
		try {
			Utils.save(MainActivity.this, Constants.ACCOUNT_CONTEXT, LoginManager.getInstance().getAccountContext());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public void twitterLogout() {
		LoginManager.getInstance().getAccountContext().setTwitter(false);
		try {
			Utils.save(MainActivity.this, Constants.ACCOUNT_CONTEXT, LoginManager.getInstance().getAccountContext());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public void publishTweet(String message, int businessID) {
		String url = "https://twitter.com/intent/tweet?source=webclient&text=" + message.replace(" ", "+");
		Intent i = new Intent(Intent.ACTION_VIEW);
		i.setData(Uri.parse(url));
		startActivity(i);

		submitTweetPost(businessID);
	}

	public void publishInstagram(String mediaPath) {
		Intent share = new Intent(Intent.ACTION_SEND);
		share.setType("image/*");
		File media = new File(mediaPath);
		Uri uri = Uri.fromFile(media);
		share.putExtra(Intent.EXTRA_STREAM, uri);
		startActivity(Intent.createChooser(share, "Share to"));
	}

	public void submitFacebookPost(int businessID) {
		mSocialBusinessID = businessID;
		mSocialPostTask = new SocialPostTask();
		if(Utils.hasHoneycomb()) {
			mSocialPostTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.FACEBOOK.getValue(),
					mSocialBusinessID);
		}
		else {
			mSocialPostTask.execute(LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.FACEBOOK.getValue(),
					mSocialBusinessID);
		}
	}

	public void submitTweetPost(int businessID) {
		mSocialBusinessID = businessID;
		mSocialPostTask = new SocialPostTask();
		if(Utils.hasHoneycomb()) {
			mSocialPostTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.TWITTER.getValue(),
					mSocialBusinessID);
		}
		else {
			mSocialPostTask.execute(LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.TWITTER.getValue(),
					mSocialBusinessID);
		}
	}

	public void submitInstagramPost(int businessID) {
		mSocialBusinessID = businessID;
		mSocialPostTask = new SocialPostTask();
		if(Utils.hasHoneycomb()) {
			mSocialPostTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.INSTAGRAM.getValue(),
					mSocialBusinessID);
		}
		else {
			mSocialPostTask.execute(LoginManager.getInstance().getAccountContext().getAccountID(),
					Constants.SocialPostType.INSTAGRAM.getValue(),
					mSocialBusinessID);
		}
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
				com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(MainActivity.this, MainActivity.FB_READ_PERMISSIONS);
				break;
			case SignInDialog.RESULT_TAP_HERE:
				break;
			default:
				break;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	
	/*
	 * Social-post related
	 */
    private class SocialPostTask extends AsyncTask<Integer, Void, Boolean> {
        protected Boolean doInBackground(Integer... params) {
    		int accountID = params[0].intValue();
    		int socialPostID = params[1].intValue();
    		int businessID = params[2].intValue();
    		
    		if(LoginManager.getInstance().userLoggedIn()) {
    			UTCWebAPI.createSocialPost(LoginManager.getInstance().getAccountContext().getToken(), 
    					accountID, 
    					socialPostID,
    					businessID
        				);
    		}
    		
    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
        	if(success.booleanValue()) {
    			Logger.info(mName, "Social post request sent");
    			DataManager.getInstance().setWalletNeedsUpdate(true);
        	}
        }
    }
    
    public boolean cancelSocialPostTask() {
    	boolean result = false;
    	if(mSocialPostTask != null) {
    		Logger.info(mName, "SocialPostTask cancelled");
    		result = mSocialPostTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "SocialPostTask was null when cancelling");
    	}
    	return result;
    }
}
