package com.unitethiscity.data;

import java.text.DecimalFormat;
import java.text.NumberFormat;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.location.Location;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class LocationParser {

	private static String mName = Constants.LOCATION_PARSER;
	
	public static final String  FAILED									= "Failed to retrieve locations";
	
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
	public static final String 	JSON_TAG_DEALID 						= "DealId";
	public static final String 	JSON_TAG_DEALAMOUNT 					= "DealAmount";
	public static final String 	JSON_TAG_MYISREDEEMED					= "MyIsRedeemed";
	public static final String 	JSON_TAG_MYISCHECKEDIN					= "MyIsCheckedIn";
	public static final String 	JSON_TAG_MYCHECKINTIME					= "MyCheckIntime";
	public static final String 	JSON_TAG_MYREDEEMDATE					= "MyRedeemDate";
	public static final String 	JSON_TAG_ENTERTAINER					= "Entertainer";
    public static final String 	JSON_TAG_DISTANCE 						= "Distance"; // not an actual used JSON tag, but used for storage locally

	public static String setLocations() {
		// getting JSON string from URL
		JSONArray json = UTCWebAPI.getLocationsSummary();

		// if JSON request didn't work, bail
		if(json == null) {
			return FAILED;
		}

		// data manager instance
		UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
		Location loc = lm.getBestLocation();

		double latitude = 0.0;
		double longitude = 0.0;

		try {
			// looping through all locations
			for(int i = 0; i < json.length(); i++) {
				JSONObject l = json.getJSONObject(i);

				// storing each JSON item in UTCLocation
				UTCLocation utcLoc = new UTCLocation(l.getInt(JSON_TAG_ID));

				if(l.has(JSON_TAG_PROPERTIES)) {
					JSONArray properties = l.getJSONArray(JSON_TAG_PROPERTIES);
					if(properties != null) {
						utcLoc.put(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));
						for(int j = 0; j < properties.length(); j++) {
							utcLoc.put(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
						}
					}
					else {
						Logger.error(mName, "properties null");
					}
				}

				if(l.has(JSON_TAG_ID)) {
					utcLoc.put(JSON_TAG_ID, String.valueOf(l.getInt(JSON_TAG_ID)));
				}

				if(l.has(JSON_TAG_BUSID)) {
					utcLoc.put(JSON_TAG_BUSID, String.valueOf(l.getInt(JSON_TAG_BUSID)));
				}

				if(l.has(JSON_TAG_BUSGUID)) {
					utcLoc.put(JSON_TAG_BUSGUID, l.getString(JSON_TAG_BUSGUID));
				}

				if(l.has(JSON_TAG_CITID)) {
					utcLoc.put(JSON_TAG_CITID, String.valueOf(l.getInt(JSON_TAG_CITID)));
				}

				if(l.has(JSON_TAG_NAME)) {
					utcLoc.put(JSON_TAG_NAME, l.getString(JSON_TAG_NAME));
				}

				if(l.has(JSON_TAG_ADDRESS)) {
					utcLoc.put(JSON_TAG_ADDRESS, l.getString(JSON_TAG_ADDRESS));
				}

				if(l.has(JSON_TAG_RATING)) {
					utcLoc.put(JSON_TAG_RATING, String.valueOf(Utils.fiveRatingToTenRating(l.getDouble(JSON_TAG_RATING))));
				}

				if(l.has(JSON_TAG_LATITUDE)) {
					latitude = l.getDouble(JSON_TAG_LATITUDE);
					utcLoc.put(JSON_TAG_LATITUDE, String.valueOf(latitude));
					Logger.verbose(mName, "added " + String.valueOf(latitude) + " latitude");
				}

				if(l.has(JSON_TAG_LONGITUDE)) {
					longitude = l.getDouble(JSON_TAG_LONGITUDE);
					utcLoc.put(JSON_TAG_LONGITUDE, String.valueOf(longitude));
					Logger.verbose(mName, "added " + String.valueOf(longitude) + " longitude");
				}

				if(l.has(JSON_TAG_CATID)) {
					utcLoc.put(JSON_TAG_CATID, String.valueOf(l.getInt(JSON_TAG_CATID)));
				}

				if(l.has(JSON_TAG_CATNAME)) {
					utcLoc.put(JSON_TAG_CATNAME, l.getString(JSON_TAG_CATNAME));
				}

				if(l.has(JSON_TAG_DEALID)) {
					utcLoc.put(JSON_TAG_DEALID, String.valueOf(l.getInt(JSON_TAG_DEALID)));
				}

				if(l.has(JSON_TAG_DEALAMOUNT)) {
					// format the string appropriately based on it's value - zero or negative deal
					// amounts should display as not applicable; even dollar amounts should only
					// display in dollar format (no decimal)
					String dealAmountStr;
					double dealAmount = l.getDouble(JSON_TAG_DEALAMOUNT);
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

					utcLoc.put(JSON_TAG_DEALAMOUNT, dealAmountStr);
				}

				if(l.has(JSON_TAG_MYISREDEEMED)) {
					utcLoc.put(JSON_TAG_MYISREDEEMED, String.valueOf(l.getBoolean(JSON_TAG_MYISREDEEMED)));
				}

				if(l.has(JSON_TAG_MYISCHECKEDIN)) {
					utcLoc.put(JSON_TAG_MYISCHECKEDIN, String.valueOf(l.getBoolean(JSON_TAG_MYISCHECKEDIN)));
				}

				if(l.has(JSON_TAG_MYCHECKINTIME)) {
					utcLoc.put(JSON_TAG_MYCHECKINTIME, l.getString(JSON_TAG_MYCHECKINTIME));
				}

				if(l.has(JSON_TAG_MYREDEEMDATE)) {
					utcLoc.put(JSON_TAG_MYREDEEMDATE, l.getString(JSON_TAG_MYREDEEMDATE));
				}

				if(l.has(JSON_TAG_ENTERTAINER)) {
					utcLoc.put(JSON_TAG_ENTERTAINER, String.valueOf(l.getBoolean(JSON_TAG_ENTERTAINER)));
				}

				if(l.has(JSON_TAG_DISTANCE)) {
					utcLoc.put(JSON_TAG_DISTANCE, "0");
				}

				double ourLatitude = 0;
				double ourLongitude = 0;

				if(latitude == 0.0 && longitude == 0.0) {
					Logger.verbose(mName, "location located at 0.0, 0.0");
					utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
				}
				else {
					float[] results = new float[4]; // results can contain initial and final bearing

					if(loc != null) {
						if(loc.getLatitude() != 0.0 && loc.getLongitude() != 0.0) {
							ourLatitude = loc.getLatitude();
							ourLongitude = loc.getLongitude();
						}
						else {
							// without a valid location, stick 'em in at the default coordinates
							ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
							ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
						}
					}
					else {
						// without a valid location, stick 'em in at the default coordinates
						ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
						ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
					}
					Location.distanceBetween(ourLatitude,
							ourLongitude,
							Double.parseDouble(l.getString(JSON_TAG_LATITUDE)),
							Double.parseDouble(l.getString(JSON_TAG_LONGITUDE)),
							results);

					if(results.length > 0) {
						float miles = results[0] / Constants.METERS_PER_MILE;
						DecimalFormat df = new DecimalFormat("#.0");
						utcLoc.put(JSON_TAG_DISTANCE, String.valueOf(df.format(miles)));
						Logger.verbose(mName, "formatted distance of " + utcLoc.get(JSON_TAG_DISTANCE));
					}
					else {
						Logger.warn(mName, "empty results when calculating distance");
						utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
					}

					latitude = 0.0;
					longitude = 0.0;
				}

				DataManager.getInstance().addLocation(utcLoc);
			}
			Logger.verbose(mName, "added " + json.length() + " locations");
		} catch (JSONException e) {
			e.printStackTrace();
		}

		return null;
	}

	public static String setLocations(String token) {
		// getting JSON string from URL
		JSONArray json = UTCWebAPI.getLocationsSummary(token);

		// if JSON request didn't work, bail
		if(json == null) {
			return FAILED;
		}

		// data manager instance
		UTCGeolocationManager lm = UTCGeolocationManager.getInstance();
		Location loc = lm.getBestLocation();

		double latitude = 0.0;
		double longitude = 0.0;

		try {
			// looping through all locations
			for(int i = 0; i < json.length(); i++) {
				JSONObject l = json.getJSONObject(i);

				// storing each JSON item in UTCLocation
				UTCLocation utcLoc = new UTCLocation(l.getInt(JSON_TAG_ID));

				if(l.has(JSON_TAG_PROPERTIES)) {
					JSONArray properties = l.getJSONArray(JSON_TAG_PROPERTIES);
					if(properties != null) {
						utcLoc.put(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));
						for(int j = 0; j < properties.length(); j++) {
							utcLoc.put(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
						}
					}
					else {
						Logger.error(mName, "properties null");
					}
				}

				if(l.has(JSON_TAG_ID)) {
					int id = l.getInt(JSON_TAG_ID);
					utcLoc.put(JSON_TAG_ID, String.valueOf(id));
				}

				if(l.has(JSON_TAG_BUSID)) {
					utcLoc.put(JSON_TAG_BUSID, String.valueOf(l.getInt(JSON_TAG_BUSID)));
				}

				if(l.has(JSON_TAG_BUSGUID)) {
					utcLoc.put(JSON_TAG_BUSGUID, l.getString(JSON_TAG_BUSGUID));
				}

				if(l.has(JSON_TAG_CITID)) {
					utcLoc.put(JSON_TAG_CITID, String.valueOf(l.getInt(JSON_TAG_CITID)));
				}

				if(l.has(JSON_TAG_NAME)) {
					utcLoc.put(JSON_TAG_NAME, l.getString(JSON_TAG_NAME));
				}

				if(l.has(JSON_TAG_ADDRESS)) {
					utcLoc.put(JSON_TAG_ADDRESS, l.getString(JSON_TAG_ADDRESS));
				}

				if(l.has(JSON_TAG_RATING)) {
					utcLoc.put(JSON_TAG_RATING, String.valueOf(Utils.fiveRatingToTenRating(l.getDouble(JSON_TAG_RATING))));
				}

				if(l.has(JSON_TAG_LATITUDE)) {
					latitude = l.getDouble(JSON_TAG_LATITUDE);
					utcLoc.put(JSON_TAG_LATITUDE, String.valueOf(latitude));
					Logger.verbose(mName, "added " + String.valueOf(latitude) + " latitude");
				}

				if(l.has(JSON_TAG_LONGITUDE)) {
					longitude = l.getDouble(JSON_TAG_LONGITUDE);
					utcLoc.put(JSON_TAG_LONGITUDE, String.valueOf(longitude));
					Logger.verbose(mName, "added " + String.valueOf(longitude) + " longitude");
				}

				if(l.has(JSON_TAG_CATID)) {
					utcLoc.put(JSON_TAG_CATID, String.valueOf(l.getInt(JSON_TAG_CATID)));
				}

				if(l.has(JSON_TAG_CATNAME)) {
					utcLoc.put(JSON_TAG_CATNAME, l.getString(JSON_TAG_CATNAME));
				}

				if(l.has(JSON_TAG_ENTERTAINER)) {
					utcLoc.put(JSON_TAG_ENTERTAINER, String.valueOf(l.getBoolean(JSON_TAG_ENTERTAINER)));
				}

				if(l.has(JSON_TAG_DEALID)) {
					utcLoc.put(JSON_TAG_DEALID, String.valueOf(l.getInt(JSON_TAG_DEALID)));
				}

				if(l.has(JSON_TAG_DEALAMOUNT)) {
					// format the string appropriately based on it's value - zero or negative deal
					// amounts should display as not applicable; even dollar amounts should only
					// display in dollar format (no decimal)
					String dealAmountStr;
					double dealAmount = l.getDouble(JSON_TAG_DEALAMOUNT);
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

					utcLoc.put(JSON_TAG_DEALAMOUNT, dealAmountStr);
				}

				if(l.has(JSON_TAG_MYISREDEEMED)) {
					utcLoc.put(JSON_TAG_MYISREDEEMED, String.valueOf(l.getBoolean(JSON_TAG_MYISREDEEMED)));
				}

				if(l.has(JSON_TAG_MYISCHECKEDIN)) {
					utcLoc.put(JSON_TAG_MYISCHECKEDIN, String.valueOf(l.getBoolean(JSON_TAG_MYISCHECKEDIN)));
				}

				if(l.has(JSON_TAG_MYCHECKINTIME)) {
					utcLoc.put(JSON_TAG_MYCHECKINTIME, l.getString(JSON_TAG_MYCHECKINTIME));
				}

				if(l.has(JSON_TAG_MYREDEEMDATE)) {
					utcLoc.put(JSON_TAG_MYREDEEMDATE, l.getString(JSON_TAG_MYREDEEMDATE));
				}

				if(l.has(JSON_TAG_DISTANCE)) {
					utcLoc.put(JSON_TAG_DISTANCE, "0");
				}

				double ourLatitude = 0;
				double ourLongitude = 0;

				if(latitude == 0.0 && longitude == 0.0) {
					Logger.verbose(mName, "location located at 0.0, 0.0");
					utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
				}
				else {
					float[] results = new float[4]; // results can contain initial and final bearing

					if(loc != null) {
						if(loc.getLatitude() != 0.0 && loc.getLongitude() != 0.0) {
							ourLatitude = loc.getLatitude();
							ourLongitude = loc.getLongitude();
						}
						else {
							// without a valid location, stick 'em in at the default coordinates
							ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
							ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
						}
					}
					else {
						// without a valid location, stick 'em in at the default coordinates
						ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
						ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
					}
					Location.distanceBetween(ourLatitude,
							ourLongitude,
							Double.parseDouble(l.getString(JSON_TAG_LATITUDE)),
							Double.parseDouble(l.getString(JSON_TAG_LONGITUDE)),
							results);

					if(results.length > 0) {
						float miles = results[0] / Constants.METERS_PER_MILE;
						DecimalFormat df = new DecimalFormat("#.0");
						utcLoc.put(JSON_TAG_DISTANCE, String.valueOf(df.format(miles)));
						Logger.verbose(mName, "formatted distance of " + utcLoc.get(JSON_TAG_DISTANCE));
					}
					else {
						Logger.warn(mName, "empty results when calculating distance");
						utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
					}

					latitude = 0.0;
					longitude = 0.0;
				}

				DataManager.getInstance().addLocation(utcLoc);
			}
			Logger.verbose(mName, "added " + json.length() + " locations");
		} catch (JSONException e) {
			e.printStackTrace();
		}

		return null;
	}

    public static String setLocation(Integer locId) throws JSONException {
        // getting JSON string from URL
        JSONObject l = UTCWebAPI.getLocation(locId);
        
        // if JSON request didn't work, bail
        if(l == null) {
        	return "Failed to retrieve locations";
        }
        
        // data manager instance
        DataManager dm = DataManager.getInstance();
        
	    Location gps = dm.getLastGPSLocation();
	    Location net = dm.getLastNetworkLocation();
	    Location loc;
		if(UTCGeolocationManager.isBetterLocation(gps, net)) {
	    	loc = gps;
	    }
	    else {
	    	loc = net;
	    }
		
		double latitude = 0.0;
		double longitude = 0.0;
                
		// storing each JSON item in UTCLocation
		UTCLocation utcLoc = new UTCLocation(l.getInt(JSON_TAG_ID));
		JSONArray properties = l.getJSONArray(JSON_TAG_PROPERTIES);
		if(properties != null) {
			utcLoc.put(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));

			for(int j = 0; j < properties.length(); j++) {
				utcLoc.put(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
			}
		}

		if(l.has(JSON_TAG_ID)) {
			utcLoc.put(JSON_TAG_ID, String.valueOf(l.getInt(JSON_TAG_ID)));
		}

		if(l.has(JSON_TAG_BUSID)) {
			utcLoc.put(JSON_TAG_BUSID, String.valueOf(l.getInt(JSON_TAG_BUSID)));
		}

		if(l.has(JSON_TAG_BUSGUID)) {
			utcLoc.put(JSON_TAG_BUSGUID, l.getString(JSON_TAG_BUSGUID));
		}

		if(l.has(JSON_TAG_CITID)) {
			utcLoc.put(JSON_TAG_CITID, String.valueOf(l.getInt(JSON_TAG_CITID)));
		}

		if(l.has(JSON_TAG_NAME)) {
			utcLoc.put(JSON_TAG_NAME, l.getString(JSON_TAG_NAME));
		}

		if(l.has(JSON_TAG_ADDRESS)) {
			utcLoc.put(JSON_TAG_ADDRESS, l.getString(JSON_TAG_ADDRESS));
		}

		if(l.has(JSON_TAG_RATING)) {
			utcLoc.put(JSON_TAG_RATING, String.valueOf(Utils.fiveRatingToTenRating(l.getDouble(JSON_TAG_RATING))));
		}

		if(l.has(JSON_TAG_LATITUDE)) {
			latitude = l.getDouble(JSON_TAG_LATITUDE);
			utcLoc.put(JSON_TAG_LATITUDE, String.valueOf(latitude));
			Logger.verbose(mName, "added " + String.valueOf(latitude) + " latitude");
		}

		if(l.has(JSON_TAG_LONGITUDE)) {
			longitude = l.getDouble(JSON_TAG_LONGITUDE);
			utcLoc.put(JSON_TAG_LONGITUDE, String.valueOf(longitude));
			Logger.verbose(mName, "added " + String.valueOf(longitude) + " longitude");
		}

		if(l.has(JSON_TAG_CATID)) {
			utcLoc.put(JSON_TAG_CATID, String.valueOf(l.getInt(JSON_TAG_CATID)));
		}

		if(l.has(JSON_TAG_CATNAME)) {
			utcLoc.put(JSON_TAG_CATNAME, l.getString(JSON_TAG_CATNAME));
		}

		if(l.has(JSON_TAG_DISTANCE)) {
			utcLoc.put(JSON_TAG_DISTANCE, "0");
		}

		double ourLatitude = 0;
		double ourLongitude = 0;

		if(latitude == 0.0 && longitude == 0.0) {
			Logger.verbose(mName, "location located at 0.0, 0.0");
			utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
		}
		else {
			float[] results = new float[4]; // results can contain initial and final bearing

			if(loc != null) {
				if(loc.getLatitude() != 0.0 && loc.getLongitude() != 0.0) {
					ourLatitude = loc.getLatitude();
					ourLongitude = loc.getLongitude();
				}
				else {
					// without a valid location, stick 'em in at the default coordinates
					ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
					ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
				}
			}
			else {
				// without a valid location, stick 'em in at the default coordinates
				ourLatitude = Constants.LOCATION_DEFAULT_LATITUDE;
				ourLongitude = Constants.LOCATION_DEFAULT_LONGITUDE;
			}
			Location.distanceBetween(ourLatitude, 
					ourLongitude, 
					Double.parseDouble(l.getString(JSON_TAG_LATITUDE)), 
					Double.parseDouble(l.getString(JSON_TAG_LONGITUDE)), 
					results);

			if(results.length > 0) {
				float miles = results[0] / Constants.METERS_PER_MILE;
				DecimalFormat df = new DecimalFormat("#.0");
				utcLoc.put(JSON_TAG_DISTANCE, String.valueOf(df.format(miles)));
				Logger.verbose(mName, "formatted distance of " + utcLoc.get(JSON_TAG_DISTANCE));
			}
			else {
				Logger.warn(mName, "empty results when calculating distance");
				utcLoc.put(JSON_TAG_DISTANCE, Constants.LOCATION_MISSING_TAG);
			}

			latitude = 0.0;
			longitude = 0.0;
		}

		DataManager.getInstance().addLocation(utcLoc);
        
        return null;
    }
}
