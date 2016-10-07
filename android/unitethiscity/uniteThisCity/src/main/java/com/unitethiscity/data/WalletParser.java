package com.unitethiscity.data;

import java.text.NumberFormat;

import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class WalletParser {

	private static String mName = Constants.WALLET_PARSER;

    public static final String	JSON_TAG_ACCOUNT_ID						= "AccId";
    public static final String 	JSON_TAG_PER_ID 						= "PerId";
    public static final String 	JSON_TAG_PER_NAME		 				= "PerName";
    public static final String 	JSON_TAG_PER_START_DATE 				= "PerStartDate";
    public static final String 	JSON_TAG_PER_END_DATE 					= "PerEndDate";
    public static final String 	JSON_TAG_CASH_AVAILABLE 				= "CashAvailable";
    public static final String 	JSON_TAG_CASH_REDEEMED 					= "CashRedeemed";
    public static final String 	JSON_TAG_NUM_CHECKINS 					= "NumCheckins";
    public static final String 	JSON_TAG_POINTS 						= "Points";
    public static final String 	JSON_TAG_CASH_REDEEMED_ALL_TIME			= "CashRedeemedAllTime";
    public static final String 	JSON_TAG_NUM_CHECKINS_ALL_TIME			= "NumCheckinsAllTime";
    public static final String 	JSON_TAG_POINTS_ALL_TIME				= "PointsAllTime";
	
    public static String setWallet(String context) {
        // getting JSON string from URL
    	try {
    		Logger.verbose(mName, "setting wallet");
    		
    		JSONObject json = new JSONObject();
    		
        	if(context != null) {
        		json = UTCWebAPI.getUserWallet(context);
        	}
        	else {
        		json = UTCWebAPI.getWallet();
        	}

            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve wallet";
            }
            
            if(!json.has(JSON_TAG_ACCOUNT_ID)) {
            	return "Failed to retrieve wallet";
            }
    		
            // data manager instance
            DataManager dm = DataManager.getInstance();
            
            dm.addToWalletData(JSON_TAG_ACCOUNT_ID, String.valueOf(json.getInt(JSON_TAG_ACCOUNT_ID)));
            
            if(json.has(JSON_TAG_PER_ID)) {
            	dm.addToWalletData(JSON_TAG_PER_ID, String.valueOf(json.getInt(JSON_TAG_PER_ID)));
            }
            
            if(json.has(JSON_TAG_PER_NAME)) {
            	dm.addToWalletData(JSON_TAG_PER_NAME, json.getString(JSON_TAG_PER_NAME));
            }
            
            if(json.has(JSON_TAG_PER_START_DATE)) {
            	dm.addToWalletData(JSON_TAG_PER_START_DATE, json.getString(JSON_TAG_PER_START_DATE));
            }
            
            if(json.has(JSON_TAG_PER_END_DATE)) {
            	dm.addToWalletData(JSON_TAG_PER_END_DATE, json.getString(JSON_TAG_PER_END_DATE));
            }
            
            if(json.has(JSON_TAG_CASH_AVAILABLE)) {
            	double money = json.getDouble(JSON_TAG_CASH_AVAILABLE);
            	NumberFormat formatter = NumberFormat.getCurrencyInstance();
            	String moneyString = formatter.format(money);
            	dm.addToWalletData(JSON_TAG_CASH_AVAILABLE, moneyString);
            }
            
            if(json.has(JSON_TAG_CASH_REDEEMED)) {
            	double money = json.getDouble(JSON_TAG_CASH_REDEEMED);
            	NumberFormat formatter = NumberFormat.getCurrencyInstance();
            	String moneyString = formatter.format(money);
            	dm.addToWalletData(JSON_TAG_CASH_REDEEMED, moneyString);
            }
            
            if(json.has(JSON_TAG_NUM_CHECKINS)) {
            	dm.addToWalletData(JSON_TAG_NUM_CHECKINS, String.valueOf(json.getDouble(JSON_TAG_NUM_CHECKINS)));
            }
            
            if(json.has(JSON_TAG_POINTS)) {
            	dm.addToWalletData(JSON_TAG_POINTS, String.valueOf(json.getDouble(JSON_TAG_POINTS)));
            }

            if(json.has(JSON_TAG_CASH_REDEEMED_ALL_TIME)) {
                double money = json.getDouble(JSON_TAG_CASH_REDEEMED_ALL_TIME);
                NumberFormat formatter = NumberFormat.getCurrencyInstance();
                String moneyString = formatter.format(money);
                dm.addToWalletData(JSON_TAG_CASH_REDEEMED_ALL_TIME, moneyString);
            }

            if(json.has(JSON_TAG_NUM_CHECKINS_ALL_TIME)) {
                dm.addToWalletData(JSON_TAG_NUM_CHECKINS_ALL_TIME, String.valueOf(json.getDouble(JSON_TAG_NUM_CHECKINS_ALL_TIME)));
            }

            if(json.has(JSON_TAG_POINTS_ALL_TIME)) {
                dm.addToWalletData(JSON_TAG_POINTS_ALL_TIME, String.valueOf(json.getDouble(JSON_TAG_POINTS_ALL_TIME)));
            }
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }

        return null;
    }
}