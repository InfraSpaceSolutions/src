package com.unitethiscity.data;

import java.util.HashMap;

public class UTCPushToken extends HashMap<String, String> {

	private static final long serialVersionUID = -1056516732044082953L;
	
	private Integer mID;
	private String mRegistrationID;
	
    public UTCPushToken() {
    	mID = Integer.valueOf(0);
    	mRegistrationID = "";
    }
	
    public UTCPushToken(Integer id, String registrationID) {
    	mID = id;
    	mRegistrationID = registrationID;
    }
	
	public Integer id() {
		return mID;
	}
	
	public String registrationID() {
		return mRegistrationID;
	}
}
