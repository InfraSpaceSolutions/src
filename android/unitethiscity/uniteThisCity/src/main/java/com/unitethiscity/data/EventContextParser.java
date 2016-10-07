package com.unitethiscity.data;

import java.text.NumberFormat;

import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class EventContextParser {

	private static String mName = Constants.EVENT_CONTEXT_PARSER;
	
	public static final String 	JSON_TAG_EVENTS 						= ""; // array with no name
	public static final String 	JSON_TAG_PROPERTIES 					= "Properties"; // array of strings
	public static final String 	JSON_TAG_LOCATIONS	 					= "Locations"; // array of ints			//
	public static final String 	JSON_TAG_ID								= "Id";
	public static final String 	JSON_TAG_ETTID 							= "EttId";
	public static final String 	JSON_TAG_BUSID 							= "BusId";
	public static final String	JSON_TAG_BUSGUID						= "BusGuid";
    public static final String 	JSON_TAG_CITID 							= "CitId";
    public static final String	JSON_TAG_BUSNAME						= "BusName";							//
    public static final String 	JSON_TAG_STARTDATE 						= "StartDate";
    public static final String 	JSON_TAG_ENDDATE 						= "EndDate";
    public static final String 	JSON_TAG_DATE_AS_STR 					= "DateAsString";
    public static final String 	JSON_TAG_SORTABLE_DATE					= "SortableDate";
    public static final String 	JSON_TAG_SUMMARY 						= "Summary";
    public static final String 	JSON_TAG_EVENT_TYPE 					= "EventType";
    public static final String 	JSON_TAG_CATID 							= "CatId";
    public static final String 	JSON_TAG_CATNAME 						= "CatName";
    
    
    //
    public static final String 	JSON_TAG_BODY							= "Body";
    public static final String 	JSON_TAG_BUSINESS_SUMMARY 				= "BusinessSummary";
    public static final String 	JSON_TAG_WEBSITE 						= "Website";
    public static final String 	JSON_TAG_FACEBOOK_LINK 					= "FacebookLink";
    public static final String 	JSON_TAG_REQUIRES_PIN 					= "RequiresPIN";
    public static final String 	JSON_TAG_DEAL_ID 						= "DealId";
    public static final String 	JSON_TAG_DEAL_AMOUNT 					= "DealAmount";
    public static final String 	JSON_TAG_CUSTOM_TERMS 					= "CustomTerms";
    public static final String 	JSON_TAG_ACCOUNT_ID 					= "AccId";
    public static final String 	JSON_TAG_MY_IS_FAVORITE 				= "MyIsFavorite";
    public static final String 	JSON_TAG_MY_IS_REDEEMED 				= "MyIsRedeemed";
	
    public static String setEventContext(Integer id, String context) {
        // getting JSON string from URL
    	try {
    		Logger.verbose(mName, "setting event context");
    		
    		JSONObject json = new JSONObject();
    		
        	if(context != null) {
        		json = UTCWebAPI.getUserEventContext(context, id);
        	}
        	else {
        		json = UTCWebAPI.getEventContext(id);
        	}

            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve event";
            }
            
            if(!json.has(JSON_TAG_ID)) {
            	return "Failed to retrieve event";
            }
    		
            // data manager instance
            DataManager dm = DataManager.getInstance();
            
            dm.initializeEventContext(Integer.valueOf(json.getInt(JSON_TAG_CATID)));
            
            dm.addToEventContext(JSON_TAG_ID, String.valueOf(json.getInt(JSON_TAG_ID)));
            
            if(json.has(JSON_TAG_ETTID)) {
            	dm.addToEventContext(JSON_TAG_ETTID, String.valueOf(json.getInt(JSON_TAG_ETTID)));
            }
            
            if(json.has(JSON_TAG_BUSID)) {
            	dm.addToEventContext(JSON_TAG_BUSID, String.valueOf(json.getInt(JSON_TAG_BUSID)));
            }
            
            if(json.has(JSON_TAG_BUSGUID)) {
            	dm.addToEventContext(JSON_TAG_BUSGUID, json.getString(JSON_TAG_BUSGUID));
            }
            
            if(json.has(JSON_TAG_CITID)) {
            	dm.addToEventContext(JSON_TAG_CITID, String.valueOf(json.getInt(JSON_TAG_CITID)));
            }

            if(json.has(JSON_TAG_BUSNAME)) {
            	dm.addToEventContext(JSON_TAG_BUSNAME, json.getString(JSON_TAG_BUSNAME));
            }
            
            if(json.has(JSON_TAG_STARTDATE)) {
            	dm.addToEventContext(JSON_TAG_STARTDATE, json.getString(JSON_TAG_STARTDATE));
            }
            
            if(json.has(JSON_TAG_ENDDATE)) {
            	dm.addToEventContext(JSON_TAG_ENDDATE, json.getString(JSON_TAG_ENDDATE));
            }
            
            if(json.has(JSON_TAG_DATE_AS_STR)) {
            	dm.addToEventContext(JSON_TAG_DATE_AS_STR, json.getString(JSON_TAG_DATE_AS_STR));
            }
            
            if(json.has(JSON_TAG_SORTABLE_DATE)) {
            	dm.addToEventContext(JSON_TAG_SORTABLE_DATE, json.getString(JSON_TAG_SORTABLE_DATE));
            }
            
            if(json.has(JSON_TAG_SUMMARY)) {
            	dm.addToEventContext(JSON_TAG_SUMMARY, json.getString(JSON_TAG_SUMMARY));
            }
            
            if(json.has(JSON_TAG_EVENT_TYPE)) {
            	dm.addToEventContext(JSON_TAG_EVENT_TYPE, json.getString(JSON_TAG_EVENT_TYPE));
            }
            
            if(json.has(JSON_TAG_CATID)) {
            	dm.addToEventContext(JSON_TAG_CATID, String.valueOf(json.getInt(JSON_TAG_CATID)));
            }
            
            if(json.has(JSON_TAG_CATNAME)) {
            	dm.addToEventContext(JSON_TAG_CATNAME, json.getString(JSON_TAG_CATNAME));
            }
            
            if(json.has(JSON_TAG_BODY)) {
            	dm.addToEventContext(JSON_TAG_BODY, json.getString(JSON_TAG_BODY));
            }
            
            if(json.has(JSON_TAG_BUSINESS_SUMMARY)) {
            	dm.addToEventContext(JSON_TAG_BUSINESS_SUMMARY, json.getString(JSON_TAG_BUSINESS_SUMMARY));
            }

            if(json.has(JSON_TAG_WEBSITE)) {
            	dm.addToEventContext(JSON_TAG_WEBSITE, json.getString(JSON_TAG_WEBSITE));
            }
            
            if(json.has(JSON_TAG_FACEBOOK_LINK)) {
            	dm.addToEventContext(JSON_TAG_FACEBOOK_LINK, json.getString(JSON_TAG_FACEBOOK_LINK));
            }
            
            if(json.has(JSON_TAG_REQUIRES_PIN)) {
            	dm.addToEventContext(JSON_TAG_REQUIRES_PIN, String.valueOf(json.getBoolean(JSON_TAG_REQUIRES_PIN)));
            }
            
            if(json.has(JSON_TAG_DEAL_ID)) {
            	dm.addToEventContext(JSON_TAG_DEAL_ID, String.valueOf(json.getInt(JSON_TAG_DEAL_ID)));
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
            	dm.addToEventContext(JSON_TAG_DEAL_AMOUNT, dealAmountStr);
            }
            
            if(json.has(JSON_TAG_CUSTOM_TERMS)) {
            	dm.addToEventContext(JSON_TAG_CUSTOM_TERMS, json.getString(JSON_TAG_CUSTOM_TERMS));
            }
            
            if(json.has(JSON_TAG_ACCOUNT_ID)) {
            	dm.addToEventContext(JSON_TAG_ACCOUNT_ID, String.valueOf(json.getInt(JSON_TAG_ACCOUNT_ID)));
            }
            
            if(json.has(JSON_TAG_MY_IS_FAVORITE)) {
            	dm.addToEventContext(JSON_TAG_MY_IS_FAVORITE, String.valueOf(json.getBoolean(JSON_TAG_MY_IS_FAVORITE)));
            }
            
            if(json.has(JSON_TAG_MY_IS_REDEEMED)) {
            	dm.addToEventContext(JSON_TAG_MY_IS_REDEEMED, String.valueOf(json.getBoolean(JSON_TAG_MY_IS_REDEEMED)));
            }
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }

        return null;
    }
}