package com.unitethiscity.data;

public class UTCMessage {
	
    private int mID;
    private int mMessageID;
    private int mToAccountID;
    private boolean mInboxRead;
    private int mFromAccountID;
    private String mFromName;
    private String mSummary;
    private String mBody;
    private String mMessageTimestamp;
    private String mMessageTimestampAsString;
    private String mMessageExpires;
    private String mMessageExpiresAsString;
	private String mBusinessGuid;
	
	public UTCMessage(int id, int msgID, int toAccID, boolean read, int fromAccID,
			String fromName, String summary, String body, String msgTs,
			String msgTsAsStr, String msgExpire, String msgExpireAsStr, String busGuid) {
	    mID = id;
	    mMessageID = msgID;
	    mToAccountID = toAccID;
	    mInboxRead = read;
	    mFromAccountID = fromAccID;
	    mFromName = fromName;
	    mSummary = summary;
	    mBody = body;
	    mMessageTimestamp = msgTs;
	    mMessageTimestampAsString = msgTsAsStr;
	    mMessageExpires = msgExpire;
	    mMessageExpiresAsString = msgExpireAsStr;
		mBusinessGuid = busGuid;
	}
	
	public boolean equals(Object msg) {
		UTCMessage message = (UTCMessage) msg;
		
		if(message == null) {
			return false;
		}
		
		if(message.getID() == mID &&
				message.getMessageID() == mMessageID &&
				message.getToAccountID() == mToAccountID &&
				message.getInboxRead() == mInboxRead &&
				message.getFromAccountID() == mFromAccountID &&
				message.getFromName().equals(mFromName) &&
				message.getSummary().equals(mSummary) &&
				message.getBody().equals(mBody) &&
				message.getMessageTimestamp().equals(mMessageTimestamp) &&
				message.getMessageTimestampAsString().equals(mMessageTimestampAsString) &&
				message.getMessageTimestamp().equals(mMessageExpires) &&
				message.getMessageTimestampAsString().equals(mMessageExpiresAsString) &&
				message.getBusinessGuid().equals(mBusinessGuid))
		{
			return true;
		}
		
		return false;
	}

	public int getID() {
		return mID;
	}

	public void setID(int id) {
		mID = id;
	}

	public int getMessageID() {
		return mMessageID;
	}
	
	public void setMessageID(int id) {
		mMessageID = id;
	}
	
	public int getToAccountID() {
		return mToAccountID;
	}
	
	public void setToAccountID(int id) {
		mToAccountID = id;
	}
	
	public boolean getInboxRead() {
		return mInboxRead;
	}
	
	public void setInboxRead(boolean read) {
		mInboxRead = read;
	}
	
	public int getFromAccountID() {
		return mFromAccountID;
	}
	
	public void setFromAccountID(int id) {
		mFromAccountID = id;
	}
	
	public String getFromName() {
		return mFromName;
	}
	
	public void setFromName(String name) {
		mFromName = name;
	}
	
	public String getSummary() {
		return mSummary;
	}
	
	public void setSummary(String summary) {
		mSummary = summary;
	}
	
	public String getBody() {
		return mBody;
	}
	
	public void setBody(String body) {
		mBody = body;
	}
	
	public String getMessageTimestamp() {
		return mMessageTimestamp;
	}
	
	public void setMessageTimestamp(String ts) {
		mMessageTimestamp = ts;
	}
	
	public String getMessageTimestampAsString() {
		return mMessageTimestampAsString;
	}
	
	public void setMessageTimestampAsString(String ts) {
		mMessageTimestampAsString = ts;
	}
	
	public String getMessageExpires() {
		return mMessageExpires;
	}
	
	public void setMessageExpires(String ts) {
		mMessageExpires = ts;
	}
	
	public String getMessageExpiresAsString() {
		return mMessageExpiresAsString;
	}
	
	public void setMessageExpiresAsString(String ts) {
		mMessageExpiresAsString = ts;
	}

	public String getBusinessGuid() { return mBusinessGuid; }

	public void setBusinessGuid(String busGuid) { mBusinessGuid = busGuid; }
}
