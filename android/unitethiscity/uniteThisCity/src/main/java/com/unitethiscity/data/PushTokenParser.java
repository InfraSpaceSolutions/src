package com.unitethiscity.data;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class PushTokenParser {

	private static String mName = Constants.PUSH_TOKEN_PARSER;

	public static final String 	JSON_TAG_TOKENS 						= ""; // array with no name
	public static final String 	JSON_TAG_PUTID							= "PutId";
	public static final String 	JSON_TAG_ACCID 							= "AccId";
	public static final String 	JSON_TAG_PDTID 							= "PdtId";
	public static final String	JSON_TAG_PUTTOKEN						= "PutToken";
    public static final String 	JSON_TAG_PUTENABLED						= "PutEnabled";
	
    public static String setPushTokens() {
    	if(LoginManager.getInstance().userLoggedIn()) {
        	// getting JSON string from URL
        	JSONArray json = UTCWebAPI.getPushTokens(LoginManager.getInstance().getAccountContext().getToken());

        	// if JSON request didn't work, bail
        	if(json == null) {
        		return "Failed to retrieve push tokens";
        	}

        	try {
        		// looping through all push tokens
        		for(int i = 0; i < json.length(); i++) {
        			JSONObject l = json.getJSONObject(i);

        			// skip if not an Android push token
        			if(!l.has(JSON_TAG_PDTID) || l.getInt(JSON_TAG_PDTID) == 2) {
        				continue;
        			}
        			
        			// storing each JSON item in UTCPushToken
        			UTCPushToken utcPt = new UTCPushToken(l.getInt(JSON_TAG_PUTID), l.getString(JSON_TAG_PUTTOKEN));
        			
        			if(l.has(JSON_TAG_PUTID)) {
        				utcPt.put(JSON_TAG_PUTID, String.valueOf(l.getInt(JSON_TAG_PUTID)));
        			}

        			if(l.has(JSON_TAG_ACCID)) {
        				utcPt.put(JSON_TAG_ACCID, String.valueOf(l.getInt(JSON_TAG_ACCID)));
        			}

        			if(l.has(JSON_TAG_PDTID)) {
        				utcPt.put(JSON_TAG_PDTID, String.valueOf(l.getInt(JSON_TAG_PDTID)));
        			}

        			if(l.has(JSON_TAG_PUTTOKEN)) {
        				utcPt.put(JSON_TAG_PUTTOKEN, l.getString(JSON_TAG_PUTTOKEN));
        			}

        			if(l.has(JSON_TAG_PUTENABLED)) {
        				utcPt.put(JSON_TAG_PUTENABLED, String.valueOf(l.getBoolean(JSON_TAG_PUTENABLED)));
        			}

        			DataManager.getInstance().addPushToken(utcPt);
        		}
        		Logger.verbose(mName, "added " + json.length() + " push tokens");
        	} catch (JSONException e) {
        		e.printStackTrace();
        	}
    	}
        
        return null;
    }
    
    public static String setEvent(Integer ptID) throws JSONException {
    	if(LoginManager.getInstance().userLoggedIn()) {
            // getting JSON string from URL
            JSONObject l = UTCWebAPI.getPushTokenByID(ptID,
            		LoginManager.getInstance().getAccountContext().getToken());
            
            // if JSON request didn't work, bail
            if(l == null) {
            	return "Failed to retrieve push token";
            }

            // storing JSON item in UTCPushToken
			UTCPushToken utcPt = new UTCPushToken(l.getInt(JSON_TAG_PUTID), l.getString(JSON_TAG_PUTTOKEN));
			
			if(l.has(JSON_TAG_PUTID)) {
				utcPt.put(JSON_TAG_PUTID, String.valueOf(l.getInt(JSON_TAG_PUTID)));
			}

			if(l.has(JSON_TAG_ACCID)) {
				utcPt.put(JSON_TAG_ACCID, String.valueOf(l.getInt(JSON_TAG_ACCID)));
			}

			if(l.has(JSON_TAG_PDTID)) {
				utcPt.put(JSON_TAG_PDTID, String.valueOf(l.getInt(JSON_TAG_PDTID)));
			}

			if(l.has(JSON_TAG_PUTTOKEN)) {
				utcPt.put(JSON_TAG_PUTTOKEN, l.getString(JSON_TAG_PUTTOKEN));
			}

			if(l.has(JSON_TAG_PUTENABLED)) {
				utcPt.put(JSON_TAG_PUTENABLED, String.valueOf(l.getBoolean(JSON_TAG_PUTENABLED)));
			}
    	}

        return null;
    }
    
    public static String setEvent(String registrationID) throws JSONException {
    	if(LoginManager.getInstance().userLoggedIn()) {
            // getting JSON string from URL
            JSONObject l = UTCWebAPI.getPushTokenByToken(LoginManager.getInstance().getAccountContext().getToken(),
            		registrationID);
            
            // if JSON request didn't work, bail
            if(l == null) {
            	return "Failed to retrieve push token";
            }

            // storing JSON item in UTCPushToken
			UTCPushToken utcPt = new UTCPushToken(l.getInt(JSON_TAG_PUTID), l.getString(JSON_TAG_PUTTOKEN));
			
			if(l.has(JSON_TAG_PUTID)) {
				utcPt.put(JSON_TAG_PUTID, String.valueOf(l.getInt(JSON_TAG_PUTID)));
			}

			if(l.has(JSON_TAG_ACCID)) {
				utcPt.put(JSON_TAG_ACCID, String.valueOf(l.getInt(JSON_TAG_ACCID)));
			}

			if(l.has(JSON_TAG_PDTID)) {
				utcPt.put(JSON_TAG_PDTID, String.valueOf(l.getInt(JSON_TAG_PDTID)));
			}

			if(l.has(JSON_TAG_PUTTOKEN)) {
				utcPt.put(JSON_TAG_PUTTOKEN, l.getString(JSON_TAG_PUTTOKEN));
			}

			if(l.has(JSON_TAG_PUTENABLED)) {
				utcPt.put(JSON_TAG_PUTENABLED, String.valueOf(l.getBoolean(JSON_TAG_PUTENABLED)));
			}
    	}

        return null;
    }
}