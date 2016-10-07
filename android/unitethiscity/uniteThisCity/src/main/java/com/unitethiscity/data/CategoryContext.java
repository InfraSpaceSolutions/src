package com.unitethiscity.data;

import java.io.Serializable;
import java.util.ArrayList;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class CategoryContext implements Serializable {

	private static final long serialVersionUID = 3962640658843637433L;

	private String mName = Constants.CATEGORY_CONTEXT;
	
	private ArrayList<UTCCategory> mCategories;
	private ArrayList<Integer> mCategoryGroupIDs;
	private ArrayList<Integer> mCategoriesPerGroup;
	private ArrayList<Integer> mCategoryIDs;
	
	private CategoryAntifilters mCategoryAntifilters;
	
	public CategoryContext() {
		mCategories = new ArrayList<UTCCategory>();
		mCategoryGroupIDs = new ArrayList<Integer>();
		mCategoriesPerGroup = new ArrayList<Integer>();
		mCategoryIDs = new ArrayList<Integer>();
		mCategoryAntifilters = new CategoryAntifilters();
	}
	
	public void addCategory(UTCCategory cat) {
		Logger.verbose(mName, "adding category");
		mCategories.add(cat);
	}
	
	public void addCategory(int index, UTCCategory cat) {
		Logger.verbose(mName, "adding category");
		mCategories.add(index, cat);
	}
	
	public void replaceCategory(int index, UTCCategory cat) {
		Logger.verbose(mName, "replacing category");
		if(index < mCategories.size()) {
			mCategories.set(index, cat);
		}
	}
	
	public void removeCategory(int index) {
		Logger.verbose(mName, "adding category");
		if(index < mCategories.size()) {
			mCategories.remove(index);
		}
	}
	
	public ArrayList<UTCCategory> getCategories() {
		return mCategories;
	}
	
	public void clearCategories() {
		Logger.verbose(mName, "clearing categories");
		mCategories.clear();
	}
	
	public int categoryExists(String name) {
		// return index is it exists, otherwise return -1
		for(int i = 0; i < mCategories.size(); i++) {
			if(mCategories.get(i).getCategoryName().compareTo(name) == 0) {
				return i;
			}
		}
		return -1;
	}
	
	public boolean matchesCount(String name, int count) {
		for(int i = 0; i < mCategories.size(); i++) {
			if(mCategories.get(i).getCategoryName().compareTo(name) == 0) {
				if(mCategories.get(i).getCount() == count) {
					return true;
				}
				else {
					return false;
				}
			}
		}
		return false;
	}

	public void addToCategoryGroupIDs(Integer id) {
		mCategoryGroupIDs.add(id);
	}
	
	public Integer getFromCategoryGroupIDs(int index) {
		return mCategoryGroupIDs.get(index);
	}
	
	public int getCategoryGroupIDsSize() {
		return mCategoryGroupIDs.size();
	}
	
	public void clearCategoryGroupIDs() {
		mCategoryGroupIDs.clear();
	}
	
	public void addToCategoriesPerGroup(Integer id) {
		mCategoriesPerGroup.add(id);
	}
	
	public Integer getFromCategoriesPerGroup(int index) {
		return mCategoriesPerGroup.get(index);
	}
	
	public void clearCategoriesPerGroup() {
		mCategoriesPerGroup.clear();
	}
	
	public int getCategoriesPerGroupSize() {
		return mCategoriesPerGroup.size();
	}
	
	public void addToCategoryIDs(Integer id) {
		mCategoryIDs.add(id);
	}
	
	public Integer getFromCategoryIDs(int index) {
		return mCategoryIDs.get(index);
	}
	
	public int getCategoryIDsSize() {
		return mCategoryIDs.size();
	}
	
	public void clearCategoryIDs() {
		mCategoryIDs.clear();
	}
	
	public CategoryAntifilters getAntifilters() {
		return mCategoryAntifilters;
	}
	
	public void setAntifilters(CategoryAntifilters ca) {
		mCategoryAntifilters = ca;
	}

}
