package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class StatFavorite {

    int ID;
    int BusinessID;
    String BusinessGUID;
    String Name;
    String Timestamp;
    String TimestampAsString;
    String TimestampSortable;
    String LocationName;

    public StatFavorite(int id, int businessID, String businessGUID, String name, String timestamp, String timestampAsString, String timestampSortable, String locationName) {
        ID = id;
        BusinessID = businessID;
        BusinessGUID = businessGUID;
        Name = name;
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

    public String getBusinessGUID() {
        return BusinessGUID;
    }

    public String getName() {
        return Name;
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
