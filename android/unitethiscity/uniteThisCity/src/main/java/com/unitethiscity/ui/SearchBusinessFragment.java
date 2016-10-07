package com.unitethiscity.ui;

import java.util.ArrayList;
import java.util.Locale;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.FavoritesParser;
import com.unitethiscity.data.LocationContextParser;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCGeolocationManager;
import com.unitethiscity.data.UTCLocation;
import com.unitethiscity.data.DataManager.ScanTask;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.annotation.SuppressLint;
import android.graphics.drawable.BitmapDrawable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.text.Editable;
import android.text.TextWatcher;
import android.annotation.TargetApi;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.Vibrator;
import android.view.InflateException;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnKeyListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

public class SearchBusinessFragment extends Fragment {

	private String mName = Constants.SEARCH_BUSINESS_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.SEARCH_BUSINESS;

	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;
	private String mLocationRetrievalError = null;

	private LinearLayout mLocationsLayout;

	private int mLocationProgress;

	private boolean mNotifiedAboutLocationProviders = false;

	private boolean mSearching = false;
	private String mSearchFilter = "";

	private String mBusinessType = "Business";

	private AsyncTask<Void, View, Integer> mLoadLocationsTask;
	private AsyncTask<Integer, Void, Integer> mLoadLocationTask;

	private Integer mLocationID;

	private int mLocationIndex = 1;
	private boolean mLoadingLocationsBlock = false;

	private UTCDialogFragment mUTCDialog;
	private RedeemRewardDialog mRewardDialog;

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
    	mParent = inflater.inflate(R.layout.fragment_search_business, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");
        
        EditText et = (EditText) mParent.findViewById(R.id.editTextBusinessSearchFilter);
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
				mSearchFilter = s.toString();
				if (mSearchFilter.isEmpty()) {
					mSearching = false;
				}
			}

		});

		View.OnClickListener categoryClick = new View.OnClickListener() {

			@Override
			public void onClick(View v) {
				hide();
				DataManager.getInstance().getSearchCategoriesFragment().fragmentActive(true);
				DataManager.getInstance().getSearchCategoriesFragment().setParent(mMenuID);
				FragmentManager fm = getActivity().getSupportFragmentManager();
				FragmentTransaction ft = fm.beginTransaction();
				ft.add(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchCategoriesFragment(),
						Constants.MenuID.SEARCH_CATEGORIES.toString());
				DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_CATEGORIES);
				ft.commit();
			}
		};
        Button b = (Button) mParent.findViewById(R.id.buttonSearchBusinessKeywordCategory);
        b.setOnClickListener(categoryClick);
		b = (Button) mParent.findViewById(R.id.buttonSearchBusinessCategory);
		b.setOnClickListener(categoryClick);
		b = (Button) mParent.findViewById(R.id.buttonSearchBusinessKeyword);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				RelativeLayout results = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchBusinessResults);
				results.setVisibility(RelativeLayout.GONE);
				RelativeLayout keyword = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchBusinessKeyword);
				keyword.setVisibility(RelativeLayout.VISIBLE);
			}
		});
		b = (Button) mParent.findViewById(R.id.buttonSearchBusinessKeywordGo);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				RelativeLayout keyword = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchBusinessKeyword);
				keyword.setVisibility(RelativeLayout.GONE);
				RelativeLayout results = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchBusinessResults);
				results.setVisibility(RelativeLayout.VISIBLE);
				if(mSearchFilter.isEmpty()) {
					mSearching = false;
				}
				else {
					mSearching = true;
				}
				mSearchFilter = mSearchFilter.trim();
				mLocationIndex = 1;
				cancelLoadLocationsTask();
				removeAllLocationResults();
				loadLocations(true);
			}
		});
		b = (Button) mParent.findViewById(R.id.buttonSearchBusinessKeywordCancel);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				((MainActivity) getActivity()).onBackPressed();
			}
		});
        
    	if(mFragmentActive == true) {
            ((MainActivity) getActivity()).clearBackPressed();

			InteractiveScrollView scroll = (InteractiveScrollView) mParent.findViewById(R.id.scrollViewSearchBusiness);
			scroll.setOnBottomReachedListener(new InteractiveScrollView.OnBottomReachedListener() {
				@Override
				public void onBottomReached() {
					Logger.info(mName, "InteractiveScrollView reached bottom");
					if(mSearching == false) {
						loadLocations(false);
					}
				}
			});

			mLocationIndex = 1;

            loadLocations(false);
    	}
    	Logger.verbose(mName, "onActivityCreated fragmentActive - " + mFragmentActive);
    }

    @Override
    public void onPause() {
    	super.onPause();
    	Logger.verbose(mName, "onPause()");
    	
    	DataManager.getInstance().cancelCheckInOrRedeemTask();
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");

		// clear search filter
		EditText et = (EditText) mParent.findViewById(R.id.editTextBusinessSearchFilter);
		et.setText("");
		mSearchFilter = "";
		mSearching = false;

		setBusinessType(mBusinessType);

    	String scanData = DataManager.getInstance().getScanData();
    	if(scanData != null) {
    		Logger.verbose(mName, "got scan data " + scanData);
    		
    		if(DataManager.getInstance().getScanTask().equals(ScanTask.REDEEM)) {
    			Logger.verbose(mName, "redeem scan");
    		}
    		else if(DataManager.getInstance().getScanTask().equals(ScanTask.CHECKIN)) {
    			Logger.verbose(mName, "checkin scan");
    		}
    		
    		DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(), 
    				scanData, Constants.Role.MEMBER.getValue(), -1);
    	}
    	else {
        	if(DataManager.getInstance().getAnalyticsState()) {
        		Logger.verbose(mName, "starting Google analytics for this screen");
        		((MainActivity) getActivity()).sendView(mName);
        	}
    	}
    }

	@Override
	public void onDetach() {
		super.onDetach();
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

	public void setBusinessType(String type) {
		mBusinessType = type;
		if (mParent != null) {
			TextView crumb = (TextView) mParent.findViewById(R.id.crumbSearchBusiness);
			TextView title = (TextView) mParent.findViewById(R.id.textViewSearchBusinessKeywordTitle);
			String typeText = type;
			if (typeText.toUpperCase().equals("BUSINESS")) {
				typeText += "E";
			}
			crumb.setText(typeText.toUpperCase() + "S");
			title.setText("SEARCH " + typeText.toUpperCase() + "S");
		}
	}
    
    public void removeSearchCategories() {
		FragmentManager fm = getActivity().getSupportFragmentManager();
		FragmentTransaction transaction = fm.beginTransaction();
		transaction.remove(DataManager.getInstance().getSearchCategoriesFragment()).commit();
		fragmentActive(true);
	    removeAllLocationResults();
	    loadLocations(true);
	    show();
		RelativeLayout keyword = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSearchBusinessKeyword);
		keyword.setVisibility(RelativeLayout.GONE);
		RelativeLayout results = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSearchBusinessResults);
		results.setVisibility(RelativeLayout.VISIBLE);
	    DataManager.getInstance().popFromMenuStack();
    }
    
	public void replaceSubmenuFragment(Constants.MenuID fID, Bundle args, boolean goBack)
	{
		if(fID != Constants.MenuID.HOME) {
			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
			// Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
			vib.vibrate(Constants.VIBRATE_LENGTH);
		}
		
		FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

		mFragmentActive = false;
		hide();
		
		// Replace whatever is in the frameLayout view with this fragment
	    switch (fID) {
			case REDEEM:
				((MainActivity) getActivity()).setFragmentID(fID);
				((MainActivity) getActivity()).setParentFragmentID(mMenuID);
				RedeemFragment rf = DataManager.getInstance().getRedeemFragment();
				rf.setParent(mMenuID);
				rf.fragmentActive(true);
				if(args.isEmpty() == false) {
					rf.setArguments(args);
				}
				transaction.add(R.id.frameLayoutMiddle, rf, fID.toString());
				break;
			case WEB:
				((MainActivity) getActivity()).setFragmentID(fID);
				((MainActivity) getActivity()).setParentFragmentID(mMenuID);
				WebFragment wf = DataManager.getInstance().getWebFragment();
				wf.setArguments(args);
				wf.setParent(mMenuID);
				wf.fragmentActive(true);
				transaction.add(R.id.frameLayoutMiddle, wf, fID.toString());
				break;
			default:
				break;
	    }
	    // add transaction to back stack if we want to go back to where we were
	    if(goBack) {
	    	DataManager.getInstance().pushToMenuStack(fID);
	    }
	    
		// Commit the transaction
		transaction.commit();
	}
	
	public void cancelAllTasks() {
		cancelLoadLocationsTask();
		DataManager.getInstance().cancelCheckInOrRedeemTask();
	}

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	public void loadLocations(boolean forceRemove) {
		if(isAdded()) ((MainActivity) getActivity()).showSpinner();

		if(mLoadingLocationsBlock == false) {
			mLoadingLocationsBlock = true;
			mLoadLocationsTask = new LoadLocationsTask();
			if (mParent != null && (forceRemove || mLocationIndex == 1)) {
				if (mParent.findViewById(R.id.linearLayoutSearchBusiness) != null) {
					((LinearLayout) mParent.findViewById(R.id.linearLayoutSearchBusiness)).removeAllViewsInLayout();
				}
				mLocationIndex = 1;
			}
			// task object could be corrupted or canceled
			// when switching menus
			if (mLoadLocationsTask != null) {
				if (Utils.hasHoneycomb()) {
					mLoadLocationsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
				} else {
					mLoadLocationsTask.execute();
				}
			}
		}
	}
	
    private class LoadLocationsTask extends AsyncTask<Void, View, Integer> {
        protected Integer doInBackground(Void... params) {
			DataManager dm = DataManager.getInstance();
			UTCGeolocationManager lm = UTCGeolocationManager.getInstance();

			int locationNum = 0;

			if((MainActivity) getActivity() == null) {
				Logger.verbose(mName, "main == null in LoadLocationsTask");
				return 0;
			}

			// make sure we can actually get a location (user must allow us)
			if(lm.areProvidersOff()) {
				Logger.info(mName, "lm.areProvidersOff == true in LoadLocationsTask");
			}

			// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadLocationsTask");
				return 0;
			}

			/////////////////////////////////////////////////////////////////////////
			//////////   Retrieve location data   ///////////////////////////////////
			/////////////////////////////////////////////////////////////////////////

			// needing a location update should be determined by our update scheme,
			// which is currently always true upon first starting up application
			if(dm.doLocationsNeedUpdated() || mLocationsLayout == null) {
				Logger.info(mName, "updating locations");
				if(LoginManager.getInstance().userLoggedIn()) {
					mLocationRetrievalError = LocationParser.setLocations(LoginManager.getInstance().getAccountContext().getToken());
				}
				else {
					mLocationRetrievalError = LocationParser.setLocations();
				}
				if(mLocationRetrievalError == null) {
					dm.sortLocationsByDistance();
					dm.sortLocationsByName();
					// we just updated, so let's not update until someone else says so
					dm.setLocationsNeedUpdated(false);
					mLocationRetrievalError = FavoritesParser.setFavorites();
				}
			}

			/////////////////////////////////////////////////////////////////////////
			//////////   Create location layouts   //////////////////////////////////
			/////////////////////////////////////////////////////////////////////////
			ArrayList<Integer> listID = dm.getIDs();

			TextView tv;
			ImageView iv;
			Button b;

			int idNum = Constants.BASE_CUSTOM_IDS + (mLocationIndex - 1);
			int idOffset = 0;
			int indexMax = mLocationIndex;
			if(mSearching) {
				indexMax += listID.size();
			}
			else {
				indexMax += Constants.LOCATION_CHUNK_SIZE;
			}

			mLocationsLayout = new LinearLayout(mParent.getContext());
			mLocationsLayout.setId(idNum + idOffset++);
			mLocationsLayout.setOrientation(LinearLayout.VERTICAL);

			Logger.verbose(mName, "number of IDs - " + String.valueOf(listID.size()));
			LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			for(; mLocationIndex <= listID.size() && mLocationIndex < indexMax; mLocationIndex++) {
				Logger.verbose(mName, String.valueOf(mLocationIndex) + " adding location ID " + listID.get(mLocationIndex - 1));

				// keep checking if this task was canceled and bail if so
				if(isCancelled() == true) {
					Logger.verbose(mName, "isCancelled == true in LoadLocationsTask (loop)");
					return 0;
				}

				UTCLocation utcLoc = dm.getLocation(listID.get(mLocationIndex - 1));

				String allLocationDetails = "";

				String name = "";
				String address = "";
				String category = "";

				if(utcLoc.containsKey(LocationParser.JSON_TAG_NAME)) {
					name = utcLoc.get(LocationParser.JSON_TAG_NAME);
					allLocationDetails = allLocationDetails + name;
				}

				if(utcLoc.containsKey(LocationParser.JSON_TAG_ADDRESS)) {
					address = utcLoc.get(LocationParser.JSON_TAG_ADDRESS);
					allLocationDetails = allLocationDetails + address;
				}

				if(utcLoc.containsKey(LocationParser.JSON_TAG_CATNAME)) {
					category = utcLoc.get(LocationParser.JSON_TAG_CATNAME);
					if(utcLoc.containsKey(LocationParser.JSON_TAG_PROPERTIES + "Size")) {
						int propertiesNum = Integer.valueOf(utcLoc.get(LocationParser.JSON_TAG_PROPERTIES + "Size")).intValue();
						for(int j = 0; j < propertiesNum; j++) {
							category = category + ", " + utcLoc.get(LocationParser.JSON_TAG_PROPERTIES + String.valueOf(j));
						}
					}
					allLocationDetails = allLocationDetails + category;
				}

				boolean entertainer = Utils.convertToBoolean(utcLoc.get(LocationParser.JSON_TAG_ENTERTAINER));
				boolean searchMatch = allLocationDetails
						.toUpperCase(Locale.getDefault())
						.contains(mSearchFilter
								.toUpperCase(Locale.getDefault()));

				if (mBusinessType.equals("Entertainer")) {
					searchMatch = searchMatch && entertainer;
				}
				else {
					searchMatch = searchMatch && !entertainer;
				}

				if(searchMatch && !dm.isCategoryAntifiltered(
						Integer.parseInt(utcLoc.get(LocationParser.JSON_TAG_CATID))) &&
								utcLoc.containsKey(LocationParser.JSON_TAG_ID)) {
					final Bundle args = new Bundle();
					args.putString(LocationParser.JSON_TAG_NAME, utcLoc.get(LocationParser.JSON_TAG_NAME));
					args.putString(LocationParser.JSON_TAG_BUSGUID, utcLoc.get(LocationParser.JSON_TAG_BUSGUID));
					args.putString(LocationParser.JSON_TAG_ID, utcLoc.get(LocationParser.JSON_TAG_ID));
					args.putString(LocationParser.JSON_TAG_DISTANCE, utcLoc.get(LocationParser.JSON_TAG_DISTANCE));

					View child = null;
					try {
						child = inflater.inflate(R.layout.location_details, null, false);
					}
					catch(InflateException ie) {
						Logger.error(mName, "could not inflate child view");
						throw new RuntimeException(ie);
					}

					child.setId(idNum + idOffset++);

					tv = (TextView) child.findViewById(R.id.redeemLocationName);
					tv.setId(idNum + idOffset++);
					tv.setClickable(true);
					tv.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_NAME)) {
						tv.setText(utcLoc.get(LocationParser.JSON_TAG_NAME));
					}

					tv = (TextView) child.findViewById(R.id.redeemDistance);
					tv.setId(idNum + idOffset++);
					tv.setClickable(true);
					tv.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_DISTANCE)) {
						double dist = Double.parseDouble(utcLoc.get(LocationParser.JSON_TAG_DISTANCE));
						if(dist < Double.parseDouble(Constants.LOCATION_MISSING_TAG)) {
							tv.setText(utcLoc.get(LocationParser.JSON_TAG_DISTANCE) + " mi");
						}
					}

					tv = (TextView) child.findViewById(R.id.redeemAddress);
					tv.setId(idNum + idOffset++);
					tv.setClickable(true);
					tv.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_ADDRESS)) {
						tv.setText(utcLoc.get(LocationParser.JSON_TAG_ADDRESS));
					}

					// keep checking if this task was canceled and bail if so
					if(isCancelled() == true) {
						Logger.verbose(mName, "isCancelled == true in LoadLocationsTask (loop)");
						return 0;
					}

					iv = (ImageView) child.findViewById(R.id.redeemRating);
					//mViewIDsForListeners.add(Integer.valueOf(idNum + idOffset));
					iv.setId(idNum + idOffset++);
					tv.setClickable(true);
					tv.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_RATING)) {
						Drawable rating = getResources().getDrawable(Constants.ratingIds[Integer.parseInt(utcLoc.get(LocationParser.JSON_TAG_RATING))]);
						iv.setImageDrawable(rating);
					}

					tv = (TextView) child.findViewById(R.id.redeemCategory);
					tv.setId(idNum + idOffset++);
					tv.setClickable(true);
					tv.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_CATNAME)) {
						tv.setText(category.toUpperCase(Locale.getDefault()));
					}

					LinearLayout redemptionDetails = (LinearLayout) child.findViewById(R.id.linearLayoutRedemptionDetails);
					LinearLayout redemptionExists = (LinearLayout) child.findViewById(R.id.linearLayoutRedemptionExists);
					redemptionDetails.setId(idNum + idOffset++);
					redemptionDetails.setClickable(true);
					redemptionDetails.setOnTouchListener(new LocationTouchListener(args));
					redemptionExists.setId(idNum + idOffset++);
					redemptionExists.setClickable(true);
					redemptionExists.setOnTouchListener(new LocationTouchListener(args));
					if(utcLoc.containsKey(LocationParser.JSON_TAG_MYISREDEEMED)) {
						String redeemed = utcLoc.get(LocationParser.JSON_TAG_MYISREDEEMED);

						if(redeemed.equals("true")) {
							redemptionDetails.setVisibility(LinearLayout.GONE);
							redemptionExists.setVisibility(LinearLayout.VISIBLE);

							if(utcLoc.containsKey(LocationParser.JSON_TAG_MYREDEEMDATE)) {
								String redemptionDate = utcLoc.get(LocationParser.JSON_TAG_MYREDEEMDATE);

								b = (Button) child.findViewById(R.id.buttonLocationRedemptionDate);
								b.setId(idNum + idOffset++);
								b.setText(redemptionDate);
							}
						}
						else {
							if(utcLoc.containsKey(LocationParser.JSON_TAG_DEALAMOUNT)) {
								String redemptionAmount = utcLoc.get(LocationParser.JSON_TAG_DEALAMOUNT);
								tv = (TextView) child.findViewById(R.id.textViewLocationRedemptionAmount);
								tv.setId(idNum + idOffset++);
								tv.setText(redemptionAmount);
							}
							final String socialTermsURL = Constants.LOCATION_TERMS_SOCIAL_DEAL_URL + utcLoc.get(LocationParser.JSON_TAG_ID);
							b = (Button) child.findViewById(R.id.buttonLocationDetails);
							b.setId(idNum + idOffset++);
							b.setClickable(true);
							b.setOnClickListener(new View.OnClickListener() {
								@Override
								public void onClick(View v) {
									final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
									// Vibrate for Constants.VIBRATE_LENGTH milliseconds
									vib.vibrate(Constants.VIBRATE_LENGTH);

									Bundle args = new Bundle();
									args.putString(Constants.WEB_FRAGMENT_URL_ARG, socialTermsURL);
									args.putString(Constants.WEB_FRAGMENT_TITLE, "UTC SOCIAL DEAL");
									replaceSubmenuFragment(Constants.MenuID.WEB, args, true);
								}
							});
							b = (Button) child.findViewById(R.id.buttonLocationRedeem);
							b.setId(idNum + idOffset++);
							b.setClickable(true);
							b.setOnTouchListener(new RedeemTouchListener(args));
						}
					}

					RelativeLayout container = (RelativeLayout) child.findViewById(R.id.relativeLayoutLocationContainer);
					container.setId(idNum + idOffset++);
					container.setClickable(true);
					container.setOnTouchListener(new LocationTouchListener(args));
					iv = (ImageView) child.findViewById(R.id.imageViewLocationFavorite);
					if(dm.getFavorites().contains(listID.get(mLocationIndex - 1))) {
						container.setBackgroundColor(Constants.FAVORITE_LOCATION_COLOR);
						iv.setVisibility(ImageView.VISIBLE);
					}

					String url = Constants.LOCATION_INFO_IMAGE + "/" + utcLoc.get(LocationParser.JSON_TAG_BUSGUID) + "@2x.png";
					ImageView logo = (ImageView) child.findViewById(R.id.imageViewLocationDetail);
					logo.setId(idNum + idOffset++);
					addImage(url, logo);

					mLocationProgress = mLocationIndex;
					publishProgress(child);
					if(mSearching && mLocationIndex == indexMax - 1) {
						try {
							Thread.sleep(1000);
							indexMax += Constants.LOCATION_CHUNK_SIZE;
						}
						catch(InterruptedException ex) {
							// do nothing, we don't care if this was interrupted
						}
					}
					else {
						try {
							Thread.sleep(Constants.LAYOUT_ADDITION_DELAY);
						} catch (InterruptedException e) {
							// don't do anything, we don't care if
							// a sleep is interrupted
						}
					}
				}


			}

			return locationNum;
        }
        
        @Override
        protected void onProgressUpdate(View... child) {
        	super.onProgressUpdate(child[0]);

        	if(mLocationProgress == 1) {
            	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setMax(DataManager.getInstance().getIDs().size());
            	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setVisibility(ProgressBar.VISIBLE);
        	}

        	try {
        		((LinearLayout) mParent.findViewById(R.id.linearLayoutSearchBusiness)).addView(child[0]);
        	}
        	catch(IllegalStateException ise) {
        		ise.printStackTrace();
        	}
        	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setProgress(mLocationProgress);
        }

        protected void onPostExecute(Integer locationNum) {
        	((MainActivity) getActivity()).hideSpinner();
        	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setVisibility(ProgressBar.GONE);
        	if(UTCGeolocationManager.getInstance().areProvidersOff() &&
        			mNotifiedAboutLocationProviders == false) {
        		Toast.makeText(mParent.getContext(), "Please turn on a location provider", Toast.LENGTH_LONG).show();
        		mNotifiedAboutLocationProviders = true;
        	}
        	if(mLocationRetrievalError != null) {
        		Toast.makeText(mParent.getContext(), mLocationRetrievalError, Toast.LENGTH_SHORT).show();
        	}
        	mLocationRetrievalError = null;
			mLoadingLocationsBlock = false;
        }
    }
    
    public boolean cancelLoadLocationsTask() {
    	boolean result = false;
		mLoadingLocationsBlock = false;
    	if(mLoadLocationsTask != null) {
    		Logger.info(mName, "LoadLocationsTask cancelled");
    		result = mLoadLocationsTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoadLocationsTask was null when cancelling");
    	}
    	return result;
    }

	private void showRedeemDialog(String busName, String dealAmount) {
		// Create and show the dialog
		mUTCDialog = UTCDialogFragment.newInstance("RedeemDialog", -1, -1, "", busName, null, dealAmount, true);
		mUTCDialog.show(getFragmentManager(), "RedeemDialog");
	}

	private void showRedeemRewardDialog(int locationID, String dealAmount, String dealDesc, int businessID, String busName, String busImage, String error) {
		Logger.info(mName, "Show redeem reward dialog: " + busName + ", " + dealAmount);

		// create and show redeem reward dialog
		mRewardDialog = RedeemRewardDialog.newInstance(busName, dealAmount, dealDesc, busImage, String.valueOf(locationID), String.valueOf(businessID), error);
		mRewardDialog.show(getFragmentManager());
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

	public void showGuestRedeemDialog() {
		// Create and show the dialog
		mUTCDialog = UTCDialogFragment.newInstance("GuestRedeemDialog");
		mUTCDialog.show(getFragmentManager(), "GuestRedeemDialog");
	}

	public void showPINDialog() {
		// Create and show the dialog
		DataManager.getInstance().setRequestingPIN(true);
		mUTCDialog = UTCDialogFragment.newInstance("PINDialog");
		mUTCDialog.show(getFragmentManager(), "PINDialog");
	}

	public void dismissRedeemDialog() {
		if(mUTCDialog != null) {
			mUTCDialog.dismiss();
		}
	}

	public void dismissPINDialog() {
		if(mUTCDialog != null) {
			mUTCDialog.dismiss();
		}
	}

    public void removeAllLocationResults() {
    	LinearLayout results = (LinearLayout) mParent.findViewById(R.id.linearLayoutSearchBusiness);
    	results.removeAllViews();
    }
    
    private class LocationTouchListener implements View.OnTouchListener {
		
    	public Bundle mArguments;
    	
    	public LocationTouchListener(Bundle args) {
    		mArguments = args;
    	}
    	
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				replaceSubmenuFragment(Constants.MenuID.REDEEM, mArguments, true);
			}
			
			return false;
		}
	}

	private class RedeemTouchListener implements View.OnTouchListener {

		public Bundle mArguments;

		public RedeemTouchListener(Bundle args) {
			mArguments = args;
		}

		@Override
		public boolean onTouch(View v, MotionEvent event) {

			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				mLocationID = Integer.valueOf(mArguments.getString(LocationParser.JSON_TAG_ID));

				Logger.info(mName, "redeem touched, ID: " + mLocationID.toString());

				loadLocation();
			}

			return false;
		}
	}

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	private void loadLocation() {
		mLoadLocationTask = new LoadLocationTask();
		if(Utils.hasHoneycomb()) {
			mLoadLocationTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mLocationID);
		}
		else {
			mLoadLocationTask.execute(mLocationID);
		}
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

			UTCLocation utcLoc = dm.getLocationContext();
			String image = Constants.LOCATION_INFO_IMAGE + "/" + utcLoc.get(LocationContextParser.JSON_TAG_BUSGUID) + "@2x.png";

			// Perform action on click
			if(LoginManager.getInstance().userLoggedIn()) {
				// show member dialog
				showRedeemRewardDialog(Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_ID)),
						utcLoc.get(LocationContextParser.JSON_TAG_DEAL_AMOUNT),
						utcLoc.get(LocationContextParser.JSON_TAG_DEAL_DESCRIPTION),
						Integer.valueOf(utcLoc.get(LocationContextParser.JSON_TAG_BUSID)),
						utcLoc.get(LocationContextParser.JSON_TAG_NAME),
						image,
						"");
			}
			else {
				// show nonmember dialog
				showGuestRedeemDialog();
			}

			if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
		}
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

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	private void addImage(String url, ImageView iv) {
		AddImageTask ait = new AddImageTask(iv);
		if (Utils.hasHoneycomb()) {
			ait.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, url);
		} else {
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
			if(img != null && mIv != null && img.getDrawable() != null && mIv.getDrawable() != null) {
				if(((BitmapDrawable)mIv.getDrawable()).getBitmap() != null && ((BitmapDrawable)img.getDrawable()).getBitmap() != null) {
					mIv.setImageBitmap(((BitmapDrawable)img.getDrawable()).getBitmap());
				}
			}
		}
	}
}