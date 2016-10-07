package com.unitethiscity.data;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.os.SystemClock;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class CategoriesParser {

	private static String mName = Constants.CATEGORIES_PARSER;
	
    public static final String 	JSON_TAG_GROUP_ID 						= "GroupId";
    public static final String 	JSON_TAG_CATID 							= "CatId";
    public static final String 	JSON_TAG_GROUP_NAME 					= "GroupName";
    public static final String 	JSON_TAG_CATEGORY_NAME 					= "CatName";
    public static final String 	JSON_TAG_COUNT	 						= "Count";
	
    public static String setCategories() throws JSONException {
    	if(DataManager.getInstance().doCategoriesNeedUpdated()) {
    		Logger.info(mName, "updating categories");
    		
        	// getting JSON string from URL
        	JSONArray json = UTCWebAPI.getCategories();

        	// if JSON request didn't work, bail
        	if(json == null) {
        		return "Failed to retrieve categories";
        	}
        	
        	// clear local categories stored by the data manager 
        	// since we are updating
        	DataManager.getInstance().clearCategories();

        	JSONObject category = null;
        	for(int i = 0; i < json.length(); i++) {
        		category = json.getJSONObject(i);

        		int groupID = category.getInt(JSON_TAG_GROUP_ID);
        		Integer id = category.getInt(JSON_TAG_CATID);
        		String groupName = category.getString(JSON_TAG_GROUP_NAME);
        		String catName = category.getString(JSON_TAG_CATEGORY_NAME);
        		int count = category.getInt(JSON_TAG_COUNT);

        		DataManager.getInstance().addCategory(new UTCCategory(
        				id.intValue(), catName, count, groupID, groupName));
        	}
        	
        	DataManager.getInstance().setCategoryTimestamp(SystemClock
        			.elapsedRealtime());
    	}

        return null;
    }
}