package com.unitethiscity.data;

/**
 * Created by Don on 2/16/2016.
 */
public class SummaryStats {

    String Name;
    String Link;
    int Today;
    int PastWeek;
    int ThisPeriod;
    int LastPeriod;
    int AllTime;

    public SummaryStats(String name, String link, int today, int pastWeek, int thisPeriod, int lastPeriod, int allTime) {
        Name = name;
        Link = link;
        Today = today;
        PastWeek = pastWeek;
        ThisPeriod = thisPeriod;
        LastPeriod = lastPeriod;
        AllTime = allTime;
    }

    public String getName() {
        return Name;
    }

    public String getLink() {
        return Link;
    }

    public int getToday() {
        return Today;
    }

    public int getPastWeek() {
        return PastWeek;
    }

    public int getThisPeriod() {
        return ThisPeriod;
    }

    public int getLastPeriod() {
        return LastPeriod;
    }

    public int getAllTime() {
        return AllTime;
    }
}
