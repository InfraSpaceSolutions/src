package com.unitethiscity.ui;


import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.text.InputFilter;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.NumberPicker;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.AnalyticsParser;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.SummaryAnalytics;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import java.lang.reflect.Field;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

/**
 * A simple {@link Fragment} subclass.
 */
public class AnalyticsSummaryFragment extends Fragment {

    private String mName = Constants.ANALYTICS_SUMMARY;

    public Constants.MenuType mMenuType = Constants.MenuType.SUB;
    public Constants.MenuID mMenuID = Constants.MenuID.ANALYTICS_SUMMARY;
    private Constants.MenuID mParentID;

    private ViewGroup mContainer;
    private View mParent;

    private int mID;

    ArrayList<SummaryAnalytics> mSummaryAnalytics;

    private String[] PERIOD_NAMES;

    private boolean mFragmentActive = false;

    private AsyncTask<Integer, Void, Integer> mLoadSummaryAnalyticsTask;
    private String mAnalyticsRetrievalError;

    private Period mPeriod = Period.TODAY;
    private Period mNewPeriod = Period.TODAY;

    public enum Period {
        ALL_TIME(0),
        TODAY(1),
        PAST_WEEK(2),
        THIS_PERIOD(3),
        LAST_PERIOD(4);

        private static final Map<Integer,Period> lookup
                = new HashMap<Integer,Period>();

        static {
            for(Period p : EnumSet.allOf(Period.class))
                lookup.put(p.getCode(), p);
        }

        private int code;

        Period(int code) {
            this.code = code;
        }

        public int getCode() { return code; }

        public static Period get(int code) {
            return lookup.get(code);
        }
    }

    public AnalyticsSummaryFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        mContainer = container;

        // Inflate the layout for this fragment
        mParent = inflater.inflate(R.layout.fragment_analytics_summary, container, false);
        return mParent;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        if (!mFragmentActive) return;
        if (isAdded()) ((MainActivity) getActivity()).showSpinner();

        PERIOD_NAMES = new String[]{
                getString(R.string.statistics_summary_period_all_time),
                getString(R.string.statistics_summary_period_today),
                getString(R.string.statistics_summary_period_past_week),
                getString(R.string.statistics_summary_period_this_period),
                getString(R.string.statistics_summary_period_last_period)
        };

        mID = mPeriod.getCode();

        TextView tv = (TextView) mParent.findViewById(R.id.analyticsRange);
        tv.setText(PERIOD_NAMES[mPeriod.ordinal()]);

        Button range = (Button) mParent.findViewById(R.id.buttonAnalyticsSummarySelectRange);
        range.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                NumberPicker rangePicker = new NumberPicker(getActivity());

                rangePicker.setMinValue(Period.ALL_TIME.ordinal());
                rangePicker.setMaxValue(Period.LAST_PERIOD.ordinal());
                rangePicker.setFormatter(new NumberPicker.Formatter() {
                    @Override
                    public String format(int value) {
                        return PERIOD_NAMES[value];
                    }
                });
                rangePicker.setOnValueChangedListener(new NumberPicker.OnValueChangeListener() {
                    @Override
                    public void onValueChange(NumberPicker picker, int oldVal, int newVal) {
                        mNewPeriod = Period.get(newVal);
                    }
                });
                rangePicker.setDescendantFocusability(ViewGroup.FOCUS_BLOCK_DESCENDANTS);

                mNewPeriod = Period.ALL_TIME;

                AlertDialog rangePickerDialog = new AlertDialog.Builder(getActivity())
                        .setTitle("Select Range")
                        .setView(rangePicker)
                        .setPositiveButton(R.string.dialog_ok,
                                new DialogInterface.OnClickListener() {
                                    public void onClick(DialogInterface dialog, int whichButton) {
                                        mPeriod = mNewPeriod;
                                        TextView tv = (TextView) mParent.findViewById(R.id.analyticsRange);
                                        tv.setText(PERIOD_NAMES[mPeriod.ordinal()]);
                                        mID = mPeriod.getCode();
                                        ((MainActivity) getActivity()).showSpinner();
                                        mLoadSummaryAnalyticsTask = new LoadSummaryAnalyticsTask();
                                        if(Utils.hasHoneycomb()) {
                                            mLoadSummaryAnalyticsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mID);
                                        }
                                        else {
                                            mLoadSummaryAnalyticsTask.execute(mID);
                                        }
                                    }
                                })
                        .setNegativeButton(R.string.dialog_cancel,
                                new DialogInterface.OnClickListener() {
                                    public void onClick(DialogInterface dialog, int whichButton) {

                                    }
                                }).create();

                Field f = null;
                try {
                    f = NumberPicker.class.getDeclaredField("mInputText");
                }
                catch(NoSuchFieldException ex) {
                    ex.printStackTrace();
                }
                if(f != null) {
                    f.setAccessible(true);
                    try {
                        EditText inputText = (EditText)f.get(rangePicker);
                        inputText.setFilters(new InputFilter[0]);
                    }
                    catch(IllegalAccessException ex) {
                        ex.printStackTrace();
                    }
                }

                rangePickerDialog.show();
            }
        });

        mLoadSummaryAnalyticsTask = new LoadSummaryAnalyticsTask();
        if(Utils.hasHoneycomb()) {
            mLoadSummaryAnalyticsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mID);
        }
        else {
            mLoadSummaryAnalyticsTask.execute(mID);
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

    public boolean isFragmentActive() {
        return mFragmentActive;
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

    public void setParent(Constants.MenuID fID) {
        mParentID = fID;
    }

    public Constants.MenuID getParent() {
        return mParentID;
    }

    public void replaceSubmenuFragment(Constants.MenuID fID, Bundle args, boolean goBack)
    {
        final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
        // Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
        vib.vibrate(Constants.VIBRATE_LENGTH);

        FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

        mFragmentActive = false;
        hide();

        // Replace whatever is in the frameLayout view with this fragment
        switch (fID) {
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
        cancelAnalyticsTask();
    }

    public void cancelAnalyticsTask() {
        mLoadSummaryAnalyticsTask.cancel(true);
    }

    private void updateAnalytics() {
        LinearLayout analytics = (LinearLayout) mParent.findViewById(R.id.linearLayoutAnalyticsResults);
        analytics.removeAllViewsInLayout();

        LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        int idNum = Constants.BASE_CUSTOM_IDS + 2000;
        int idOffset = 0;

        Logger.info(mName, "Adding " + String.valueOf(mSummaryAnalytics.size()) + " analytics for ID " + String.valueOf(mID));

        NumberFormat formatter = new DecimalFormat("#0.00");
        for(int i = 0; i < mSummaryAnalytics.size(); i++) {
            SummaryAnalytics analytic = mSummaryAnalytics.get(i);
            View child = null;
            try {
                child = inflater.inflate(R.layout.analytic_item, null, false);
            }
            catch(InflateException ie) {
                Logger.error(mName, "could not inflate child view");
                throw new RuntimeException(ie);
            }

            child.setId(idNum + idOffset++);

            // alternate colors for grouping - #ffffff and ##d8d8d8
            if(analytic.getGroup() % 2 == 0) {
                (child.findViewById(R.id.relativeLayoutAnalyticItem)).setBackgroundColor(Color.parseColor("#d8d8d8"));
            }

            TextView tv = (TextView) child.findViewById(R.id.analyticName);
            tv.setText(analytic.getName());
            tv.setId(idNum + idOffset++);

            tv = (TextView) child.findViewById(R.id.analyticTotal);
            tv.setText(String.valueOf(analytic.getTotal()));
            tv.setId(idNum + idOffset++);

            double percent = analytic.getPercent();
            if(percent >= 1.0) {
                percent = 1.0;
            }
            tv = (TextView) child.findViewById(R.id.analyticPercent);
            tv.setText(String.valueOf(formatter.format(percent * 100)) + "%");
            tv.setId(idNum + idOffset++);

            analytics.addView(child);
        }
    }

    private class LoadSummaryAnalyticsTask extends AsyncTask<Integer, Void, Integer> {
        protected Integer doInBackground(Integer... params) {
            if((MainActivity) getActivity() == null) {
                Logger.verbose(mName, "main == null in LoadSummaryStatisticsTask");
                mAnalyticsRetrievalError = "An error occurred when retrieving " +
                        "summary statistics information";
                return 0;
            }

            // bail if this task was canceled early
            if(isCancelled() == true) {
                Logger.verbose(mName, "isCancelled == true in LoadSummaryStatisticsTask");
                mAnalyticsRetrievalError = "An error occurred when retrieving " +
                        "summary statistics information";
                return 0;
            }

            mAnalyticsRetrievalError = AnalyticsParser.setSummaryAnalytics(params[0]);

            if(mAnalyticsRetrievalError != null) {
                return 0;
            }

            return null;
        }

        protected void onPostExecute(Integer error) {

            if(mAnalyticsRetrievalError != null) {
                Toast.makeText(mParent.getContext(), mAnalyticsRetrievalError, Toast.LENGTH_SHORT).show();
                return;
            }

            DataManager dm = DataManager.getInstance();
            mSummaryAnalytics = dm.getSummaryAnalytics();
            updateAnalytics();

            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
}
