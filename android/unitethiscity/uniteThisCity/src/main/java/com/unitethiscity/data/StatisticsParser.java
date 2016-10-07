package com.unitethiscity.data;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;

/**
 * Created by Don on 2/16/2016.
 */
public class StatisticsParser {
    private static String mName = Constants.STATISTICS_PARSER;

    // SummaryStats
    public static final String 	JSON_TAG_SUMMARY_STATS_NAME    						= "Name";
    public static final String 	JSON_TAG_SUMMARY_STATS_LINK    						= "Link";
    public static final String 	JSON_TAG_SUMMARY_STATS_TODAY    					= "Today";
    public static final String 	JSON_TAG_SUMMARY_STATS_PAST_WEEK  					= "PastWeek";
    public static final String 	JSON_TAG_SUMMARY_STATS_THIS_PERIOD					= "ThisPeriod";
    public static final String 	JSON_TAG_SUMMARY_STATS_LAST_PERIOD					= "LastPeriod";
    public static final String 	JSON_TAG_SUMMARY_STATS_ALL_TIME  					= "AllTime";

    // StatFavorite
    public static final String 	JSON_TAG_STAT_FAVORITE_ID    						= "Id";
    public static final String 	JSON_TAG_STAT_FAVORITE_BUS_ID    					= "BusId";
    public static final String 	JSON_TAG_STAT_FAVORITE_BUS_GUID    					= "BusGuid";
    public static final String 	JSON_TAG_STAT_FAVORITE_NAME  					    = "Name";
    public static final String 	JSON_TAG_STAT_FAVORITE_TIMESTAMP					= "Timestamp";
    public static final String 	JSON_TAG_STAT_FAVORITE_TIMESTAMP_AS_STRING			= "TimestampAsString";
    public static final String 	JSON_TAG_STAT_FAVORITE_TIMESTAMP_SORTABLE  		    = "TimestampSortable";
    public static final String 	JSON_TAG_STAT_FAVORITE_LOCATION_NAME  				= "LocationName";

    // StatRedemption
    public static final String 	JSON_TAG_STAT_REDEMPTION_ID    						= "Id";
    public static final String 	JSON_TAG_STAT_REDEMPTION_BUS_ID    					= "BusId";
    public static final String 	JSON_TAG_STAT_REDEMPTION_BUS_GUID    				= "BusGuid";
    public static final String 	JSON_TAG_STAT_REDEMPTION_NAME  					    = "Name";
    public static final String 	JSON_TAG_STAT_REDEMPTION_PERIOD 					= "Period";
    public static final String 	JSON_TAG_STAT_REDEMPTION_TIMESTAMP      			= "Timestamp";
    public static final String 	JSON_TAG_STAT_REDEMPTION_TIMESTAMP_AS_STRING		= "TimestampAsString";
    public static final String 	JSON_TAG_STAT_REDEMPTION_TIMESTAMP_SORTABLE  		= "TimestampSortable";
    public static final String 	JSON_TAG_STAT_REDEMPTION_PIN        				= "Pin";
    public static final String 	JSON_TAG_STAT_REDEMPTION_DEAL         				= "Deal";
    public static final String 	JSON_TAG_STAT_REDEMPTION_AMOUNT       				= "Amount";

    // StatCheckIn
    public static final String 	JSON_TAG_STAT_CHECK_IN_ID    						= "Id";
    public static final String 	JSON_TAG_STAT_CHECK_IN_BUS_ID    					= "BusId";
    public static final String 	JSON_TAG_STAT_CHECK_IN_NAME  					    = "Name";
    public static final String 	JSON_TAG_STAT_CHECK_IN_PERIOD    					= "Period";
    public static final String 	JSON_TAG_STAT_CHECK_IN_TIMESTAMP					= "Timestamp";
    public static final String 	JSON_TAG_STAT_CHECK_IN_TIMESTAMP_AS_STRING			= "TimestampAsString";
    public static final String 	JSON_TAG_STAT_CHECK_IN_TIMESTAMP_SORTABLE  		    = "TimestampSortable";
    public static final String 	JSON_TAG_STAT_CHECK_IN_LOCATION_NAME				= "LocationName";

    // StatRating
    public static final String 	JSON_TAG_STAT_RATING_ID    					        = "Id";
    public static final String 	JSON_TAG_STAT_RATING_BUS_ID    				        = "BusId";
    public static final String 	JSON_TAG_STAT_RATING_BUS_GUID    			        = "BusGuid";
    public static final String 	JSON_TAG_STAT_RATING_NAME  					        = "Name";
    public static final String 	JSON_TAG_STAT_RATING_TIMESTAMP				        = "Timestamp";
    public static final String 	JSON_TAG_STAT_RATING_TIMESTAMP_AS_STRING	        = "TimestampAsString";
    public static final String 	JSON_TAG_STAT_RATING_TIMESTAMP_SORTABLE  	        = "TimestampSortable";
    public static final String 	JSON_TAG_STAT_RATING_RATING					        = "Rating";
    public static final String 	JSON_TAG_STAT_RATING_LOCATION_NAME			        = "LocationName";

    // StatTip
    public static final String 	JSON_TAG_STAT_TIP_ID    					        = "Id";
    public static final String 	JSON_TAG_STAT_TIP_BUS_ID    				        = "BusId";
    public static final String 	JSON_TAG_STAT_TIP_BUS_GUID    			            = "BusGuid";
    public static final String 	JSON_TAG_STAT_TIP_NAME  					        = "Name";
    public static final String 	JSON_TAG_STAT_TIP_TIMESTAMP				            = "Timestamp";
    public static final String 	JSON_TAG_STAT_TIP_TIMESTAMP_AS_STRING	            = "TimestampAsString";
    public static final String 	JSON_TAG_STAT_TIP_TIMESTAMP_SORTABLE  	            = "TimestampSortable";
    public static final String 	JSON_TAG_STAT_TIP_TIP_TEXT				            = "TipText";
    public static final String 	JSON_TAG_STAT_TIP_LOCATION_NAME			            = "LocationName";

    // SummaryRedemption
    public static final String 	JSON_TAG_SUMMARY_REDEMPTION_ACC_ID		            = "AccID";
    public static final String 	JSON_TAG_SUMMARY_REDEMPTION_BUS_ID    				= "BusID";
    public static final String 	JSON_TAG_SUMMARY_REDEMPTION_NAME    			    = "Name";
    public static final String 	JSON_TAG_SUMMARY_REDEMPTION_COUNT  					= "Count";
    public static final String 	JSON_TAG_SUMMARY_REDEMPTION_SUM			            = "Sum";

    // SummaryCheckIn
    public static final String 	JSON_TAG_SUMMARY_CHECK_IN_ACC_ID		            = "AccID";
    public static final String 	JSON_TAG_SUMMARY_CHECK_IN_BUS_ID    				= "BusID";
    public static final String 	JSON_TAG_SUMMARY_CHECK_IN_NAME    			        = "Name";
    public static final String 	JSON_TAG_SUMMARY_CHECK_IN_COUNT  					= "Count";

    public static String setSummaryStats(Integer id) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting summary stats");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getSummaryStats(LoginManager.getInstance().getAccountContext().getToken(), id);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve summary stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearSummaryStats();

            ArrayList<SummaryStats> stats = new ArrayList<SummaryStats>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_SUMMARY_STATS_NAME) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_LINK) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_TODAY) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_PAST_WEEK) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_THIS_PERIOD) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_LAST_PERIOD) &&
                            jsonObj.has(JSON_TAG_SUMMARY_STATS_ALL_TIME)) {

                        SummaryStats stat = new SummaryStats(jsonObj.getString(JSON_TAG_SUMMARY_STATS_NAME),
                                jsonObj.getString(JSON_TAG_SUMMARY_STATS_LINK),
                                jsonObj.getInt(JSON_TAG_SUMMARY_STATS_TODAY),
                                jsonObj.getInt(JSON_TAG_SUMMARY_STATS_PAST_WEEK),
                                jsonObj.getInt(JSON_TAG_SUMMARY_STATS_THIS_PERIOD),
                                jsonObj.getInt(JSON_TAG_SUMMARY_STATS_LAST_PERIOD),
                                jsonObj.getInt(JSON_TAG_SUMMARY_STATS_ALL_TIME));

                        stats.add(stat);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setSummaryStats(stats);
        }

        return null;
    }

    public static String setStatFavorites(Integer id) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat favorites");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatFavorite(LoginManager.getInstance().getAccountContext().getToken(), id);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve favorites stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatFavorites();

            ArrayList<StatFavorite> favs = new ArrayList<StatFavorite>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_FAVORITE_ID) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_BUS_GUID) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_FAVORITE_LOCATION_NAME)) {

                        StatFavorite fav = new StatFavorite(jsonObj.getInt(JSON_TAG_STAT_FAVORITE_ID),
                                jsonObj.getInt(JSON_TAG_STAT_FAVORITE_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_BUS_GUID),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_NAME),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_FAVORITE_LOCATION_NAME));

                        favs.add(fav);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatFavorites(favs);
        }

        return null;
    }

    public static String setStatRedemptions(Integer id, int scopeid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat redemptions");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatRedemption(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve redemptions stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatRedemptions();

            ArrayList<StatRedemption> redemptions = new ArrayList<StatRedemption>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_REDEMPTION_ID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_BUS_GUID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_PIN) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_DEAL) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_AMOUNT)) {

                        StatRedemption redemption = new StatRedemption(jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_ID),
                                jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_BUS_GUID),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_NAME),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_PERIOD),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_PIN),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_DEAL),
                                jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_AMOUNT));

                        redemptions.add(redemption);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatRedemptions(redemptions);
        }

        return null;
    }

    public static String setStatRedemptions(Integer id, int scopeid, int accid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat redemptions");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatRedemption(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid, accid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve redemptions stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatRedemptions();

            ArrayList<StatRedemption> redemptions = new ArrayList<StatRedemption>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_REDEMPTION_ID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_BUS_GUID) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_PIN) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_DEAL) &&
                            jsonObj.has(JSON_TAG_STAT_REDEMPTION_AMOUNT)) {

                        StatRedemption redemption = new StatRedemption(jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_ID),
                                jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_BUS_GUID),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_NAME),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_PERIOD),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_PIN),
                                jsonObj.getString(JSON_TAG_STAT_REDEMPTION_DEAL),
                                jsonObj.getInt(JSON_TAG_STAT_REDEMPTION_AMOUNT));

                        redemptions.add(redemption);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatRedemptions(redemptions);
        }

        return null;
    }

    public static String setStatCheckIns(Integer id, int scopeid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat check ins");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatCheckIn(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve check-ins stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatCheckIns();

            ArrayList<StatCheckIn> checkIns = new ArrayList<StatCheckIn>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_CHECK_IN_ID) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_LOCATION_NAME)) {

                        StatCheckIn checkIn = new StatCheckIn(jsonObj.getInt(JSON_TAG_STAT_CHECK_IN_ID),
                                jsonObj.getInt(JSON_TAG_STAT_CHECK_IN_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_NAME),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_PERIOD),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_LOCATION_NAME));

                        checkIns.add(checkIn);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatCheckIns(checkIns);
        }

        return null;
    }

    public static String setStatCheckIns(Integer id, int scopeid, int accid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat check ins");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatCheckIn(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid, accid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve check-ins stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatCheckIns();

            ArrayList<StatCheckIn> checkIns = new ArrayList<StatCheckIn>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_CHECK_IN_ID) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_PERIOD) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_CHECK_IN_LOCATION_NAME)) {

                        StatCheckIn checkIn = new StatCheckIn(jsonObj.getInt(JSON_TAG_STAT_CHECK_IN_ID),
                                jsonObj.getInt(JSON_TAG_STAT_CHECK_IN_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_NAME),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_PERIOD),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_CHECK_IN_LOCATION_NAME));

                        checkIns.add(checkIn);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatCheckIns(checkIns);
        }

        return null;
    }

    public static String setStatRatings(Integer id) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat ratings");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatRating(LoginManager.getInstance().getAccountContext().getToken(), id);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve ratings stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatRatings();

            ArrayList<StatRating> ratings = new ArrayList<StatRating>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_RATING_ID) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_BUS_GUID) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_RATING) &&
                            jsonObj.has(JSON_TAG_STAT_RATING_LOCATION_NAME)) {

                        StatRating rating = new StatRating(jsonObj.getInt(JSON_TAG_STAT_RATING_ID),
                                jsonObj.getInt(JSON_TAG_STAT_RATING_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_RATING_BUS_GUID),
                                jsonObj.getString(JSON_TAG_STAT_RATING_NAME),
                                jsonObj.getString(JSON_TAG_STAT_RATING_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_RATING_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_RATING_TIMESTAMP_SORTABLE),
                                jsonObj.getInt(JSON_TAG_STAT_RATING_RATING),
                                jsonObj.getString(JSON_TAG_STAT_RATING_LOCATION_NAME));

                        ratings.add(rating);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatRatings(ratings);
        }

        return null;
    }

    public static String setStatTips(Integer id) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting stat tips");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getStatTip(LoginManager.getInstance().getAccountContext().getToken(), id);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve tips stats";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearStatTips();

            ArrayList<StatTip> tips = new ArrayList<StatTip>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_STAT_TIP_ID) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_BUS_ID) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_BUS_GUID) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_NAME) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_TIMESTAMP) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_TIMESTAMP_AS_STRING) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_TIMESTAMP_SORTABLE) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_TIP_TEXT) &&
                            jsonObj.has(JSON_TAG_STAT_TIP_LOCATION_NAME)) {

                        StatTip tip = new StatTip(jsonObj.getInt(JSON_TAG_STAT_TIP_ID),
                                jsonObj.getInt(JSON_TAG_STAT_TIP_BUS_ID),
                                jsonObj.getString(JSON_TAG_STAT_TIP_BUS_GUID),
                                jsonObj.getString(JSON_TAG_STAT_TIP_NAME),
                                jsonObj.getString(JSON_TAG_STAT_TIP_TIMESTAMP),
                                jsonObj.getString(JSON_TAG_STAT_TIP_TIMESTAMP_AS_STRING),
                                jsonObj.getString(JSON_TAG_STAT_TIP_TIMESTAMP_SORTABLE),
                                jsonObj.getString(JSON_TAG_STAT_TIP_TIP_TEXT),
                                jsonObj.getString(JSON_TAG_STAT_TIP_LOCATION_NAME));

                        tips.add(tip);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setStatTips(tips);
        }

        return null;
    }

    public static String setSummaryRedemptions(Integer id, int scopeid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting summary redemptions");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getSummaryRedemptions(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve redemption user summary";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearSummaryRedemptions();

            ArrayList<SummaryRedemption> redemptions = new ArrayList<SummaryRedemption>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_SUMMARY_REDEMPTION_ACC_ID) &&
                            jsonObj.has(JSON_TAG_SUMMARY_REDEMPTION_BUS_ID) &&
                            jsonObj.has(JSON_TAG_SUMMARY_REDEMPTION_NAME) &&
                            jsonObj.has(JSON_TAG_SUMMARY_REDEMPTION_COUNT) &&
                            jsonObj.has(JSON_TAG_SUMMARY_REDEMPTION_SUM)) {

                        SummaryRedemption redemption = new SummaryRedemption(
                                jsonObj.getInt(JSON_TAG_SUMMARY_REDEMPTION_ACC_ID),
                                jsonObj.getInt(JSON_TAG_SUMMARY_REDEMPTION_BUS_ID),
                                jsonObj.getString(JSON_TAG_SUMMARY_REDEMPTION_NAME),
                                jsonObj.getInt(JSON_TAG_SUMMARY_REDEMPTION_COUNT),
                                jsonObj.getDouble(JSON_TAG_SUMMARY_REDEMPTION_SUM));

                        redemptions.add(redemption);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setSummaryRedemptions(redemptions);
        }

        return null;
    }

    public static String setSummaryCheckIns(Integer id, int scopeid) {
        if(LoginManager.getInstance().userLoggedIn()) {
            Logger.verbose(mName, "setting summary checkins");

            // getting JSON string from URL
            JSONArray json = UTCWebAPI.getSummaryCheckIns(LoginManager.getInstance().getAccountContext().getToken(), id, scopeid);

            // if JSON request didn't work, bail
            if(json == null) {
                return "Failed to retrieve check-in user summary";
            }

            // data manager instance
            DataManager dm = DataManager.getInstance();
            dm.clearSummaryCheckIns();

            ArrayList<SummaryCheckIn> checkins = new ArrayList<SummaryCheckIn>();

            for(int i = 0; i < json.length(); i++) {
                try {
                    JSONObject jsonObj = json.getJSONObject(i);
                    if(jsonObj.has(JSON_TAG_SUMMARY_CHECK_IN_ACC_ID) &&
                            jsonObj.has(JSON_TAG_SUMMARY_CHECK_IN_BUS_ID) &&
                            jsonObj.has(JSON_TAG_SUMMARY_CHECK_IN_NAME) &&
                            jsonObj.has(JSON_TAG_SUMMARY_CHECK_IN_COUNT)) {

                        SummaryCheckIn checkin = new SummaryCheckIn(
                                jsonObj.getInt(JSON_TAG_SUMMARY_CHECK_IN_ACC_ID),
                                jsonObj.getInt(JSON_TAG_SUMMARY_CHECK_IN_BUS_ID),
                                jsonObj.getString(JSON_TAG_SUMMARY_CHECK_IN_NAME),
                                jsonObj.getInt(JSON_TAG_SUMMARY_CHECK_IN_COUNT));

                        checkins.add(checkin);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            dm.setSummaryCheckIns(checkins);
        }

        return null;
    }
}
