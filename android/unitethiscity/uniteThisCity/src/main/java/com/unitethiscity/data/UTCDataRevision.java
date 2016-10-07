package com.unitethiscity.data;

import java.io.Serializable;

public class UTCDataRevision implements Serializable {

	private static final long serialVersionUID = 3784821027846733685L;
	
	/*
	 * Dataset revision ID/name map as of 6/18/2013:
	 * 
	 *  ID		Name
	 * ------------------------------------
	 *   1		LocationInfo
	 *   2		Categories
	 *   3		Deals
	 *   4		EventInfo
	 *  
	 */
	
	private Integer mDataRevisionID;
	private String mRevisionName;
	private Integer mRevision;
	private String mRevisionTimestamp;
	
    public UTCDataRevision() {
    	mDataRevisionID = Integer.valueOf(0);
    	mRevisionName = "";
    	mRevision = Integer.valueOf(0);
    	mRevisionTimestamp = "";
    }
	
    public UTCDataRevision(Integer drvID, String name, Integer rev, String ts) {
    	mDataRevisionID = drvID;
    	mRevisionName = name;
    	mRevision = rev;
    	mRevisionTimestamp = ts;
    }

	public boolean equals(Object rev) {
		UTCDataRevision revision = (UTCDataRevision) rev;
		
		if(revision == null) {
			return false;
		}
		
		if(revision.getDataRevisionID() == mDataRevisionID &&
				revision.getRevisionName().equals(mRevisionName) &&
				revision.getRevision() == mRevision &&
				revision.getRevisionTimestamp().equals(mRevisionTimestamp)) 
		{
			return true;
		}
		
		return false;
	}
    
    public Integer getDataRevisionID() {
    	return mDataRevisionID;
    }
    
    public String getRevisionName() {
    	return mRevisionName;
    }
    
    public Integer getRevision() {
    	return mRevision;
    }
    
    public String getRevisionTimestamp() {
    	return mRevisionTimestamp;
    }
    
    public boolean revisionMatches(Integer rev) {
    	return mRevision.equals(rev);
    }
}
