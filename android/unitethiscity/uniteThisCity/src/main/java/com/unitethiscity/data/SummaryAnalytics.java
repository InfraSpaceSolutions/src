package com.unitethiscity.data;

/**
 * Created by Don on 2/20/2016.
 */
public class SummaryAnalytics {

    private String Name;
    private int Total;
    private double Percent;
    private int Group;

    public SummaryAnalytics(String name, int total, double percent, int group) {
        Name = name;
        Total = total;
        Percent = percent;
        Group = group;
    }

    public String getName() {
        return Name;
    }

    public int getTotal() {
        return Total;
    }

    public double getPercent() {
        return Percent;
    }

    public int getGroup() {
        return Group;
    }

}
