package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class StatRating {

    int ID;
    int BusinessID;
    String BusinessGUID;
    String Name;
    String Timestamp;
    String TimestampAsString;
    String TimestampSortable;
    int Rating;
    String LocationName;

    public StatRating(int id, int businessID, String businessGUID, String name, String timestamp, String timestampAsString, String timestampSortable, int rating, String locationName) {
        ID = id;
        BusinessID = businessID;
        BusinessGUID = businessGUID;
        Name = name;
        Timestamp = timestamp;
        TimestampAsString = timestampAsString;
        TimestampSortable = timestampSortable;
        Rating = rating;
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

    public int getRating() {
        return Rating;
    }

    public String getLocationName() {
        return LocationName;
    }
}
