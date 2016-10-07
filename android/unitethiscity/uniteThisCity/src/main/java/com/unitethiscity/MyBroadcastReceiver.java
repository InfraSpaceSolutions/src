package com.unitethiscity;

import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.ProximityLocationService;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.content.BroadcastReceiver;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.location.Location;
import android.os.AsyncTask;
import android.os.Bundle;

public class MyBroadcastReceiver extends BroadcastReceiver {

	private static String mName = Constants.MY_BROADCAST_RECEIVER;
	private Context mContext;
	
    @Override
    public final void onReceive(Context context, Intent intent) {
    	Logger.info(mName, "received broadcast");
    	
    	mContext = context;
    	
    	String action = intent.getAction();
    	if(action.equals("com.google.android.c2dm.intent.REGISTRATION") || 
    			action.equals("com.google.android.c2dm.intent.RECEIVE") ||
    			action.equals("com.google.android.c2dm.intent.RETRY")) {
            MyIntentService.runIntentInService(context, intent);
            
            setResult(Activity.RESULT_OK, null, null);
    	}
    	else if(action.equals(ProximityLocationService.PROXIMITY_LOCATION_ACTION)) {
    		Bundle locationExtras = intent.getExtras();
    		double lat = locationExtras.getDouble(ProximityLocationService.PROXIMITY_LOCATION_ACTION + ".Latitude");
    		double lon = locationExtras.getDouble(ProximityLocationService.PROXIMITY_LOCATION_ACTION + ".Longitude");
    		String prov = locationExtras.getString(ProximityLocationService.PROXIMITY_LOCATION_ACTION + ".Provider");

    		Location proxLocation = new Location(prov);
    		proxLocation.setLatitude(lat);
    		proxLocation.setLongitude(lon);
    		DataManager.getInstance().setLastProximityLocation(proxLocation);
    		
    		ProximityTask updateLocation = new ProximityTask();
			if(Utils.hasHoneycomb()) {
				updateLocation.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, proxLocation);
			}
			else {
				updateLocation.execute(proxLocation);
			}
    	}
    	else {
    		DataManager.getInstance().setMenuFromNotification(2);
    		
    		setResult(Activity.RESULT_OK, null, null);
    	}
    }
    
	/**
	 * Send up proximity location coordinates.
	 * 
	 * @author Sanctuary Software Studio, Inc.
	 *
	 */
    private class ProximityTask extends AsyncTask<Location, Void, Void> {
        protected Void doInBackground(Location... params) {
        	Logger.info(mName, "proximity task");
        	
            // get account token and registration ID from shared preferences
            SharedPreferences settings = mContext.getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
            String accountToken = settings.getString(Constants.SHARED_PREFERENCES_ACCTOKEN, "");
            String registrationID = settings.getString(Constants.GCM_REGISTRATION_ID, "");
        	
			for(int i = 0; i < params.length; i++) {
				Location thisLocation = params[i];
				
	            if(registrationID != null && !registrationID.equals("") 
	            		&& accountToken != null && !accountToken.equals(""))
	            {
	            	Logger.info(mName, "requesting proximity notification");
	            	UTCWebAPI.proximity(accountToken, registrationID,
	            			thisLocation.getLatitude(),
	            			thisLocation.getLongitude());
	            }
			}
        	
        	return null;
        }
        
        protected void onPostExecute(Void success) {
        	
        }
    }
}
