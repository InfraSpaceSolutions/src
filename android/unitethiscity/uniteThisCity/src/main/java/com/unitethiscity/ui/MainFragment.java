package com.unitethiscity.ui;

import java.util.ArrayList;
import java.util.Random;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCLocation;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.annotation.TargetApi;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

public class MainFragment extends Fragment {
	
	private String mName = Constants.MAIN_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.MAIN;
	public Constants.MenuID mMenuID = Constants.MenuID.HOME;
	
	private View mParent;
	
	private boolean mFragmentActive = true;
	
	private Integer[] mImageIds = {
			R.drawable.splash_tip_wallet,
			R.drawable.splash_tip_favorites,
			R.drawable.splash_tip_utc,
			R.drawable.splash_tip_mailbox,
			R.drawable.splash_tip_search,
			R.drawable.splash_tip_quick,
			R.drawable.splash_tip_account_nonmember
	};
	
	private Integer mImageIdAccount = R.drawable.splash_tip_account;
	private boolean mSwitchAccountImage = true;
	
	private ImageSwitcherTask mImageSwitcherTask;
	private int mImageIndex = 6;
//	private int[] mNameIDs = {R.id.locationNames1, R.id.locationNames2, R.id.locationNames3, R.id.locationNames4, 
//			R.id.locationNames5, R.id.locationNames6,};
	private int[] mNameIDs = {
			R.id.name1Row1, R.id.name2Row1, R.id.name1Row2, R.id.name2Row2, R.id.name1Row3, R.id.name2Row3,
			R.id.name1Row4, R.id.name1Row5, R.id.name2Row5, R.id.name1Row6
	};
//	private long[] mNameDurations = {9000L, 7000, 11000L, 8000L, 12000L, 10000L};
//	private long[] mNameDelays = {0L, 6000L, 1500L, 4500L, 7500L, 3000L};
	
	private UTCDialogFragment mUTCDialog;
	
	private boolean mCheckIn = false;
	private boolean mRedeem = false;

    @TargetApi(Build.VERSION_CODES.HONEYCOMB_MR2)
	@Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_main, container, false);
    	
//		int numOfLocations = DataManager.getInstance().getLocations().size();
//		Random randomGen = new Random();
//		int namesLine = 0;
//		int namesPerLine = 0;
//		ArrayList<String> usedNames = new ArrayList<String>();
//		String line = "";
//		while(namesLine < 6) {
//			UTCLocation utcLoc = DataManager.getInstance()
//					.getLocation(Integer.valueOf(randomGen.nextInt(numOfLocations - 1)));
//			if(utcLoc != null) {
//				if(!usedNames.contains(utcLoc.get(LocationParser.JSON_TAG_NAME))) {
//					usedNames.add(utcLoc.get(LocationParser.JSON_TAG_NAME));
//					line = line + "     " + utcLoc.get(LocationParser.JSON_TAG_NAME);
//					if(++namesPerLine >= 1) {
//						((TextView) mParent.findViewById(mNameIDs[namesLine])).setText(line);
//
//						Animation inFromRight = new TranslateAnimation(
//		                        Animation.RELATIVE_TO_PARENT,  +1.0f,
//		                        Animation.RELATIVE_TO_PARENT,  -1.0f,
//		                        Animation.RELATIVE_TO_PARENT,  0.0f,
//		                        Animation.RELATIVE_TO_PARENT,   0.0f
//		                        );
//						inFromRight.setFillAfter(true);
//						inFromRight.setRepeatCount(Animation.INFINITE);
//						inFromRight.setRepeatMode(Animation.INFINITE);
//						inFromRight.setDuration(mNameDurations[namesLine]);
//						inFromRight.setStartOffset(mNameDelays[namesLine]);
//						inFromRight.setInterpolator(new AccelerateInterpolator());
//		                        
//		                ((TextView) mParent.findViewById(mNameIDs[namesLine])).startAnimation(inFromRight);
//						
//						namesLine++;
//						namesPerLine = 0;
//						line = "";
//					}
//				}
//			}
//		}
    	
		int numOfLocations = DataManager.getInstance().getLocations().size();
		if(numOfLocations >= mNameIDs.length) {
			Random randomGen = new Random();
			ArrayList<String> usedNames = new ArrayList<String>();
			for(int i = 0; i < mNameIDs.length;) {
				UTCLocation utcLoc = DataManager.getInstance()
						.getLocation(Integer.valueOf(randomGen.nextInt(numOfLocations - 1)));
				if(utcLoc != null) {
					String name = utcLoc.get(LocationParser.JSON_TAG_NAME);
					if(!usedNames.contains(name)) {
						Utils.TypeFace((TextView) mParent.findViewById(mNameIDs[i]),
								getActivity().getAssets(),
								"fonts/merriweathersans-eb.ttf");
						((TextView) mParent.findViewById(mNameIDs[i++])).setText(name.toUpperCase());
					}
				}
			}
		}
		else {
			for(int i = 0; i < mNameIDs.length; i++) {
				((TextView) mParent.findViewById(mNameIDs[i])).setText("");
			}
		}
    	
    	ImageButton ib = (ImageButton) mParent.findViewById(R.id.imageButtonMain);
    	ib.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);

        		((MainActivity) getActivity()).replaceFragment(Constants.MenuID.UTC, false);
			}
		});
    	
        return mParent;
    }
    
    @Override
    public void onPause() {
    	super.onPause();
    	Logger.verbose(mName, "onPause()");
    	
    	((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setVisibility(ImageView.GONE);
    	DataManager.getInstance().cancelCheckInOrRedeemTask();
    }
	
	@Override
	public void onResume() {
		super.onResume();
		Logger.verbose(mName, "onResume()");

		// no longer using scanner
		/*
    	String scanData = DataManager.getInstance().getScanData();
    	if(scanData != null) {
    		Logger.verbose(mName, "got scan data " + scanData);

    		DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
    				scanData, Constants.Role.MEMBER.getValue(), -1, mCheckIn, mRedeem);
    	}
    	else {
    	*/

		// don't show image tips for now (or ever again?)
		//startImageSwitcherTask();
		
		boolean facebookEnabled = LoginManager.getInstance().getAccountContext().getFacebook();
		Logger.info(mName, "Facebook session open - " + String.valueOf(facebookEnabled));
		Logger.info(mName, "User logged in - " + String.valueOf(LoginManager.getInstance().userLoggedIn()));
		if(!facebookEnabled && LoginManager.getInstance().userLoggedIn()) {
			if(DataManager.getInstance().reminderLimitHit()) {
				Logger.info(mName, "showing reminder");
				DataManager.getInstance().resetReminder();
			    DialogFragment utcReminder = new UTCReminderDialogFragment();
			    utcReminder.show(getActivity().getSupportFragmentManager(),
			    		"UTCReminderDialogFragment");
			}
			else {
				Logger.info(mName, "incrementing reminder");
				DataManager.getInstance().incrementReminder();
			}
		}
		
    	if(DataManager.getInstance().getAnalyticsState()) {
    		Logger.verbose(mName, "starting Google analytics for this screen");
    		((MainActivity) getActivity()).sendView(mName);
    	}
    	//}
	}
	
    public void fragmentActive(boolean activeState) {
    	Logger.verbose(mName, "fragmentActive before - " + mFragmentActive);
    	if(activeState != mFragmentActive) {
        	mFragmentActive = activeState;
        	Logger.verbose(mName, "fragmentActive after - " + mFragmentActive);
    	}
    }
	
    
    public void startImageSwitcherTask() {
    	// cancel task to avoid potentially duplicated tasks
    	cancelImageSwitcherTask();
    	
		// restart images once end is reached
		if(mImageIndex >= mImageIds.length) {
			mImageIndex = 0;
		}
		
		MainActivity main = (MainActivity) getActivity();
		if(mImageIndex == mImageIds.length - 2) {
			if(main != null) {
				((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setVisibility(ImageView.GONE);
			}
			((ImageView) mParent.findViewById(R.id.imageViewTipQuick)).setVisibility(ImageView.VISIBLE);
			((ImageView) mParent.findViewById(R.id.imageViewTip)).setVisibility(ImageView.GONE);
			mImageIndex++;
		}
		else if(mImageIndex == mImageIds.length - 1) {
			if(LoginManager.getInstance().userLoggedIn() && mSwitchAccountImage) {
				((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setImageResource(mImageIdAccount);
				mSwitchAccountImage = false;
			}
			else if(mSwitchAccountImage) {
				((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setImageResource(mImageIds[mImageIndex]);
				mSwitchAccountImage = false;
			}
			if(main != null) {
				((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setVisibility(ImageView.VISIBLE);
			}
			((ImageView) mParent.findViewById(R.id.imageViewTipQuick)).setVisibility(ImageView.GONE);
			((ImageView) mParent.findViewById(R.id.imageViewTip)).setVisibility(ImageView.GONE);
			mImageIndex = 0;
		}
		else {
			if(main != null) {
				((ImageView) getActivity().findViewById(R.id.imageViewTipAccount)).setVisibility(ImageView.GONE);
			}
			((ImageView) mParent.findViewById(R.id.imageViewTipQuick)).setVisibility(ImageView.GONE);
			((ImageView) mParent.findViewById(R.id.imageViewTip)).setVisibility(ImageView.VISIBLE);
			// set the image we want to show
			((ImageView) mParent.findViewById(R.id.imageViewTip))
			.setImageResource(mImageIds[mImageIndex++]);
		}
		
    	if(mFragmentActive) {
    		mImageSwitcherTask = new ImageSwitcherTask();
			if(Utils.hasHoneycomb()) {
				mImageSwitcherTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mFragmentActive);
			}
			else {
				mImageSwitcherTask.execute(mFragmentActive);
			}
    	}
    }
    
    private class ImageSwitcherTask extends AsyncTask<Boolean, Void, Boolean> {
        protected Boolean doInBackground(Boolean... params) {
        	
        	long startTime = System.currentTimeMillis();
        	
        	while((System.currentTimeMillis() - startTime) < Constants.TIP_LENGTH) {
        		if(isCancelled() == true) {
        			break;
        		}
        	}

        	return params[0];
        }
        
        protected void onPostExecute(Boolean fragmentActive) {
        	startImageSwitcherTask();
        }
    }
    
    public boolean cancelImageSwitcherTask() {
    	boolean result = false;
    	if(mImageSwitcherTask != null) {
    		Logger.info(mName, "ImageSwitcherTask cancelled");
    		result = mImageSwitcherTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "ImageSwitcherTask was null when cancelling");
    	}
    	return result;
    }
    
    public void setUnifiedActionType(boolean checkIn, boolean redeem) {
    	mCheckIn = checkIn;
    	mRedeem = redeem;
    }
    
    private void showRedeemDialog() {
    	String qrdata = Constants.MEMBER_IDENTIFIER_URL
    			+ LoginManager.getInstance().getAccountContext().getAccountID().toString() 
    			+ "&h="
    			+ Utils.md5(LoginManager.getInstance().getAccountContext().getAccountGUID() 
    			+ "-"
    			+ Constants.MEMBER_HASH_KEY);
    	
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("UnifiedRedeemDialog", -1, -1, qrdata, "", null, "", true);
    	mUTCDialog.show(getFragmentManager(), "UnifiedRedeemDialog");
    }
    
    public void showCheckInConfirmDialog(int locationID, int businessID, String description, String error) {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("CheckInConfirmDialog", locationID, businessID, error, null, description, "", error == null);
    	mUTCDialog.show(getFragmentManager(), "CheckInConfirmDialog");
    }
    
    public void showRedeemConfirmDialog(int locationID, int businessID, String description, String error) {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("RedeemConfirmDialog", locationID, businessID, error, null, description, "", error == null);
    	mUTCDialog.show(getFragmentManager(), "RedeemConfirmDialog");
    }
    
    public void showUnifiedActionConfirmDialog(int locationID, int businessID, boolean checkedIn, boolean redeemed, String dealAmount, 
    		String busName, String description, String error) {
        // Create and show the dialog
    	if(redeemed) {
        	mUTCDialog = UTCDialogFragment.newInstance("RedeemConfirmDialog", locationID, businessID, dealAmount, busName, description, dealAmount, error == null);
        	mUTCDialog.show(getFragmentManager(), "RedeemConfirmDialog");
    	}
    	else if(checkedIn) {
        	mUTCDialog = UTCDialogFragment.newInstance("CheckInConfirmDialog", locationID, businessID, error, busName, description, dealAmount, error == null);
        	mUTCDialog.show(getFragmentManager(), "CheckInConfirmDialog");
    	}
    }
    
    public boolean redeemClicked() {
    	return mRedeem;
    }
    
    public boolean checkInClicked() {
    	return mCheckIn;
    }
    
    public void showPINDialog() {
        // Create and show the dialog
    	DataManager.getInstance().setRequestingPIN(true);
    	mUTCDialog = UTCDialogFragment.newInstance("PINDialog");
    	mUTCDialog.setUnifiedSettings(true, mCheckIn, mRedeem);
    	mUTCDialog.show(getFragmentManager(), "PINDialog");
    }
    
    public void dismissPINDialog() {
    	if(mUTCDialog != null) {
    		mUTCDialog.dismiss();
    	}
    }
    
    public void dismissRedeemDialog() {
    	if(mUTCDialog != null) {
    		mUTCDialog.dismiss();
    	}
    }
    
    private void showGuestRedeemDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestRedeemDialog");
    	mUTCDialog.show(getFragmentManager(), "GuestRedeemDialog");
    }
    
    public void switchAccountImage() {
    	mSwitchAccountImage = true;
    }
}