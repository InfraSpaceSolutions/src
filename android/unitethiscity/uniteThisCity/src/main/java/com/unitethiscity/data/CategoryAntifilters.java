package com.unitethiscity.data;

import java.io.Serializable;
import java.util.ArrayList;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class CategoryAntifilters implements Serializable {

	private static final long serialVersionUID = -8880484564035093397L;

	private String mName = Constants.CATEGORY_ANTIFILTERS;
	
	private ArrayList<Integer> mAntifilterCategoryIDs;
	private ArrayList<Integer> mAntifilterGroupIDs;
	
	public CategoryAntifilters() {
		mAntifilterCategoryIDs = new ArrayList<Integer>();
		mAntifilterGroupIDs = new ArrayList<Integer>();
	}
	
	public void addCategoryToAntifilters(Integer id) {
		Logger.verbose(mName, "ignoring category ID " + id.toString());
		mAntifilterCategoryIDs.add(id);
	}
	
	public void addGroupToAntifilters(Integer id) {
		mAntifilterGroupIDs.add(id);
	}
	
	public void removeCategoryFromAntifilters(Integer id) {
		Logger.verbose(mName, "no longer ignoring category ID " + id.toString());
		mAntifilterCategoryIDs.remove(id);
	}
	
	public void removeGroupFromAntifilters(Integer id) {
		mAntifilterGroupIDs.remove(id);
	}
	
	public boolean isCategoryAntifiltered(Integer id) {
		return mAntifilterCategoryIDs.contains(id);
	}
	
	public boolean isGroupAntifiltered(Integer id) {
		return mAntifilterGroupIDs.contains(id);
	}
	
	private void setCategoryIDs(ArrayList<Integer> ids) {
		mAntifilterCategoryIDs = ids;
	}
	
	private void setGroupIDs(ArrayList<Integer> ids) {
		mAntifilterGroupIDs = ids;
	}
	
	public int categoryAntifilterSize() {
		return mAntifilterCategoryIDs.size();
	}
	
	public int groupAntifilterSize() {
		return mAntifilterGroupIDs.size();
	}
	
	public CategoryAntifilters getCopy() {
		CategoryAntifilters ca = new CategoryAntifilters();
		
		ca.setCategoryIDs(mAntifilterCategoryIDs);
		ca.setGroupIDs(mAntifilterGroupIDs);
		
		return ca;
	}
}
