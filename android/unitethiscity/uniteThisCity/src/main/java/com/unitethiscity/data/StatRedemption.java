package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class StatRedemption {

    int ID;
    int BusinessID;
    String BusinessGUID;
    String Name;
    String Period;
    String Timestamp;
    String TimestampAsString;
    String TimestampSortable;
    String Pin;
    String Deal;
    int Amount;

    public StatRedemption(int id, int businessID, String businessGUID, String name, String period, String timestamp, String timestampAsString, String timestampSortable, String pin, String deal, int amount) {
        ID = id;
        BusinessID = businessID;
        BusinessGUID = businessGUID;
        Name = name;
        Period = period;
        Timestamp = timestamp;
        TimestampAsString = timestampAsString;
        TimestampSortable = timestampSortable;
        Pin = pin;
        Deal = deal;
        Amount = amount;
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

    public String getPin() {
        return Pin;
    }

    public String getDeal() {
        return Deal;
    }

    public int getAmount() {
        return Amount;
    }
}
