package com.unitethiscity.ui;

import java.util.Calendar;
import java.util.Date;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.DatePickerDialog;
import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.graphics.Color;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.SpannableStringBuilder;
import android.text.TextWatcher;
import android.text.format.Time;
import android.text.style.ForegroundColorSpan;
import android.text.style.StyleSpan;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnFocusChangeListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.RadioGroup.OnCheckedChangeListener;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.FacebookContext;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class SubscribeFragment extends Fragment {
	
	private String mName = Constants.SUBSCRIBE_FRAGMENT;
	
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.SUBSCRIBE;
	
	private ViewGroup mContainer;
	private View mParent;
	
	private boolean mFragmentActive = false;

	private boolean mUseFacebookInfo = false;
	private String mFirstName = "";
	private String mLastName = "";
	private String mEmail = "";
	private String mZipCode = "";
	private String mGender = "male";
	private String mBirthdate = "";
	private String mPromoCode = "";
	private String mPrdID = "1";
	private String mFacebookID = "";
	private boolean mTermsAndConditionsChecked = false;
	
    private int mYear;
    private int mMonth;
    private int mDay;
	
	private String mAccount = "";
	private String mPassword = "";
	
	SubscribeTask mSubscribeTask;
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
    	Logger.verbose(mName, "onCreateView()");
    	
    	mContainer = container;
    	
        // Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_subscribe, container, false);
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        
        // set "Unite This City." to bold and black
        TextView desc = (TextView) mParent.findViewById(R.id.subscribeDescription);
        SpannableStringBuilder sb = new SpannableStringBuilder(getString(R.string.subscribe_description));
        ForegroundColorSpan fcs = new ForegroundColorSpan(Color.rgb(0, 0, 0));
        StyleSpan bss = new StyleSpan(android.graphics.Typeface.BOLD);       
        sb.setSpan(bss, 313, 329, Spannable.SPAN_INCLUSIVE_INCLUSIVE);
        sb.setSpan(fcs, 313, 329, Spannable.SPAN_INCLUSIVE_INCLUSIVE);
        desc.setText(sb);

		FacebookContext fc = LoginManager.getInstance().getFacebookContext();
		mFacebookID = fc.getUserID();
		if(mFacebookID != null) {
			mUseFacebookInfo = true;
		}

    	// EditText listeners
        EditText et = (EditText) mParent.findViewById(R.id.editTextFirstName);
        et.addTextChangedListener(new TextWatcher() {
            public void afterTextChanged(Editable s)
            {
                mFirstName = s.toString();
            }
            
            public void beforeTextChanged(CharSequence s, int start, int count, int after) { }
            public void onTextChanged(CharSequence s, int start, int before, int count) { }
        });
		if(mUseFacebookInfo && fc.getFirstName() != null) {
			et.setText(fc.getFirstName());
			mFirstName = fc.getFirstName();
		}
        
        et = (EditText) mParent.findViewById(R.id.editTextLastName);
        et.addTextChangedListener(new TextWatcher() {
            public void afterTextChanged(Editable s)
            {
                mLastName = s.toString();
            }
            
            public void beforeTextChanged(CharSequence s, int start, int count, int after) { }
            public void onTextChanged(CharSequence s, int start, int before, int count) { }
        });
		if(mUseFacebookInfo && fc.getLastName() != null) {
			et.setText(fc.getLastName());
			mLastName = fc.getLastName();
		}
    	
        et = (EditText) mParent.findViewById(R.id.editTextEmail);
        et.addTextChangedListener(new TextWatcher() {
            public void afterTextChanged(Editable s)
            {
                mEmail = s.toString();
            }
            
            public void beforeTextChanged(CharSequence s, int start, int count, int after) { }
            public void onTextChanged(CharSequence s, int start, int before, int count) { }
        });
		if(mUseFacebookInfo && fc.getEmail() != null) {
			et.setText(fc.getEmail());
			mEmail = fc.getEmail();
		}
        
        et = (EditText) mParent.findViewById(R.id.editTextZipCode);
        et.addTextChangedListener(new TextWatcher() {
            public void afterTextChanged(Editable s)
            {
                mZipCode = s.toString();
            }
            
            public void beforeTextChanged(CharSequence s, int start, int count, int after) { }
            public void onTextChanged(CharSequence s, int start, int before, int count) { }
        });
        
        RadioGroup rg = (RadioGroup) mParent.findViewById(R.id.radioGroupGender);
        rg.setOnCheckedChangeListener(new OnCheckedChangeListener() {
			@Override
			public void onCheckedChanged(RadioGroup group, int checkedId) {
				if(checkedId != -1) {
					RadioButton thisRb = (RadioButton) mParent.findViewById(checkedId);
					if(thisRb != null) {
						mGender = thisRb.getText().toString().toLowerCase();
					}
				}
			}
        });
		if(mUseFacebookInfo && fc.getGender() != null) {
			if(fc.getGender().equals("male")) {
				((RadioButton) mParent.findViewById(R.id.radioMale)).toggle();
			}
			else {
				((RadioButton) mParent.findViewById(R.id.radioFemale)).toggle();
			}
			mGender = fc.getGender();
		}
        
        final Calendar c = Calendar.getInstance();
        final TextView birthdateString = (TextView) mParent.findViewById(R.id.subscribeBirthdateString);

		if(mUseFacebookInfo && fc.getBirthday() != null) {
			mBirthdate = fc.getBirthday();
		}
		else {
			mYear = c.get(Calendar.YEAR);
			mMonth = c.get(Calendar.MONTH) + 1;
			mDay = c.get(Calendar.DAY_OF_MONTH);
			mBirthdate = String.format("%02d/%02d/%04d", mMonth, mDay, mYear);
		}
    	birthdateString.setText(mBirthdate);
        Button birthdateButton = (Button) mParent.findViewById(R.id.buttonBirthdate);
        birthdateButton.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View arg0) {
				DatePickerDialog.OnDateSetListener thisListener = 
						new DatePickerDialog.OnDateSetListener() {
			         
                    @Override
                    public void onDateSet(DatePicker view, int year,
                            int monthOfYear, int dayOfMonth) {
                        mYear = year;
                        mMonth = monthOfYear + 1;
                        mDay = dayOfMonth;
                        mBirthdate = String.format("%02d/%02d/%04d", 
                        		mMonth, mDay, mYear);
                    	birthdateString.setText(mBirthdate);
                    }
                };
				
		        DatePickerDialog dpd = new DatePickerDialog(mParent.getContext(),
		        		thisListener, mYear, mMonth, mDay);
		        dpd.show();
			}
        });

        et = (EditText) mParent.findViewById(R.id.editTextPromoCode);
        et.addTextChangedListener(new TextWatcher() {
            public void afterTextChanged(Editable s)
            {
                mPromoCode = s.toString();
            }
            
            public void beforeTextChanged(CharSequence s, int start, int count, int after) { }
            public void onTextChanged(CharSequence s, int start, int before, int count) { }
        });

        // Checkbox listener
        final CheckBox cb = (CheckBox) mParent.findViewById(R.id.checkBoxSubscribeVerify);
        cb.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
        	   @Override
        	   public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        		   mTermsAndConditionsChecked = isChecked;
        	   }
        });
        
        // TextView listener
        final TextView tv = (TextView) mParent.findViewById(R.id.subscribeTerms);
        tv.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
				Uri url = Uri.parse(Constants.UNITE_THIS_CITY_TERMS_AND_CONDITIONS);
				Intent launchBrowser = new Intent(Intent.ACTION_VIEW, url);
				try {
					startActivity(launchBrowser);
				}
				catch(ActivityNotFoundException anfe) {
					anfe.printStackTrace();
				}
            }
        });
        // set "here" to orange/yellow
        Spannable wordToSpan = new SpannableString(getString(R.string.subscribe_verify_terms));        
        wordToSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 204, 0)), 30, 34, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        tv.setText(wordToSpan);
        
        // Button listener
        final Button b = (Button) mParent.findViewById(R.id.buttonSubscribeSubmit);
        b.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
            	// verify form information?
            	boolean anyBlankFields = false;
            	// make sure all EditText fields have been populated (besides promo code)
            	anyBlankFields |= mFirstName == "";
            	anyBlankFields |= mLastName == "";
            	anyBlankFields |= mEmail == "";
            	anyBlankFields |= mZipCode == "";
            	anyBlankFields |= mGender == "";
            	anyBlankFields |= mBirthdate == "";
            	
                // check blank field verification
            	if(anyBlankFields) {
            		Toast.makeText(getActivity(), "Please fill in all information",
            				Toast.LENGTH_SHORT).show();
            		return;
            	}
            	
            	if(!mEmail.contains("@")) {
            		Toast.makeText(getActivity(), "Please enter a valid email address",
            				Toast.LENGTH_SHORT).show();
            		return;
            	}
            	
            	if(mZipCode.length() != 5) {
            		Toast.makeText(getActivity(), "Please enter a valid ZIP code",
            				Toast.LENGTH_SHORT).show();
            		return;
            	}
            	
                // submit form information
            	if(mTermsAndConditionsChecked) {
            		Logger.info(mName, "terms and conditions agreed upon");
            		Logger.info(mName, mFirstName + " " + mLastName + " " + mEmail + " " + mZipCode + " " + mGender +
							" " + mBirthdate + " " + mPromoCode + " " + mPrdID);
            		
            		((MainActivity) getActivity()).showSpinner();
            		mSubscribeTask = new SubscribeTask();
					if(Utils.hasHoneycomb()) {
						mSubscribeTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
					}
					else {
						mSubscribeTask.execute();
					}
            	}
            	else {
            		Logger.info(mName, "terms and conditions were not accepted");
            		
            		Toast.makeText(getActivity(), "Please accept the Terms and Conditions before signing up",
            				Toast.LENGTH_SHORT).show();
            	}
            }
        });

		if(mUseFacebookInfo && fc.getAccount() != null && fc.getPassword() != null) {
			mAccount = fc.getAccount();
			mPassword = fc.getPassword();
			((MainActivity) getActivity()).setAccountFragment();
			LoginManager.getInstance().login((MainActivity) getActivity(), mAccount, mPassword);
			clearAllInformation();
		}
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
    
    private void clearAllInformation() {
        EditText et = (EditText) mParent.findViewById(R.id.editTextFirstName);
		mFirstName = "";
        et.setText("");

        et = (EditText) mParent.findViewById(R.id.editTextLastName);
		mLastName = "";
        et.setText("");

        et = (EditText) mParent.findViewById(R.id.editTextEmail);
		mEmail = "";
        et.setText("");

        et = (EditText) mParent.findViewById(R.id.editTextZipCode);
		mZipCode = "";
        et.setText("");
        
        RadioGroup rg = (RadioGroup) mParent.findViewById(R.id.radioGroupGender);
        rg.clearCheck();
        
        Calendar now = Calendar.getInstance();
    	mBirthdate = String.format("%02d/%02d/%04d", 
    			now.get(Calendar.YEAR), now.get(Calendar.MONTH), now.get(Calendar.DAY_OF_MONTH));
    	TextView birthdateString = 
    			(TextView) mParent.findViewById(R.id.subscribeBirthdateString);
    	birthdateString.setText(mBirthdate);
        
        et = (EditText) mParent.findViewById(R.id.editTextPromoCode);
        et.setText("");
        mPromoCode = "";

        CheckBox cb = (CheckBox) mParent.findViewById(R.id.checkBoxSubscribeVerify);
        cb.setChecked(false);
        mTermsAndConditionsChecked = false;
    }
    
    private class SubscribeTask extends AsyncTask<Void, Void, String> {
    	protected String doInBackground(Void... params) {

			JSONObject json = new JSONObject();

			if(mUseFacebookInfo == false) {
				// request free sign up (version 2 - phone number replaced by zip code)
				json = UTCWebAPI.freeSignUp2(mFirstName, mLastName, mEmail,
						mZipCode, mGender, mBirthdate, mPromoCode);
			}
			else {
				// request free sign up (version 3 - Facebook signup)
				json = UTCWebAPI.freeSignUp3(mFirstName, mLastName, mEmail,
						mZipCode, mGender, mBirthdate, mPromoCode, mFacebookID);
			}


    		// if JSON request didn't work, bail
    		if(json == null) {
    			return "Sign up request failed (1)";
    		}

    		if(json.has("Message")) {		
    			try {
    				return json.getString("Message");
    			} catch (JSONException e) {
    				e.printStackTrace();
    			}
    		}
    		
    		if(!json.has("Account") && !json.has("Password")) {
    			return "Sign up request failed (2)";
    		}
    		
    		try {
				mAccount = json.getString("Account");
			} catch (JSONException e) {
				e.printStackTrace();
			}
    		try {
				mPassword = json.getString("Password");
			} catch (JSONException e) {
				e.printStackTrace();
			}
    		
			return null;
    	}

    	protected void onPostExecute(String result) {
    		((MainActivity) getActivity()).hideSpinner();
    		if(result == null) {
    			Logger.info(mName, "Account - " + mAccount + ", Password " + mPassword);
    			((MainActivity) getActivity()).setAccountFragment();
    			LoginManager.getInstance().login((MainActivity) getActivity(), mAccount, mPassword);
    			clearAllInformation();
    		}
    		else {
        		Toast.makeText(getActivity(), result, Toast.LENGTH_SHORT).show();
    		}
    	}
    }
    
    public boolean cancelSubscribeTask() {
    	boolean result = false;
    	if(mSubscribeTask != null) {
    		result = mSubscribeTask.cancel(true);
    	}
    	return result;
    }
}