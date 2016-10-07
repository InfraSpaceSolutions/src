package com.unitethiscity.data;

import java.util.Iterator;

import android.content.Context;
import android.location.GpsSatellite;
import android.location.GpsStatus;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class UTCGeolocationManager {
	
	private String mName = Constants.GEOLOCATION_MANAGER;
	private static UTCGeolocationManager mInstance = null;

	private Context mContext;
	private LocationManager mLocationManager;
	private Location mLocation;
	private int mNumberOfSatellites;
	private boolean mCoarseLocationAccess = false;
	private boolean mFineLocationAccess = false;
	
	public UTCGeolocationManager() {
		mLocation = new Location(LocationManager.GPS_PROVIDER);
		mNumberOfSatellites = 0;
	}
	
	public static UTCGeolocationManager getInstance() {
		if(mInstance == null) {
			mInstance = new UTCGeolocationManager();
		}
		return mInstance;
	}
	
	public static void destroyInstance() {
		mInstance = null;
	}
	
	public void cleanup() {
		mLocationManager = null;
		mLocation = null;
		mNumberOfSatellites = 0;
	}
	
	public void initializeLocationManager(Context c) {
		mContext = c;
		if(mContext != null && mLocationManager == null) {
			mLocationManager = (LocationManager) c.getSystemService(Context.LOCATION_SERVICE);
		}
	}
	
	public LocationManager getLocationManager() {
		return mLocationManager;
	}

	public void setCoarseAccess(boolean access) { mCoarseLocationAccess = access; }

	public void setFineAccess(boolean access) {
		mFineLocationAccess = access;
	}

	public boolean getCoarseAccess() { return mCoarseLocationAccess; };

	public boolean getFineAccess() { return mFineLocationAccess; };
	
	public Location getLocation() {
		return mLocation;
	}

    /** Determines whether one Location reading is better than the current Location fix
      * @param location  The new Location that you want to evaluate
      * @param currentBestLocation  The current Location fix, to which you want to compare the new one
      */
    public static boolean isBetterLocation(Location location, Location currentBestLocation) {
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
    private static boolean isSameProvider(String provider1, String provider2) {
        if (provider1 == null) {
          return provider2 == null;
        }
        return provider1.equals(provider2);
    }
    
    public boolean areLocationsStale() {
        return isCurrentLocationStale() && isGPSLocationStale() && isNetworkLocationStale();
    }
    
    public boolean isCurrentLocationStale() {
    	boolean isStale = false;
    	
    	if(mLocation != null) {
    		if(System.currentTimeMillis() - mLocation.getTime() > Constants.TWO_MINUTES) {
    			isStale = true;
    		}
    	}
    	
    	return isStale;
    }
    
    public boolean isGPSLocationStale() {
    	boolean isStale = false;

		if(mFineLocationAccess) {
			try {
				Location locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
				if (locationGPS != null) {
					if (System.currentTimeMillis() - locationGPS.getTime() > Constants.TWO_MINUTES) {
						isStale = true;
					}
				}
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in isGPSLocationStale()");
			}
		}
    	
    	return isStale;
    }
    
    public boolean isNetworkLocationStale() {
    	boolean isStale = false;

		if(mCoarseLocationAccess) {
			try {
				Location locationNetwork = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
				if (locationNetwork != null) {
					if (System.currentTimeMillis() - locationNetwork.getTime() > Constants.TWO_MINUTES) {
						isStale = true;
					}
				}
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in isNetworkLocationStale()");
			}
		}

    	return isStale;
    }
    
    public boolean areLocationsVeryStale() {
        return isCurrentLocationStale() && isGPSLocationStale() && isNetworkLocationStale();
    }
    
    public boolean isCurrentLocationVeryStale() {
    	boolean isStale = false;
    	
    	if(mLocation != null) {
    		if(System.currentTimeMillis() - mLocation.getTime() > Constants.FIFTEEN_MINUTES) {
    			isStale = true;
    		}
    	}
    	
    	return isStale;
    }
    
    public boolean isGPSLocationVeryStale() {
    	boolean isStale = false;

		if(mFineLocationAccess) {
			try {
				Location locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
				if (locationGPS != null) {
					if (System.currentTimeMillis() - locationGPS.getTime() > Constants.FIFTEEN_MINUTES) {
						isStale = true;
					}
				}
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in isGPSLocationVeryStale()");
			}
		}
    	
    	return isStale;
    }
    
    public boolean isNetworkLocationVeryStale() {
    	boolean isStale = false;

		if(mCoarseLocationAccess) {
			try {
				Location locationNetwork = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
				if (locationNetwork != null) {
					if (System.currentTimeMillis() - locationNetwork.getTime() > Constants.FIFTEEN_MINUTES) {
						isStale = true;
					}
				}
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in isNetworkLocationVeryStale()");
			}
		}
    	
    	return isStale;
    }
    
    /** Return best location between current, last GPS, and last network locations */
    public Location getBestLocation() {
		Location locationGPS = new Location("");
		Location locationNetwork = new Location("");

		if(mLocationManager != null) {
			try {
				locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in getBestLocation() for locationGPS");
			}

			try {
				locationNetwork = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in getBestLocation() for locationNetwork");
			}
		}

        try {
			if (isBetterLocation(mLocation, locationGPS) &&
					isBetterLocation(mLocation, locationNetwork)) {
				return mLocation;
			}

			if (isBetterLocation(locationGPS, mLocation) &&
					isBetterLocation(locationGPS, locationNetwork)) {
				return locationGPS;
			}
		}
		catch (Exception ex) {
			return locationNetwork;
		}
        	
        return locationNetwork;
    }
	
/*
    private Location getNewestLocation() {
        Location locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
        Location locationNetwork = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);

        long GPSLocationTime = 0;
        if (locationGPS != null) { 
        	GPSLocationTime = locationGPS.getTime(); 
        }

        long NetworkLocationTime = 0;

        if (locationNetwork != null) {
            NetworkLocationTime = locationNetwork.getTime();
        }

        if ( 0 < GPSLocationTime - NetworkLocationTime ) {
            return locationGPS;
        }
        else{
            return locationNetwork;
        }
    }
*/
    
    public void locationSingleUpdate(int providers) {
		// force location update
		DataManager.getInstance().forceLocationUpdate();

    	// providers = 1, GPS only
    	// providers = 2, network only
    	// providers = 3, both
	    if(mFineLocationAccess && mLocationManager.isProviderEnabled(LocationManager.GPS_PROVIDER) && (providers & 1) != 0) {
			try {
				if (Utils.hasGingerbread()) {
					mLocationManager.requestSingleUpdate(LocationManager.GPS_PROVIDER, locationListener, null);
				} else {
					mLocationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER,
							Constants.TWO_MINUTES,
							Constants.LOCATION_ACCURACY_LIMIT,
							locationListener);
				}
				Logger.info(mName, "Requesting single location update from GPS");
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in locationSingleUpdate() for GPS");
			}
	    }

	    if(mFineLocationAccess && mLocationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER) && (providers & 2) != 0) {
			try {
				if (Utils.hasGingerbread()) {
					mLocationManager.requestSingleUpdate(LocationManager.NETWORK_PROVIDER, locationListener, null);
				} else {
					mLocationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER,
							Constants.TWO_MINUTES,
							Constants.LOCATION_ACCURACY_LIMIT,
							locationListener);
				}
				Logger.info(mName, "Requesting single location update from network");
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in locationSingleUpdate() for network");
			}
	    }
    }
    
    public void removeLocationUpdates() {
		if(mCoarseLocationAccess || mFineLocationAccess) {
			try {
				mLocationManager.removeUpdates(locationListener);
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception in removeLocationUpdates()");
			}
		}
    }
    
    public boolean areProvidersOff() {
		Location locationGPS = null;
		Location locationNetwork = null;
		try {
			if (mFineLocationAccess) {
				locationGPS = mLocationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
			}
			if (mCoarseLocationAccess) {
				locationNetwork = mLocationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
			}
		}
		catch(SecurityException ex) {
			Logger.error(mName, "Security exception in areProvidersOff()");
		}
        
        // check ability to grab at least one of the two locations
        // seems as though isProviderEnabled for GPS can return true even when location sharing is off completely
    	return locationGPS == null && locationNetwork == null;
    }
    
    // Define a listener that responds to GPS status updates
    GpsStatus.Listener gpsListener = new GpsStatus.Listener() {
        public void onGpsStatusChanged(int event) {
            if (event == GpsStatus.GPS_EVENT_SATELLITE_STATUS) {
                GpsStatus status = mLocationManager.getGpsStatus(null);
                Iterable<GpsSatellite> sats = status.getSatellites();
                // Check number of satellites in list to determine fix state
                Iterator<GpsSatellite> itr = sats.iterator();
                mNumberOfSatellites = 0;
                while(itr.hasNext()) {
                	mNumberOfSatellites++;
                	itr.next();
                }
                Logger.info(mName, "GPS status change, number of satellites - " + mNumberOfSatellites);
            }
        }
    };
    
	// Define a listener that responds to location updates
	public LocationListener locationListener = new LocationListener() {
	    public void onLocationChanged(Location location) {
			try {
				// Called when a new location is found by the location provider.
				String provider = location.getProvider();
				if (provider.equals(LocationManager.GPS_PROVIDER)) {
					DataManager.getInstance().setLastGPSLocation(location);
				} else if (provider.equals(LocationManager.NETWORK_PROVIDER)) {
					DataManager.getInstance().setLastNetworkLocation(location);
				} else if (provider.equals(LocationManager.PASSIVE_PROVIDER)) {
					DataManager.getInstance().setLastPassiveLocation(location);
				}

				if (isBetterLocation(location, mLocation)) {
					mLocation = location;
				}

				Logger.info(mName, "onLocationChanged - accuracy "
						+ String.valueOf(location.getAccuracy())
						+ " meters, age "
						+ String.valueOf(System.currentTimeMillis() - location.getTime())
						+ " milliseconds");

				if (mNumberOfSatellites < Constants.LOCATION_MIN_GPS_SATS) {
					Logger.info(mName, "Number of satellties locked too low - " + mNumberOfSatellites);
				}

				if(mFineLocationAccess || mCoarseLocationAccess) {
					mLocationManager.removeUpdates(locationListener);
				}

				if (location.getAccuracy() > Constants.LOCATION_ACCURACY_LIMIT) {
					Logger.info(mName, "Accuracy too poor, requesting update again");
					if (provider.equals(LocationManager.NETWORK_PROVIDER)) {
						locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_GPS);
					} else {
						locationSingleUpdate(Constants.LOCATION_SINGLE_UPDATE_BOTH);
					}
				} else {
					// update UTC locations since our geolocation has changed
					DataManager.getInstance().setLocationsNeedUpdated(true);
					DataManager.getInstance().forceLocationUpdate();
					// let's update the wallet too while we're at it - local changes
					// force an update, but if a business scans on their side the user
					// won't see an updated wallet. with this, eventually (relatively)
					// soon they will.
					DataManager.getInstance().setWalletNeedsUpdate(true);
				}
			}
			catch(SecurityException ex) {
				Logger.error(mName, "Security exception onLocationChanged");
			}
	    }

	    public void onStatusChanged(String provider, int status, Bundle extras) {}

	    public void onProviderEnabled(String provider) {}

	    public void onProviderDisabled(String provider) {}
	};
}
