package com.unitethiscity.ui;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.EventContextParser;
import com.unitethiscity.data.EventsParser;
import com.unitethiscity.data.UTCEvent;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.app.DatePickerDialog;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.text.Editable;
import android.text.TextWatcher;
import android.annotation.TargetApi;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.Vibrator;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

public class SearchEventFragment extends Fragment {

	private String mName = Constants.SEARCH_EVENT_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.SEARCH_EVENT;

	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;
	private String mEventRetrievalError = null;
	
	private int mEventProgress;
	
	private String mSearchFilter = "";
	private String mDateFilter = "";

	private String mEventType = "Event";
	
	private AsyncTask<Void, View, Integer> mLoadEventsTask;

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
    	mParent = inflater.inflate(R.layout.fragment_search_event, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");
        
        EditText et = (EditText) mParent.findViewById(R.id.editTextEventSearchFilter);
        et.addTextChangedListener(new TextWatcher() {

			@Override
			public void afterTextChanged(Editable s) {
				
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start,
					int count, int after) {
				
			}

			@Override
			public void onTextChanged(CharSequence s, int start,
					int before, int count) {
				mSearchFilter = s.toString();
			}
        	
        });

		View.OnClickListener categoryClick = new View.OnClickListener() {

			@Override
			public void onClick(View v) {
				hide();
				DataManager.getInstance().getSearchCategoriesFragment().fragmentActive(true);
				DataManager.getInstance().getSearchCategoriesFragment().setParent(mMenuID);
				FragmentManager fm = getActivity().getSupportFragmentManager();
				FragmentTransaction ft = fm.beginTransaction();
				ft.add(R.id.frameLayoutMiddle, DataManager.getInstance().getSearchCategoriesFragment(),
						Constants.MenuID.SEARCH_CATEGORIES.toString());
				DataManager.getInstance().pushToMenuStack(Constants.MenuID.SEARCH_CATEGORIES);
				ft.commit();
			}
		};
        Button b = (Button) mParent.findViewById(R.id.buttonSearchEventKeywordCategory);
        b.setOnClickListener(categoryClick);
		b = (Button) mParent.findViewById(R.id.buttonSearchEventCategory);
		b.setOnClickListener(categoryClick);
		b = (Button) mParent.findViewById(R.id.buttonSearchEventKeyword);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				RelativeLayout results = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchEventResults);
				results.setVisibility(RelativeLayout.GONE);
				RelativeLayout keyword = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchEventKeyword);
				keyword.setVisibility(RelativeLayout.VISIBLE);
			}
		});
		b = (Button) mParent.findViewById(R.id.buttonSearchEventDateSelect);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View arg0) {
				DatePickerDialog.OnDateSetListener thisListener =
						new DatePickerDialog.OnDateSetListener() {

							@Override
							public void onDateSet(DatePicker view, int year,
												  int monthOfYear, int dayOfMonth) {
								mDateFilter = String.format("%02d/%02d/%04d",
										monthOfYear + 1, dayOfMonth, year);
								EditText et = (EditText) mParent.findViewById(R.id.editTextEventSearchDate);
								et.setText(mDateFilter);
							}
						};

				final Calendar c = Calendar.getInstance();
				DatePickerDialog dpd = new DatePickerDialog(mParent.getContext(),
						thisListener, c.get(Calendar.YEAR), c.get(Calendar.MONTH),
						c.get(Calendar.DAY_OF_MONTH));
				dpd.show();
			}
		});
		b = (Button) mParent.findViewById(R.id.buttonSearchEventKeywordGo);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				cancelLoadEventsTask();

				RelativeLayout keyword = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchEventKeyword);
				keyword.setVisibility(RelativeLayout.GONE);
				RelativeLayout results = (RelativeLayout) mContainer.findViewById(R.id.relativeLayoutSearchEventResults);
				results.setVisibility(RelativeLayout.VISIBLE);

				removeAllEventResults();
				loadEvents();
			}
		});
		b = (Button) mParent.findViewById(R.id.buttonSearchEventKeywordCancel);
		b.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				((MainActivity) getActivity()).onBackPressed();
			}
		});

		TextView crumb = (TextView) mParent.findViewById(R.id.crumbSearchEvent);
		TextView title = (TextView) mParent.findViewById(R.id.textViewSearchEventKeywordTitle);
		crumb.setText(mEventType.toUpperCase() + "S");
		title.setText("SEARCH " + mEventType.toUpperCase() + "S");
        
    	if(mFragmentActive == true) {
            ((MainActivity) getActivity()).clearBackPressed();
            loadEvents();
    	}
    	Logger.verbose(mName, "onActivityCreated fragmentActive - " + mFragmentActive);
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");

		// clear search filter
		EditText et = (EditText) mParent.findViewById(R.id.editTextEventSearchFilter);
		et.setText("");
		mSearchFilter = "";
		final Calendar c = Calendar.getInstance();
		mDateFilter = String.format("%02d/%02d/%04d", c.get(Calendar.MONTH) + 1,
				c.get(Calendar.DAY_OF_MONTH), c.get(Calendar.YEAR));
		et = (EditText) mParent.findViewById(R.id.editTextEventSearchDate);
		et.setText(mDateFilter);

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

	public void setEventType(String type) {
		mEventType = type;
		if (mParent != null) {
			TextView crumb = (TextView) mParent.findViewById(R.id.crumbSearchEvent);
			TextView title = (TextView) mParent.findViewById(R.id.textViewSearchEventKeywordTitle);
			crumb.setText(mEventType.toUpperCase() + "S");
			title.setText("SEARCH " + mEventType.toUpperCase() + "S");
		}
	}
    
    public void removeSearchCategories() {
		FragmentManager fm = getActivity().getSupportFragmentManager();
		FragmentTransaction transaction = fm.beginTransaction();
		transaction.remove(DataManager.getInstance().getSearchCategoriesFragment()).commit();
		fragmentActive(true);
	    removeAllEventResults();
	    loadEvents();
	    show();
		RelativeLayout keyword = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSearchEventKeyword);
		keyword.setVisibility(RelativeLayout.GONE);
		RelativeLayout results = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutSearchEventResults);
		results.setVisibility(RelativeLayout.VISIBLE);
	    DataManager.getInstance().popFromMenuStack();
    }
    
	public void replaceSubmenuFragment(Constants.MenuID fID, Bundle args, boolean goBack)
	{
		if(fID != Constants.MenuID.HOME) {
			final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
			// Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
			vib.vibrate(Constants.VIBRATE_LENGTH);
		}
		
		FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

		mFragmentActive = false;
		hide();
		
		// Replace whatever is in the frameLayout view with this fragment
	    switch (fID) {
			case EVENT:
				((MainActivity) getActivity()).setFragmentID(fID);
				EventFragment ef = DataManager.getInstance().getEventFragment();
				ef.fragmentActive(true);
				ef.setParent(mMenuID);
				if(args.isEmpty() == false) {
					ef.setArguments(args);
				}
				transaction.add(R.id.frameLayoutMiddle, ef, fID.toString());
				break;
			default:
				break;
	    }
	    // add transaction to back stack if we want to go back to where we were
	    if(goBack) {
	    	DataManager.getInstance().pushToMenuStack(fID);
	    }
	    
		// Commit the transaction
		transaction.commit();
	}
	
	public void cancelAllTasks() {
		cancelLoadEventsTask();
	}

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	public void loadEvents() {
		if(isAdded()) ((MainActivity) getActivity()).showSpinner();
        mLoadEventsTask = new LoadEventsTask();
        // task object could be corrupted or canceled
        // when switching menus
        if(mLoadEventsTask != null) {
            if(Utils.hasHoneycomb()) {
            	mLoadEventsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
            }
            else {
            	mLoadEventsTask.execute();
            }
        }
	}
	
    private class LoadEventsTask extends AsyncTask<Void, View, Integer> {
        protected Integer doInBackground(Void... params) {
            DataManager dm = DataManager.getInstance();

            int eventNum = 0;

        	if((MainActivity) getActivity() == null) {
        		Logger.verbose(mName, "main == null in LoadEventsTask");
        		return 0;
        	}
        	
        	// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadEventsTask");
				return 0;
			}
            
    		/////////////////////////////////////////////////////////////////////////
    		//////////   Retrieve event data   //////////////////////////////////////
    		/////////////////////////////////////////////////////////////////////////
			boolean sort = dm.doEventsNeedUpdated();
			mEventRetrievalError = EventsParser.setEvents();
			if(sort && mEventRetrievalError == null) {
				dm.sortEventsByDate();
			}
    		
    		/////////////////////////////////////////////////////////////////////////
    		//////////   Create event layouts   /////////////////////////////////////
    		/////////////////////////////////////////////////////////////////////////
    		ArrayList<Integer> listID = dm.getEventIDs();

            TextView tv;
            
    		int idNum = Constants.BASE_CUSTOM_IDS;
    		int idOffset = 10000;

			int backgroundIndex = 1;
    		
    		Logger.verbose(mName, "number of event IDs - " + String.valueOf(listID.size()));
    		LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    		for(int i = 1; i <= listID.size(); i++) {
    			Logger.verbose(mName, String.valueOf(i) + " adding location ID " + listID.get(i - 1));

    			// keep checking if this task was canceled and bail if so
    			if(isCancelled() == true) {
    				Logger.verbose(mName, "isCancelled == true in LoadEventsTask (loop)");
    				return 0;
    			}

    			UTCEvent utcEvt = dm.getEvent(listID.get(i - 1));
    			
    			// check if location should be added based on category filter
    			if(utcEvt.containsKey(EventsParser.JSON_TAG_ID)) {
    				
    				String allEventDetails = "";
    				
    				String name = "";
    				String summary = "";
    				String category = "";
    				String eventType = "";

	    			if(utcEvt.containsKey(EventsParser.JSON_TAG_BUSNAME)) {
	    				name = utcEvt.get(EventsParser.JSON_TAG_BUSNAME);
	    				allEventDetails = allEventDetails + name;
	    			}
	    			
	    			if(utcEvt.containsKey(EventsParser.JSON_TAG_SUMMARY)) {
	    				summary = utcEvt.get(EventsParser.JSON_TAG_SUMMARY);
	    				allEventDetails = allEventDetails + summary;
	    			}
	    			
	    			if(utcEvt.containsKey(EventsParser.JSON_TAG_CATNAME)) {
	    				category = utcEvt.get(EventsParser.JSON_TAG_CATNAME);
	    				if(utcEvt.containsKey(EventsParser.JSON_TAG_PROPERTIES + "Size")) {
	    		            int propertiesNum = Integer.valueOf(utcEvt.get(EventsParser.JSON_TAG_PROPERTIES + "Size")).intValue();
	    		            for(int j = 0; j < propertiesNum; j++) {
	    		            	category = category + ", " + utcEvt.get(EventsParser.JSON_TAG_PROPERTIES + String.valueOf(j));
	    		            }
	    				}
	    				allEventDetails = allEventDetails + category;
	    			}
	    			
	    			if(utcEvt.containsKey(EventsParser.JSON_TAG_EVENT_TYPE)) {
	    				eventType = utcEvt.get(EventsParser.JSON_TAG_EVENT_TYPE);
	    				allEventDetails = allEventDetails + eventType;
	    			}
	    			
	    			boolean searchMatch = allEventDetails
	    									.toUpperCase(Locale.getDefault())
	    									.contains(mSearchFilter
	    											.toUpperCase(Locale.getDefault()));

					// filter by date
					String startDateStr = utcEvt.get(EventsParser.JSON_TAG_STARTDATE);
					String endDateStr = utcEvt.get(EventsParser.JSON_TAG_ENDDATE);
					DateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.ENGLISH);
					DateFormat formatSearch = new SimpleDateFormat("MM/dd/yyyy", Locale.ENGLISH);
					try {
						Date startDate = format.parse(startDateStr);
						Date endDate = format.parse(endDateStr);
						Date searchDate = formatSearch.parse(mDateFilter);
						searchMatch = searchMatch && (searchDate.compareTo(endDate) == 0 ||
								searchDate.before(endDate));
					}
					catch (ParseException pex) {
						Logger.error(mName, pex.toString());
					}

					// filter by type
					searchMatch = searchMatch && eventType.equals(mEventType);
	    			
    				if(searchMatch && !dm.isCategoryAntifiltered(
    						Integer.parseInt(utcEvt.get(EventsParser.JSON_TAG_CATID)))
    						) {
    					final Bundle args = new Bundle();
    					args.putString(EventsParser.JSON_TAG_BUSGUID, utcEvt.get(EventsParser.JSON_TAG_BUSGUID));
    					args.putString(EventsParser.JSON_TAG_ID, utcEvt.get(EventsParser.JSON_TAG_ID));
    	    			
    	    			View child = null;
    					try {
    						child = inflater.inflate(R.layout.event_details, null, false);
    					}
    					catch(InflateException ie) {
    						Logger.error(mName, "could not inflate child view");
    						throw new RuntimeException(ie);
    					}
    	    			
    	    			child.setId(idNum + idOffset++);

						// alternate white and gray
						if((backgroundIndex++ % 2) == 1) {
							RelativeLayout container = (RelativeLayout) child.findViewById(R.id.relativeLayoutEventDetailsContainer);
							container.setBackgroundColor(Color.parseColor("#FFFFFF"));
						}
    	    			
    	    			tv = (TextView) child.findViewById(R.id.eventDetailsFrom);
    	    			tv.setId(idNum + idOffset++);
    	    			tv.setClickable(true);
    	    			tv.setOnTouchListener(new EventTouchListener(args));
    	    			tv.setText(name);
    	    			
    	    			tv = (TextView) child.findViewById(R.id.eventDetailsDate);
    	    			tv.setId(idNum + idOffset++);
    	    			tv.setClickable(true);
    	    			tv.setOnTouchListener(new EventTouchListener(args));
    	    			
    	    			if(utcEvt.containsKey(EventsParser.JSON_TAG_DATE_AS_STR)) {
    	    				tv.setText(utcEvt.get(EventsParser.JSON_TAG_DATE_AS_STR));
    	    			}
    	    			
    	    			tv = (TextView) child.findViewById(R.id.eventDetailsSummary);
    	    			tv.setId(idNum + idOffset++);
    	    			tv.setClickable(true);
    	    			tv.setOnTouchListener(new EventTouchListener(args));
    	    			tv.setText(summary);

						Button readMore = (Button) child.findViewById(R.id.buttonEventDetailsReadMore);
						readMore.setOnTouchListener(new EventTouchListener(args));

						String url = Constants.LOCATION_INFO_IMAGE + "/" + utcEvt.get(EventContextParser.JSON_TAG_BUSGUID) + "@2x.png";
						ImageView logo = (ImageView) child.findViewById(R.id.imageViewEventDetails);
						logo.setId(idNum + idOffset++);
						addImage(url, logo);

    	    			// keep checking if this task was canceled and bail if so
    	    			if(isCancelled() == true) {
    	    				Logger.verbose(mName, "isCancelled == true in LoadEventsTask (loop)");
    	    				return 0;
    	    			}
    	    			
    	    			mEventProgress = i;
    	    			publishProgress(child);
        				try {
    						Thread.sleep(2 * Constants.LAYOUT_ADDITION_DELAY);
    					} catch (InterruptedException e) {
    						// don't do anything, we don't care if 
    						// a sleep is interrupted
    					}
    				}
    			}
        	}

        	return eventNum;
        }
        
        @Override
        protected void onProgressUpdate(View... child) {
        	super.onProgressUpdate(child[0]);

        	if(mEventProgress == 1) {
            	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setMax(DataManager.getInstance().getEventIDs().size());
            	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setVisibility(ProgressBar.VISIBLE);
        	}

        	try {
        		((LinearLayout) mParent.findViewById(R.id.linearLayoutSearchEvent)).addView(child[0]);
        	}
        	catch(IllegalStateException ise) {
        		ise.printStackTrace();
        	}
        	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setProgress(mEventProgress);
        }

        protected void onPostExecute(Integer locationNum) {
        	((MainActivity) getActivity()).hideSpinner();
        	((ProgressBar) getActivity().findViewById(R.id.progressBarHorizontal)).setVisibility(ProgressBar.GONE);
        	if(mEventRetrievalError != null) {
        		Toast.makeText(mParent.getContext(), mEventRetrievalError, Toast.LENGTH_SHORT).show();
        	}
        	mEventRetrievalError = null;
        }
    }
    
    public boolean cancelLoadEventsTask() {
    	boolean result = false;
    	if(mLoadEventsTask != null) {
    		Logger.info(mName, "LoadEventsTask cancelled");
    		result = mLoadEventsTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoadEventsTask was null when cancelling");
    	}
    	return result;
    }
    
    public void removeAllEventResults() {
    	LinearLayout results = (LinearLayout) mParent.findViewById(R.id.linearLayoutSearchEvent);
    	results.removeAllViews();
    }
    
    private class EventTouchListener implements View.OnTouchListener {
		
    	public Bundle mArguments;
    	
    	public EventTouchListener(Bundle args) {
    		mArguments = args;
    	}
    	
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				replaceSubmenuFragment(Constants.MenuID.EVENT, mArguments, true);
			}
			
			return false;
		}
	}

	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	private void addImage(String url, ImageView iv) {
		AddImageTask ait = new AddImageTask(iv);
		if(Utils.hasHoneycomb()) {
			ait.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, url);
		}
		else {
			ait.execute(url);
		}
	}

	private class AddImageTask extends AsyncTask<String, Void, ImageView> {

		ImageView mIv;

		AddImageTask(ImageView iv) {
			mIv = iv;
		}

		protected ImageView doInBackground(String... url) {
			ImageView img = new ImageView(mParent.getContext());

			if(isAdded()) ((MainActivity) getActivity())
					.getImageDownloader()
					.download(url[0], img);

			return img;
		}

		protected void onPostExecute(ImageView img) {
			if(img != null && mIv != null && img.getDrawable() != null && mIv.getDrawable() != null) {
				if(((BitmapDrawable)mIv.getDrawable()).getBitmap() != null && ((BitmapDrawable)img.getDrawable()).getBitmap() != null) {
					mIv.setImageBitmap(((BitmapDrawable)img.getDrawable()).getBitmap());
				}
			}
		}
	}
}