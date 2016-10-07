package com.unitethiscity.data;

import java.text.NumberFormat;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class LocationContextParser {

	private static String mName = Constants.LOCATION_CONTEXT_PARSER;
	
	public static final String 	JSON_TAG_LOCATIONS 						= ""; // array with no name
	public static final String 	JSON_TAG_PROPERTIES 					= "Properties"; // array of strings
	public static final String 	JSON_TAG_ID								= "Id";
	public static final String 	JSON_TAG_BUSID 							= "BusId";
	public static final String 	JSON_TAG_BUSGUID 						= "BusGuid";
    public static final String 	JSON_TAG_CITID 							= "CitId";
    public static final String 	JSON_TAG_NAME 							= "Name";
    public static final String 	JSON_TAG_ADDRESS 						= "Address";
    public static final String 	JSON_TAG_RATING 						= "Rating";
    public static final String 	JSON_TAG_LATITUDE 						= "Latitude";
    public static final String 	JSON_TAG_LONGITUDE 						= "Longitude";
    public static final String 	JSON_TAG_CATID 							= "CatId";
    public static final String 	JSON_TAG_CATNAME 						= "CatName";
    public static final String 	JSON_TAG_ENTERTAINER					= "Entertainer";
    public static final String 	JSON_TAG_DISTANCE 						= "Distance"; // not an actual used JSON tag, but used for storage locally
    public static final String 	JSON_TAG_SUMMARY 						= "Summary";
    public static final String 	JSON_TAG_PHONE 							= "Phone";
    public static final String 	JSON_TAG_WEBSITE 						= "Website";
    public static final String 	JSON_TAG_FACEBOOK_LINK 					= "FacebookLink";
    public static final String 	JSON_TAG_FACEBOOK_ID 					= "FacebookId";
    public static final String 	JSON_TAG_REQUIRES_PIN 					= "RequiresPIN";
    public static final String 	JSON_TAG_DEAL_ID 						= "DealId";
    public static final String 	JSON_TAG_DEAL_AMOUNT 					= "DealAmount";
    public static final String  JSON_TAG_DEAL_DESCRIPTION               = "DealDescription";
    public static final String  JSON_TAG_DEAL_NAME                      = "DealName";
    public static final String 	JSON_TAG_CUSTOM_TERMS 					= "CustomTerms";
    public static final String 	JSON_TAG_ACCOUNT_ID 					= "AccId";
    public static final String 	JSON_TAG_MY_TIP 						= "MyTip";
    public static final String 	JSON_TAG_MY_RATING	 					= "MyRating";
    public static final String 	JSON_TAG_MY_IS_FAVORITE 				= "MyIsFavorite";
    public static final String 	JSON_TAG_MY_IS_CHECKED_IN				= "MyIsCheckedIn";
    public static final String 	JSON_TAG_MY_IS_REDEEMED 				= "MyIsRedeemed";
    public static final String 	JSON_TAG_MY_CHECK_IN_TIME				= "MyCheckInTime";
    public static final String 	JSON_TAG_MY_REDEEM_DATE					= "MyRedeemDate";
    public static final String  JSON_TAG_NUM_MENU_ITEMS                 = "NumMenuItems";
    public static final String  JSON_TAG_NUM_GALLERY_ITEMS              = "NumGalleryItems";
    public static final String  JSON_TAG_NUM_EVENTS                     = "NumEvents";
    public static final String  JSON_TAG_MENU_LINK                      = "MenuLink";
    public static final String  JSON_TAG_POINTS_NEEDED                  = "PointsNeeded";
    public static final String  JSON_TAG_POINTS_COLLECTED               = "PointsCollected";
    public static final String  JSON_TAG_LOYALTY_SUMMARY                = "LoyaltySummary";
	
    public static String setLocationContext(Integer id, String context) {
        // getting JSON string from URL
    	try {
    		Logger.verbose(mName, "setting location context");
    		
    		JSONObject json = new JSONObject();
    		
        	if(context != null) {
        		json = UTCWebAPI.getUserLocationContext(context, id);
        	}
        	else {
        		json = UTCWebAPI.getLocationContext(id);
        	}

            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve location";
            }
            
            if(!json.has(JSON_TAG_ID)) {
            	return "Failed to retrieve location";
            }
    		
            // data manager instance
            DataManager dm = DataManager.getInstance();
            
            dm.initializeLocationContext(Integer.valueOf(json.getInt(JSON_TAG_ID)));
            
            if(json.has(JSON_TAG_PROPERTIES)) {
                JSONArray properties = json.getJSONArray(JSON_TAG_PROPERTIES);
                if(properties != null) {
                	dm.addToLocationContext(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));
                	
                	for(int j = 0; j < properties.length(); j++) {
                		dm.addToLocationContext(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
                	}
                }
                else {
                	Logger.error(mName, "properties null");
                }
            }
            
            dm.addToLocationContext(JSON_TAG_ID, String.valueOf(json.getInt(JSON_TAG_ID)));
            
            if(json.has(JSON_TAG_BUSID)) {
            	dm.addToLocationContext(JSON_TAG_BUSID, String.valueOf(json.getInt(JSON_TAG_BUSID)));
            }
            
            if(json.has(JSON_TAG_BUSGUID)) {
            	dm.addToLocationContext(JSON_TAG_BUSGUID, json.getString(JSON_TAG_BUSGUID));
            }
            
            if(json.has(JSON_TAG_CITID)) {
            	dm.addToLocationContext(JSON_TAG_CITID, String.valueOf(json.getInt(JSON_TAG_CITID)));
            }
            
            if(json.has(JSON_TAG_NAME)) {
            	dm.addToLocationContext(JSON_TAG_NAME, json.getString(JSON_TAG_NAME));
            }
            
            if(json.has(JSON_TAG_ADDRESS)) {
            	dm.addToLocationContext(JSON_TAG_ADDRESS, json.getString(JSON_TAG_ADDRESS));
            }
            
            if(json.has(JSON_TAG_RATING)) {
            	dm.addToLocationContext(JSON_TAG_RATING,  String.valueOf(Utils.fiveRatingToTenRating(json.getDouble(JSON_TAG_RATING))));
            }
            
            if(json.has(JSON_TAG_LONGITUDE)) {
            	dm.addToLocationContext(JSON_TAG_LONGITUDE, String.valueOf(json.getDouble(JSON_TAG_LONGITUDE)));
            }
            
            if(json.has(JSON_TAG_LATITUDE)) {
            	dm.addToLocationContext(JSON_TAG_LATITUDE, String.valueOf(json.getDouble(JSON_TAG_LATITUDE)));
            }
            
            if(json.has(JSON_TAG_CATID)) {
            	dm.addToLocationContext(JSON_TAG_CATID, String.valueOf(json.getInt(JSON_TAG_CATID)));
            }
            
            if(json.has(JSON_TAG_CATNAME)) {
            	dm.addToLocationContext(JSON_TAG_CATNAME, json.getString(JSON_TAG_CATNAME));
            }

            if(json.has(JSON_TAG_ENTERTAINER)) {
                dm.addToLocationContext(JSON_TAG_ENTERTAINER, String.valueOf(json.getBoolean(JSON_TAG_ENTERTAINER)));
            }
            
            if(json.has(JSON_TAG_SUMMARY)) {
            	dm.addToLocationContext(JSON_TAG_SUMMARY, json.getString(JSON_TAG_SUMMARY));
            }
            
            if(json.has(JSON_TAG_PHONE)) {
            	dm.addToLocationContext(JSON_TAG_PHONE, json.getString(JSON_TAG_PHONE));
            }
            
            if(json.has(JSON_TAG_WEBSITE)) {
            	dm.addToLocationContext(JSON_TAG_WEBSITE, json.getString(JSON_TAG_WEBSITE));
            }
            
            if(json.has(JSON_TAG_FACEBOOK_LINK)) {
            	dm.addToLocationContext(JSON_TAG_FACEBOOK_LINK, json.getString(JSON_TAG_FACEBOOK_LINK));
            }
            
            if(json.has(JSON_TAG_FACEBOOK_ID)) {
            	dm.addToLocationContext(JSON_TAG_FACEBOOK_ID, json.getString(JSON_TAG_FACEBOOK_ID));
            }
            
            if(json.has(JSON_TAG_REQUIRES_PIN)) {
            	dm.addToLocationContext(JSON_TAG_REQUIRES_PIN, String.valueOf(json.getBoolean(JSON_TAG_REQUIRES_PIN)));
            }
            
            if(json.has(JSON_TAG_DEAL_ID)) {
            	dm.addToLocationContext(JSON_TAG_DEAL_ID, String.valueOf(json.getInt(JSON_TAG_DEAL_ID)));
            }
            
            if(json.has(JSON_TAG_DEAL_AMOUNT)) {
            	// format the string appropriately based on it's value - zero or negative deal
            	// amounts should display as not applicable; even dollar amounts should only 
            	// display in dollar format (no decimal)
            	String dealAmountStr;
            	double dealAmount = json.getDouble(JSON_TAG_DEAL_AMOUNT);
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
            	dm.addToLocationContext(JSON_TAG_DEAL_AMOUNT, dealAmountStr);
            }

            if(json.has(JSON_TAG_DEAL_DESCRIPTION)) {
                dm.addToLocationContext(JSON_TAG_DEAL_DESCRIPTION, json.getString(JSON_TAG_DEAL_DESCRIPTION));
            }

            if(json.has(JSON_TAG_DEAL_NAME)) {
                dm.addToLocationContext(JSON_TAG_DEAL_NAME, json.getString(JSON_TAG_DEAL_NAME));
            }
            
            if(json.has(JSON_TAG_CUSTOM_TERMS)) {
            	dm.addToLocationContext(JSON_TAG_CUSTOM_TERMS, json.getString(JSON_TAG_CUSTOM_TERMS));
            }
            
            if(json.has(JSON_TAG_ACCOUNT_ID)) {
            	dm.addToLocationContext(JSON_TAG_ACCOUNT_ID, String.valueOf(json.getInt(JSON_TAG_ACCOUNT_ID)));
            }
            
            if(json.has(JSON_TAG_MY_TIP)) {
            	dm.addToLocationContext(JSON_TAG_MY_TIP, json.getString(JSON_TAG_MY_TIP));
            }
            
            if(json.has(JSON_TAG_MY_RATING)) {
            	dm.addToLocationContext(JSON_TAG_MY_RATING, String.valueOf(Utils.fiveRatingToTenRating(json.getDouble(JSON_TAG_MY_RATING))));
            }
            
            if(json.has(JSON_TAG_MY_IS_FAVORITE)) {
            	dm.addToLocationContext(JSON_TAG_MY_IS_FAVORITE, String.valueOf(json.getBoolean(JSON_TAG_MY_IS_FAVORITE)));
            }
            
            if(json.has(JSON_TAG_MY_IS_CHECKED_IN)) {
            	dm.addToLocationContext(JSON_TAG_MY_IS_CHECKED_IN, String.valueOf(json.getBoolean(JSON_TAG_MY_IS_CHECKED_IN)));
            }
            
            if(json.has(JSON_TAG_MY_IS_REDEEMED)) {
            	dm.addToLocationContext(JSON_TAG_MY_IS_REDEEMED, String.valueOf(json.getBoolean(JSON_TAG_MY_IS_REDEEMED)));
            }
            
            if(json.has(JSON_TAG_MY_CHECK_IN_TIME)) {
            	dm.addToLocationContext(JSON_TAG_MY_CHECK_IN_TIME, json.getString(JSON_TAG_MY_CHECK_IN_TIME));
            }
            
            if(json.has(JSON_TAG_MY_REDEEM_DATE)) {
            	dm.addToLocationContext(JSON_TAG_MY_REDEEM_DATE, json.getString(JSON_TAG_MY_REDEEM_DATE));
            }

            if(json.has(JSON_TAG_NUM_MENU_ITEMS)) {
                dm.addToLocationContext(JSON_TAG_NUM_MENU_ITEMS, String.valueOf(json.getInt(JSON_TAG_NUM_MENU_ITEMS)));
            }

            if(json.has(JSON_TAG_NUM_GALLERY_ITEMS)) {
                dm.addToLocationContext(JSON_TAG_NUM_GALLERY_ITEMS, String.valueOf(json.getInt(JSON_TAG_NUM_GALLERY_ITEMS)));
            }

            if(json.has(JSON_TAG_NUM_EVENTS)) {
                dm.addToLocationContext(JSON_TAG_NUM_EVENTS, String.valueOf(json.getInt(JSON_TAG_NUM_EVENTS)));
            }

            if(json.has(JSON_TAG_MENU_LINK)) {
                dm.addToLocationContext(JSON_TAG_MENU_LINK, json.getString(JSON_TAG_MENU_LINK));
            }

            if(json.has(JSON_TAG_POINTS_NEEDED)) {
                dm.addToLocationContext(JSON_TAG_POINTS_NEEDED, String.valueOf(json.getInt(JSON_TAG_POINTS_NEEDED)));
            }

            if(json.has(JSON_TAG_POINTS_COLLECTED)) {
                dm.addToLocationContext(JSON_TAG_POINTS_COLLECTED, String.valueOf(json.getInt(JSON_TAG_POINTS_COLLECTED)));
            }

            if(json.has(JSON_TAG_LOYALTY_SUMMARY)) {
                dm.addToLocationContext(JSON_TAG_LOYALTY_SUMMARY, json.getString(JSON_TAG_LOYALTY_SUMMARY));
            }
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }

        return null;
    }
}