package com.unitethiscity.data;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.os.SystemClock;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class InboxParser {

	private static String mName = Constants.INBOX_PARSER;
	
    public static final String 	JSON_TAG_ID		 						= "Id";
    public static final String 	JSON_TAG_MESSAGE_ID						= "MsgId";
    public static final String 	JSON_TAG_TO_ACC_ID	 					= "ToAccId";
    public static final String 	JSON_TAG_INBOX_READ	 					= "InbRead";
    public static final String 	JSON_TAG_FROM_ACC_ID	 				= "FromAccID";
    public static final String 	JSON_TAG_FROM_NAME	 					= "FromName";
    public static final String 	JSON_TAG_SUMMARY	 					= "Summary";
    public static final String 	JSON_TAG_BODY	 						= "Body";
    public static final String 	JSON_TAG_MESSAGE_TS	 					= "MsgTS";
    public static final String 	JSON_TAG_MESSAGE_TS_AS_STR	 			= "MsgTSAsStr";
    public static final String 	JSON_TAG_MESSAGE_EXP	 				= "MsgExpires";
    public static final String 	JSON_TAG_MESSAGE_EXP_AS_STR	 			= "MsgExpiresAsStr";
	public static final String	JSON_TAG_BUSINESS_GUID					= "BusGuid";
    
    public static String setMessages() throws JSONException {
    	if(LoginManager.getInstance().userLoggedIn() && 
    			DataManager.getInstance().doMessagesNeedUpdated()) {
    		
    		DataManager.getInstance().clearMessages();
    		
        	// getting JSON string from URL
        	JSONArray json = UTCWebAPI.getAllMessages(LoginManager.getInstance().getAccountContext().getToken());

        	// if JSON request didn't work, bail
        	if(json == null) {
        		return "Failed to retrieve messages";
        	}
        	
        	Logger.info(mName, json.toString());

        	JSONObject message = null;
        	for(int i = 0; i < json.length(); i++) {
        		message = json.getJSONObject(i);

        		UTCMessage newMessage = new UTCMessage(
        				message.getInt(JSON_TAG_ID),
        				message.getInt(JSON_TAG_MESSAGE_ID),
        				message.getInt(JSON_TAG_TO_ACC_ID),
        				message.getBoolean(JSON_TAG_INBOX_READ),
        				message.getInt(JSON_TAG_FROM_ACC_ID),
        				message.getString(JSON_TAG_FROM_NAME),
        				message.getString(JSON_TAG_SUMMARY),
        				message.getString(JSON_TAG_BODY),
        				message.getString(JSON_TAG_MESSAGE_TS),
        				message.getString(JSON_TAG_MESSAGE_TS_AS_STR),
        				message.getString(JSON_TAG_MESSAGE_EXP),
        				message.getString(JSON_TAG_MESSAGE_EXP_AS_STR),
        				message.getString(JSON_TAG_BUSINESS_GUID));

        		DataManager.getInstance().addMessage(i, newMessage);
        	}
        	
        	DataManager.getInstance().setMessagesTimestamp(SystemClock.elapsedRealtime());
    	}

        return null;
    }

	public static String setMessage(Integer id) throws JSONException {
		if(LoginManager.getInstance().userLoggedIn()) {
			DataManager.getInstance().clearCurrentMessage();

			// getting JSON string from URL
			JSONObject message = UTCWebAPI.getMessage(LoginManager.getInstance().getAccountContext().getToken(), id);

			// if JSON request didn't work, bail
			if(message == null) {
				return "Failed to retrieve message";
			}

			Logger.info(mName, message.toString());

			UTCMessage newMessage = new UTCMessage(
					message.getInt(JSON_TAG_ID),
					message.getInt(JSON_TAG_MESSAGE_ID),
					message.getInt(JSON_TAG_TO_ACC_ID),
					message.getBoolean(JSON_TAG_INBOX_READ),
					message.getInt(JSON_TAG_FROM_ACC_ID),
					message.getString(JSON_TAG_FROM_NAME),
					message.getString(JSON_TAG_SUMMARY),
					message.getString(JSON_TAG_BODY),
					message.getString(JSON_TAG_MESSAGE_TS),
					message.getString(JSON_TAG_MESSAGE_TS_AS_STR),
					message.getString(JSON_TAG_MESSAGE_EXP),
					message.getString(JSON_TAG_MESSAGE_EXP_AS_STR),
					message.getString(JSON_TAG_BUSINESS_GUID));

			DataManager.getInstance().setCurrentMessage(newMessage);
		}

		return null;
	}
}