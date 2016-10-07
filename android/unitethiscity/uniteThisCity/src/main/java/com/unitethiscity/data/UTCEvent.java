package com.unitethiscity.data;

import java.util.HashMap;

public class UTCEvent extends HashMap<String, String> {

	private static final long serialVersionUID = -8337657043145550984L;
	
	private Integer mID;
	
    public UTCEvent() {
    	mID = Integer.valueOf(0);
    }
	
    public UTCEvent(Integer id) {
    	mID = id;
    }
	
	public Integer id() {
		return mID;
	}
}