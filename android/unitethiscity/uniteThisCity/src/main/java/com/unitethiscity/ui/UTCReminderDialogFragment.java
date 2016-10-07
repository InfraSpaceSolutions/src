package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;

import com.unitethiscity.R;

public class UTCReminderDialogFragment extends DialogFragment {
	@Override
	public Dialog onCreateDialog(Bundle savedInstanceState) {
		
		// Use the Builder class for convenient dialog construction
		AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
		builder.setTitle(R.string.social_facebook_reminder_title)
		.setMessage(R.string.social_facebook_reminder_description)
		.setPositiveButton(R.string.social_facebook_reminder_button_ok, new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int id) {
				// user wants to connect
				((MainActivity) getActivity()).showSpinner();
				((MainActivity) getActivity()).facebookLogin(true, true);
			}
		})
		.setNegativeButton(R.string.social_facebook_reminder_button_not_now, new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int id) {
				// user doesn't want to connect to Facebook
				// do nothing
			}
		});
		
		return builder.create();
		
	}
}