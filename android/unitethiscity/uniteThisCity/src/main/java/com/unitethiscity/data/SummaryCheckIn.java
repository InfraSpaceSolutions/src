package com.unitethiscity.data;

/**
 * Created by Donald on 4/6/2016.
 */
public class SummaryCheckIn {

    int AccountID;
    int BusinessID;
    String Name;
    int Count;

    public SummaryCheckIn(int accountID, int businessID, String name, int count) {
        AccountID = accountID;
        BusinessID = businessID;
        Name = name;
        Count = count;
    }

    public int getAccountID() {
        return AccountID;
    }

    public int getBusinessID() { return BusinessID; }

    public String getName() {
        return Name;
    }

    public int getCount() { return Count; }

}
