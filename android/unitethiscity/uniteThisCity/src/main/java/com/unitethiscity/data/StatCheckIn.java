package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class StatCheckIn {

    int ID;
    int BusinessID;
    String Name;
    String Period;
    String Timestamp;
    String TimestampAsString;
    String TimestampSortable;
    String LocationName;

    public StatCheckIn(int id, int businessID, String name, String period, String timestamp, String timestampAsString, String timestampSortable, String locationName) {
        ID = id;
        BusinessID = businessID;
        Name = name;
        Period = period;
        Timestamp = timestamp;
        TimestampAsString = timestampAsString;
        TimestampSortable = timestampSortable;
        LocationName = locationName;
    }

    public int getID() {
        return ID;
    }

    public int getBusinessID() {
        return BusinessID;
    }

    public String getName() {
        return Name;
    }

    public String getPeriod() {
        return Period;
    }

    public String getTimestamp() {
        return Timestamp;
    }

    public String getTimestampAsString() {
        return TimestampAsString;
    }

    public String getTimestampSortable() {
        return TimestampSortable;
    }

    public String getLocationName() {
        return LocationName;
    }
}
