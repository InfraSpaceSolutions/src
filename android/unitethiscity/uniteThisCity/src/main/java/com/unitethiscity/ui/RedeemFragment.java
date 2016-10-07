package com.unitethiscity.ui;

import java.text.NumberFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.TimeZone;
import java.util.TreeMap;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.SystemClock;
import android.os.Vibrator;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.text.Editable;
import android.text.TextWatcher;
import android.text.format.Time;
import android.util.TypedValue;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.webkit.WebView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.ScrollView;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.EventContextParser;
import com.unitethiscity.data.EventsParser;
import com.unitethiscity.data.FavoritesParser;
import com.unitethiscity.data.LocationContextParser;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCEvent;
import com.unitethiscity.data.UTCGeolocationManager;
import com.unitethiscity.data.UTCLocation;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class RedeemFragment extends Fragment implements RateDialog.RateDialogListener {
	
	private String mName = Constants.REDEEM_FRAGMENT;
	
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.REDEEM;
	private Constants.MenuID mParentID;
	
	private View mParent;
	
	private String mLocationRetrievalError;
	
	private boolean mFragmentActive = false;

	private boolean mMoreInfoLayoutOpen;
	private boolean mReviewLayoutOpen;
	
	private String mReviewString;
	
	private long mTimestamp;
	
	private UTCDialogFragment mUTCDialog;
	private RedeemRewardDialog mRewardDialog;
	private LoyaltyDialog mLoyaltyDialog;

	private AsyncTask<String, Void, ImageView> mAddImageTask;
	private AsyncTask<Integer, Void, Integer> mLoadLocationTask;
	private AsyncTask<Integer, Void, Boolean> mToggleFavoriteTask;
	private AsyncTask<Float, Void, Integer> mRateItTask;
	private AsyncTask<String, Void, Boolean> mReviewTask;
	private AsyncTask<Void, Void, Boolean> mGalleryTask;
	private AsyncTask<Void, Void, Boolean> mMenuTask;
	private AsyncTask<Void, View, Integer> mLoadEventsTask;
	
	private Integer mLocationID;
	private String mDistance;
	private String mBusinessName;
	private float mRating;
	
	private JSONObject mJSONReply;
	
	private String mDealAmount;

	private String mEventRetrievalError = null;
	private int mEventProgress;
	private String mSearchFilter = "";

	private HashMap<Integer, GalleryItem> mGalleryItems;
	private HashMap<Integer, MenuItem> mMenuItems;
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_redeem, container, false);
        return mParent;
    }

	@Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        if(!mFragmentActive) return;
        if(isAdded()) ((MainActivity) getActivity()).showSpinner();
        clearContents();

		mMoreInfoLayoutOpen = false;
        mReviewLayoutOpen = false;
        
        mTimestamp = SystemClock.elapsedRealtime();

		mBusinessName = getArguments().getString(LocationParser.JSON_TAG_NAME);
        String guid = getArguments().getString(LocationParser.JSON_TAG_BUSGUID);
        mLocationID = Integer.valueOf(getArguments().getString(LocationParser.JSON_TAG_ID));
        mDistance = getArguments().getString(LocationParser.JSON_TAG_DISTANCE);
        
		String url = Constants.LOCATION_INFO_IMAGE + "/" + guid + "@2x.png";
		
		loadLocation();

		ImageView location = (ImageView) mParent.findViewById(R.id.imageViewContextLocation);
		addImage(url, location);
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	
    	mFragmentActive = true;
    	
    	if(DataManager.getInstance().getAnalyticsState()) {
    		Logger.verbose(mName, "starting Google analytics for this screen");
    		((MainActivity) getActivity()).sendView(mName);
    	}
    }
    
    @Override
    public void onPause() {
    	super.onPause();
    	
    	cancelAllTasks();
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
    
    public void setParent(Constants.MenuID fID) {
    	mParentID = fID;
    }
    
    public Constants.MenuID getParent() {
    	return mParentID;
    }
    
	public void replaceSubmenuFragment(Constants.MenuID fID, Bundle args, boolean goBack)
	{
		final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
		// Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
		vib.vibrate(Constants.VIBRATE_LENGTH);
		
		FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

		mFragmentActive = false;
		
		// Replace whatever is in the frameLayout view with this fragment
	    switch (fID) {
			case WEB:
				((MainActivity) getActivity()).setFragmentID(fID);
				((MainActivity) getActivity()).setParentFragmentID(mMenuID);
				WebFragment wf = DataManager.getInstance().getWebFragment();
				wf.setArguments(args);
				wf.setParent(mMenuID);
				wf.fragmentActive(true);
				transaction.add(R.id.frameLayoutMiddle, wf, fID.toString());

				// add transaction to back stack if we want to go back to where we were
				if(goBack) {
					DataManager.getInstance().pushToMenuStack(fID);
				}

				// Commit the transaction
				transaction.commit();

				break;
			case EVENT:
				((MainActivity) getActivity()).setFragmentID(fID);
				((MainActivity) getActivity()).setParentFragmentID(mMenuID);
				EventFragment ef = DataManager.getInstance().getEventFragment();
				ef.setArguments(args);
				ef.setParent(mMenuID);
				ef.fragmentActive(true);
				transaction.add(R.id.frameLayoutMiddle, ef, fID.toString());

				// add transaction to back stack if we want to go back to where we were
				if(goBack) {
					DataManager.getInstance().pushToMenuStack(fID);
				}

				// Commit the transaction
				transaction.commit();
				break;
			default:
				break;
	    }
	}
    
    public void cancelAllTasks() {
		cancelAddImageTask();
		cancelLoadLocationTask();
	    cancelToggleFavoriteTask();
	    cancelRateItTask();
	    cancelReviewTask();
		cancelGalleryTask();
		cancelMenuTask();
    }
    
    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
	public void loadLocation() {
		mLoadLocationTask = new LoadLocationTask();
        if(Utils.hasHoneycomb()) {
        	mLoadLocationTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mLocationID);
        }
        else {
        	mLoadLocationTask.execute(mLocationID);
        }
    }

	public void updateRedemptionAndLoyaltyStatus() {

		final UTCLocation utcLoc = DataManager.getInstance().getLocationContext();
		TextView tv;

		final MainActivity main = (MainActivity)getActivity();

		if(main != null) {
			LinearLayout socialDeal = (LinearLayout) main.findViewById(R.id.linearLayoutContextUTCSocialDeal);

			if (utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_IS_REDEEMED)) {
				if (Boolean.parseBoolean(utcLoc.get(LocationContextParser.JSON_TAG_MY_IS_REDEEMED))) {
					if (utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_REDEEM_DATE)) {
						String date = utcLoc.get(LocationContextParser.JSON_TAG_MY_REDEEM_DATE);
						Button redeemDate = (Button) main.findViewById(R.id.buttonContextRedemptionRedeemedDate);
						redeemDate.setText(date);
					}

					socialDeal.setVisibility(LinearLayout.INVISIBLE);
					socialDeal = (LinearLayout) main.findViewById(R.id.linearLayoutContextUTCSocialDealRedeemed);
					socialDeal.setVisibility(LinearLayout.VISIBLE);
				} else {
					socialDeal.setVisibility(LinearLayout.VISIBLE);
					socialDeal = (LinearLayout) main.findViewById(R.id.linearLayoutContextUTCSocialDealRedeemed);
					socialDeal.setVisibility(LinearLayout.INVISIBLE);
				}
			}

			tv = (TextView) main.findViewById(R.id.contextLoyaltyDescription);
			if (utcLoc.containsKey(LocationContextParser.JSON_TAG_LOYALTY_SUMMARY)) {
				tv.setText(utcLoc.get(LocationContextParser.JSON_TAG_LOYALTY_SUMMARY));
			}

			Button btn = (Button) main.findViewById(R.id.buttonContextLoyaltyStatus);
			if (utcLoc.containsKey(LocationContextParser.JSON_TAG_POINTS_NEEDED) &&
					utcLoc.containsKey(LocationContextParser.JSON_TAG_POINTS_COLLECTED)) {
				int needed = Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_POINTS_NEEDED));
				int collected = Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_POINTS_COLLECTED));
				if(collected >= needed) {
					btn.setText("LOYALTY REWARDED");
				}
				else {
					btn.setText(String.valueOf(needed - collected) + " POINTS NEEDED");
				}
			}

			btn = (Button) main.findViewById(R.id.buttonContextEarnPoint);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN) &&
					Boolean.parseBoolean(utcLoc.get(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN))) {
				btn.setText("EARNED");
				btn.setBackgroundColor(Color.parseColor("#616161"));
				btn.setTextColor(Color.parseColor("#BDBDBD"));
			}
		}
	}

	public void showLoyaltyDialog(int locationID, int businessID, String busName, String busImage, String error) {
		// create and show loyalty dialog
		mLoyaltyDialog = LoyaltyDialog.newInstance(busName, busImage, String.valueOf(locationID), String.valueOf(businessID), error);
		mLoyaltyDialog.show(getFragmentManager());
	}

	private void showRedeemRewardDialog(int locationID, String dealAmount, String dealDesc, int businessID, String busName, String busImage, String error) {
		Logger.info(mName, "Show redeem reward dialog: " + busName + ", " + dealAmount);

		// create and show redeem reward dialog
		mRewardDialog = RedeemRewardDialog.newInstance(busName, dealAmount, dealDesc, busImage, String.valueOf(locationID), String.valueOf(businessID), error);
		mRewardDialog.show(getFragmentManager());
	}

	public void permissionResultRedeem(int result) {
		if(result == PackageManager.PERMISSION_GRANTED && mRewardDialog != null) {
			mRewardDialog.successPermissionWriteExternalStorage();
		}
	}

	public void permissionResultLoyalty(int result) {
		if(result == PackageManager.PERMISSION_GRANTED && mRewardDialog != null) {
			mLoyaltyDialog.successPermissionWriteExternalStorage();
		}
	}

	public void updateReedeemRewardDialog(String errorMessage) {
		if(mRewardDialog != null) {
			if (errorMessage == null) {
				mRewardDialog.updateSuccess();
			}
			else {
				mRewardDialog.updateFailure(errorMessage);
			}
		}
	}

    private void showRedeemDialog(String busName, String dealAmount) {
    	String qrdata = Constants.MEMBER_IDENTIFIER_URL
    			+ LoginManager.getInstance().getAccountContext().getAccountID().toString() 
    			+ "&h="
    			+ Utils.md5(LoginManager.getInstance().getAccountContext().getAccountGUID() 
    			+ "-"
    			+ Constants.MEMBER_HASH_KEY);
    	
    	Logger.info(mName, "Show redeem dialog: " + busName + ", " + dealAmount);
    	
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("RedeemDialog", -1, -1, qrdata, busName, null, dealAmount, true);
    	mUTCDialog.show(getFragmentManager(), "RedeemDialog");
    }
    
    public void showCheckInConfirmDialog(int locationID, int businessID, String busName, String busImage, String description, String error) {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("CheckInConfirmDialog", locationID, businessID, busImage, error, busName, description, "", error == null);
    	mUTCDialog.show(getFragmentManager(), "CheckInConfirmDialog");
    }
    
    public void showRedeemConfirmDialog(int locationID, int businessID, String busName, String busImage, String description, String error) {
        // Create and show the dialog
    	String data = "";
		if(DataManager.getInstance()
				.getLocationContext()
				.containsKey(LocationContextParser.JSON_TAG_DEAL_AMOUNT)
				&& error == null) {
			data = DataManager.getInstance()
					.getLocationContext()
					.get(LocationContextParser.JSON_TAG_DEAL_AMOUNT);
		}
		else {
			data = error;
		}

    	mUTCDialog = UTCDialogFragment.newInstance("RedeemConfirmDialog", locationID, businessID, busImage, data, busName, description, "", error == null);
    	mUTCDialog.show(getFragmentManager(), "RedeemConfirmDialog");
    }
    
    private void showGuestRedeemDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestRedeemDialog");
    	mUTCDialog.show(getFragmentManager(), "GuestRedeemDialog");
    }
    
    private void showGuestReviewDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestReviewDialog");
    	mUTCDialog.show(getFragmentManager(), "GuestReviewDialog");
    }
    
    private void showGuestRatingDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestRateDialog");
    	mUTCDialog.show(getFragmentManager(), "GuestRateDialog");
    }
    
    private void showGuestFavoriteDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("GuestFavoriteDialog");
    	mUTCDialog.show(getFragmentManager(), "GuestFavoriteDialog");
    }
    
    public void showPINDialog() {
        // Create and show the dialog
    	DataManager.getInstance().setRequestingPIN(true);
    	mUTCDialog = UTCDialogFragment.newInstance("PINDialog");
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

	@Override
	public void onFinishRateDialog(int result, float rating) {
		if(isAdded()) ((MainActivity) getActivity()).showSpinner();
		mRateItTask = new RateItTask();
		if(Utils.hasHoneycomb()) {
			mRateItTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, rating);
		}
		else {
			mRateItTask.execute(rating);
		}
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

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	private void addImageSerial(String url, ImageView iv) {
		AddImageTask ait = new AddImageTask(iv);
		if(Utils.hasHoneycomb()) {
			ait.executeOnExecutor(AsyncTask.SERIAL_EXECUTOR, url);
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
    
    public boolean cancelAddImageTask() {
    	boolean result = false;
    	if(mAddImageTask != null) {
    		result = mAddImageTask.cancel(true);
    	}
    	return result;
    }
    
    private class LoadLocationTask extends AsyncTask<Integer, Void, Integer> {
        protected Integer doInBackground(Integer... params) {
            UTCGeolocationManager lm = UTCGeolocationManager.getInstance();

        	if((MainActivity) getActivity() == null) {
        		Logger.verbose(mName, "main == null in LoadLocationsTask");
				mLocationRetrievalError = "An error occurred when retrieving " +
						"location information";
        		return 0;
        	}
        	
        	// make sure we can actually get a location (user must allow us)
        	if(lm.areProvidersOff()) {
        		Logger.info(mName, "lm.areProvidersOff == true in LoadLocationTask");
        	}
        	
        	// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadLocationTask");
				mLocationRetrievalError = "An error occurred when retrieving " +
						"location information";
				return 0;
			}
    		
			if(LoginManager.getInstance().userLoggedIn()) {
				mLocationRetrievalError = LocationContextParser.setLocationContext(
						params[0], 
						LoginManager.getInstance().getAccountContext().getToken());
			}
			else {
				mLocationRetrievalError = LocationContextParser.setLocationContext(
						params[0], null);
			}
    		
    		return null;
        }
        
        @SuppressWarnings("deprecation")
		@SuppressLint("NewApi")
		protected void onPostExecute(Integer error) {
        	
        	if(mLocationRetrievalError != null) {
        		Toast.makeText(mParent.getContext(), mLocationRetrievalError, Toast.LENGTH_SHORT).show();
        		return;
        	}
        	
            DataManager dm = DataManager.getInstance();
            
    		final UTCLocation utcLoc = dm.getLocationContext();
    		
    		TextView tv;
    		
    		tv = (TextView) mParent.findViewById(R.id.contextLocationName);
    		if(utcLoc.containsKey(LocationParser.JSON_TAG_NAME)) {
    			tv.setText(utcLoc.get(LocationParser.JSON_TAG_NAME));
				tv = (TextView) mParent.findViewById(R.id.textViewMoreInfoBusinessName);
				tv.setText(utcLoc.get(LocationParser.JSON_TAG_NAME));
				tv = (TextView) mParent.findViewById(R.id.contextLocationNameReview);
				tv.setText(utcLoc.get(LocationParser.JSON_TAG_NAME));
				mSearchFilter = utcLoc.get(LocationParser.JSON_TAG_NAME);
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.contextDistance);
    		double dist = Double.parseDouble(mDistance);
    		if(dist < Double.parseDouble(Constants.LOCATION_MISSING_TAG)) {
    			tv.setText(mDistance + " mi");
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.contextAddress);
    		if(utcLoc.containsKey(LocationParser.JSON_TAG_ADDRESS)) {
    			tv.setText(utcLoc.get(LocationParser.JSON_TAG_ADDRESS));
    		}
    		
    		ImageView iv;
    		iv = (ImageView) mParent.findViewById(R.id.contextRating);
    		if(utcLoc.containsKey(LocationParser.JSON_TAG_RATING)) {
    			// get location rating number parsed as an int, with Drawable ID looked up by ratingIds
    			if(isAdded()) {
        			Drawable rating = getResources().getDrawable(
							Constants.ratingIds[Integer.parseInt(
									utcLoc.get(LocationContextParser.JSON_TAG_RATING))]
					);
        			iv.setImageDrawable(rating);
    			}
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.contextCategory);
			if(utcLoc.containsKey(LocationParser.JSON_TAG_CATNAME)) {
				String category = utcLoc.get(LocationParser.JSON_TAG_CATNAME);
				if(utcLoc.containsKey(LocationParser.JSON_TAG_PROPERTIES + "Size")) {
		            int propertiesNum = Integer.valueOf(utcLoc.get(LocationParser.JSON_TAG_PROPERTIES + "Size")).intValue();
		            for(int j = 0; j < propertiesNum; j++) {
		            	category = category + ", " + utcLoc.get(LocationParser.JSON_TAG_PROPERTIES + String.valueOf(j));
		            }
				}
				tv.setText(category.toUpperCase(Locale.getDefault()));
			}

    		tv = (TextView) mParent.findViewById(R.id.contextDescription);
    		if(utcLoc.containsKey(LocationContextParser.JSON_TAG_SUMMARY)) {
    			tv.setText(utcLoc.get(LocationContextParser.JSON_TAG_SUMMARY));
    		}

			tv = (TextView) mParent.findViewById(R.id.contextDistance);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_DISTANCE)) {
				tv.setText(utcLoc.get(LocationContextParser.JSON_TAG_DISTANCE));
			}

			LinearLayout socialDeal = (LinearLayout) getActivity().findViewById(R.id.linearLayoutContextUTCSocialDeal);

			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_IS_REDEEMED)) {
				if(Boolean.parseBoolean(utcLoc.get(LocationContextParser.JSON_TAG_MY_IS_REDEEMED))) {
					if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_REDEEM_DATE)) {
						String date = utcLoc.get(LocationContextParser.JSON_TAG_MY_REDEEM_DATE);
						Button redeemDate = (Button) getActivity().findViewById(R.id.buttonContextRedemptionRedeemedDate);
						redeemDate.setText(date);
					}

					socialDeal.setVisibility(LinearLayout.INVISIBLE);
					socialDeal = (LinearLayout) getActivity().findViewById(R.id.linearLayoutContextUTCSocialDealRedeemed);
					socialDeal.setVisibility(LinearLayout.VISIBLE);
				}
				else {
					socialDeal.setVisibility(LinearLayout.VISIBLE);
					socialDeal = (LinearLayout) getActivity().findViewById(R.id.linearLayoutContextUTCSocialDealRedeemed);
					socialDeal.setVisibility(LinearLayout.INVISIBLE);

					Button redeem = (Button) getActivity().findViewById(R.id.buttonContextRedeem);
					redeem.setOnClickListener(new View.OnClickListener() {
						public void onClick(View v) {
							// only perform action after the appropriate dwell time has occurred
							if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
								return;
							}

							mTimestamp = SystemClock.elapsedRealtime();

							final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
							// Vibrate for Constants.VIBRATE_LENGTH milliseconds
							vib.vibrate(Constants.VIBRATE_LENGTH);

							// Perform action on click
							if(LoginManager.getInstance().userLoggedIn()) {
								String busID = utcLoc.get(LocationContextParser.JSON_TAG_BUSID);
								String image = Constants.LOCATION_INFO_IMAGE + "/" + utcLoc.get(LocationContextParser.JSON_TAG_BUSGUID) + "@2x.png";
								String desc = utcLoc.get(LocationContextParser.JSON_TAG_DEAL_DESCRIPTION);

								// show member dialog
								showRedeemRewardDialog(Integer.valueOf(mLocationID), mDealAmount, desc, Integer.valueOf(busID), mBusinessName, image, "");
							}
							else {
								// show nonmember dialog
								showGuestRedeemDialog();
							}
						}
					});

					tv = (TextView) mParent.findViewById(R.id.contextAvailableCash);
					if(utcLoc.containsKey(LocationContextParser.JSON_TAG_DEAL_AMOUNT)) {
						mDealAmount = utcLoc.get(LocationContextParser.JSON_TAG_DEAL_AMOUNT);
						// scale available cash text view text size based on the amount of
						// characters in the deal amount string
						tv.setTextSize(TypedValue.COMPLEX_UNIT_PX, tv.getTextSize() - 2*mDealAmount.length());
						tv.setText(mDealAmount);
					}
				}
			}

			tv = (TextView) mParent.findViewById(R.id.contextLoyaltyDescription);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_LOYALTY_SUMMARY)) {
				tv.setText(utcLoc.get(LocationContextParser.JSON_TAG_LOYALTY_SUMMARY));
			}

			Button btn = (Button) mParent.findViewById(R.id.buttonContextLoyaltyStatus);
			if (utcLoc.containsKey(LocationContextParser.JSON_TAG_POINTS_NEEDED) &&
					utcLoc.containsKey(LocationContextParser.JSON_TAG_POINTS_COLLECTED)) {
				int needed = Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_POINTS_NEEDED));
				int collected = Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_POINTS_COLLECTED));
				if(collected >= needed) {
					btn.setText("LOYALTY REWARDED");
				}
				else {
					btn.setText(String.valueOf(needed - collected) + " POINTS NEEDED");
				}
			}

			btn = (Button) mParent.findViewById(R.id.buttonContextEarnPoint);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN) &&
					Boolean.parseBoolean(utcLoc.get(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN))) {
				btn.setText("EARNED");
				btn.setBackgroundColor(Color.parseColor("#616161"));
				btn.setTextColor(Color.parseColor("#BDBDBD"));
			}

			btn = (Button) mParent.findViewById(R.id.buttonContextEarnPoint);
			btn.setOnClickListener(new View.OnClickListener() {
				@Override
				public void onClick(View v) {

					DataManager.getInstance().setScanTask(DataManager.ScanTask.CHECKIN);
					DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
							null, Constants.Role.MEMBER.getValue(), -1, true, false);
				}
			});
    		
    		final String socialTermsURL = Constants.LOCATION_TERMS_SOCIAL_DEAL_URL + utcLoc.get(LocationContextParser.JSON_TAG_ID);
    		tv = (TextView) mParent.findViewById(R.id.contextTermsAndConditionsSocial);
			tv.setOnClickListener(new View.OnClickListener() {

				@Override
				public void onClick(View v) {
					// only perform action after the appropriate dwell time has occurred
					if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
						return;
					}

					mTimestamp = SystemClock.elapsedRealtime();

					final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
					// Vibrate for Constants.VIBRATE_LENGTH milliseconds
					vib.vibrate(Constants.VIBRATE_LENGTH);

					Bundle args = new Bundle();
					args.putString(Constants.WEB_FRAGMENT_URL_ARG, socialTermsURL);
					args.putString(Constants.WEB_FRAGMENT_TITLE, "UTC SOCIAL DEAL");
					DataManager.getInstance().getWebFragment().setParent(Constants.MenuID.REDEEM);
					replaceSubmenuFragment(Constants.MenuID.WEB, args, true);
				}
			});
			final String loyaltyTermsURL = Constants.LOCATION_TERMS_LOYALTY_DEAL_URL + utcLoc.get(LocationContextParser.JSON_TAG_ID);
			tv = (TextView) mParent.findViewById(R.id.contextTermsAndConditionsLoyalty);
			tv.setOnClickListener(new View.OnClickListener() {

				@Override
				public void onClick(View v) {
					// only perform action after the appropriate dwell time has occurred
					if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
						return;
					}

					mTimestamp = SystemClock.elapsedRealtime();

					final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
					// Vibrate for Constants.VIBRATE_LENGTH milliseconds
					vib.vibrate(Constants.VIBRATE_LENGTH);

					Bundle args = new Bundle();
					args.putString(Constants.WEB_FRAGMENT_URL_ARG, loyaltyTermsURL);
					args.putString(Constants.WEB_FRAGMENT_TITLE, "UTC LOYALTY DEAL");
					DataManager.getInstance().getWebFragment().setParent(Constants.MenuID.REDEEM);
					replaceSubmenuFragment(Constants.MenuID.WEB, args, true);
				}
			});
    		
    		// add image view touch listeners
    		String address = utcLoc.get(LocationContextParser.JSON_TAG_ADDRESS);
    		ImageButton ib = (ImageButton) mParent.findViewById(R.id.imageButtonMaps);
    		if(address != null) {
    			if(!address.equals("")) {
    				final String addressURL = "geo:0,0?q=" + address.replace(" ", "+");
    	    		ib.setOnClickListener(new View.OnClickListener() {
    	    			
    					@Override
    					public void onClick(View v) {
    						// only perform action after the appropriate dwell time has occurred
    						if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
    							return;
    						}
    						
    						mTimestamp = SystemClock.elapsedRealtime();
    						
    		    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    		    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    		    			vib.vibrate(Constants.VIBRATE_LENGTH);
    		    			
    	    				Uri url = Uri.parse(addressURL);
    	    				Intent launchMaps = new Intent(Intent.ACTION_VIEW, url);
    	    				try {
    	    					startActivity(launchMaps);
    	    				}
    	    				catch(ActivityNotFoundException anfe) {
    	    					anfe.printStackTrace();
    	    				}
    	    			}
    	    		});
    			}
    			else {
        			// setImageAlpha for API level 16 and above - anything below needs setAlpha
    				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_maps));
        			if(Utils.hasJellyBean()) {
        				ib.setImageAlpha(80);
        			}
        			else {
        				ib.setAlpha(80);
        			}
    			}
    		}
    		else {
    			// setImageAlpha for API level 16 and above - anything below needs setAlpha
				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_maps));
    			if(Utils.hasJellyBean()) {
    				ib.setImageAlpha(80);
    			}
    			else {
    				ib.setAlpha(80);
    			}
    		}
    		
    		final String facebookID = utcLoc.get(LocationContextParser.JSON_TAG_FACEBOOK_ID);
    		ib = (ImageButton) mParent.findViewById(R.id.imageButtonFacebook);
    		if(facebookID != null) {
    			if(!facebookID.equals("")) {
    	    		ib.setOnClickListener(new View.OnClickListener() {
    	    			
    					@Override
    					public void onClick(View v) {
    						// only perform action after the appropriate dwell time has occurred
    						if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
    							return;
    						}
    						
    						mTimestamp = SystemClock.elapsedRealtime();
    						
    		    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    		    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    		    			vib.vibrate(Constants.VIBRATE_LENGTH);
    		    			
    	    				Uri url = Uri.parse("fb://page/" + facebookID);
    	    				Intent launchFB = new Intent(Intent.ACTION_VIEW, url);
    	    				try {
    	    					startActivity(launchFB);
    	    				}
    	    				catch(ActivityNotFoundException anfe) {
        	    				url = Uri.parse("http://touch.facebook.com/pages/x/" + facebookID);
        	    				Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
        	    				try {
        	    					startActivity(launchBrowser);
        	    				}
        	    				catch(ActivityNotFoundException anfe2) {
        	    					anfe2.printStackTrace();
        	    				}
    	    				}
    	    			}
    	    		});
    			}
    			else {
        			// setImageAlpha for API level 16 and above - anything below needs setAlpha
    				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_facebook));
        			if(Utils.hasJellyBean()) {
        				ib.setImageAlpha(80);
        			}
        			else {
        				ib.setAlpha(80);
        			}
    			}
    		}
    		else {
    			// setImageAlpha for API level 16 and above - anything below needs setAlpha
				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_facebook));
    			if(Utils.hasJellyBean()) {
    				ib.setImageAlpha(80);
    			}
    			else {
    				ib.setAlpha(80);
    			}
    		}
    		
    		String phone = utcLoc.get(LocationContextParser.JSON_TAG_PHONE);
    		ib = (ImageButton) mParent.findViewById(R.id.imageButtonPhone);
    		if(phone != null) {
    			if(!phone.equals("")) {
    				final String phoneURL = "tel:" + phone;
    	    		ib.setOnClickListener(new View.OnClickListener() {
    	    			
    					@Override
    					public void onClick(View v) {
    						// only perform action after the appropriate dwell time has occurred
    						if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
    							return;
    						}
    						
    						mTimestamp = SystemClock.elapsedRealtime();
    						
    		    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    		    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    		    			vib.vibrate(Constants.VIBRATE_LENGTH);
    		    			
    	    				Uri url = Uri.parse(phoneURL);
    	    				Intent launchPhone = new Intent(Intent.ACTION_DIAL, url);
    	    				try {
    	    					startActivity(launchPhone);
    	    				}
    	    				catch(ActivityNotFoundException anfe) {
    	    					Toast.makeText(getActivity(), 
    	    							"Device does not support dialing", Toast.LENGTH_SHORT).show();
    	    				}
    	    			}
    	    		});
    			}
    			else {
        			// setImageAlpha for API level 16 and above - anything below needs setAlpha
    				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_phone));
        			if(Utils.hasJellyBean()) {
        				ib.setImageAlpha(80);
        			}
        			else {
        				ib.setAlpha(80);
        			}
    			}
    		}
    		else {
    			// setImageAlpha for API level 16 and above - anything below needs setAlpha
				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_phone));
    			if(Utils.hasJellyBean()) {
    				ib.setImageAlpha(80);
    			}
    			else {
    				ib.setAlpha(80);
    			}
    		}
    		
    		final String websiteURL = utcLoc.get(LocationContextParser.JSON_TAG_WEBSITE);
    		ib = (ImageButton) mParent.findViewById(R.id.imageButtonWebsite);
    		if(websiteURL != null) {
    			if(!websiteURL.equals("")) {
    	    		ib.setOnClickListener(new View.OnClickListener() {
    	    			
    					@Override
    					public void onClick(View v) {
    						// only perform action after the appropriate dwell time has occurred
    						if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
    							return;
    						}
    						
    						mTimestamp = SystemClock.elapsedRealtime();
    						
    		    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    		    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    		    			vib.vibrate(Constants.VIBRATE_LENGTH);
    		    			
    	    				Uri url = Uri.parse(websiteURL);
    	    				Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
    	    				try {
    	    					startActivity(launchBrowser);
    	    				}
    	    				catch(ActivityNotFoundException anfe) {
    	    					anfe.printStackTrace();
    	    				}
    	    			}
    	    		});
    			}
    			else {
        			// setImageAlpha for API level 16 and above - anything below needs setAlpha
    				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_website));
        			if(Utils.hasJellyBean()) {
        				ib.setImageAlpha(80);
        			}
        			else {
        				ib.setAlpha(80);
        			}
    			}
    		}
    		else {
    			// setImageAlpha for API level 16 and above - anything below needs setAlpha
				ib.setImageDrawable(getActivity().getResources().getDrawable(R.drawable.btn_website));
    			if(Utils.hasJellyBean()) {
    				ib.setImageAlpha(80);
    			}
    			else {
    				ib.setAlpha(80);
    			}
    		}

    		// adjust favorites image
    		Bundle args = new Bundle();
    		args.putInt(LocationContextParser.JSON_TAG_ID, Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_ID)));
    		iv = (ImageView) mParent.findViewById(R.id.imageViewToggleFavorites);
    		if(Boolean.parseBoolean(utcLoc.get(LocationContextParser.JSON_TAG_MY_IS_FAVORITE))) {
    			if(isAdded() && dm.getFavorites().contains(
    					Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_ID)))
    					) {
        			Drawable rating = getResources().getDrawable(R.drawable.btn_remove_from_favorites);
        			iv.setImageDrawable(rating);
    			}
    		}
    		iv.setOnTouchListener(new FavoritesTouchListener(args));

    		iv = (ImageView) mParent.findViewById(R.id.imageViewRateIt);
    		if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MY_RATING)) {
				mRating = Float.parseFloat(utcLoc.get(LocationContextParser.JSON_TAG_MY_RATING)) / 2;
    		}
    		iv.setOnTouchListener(new RateTouchListener());
    		
    		args.clear();
            
            EditText reviewEdit = (EditText) getActivity().findViewById(R.id.editTextReview);
            reviewEdit.addTextChangedListener(new TextWatcher() {

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
					mReviewString = s.toString();
				}

			});
            
            final Button postReviewButton = (Button) mParent.findViewById(R.id.buttonReviewPost);
            postReviewButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					// only perform action after the appropriate dwell time has occurred
					if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
						return;
					}
					
					mTimestamp = SystemClock.elapsedRealtime();
					
					String review = "";
					if(DataManager.getInstance().getLocationContext().containsKey(LocationContextParser.JSON_TAG_MY_TIP)) {
						review = DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_MY_TIP);
					}
					
					if(review.compareTo(mReviewString) == 0) {
						return;
					}
					
					if(isAdded()) ((MainActivity) getActivity()).showSpinner();
					mReviewTask = new ReviewTask();
			        if(Utils.hasHoneycomb()) {
			        	mReviewTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mReviewString);
			        }
			        else {
			        	mReviewTask.execute(mReviewString);
			        }
				}
			});


			int numOfItems = 0;
			int numGalleryItems = 0;
			int numMenuItems = 0;
			int numEventItems = 0;

			TextView gallery = (TextView)mParent.findViewById(R.id.contextMoreInfoButtonGallery);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_NUM_GALLERY_ITEMS)) {
				numGalleryItems = Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_NUM_GALLERY_ITEMS));
				numOfItems += numGalleryItems;

				if(numGalleryItems == 0) {
					gallery.setVisibility(TextView.GONE);
				}
				else if(numGalleryItems > 0) {
					Logger.info(mName, "business contains gallery");
					gallery.setClickable(true);
					gallery.setOnClickListener(new View.OnClickListener() {
						@Override
						public void onClick(View v) {

							TextView gButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonGallery);
							TextView mButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonMenu);
							TextView cButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonCalendar);
							LinearLayout gItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutGalleryItems);
							RelativeLayout mItemsLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutMenuItems);
							LinearLayout cItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutCalendarItems);

							activeButton(gButton);
							inactiveButton(mButton);
							inactiveButton(cButton);

							gItemsLayout.setVisibility(LinearLayout.VISIBLE);
							mItemsLayout.setVisibility(RelativeLayout.GONE);
							cItemsLayout.setVisibility(LinearLayout.GONE);
						}
					});

					mGalleryTask = new GalleryTask(Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_BUSID)));
					if(Utils.hasHoneycomb()) {
						mGalleryTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
					}
					else {
						mGalleryTask.execute();
					}
				}
			}
			else {
				gallery.setVisibility(TextView.GONE);
			}
			TextView menu = (TextView)mParent.findViewById(R.id.contextMoreInfoButtonMenu);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_NUM_MENU_ITEMS)) {
				numMenuItems = Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_NUM_MENU_ITEMS));
				numOfItems += numMenuItems;

				if(numMenuItems == 0) {
					menu.setVisibility(TextView.GONE);
				}
				else if(numMenuItems > 0) {
					Logger.info(mName, "business contains menu");
					menu.setClickable(true);
					menu.setOnClickListener(new View.OnClickListener() {
						@Override
						public void onClick(View v) {

							TextView gButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonGallery);
							TextView mButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonMenu);
							TextView cButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonCalendar);
							LinearLayout gItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutGalleryItems);
							RelativeLayout mItemsLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutMenuItems);
							LinearLayout cItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutCalendarItems);

							inactiveButton(gButton);
							activeButton(mButton);
							inactiveButton(cButton);

							gItemsLayout.setVisibility(LinearLayout.GONE);
							mItemsLayout.setVisibility(RelativeLayout.VISIBLE);
							cItemsLayout.setVisibility(LinearLayout.GONE);
						}
					});

					mMenuTask = new MenuTask(Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_BUSID)));
					if(Utils.hasHoneycomb()) {
						mMenuTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
					}
					else {
						mMenuTask.execute();
					}
				}
			}
			else {
				menu.setVisibility(TextView.GONE);
			}
			TextView calendar = (TextView)mParent.findViewById(R.id.contextMoreInfoButtonCalendar);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_NUM_EVENTS)) {
				numEventItems = Integer.parseInt(utcLoc.get(LocationContextParser.JSON_TAG_NUM_EVENTS));
				numOfItems += numEventItems;

				if(numEventItems == 0) {
					calendar.setVisibility(TextView.GONE);
				}
				else if(numEventItems > 0) {
					Logger.info(mName, "business contains events");
					loadEvents();
					calendar.setClickable(true);
					calendar.setOnClickListener(new View.OnClickListener() {
						@Override
						public void onClick(View v) {

							TextView gButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonGallery);
							TextView mButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonMenu);
							TextView cButton = (TextView) mParent.findViewById(R.id.contextMoreInfoButtonCalendar);
							LinearLayout gItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutGalleryItems);
							RelativeLayout mItemsLayout = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutMenuItems);
							LinearLayout cItemsLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutCalendarItems);

							inactiveButton(gButton);
							inactiveButton(mButton);
							activeButton(cButton);

							gItemsLayout.setVisibility(LinearLayout.GONE);
							mItemsLayout.setVisibility(RelativeLayout.GONE);
							cItemsLayout.setVisibility(LinearLayout.VISIBLE);
						}
					});
				}
			}
			else {
				calendar.setVisibility(TextView.GONE);
			}

			// default selected more info category logic
			if(numGalleryItems > 0) {
				gallery.performClick();
			}
			else if(numMenuItems > 0) {
				menu.performClick();
			}
			else if(numEventItems > 0) {
				calendar.performClick();
			}

			final ImageView contextMoreInfoButton = (ImageView) mParent.findViewById(R.id.imageViewContextMoreInfo);
			final ImageButton moreInfoButton  = (ImageButton) mParent.findViewById(R.id.imageButtonMoreInfo);
			final int fNumOfItems = numOfItems;
			if(numOfItems > 0) {
				MoreInfoClickListener infoClick = new MoreInfoClickListener();
				contextMoreInfoButton.setClickable(true);
				contextMoreInfoButton.setOnClickListener(infoClick);
				moreInfoButton.setClickable(true);
				moreInfoButton.setOnClickListener(infoClick);
				contextMoreInfoButton.setVisibility(ImageView.VISIBLE);
				moreInfoButton.setVisibility(ImageButton.VISIBLE);
			}

			final ImageView back = (ImageView)mParent.findViewById(R.id.imageViewContextBack);
			back.setOnTouchListener(new View.OnTouchListener() {
				@Override
				public boolean onTouch(View v, MotionEvent event) {

					ScrollView redeem = (ScrollView) mParent.findViewById(R.id.scrollViewRedeem);
					LinearLayout moreInfo = (LinearLayout) mParent.findViewById(R.id.linearLayoutMoreInfo);
					LinearLayout review = (LinearLayout) mParent.findViewById(R.id.linearLayoutReview);
					ImageView moreInfoButton = (ImageView) mParent.findViewById(R.id.imageViewContextMoreInfo);

					if (mMoreInfoLayoutOpen) {
						moreInfo.setVisibility(LinearLayout.GONE);
						mMoreInfoLayoutOpen = false;
					}
					else if (mReviewLayoutOpen) {
						review.setVisibility(LinearLayout.GONE);
						mReviewLayoutOpen = false;
					}

					back.setVisibility(ImageView.GONE);
					redeem.setVisibility(ScrollView.VISIBLE);
					if(fNumOfItems > 0) {
						moreInfoButton.setVisibility(ImageView.VISIBLE);
					}

					return true;
				}
			});

			iv = (ImageView) mParent.findViewById(R.id.imageViewLeaveReview);
			iv.setOnTouchListener(new ReviewTouchListener());

			final Button cancelReviewButton = (Button) mParent.findViewById(R.id.buttonReviewCancel);
			cancelReviewButton.setOnClickListener(new View.OnClickListener() {

				@Override
				public void onClick(View v) {
					// only perform action after the appropriate dwell time has occurred
					if ((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
						return;
					}

					mTimestamp = SystemClock.elapsedRealtime();

					// hide review layout
					LinearLayout review = (LinearLayout) getActivity().findViewById(R.id.linearLayoutReview);
					review.setVisibility(LinearLayout.GONE);

					// show redeem layout
					ScrollView redeem = (ScrollView) mParent.findViewById(R.id.scrollViewRedeem);
					redeem.setVisibility(ScrollView.VISIBLE);

					// hide back
					ImageView back = (ImageView) mParent.findViewById(R.id.imageViewContextBack);
					back.setVisibility(ImageView.GONE);

					// show more info
					ImageView moreInfoButton = (ImageView) mParent.findViewById(R.id.imageViewContextMoreInfo);
					if (fNumOfItems > 0) {
						moreInfoButton.setVisibility(ImageView.VISIBLE);
					}

					mReviewLayoutOpen = false;

					((TextView) getActivity().findViewById(R.id.crumbContextRedeem)).setText(R.string.redeem);
				}
			});

			Button viewMenu = (Button) mParent.findViewById(R.id.buttonViewMenu);
			LinearLayout viewMenuWrapper = (LinearLayout) mParent.findViewById(R.id.linearLayoutViewMenuButtonWrapper);
			if(utcLoc.containsKey(LocationContextParser.JSON_TAG_MENU_LINK)) {
				final String menuLink = utcLoc.get(LocationContextParser.JSON_TAG_MENU_LINK);

				if(menuLink.equals("")) {
					viewMenu.setVisibility(Button.GONE);
					viewMenuWrapper.setVisibility(LinearLayout.GONE);
				}
				else {
					viewMenu.setOnClickListener(new View.OnClickListener() {
						@Override
						public void onClick(View v) {
							Uri url = Uri.parse(menuLink);
							Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
							try {
								startActivity(launchBrowser);
							} catch (ActivityNotFoundException anfe) {
								anfe.printStackTrace();
							}
						}
					});
				}
			}
			else {
				viewMenu.setVisibility(Button.GONE);
				viewMenuWrapper.setVisibility(LinearLayout.GONE);
			}

			WebView reviews = (WebView)mParent.findViewById(R.id.webViewMemberReviews);
			reviews.getSettings().setLoadWithOverviewMode(true);
			//reviews.getSettings().setUseWideViewPort(true);
			reviews.loadUrl(Constants.LOCATION_INFO_REVIEWS + utcLoc.get(LocationContextParser.JSON_TAG_ID));

            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }

	private void inactiveButton(TextView button) {
		button.setBackgroundColor(Color.parseColor("#fb8c00"));
		button.setTextColor(Color.parseColor("#263238"));
	}

	private void activeButton(TextView button) {
		button.setBackgroundColor(Color.parseColor("#757575"));
		button.setTextColor(Color.parseColor("#ffffff"));
	}

    public boolean cancelLoadLocationTask() {
    	boolean result = false;
    	if(mLoadLocationTask != null) {
    		Logger.info(mName, "LoadLocationsTask cancelled");
    		result = mLoadLocationTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoadLocationsTask was null when cancelling");
    	}
    	return result;
    }
    
    public void clearContents() {
    	TextView tv;
		
        	ImageView iv = (ImageView) mParent.findViewById(R.id.imageViewContextLocation);
        	if(isAdded()) {
            	Drawable empty = getResources().getDrawable(R.drawable.location_empty);
            	iv.setImageDrawable(empty);
        	}
        	
    		tv = (TextView) mParent.findViewById(R.id.contextLocationName);
    		tv.setText("");
    		
    		tv = (TextView) mParent.findViewById(R.id.contextDistance);
    		tv.setText("");
    		
    		tv = (TextView) mParent.findViewById(R.id.contextAddress);
    		tv.setText("");
    		
    		iv = (ImageView) mParent.findViewById(R.id.contextRating);
        	if(isAdded()) {
        		Drawable rating = getResources().getDrawable(Constants.ratingIds[0]);
        		iv.setImageDrawable(rating);
        	}
    		
    		tv = (TextView) mParent.findViewById(R.id.contextCategory);
    		tv.setText("");

    		tv = (TextView) mParent.findViewById(R.id.contextDescription);
    		tv.setText("");
    		
    		tv = (TextView) mParent.findViewById(R.id.contextAvailableCash);
    		tv.setText("");
    }
    
    private class ToggleFavoriteTask extends AsyncTask<Integer, Void, Boolean> {
        protected Boolean doInBackground(Integer... params) {
    		Integer addFavorite = params[0];
    		Integer locId = Integer.valueOf(getArguments().getString(LocationParser.JSON_TAG_ID));
    		
    		if(addFavorite.intValue() == 0) {
    			UTCWebAPI.deleteFavorite(LoginManager.getInstance().getAccountContext().getToken(), locId);
    			FavoritesParser.setFavorites();
    			return true;
    		}
    		else {
    			UTCWebAPI.postFavorite(LoginManager.getInstance().getAccountContext().getToken(), locId);
    			FavoritesParser.setFavorites();
    			return false;
    		}
        }
        
        protected void onPostExecute(Boolean addTo) {
            // toggle favorite drawable
        	ImageView iv = (ImageView) mParent.findViewById(R.id.imageViewToggleFavorites);
        	if(addTo && isAdded()) {
        		Drawable rating = getResources().getDrawable(R.drawable.btn_add_to_favorites);
        		iv.setImageDrawable(rating);
        	}
        	else if(isAdded()) {
        		Drawable rating = getResources().getDrawable(R.drawable.btn_remove_from_favorites);
        		iv.setImageDrawable(rating);
        	}
        	
        	if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
    
    public boolean cancelToggleFavoriteTask() {
    	boolean result = false;
    	if(mToggleFavoriteTask != null) {
    		Logger.info(mName, "ToggleFavoriteTask cancelled");
    		result = mToggleFavoriteTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "ToggleFavoriteTask was null when cancelling");
    	}
    	return result;
    }
    
    private class FavoritesTouchListener implements View.OnTouchListener {
		
    	public Bundle mArguments;
    	
    	public FavoritesTouchListener(Bundle args) {
    		mArguments = args;
    	}
    	
		@TargetApi(Build.VERSION_CODES.HONEYCOMB)
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				// only perform action after the appropriate dwell time has occurred
				if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
					return false;
				}
				
				mTimestamp = SystemClock.elapsedRealtime();
				
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);
    			
    			if(LoginManager.getInstance().userLoggedIn()) {
    				if(isAdded()) ((MainActivity) getActivity()).showSpinner();
    				Integer locId = mArguments.getInt(LocationContextParser.JSON_TAG_ID);
    				if(Boolean.parseBoolean(DataManager.getInstance().getLocationContext().
    						get(LocationContextParser.JSON_TAG_MY_IS_FAVORITE))) {
    					mToggleFavoriteTask = new ToggleFavoriteTask();
    			        if(Utils.hasHoneycomb()) {
    			        	mToggleFavoriteTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, Integer.valueOf(0), locId);
    			        }
    			        else {
    			        	mToggleFavoriteTask.execute(Integer.valueOf(0), locId);
    			        }
    					DataManager.getInstance().removeFavorite(locId);
    					DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_IS_FAVORITE, "false");
    				}
    				else {
    					mToggleFavoriteTask = new ToggleFavoriteTask();
    			        if(Utils.hasHoneycomb()) {
    			        	mToggleFavoriteTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, Integer.valueOf(1), locId);
    			        }
    			        else {
    			        	mToggleFavoriteTask.execute(Integer.valueOf(1), locId);
    			        }
    					DataManager.getInstance().addFavorite(locId);
    					DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_IS_FAVORITE, "true");
    				}
    			}
    			else {
    			    showGuestFavoriteDialog();
    			}
			}
			
			DataManager.getInstance().setLocalUpdateNeeded(true);
			
			return false;
		}
	}

    private class RateItTask extends AsyncTask<Float, Void, Integer> {
        protected Integer doInBackground(Float... params) {
    		Float rating = params[0];
    		Integer locId = Integer.valueOf(getArguments().getString(LocationParser.JSON_TAG_ID));

    		int getRating = 0;
    		if(LoginManager.getInstance().userLoggedIn()) {
        		UTCWebAPI.postRating(LoginManager.getInstance().getAccountContext().getToken(), locId, (int) rating.floatValue());
        		try {
					LocationParser.setLocation(locId);
				} catch (JSONException e) {
					e.printStackTrace();
				}
    		}

    		if(DataManager.getInstance().getLocation(locId).containsKey(LocationParser.JSON_TAG_RATING)) {
    			UTCLocation loc = DataManager.getInstance().getLocation(locId);
    			String ratingStr = loc.get(LocationParser.JSON_TAG_RATING);
    			getRating = Integer.parseInt(ratingStr);
    		}

    		return getRating;
        }

        protected void onPostExecute(Integer rating) {
    		if(isAdded()) {
        		// get location rating number parsed as an int, with Drawable
    			// ID looked up by ratingIds
        		Drawable ratingDrawable = getResources().getDrawable(
        				Constants.ratingIds[rating]
        						);
        		((ImageView) mParent.findViewById(R.id.contextRating))
        			.setImageDrawable(ratingDrawable);
    		}

    		if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }

    public boolean cancelRateItTask() {
    	boolean result = false;
    	if(mRateItTask != null) {
    		Logger.info(mName, "RateItTask cancelled");
    		result = mRateItTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "RateItTask was null when cancelling");
    	}
    	return result;
    }
    
    private class RateTouchListener implements View.OnTouchListener {
		@TargetApi(Build.VERSION_CODES.HONEYCOMB)
		@Override
		public boolean onTouch(View v, MotionEvent event) {			
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				// only perform action after the appropriate dwell time has occurred
				if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
					return false;
				}
				
				mTimestamp = SystemClock.elapsedRealtime();
				
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);
    			
				if(LoginManager.getInstance().userLoggedIn()) {
					RateDialog rate = RateDialog.newInstance(mBusinessName, mRating);
					rate.show(getFragmentManager());
				}
				else {
				    showGuestRatingDialog();
				}
			}
			
			DataManager.getInstance().setLocalUpdateNeeded(true);
			
			return false;
		}
	}
    
    private class ReviewTask extends AsyncTask<String, Void, Boolean> {
        protected Boolean doInBackground(String... params) {
    		String review = params[0];
    		Integer locId = Integer.valueOf(getArguments().getString(LocationContextParser.JSON_TAG_ID));
    		int accId = 0;
    		String signature = "";
    		
    		if(DataManager.getInstance().getLocationContext().containsKey(LocationContextParser.JSON_TAG_ACCOUNT_ID)) {
    			accId = Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ACCOUNT_ID));
    		}
    		
    		signature = LoginManager.getInstance().getAccountContext().getSignature();
    		
    		if(LoginManager.getInstance().userLoggedIn()) {
    			// full potential format "yyyy-MM-dd'T'HH:mm:ss.SSSSSSSZ"
    			SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SS", Locale.US);
    			SimpleDateFormat dateFormatUTC = new SimpleDateFormat("yyyy-MM-ddHH:mm:ss", Locale.US);
    			Calendar today = Calendar.getInstance();
    			Calendar todayUTC = Calendar.getInstance();
    			today.setTimeZone(TimeZone.getDefault());
    			todayUTC.setTimeZone(TimeZone.getTimeZone(Time.TIMEZONE_UTC));
    			mJSONReply = null;
    			
    			mJSONReply = UTCWebAPI.postTip(LoginManager.getInstance().getAccountContext().getToken(), 
        				locId, 
        				accId,
        				review,
        				signature,
        				dateFormat.format(today.getTime()),
        				dateFormatUTC.format(todayUTC.getTime())
        				);
        		if(mJSONReply != null) {
        			return Boolean.valueOf(false);
        		}
    		}
    		
    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
        	if(success.booleanValue()) {
    			// hide soft keyboard
        		EditText review = (EditText) getActivity().findViewById(R.id.editTextReview);
			    InputMethodManager imm = (InputMethodManager) getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
			    imm.hideSoftInputFromWindow(review.getApplicationWindowToken(), 0);
			    
			    // update review edit so user sees latest review if they go directly back to edit it again
			    review.setText(mReviewString);
			    
			    // update local data (the review text) for the location context
			    DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_TIP, mReviewString);
			    
			    // remove review layout from user view
    			LinearLayout reviewLayout = (LinearLayout) getActivity().findViewById(R.id.linearLayoutReview);
    			reviewLayout.setVisibility(LinearLayout.GONE);
    			mReviewLayoutOpen = false;

				// show redeem layout
				ScrollView redeem = (ScrollView)mParent.findViewById(R.id.scrollViewRedeem);
				redeem.setVisibility(ScrollView.VISIBLE);

				// show redeem and hide edit
				TextView crumb = (TextView)mParent.findViewById(R.id.crumbContextRedeem);
				crumb.setText(R.string.redeem);
				ImageView back = (ImageView)mParent.findViewById(R.id.imageViewContextBack);
				back.setVisibility(ImageView.GONE);

				WebView reviews = (WebView)mParent.findViewById(R.id.webViewMemberReviews);
				reviews.reload();
        	}
        	
        	if(mJSONReply != null) {
    			if(mJSONReply.has("Message")) {
    				try {
    					Toast.makeText(getActivity(), mJSONReply.getString("Message"), Toast.LENGTH_SHORT).show();
    				} catch (JSONException e) {
    					e.printStackTrace();
    				}
    			}
        	}
        	
        	if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
    
    public boolean cancelReviewTask() {
    	boolean result = false;
    	if(mReviewTask != null) {
    		Logger.info(mName, "ReviewTask cancelled");
    		result = mReviewTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "ReviewTask was null when cancelling");
    	}
    	return result;
    }

	private class GalleryItem {

		public final static String	JSON_TAG_ID			= "Id";
		public final static String	JSON_TAG_BUS_ID		= "BusId";
		public final static String	JSON_TAG_SEQUENCE	= "Sequence";
		public final static String	JSON_TAG_IMAGE_ID	= "ImageId";

		private int mID;
		private int mBusID;
		private int mSequence;
		private String mImageID;

		public GalleryItem(int id, int busID, int sequence, String imageID) {
			mID = id;
			mBusID = busID;
			mSequence = sequence;
			mImageID = imageID;
		}

		public int getID() {
			return mID;
		}

		public int getBusinessID() {
			return mBusID;
		}

		public int getSequence() {
			return mSequence;
		}

		public String getImageID() {
			return mImageID;
		}

	}

	private class MenuItem {

		public final static String	JSON_TAG_ID			= "Id";
		public final static String	JSON_TAG_BUS_ID		= "BusId";
		public final static String	JSON_TAG_SEQUENCE	= "Sequence";
		public final static String	JSON_TAG_NAME		= "Name";
		public final static String	JSON_TAG_PRICE		= "Price";

		private int mID;
		private int mBusID;
		private int mSequence;
		private String mName;
		private double mPrice;

		public MenuItem(int id, int busID, int sequence, String name, double price) {
			mID = id;
			mBusID = busID;
			mSequence = sequence;
			mName = name;
			mPrice = price;
		}

		public int getID() {
			return mID;
		}

		public int getBusinessID() {
			return mBusID;
		}

		public int getSequence() {
			return mSequence;
		}

		public String getName() {
			return mName;
		}

		public double getPrice() {
			return mPrice;
		}

	}

	private class GalleryTask extends AsyncTask<Void, Void, Boolean> {

		int mBusID = 0;

		public GalleryTask(int busID) {
			mBusID = busID;
		}

		protected Boolean doInBackground(Void... params) {
			Logger.info(mName, "get gallery for " + String.valueOf(mBusID));

			// getting JSON array from URL
			JSONArray json = UTCWebAPI.getGallery(mBusID);

			mGalleryItems = new HashMap<Integer, GalleryItem>();

			// if JSON request didn't work, bail
			if(json == null) {
				Logger.error(mName, "error requesting gallery");
				Boolean.valueOf(false);
			}

			Logger.info(mName, "number of gallery items returned: " + String.valueOf(json.length()));

			for(int i = 0; i < json.length(); i++) {
				try {
					JSONObject jsonGallery = json.getJSONObject(i);

					if(jsonGallery.has(GalleryItem.JSON_TAG_ID) &&
							jsonGallery.has(GalleryItem.JSON_TAG_BUS_ID) &&
							jsonGallery.has(GalleryItem.JSON_TAG_SEQUENCE) &&
							jsonGallery.has(GalleryItem.JSON_TAG_IMAGE_ID)) {

						GalleryItem gItem = new GalleryItem(
								jsonGallery.getInt(GalleryItem.JSON_TAG_ID),
								jsonGallery.getInt(GalleryItem.JSON_TAG_BUS_ID),
								jsonGallery.getInt(GalleryItem.JSON_TAG_SEQUENCE),
								jsonGallery.getString(GalleryItem.JSON_TAG_IMAGE_ID));

						mGalleryItems.put(gItem.getSequence(), gItem);

						Logger.info(mName, "added to list gallery item " + gItem.getImageID());
					}

				} catch (JSONException e) {
					e.printStackTrace();
				}
			}

			return Boolean.valueOf(true);
		}

		protected void onPostExecute(Boolean success) {
			if(success.booleanValue()) {
				Map<Integer, GalleryItem> sortedGalleryItems = new TreeMap<Integer, GalleryItem>(mGalleryItems);
				List<GalleryItem> galleryList = new ArrayList<GalleryItem>(sortedGalleryItems.values());

				LinearLayout gallery = (LinearLayout)mParent.findViewById(R.id.linearLayoutGalleryItems);
				int idNum = Constants.BASE_CUSTOM_IDS + 500;
				int idOffset = 0;

				gallery.removeAllViewsInLayout();

				if(sortedGalleryItems != null && sortedGalleryItems.size() > 0) {

					String url;
					String urlPostfix = ".thumb.png";
					int rows = ((sortedGalleryItems.size() - 1) / 3) + 1;
					int itemsInLastRow = ((sortedGalleryItems.size() - 1) % 3) + 1;

					RelativeLayout row = null;
					LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
					for (int i = 0; i < rows; i++) {
						row = (RelativeLayout) inflater.inflate(R.layout.gallery_row, null, false);
						row.setId(idNum + idOffset++);

						ImageView galleryItem = null;

						int index = i * 3;
						if(index < galleryList.size()) {
							url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + urlPostfix;
							galleryItem = (ImageView) row.findViewById(R.id.imageViewGalleryRowItem1);
							galleryItem.setId(idNum + idOffset++);
							galleryItem.setVisibility(ImageView.VISIBLE);
							addImage(url, galleryItem);
							url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + ".png";
							galleryItem.setOnTouchListener(new ImageViewTouchListener(url));
							Logger.info(mName, "added to view gallery item " + galleryList.get(index).getImageID());
						}

						if (i == rows - 1) {
							// last row
							index = (i * 3) + 1;
							if(itemsInLastRow > 1 && index < galleryList.size()) {
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + urlPostfix;
								galleryItem = (ImageView)row.findViewById(R.id.imageViewGalleryRowItem2);
								galleryItem.setId(idNum + idOffset++);
								galleryItem.setVisibility(ImageView.VISIBLE);
								addImage(url, galleryItem);
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + ".png";
								galleryItem.setOnTouchListener(new ImageViewTouchListener(url));
								Logger.info(mName, "added to view gallery item " + galleryList.get(index).getImageID());
							}

							index = (i * 3) + 2;
							if(itemsInLastRow > 2 && index < galleryList.size()) {
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + urlPostfix;
								galleryItem = (ImageView)row.findViewById(R.id.imageViewGalleryRowItem3);
								galleryItem.setId(idNum + idOffset++);
								galleryItem.setVisibility(ImageView.VISIBLE);
								addImage(url, galleryItem);
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + ".png";
								galleryItem.setOnTouchListener(new ImageViewTouchListener(url));
								Logger.info(mName, "added to view gallery item " + galleryList.get(index).getImageID());
							}
						}
						else {
							index = (i * 3) + 1;
							if(index < galleryList.size()) {
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + urlPostfix;
								galleryItem = (ImageView) row.findViewById(R.id.imageViewGalleryRowItem2);
								galleryItem.setId(idNum + idOffset++);
								galleryItem.setVisibility(ImageView.VISIBLE);
								addImage(url, galleryItem);
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + ".png";
								galleryItem.setOnTouchListener(new ImageViewTouchListener(url));
								Logger.info(mName, "added to view gallery item " + galleryList.get(index).getImageID());
							}

							index = (i * 3) + 2;
							if(index < galleryList.size()) {
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + urlPostfix;
								galleryItem = (ImageView) row.findViewById(R.id.imageViewGalleryRowItem3);
								galleryItem.setId(idNum + idOffset++);
								galleryItem.setVisibility(ImageView.VISIBLE);
								addImage(url, galleryItem);
								url = Constants.BUSINESS_GALLERY_URL + galleryList.get(index).getImageID() + ".png";
								galleryItem.setOnTouchListener(new ImageViewTouchListener(url));
								Logger.info(mName, "added to view gallery item " + galleryList.get(index).getImageID());
							}
						}

						gallery.addView(row);
						Logger.info(mName, "added gallery row " + String.valueOf(i));
					}
				}
			}
		}
	}

	public boolean cancelGalleryTask() {
		boolean result = false;
		if(mGalleryTask != null) {
			Logger.info(mName, "GalleryTask cancelled");
			result = mGalleryTask.cancel(true);
		}
		else {
			Logger.verbose(mName, "GalleryTask was null when cancelling");
		}
		return result;
	}

	private class MenuTask extends AsyncTask<Void, Void, Boolean> {

		int mBusID = 0;

		public MenuTask(int busID) {
			mBusID = busID;
		}

		protected Boolean doInBackground(Void... params) {
			// getting JSON array from URL
			JSONArray json = UTCWebAPI.getMenu(mBusID);

			mMenuItems = new HashMap<Integer, MenuItem>();

			// if JSON request didn't work, bail
			if(json == null) {
				Logger.error(mName, "error requesting menu");
				Boolean.valueOf(false);
			}

			Logger.info(mName, "number of menu items returned: " + String.valueOf(json.length()));

			for(int i = 0; i < json.length(); i++) {
				try {
					JSONObject jsonMenu = json.getJSONObject(i);

					if(jsonMenu.has(MenuItem.JSON_TAG_ID) &&
							jsonMenu.has(MenuItem.JSON_TAG_BUS_ID) &&
							jsonMenu.has(MenuItem.JSON_TAG_SEQUENCE) &&
							jsonMenu.has(MenuItem.JSON_TAG_NAME) &&
							jsonMenu.has(MenuItem.JSON_TAG_PRICE)) {

						MenuItem mItem = new MenuItem(
								jsonMenu.getInt(MenuItem.JSON_TAG_ID),
								jsonMenu.getInt(MenuItem.JSON_TAG_BUS_ID),
								jsonMenu.getInt(MenuItem.JSON_TAG_SEQUENCE),
								jsonMenu.getString(MenuItem.JSON_TAG_NAME),
								jsonMenu.getDouble(MenuItem.JSON_TAG_PRICE));

						mMenuItems.put(mItem.getSequence(), mItem);

						Logger.info(mName, "added to list menu item " + mItem.getName() + ", " + mItem.getPrice());
					}

				} catch (JSONException e) {
					e.printStackTrace();
				}
			}

			return Boolean.valueOf(true);
		}

		protected void onPostExecute(Boolean success) {
			if(success.booleanValue()) {
				Map<Integer, MenuItem> sortedMenuItems = new TreeMap<Integer, MenuItem>(mMenuItems);

				int idNum = Constants.BASE_CUSTOM_IDS + 600;
				int idOffset = 0;

				if(sortedMenuItems != null && sortedMenuItems.size() > 0) {
					LinearLayout menuItems = (LinearLayout)mParent.findViewById(R.id.linearLayoutMenuItems);
					LayoutInflater inflater = (LayoutInflater)mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);

					TextView tv;

					menuItems.removeAllViewsInLayout();

					NumberFormat formatter = NumberFormat.getCurrencyInstance();

					for(int i = 0; i < sortedMenuItems.size(); i++) {
						RelativeLayout menuItem = (RelativeLayout)inflater.inflate(R.layout.menu_item, null, false);
						menuItem.setId(idNum + idOffset++);

						MenuItem item = sortedMenuItems.get(Integer.valueOf(i + 1));

						tv = (TextView) menuItem.findViewById(R.id.textViewMenuItemName);
						tv.setId(idNum + idOffset++);
						tv.setText(item.getName());

						tv = (TextView) menuItem.findViewById(R.id.textViewMenuItemPrice);
						tv.setId(idNum + idOffset++);
						tv.setText(formatter.format(item.getPrice()));

						menuItems.addView(menuItem);
						Logger.info(mName, "added to view menu item " + item.getName() + ", " + String.valueOf(item.getPrice()));
					}
				}
			}
		}
	}

	public boolean cancelMenuTask() {
		boolean result = false;
		if(mMenuTask != null) {
			Logger.info(mName, "MenuTask cancelled");
			result = mMenuTask.cancel(true);
		}
		else {
			Logger.verbose(mName, "MenuTask was null when cancelling");
		}
		return result;
	}
    
    private class ReviewTouchListener implements View.OnTouchListener {
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);
    			
				LinearLayout review = (LinearLayout) getActivity().findViewById(R.id.linearLayoutReview);
				
				if(LoginManager.getInstance().userLoggedIn()) {
					if(mReviewLayoutOpen == false) {
						// show current review and edit
						TextView crumb = (TextView) getActivity().findViewById(R.id.crumbContextRedeem);
						crumb.setText(R.string.redeem_post_review);
						EditText reviewEdit = (EditText) getActivity().findViewById(R.id.editTextReview);
						if(DataManager.getInstance().getLocationContext().containsKey(LocationContextParser.JSON_TAG_MY_TIP)) {
							reviewEdit.setText(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_MY_TIP));
						}
						reviewEdit.setVisibility(LinearLayout.VISIBLE);
						mReviewLayoutOpen = true;
						reviewEdit.requestFocus();
						// bring up soft keyboard
						InputMethodManager imm = (InputMethodManager) getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
						imm.showSoftInput(reviewEdit, InputMethodManager.SHOW_IMPLICIT);

						// hide redeem layout
						ScrollView redeem = (ScrollView)mParent.findViewById(R.id.scrollViewRedeem);
						redeem.setVisibility(ScrollView.GONE);

						// show review layout
						review.setVisibility(LinearLayout.VISIBLE);

						ImageView back = (ImageView)mParent.findViewById(R.id.imageViewContextBack);
						back.setVisibility(ImageView.VISIBLE);

						reviewEdit.selectAll();
					}
				}
				else {
					// show non-member notification
				    showGuestReviewDialog();
				}
			}
			
			return false;
		}
	}

	private class MoreInfoClickListener implements View.OnClickListener {
		@TargetApi(Build.VERSION_CODES.HONEYCOMB)
		@Override
		public void onClick(View v) {
			// only perform action after the appropriate dwell time has occurred
			if((SystemClock.elapsedRealtime() - mTimestamp) < Constants.ACTION_DWELL) {
				return;
			}

			mTimestamp = SystemClock.elapsedRealtime();

			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
			vib.vibrate(Constants.VIBRATE_LENGTH);

			// switch to more info
			ScrollView redeem = (ScrollView)mParent.findViewById(R.id.scrollViewRedeem);
			ImageView back = (ImageView)mParent.findViewById(R.id.imageViewContextBack);
			LinearLayout moreInfo = (LinearLayout)mParent.findViewById(R.id.linearLayoutMoreInfo);
			ImageView moreInfoButton = (ImageView)mParent.findViewById(R.id.imageViewContextMoreInfo);

			redeem.setVisibility(ScrollView.GONE);
			back.setVisibility(ImageView.VISIBLE);
			moreInfo.setVisibility(LinearLayout.VISIBLE);
			moreInfoButton.setVisibility(ImageView.GONE);

			mMoreInfoLayoutOpen = true;
		}
	}

	private class ImageViewTouchListener implements View.OnTouchListener {

		String mURL = "";

		ImageViewTouchListener(String url) {
			mURL = url;
		}

		@Override
		public boolean onTouch(View v, MotionEvent event) {

			if(event.getAction() == MotionEvent.ACTION_UP) {
				ImageView thisImage = (ImageView) v;
				Drawable thisDrawable = null;

				if (thisImage != null && thisImage.getDrawable() != null) {
					thisDrawable = thisImage.getDrawable();
				}

				ImageViewerDialog viewer = ImageViewerDialog.newInstance(thisDrawable, mURL);
				viewer.show(getFragmentManager());
			}

			return true;
		}
	}

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	public void loadEvents() {
		mLoadEventsTask = new LoadEventsTask();
		// task object could be corrupted or canceled
		// when switching menus
		if(mLoadEventsTask != null) {
			if(Utils.hasHoneycomb()) {
				mLoadEventsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
			}
			else {
				mLoadEventsTask.execute();
			}
		}
	}

	private class LoadEventsTask extends AsyncTask<Void, View, Integer> {
		protected Integer doInBackground(Void... params) {
			DataManager dm = DataManager.getInstance();

			int eventNum = 0;

			if((MainActivity) getActivity() == null) {
				Logger.verbose(mName, "main == null in LoadEventsTask");
				return 0;
			}

			// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadEventsTask");
				return 0;
			}

			/////////////////////////////////////////////////////////////////////////
			//////////   Retrieve event data   //////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////
			boolean sort = dm.doEventsNeedUpdated();
			mEventRetrievalError = EventsParser.setEvents();
			if(sort && mEventRetrievalError == null) {
				dm.sortEventsByDate();
			}

			/////////////////////////////////////////////////////////////////////////
			//////////   Create event layouts   /////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////
			ArrayList<Integer> listID = dm.getEventIDs();

			TextView tv;

			int idNum = Constants.BASE_CUSTOM_IDS;
			int idOffset = 20000;

			Logger.verbose(mName, "number of event IDs - " + String.valueOf(listID.size()));
			LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			for(int i = 1; i <= listID.size(); i++) {
				Logger.verbose(mName, String.valueOf(i) + " adding location ID " + listID.get(i - 1));

				// keep checking if this task was canceled and bail if so
				if(isCancelled() == true) {
					Logger.verbose(mName, "isCancelled == true in LoadEventsTask (loop)");
					return 0;
				}

				UTCEvent utcEvt = dm.getEvent(listID.get(i - 1));

				// check if location should be added based on category filter
				if(utcEvt.containsKey(EventsParser.JSON_TAG_ID)) {

					String allEventDetails = "";

					String name = "";
					String summary = "";
					String category = "";
					String eventType = "";

					if(utcEvt.containsKey(EventsParser.JSON_TAG_BUSNAME)) {
						name = utcEvt.get(EventsParser.JSON_TAG_BUSNAME);
						allEventDetails = allEventDetails + name;
					}

					if(utcEvt.containsKey(EventsParser.JSON_TAG_SUMMARY)) {
						summary = utcEvt.get(EventsParser.JSON_TAG_SUMMARY);
						allEventDetails = allEventDetails + summary;
					}

					if(utcEvt.containsKey(EventsParser.JSON_TAG_CATNAME)) {
						category = utcEvt.get(EventsParser.JSON_TAG_CATNAME);
						if(utcEvt.containsKey(EventsParser.JSON_TAG_PROPERTIES + "Size")) {
							int propertiesNum = Integer.valueOf(utcEvt.get(EventsParser.JSON_TAG_PROPERTIES + "Size")).intValue();
							for(int j = 0; j < propertiesNum; j++) {
								category = category + ", " + utcEvt.get(EventsParser.JSON_TAG_PROPERTIES + String.valueOf(j));
							}
						}
						allEventDetails = allEventDetails + category;
					}

					if(utcEvt.containsKey(EventsParser.JSON_TAG_EVENT_TYPE)) {
						eventType = utcEvt.get(EventsParser.JSON_TAG_EVENT_TYPE);
						allEventDetails = allEventDetails + eventType;
					}

					boolean searchMatch = allEventDetails
							.toUpperCase(Locale.getDefault())
							.contains(mSearchFilter
									.toUpperCase(Locale.getDefault()));

					if(searchMatch && !dm.isCategoryAntifiltered(
							Integer.parseInt(utcEvt.get(EventsParser.JSON_TAG_CATID)))
							) {
						final Bundle args = new Bundle();
						args.putString(EventsParser.JSON_TAG_BUSGUID, utcEvt.get(EventsParser.JSON_TAG_BUSGUID));
						args.putString(EventsParser.JSON_TAG_ID, utcEvt.get(EventsParser.JSON_TAG_ID));

						View child = null;
						try {
							child = inflater.inflate(R.layout.event_details, null, false);
						}
						catch(InflateException ie) {
							Logger.error(mName, "could not inflate child view");
							throw new RuntimeException(ie);
						}

						child.setId(idNum + idOffset++);

						// alternate white and gray
						if((i % 2) == 1) {
							RelativeLayout container = (RelativeLayout) child.findViewById(R.id.relativeLayoutEventDetailsContainer);
							container.setBackgroundColor(Color.parseColor("#FFFFFF"));
						}

						tv = (TextView) child.findViewById(R.id.eventDetailsFrom);
						tv.setId(idNum + idOffset++);
						tv.setClickable(true);
						tv.setOnTouchListener(new EventTouchListener(args));
						tv.setText(name);

						tv = (TextView) child.findViewById(R.id.eventDetailsDate);
						tv.setId(idNum + idOffset++);
						tv.setClickable(true);
						tv.setOnTouchListener(new EventTouchListener(args));

						if(utcEvt.containsKey(EventsParser.JSON_TAG_DATE_AS_STR)) {
							tv.setText(utcEvt.get(EventsParser.JSON_TAG_DATE_AS_STR));
						}

						tv = (TextView) child.findViewById(R.id.eventDetailsSummary);
						tv.setId(idNum + idOffset++);
						tv.setClickable(true);
						tv.setOnTouchListener(new EventTouchListener(args));
						tv.setText(summary);

						Button readMore = (Button) child.findViewById(R.id.buttonEventDetailsReadMore);
						readMore.setOnTouchListener(new EventTouchListener(args));

						String url = Constants.LOCATION_INFO_IMAGE + "/" + utcEvt.get(EventContextParser.JSON_TAG_BUSGUID) + "@2x.png";
						ImageView logo = (ImageView) child.findViewById(R.id.imageViewEventDetails);
						logo.setId(idNum + idOffset++);
						addImageSerial(url, logo);

						// keep checking if this task was canceled and bail if so
						if(isCancelled() == true) {
							Logger.verbose(mName, "isCancelled == true in LoadEventsTask (loop)");
							return 0;
						}

						mEventProgress = i;
						publishProgress(child);
						try {
							Thread.sleep(2 * Constants.LAYOUT_ADDITION_DELAY);
						} catch (InterruptedException e) {
							// don't do anything, we don't care if
							// a sleep is interrupted
						}
					}
				}
			}

			return eventNum;
		}

		@Override
		protected void onProgressUpdate(View... child) {
			super.onProgressUpdate(child[0]);

			try {
				((LinearLayout) mParent.findViewById(R.id.linearLayoutCalendarItems)).addView(child[0]);
			}
			catch(IllegalStateException ise) {
				ise.printStackTrace();
			}
		}

		protected void onPostExecute(Integer locationNum) {
			if(mEventRetrievalError != null) {
				Toast.makeText(mParent.getContext(), mEventRetrievalError, Toast.LENGTH_SHORT).show();
			}
			mEventRetrievalError = null;
		}
	}

	public boolean cancelLoadEventsTask() {
		boolean result = false;
		if(mLoadEventsTask != null) {
			Logger.info(mName, "LoadEventsTask cancelled");
			result = mLoadEventsTask.cancel(true);
		}
		else {
			Logger.verbose(mName, "LoadEventsTask was null when cancelling");
		}
		return result;
	}

	public void removeAllEventResults() {
		LinearLayout results = (LinearLayout) mParent.findViewById(R.id.linearLayoutCalendarItems);
		results.removeAllViews();
	}

	private class EventTouchListener implements View.OnTouchListener {

		public Bundle mArguments;

		public EventTouchListener(Bundle args) {
			mArguments = args;
		}

		@Override
		public boolean onTouch(View v, MotionEvent event) {
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				Logger.info(mName, "calendar item touch");
				replaceSubmenuFragment(Constants.MenuID.EVENT, mArguments, true);
			}

			return false;
		}
	}
}