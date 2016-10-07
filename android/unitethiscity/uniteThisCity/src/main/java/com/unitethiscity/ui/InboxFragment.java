package com.unitethiscity.ui;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import org.json.JSONException;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.InboxParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCMessage;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.annotation.TargetApi;
import android.graphics.drawable.BitmapDrawable;
import android.os.Build;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.SystemClock;
import android.os.Vibrator;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

public class InboxFragment extends Fragment {
	
	private String mName = Constants.INBOX_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.MAIN;
	public Constants.MenuID mMenuID = Constants.MenuID.INBOX;
	
	private LayoutInflater mInflater;
	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;
	private String mMessagesRetrievalError = null;
	private String mMessageRetrievalError = null;
	
	private LinearLayout mMessagesLayout;
	
	private AsyncTask<Void, Void, Integer> mLoadMessagesTask;
	private AsyncTask<Integer, Void, Integer> mLoadMessageTask;

    @Override
    public void onCreate(Bundle savedInstanceState) {
    	super.onCreate(savedInstanceState);
    	Logger.verbose(mName, "onCreate()");
    }
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
    	Logger.verbose(mName, "onCreateView()");
    	
    	mInflater = inflater;
    	mContainer = container;
    	
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_inbox, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");

    	if(mFragmentActive == true) {
            ((MainActivity) getActivity()).clearBackPressed();
            loadMessages();
    	}
    	Logger.verbose(mName, "onActivityCreated fragmentActive - " + mFragmentActive);
    }

    @Override
    public void onPause() {
    	super.onPause();
    	Logger.verbose(mName, "onPause()");
    }
    
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");
    	
    	mFragmentActive = true;
    	
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
    
	public void replaceSubmenuFragment(Constants.MenuID fID, Bundle args, boolean goBack)
	{
		final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
		// Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
		vib.vibrate(Constants.VIBRATE_LENGTH);
		
		FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

		mFragmentActive = false;
		
		// Replace whatever is in the frameLayout view with this fragment
	    switch (fID) {
	    case WEB:
	    	((MainActivity) getActivity()).setFragmentID(fID);
	    	((MainActivity) getActivity()).setParentFragmentID(mMenuID);
	    	WebFragment wf = DataManager.getInstance().getWebFragment();
			wf.setArguments(args);
			wf.setParent(mMenuID);
			wf.fragmentActive(true);
			transaction.replace(R.id.frameLayoutMiddle, wf, fID.toString());
			
		    // add transaction to back stack if we want to go back to where we were
		    if(goBack) {
		    	DataManager.getInstance().pushToMenuStack(fID);
		    }
		    
			// Commit the transaction
			transaction.commit();
			
	    	break;
	    default:
	    	break;
	    }
	}
	
	public void cancelAllTasks() {
		cancelLoadMessagesTask();
	}

	public void loadMessages() {
		if(isAdded()) ((MainActivity) getActivity()).showSpinner();
        mLoadMessagesTask = new LoadMessagesTask();

		// make sure messages are visible
		LinearLayout messages = (LinearLayout)mParent.findViewById(R.id.linearLayoutInboxMessages);
		messages.setVisibility(LinearLayout.VISIBLE);
		messages.removeAllViewsInLayout();

        // task object could be corrupted or canceled
        // when switching menus
        if(mLoadMessagesTask != null) {
			if(Utils.hasHoneycomb()) {
				mLoadMessagesTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
			}
			else {
				mLoadMessagesTask.execute();
			}
        }
	}

	public void loadMessage(Integer id) {
		if(isAdded()) ((MainActivity) getActivity()).showSpinner();
		mLoadMessageTask = new LoadMessageTask();

		// task object could be corrupted or canceled
		// when switching menus
		if(mLoadMessageTask != null) {
			if(Utils.hasHoneycomb()) {
				mLoadMessageTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, id);
			}
			else {
				mLoadMessageTask.execute(id);
			}
		}
	}
	
    private class LoadMessagesTask extends AsyncTask<Void, Void, Integer> {
        protected Integer doInBackground(Void... params) {
            DataManager dm = DataManager.getInstance();

        	if((MainActivity) getActivity() == null) {
        		Logger.verbose(mName, "main == null in LoadMessagesTask");
        		return -1;
        	}
        	
        	// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadMessagesTask");
				return -1;
			}
            
    		/////////////////////////////////////////////////////////////////////////
    		//////////   Retrieve message data   ////////////////////////////////////
    		/////////////////////////////////////////////////////////////////////////
			mMessagesRetrievalError = null;
			try {
				mMessagesRetrievalError = InboxParser.setMessages();
			} catch (JSONException e1) {
				e1.printStackTrace();
				return -1;
			}
			
			if(mMessagesRetrievalError != null) {
				return -1;
			}
    		
    		/////////////////////////////////////////////////////////////////////////
    		//////////   Create message layouts   ///////////////////////////////////
    		/////////////////////////////////////////////////////////////////////////
            TextView tv;
            
    		int idNum = Constants.BASE_CUSTOM_IDS;
    		int idOffset = 10000;
    		
    		mMessagesLayout = new LinearLayout(mParent.getContext());
    		mMessagesLayout.setId(idNum + idOffset++);
    		mMessagesLayout.setOrientation(LinearLayout.VERTICAL);
    		
    		//LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            // Inflate the layout for this fragment
        	//mParent = mInflater.inflate(R.layout.fragment_inbox, mContainer, false);
    		for(int i = 0; i < dm.getMessages().size(); i++) {
    			// keep checking if this task was canceled and bail if so
    			if(isCancelled() == true) {
    				Logger.verbose(mName, "isCancelled == true in LoadMessagesTask (loop)");
    				return -1;
    			}
				
    			UTCMessage message = dm.getMessages().get(i);
    			
    			final Bundle args = new Bundle();
    			String messageURL = Constants.MESSAGE_VIEW_URL + 
    					String.valueOf(message.getID()) + 
    					"&tok=" +
    					LoginManager.getInstance().getAccountContext().getToken();
    			args.putString(Constants.WEB_FRAGMENT_URL_ARG, messageURL);
				args.putInt(Constants.INBOX_FRAGMNENT_ID_ARG, message.getID());
				args.putString(Constants.INBOX_FRAGMENT_SUMMARY_ARG, message.getSummary());
				args.putString(Constants.INBOX_FRAGMENT_BODY_ARG, message.getBody());
				args.putBoolean(Constants.INBOX_FRAGMENT_READ_ARG, message.getInboxRead());

    			View child = null;
    			try {
    				child = mInflater.inflate(R.layout.message_details, null, false);
    			}
    			catch(InflateException ie) {
    				Logger.error(mName, "could not inflate child view");
    				ie.printStackTrace();
    				return -1;
    			}

    			child.setId(idNum + idOffset++);

    			tv = (TextView) child.findViewById(R.id.messageDetailsFrom);
    			tv.setId(idNum + idOffset++);
    			tv.setClickable(true);
    			tv.setOnTouchListener(new MessageTouchListener(args));
    			tv.setText(message.getFromName());

    			Button b = (Button) child.findViewById(R.id.messageDetailsDate);
    			b.setId(idNum + idOffset++);
    			b.setClickable(true);
    			b.setOnTouchListener(new MessageTouchListener(args));
    			Date date = null;
    			try {
    				date = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS", Locale.US).parse(message.getMessageTimestamp());
    			} catch (ParseException e) {
    				e.printStackTrace();
    			};
    			if(date != null) {
    				String formattedDate = new SimpleDateFormat("MM/dd/yy", Locale.US).format(date);
    				b.setText("RECEIVED " + formattedDate);
    			}
    			else {
    				b.setText("RECEIVED - NO DATE");
    			}

				b = (Button) child.findViewById(R.id.buttonMessageDetailsReadMore);
				b.setClickable(true);
				b.setOnTouchListener(new MessageTouchListener(args));

    			// keep checking if this task was canceled and bail if so
    			if(isCancelled() == true) {
    				Logger.verbose(mName, "isCancelled == true in LoadMessagesTask (loop)");
    				return -1;
    			}

    			tv = (TextView) child.findViewById(R.id.messageDetailsSummary);
    			tv.setId(idNum + idOffset++);
    			tv.setClickable(true);
    			tv.setOnTouchListener(new MessageTouchListener(args));
    			tv.setText(message.getSummary());

    			RelativeLayout container = (RelativeLayout) child.findViewById(R.id.relativeLayoutMessageDetailsContainer);
    			if(!message.getInboxRead()) {
					container.setBackgroundColor(Constants.FAVORITE_LOCATION_COLOR);
					TextView newMessage = (TextView) container.findViewById(R.id.textViewMessageDetailsNew);
					newMessage.setVisibility(TextView.VISIBLE);
    			}
				container.setClickable(true);
				container.setOnTouchListener(new MessageTouchListener(args));

				String url = Constants.LOCATION_INFO_IMAGE + "/" + message.getBusinessGuid() + "@2x.png";
				ImageView logo = (ImageView) child.findViewById(R.id.imageViewMessageDetails);
				logo.setId(idNum + idOffset++);
				addImage(url, logo);

				mMessagesLayout.addView(child);
        	}

    		return 0;
        }
        
        protected void onPostExecute(Integer error) {
        	if(isCancelled() == false && mMessagesLayout != null) {
        		LinearLayout rootLayout = (LinearLayout) mParent.findViewById(R.id.linearLayoutInboxMessages);
				if(mMessagesLayout.getChildCount() > 0) {
					try {
						rootLayout.addView(mMessagesLayout);
					} catch (IllegalStateException ise) {
						ise.printStackTrace();
					}
				}
				else {
					RelativeLayout none = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutInboxNone);
					none.setVisibility(RelativeLayout.VISIBLE);
				}
        	}
        	if(mMessagesRetrievalError != null) {
        		Toast.makeText(mParent.getContext(), mMessagesRetrievalError, Toast.LENGTH_SHORT).show();
        	}
        	
        	if(!isAdded()) return;
        	((MainActivity) getActivity()).hideSpinner();
        	mMessagesRetrievalError = null;
        }
    }
    
    public boolean cancelLoadMessagesTask() {
    	boolean result = false;
    	if(mLoadMessagesTask != null) {
    		Logger.info(mName, "LoadMessagesTask cancelled");
    		result = mLoadMessagesTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "LoadMessagesTask was null when cancelling");
    	}
    	return result;
    }

	private class LoadMessageTask extends AsyncTask<Integer, Void, Integer> {
		protected Integer doInBackground(Integer... params) {
			DataManager dm = DataManager.getInstance();

			if((MainActivity) getActivity() == null) {
				Logger.verbose(mName, "main == null in LoadMessageTask");
				return -1;
			}

			// bail if this task was canceled early
			if(isCancelled() == true) {
				Logger.verbose(mName, "isCancelled == true in LoadMessageTask");
				return -1;
			}

			/////////////////////////////////////////////////////////////////////////
			//////////   Retrieve message data   ////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////
			mMessageRetrievalError = null;
			try {
				mMessageRetrievalError = InboxParser.setMessage(params[0]);
			} catch (JSONException e1) {
				e1.printStackTrace();
				return -1;
			}

			if(mMessageRetrievalError != null) {
				return -1;
			}

			return 0;
		}

		protected void onPostExecute(Integer error) {
			DataManager dm = DataManager.getInstance();
			UTCMessage message = dm.getCurrentMessage();

			((MainActivity) getActivity()).hideSpinner();

			if(isCancelled() == false && mMessagesLayout != null && message != null) {
				bindMessage(message);
			}

			if(mMessageRetrievalError != null) {
				Toast.makeText(mParent.getContext(), mMessageRetrievalError, Toast.LENGTH_SHORT).show();
			}

			if(!isAdded()) return;
			mMessageRetrievalError = null;
		}
	}

	public boolean cancelLoadMessageTask() {
		boolean result = false;
		if(mLoadMessageTask != null) {
			Logger.info(mName, "LoadMessageTask cancelled");
			result = mLoadMessageTask.cancel(true);
		}
		else {
			Logger.verbose(mName, "LoadMessageTask was null when cancelling");
		}
		return result;
	}

	private void bindMessage(UTCMessage msg) {
		TextView body = (TextView)mParent.findViewById(R.id.messageBody);
		body.setText(msg.getBody());
	}

    private class MessageTouchListener implements View.OnTouchListener {
		
    	public Bundle mArguments;
    	
    	public MessageTouchListener(Bundle args) {
    		mArguments = args;
    	}
    	
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			if(event.getAction() == (MotionEvent.ACTION_UP)) {
				// force an inbox update (message could be considered read or get deleted?)
				DataManager.getInstance().setMessagesTimestamp(SystemClock.elapsedRealtime()
						- Constants.MESSAGES_UPDATE_DWELL);
				//replaceSubmenuFragment(Constants.MenuID.WEB, mArguments, true);

				final LinearLayout message = (LinearLayout)mParent.findViewById(R.id.linearLayoutMessage);
				final LinearLayout messages = (LinearLayout)mParent.findViewById(R.id.linearLayoutInboxMessages);
				TextView summary = (TextView)mParent.findViewById(R.id.messageSummary);
				final TextView body = (TextView)mParent.findViewById(R.id.messageBody);
				final ImageView back = (ImageView) mParent.findViewById(R.id.imageViewInboxBack);
				Button delete = (Button)mParent.findViewById(R.id.buttonMessageDelete);
				Button optout = (Button)mParent.findViewById(R.id.buttonMessageOptOut);

				final Integer id = mArguments.getInt(Constants.INBOX_FRAGMNENT_ID_ARG);
				String summaryStr = mArguments.getString(Constants.INBOX_FRAGMENT_SUMMARY_ARG);
				String bodyStr = mArguments.getString(Constants.INBOX_FRAGMENT_BODY_ARG);
				boolean read = mArguments.getBoolean(Constants.INBOX_FRAGMENT_READ_ARG);

				// get individual message
				loadMessage(id);

				messages.setVisibility(LinearLayout.GONE);
				summary.setText(summaryStr);
				message.setVisibility(LinearLayout.VISIBLE);
				back.setVisibility(ImageView.VISIBLE);
				back.setClickable(true);
				back.setOnClickListener(new View.OnClickListener() {
					@Override
					public void onClick(View v) {
						message.setVisibility(LinearLayout.GONE);
						back.setVisibility(ImageView.GONE);
						body.setText("");
						loadMessages();
					}
				});

				delete.setOnClickListener(new View.OnClickListener() {
					@Override
					public void onClick(View v) {
						DeleteMessageTask deleteTask = new DeleteMessageTask(id);
						if(Utils.hasHoneycomb()) {
							deleteTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
						}
						else {
							deleteTask.execute();
						}
					}
				});

				optout.setOnClickListener(new View.OnClickListener() {
					@Override
					public void onClick(View v) {
						OptOutTask optOutTask = new OptOutTask(id);
						if(Utils.hasHoneycomb()) {
							optOutTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
						}
						else {
							optOutTask.execute();
						}
					}
				});

				if(read == false) {
					ReadMessageTask readTask = new ReadMessageTask(id);
					if(Utils.hasHoneycomb()) {
						readTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
					}
					else {
						readTask.execute();
					}
				}
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
			if(img != null && mIv != null) {
				if(img.getDrawable() != null && ((BitmapDrawable)img.getDrawable()).getBitmap() != null) {
					mIv.setImageBitmap(((BitmapDrawable)img.getDrawable()).getBitmap());
				}
			}
		}
	}

	private class ReadMessageTask extends AsyncTask<Void, Void, Void> {

		Integer mID;

		ReadMessageTask(Integer id) {
			mID = id;
		}

		protected Void doInBackground(Void... v) {
			UTCWebAPI.readMessage(LoginManager.getInstance().getAccountContext().getToken(), mID);

			return null;
		}

		protected void onPostExecute(Void v) {
			// do nothing
		}
	}

	private class DeleteMessageTask extends AsyncTask<Void, Void, Void> {

		Integer mID;

		DeleteMessageTask(Integer id) {
			mID = id;
		}

		protected Void doInBackground(Void... v) {
			UTCWebAPI.deleteMessage(LoginManager.getInstance().getAccountContext().getToken(), mID);

			return null;
		}

		protected void onPostExecute(Void v) {
			showAndReloadMessages();
		}
	}

	private class OptOutTask extends AsyncTask<Void, Void, Void> {

		Integer mID;

		OptOutTask(Integer id) {
			mID = id;
		}

		protected Void doInBackground(Void... v) {
			UTCWebAPI.optOut(LoginManager.getInstance().getAccountContext().getToken(), mID);

			return null;
		}

		protected void onPostExecute(Void v) {
			showAndReloadMessages();
		}
	}

	private void showAndReloadMessages() {
		LinearLayout message = (LinearLayout)mParent.findViewById(R.id.linearLayoutMessage);
		ImageView back = (ImageView) mParent.findViewById(R.id.imageViewInboxBack);
		message.setVisibility(LinearLayout.GONE);
		back.setVisibility(ImageView.GONE);
		loadMessages();
	}
}
