package com.unitethiscity.data;

public class UTCCategory {
	
	private int mID;
	private String mCategoryName;
	private int mCount;
	private int mGroupID;
	private String mGroupName;
	
	public UTCCategory(int id, String catName, int count, int groupID, 
			String groupName) {
		mID = id;
		mCategoryName = catName;
		mCount = count;
		mGroupID = groupID;
		mGroupName = groupName;
	}

	public int getID() {
		return mID;
	}

	public void setID(int id) {
		mID = id;
	}

	public String getCategoryName() {
		return mCategoryName;
	}

	public void setCategoryName(String categoryName) {
		mCategoryName = categoryName;
	}

	public int getCount() {
		return mCount;
	}

	public void setCount(int count) {
		mCount = count;
	}

	public int getGroupID() {
		return mGroupID;
	}

	public void setGroupID(int groupID) {
		mGroupID = groupID;
	}

	public String getGroupName() {
		return mGroupName;
	}

	public void setGroupName(String groupName) {
		mGroupName = groupName;
	}
}
