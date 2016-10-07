package com.unitethiscity.ui;

import com.unitethiscity.R;

import android.app.Activity;
import android.os.Bundle;
import android.view.Window;

public class NotificationActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		// dummy activity for now - user can press back to go 
		// back to normal 
		getWindow().requestFeature(Window.FEATURE_PROGRESS);
		setContentView(R.layout.activity_main);
	}
	
}
