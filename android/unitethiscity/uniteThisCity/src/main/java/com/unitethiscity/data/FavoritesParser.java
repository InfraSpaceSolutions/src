package com.unitethiscity.data;

import org.json.JSONArray;
import org.json.JSONException;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class FavoritesParser {

	private static String mName = Constants.FAVORITES_PARSER;
	
    public static String setFavorites() {
    	if(LoginManager.getInstance().userLoggedIn()) {
    		Logger.verbose(mName, "setting favorites");
    		
            // getting JSON string from URL
    		JSONArray json = UTCWebAPI.getFavorites(LoginManager.getInstance().getAccountContext().getToken());
            
            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve favorites";
            }
            
            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearFavorites();
            
            for(int i = 0; i < json.length(); i++) {
            	try {
					dm.addFavorite(Integer.valueOf(json.getInt(i)));
				} catch (JSONException e) {
					e.printStackTrace();
				}
            }
    	}

        return null;
    }
}