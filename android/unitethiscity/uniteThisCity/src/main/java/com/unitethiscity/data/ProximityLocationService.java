package com.unitethiscity.data;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.IBinder;
import android.support.v4.content.ContextCompat;

public class ProximityLocationService extends Service {
	
	private String mName = Constants.PROXIMITY_LOCATION_SERVICE;
	
	public final static int 	MILLI 									= 1000;
	public final static long 	NANO 									= 1000 * 1000 * 1000;
	public final static String  PROXIMITY_LOCATION_ACTION 				= "com.unitethiscity.data.ProximityLocationService";
	public final static int 	LOCATION_PROXIMITY_UPDATE_INTERVAL		= 1 * 60 * MILLI; // milliseconds
	public final static int 	LOCATION_PROXIMITY_MIN_DISTANCE_UPDATE 	= 100; // meters
	public final static long 	PROXIMITY_THROTTLE_TIME 				= 10 * 60 * NANO; // nanoseconds
	
	private long mLastProximityUpload;

	public LocationManager locationManager;
	public ProxmityLocationListener listener;
	public Location previousBestLocation = null;
	
	Intent intent;
	int counter = 0;
	
	@Override
	public void onCreate() {
	    super.onCreate();
	    intent = new Intent(PROXIMITY_LOCATION_ACTION);
	}
	
	@Override
	public void onStart(Intent intent, int startId) {
		Logger.info(mName, "proximity service starting");
		
		mLastProximityUpload = 0;
		
	    locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);

		if(ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION)
				== PackageManager.PERMISSION_GRANTED) {
			requestLocationUpdates(LOCATION_PROXIMITY_UPDATE_INTERVAL,
					LOCATION_PROXIMITY_MIN_DISTANCE_UPDATE);
		}
	}
	
	@Override
	public IBinder onBind(Intent intent) {
	    return null;
	}
	
	protected boolean isBetterLocation(Location location, Location currentBestLocation) {
	    if (currentBestLocation == null) {
	        // A new location is always better than no location
	        return true;
	    }
	
	    // Check whether the new location fix is newer or older
	    long timeDelta = location.getTime() - currentBestLocation.getTime();
	    boolean isSignificantlyNewer = timeDelta > Constants.TWO_MINUTES;
	    boolean isSignificantlyOlder = timeDelta < -Constants.TWO_MINUTES;
	    boolean isNewer = timeDelta > 0;
	
	    // If it's been more than two minutes since the current location, use the new location
	    // because the user has likely moved
	    if (isSignificantlyNewer) {
	        return true;
	    // If the new location is more than two minutes older, it must be worse
	    } else if (isSignificantlyOlder) {
	        return false;
	    }
	
	    // Check whether the new location fix is more or less accurate
	    int accuracyDelta = (int) (location.getAccuracy() - currentBestLocation.getAccuracy());
	    boolean isLessAccurate = accuracyDelta > 0;
	    boolean isMoreAccurate = accuracyDelta < 0;
	    boolean isSignificantlyLessAccurate = accuracyDelta > 200;
	
	    // Check if the old and new location are from the same provider
	    boolean isFromSameProvider = isSameProvider(location.getProvider(),
	            currentBestLocation.getProvider());
	
	    // Determine location quality using a combination of timeliness and accuracy
	    if (isMoreAccurate) {
	        return true;
	    } else if (isNewer && !isLessAccurate) {
	        return true;
	    } else if (isNewer && !isSignificantlyLessAccurate && isFromSameProvider) {
	        return true;
	    }
	    return false;
	}
	
	/** Checks whether two providers are the same */
	private boolean isSameProvider(String provider1, String provider2) {
	    if (provider1 == null) {
	      return provider2 == null;
	    }
	    return provider1.equals(provider2);
	}
	
	@Override
	public void onDestroy() {       
	   // handler.removeCallbacks(sendUpdatesToUI);     
	    super.onDestroy();
		Logger.verbose(mName, "onDestroy");
		if(ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION)
				== PackageManager.PERMISSION_GRANTED && listener != null) {
			locationManager.removeUpdates(listener);
		}
	}
	
	private void requestLocationUpdates(long time, float distance) {
	    /* one could return a provider based on some criteria...
		Criteria crta = new Criteria(); 
	    crta.setAccuracy(Criteria.ACCURACY_FINE); 
	    crta.setAltitudeRequired(false); 
	    crta.setBearingRequired(false); 
	    crta.setCostAllowed(true); 
	    crta.setPowerRequirement(Criteria.POWER_LOW); 
	    String provider = locationManager.getBestProvider(crta, true);
	    */
		/* or use the passive provider - this is a special provider 
		   which will use the results of other active providers that
		   are commonly used by a variety of other applications
		   and (should) considerably reduce battery consumption
		*/
	    String provider = LocationManager.PASSIVE_PROVIDER;
	    
	    listener = new ProxmityLocationListener();
		if(ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION)
				== PackageManager.PERMISSION_GRANTED) {
			locationManager.requestLocationUpdates(provider,
					time,
					distance,
					listener);
		}
	}
	
	public class ProxmityLocationListener implements LocationListener
	{
	    public void onLocationChanged(final Location loc)
	    {
	    	Logger.info(mName, "proximity location update");
	        if(isBetterLocation(loc, previousBestLocation) && proximityTimerExpired()) {
	        	Logger.info(mName, "   better location, broadcasting update");
	        	
            	long now = System.nanoTime();
            	Logger.info(mName, "   last proximity upload time: " 
            			+ String.valueOf(mLastProximityUpload / NANO)
            			+ ", now: " + String.valueOf(now / NANO));
            	mLastProximityUpload = now;
	        	
	        	previousBestLocation = loc;
	        	
	        	// broadcast location information
	            intent.putExtra(PROXIMITY_LOCATION_ACTION + ".Latitude", loc.getLatitude());
	            intent.putExtra(PROXIMITY_LOCATION_ACTION + ".Longitude", loc.getLongitude());     
	            intent.putExtra(PROXIMITY_LOCATION_ACTION + ".Provider", loc.getProvider());                
	            sendBroadcast(intent);
	        }                               
	    }
	
	    public void onProviderDisabled(String provider)
	    {
	        
	    }
	
	    public void onProviderEnabled(String provider)
	    {
	        
	    }

		@Override
		public void onStatusChanged(String provider, int status, Bundle extras)
		{
			
		}
	}
	
    private boolean proximityTimerExpired()
    {
    	return (System.nanoTime() - mLastProximityUpload) >= PROXIMITY_THROTTLE_TIME;
    }
}
