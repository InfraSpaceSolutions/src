package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class StatTip {

    int ID;
    int BusinessID;
    String BusinessGUID;
    String Name;
    String Timestamp;
    String TimestampAsString;
    String TimestampSortable;
    String TipText;
    String LocationName;

    public StatTip(int id, int businessID, String businessGUID, String name, String timestamp, String timestampAsString, String timestampSortable, String tipText, String locationName) {
        ID = id;
        BusinessID = businessID;
        BusinessGUID = businessGUID;
        Name = name;
        Timestamp = timestamp;
        TimestampAsString = timestampAsString;
        TimestampSortable = timestampSortable;
        TipText = tipText;
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

    public String getTipText() {
        return TipText;
    }

    public String getLocationName() {
        return LocationName;
    }

}
