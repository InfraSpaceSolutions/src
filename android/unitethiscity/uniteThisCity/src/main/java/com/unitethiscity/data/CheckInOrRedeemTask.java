package com.unitethiscity.data;

import java.text.NumberFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.data.DataManager.ScanTask;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.ui.InteractiveScrollView;
import com.unitethiscity.ui.MainActivity;

import android.location.Location;
import android.os.AsyncTask;
import android.widget.Toast;

public class CheckInOrRedeemTask extends AsyncTask<String, Void, Boolean> {
	
	private String mName = Constants.CHECK_IN_OR_REDEEM_TASK;
	
	private JSONObject mJSONReply;
	private MainActivity mMain;
	
	private boolean mFirstPINRequest;
	private int mRole;
	private int mLocationID;
	private boolean mCheckIn;
	private boolean mRedeem;
	
	public CheckInOrRedeemTask(MainActivity main, int role, int locID) {
		this.mMain = main;
		mFirstPINRequest = false;
		mRole = role;
		mLocationID = locID;
		mCheckIn = false;
		mRedeem = false;
	}
	
	public CheckInOrRedeemTask(MainActivity main, int role, int locID, boolean checkIn, boolean redeem) {
		this.mMain = main;
		mFirstPINRequest = false;
		mRole = role;
		mLocationID = locID;
		mCheckIn = checkIn;
		mRedeem = redeem;
	}
	
    protected Boolean doInBackground(String... params) {
		String qurl = params[0];
		
		mJSONReply = null;

		double lat = 0.0;
		double lon = 0.0;

		Location loc = UTCGeolocationManager.getInstance().getBestLocation();

		if(loc != null) {
			lat = loc.getLatitude();
			lon = loc.getLongitude();
		}

		if(lat == 0.0 && lon == 0.0) {
			lat = Constants.LOCATION_DEFAULT_LATITUDE;
			lon = Constants.LOCATION_DEFAULT_LONGITUDE;
		}
		
		if(DataManager.getInstance().getScanTask().equals(ScanTask.CHECKIN)) {
    		if(mRole == 2) {
        		mJSONReply = UTCWebAPI.checkIn(LoginManager.getInstance().getAccountContext().getToken(), 
    					Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ACCOUNT_ID)).intValue(), 
    					mRole,
    					Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ID)), 
    					LoginManager.getInstance().getAccountContext().getAccountID(), 
    					qurl, 
    					lat, 
    					lon);
    		}
    		else if(mRole == 3) {
        		mJSONReply = UTCWebAPI.checkIn(LoginManager.getInstance().getAccountContext().getToken(), 
        				LoginManager.getInstance().getAccountContext().getAccountID(), 
    					mRole,
    					mLocationID, 
    					0, 
    					qurl, 
    					lat, 
    					lon);
    		}
		}
		else if(DataManager.getInstance().getScanTask().equals(ScanTask.REDEEM)) {
    		if(mRole == 2) {
        		mJSONReply = UTCWebAPI.redeem(LoginManager.getInstance().getAccountContext().getToken(), 
    					Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ACCOUNT_ID)).intValue(), 
    					mRole,
    					Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ID)), 
    					Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_DEAL_ID)).intValue(), 
    					LoginManager.getInstance().getAccountContext().getAccountID(), 
    					qurl, 
    					DataManager.getInstance().getPIN(),
    					lat, 
    					lon);
    		}
    		else if(mRole == 3) {
        		mJSONReply = UTCWebAPI.redeem(LoginManager.getInstance().getAccountContext().getToken(), 
        				LoginManager.getInstance().getAccountContext().getAccountID(), 
    					mRole,
    					mLocationID, 
    					0, 
    					0, 
    					qurl, 
    					DataManager.getInstance().getPIN(),
    					lat, 
    					lon);
    		}
    		
    		// clear PIN after request
    		DataManager.getInstance().setPIN("");
		}
		else if(DataManager.getInstance().getScanTask().equals(ScanTask.UNIFIED_ACTION)) {
    		mJSONReply = UTCWebAPI.unifiedAction(LoginManager.getInstance().getAccountContext().getToken(), 
    					LoginManager.getInstance().getAccountContext().getAccountID(), 
    					mRole,
    					LoginManager.getInstance().getAccountContext().getAccountID(), 
    					qurl, 
    					DataManager.getInstance().getPIN(),
    					lat, 
    					lon,
    					mCheckIn,
    					mRedeem);
    		
    		// clear PIN after request
    		DataManager.getInstance().setPIN("");
		}
		
		return Boolean.valueOf(true);
    }
    
    protected void onPostExecute(Boolean success) {
    	if(!DataManager.getInstance().getScanTask().equals(ScanTask.UNIFIED_ACTION)) {
    		// dismiss redeem dialog
    		if(mRole == 2) {
    			if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
    				DataManager.getInstance().getUTCFragment().dismissRedeemDialog();
    			}
    			else if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
    				DataManager.getInstance().getRedeemFragment().dismissRedeemDialog();
    			}
				else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
					DataManager.getInstance().getFavoriteFragment().dismissRedeemDialog();
				}
    		}
    		else if(mRole == 3) {
    			DataManager.getInstance().getBusinessFragment().dismissBusinessDialog();
    		}
    	}
    	
    	if(success.booleanValue()) {
    		DataManager.getInstance().setWalletNeedsUpdate(true);

    		if(mMain != null) {
				String locIDStr = DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_ID);
				int busID = Integer.valueOf(DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_BUSID)).intValue();
				String description = DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_SUMMARY);
				String busName = DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_NAME);
				String image = Constants.LOCATION_INFO_IMAGE + "/" + DataManager.getInstance().getLocationContext().get(LocationContextParser.JSON_TAG_BUSGUID) + "@2x.png";
				String errorMessage = null;

        		if(DataManager.getInstance().getScanTask().equals(ScanTask.CHECKIN)) {
        			if(mRole == 2) {

        				if(mJSONReply != null && mJSONReply.has("Message")) {
							Logger.info(mName, "reply from check-in request:");
        					try {
								errorMessage = mJSONReply.getString("Message");
								Logger.info(mName, errorMessage);
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
						else {
							DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN, "true");
							SimpleDateFormat sdf = new SimpleDateFormat("M/dd/yyyy H:mm a");
							String currentDateandTime = sdf.format(new Date());
							DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_CHECK_IN_TIME, currentDateandTime);
						}

						if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
							DataManager.getInstance().getUTCFragment().showLoyaltyDialog(
									Integer.valueOf(locIDStr).intValue(),
									busID, busName, image, errorMessage);
						}
						else if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
							DataManager.getInstance().getRedeemFragment().showLoyaltyDialog(
									Integer.valueOf(locIDStr).intValue(),
									busID, busName, image, errorMessage);
						}
						else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
							DataManager.getInstance().getFavoriteFragment().showLoyaltyDialog(
									Integer.valueOf(locIDStr).intValue(),
									busID, busName, image, errorMessage);
						}
						else if(DataManager.getInstance().fromBigButton()) {
	        				DataManager.getInstance().getUTCFragment().showLoyaltyDialog(
									Integer.valueOf(locIDStr).intValue(),
									busID, busName, image, errorMessage);
        				}
        				
        				/* can no longer automatically post with prefilled message
        				mMain.publishStory(null,
        						null,
        						null, 
        						Constants.BUSINESS_DIRECTORY + locIDStr + Constants.BUSINESS_DIRECTORY_NO_MAPS, 
        						null,
        						Constants.DEFAULT_DESCRIPTION_PREFIX + busName + ".",
        						busID); */
        			}
        			else if(mRole == 3) {
        				DataManager.getInstance().getBusinessFragment().showLoyaltyDialog(
								Integer.valueOf(locIDStr).intValue(),
								busID, busName, image, errorMessage);
        			}
        		}
        		else if(DataManager.getInstance().getScanTask().equals(ScanTask.REDEEM)) {
        			if(mRole == 2) {
            			if(DataManager.getInstance().requestingPIN()) {
            				Logger.info(mName, "was requesting PIN");
            				DataManager.getInstance().setRequestingPIN(false);
            				DataManager.getInstance().setPIN("");
            				if(DataManager.getInstance().fromBigButton()) {
            					DataManager.getInstance().getUTCFragment().dismissPINDialog();
            				}
							else {
								if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
									DataManager.getInstance().getRedeemFragment().dismissPINDialog();
								}
								else if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
									DataManager.getInstance().getUTCFragment().dismissPINDialog();
								}
								else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
									DataManager.getInstance().getFavoriteFragment().dismissPINDialog();
								}
								else if(DataManager.getInstance().getSearchBusinessFragment().isFragmentActive()) {
									DataManager.getInstance().getSearchBusinessFragment().dismissPINDialog();
								}
							}
            			}
        				
        				if(mJSONReply != null && mJSONReply.has("Message")) {
							Logger.info(mName, "reply from redeem request:");
        					try {
								errorMessage = mJSONReply.getString("Message");
								Logger.info(mName, errorMessage);
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
            			
            			if(errorMessage != null && errorMessage.startsWith("PIN")) {
        					mFirstPINRequest = true;
        					if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
								Logger.info(mName, "requesting PIN from UTC");
								//DataManager.getInstance().getUTCFragment().dismissRedeemDialog();
            					DataManager.getInstance().getUTCFragment().showPINDialog();
            				}
            				else if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
								Logger.info(mName, "requesting PIN from Redeem");
								//DataManager.getInstance().getRedeemFragment().dismissRedeemDialog();
            					DataManager.getInstance().getRedeemFragment().showPINDialog();
            				}
							else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
								Logger.info(mName, "requesting PIN from Favorite");
								//DataManager.getInstance().getFavoriteFragment().dismissRedeemDialog();
								DataManager.getInstance().getFavoriteFragment().showPINDialog();
							}
							else if(DataManager.getInstance().getSearchBusinessFragment().isFragmentActive()) {
								Logger.info(mName, "requesting PIN from Search Business");
								//DataManager.getInstance().getSearchBusinessFragment().dismissRedeemDialog();
								DataManager.getInstance().getSearchBusinessFragment().showPINDialog();
							}
            			}
            			else {
							DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_IS_REDEEMED, "true");
							SimpleDateFormat sdf = new SimpleDateFormat("M/dd/yyyy H:mm a");
							String currentDateandTime = sdf.format(new Date());
							DataManager.getInstance().addToLocationContext(LocationContextParser.JSON_TAG_MY_REDEEM_DATE, currentDateandTime);

							if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
								Logger.info(mName, "update redeem reward dialog from Redeem");
								DataManager.getInstance().getRedeemFragment().updateReedeemRewardDialog(errorMessage);
							}
							else if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
								Logger.info(mName, "update redeem reward dialog from UTC");
								DataManager.getInstance().getUTCFragment().updateReedeemRewardDialog(errorMessage);
							}
							else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
								Logger.info(mName, "update redeem reward dialog from Favorite");
								DataManager.getInstance().getFavoriteFragment().updateReedeemRewardDialog(errorMessage);
							}
							else if(DataManager.getInstance().getSearchBusinessFragment().isFragmentActive()) {
								Logger.info(mName, "update redeem reward dialog from Search Business");
								DataManager.getInstance().getSearchBusinessFragment().updateReedeemRewardDialog(errorMessage);
							}
            			}
            			
            			/* can no longer automatically post with prefilled message
        				mMain.publishStory(null,
        						null,
        						null, 
        						Constants.BUSINESS_DIRECTORY + locIDStr + Constants.BUSINESS_DIRECTORY_NO_MAPS, 
        						null,
        						Constants.DEFAULT_DESCRIPTION_PREFIX + busName + ".",
        						busID);
        						( */
        			}
        			else if(mRole == 3) {
            			if(DataManager.getInstance().requestingPIN()) {
            				DataManager.getInstance().setRequestingPIN(false);
            				DataManager.getInstance().setPIN("");
            				DataManager.getInstance().getBusinessFragment().dismissPINDialog();
            			}
            			DataManager.getInstance().getBusinessFragment().updateReedeemRewardDialog(errorMessage);
        			}
        		}
        		else if(DataManager.getInstance().getScanTask().equals(ScanTask.UNIFIED_ACTION) && mJSONReply != null) {
        			if(mJSONReply.has("Message") && !DataManager.getInstance().requestingPIN()) {
        				String message = "";
        				try {
							message = mJSONReply.getString("Message");
						} catch (JSONException e) {
							e.printStackTrace();
						}
        				
        				if(message.startsWith("PIN")) {
        					Logger.info(mName, "requesting PIN");
        					mFirstPINRequest = true;
                			DataManager.getInstance().getMainFragment()
            					.showPINDialog();
        				}
        				else {
        					DataManager.getInstance().getMainFragment()
        						.dismissRedeemDialog();
        					
                			if(DataManager.getInstance().getMainFragment().checkInClicked()) {
                    			DataManager.getInstance().getMainFragment()
                					.showCheckInConfirmDialog(-1, -1, null, message);
                			}
                			else if(DataManager.getInstance().getMainFragment().redeemClicked()) {
                    			DataManager.getInstance().getMainFragment()
                					.showRedeemConfirmDialog(-1, -1, null, message);
                			}
        				}
        			}
        			else if(mJSONReply.has("Message")) {
        				try {
							Toast.makeText(mMain, mJSONReply.getString("Message"), Toast.LENGTH_SHORT).show();
						} catch (JSONException e) {
							e.printStackTrace();
						}
        			}
        			else if(!mJSONReply.has("Message")) {
        				boolean checkedIn = false;
        				boolean redeemed = false;
        				String dealAmountStr = "";
        				busName = "";
        				locIDStr = "";
        				busID = 0;
        				if(mJSONReply.has("CheckedIn")) {
        					try {
								checkedIn = mJSONReply.getBoolean("CheckedIn");
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
        				if(mJSONReply.has("Redeemed")) {
        					try {
								redeemed = mJSONReply.getBoolean("Redeemed");
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
        				if(mJSONReply.has("DealAmount")) {
        	            	// format the string appropriately based on it's value - zero or negative deal
        	            	// amounts should display as not applicable; even dollar amounts should only 
        	            	// display in dollar format (no decimal)
        	            	double dealAmount = 0.0;
        	            	try {
								dealAmount = mJSONReply.getDouble("DealAmount");
							} catch (JSONException e) {
								e.printStackTrace();
							}
        	            	if(dealAmount <= 0) {
        	            		dealAmountStr = "N/A";
        	            	}
        	            	else {
        	            		int dealAmountInt = (int) dealAmount;
        	            		if(dealAmountInt == dealAmount) {
        	            			dealAmountStr = String.valueOf(dealAmountInt);
        	            			dealAmountStr = "$" + dealAmountStr;
        	            		}
        	            		else {
        	            			dealAmountStr = NumberFormat.getCurrencyInstance().format(dealAmount);
        	            		}
        	            	}
        				}
        				if(mJSONReply.has("BusName")) {
        					try {
								busName = mJSONReply.getString("BusName");
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
        				if(mJSONReply.has("LocId")) {
        					try {
								locIDStr = String.valueOf(mJSONReply.getInt("LocId"));
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
        				if(mJSONReply.has("BusId")) {
        					try {
								busID = mJSONReply.getInt("BusId");
							} catch (JSONException e) {
								e.printStackTrace();
							}
        				}
        				
        				// post to Facebook if there is an active session
        				// was:
//        				String action = checkedIn == true ? "Check In" : "Redemption";
//        				mMain.publishStory(busName, 
//        						"Unite This City " + action, 
//        						Constants.DEFAULT_DESCRIPTION_PREFIX + busName + ".", 
//        						Constants.BUSINESS_DIRECTORY + locIDStr + Constants.BUSINESS_DIRECTORY_NO_MAPS, 
//        						Constants.DEFAULT_FB_POST_IMAGE);
        				// now:
        				mMain.publishStory(null,
        						null,
        						null, 
        						Constants.BUSINESS_DIRECTORY + locIDStr + Constants.BUSINESS_DIRECTORY_NO_MAPS, 
        						null,
        						Constants.DEFAULT_DESCRIPTION_PREFIX + busName + ".",
        						busID);
        				
        				mFirstPINRequest = false;
        				
        				DataManager.getInstance().setRequestingPIN(false);
        				DataManager.getInstance().setPIN("");
        				DataManager.getInstance().getMainFragment().dismissPINDialog();
        				DataManager.getInstance().setScanData(null);
        				
    					DataManager.getInstance().getMainFragment()
							.dismissRedeemDialog();
        				
        				DataManager.getInstance().getMainFragment()
        					.showUnifiedActionConfirmDialog(
        							Integer.valueOf(locIDStr).intValue(),
        							busID,
        							checkedIn, 
        							redeemed, 
        							dealAmountStr, 
        							busName,
        							"",
        							null);
        				DataManager.getInstance().getMainFragment().startImageSwitcherTask();
        			}
        		}
    		}

			DataManager.getInstance().forceLocationUpdate();

    		if(DataManager.getInstance().getRedeemFragment().isFragmentActive()) {
				DataManager.getInstance().setLocationsNeedUpdated(true);
    			DataManager.getInstance().getRedeemFragment().loadLocation();
				DataManager.getInstance().getRedeemFragment().updateRedemptionAndLoyaltyStatus();
    		}
			else if(DataManager.getInstance().getUTCFragment().isFragmentActive()) {
				DataManager.getInstance().setLocationsNeedUpdated(true);
				DataManager.getInstance().getUTCFragment().loadLocations(true);
			}
			else if(DataManager.getInstance().getFavoriteFragment().isFragmentActive()) {
				DataManager.getInstance().setLocationsNeedUpdated(true);
				DataManager.getInstance().getFavoriteFragment().loadLocations();
			}
			else if(DataManager.getInstance().getSearchBusinessFragment().isFragmentActive()) {
				DataManager.getInstance().setLocationsNeedUpdated(true);
				DataManager.getInstance().getSearchBusinessFragment().loadLocations(true);
			}
			else if(DataManager.getInstance().getBusinessFragment().isFragmentActive()) {
				DataManager.getInstance().setLocationsNeedUpdated(true);
				DataManager.getInstance().getBusinessFragment().loadLocations();
			}
		} else {
			Logger.info(mName, "unsuccessful request");
    	}
    }
}
