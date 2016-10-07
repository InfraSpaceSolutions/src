package com.unitethiscity.ui;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.content.Context;
import android.os.Bundle;
import android.os.Vibrator;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;

public class SearchFragment extends Fragment {

	private String mName = Constants.SEARCH_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.MAIN;
	public Constants.MenuID mMenuID = Constants.MenuID.SEARCH;

	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		mContainer = container;
		
		// Inflate the layout for this fragment
		mParent = inflater.inflate(R.layout.fragment_search, container, false);
		
		ImageButton ib = (ImageButton) mParent.findViewById(R.id.imageButtonBusiness);
		ib.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
            	
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);
    			
    			DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
    			((MainActivity) getActivity()).setFragmentID(Constants.MenuID.SEARCH_BUSINESS);
    			FragmentManager fm = getActivity().getSupportFragmentManager();
    			FragmentTransaction ft = fm.beginTransaction();
    			ft.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchBusinessFragment(),
    					Constants.MenuID.SEARCH_BUSINESS.toString());
				DataManager.getInstance().getSearchBusinessFragment().setBusinessType("Business");
    			DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_BUSINESS);
    			ft.commit();
            }
        });
        
		ib = (ImageButton) mParent.findViewById(R.id.imageButtonEvent);
		ib.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
            	
    			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
    			// Vibrate for Constants.VIBRATE_LENGTH milliseconds
    			vib.vibrate(Constants.VIBRATE_LENGTH);
    			
    			DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
    			((MainActivity) getActivity()).setFragmentID(Constants.MenuID.SEARCH_EVENT);
    			FragmentManager fm = getActivity().getSupportFragmentManager();
    			FragmentTransaction ft = fm.beginTransaction();
    			ft.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchEventFragment(),
    					Constants.MenuID.SEARCH_EVENT.toString());
				DataManager.getInstance().getSearchEventFragment().setEventType("Event");
    			DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_EVENT);
    			ft.commit();
            }
        });

		ib = (ImageButton) mParent.findViewById(R.id.imageButtonSpecials);
		ib.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {

				final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
				// Vibrate for Constants.VIBRATE_LENGTH milliseconds
				vib.vibrate(Constants.VIBRATE_LENGTH);

				DataManager.getInstance().getSearchEventFragment().fragmentActive(true);
				((MainActivity) getActivity()).setFragmentID(Constants.MenuID.SEARCH_EVENT);
				FragmentManager fm = getActivity().getSupportFragmentManager();
				FragmentTransaction ft = fm.beginTransaction();
				ft.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchEventFragment(),
						Constants.MenuID.SEARCH_EVENT.toString());
				DataManager.getInstance().getSearchEventFragment().setEventType("Special");
				DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_EVENT);
				ft.commit();
			}
		});

		ib = (ImageButton) mParent.findViewById(R.id.imageButtonEntertainers);
		ib.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {

				final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
				// Vibrate for Constants.VIBRATE_LENGTH milliseconds
				vib.vibrate(Constants.VIBRATE_LENGTH);

				DataManager.getInstance().getSearchBusinessFragment().fragmentActive(true);
				((MainActivity) getActivity()).setFragmentID(Constants.MenuID.SEARCH_BUSINESS);
				FragmentManager fm = getActivity().getSupportFragmentManager();
				FragmentTransaction ft = fm.beginTransaction();
				ft.replace(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchBusinessFragment(),
						Constants.MenuID.SEARCH_BUSINESS.toString());
				DataManager.getInstance().getSearchBusinessFragment().setBusinessType("Entertainer");
				DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_BUSINESS);
				ft.commit();
			}
		});
        
		return mParent;
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
}