package com.unitethiscity.ui;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

import com.unitethiscity.R;

public class ErrorActivity extends Activity {

    public static final String TAG = ErrorActivity.class.getSimpleName();

    public static final String ARG_ERROR = "error";

    private String mError;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_error);

        Bundle extras = getIntent().getExtras();
        if(extras != null) {
            mError = extras.getString(ARG_ERROR, "");
        }

        TextView error = (TextView) findViewById(R.id.textViewError);
        error.setText(mError);
    }
}
