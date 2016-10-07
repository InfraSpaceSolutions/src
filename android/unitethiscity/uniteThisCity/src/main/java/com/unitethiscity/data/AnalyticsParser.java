package com.unitethiscity.data;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;

/**
 * Created by Don on 2/20/2016.
 */
public class AnalyticsParser {
    private static String mName = Constants.ANALYTICS_PARSER;

    public static final String 	JSON_TAG_NAME    						= "Name";
    public static final String 	JSON_TAG_TOTAL    						= "Total";
    public static final String 	JSON_TAG_PERCENT    					= "Percent";
    public static final String 	JSON_TAG_GROUP  	    				= "Group";

    public static String setSummaryAnalytics(Integer id) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting analytics");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getSummaryAnalytics(LoginManager.getInstance().getAccountContext().getToken(), id);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve analytics";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearSummaryAnalytics();

            ArrayList<SummaryAnalytics> analytics = new ArrayList<SummaryAnalytics>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_NAME) && jsonObj.has(JSON_TAG_TOTAL) &&
                            jsonObj.has(JSON_TAG_PERCENT) && jsonObj.has(JSON_TAG_GROUP)) {

                        SummaryAnalytics analytic = new SummaryAnalytics(jsonObj.getString(JSON_TAG_NAME),
                                jsonObj.getInt(JSON_TAG_TOTAL), jsonObj.getDouble(JSON_TAG_PERCENT),
                                jsonObj.getInt(JSON_TAG_GROUP));

                        analytics.add(analytic);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setSummaryAnalytics(analytics);
        }

        return null;
    }

    public static String setSummaryAnalytics(Integer id, Integer busId) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting business analytics");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getSummaryAnalytics(LoginManager.getInstance().getAccountContext().getToken(), id, busId);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve business analytics";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearSummaryAnalyticsBusiness();

            ArrayList<SummaryAnalytics> analytics = new ArrayList<SummaryAnalytics>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_NAME) && jsonObj.has(JSON_TAG_TOTAL) &&
                            jsonObj.has(JSON_TAG_PERCENT) && jsonObj.has(JSON_TAG_GROUP)) {

                        SummaryAnalytics analytic = new SummaryAnalytics(jsonObj.getString(JSON_TAG_NAME),
                                jsonObj.getInt(JSON_TAG_TOTAL), jsonObj.getDouble(JSON_TAG_PERCENT),
                                jsonObj.getInt(JSON_TAG_GROUP));

                        analytics.add(analytic);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setSummaryAnalyticsBusiness(analytics);
        }

        return null;
    }
}
