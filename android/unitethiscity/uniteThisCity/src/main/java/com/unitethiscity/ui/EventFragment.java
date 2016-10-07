package com.unitethiscity.ui;

import android.annotation.TargetApi;
import android.graphics.drawable.BitmapDrawable;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.EventContextParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCEvent;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class EventFragment extends Fragment {
	
	private String mName = Constants.EVENT_FRAGMENT;
	
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.EVENT;
	private Constants.MenuID mParentID;
	
	private View mParent;
	
	private String mEventRetrievalError;
	
	private boolean mFragmentActive = false;
	
	private AsyncTask<String, Void, ImageView> mAddImageTask;
	private AsyncTask<Integer, Void, Integer> mLoadEventTask;
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_event, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        if(isAdded()) ((MainActivity) getActivity()).showSpinner();
        clearContents();
        
        String guid = getArguments().getString(EventContextParser.JSON_TAG_BUSGUID);
        Integer evtID = Integer.valueOf(getArguments().getString(EventContextParser.JSON_TAG_ID));
        
		String url = Constants.LOCATION_INFO_IMAGE + "/" + guid + "@2x.png";
		
		if(mFragmentActive) {
			mLoadEventTask = new LoadEventTask();
			if(Utils.hasHoneycomb()) {
				mLoadEventTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, evtID);
			}
			else {
				mLoadEventTask.execute(evtID);
			}

			addImage(url);
		}
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	
    	mFragmentActive = true;
    	
    	if(DataManager.getInstance().getAnalyticsState()) {
    		Logger.verbose(mName, "starting Google analytics for this screen");
    		((MainActivity) getActivity()).sendView(mName);
    	}
    }
    
    @Override
    public void onPause() {
    	super.onPause();
    	
    	cancelAllTasks();
    }
    
    public void fragmentActive(boolean activeState) {
    	Logger.verbose(mName, "fragmentActive before - " + mFragmentActive);
    	if(activeState != mFragmentActive) {
        	mFragmentActive = activeState;
        	Logger.verbose(mName, "fragmentActive after - " + mFragmentActive);
    	}
    }
    
    public void cancelAllTasks() {
		cancelAddImageTask();
		cancelLoadEventTask();
    }

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	private void addImage(String url) {
		mAddImageTask = new AddImageTask();
		if(Utils.hasHoneycomb()) {
			mAddImageTask.executeOnExecutor(AsyncTask.SERIAL_EXECUTOR, url);
		}
		else {
			mAddImageTask.execute(url);
		}
	}
    
    private class AddImageTask extends AsyncTask<String, Void, ImageView> {
        protected ImageView doInBackground(String... url) {
        	ImageView img = new ImageView(mParent.getContext());
        	
        	if(isAdded()) ((MainActivity) getActivity())
        		.getImageDownloader()
        		.download(url[0], img);
        	
        	return img;
        }
        
        protected void onPostExecute(ImageView img) {
        	if(img != null) {
        		ImageView location = (ImageView) mParent.findViewById(R.id.imageViewBusiness);
        		if(((BitmapDrawable)img.getDrawable()).getBitmap() != null) {
        			location.setImageBitmap(((BitmapDrawable)img.getDrawable()).getBitmap());
        		}
        	}
        }
    }
    
    public boolean cancelAddImageTask() {
    	boolean result = false;
    	if(mAddImageTask != null) {
    		result = mAddImageTask.cancel(true);
    	}
    	return result;
    }
    
    private class LoadEventTask extends AsyncTask<Integer, Void, Integer> {
        protected Integer doInBackground(Integer... params) {

        	if((MainActivity) getActivity() == null) {
        		Logger.verbose(mName, "main == null in LoadEVentTask");
        		mEventRetrievalError = "An error occurred when retrieving " +
						"event information";
        		return 0;
        	}
        	
        	// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadEventTask");
				mEventRetrievalError = "An error occurred when retrieving " +
						"event information";
				return 0;
			}
    		
			if(LoginManager.getInstance().userLoggedIn()) {
				mEventRetrievalError = EventContextParser.setEventContext(
						params[0], 
						LoginManager.getInstance().getAccountContext().getToken());
			}
			else {
				mEventRetrievalError = EventContextParser.setEventContext(
						params[0], null);
			}
    		
    		return null;
        }
        
        protected void onPostExecute(Integer error) {
        	
        	if(mEventRetrievalError != null) {
        		Toast.makeText(mParent.getContext(), mEventRetrievalError, Toast.LENGTH_SHORT).show();
        		return;
        	}
        	
            DataManager dm = DataManager.getInstance();
            
    		UTCEvent utcEvt = dm.getEventContext();
    		
    		TextView tv;
    		
    		tv = (TextView) mParent.findViewById(R.id.eventContextBusinessName);
    		if(utcEvt.containsKey(EventContextParser.JSON_TAG_BUSNAME)) {
    			tv.setText(utcEvt.get(EventContextParser.JSON_TAG_BUSNAME));
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.eventContextType);
    		if(utcEvt.containsKey(EventContextParser.JSON_TAG_EVENT_TYPE)) {
    			tv.setText(utcEvt.get(EventContextParser.JSON_TAG_EVENT_TYPE));
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.eventContextDate);
    		if(utcEvt.containsKey(EventContextParser.JSON_TAG_DATE_AS_STR)) {
    			tv.setText(utcEvt.get(EventContextParser.JSON_TAG_DATE_AS_STR));
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.eventContextSummary);
    		if(utcEvt.containsKey(EventContextParser.JSON_TAG_SUMMARY)) {
    			tv.setText(utcEvt.get(EventContextParser.JSON_TAG_SUMMARY));
    		}
    		
    		tv = (TextView) mParent.findViewById(R.id.eventContextBody);
    		if(utcEvt.containsKey(EventContextParser.JSON_TAG_BODY)) {
    			tv.setText(utcEvt.get(EventContextParser.JSON_TAG_BODY));
    		}
    		
            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
    
    public boolean cancelLoadEventTask() {
    	boolean result = false;
    	if(mLoadEventTask != null) {
    		Logger.info(mName, "LoadEventTask cancelled");
    		result = mLoadEventTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoadEventTask was null when cancelling");
    	}
    	return result;
    }
    
    public void clearContents() {
    	TextView tv;

    	ImageView iv = (ImageView) mParent.findViewById(R.id.imageViewBusiness);
    	iv.setImageDrawable(getResources().getDrawable(R.drawable.location_empty));
    	
    	tv = (TextView) mParent.findViewById(R.id.eventContextBusinessName);
    	tv.setText("");

    	tv = (TextView) mParent.findViewById(R.id.eventContextDate);
    	tv.setText("");

    	tv = (TextView) mParent.findViewById(R.id.eventContextType);
    	tv.setText("");

    	tv = (TextView) mParent.findViewById(R.id.eventContextSummary);
    	tv.setText("");

    	tv = (TextView) mParent.findViewById(R.id.eventContextBody);
    	tv.setText("");
    }

	public void setParent(Constants.MenuID fID) {
		mParentID = fID;
	}

	public Constants.MenuID getParent() {
		return mParentID;
	}
}