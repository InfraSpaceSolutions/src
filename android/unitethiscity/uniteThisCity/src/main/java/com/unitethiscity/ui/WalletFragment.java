package com.unitethiscity.ui;

import java.text.NumberFormat;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.WalletParser;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import android.support.v4.app.Fragment;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

public class WalletFragment extends Fragment {
	
	private String mName = Constants.WALLET_FRAGMENT;
	public Constants.MenuType mMenuType = Constants.MenuType.MAIN;
	public Constants.MenuID mMenuID = Constants.MenuID.WALLET;
	
	private View mParent;
	
	private boolean mFragmentActive = false;
	
	private AsyncTask<String, Void, Boolean> mWalletTask;
	
	private String mStringReply;
	
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
    	// Inflate the layout for this fragment
    	mParent = inflater.inflate(R.layout.fragment_wallet, container, false);
    	mParent.findViewById(R.id.scrollViewWallet).setVisibility(View.INVISIBLE);
        
        return mParent;
    }
    
    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

    	((MainActivity) getActivity()).showSpinner();
    	
        String token = null;
        mWalletTask = new WalletTask();
        if(LoginManager.getInstance().userLoggedIn()) {
        	token = LoginManager.getInstance().getAccountContext().getToken();
        }
		if(Utils.hasHoneycomb()) {
			mWalletTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, token);
		}
		else {
			mWalletTask.execute(token);
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
    
    private class WalletTask extends AsyncTask<String, Void, Boolean> {
        protected Boolean doInBackground(String... params) {
    		String context = params[0];
    		
    		mStringReply = null;
    		
            if(DataManager.getInstance().doesWalletNeedUpdate()) {
            	mStringReply = WalletParser.setWallet(context);

        		if(mStringReply != null) {
        			return Boolean.valueOf(false);
        		}
                
                DataManager.getInstance().setWalletNeedsUpdate(false);
            }

    		return Boolean.valueOf(true);
        }
        
        protected void onPostExecute(Boolean success) {
        	if(success.booleanValue()) {
                DataManager dm = DataManager.getInstance();
                TextView tv;
                String value = null;
                
                double cashAvailable = Double.parseDouble(
                		dm.getFromWalletData(
                				WalletParser.JSON_TAG_CASH_AVAILABLE)
                				.substring(1)
                				.replace(",", "")
                				) 
                				-
                		Double.parseDouble(
                				dm.getFromWalletData(WalletParser.JSON_TAG_CASH_REDEEMED)
                				.substring(1)
                				.replace(",", "")
                				);
            	NumberFormat formatter = NumberFormat.getCurrencyInstance();
            	value = formatter.format(cashAvailable);
                tv = (TextView) mParent.findViewById(R.id.walletCash);
                String cashAmount;
            	if(value != null) {
                	cashAmount = value;
            	}
            	else {
            		cashAmount = "$0";
            	}
            	if(tv == null) {
            		return;
            	}
            	tv.setText(cashAmount);
                
                if(LoginManager.getInstance().userLoggedIn()) {
                	tv = (TextView) mParent.findViewById(R.id.walletName);
                	String displayName = "HEY " + LoginManager.getInstance().getAccountContext().getAccountFirstName().toUpperCase();
                	tv.setText(displayName);

					value = dm.getFromWalletData(WalletParser.JSON_TAG_CASH_REDEEMED);
					tv = (TextView) mParent.findViewById(R.id.walletCashSavingsThisMonth);
					tv.setText(value);

					value = dm.getFromWalletData(WalletParser.JSON_TAG_CASH_REDEEMED_ALL_TIME);
					tv = (TextView) mParent.findViewById(R.id.walletCashSavingsAllTime);
					tv.setText(value);
                	
                	value = dm.getFromWalletData(WalletParser.JSON_TAG_POINTS_ALL_TIME);
                	tv = (TextView) mParent.findViewById(R.id.walletCheckInPointsAmount);
                	String checkInsPoints;
                	if(value != null) {
                		// chop off .0 in value if present
                		if(value.length() > 2) {
                    		if(value.substring(value.length() - 2).equals(".0")) {
                    			value = value.substring(0, value.length() - 2);
                    	    }
                		}

                		if(value.equals("1")) {
                			checkInsPoints = value + " POINT";
                		}
                		else {
                			checkInsPoints = value + " POINTS";
                		}
                	}
                	else {
                		checkInsPoints = "0 POINTS";
                	}
                	tv.setText(checkInsPoints);
                	
                	tv = (TextView) mParent.findViewById(R.id.walletMembersOnly);
                	tv.setVisibility(TextView.GONE);
                }
                else {
                	if(!isAdded()) return;
                	tv = (TextView) mParent.findViewById(R.id.walletName);
                	String displayName = getActivity().getResources().getString(R.string.wallet_name_nonmember);
                	tv.setText(displayName);
                	
                	if(!isAdded()) return;
                	tv = (TextView) mParent.findViewById(R.id.walletCheckInPointsAmount);
                	String pointsNonmember = getActivity().getResources().getString(R.string.wallet_default_points);
                	tv.setText(pointsNonmember);
                	
                	if(!isAdded()) return;
                	tv = (TextView) mParent.findViewById(R.id.walletMembersOnly);
                	tv.setVisibility(TextView.VISIBLE);
                }
        	}
        	else {
        		if(!isAdded()) return;
        		Toast.makeText(getActivity(), mStringReply, Toast.LENGTH_SHORT).show();
        	}
        	
        	mParent.findViewById(R.id.scrollViewWallet).setVisibility(View.VISIBLE);
        	
        	if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
    
    public boolean cancelWalletTask() {
    	boolean result = false;
    	if(mWalletTask != null) {
    		Logger.info(mName, "WalletTask cancelled");
    		result = mWalletTask.cancel(true);
    	}
    	else {
    		Logger.verbose(mName, "WalletTask was null when cancelling");
    	}
    	return result;
    }
}