package com.unitethiscity.ui;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.ProximityLocationService;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class AccountFragment extends Fragment implements SignInDialog.SignInDialogListener {
	
	private String mName = Constants.ACCOUNT_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.MAIN;
	public Constants.MenuID mMenuID = Constants.MenuID.ACCOUNT;
	
	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;
	private boolean mGoToSignin = false;
	private boolean mSignIntoFacebook = false;
	private boolean mFacebookLoginFromSignin = false;

	SignInDialog mSignInDialog;
	
	private String mEmail;
	private String mPassword;

    @Override
    public void onCreate(Bundle savedInstanceState) {
    	super.onCreate(savedInstanceState);
    	Logger.verbose(mName, "onCreate()");
    }
    
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
    	Logger.verbose(mName, "onCreateView()");
    	
    	mContainer = container;
    	
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_account, container, false);
    	
		String versionText = "";
		PackageInfo pInfo;
		try {
			pInfo = getActivity().getPackageManager().getPackageInfo(getActivity().getPackageName(), 0);
			versionText = pInfo.versionName;
		} catch (NameNotFoundException e) {
			e.printStackTrace();
		}
		
    	TextView version = (TextView) mParent.findViewById(R.id.version);
    	version.setText(versionText);
    	
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");
        
        mEmail = "";
        mPassword = "";
        
    	((MainActivity) getActivity()).showSpinner();

    	if(mFragmentActive == true) {
    		((MainActivity) getActivity()).clearBackPressed();

            TextView tv;
			Button bt;
            if(LoginManager.getInstance().userLoggedIn()) {
            	Logger.info(mName, "account ID - " + LoginManager.getInstance().getAccountContext().getAccountID().toString());
            	Logger.info(mName, "account token - " + LoginManager.getInstance().getAccountContext().getToken());
            	
            	tv = (TextView) mParent.findViewById(R.id.accountUserName);
            	if(tv != null) {
            		tv.setText(LoginManager.getInstance().getAccountContext().getAccountFullName());
            	}
            	RelativeLayout relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutCurrentUser);
        		if(relLayout != null) {
        			bt = (Button) relLayout.findViewById(R.id.buttonSignIn);
        			bt.setVisibility(Button.GONE);
        			bt = (Button) relLayout.findViewById(R.id.buttonSignOut);
        			bt.setVisibility(Button.VISIBLE);
        		}
        		
        		if(LoginManager.getInstance().getAvatar() != null) {
            		ImageView iv = (ImageView) mParent.findViewById(R.id.imageViewUserPicture);
            		iv.setImageBitmap(LoginManager.getInstance().getAvatar());
        		}
        		else {
        			Logger.warn(mName, "missing avatar");
        		}

				if(mSignIntoFacebook) {// || LoginManager.getInstance().getFacebookContext().getUserID() != null) {
					mSignIntoFacebook = false;
					((MainActivity) getActivity()).registerWithGCM();
					facebookLogin();
				}
        		
        		showSocialElements();
        		hideGuestElements();
        		showBusinessButton();
            }
            
            final Button readMoreButton = (Button) mParent.findViewById(R.id.buttonReadMore);
            readMoreButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					Uri url = Uri.parse(Constants.UNITE_THIS_CITY_HOME_URL);
					Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
    				try {
    					startActivity(launchBrowser);
    				}
    				catch(ActivityNotFoundException anfe) {
    					anfe.printStackTrace();
    				}
				}
			});

			final Button viewTutorialButton = (Button) mParent.findViewById(R.id.buttonViewTutorial);
			viewTutorialButton.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					Intent tutorialIntent = new Intent(getActivity(), TutorialActivity.class);
					tutorialIntent.setFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
					getActivity().startActivity(tutorialIntent);
				}
			});
            
            final Button joinNowButton = (Button) mParent.findViewById(R.id.buttonJoinNow);
            joinNowButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					((MainActivity) getActivity()).setSubscribeFragment();
				}
			});

			final Button joinNowFacebookButton = (Button) mParent.findViewById(R.id.buttonJoinNowFacebook);
			joinNowFacebookButton.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					// start Facebook Login
					com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(getActivity(), MainActivity.FB_READ_PERMISSIONS);
				}
			});
            
            final Button signInButton = (Button) mParent.findViewById(R.id.buttonSignIn);
            signInButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					mSignInDialog = SignInDialog.newInstance();
					mSignInDialog.show(getActivity().getSupportFragmentManager());
				}
			});
            
            final Button signOutButton = (Button) mParent.findViewById(R.id.buttonSignOut);
            signOutButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					((MainActivity) getActivity()).unregisterFromGCM();
					LoginManager.getInstance().logout((MainActivity) getActivity());
				}
			});
            
            final Button cancelButton = (Button) mParent.findViewById(R.id.buttonCancel);
            cancelButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					RelativeLayout relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSignIn);
					relLayout.setVisibility(RelativeLayout.GONE);
					relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutCurrentUser);
					relLayout.setVisibility(RelativeLayout.VISIBLE);
					EditText et = (EditText) mParent.findViewById(R.id.editTextPassword);
					if(et != null) {
						et.setText("");
					}
				}
			});
            
            EditText et = (EditText) mParent.findViewById(R.id.editTextEmail);
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
            
            et = (EditText) mParent.findViewById(R.id.editTextPassword);
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
            
            final Button loginButton = (Button) mParent.findViewById(R.id.buttonLogin);
            loginButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					LoginManager.getInstance().login((MainActivity) getActivity(), 
							mEmail, 
							mPassword);
				}
			});

			ImageView facebook = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialFacebook);
			ImageView twitter = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialTwitter);
			ImageView instagram = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialInstagram);
			facebook.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					SharedPreferences settings = getActivity().getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
					int social = settings.getInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, RedeemRewardDialog.DEFAULT_SOCIAL_NONE);

					if (social == RedeemRewardDialog.DEFAULT_SOCIAL_FACEBOOK) {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_NONE;
						showDefaultSocialNone();
					}
					else {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_FACEBOOK;
						showDefaultSocialFacebook();
					}

					settings.edit().putInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, social).commit();
				}
			});
			twitter.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					SharedPreferences settings = getActivity().getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
					int social = settings.getInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, RedeemRewardDialog.DEFAULT_SOCIAL_NONE);

					if (social == RedeemRewardDialog.DEFAULT_SOCIAL_TWITTER) {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_NONE;
						showDefaultSocialNone();
					}
					else {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_TWITTER;
						showDefaultSocialTwitter();
					}

					settings.edit().putInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, social).commit();
				}
			});
			instagram.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {
					SharedPreferences settings = getActivity().getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
					int social = settings.getInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, RedeemRewardDialog.DEFAULT_SOCIAL_NONE);

					if (social == RedeemRewardDialog.DEFAULT_SOCIAL_INSTAGRAM) {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_NONE;
						showDefaultSocialNone();
					}
					else {
						social = RedeemRewardDialog.DEFAULT_SOCIAL_INSTAGRAM;
						showDefaultSocialInstagram();
					}

					settings.edit().putInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, social).commit();
				}
			});
			
			final CheckBox notifications = (CheckBox) mParent.findViewById(R.id.checkboxAccountNotifications);
            final SharedPreferences settings = getActivity()
            		.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
            boolean notificationsEnabled = settings
            		.getBoolean(Constants.SHARED_PREFERENCES_NOTIFICATIONS, true);
            notifications.setChecked(notificationsEnabled);
            notifications.setOnCheckedChangeListener(new OnCheckedChangeListener() {

				@Override
				public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
					settings.edit().putBoolean(Constants.SHARED_PREFERENCES_NOTIFICATIONS, 
							isChecked).commit();
					
					Intent intent = new Intent(getActivity(), ProximityLocationService.class);
					
					if(isChecked) {
						// stop the proximity service
						getActivity().startService(intent);
					}
					else {
						// start the proximity service
						getActivity().stopService(intent);
					}
				}
            	
            });
			
            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
    	}

		if(mGoToSignin) {
			mGoToSignin = false;
			RelativeLayout relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutCurrentUser);
			relLayout.setVisibility(RelativeLayout.GONE);
			relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSignIn);
			relLayout.setVisibility(RelativeLayout.VISIBLE);
		}

    	Logger.verbose(mName, "onActivityCreated fragmentActive - " + mFragmentActive);
    }
    
    @Override
    public void onPause() {
    	super.onPause();
		Logger.verbose(mName, "onPause()");
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");

    	if(DataManager.getInstance().getAnalyticsState()) {
    		Logger.verbose(mName, "starting Google analytics for this screen");
    		((MainActivity) getActivity()).sendView(mName);
    	}
    }
    
    public void fragmentActive(boolean activeState) {
    	Logger.verbose(mName, "fragmentActive before - " + mFragmentActive);
    	if(activeState != mFragmentActive) {
        	mFragmentActive = activeState;
        	Logger.verbose(mName, "fragmentActive after - " + mFragmentActive);
    	}
    }

	public boolean isFragmentActive() {
		return mFragmentActive;
	}
    
    public void hide() {
    	if(mContainer != null) {
            for(int i = 0; i < mContainer.getChildCount(); i++) {
                View v = mContainer.getChildAt(i);
                v.setVisibility(View.GONE);
            }
    	}
    }
    
    public void show() {
    	if(mContainer != null) {
            for(int i = 0; i < mContainer.getChildCount(); i++) {
                View v = mContainer.getChildAt(i);
                v.setVisibility(View.VISIBLE);
            }
    	}
    }
	
	public void loggedIn() {
		RelativeLayout relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSignIn);
		relLayout.setVisibility(RelativeLayout.GONE);
		relLayout = (RelativeLayout)mParent.findViewById(R.id.relativeLayoutCurrentUser);
		Button bt = (Button) relLayout.findViewById(R.id.buttonSignIn);
		bt.setVisibility(Button.GONE);
		bt = (Button) relLayout.findViewById(R.id.buttonSignOut);
		bt.setVisibility(Button.VISIBLE);
		relLayout.setVisibility(RelativeLayout.VISIBLE);
		TextView tv = (TextView) relLayout.findViewById(R.id.accountUserName);
		tv.setText(LoginManager.getInstance().getAccountContext().getAccountFullName());
		
		showSocialElements();
		hideGuestElements();
		showBusinessButton();
		
		DataManager.getInstance().setLocalUpdateNeeded(true);
		DataManager.getInstance().setLocationsNeedUpdated(true);
		
		// start the proximity service
		FragmentActivity parent = getActivity();
		if(parent != null) {
			Intent intent = new Intent(parent, ProximityLocationService.class);
			parent.startService(intent);
		}
	}

	public void navigateToSignin() {
		mGoToSignin = true;
	}

	public void facebookSignin() {
		mSignIntoFacebook = true;
	}

	public void loggedOut() {
		TextView tv = (TextView) mParent.findViewById(R.id.accountUserName);
		if(tv != null) {
			tv.setText(mParent.getResources().getString(R.string.account_guest_user));
		}
		EditText et = (EditText) mParent.findViewById(R.id.editTextPassword);
		if(et != null) {
			et.setText("");
		}
		RelativeLayout relLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutCurrentUser);
		if(relLayout != null) {
			Button bt = (Button) relLayout.findViewById(R.id.buttonSignOut);
			bt.setVisibility(Button.GONE);
			bt = (Button) relLayout.findViewById(R.id.buttonSignIn);
			bt.setVisibility(Button.VISIBLE); 
		}
		
		hideSocialElements();
		showGuestElements();
		hideBusinessButton();
		
		DataManager.getInstance().setLocalUpdateNeeded(true);
		DataManager.getInstance().setLocationsNeedUpdated(true);
		
		// stop the proximity service
		FragmentActivity parent = getActivity();
		if(parent != null) {
			Intent intent = new Intent(parent, ProximityLocationService.class);
			parent.stopService(intent);
		}
	}
	
	public void facebookLogin() {
		((MainActivity) getActivity()).facebookLogin(true, true);
	}
	
	public void facebookLogout() {
		((MainActivity) getActivity()).facebookLogout();
	}
	
	private void hideGuestElements() {
		// hide guest-related text and buttons
		Button bt = (Button) mParent.findViewById(R.id.buttonReadMore);
		bt.setVisibility(Button.GONE);
		bt = (Button) mParent.findViewById(R.id.buttonJoinNow);
		bt.setVisibility(Button.GONE);
		bt = (Button) mParent.findViewById(R.id.buttonJoinNowFacebook);
		bt.setVisibility(Button.GONE);
		TextView tv = (TextView) mParent.findViewById(R.id.accountUTCDescription);
		tv.setVisibility(TextView.GONE);
		tv = (TextView) mParent.findViewById(R.id.accountNotAMember);
		tv.setVisibility(TextView.GONE);
	}
	
	private void showGuestElements() {
		// show guest-related text and buttons again
		Button bt = (Button) mParent.findViewById(R.id.buttonReadMore);
		bt.setVisibility(Button.VISIBLE);
		bt = (Button) mParent.findViewById(R.id.buttonJoinNow);
		bt.setVisibility(Button.VISIBLE);
		bt = (Button) mParent.findViewById(R.id.buttonJoinNowFacebook);
		bt.setVisibility(Button.VISIBLE);
		TextView tv = (TextView) mParent.findViewById(R.id.accountUTCDescription);
		tv.setVisibility(TextView.VISIBLE);
		tv = (TextView) mParent.findViewById(R.id.accountNotAMember);
		tv.setVisibility(TextView.VISIBLE);
	}
	
	private void showBusinessButton() {
		if(LoginManager.getInstance().getAccountContext().hasBusinessRoles() && Constants.ENABLE_BUSINESS_BUTTON) {
			final Button bt = (Button) mParent.findViewById(R.id.buttonBusiness);
            bt.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					((MainActivity) getActivity()).setFragmentID(Constants.MenuID.BUSINESS);
					DataManager.getInstance().getBusinessFragment().fragmentActive(true);
					FragmentManager fm = getActivity().getSupportFragmentManager();
					FragmentTransaction ft = fm.beginTransaction();
					ft.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getBusinessFragment(), 
							Constants.MenuID.BUSINESS.toString());
					DataManager.getInstance().pushToMenuStack(Constants.MenuID.BUSINESS);
					ft.commit();
				}
			});
            bt.setVisibility(Button.VISIBLE);
		}
	}
	
	private void hideBusinessButton() {
		((Button) mParent.findViewById(R.id.buttonBusiness)).setVisibility(Button.GONE);
	}
	
	private void showSocialElements() {
		RelativeLayout social = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSocial);
		social.setVisibility(RelativeLayout.VISIBLE);

		SharedPreferences settings = getActivity().getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
		int defaultSocial = settings.getInt(Constants.SHARED_PREFERENCES_DEFAULT_SOCIAL, RedeemRewardDialog.DEFAULT_SOCIAL_NONE);

		switch (defaultSocial)
		{
			case RedeemRewardDialog.DEFAULT_SOCIAL_NONE:
				showDefaultSocialNone();
				break;
			case RedeemRewardDialog.DEFAULT_SOCIAL_FACEBOOK:
				showDefaultSocialFacebook();
				break;
			case RedeemRewardDialog.DEFAULT_SOCIAL_TWITTER:
				showDefaultSocialTwitter();
				break;
			case RedeemRewardDialog.DEFAULT_SOCIAL_INSTAGRAM:
				showDefaultSocialInstagram();
				break;
			default:
				showDefaultSocialNone();
				break;
		}
	}

	private void showDefaultSocialNone() {
		ImageView facebook = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialFacebook);
		ImageView twitter = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialTwitter);
		ImageView instagram = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialInstagram);

		facebook.setImageResource(R.drawable.btn_default_social_facebook_off);
		twitter.setImageResource(R.drawable.btn_default_social_twitter_off);
		instagram.setImageResource(R.drawable.btn_default_social_instagram_off);
	}

	private void showDefaultSocialFacebook() {
		ImageView facebook = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialFacebook);
		ImageView twitter = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialTwitter);
		ImageView instagram = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialInstagram);

		facebook.setImageResource(R.drawable.btn_default_social_facebook_on);
		twitter.setImageResource(R.drawable.btn_default_social_twitter_off);
		instagram.setImageResource(R.drawable.btn_default_social_instagram_off);
	}

	private void showDefaultSocialTwitter() {
		ImageView facebook = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialFacebook);
		ImageView twitter = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialTwitter);
		ImageView instagram = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialInstagram);

		facebook.setImageResource(R.drawable.btn_default_social_facebook_off);
		twitter.setImageResource(R.drawable.btn_default_social_twitter_on);
		instagram.setImageResource(R.drawable.btn_default_social_instagram_off);
	}

	private void showDefaultSocialInstagram() {
		ImageView facebook = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialFacebook);
		ImageView twitter = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialTwitter);
		ImageView instagram = (ImageView) mParent.findViewById(R.id.imageViewDefaultSocialInstagram);

		facebook.setImageResource(R.drawable.btn_default_social_facebook_off);
		twitter.setImageResource(R.drawable.btn_default_social_twitter_off);
		instagram.setImageResource(R.drawable.btn_default_social_instagram_on);
	}
	
	private void hideSocialElements() {
		((RelativeLayout) mParent.findViewById(R.id.relativeLayoutSocial)).setVisibility(RelativeLayout.GONE);
	}

	@Override
	public void onFinishSignInDialog(int result, String email, String password) {
		Logger.info(mName, "onFinishSignInDialog: " + String.valueOf(result));

		switch (result) {
			case SignInDialog.RESULT_EMAIL_LOGIN:
				LoginManager.getInstance().login((MainActivity)getActivity(), mEmail, mPassword);
				break;
			case SignInDialog.RESULT_FACEBOOK_LOGIN:
				// start Facebook Login
				mFacebookLoginFromSignin = true;
				com.facebook.login.LoginManager.getInstance().logInWithReadPermissions(getActivity(), MainActivity.FB_READ_PERMISSIONS);
				break;
			case SignInDialog.RESULT_TAP_HERE:
				break;
			default:
				break;
		}
	}
}