package com.unitethiscity.ui;

import java.io.IOException;
import java.util.ArrayList;

import org.json.JSONException;

import com.unitethiscity.R;
import com.unitethiscity.data.CategoriesParser;
import com.unitethiscity.data.CategoryAntifilters;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.UTCCategory;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.support.v4.app.Fragment;
import android.content.Context;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

public class SearchCategoriesFragment extends Fragment {

	private String mName = Constants.SEARCH_CATEGORIES_FRAGMENT;
	
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.SEARCH_CATEGORIES;
	private Constants.MenuID mParentID;

	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;
	
	private AsyncTask<Void, Void, Boolean> mCategoriesTask;

	public String mLastGroup;
	public ArrayList<Integer> mGroupIDs;

	private int mIdNum;
	private int mIdOffset;
	
	private String mStringReply;
	
	public boolean mCategoryStateChanged;
	
	private boolean mApplyButtonHit;
	
	private boolean mSaveExisted;
	
	private UTCDialogFragment mUTCDialog;

    @Override
    public void onCreate(Bundle savedInstanceState) {
    	super.onCreate(savedInstanceState);
    	Logger.verbose(mName, "onCreate()");
    }
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
    	Logger.verbose(mName, "onCreateView()");
    	
    	mContainer = container;
    	
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_search_categories, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");
		
    	((MainActivity) getActivity()).showSpinner();
    	
    	mSaveExisted = false;
    	
        // check if category antifilters have been previously saved, and attempt to 
        // deserialize it into our category context within the data manager
        if(Utils.saveExists((MainActivity) getActivity(), Constants.CATEGORY_ANTIFILTERS)) {
        	mSaveExisted = true;
        	try {
				Object obj = Utils.open((MainActivity) getActivity(), Constants.CATEGORY_ANTIFILTERS);
				DataManager.getInstance().setCategoryAntifilters(
						(CategoryAntifilters) obj);
			} catch (IOException e) {
				e.printStackTrace();
			} catch (ClassNotFoundException e) {
				e.printStackTrace();
			}
        }
    	
    	mCategoryStateChanged = false;
    	
    	mApplyButtonHit = false;
    	
    	mGroupIDs = new ArrayList<Integer>();
    	executeCategoriesTask();
    	
        final Button selectAll = (Button) mParent.findViewById(R.id.buttonSelectAll);
        selectAll.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				selectAll(true);
			}
		});
        
        final Button selectNone = (Button) mParent.findViewById(R.id.buttonSelectNone);
        selectNone.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				selectNone(true);
			}
		});
    	
        final Button apply = (Button) mParent.findViewById(R.id.buttonApply);
        apply.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				// check if there have been any categories selected
				boolean noneSelected = DataManager.getInstance().getCategoryContext().getCategoryIDsSize() ==
						DataManager.getInstance().getCategoryAntifilters().categoryAntifilterSize();
				
				// check apply button hit to prevent multiple user button presses from executing 
				// multiple calls to removing the search categories fragment
				if(!mApplyButtonHit) {
					Logger.info(mName, "applying category filters");

					if(noneSelected) {
						//Toast.makeText(getActivity(), "No categories have been selected.", Toast.LENGTH_SHORT).show();
						showWarningDialog();
					}
					
					saveCategoryFilterSettings();

					mApplyButtonHit = true;
					if(mParentID == Constants.MenuID.SEARCH_BUSINESS) {
						DataManager.getInstance().getSearchBusinessFragment().removeSearchCategories();
					}
					else if(mParentID == Constants.MenuID.SEARCH_EVENT) {
						DataManager.getInstance().getSearchEventFragment().removeSearchCategories();
					}
				}
			}
		});
    }
    
    @Override
    public void onPause() {
    	super.onPause();
    	Logger.verbose(mName, "onPause()");
    	
    	if(!mCategoryStateChanged && 
    			!Utils.saveExists((MainActivity) getActivity(), Constants.CATEGORY_ANTIFILTERS)) {
    		DataManager.getInstance().setCategoryAntifilters(new CategoryAntifilters());
    	}
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");
    	
        if(DataManager.getInstance().getAnalyticsState()) {
        	Logger.verbose(mName, "starting Google analytics for this screen");
        	((MainActivity) getActivity()).sendView(mName);
        }
    }
    
    public void fragmentActive(boolean activeState) {
    	Logger.verbose(mName, "fragmentActive before - " + mFragmentActive);
    	if(activeState != mFragmentActive) {
        	mFragmentActive = activeState;
        	Logger.verbose(mName, "fragmentActive after - " + mFragmentActive);
    	}
    }
    
    public void hide() {
    	if(mContainer != null) {
            for(int i = 0; i < mContainer.getChildCount(); i++) {
                View v = mContainer.getChildAt(i);
                v.setVisibility(View.GONE);
            }
    	}
    }
    
    public void show() {
    	if(mContainer != null) {
            for(int i = 0; i < mContainer.getChildCount(); i++) {
                View v = mContainer.getChildAt(i);
                v.setVisibility(View.VISIBLE);
            }
    	}
    }
    
    public void setParent(Constants.MenuID fID) {
    	mParentID = fID;
    }
    
    public Constants.MenuID getParent() {
    	return mParentID;
    }
    
	public void executeCategoriesTask() {
    	mCategoriesTask = new CategoriesTask();
		if(Utils.hasHoneycomb()) {
			mCategoriesTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
		}
		else {
			mCategoriesTask.execute();
		}
	}
	
    private class CategoriesTask extends AsyncTask<Void, Void, Boolean> {
        protected Boolean doInBackground(Void... params) {
    		mStringReply = null;
    		
    		try {
    			mStringReply = CategoriesParser.setCategories();
    		} catch (JSONException e) {
    			e.printStackTrace();
    		}

    		if(mStringReply != null) {
    			return Boolean.valueOf(false);
    		}

    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
        	if(!success.booleanValue() && mStringReply != null &&
        			((MainActivity) getActivity()) != null) {
        		Toast.makeText((MainActivity) getActivity(), mStringReply, Toast.LENGTH_SHORT).show();
        	}
        	else {
        		if(mFragmentActive) {
            		final DataManager dm = DataManager.getInstance();
            		final ArrayList<UTCCategory> categories = dm.getCategoryContext().getCategories();

            		int categoryOrderNumber = 0;
            		int categoriesPerGroup = 0;
            		
            		LinearLayout search = (LinearLayout) mParent.findViewById(R.id.linearLayoutSearchCategories);
            		
            		mIdNum = Constants.BASE_CUSTOM_IDS;
            		mIdOffset = -1000;
            		
            		dm.getCategoryContext().clearCategoriesPerGroup();
            		dm.getCategoryContext().clearCategoryGroupIDs();
            		dm.getCategoryContext().clearCategoryIDs();
            		
            		if(categories != null) {
            			String lastGroup = "";

            			LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);

            			boolean group = false;
            			View child = null;
            			TextView tv;
            			for(int i = 0; i < categories.size();) {
            				final UTCCategory cat = (UTCCategory) categories.get(i);
            				final int index = i;

            				if(cat.getGroupName().compareToIgnoreCase(lastGroup) != 0) {

            					if(i != 0) {
            						dm.getCategoryContext().addToCategoriesPerGroup(Integer.valueOf(categoriesPerGroup));
            					}
            					categoriesPerGroup = 0;

            					// set group layout
            					try {
            						child = inflater.inflate(R.layout.category_group, null, false);
            					}
            					catch(InflateException ie) {
            						Logger.error(mName, "could not inflate child view");
            						throw new RuntimeException(ie);
            					}

            					mLastGroup = lastGroup;
            					mGroupIDs.add(cat.getGroupID());

            					tv = (TextView) child.findViewById(R.id.groupName);
            					lastGroup = cat.getGroupName();
            					tv.setText(lastGroup);
            					
            					group = true;
            				}
            				else {
            					categoriesPerGroup++;
            					// set category layout
            					try {
            						child = inflater.inflate(R.layout.category, null, false);
            					}
            					catch(InflateException ie) {
            						Logger.error(mName, "could not inflate child view");
            						throw new RuntimeException(ie);
            					}
            					
            					final ImageView iv = (ImageView) child.findViewById(R.id.imageViewCategoryCheck);
            					if(!dm.isCategoryAntifiltered(cat.getID())) { // !
            						iv.setImageResource(R.drawable.check_on);
            					}

            					Logger.verbose(mName, cat.getCategoryName() + " " + 
            							String.valueOf(dm.isCategoryAntifiltered(cat.getID())));
            					
            					child.setOnClickListener(new OnClickListener() {
            						@Override
            						public void onClick(View v)
            						{
            							if(!dm.isCategoryAntifiltered(cat.getID())) { // !
            								((ImageView) v.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_off);
            								dm.addCategoryToAntifilters(cat.getID());
            								dm.needLocalUpdate();
            							}
            							else {
            								((ImageView) v.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_on);
            								dm.removeCategoryFromAntifilters(cat.getID());
            								dm.needLocalUpdate();
            							}
										updateCategoryGroups();
            							DataManager.getInstance().getCategoryContext().replaceCategory(index, cat);
            							mCategoryStateChanged = true;
            						}
            					});

            					tv = (TextView) child.findViewById(R.id.categoryName);
            					tv.setText(cat.getCategoryName());
            					tv = (TextView) child.findViewById(R.id.categoryOrderNumber);
            					tv.setText(String.valueOf(categoryOrderNumber++));
            					
            					i++;
            					
            					group = false;
            				}

            				if(group) {
            					dm.getCategoryContext().addToCategoryGroupIDs(Integer.valueOf(mIdNum + mIdOffset));
            				}
            				else {
            					dm.getCategoryContext().addToCategoryIDs(Integer.valueOf(mIdNum + mIdOffset));
            				}
            				child.setId(mIdNum + mIdOffset++);
            				search.addView(child);
            			}
            			
            			dm.getCategoryContext().addToCategoriesPerGroup(Integer.valueOf(categoriesPerGroup));
            			
            			for(int i = 0; i < dm.getCategoryContext().getCategoryGroupIDsSize(); i++) {
            				View groupChild = search.findViewById(dm.getCategoryContext().getFromCategoryGroupIDs(i));

							if(!dm.isGroupAntifiltered(mGroupIDs.get(i))) {
								((ImageView) groupChild.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow_highlight);
							}

            				final int groupIndex = i;
            				groupChild.setOnClickListener(new OnClickListener() {

            					@Override
            					public void onClick(View v) 
            					{
            						DataManager dm = DataManager.getInstance();
            						
            						int firstID = dm.getCategoryContext().getFromCategoryGroupIDs(groupIndex) + 1;
            						int lastID = firstID + dm.getCategoryContext().getFromCategoriesPerGroup(groupIndex);
            						
            						if(dm.isGroupAntifiltered(mGroupIDs.get(groupIndex))) {
            							dm.removeGroupFromAntifilters(mGroupIDs.get(groupIndex));
										((ImageView) v.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow_highlight);
            						}
            						else {
            							dm.addGroupToAntifilters(mGroupIDs.get(groupIndex));
										((ImageView) v.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow);
            						}
            						
            						for(int i = firstID; i < lastID; i++) {
            							View categoryChild = ((LinearLayout) mParent.
            									findViewById(R.id.linearLayoutSearchCategories)).
            									findViewById(i);
            							
            							TextView tv = (TextView) categoryChild.findViewById(R.id.categoryOrderNumber);
            							int categoryIndex = Integer.valueOf(String.valueOf(tv.getText()));
            							UTCCategory cat = categories.get(categoryIndex);
            							
            							if(dm.isGroupAntifiltered(mGroupIDs.get(groupIndex))) {
            								((ImageView) categoryChild.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_off);
            								dm.addCategoryToAntifilters(cat.getID());
            								dm.needLocalUpdate();
            							}
            							else {
            								((ImageView) categoryChild.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_on);
            								categoryChild.findViewById(R.id.relativeLayoutCategory).setBackgroundColor(Color.WHITE);
            								dm.removeCategoryFromAntifilters(cat.getID());
            								dm.needLocalUpdate();
            							}
            						}
            						
            						mCategoryStateChanged = true;
            					}
            				});
            			}
            		}
            		
            		if(!mSaveExisted) {
            			selectNone(false);
            		}
            		
            		if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        		}
        	}
        }
    }

	private void updateCategoryGroups() {
		DataManager dm = DataManager.getInstance();

		ArrayList<UTCCategory> categories = dm.getCategoryContext().getCategories();

		for(int i = 0; i < dm.getCategoryContext().getCategoryGroupIDsSize(); i++) {
			int firstID = dm.getCategoryContext().getFromCategoryGroupIDs(i) + 1;
			int lastID = firstID + dm.getCategoryContext().getFromCategoriesPerGroup(i);
			boolean allOn = true;

			for(int j = firstID; j < lastID; j++) {
				View categoryChild = ((LinearLayout) mParent.
						findViewById(R.id.linearLayoutSearchCategories)).
						findViewById(j);

				TextView tv = (TextView) categoryChild.findViewById(R.id.categoryOrderNumber);
				int categoryIndex = Integer.valueOf(String.valueOf(tv.getText()));
				UTCCategory cat = categories.get(categoryIndex);
				allOn = allOn && !dm.isCategoryAntifiltered(cat.getID());
			}

			View groupChild = ((LinearLayout) mParent.
					findViewById(R.id.linearLayoutSearchCategories)).
					findViewById(firstID - 1);
			if(allOn) {
				dm.removeGroupFromAntifilters(mGroupIDs.get(i));
				((ImageView) groupChild.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow_highlight);
			}
			else {
				dm.addGroupToAntifilters(mGroupIDs.get(i));
				((ImageView) groupChild.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow);
			}
		}
	}
    
    public boolean cancelCategoriesTask() {
    	boolean result = false;
    	if(mCategoriesTask != null) {
    		Logger.info(mName, "CategoriesTask cancelled");
    		result = mCategoriesTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "CategoriesTask was null when cancelling");
    	}
    	return result;
    }
    
    private void selectAll(boolean stateChange) {
		DataManager dm = DataManager.getInstance();

		ArrayList<UTCCategory> categories = dm.getCategoryContext().getCategories();

		for(int i = 0; i < dm.getCategoryContext().getCategoryGroupIDsSize(); i++) {
			int firstID = dm.getCategoryContext().getFromCategoryGroupIDs(i) + 1;
			int lastID = firstID + dm.getCategoryContext().getFromCategoriesPerGroup(i);

			dm.removeGroupFromAntifilters(mGroupIDs.get(i));

			for(int j = firstID; j < lastID; j++) {
				View categoryChild = ((LinearLayout) mParent.
						findViewById(R.id.linearLayoutSearchCategories)).
						findViewById(j);

				TextView tv = (TextView) categoryChild.findViewById(R.id.categoryOrderNumber);
				int categoryIndex = Integer.valueOf(String.valueOf(tv.getText()));
				UTCCategory cat = categories.get(categoryIndex);

				((ImageView) categoryChild.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_on);
				dm.removeCategoryFromAntifilters(cat.getID());
				dm.needLocalUpdate();
			}

			View groupChild = ((LinearLayout) mParent.
					findViewById(R.id.linearLayoutSearchCategories)).
					findViewById(firstID - 1);

			dm.removeGroupFromAntifilters(mGroupIDs.get(i));
			((ImageView) groupChild.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow_highlight);
		}
		
		mCategoryStateChanged = stateChange;
    }
    
    private void selectNone(boolean stateChange) {
		DataManager dm = DataManager.getInstance();

		ArrayList<UTCCategory> categories = dm.getCategoryContext().getCategories();

		for(int i = 0; i < dm.getCategoryContext().getCategoryGroupIDsSize(); i++) {
			int firstID = dm.getCategoryContext().getFromCategoryGroupIDs(i) + 1;
			int lastID = firstID + dm.getCategoryContext().getFromCategoriesPerGroup(i);

			dm.addGroupToAntifilters(mGroupIDs.get(i));

			for(int j = firstID; j < lastID; j++) {
				View categoryChild = ((LinearLayout) mParent.
						findViewById(R.id.linearLayoutSearchCategories)).
						findViewById(j);

				TextView tv = (TextView) categoryChild.findViewById(R.id.categoryOrderNumber);
				int categoryIndex = Integer.valueOf(String.valueOf(tv.getText()));
				UTCCategory cat = categories.get(categoryIndex);

				((ImageView) categoryChild.findViewById(R.id.imageViewCategoryCheck)).setImageResource(R.drawable.check_off);
				dm.addCategoryToAntifilters(cat.getID());
				dm.needLocalUpdate();
			}

			View groupChild = ((LinearLayout) mParent.
					findViewById(R.id.linearLayoutSearchCategories)).
					findViewById(firstID - 1);

			dm.addGroupToAntifilters(mGroupIDs.get(i));
			((ImageView) groupChild.findViewById(R.id.imageViewSelectArrow)).setImageResource(R.drawable.search_select_arrow);
		}
		
		mCategoryStateChanged = stateChange;
    }
    
    public void saveCategoryFilterSettings() {
    	if(mCategoryStateChanged || 
    			!Utils.saveExists((MainActivity) getActivity(), Constants.CATEGORY_ANTIFILTERS)) {
    		mCategoryStateChanged = false;
    		try {
    			Utils.save((MainActivity) getActivity(), 
    					Constants.CATEGORY_ANTIFILTERS, 
    					DataManager.getInstance().getCategoryAntifilters());
    		} catch (IOException e) {
    			e.printStackTrace();
    		}
    	}
    }
    
    private void showWarningDialog() {
        // Create and show the dialog
    	mUTCDialog = UTCDialogFragment.newInstance("WarningDialog",
    			"", "No Categories Selected");
    	mUTCDialog.show(getFragmentManager(), "WarningDialog");
    }
}