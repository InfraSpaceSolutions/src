package com.unitethiscity.data;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.os.SystemClock;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class EventsParser {

	private static String mName = Constants.EVENTS_PARSER;

	public static final String 	JSON_TAG_EVENTS 						= ""; // array with no name
	public static final String 	JSON_TAG_PROPERTIES 					= "Properties"; // array of strings
	public static final String 	JSON_TAG_ID								= "Id";
	public static final String 	JSON_TAG_ETTID 							= "EttId";
	public static final String 	JSON_TAG_BUSID 							= "BusId";
	public static final String	JSON_TAG_BUSGUID						= "BusGuid";
    public static final String 	JSON_TAG_CITID 							= "CitId";
    public static final String 	JSON_TAG_BUSNAME						= "BusName";
    public static final String 	JSON_TAG_STARTDATE 						= "StartDate";
    public static final String 	JSON_TAG_ENDDATE 						= "EndDate";
    public static final String 	JSON_TAG_DATE_AS_STR 					= "DateAsString";
    public static final String 	JSON_TAG_SORTABLE_DATE					= "SortableDate";
    public static final String 	JSON_TAG_SUMMARY 						= "Summary";
    public static final String 	JSON_TAG_EVENT_TYPE 					= "EventType";
    public static final String 	JSON_TAG_CATID 							= "CatId";
    public static final String 	JSON_TAG_CATNAME 						= "CatName";
	
    public static String setEvents() {
    	if(DataManager.getInstance().doEventsNeedUpdated()) {
    	       // getting JSON string from URL
            JSONArray json = UTCWebAPI.getEvents();
            
            // if JSON request didn't work, bail
            if(json == null) {
            	return "Failed to retrieve events";
            }
            
            try {
                // looping through all events
                for(int i = 0; i < json.length(); i++) {
                    JSONObject l = json.getJSONObject(i);
                    
                    // storing each JSON item in UTCLocation
                    UTCEvent utcEvt = new UTCEvent(l.getInt(JSON_TAG_ID));
                    JSONArray properties = l.getJSONArray(JSON_TAG_PROPERTIES);
                    if(properties != null) {
                    	utcEvt.put(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));
                    	
                    	for(int j = 0; j < properties.length(); j++) {
                    		utcEvt.put(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
                    	}
                    }
                    
                    if(l.has(JSON_TAG_ID)) {
                    	utcEvt.put(JSON_TAG_ID, String.valueOf(l.getInt(JSON_TAG_ID)));
                    }
                    
                    if(l.has(JSON_TAG_ETTID)) {
                    	utcEvt.put(JSON_TAG_ETTID, String.valueOf(l.getInt(JSON_TAG_ETTID)));
                    }
                    
                    if(l.has(JSON_TAG_BUSID)) {
                    	utcEvt.put(JSON_TAG_BUSID, String.valueOf(l.getInt(JSON_TAG_BUSID)));
                    }
                    
                    if(l.has(JSON_TAG_BUSGUID)) {
                    	utcEvt.put(JSON_TAG_BUSGUID, l.getString(JSON_TAG_BUSGUID));
                    }
                    
                    if(l.has(JSON_TAG_CITID)) {
                    	utcEvt.put(JSON_TAG_CITID, String.valueOf(l.getInt(JSON_TAG_CITID)));
                	}
                    
                    if(l.has(JSON_TAG_BUSNAME)) {
                    	utcEvt.put(JSON_TAG_BUSNAME, l.getString(JSON_TAG_BUSNAME));
                	}
                    
                    if(l.has(JSON_TAG_STARTDATE)) {
                    	utcEvt.put(JSON_TAG_STARTDATE, l.getString(JSON_TAG_STARTDATE));
                    }
                    
                    if(l.has(JSON_TAG_ENDDATE)) {
                    	utcEvt.put(JSON_TAG_ENDDATE, l.getString(JSON_TAG_ENDDATE));
                    }
                    
                    if(l.has(JSON_TAG_DATE_AS_STR)) {
                    	utcEvt.put(JSON_TAG_DATE_AS_STR, l.getString(JSON_TAG_DATE_AS_STR));
                    }
                    
                    if(l.has(JSON_TAG_SORTABLE_DATE)) {
                    	utcEvt.put(JSON_TAG_SORTABLE_DATE, l.getString(JSON_TAG_SORTABLE_DATE));
                    }
                    
                    if(l.has(JSON_TAG_SUMMARY)) {
                    	utcEvt.put(JSON_TAG_SUMMARY, l.getString(JSON_TAG_SUMMARY));
                    }
                    
                    if(l.has(JSON_TAG_EVENT_TYPE)) {
                    	utcEvt.put(JSON_TAG_EVENT_TYPE, l.getString(JSON_TAG_EVENT_TYPE));
                    }
                    
                    if(l.has(JSON_TAG_CATID)) {
                    	utcEvt.put(JSON_TAG_CATID, String.valueOf(l.getInt(JSON_TAG_CATID)));
                    }
                    
                    if(l.has(JSON_TAG_CATNAME)) {
                    	utcEvt.put(JSON_TAG_CATNAME, l.getString(JSON_TAG_CATNAME));
                    } 
                    
                    DataManager.getInstance().addEvent(utcEvt);
                    
                    DataManager.getInstance().setEventsTimestamp(SystemClock.elapsedRealtime());
                }
                Logger.verbose(mName, "added " + json.length() + " events");
            } catch (JSONException e) {
                e.printStackTrace();
            }
    	}
        
        return null;
    }
    
    public static String setEvent(Integer evtID) throws JSONException {
    	if(DataManager.getInstance().doEventsNeedUpdated()) {
            // getting JSON string from URL
            JSONObject l = UTCWebAPI.getEvent(evtID);
            
            // if JSON request didn't work, bail
            if(l == null) {
            	return "Failed to retrieve event";
            }

            // storing each JSON item in UTCLocation
            UTCEvent utcEvt = new UTCEvent(l.getInt(JSON_TAG_CATID));
            JSONArray properties = l.getJSONArray(JSON_TAG_PROPERTIES);
            if(properties != null) {
            	utcEvt.put(JSON_TAG_PROPERTIES + "Size", String.valueOf(properties.length()));

            	for(int j = 0; j < properties.length(); j++) {
            		utcEvt.put(JSON_TAG_PROPERTIES + String.valueOf(j), properties.getString(j));
            	}
            }

            if(l.has(JSON_TAG_ID)) {
            	utcEvt.put(JSON_TAG_ID, String.valueOf(l.getInt(JSON_TAG_ID)));
            }

            if(l.has(JSON_TAG_ETTID)) {
            	utcEvt.put(JSON_TAG_ETTID, String.valueOf(l.getInt(JSON_TAG_ETTID)));
            }

            if(l.has(JSON_TAG_BUSID)) {
            	utcEvt.put(JSON_TAG_BUSID, String.valueOf(l.getInt(JSON_TAG_BUSID)));
            }

            if(l.has(JSON_TAG_BUSGUID)) {
            	utcEvt.put(JSON_TAG_BUSGUID, l.getString(JSON_TAG_BUSGUID));
            }

            if(l.has(JSON_TAG_CITID)) {
            	utcEvt.put(JSON_TAG_CITID, String.valueOf(l.getInt(JSON_TAG_CITID)));
            }
            
            if(l.has(JSON_TAG_BUSNAME)) {
            	utcEvt.put(JSON_TAG_BUSNAME, l.getString(JSON_TAG_BUSNAME));
        	}

            if(l.has(JSON_TAG_STARTDATE)) {
            	utcEvt.put(JSON_TAG_STARTDATE, l.getString(JSON_TAG_STARTDATE));
            }

            if(l.has(JSON_TAG_ENDDATE)) {
            	utcEvt.put(JSON_TAG_ENDDATE, l.getString(JSON_TAG_ENDDATE));
            }

            if(l.has(JSON_TAG_DATE_AS_STR)) {
            	utcEvt.put(JSON_TAG_DATE_AS_STR, l.getString(JSON_TAG_DATE_AS_STR));
            }

            if(l.has(JSON_TAG_SORTABLE_DATE)) {
            	utcEvt.put(JSON_TAG_SORTABLE_DATE, l.getString(JSON_TAG_SORTABLE_DATE));
            }

            if(l.has(JSON_TAG_SUMMARY)) {
            	utcEvt.put(JSON_TAG_SUMMARY, l.getString(JSON_TAG_SUMMARY));
            }

            if(l.has(JSON_TAG_EVENT_TYPE)) {
            	utcEvt.put(JSON_TAG_EVENT_TYPE, l.getString(JSON_TAG_EVENT_TYPE));
            }

            if(l.has(JSON_TAG_CATID)) {
            	utcEvt.put(JSON_TAG_CATID, String.valueOf(l.getInt(JSON_TAG_CATID)));
            }

            if(l.has(JSON_TAG_CATNAME)) {
            	utcEvt.put(JSON_TAG_CATNAME, l.getString(JSON_TAG_CATNAME));
            } 

            DataManager.getInstance().addEvent(utcEvt);
            
            DataManager.getInstance().setEventsTimestamp(SystemClock.elapsedRealtime());
    	}

        return null;
    }
}