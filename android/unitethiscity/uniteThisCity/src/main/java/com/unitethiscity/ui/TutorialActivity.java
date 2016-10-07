package com.unitethiscity.ui;

import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.app.Activity;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.TextView;
import android.widget.ViewFlipper;

import com.unitethiscity.R;
import com.unitethiscity.general.Constants;

public class TutorialActivity extends Activity {

    private String mName = Constants.TUTORIAL_ACTIVITY;

    private ViewFlipper mViewFlipper;
    private float mLastX;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tutorial);
        mViewFlipper = (ViewFlipper) findViewById(R.id.viewFlipper);

        // show the tutorial if not previously shown
        SharedPreferences settings = getSharedPreferences(Constants.SHARED_PREFERENCES_NAME, 0);
        settings.edit().putBoolean(Constants.SHARED_PREFERENCES_TUTORIAL_SHOWN, true).apply();


        TextView tv = (TextView) findViewById(R.id.textViewCloseTutorial);
        tv.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                finish();
                return true;
            }
        });

        tv = (TextView) findViewById(R.id.textViewWatchVideo);
        tv.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                Uri uri = Uri.parse(Constants.TUTORIAL_VIDEO_URL);
                Intent intent = new Intent(Intent.ACTION_VIEW, uri);
                startActivity(intent);
                return true;
            }
        });
    }

    @Override
    public boolean onTouchEvent(MotionEvent e) {

        switch (e.getAction()) {
            case MotionEvent.ACTION_DOWN:
                mLastX = e.getX();

                Log.i(mName, "Touch event down: " + String.valueOf(mLastX));
                break;
            case MotionEvent.ACTION_UP:
                float currentX = e.getX();

                Log.i(mName, "Touch event up: " + String.valueOf(mLastX) + ", " + String.valueOf(currentX));

                // only handle right-to-left motion
                if(mLastX > currentX) {
                    // flipper contains 8 images
                    if(mViewFlipper.getDisplayedChild() == 7) {
                        break;
                    }

                    mViewFlipper.setInAnimation(this, R.anim.slide_in_from_right);
                    mViewFlipper.setOutAnimation(this, R.anim.slide_out_to_left);
                    mViewFlipper.showNext();
                }
                break;
        }

        return false;
    }
}
