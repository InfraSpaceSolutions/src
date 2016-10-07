package com.unitethiscity.data;

import java.util.HashMap;

public class UTCLocation extends HashMap<String, String> {

	private static final long serialVersionUID = 4992131103719302335L;
	private Integer mID;
	
    public UTCLocation() {
    	mID = Integer.valueOf(0);
    }
	
    public UTCLocation(Integer id) {
    	mID = id;
    }
	
	public Integer id() {
		return mID;
	}
}
