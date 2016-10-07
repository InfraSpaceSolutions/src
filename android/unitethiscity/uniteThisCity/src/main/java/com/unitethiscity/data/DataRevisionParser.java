package com.unitethiscity.data;

import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class DataRevisionParser {

	private static String mName = Constants.DATA_REVISION_PARSER;
	
    public static final String 	JSON_TAG_REVISION_ID			= "DrvId";
    public static final String 	JSON_TAG_REVISION_NAME			= "Name";
    public static final String 	JSON_TAG_REVISION				= "Revision";
    public static final String 	JSON_TAG_REVISION_TIMESTAMP		= "Timestamp";
    
    public static final String	REVISION_DATASET_LOCATION_INFO	= "LocationInfo";
    public static final String	REVISION_DATASET_CATEGORIES		= "Categories";
    public static final String	REVISION_DATASET_DEALS			= "Deals";
    public static final String	REVISION_DATASET_EVENT_INFO		= "EventInfo";
	
    public static String setRevision(String context) {
        // getting JSON string from URL
    	try {
    		Logger.verbose(mName, "setting data revision for " + context);
    		
    		JSONObject json = UTCWebAPI.getDataRevisionByName(context);

            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve " + context + " data revision";
            }
            
            if(!json.has(JSON_TAG_REVISION_ID)) {
            	return "Failed to retrieve " + context + " data revision";
            }
    		
            Logger.info(mName, json.toString());
            
            // data manager instance
            DataManager dm = DataManager.getInstance();
            
            Integer drvID = 0;
            String name = "";
            Integer rev = 0;
            String ts = "";
            
            drvID = json.getInt(JSON_TAG_REVISION_ID);
            
            if(json.has(JSON_TAG_REVISION_NAME)) {
            	name = json.getString(JSON_TAG_REVISION_NAME);
            }
            
            if(json.has(JSON_TAG_REVISION)) {
            	rev = json.getInt(JSON_TAG_REVISION);
            }
            
            if(json.has(JSON_TAG_REVISION_TIMESTAMP)) {
            	ts = json.getString(JSON_TAG_REVISION_TIMESTAMP);
            }
            
            if(dm != null) {
            	if(context.equals(REVISION_DATASET_LOCATION_INFO)) {
            		dm.setLocationInfoDataRevision(drvID, name, rev, ts);
            	}
            	else if(context.equals(REVISION_DATASET_CATEGORIES)) {
            		dm.setCategoriesDataRevision(drvID, name, rev, ts);
            	}
            	else if(context.equals(REVISION_DATASET_DEALS)) {
            		dm.setDealsDataRevision(drvID, name, rev, ts);
            	}
            	else if(context.equals(REVISION_DATASET_EVENT_INFO)) {
            		dm.setEventInfoDataRevision(drvID, name, rev, ts);
            	}
            }
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }

        return null;
    }
    
    public static String setRevisionLocationInfo() {
        return setRevision(REVISION_DATASET_LOCATION_INFO);
    }
    
    public static String setRevisionCategories() {
        return setRevision(REVISION_DATASET_CATEGORIES);
    }
    
    public static String setRevisionDeals() {
        return setRevision(REVISION_DATASET_DEALS);
    }
    
    public static String setRevisionEventInfo() {
        return setRevision(REVISION_DATASET_EVENT_INFO);
    }
}
