package com.unitethiscity.ui;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.TextView;
import android.widget.Toast;

public class WebFragment extends Fragment {

	private String mName = Constants.WEB_FRAGMENT;
	
	public Constants.MenuType mMenuType = Constants.MenuType.SUB;
	public Constants.MenuID mMenuID = Constants.MenuID.WEB;
	private Constants.MenuID mParentID;

	private View mParent;
	private WebView mWebView;
	
	private boolean mFragmentActive = false;
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, 
							 Bundle savedInstanceState) {

		// Inflate the layout for this fragment
		mParent = inflater.inflate(R.layout.fragment_web, container, false);

		mWebView = (WebView) mParent.findViewById(R.id.webViewWeb);
		//mWebView.getSettings().setLoadWithOverviewMode(true);
		//mWebView.getSettings().setUseWideViewPort(true);
		//mWebView.getSettings().setBuiltInZoomControls(true);
		//mWebView.getSettings().setJavaScriptEnabled(true);

		final Activity activity = getActivity();
		mWebView.setWebChromeClient(new WebChromeClient() {
			public void onProgressChanged(WebView view, int progress) {
				// Activities and WebViews measure progress with different scales.
				// The progress meter will automatically disappear when we reach 100%
				activity.setProgress(progress * 1000);
			}
		});
		mWebView.setWebViewClient(new WebViewClient() {
			public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
				Toast.makeText(activity, "Oh no! " + description, Toast.LENGTH_SHORT).show();
			}
		});
		
		Bundle args = this.getArguments();
		if(args.containsKey(Constants.WEB_FRAGMENT_URL_ARG)) {
			mWebView.loadUrl(args.getString(Constants.WEB_FRAGMENT_URL_ARG));
		}
		else {
			mWebView.loadUrl(Constants.UNITE_THIS_CITY_HOME_URL);
		}
		if(args.containsKey(Constants.WEB_FRAGMENT_TITLE)) {
			TextView header = (TextView) mParent.findViewById(R.id.textViewWebHeader);
			header.setVisibility(TextView.VISIBLE);
			header.setText(args.getString(Constants.WEB_FRAGMENT_TITLE));
		}
		
		return mParent;
	}
	
    @Override
    public void onResume() {
    	super.onResume();
    	Logger.verbose(mName, "onResume()");
    	
        if(DataManager.getInstance().getAnalyticsState()) {
        	Logger.verbose(mName, "starting Google analytics for this screen - " + mParentID.toString());
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
    
    public void setParent(Constants.MenuID fID) {
    	mParentID = fID;
    }
    
    public Constants.MenuID getParent() {
    	return mParentID;
    }
    
    public boolean isFragmentActive() {
    	return mFragmentActive;
    }
	
    public boolean canGoBack() {
    	return mWebView.canGoBack();
    }
    
    public void goBack() {
    	mWebView.goBack();
    }
    
    public void destroyWebView() {
		// sluggishness occurs if web view used and then fragment navigated away from
		// therefore destroy it after we're done using it
		if(mWebView != null) {
			mWebView.destroy();
		}
    }
}
