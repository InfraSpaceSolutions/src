package com.unitethiscity.data;

/**
 * Created by Donald on 4/6/2016.
 */
public class SummaryRedemption {

    int AccountID;
    int BusinessID;
    String Name;
    int Count;
    double Sum;

    public SummaryRedemption(int accountID, int businessID, String name, int count, double sum) {
        AccountID = accountID;
        BusinessID = businessID;
        Name = name;
        Count = count;
        Sum = sum;
    }

    public int getAccountID() { return AccountID; }

    public int getBusinessID() {
        return BusinessID;
    }

    public String getName() { return Name; }

    public int getCount() {
        return Count;
    }

    public double getSum() { return Sum; }

}
