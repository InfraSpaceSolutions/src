package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.text.InputFilter;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.NumberPicker;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.StatisticsParser;
import com.unitethiscity.data.SummaryStats;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

public class StatisticsSummaryFragment extends Fragment {

    private String mName = Constants.STATISTICS_SUMMARY;

    public Constants.MenuType mMenuType = Constants.MenuType.SUB;
    public Constants.MenuID mMenuID = Constants.MenuID.STATISTICS_SUMMARY;
    private Constants.MenuID mParentID;

    private ViewGroup mContainer;
    private View mParent;

    private boolean mFragmentActive = false;

    private String mLocationName;
    private int mLocationID;
    private int mBusinessID;
    private String mBusinessGUID;

    ArrayList<SummaryStats> mSummaryStats;

    private String[] PERIOD_NAMES;

    private int[] STAT_VIEW_IDS = new int[] {
            R.id.statisticsSummaryAmountSocialDeals,
            R.id.statisticsSummaryAmountLoyaltyPoints,
            R.id.statisticsSummaryAmountFavorites,
            R.id.statisticsSummaryAmountRatings,
            R.id.statisticsSummaryAmountReviews
    };

    private Period mPeriod = Period.ALL_TIME;
    private Period mNewPeriod = Period.ALL_TIME;

    private AsyncTask<Integer, Void, Integer> mLoadSummaryStatisticsTask;
    private String mStatisticsRetrievalError;

    public enum StatType {
        SOCIAL_DEALS(0),
        LOYALTY_POINTS(1),
        FAVORITES(2),
        RATINGS(3),
        REVIEWS(4);

        private static final Map<Integer,StatType> lookup
                = new HashMap<Integer,StatType>();

        static {
            for(StatType s : EnumSet.allOf(StatType.class))
                lookup.put(s.getCode(), s);
        }

        private int code;

        StatType(int code) {
            this.code = code;
        }

        public int getCode() { return code; }

        public static StatType get(int code) {
            return lookup.get(code);
        }
    }

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

    public StatisticsSummaryFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        mContainer = container;

        // Inflate the layout for this fragment
        mParent = inflater.inflate(R.layout.fragment_statistics_summary, container, false);
        return mParent;
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putString("mLocationName", mLocationName);
        outState.putInt("mLocationID", mLocationID);
        outState.putInt("mBusinessID", mBusinessID);
        outState.putString("mBusinessGUID", mBusinessGUID);
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        if (!mFragmentActive) return;
        if (isAdded()) ((MainActivity) getActivity()).showSpinner();

        mSummaryStats = new ArrayList<SummaryStats>();

        PERIOD_NAMES = new String[]{
                getString(R.string.statistics_summary_period_all_time),
                getString(R.string.statistics_summary_period_today),
                getString(R.string.statistics_summary_period_past_week),
                getString(R.string.statistics_summary_period_this_period),
                getString(R.string.statistics_summary_period_last_period)
        };

        if(getArguments() != null) {
            mLocationName = getArguments().getString(LocationParser.JSON_TAG_NAME);
            mLocationID = Integer.valueOf(getArguments().getString(LocationParser.JSON_TAG_ID));
            mBusinessID = Integer.valueOf(getArguments().getString(LocationParser.JSON_TAG_BUSID));
            mBusinessGUID = getArguments().getString(LocationParser.JSON_TAG_BUSGUID);
        }
        else if(savedInstanceState != null) {
            mLocationName = savedInstanceState.getString("mLocationName");
            mLocationID = savedInstanceState.getInt("mLocationID");
            mBusinessID = savedInstanceState.getInt("mBusinessID");
            mBusinessGUID = savedInstanceState.getString("mBusinessGUID");
        }

        final Bundle args = new Bundle();
        args.putInt(LocationParser.JSON_TAG_ID, mLocationID);
        args.putInt(LocationParser.JSON_TAG_BUSID, mBusinessID);
        args.putString(LocationParser.JSON_TAG_BUSGUID, mBusinessGUID);

        ImageView analytics = (ImageView) mParent.findViewById(R.id.imageViewStatisticSummaryBusinessAnalytics);
        analytics.setClickable(true);
        analytics.setOnClickListener(new AnalyticsClickListener(args));

        TextView tv = (TextView) mParent.findViewById(R.id.statisticsSummaryName);
        tv.setText(mLocationName);

        tv = (TextView) mParent.findViewById(R.id.statisticsSummaryPeriod);
        tv.setText(PERIOD_NAMES[mPeriod.ordinal()]);

        RelativeLayout rl = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutStatisticsSummarySocialDeals);
        rl.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                DataManager.getInstance().getStatisticsListFragment().setScope(mPeriod.getCode());
                DataManager.getInstance().getStatisticsListFragment().setStatType(StatType.SOCIAL_DEALS);
                replaceSubmenuFragment(Constants.MenuID.STATISTICS_LIST, args, true);
                return false;
            }
        });

        rl = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutStatisticsSummaryLoyaltyPoints);
        rl.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                DataManager.getInstance().getStatisticsListFragment().setScope(mPeriod.getCode());
                DataManager.getInstance().getStatisticsListFragment().setStatType(StatType.LOYALTY_POINTS);
                replaceSubmenuFragment(Constants.MenuID.STATISTICS_LIST, args, true);
                return false;
            }
        });

        rl = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutStatisticsSummaryFavorites);
        rl.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                DataManager.getInstance().getStatisticsListFragment().setScope(mPeriod.getCode());
                DataManager.getInstance().getStatisticsListFragment().setStatType(StatType.FAVORITES);
                replaceSubmenuFragment(Constants.MenuID.STATISTICS_LIST, args, true);
                return false;
            }
        });

        rl = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutStatisticsSummaryRatings);
        rl.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                DataManager.getInstance().getStatisticsListFragment().setScope(mPeriod.getCode());
                DataManager.getInstance().getStatisticsListFragment().setStatType(StatType.RATINGS);
                replaceSubmenuFragment(Constants.MenuID.STATISTICS_LIST, args, true);
                return false;
            }
        });

        rl = (RelativeLayout) mParent.findViewById(R.id.relativeLayoutStatisticsSummaryReviews);
        rl.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                DataManager.getInstance().getStatisticsListFragment().setScope(mPeriod.getCode());
                DataManager.getInstance().getStatisticsListFragment().setStatType(StatType.REVIEWS);
                replaceSubmenuFragment(Constants.MenuID.STATISTICS_LIST, args, true);
                return false;
            }
        });

        Button range = (Button) mParent.findViewById(R.id.buttonStatisticsSummarySelectRange);
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
                                        TextView tv = (TextView) mParent.findViewById(R.id.statisticsSummaryPeriod);
                                        tv.setText(PERIOD_NAMES[mPeriod.ordinal()]);
                                        updateStatisticsPeriod();
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

        mLoadSummaryStatisticsTask = new LoadSummaryStatisticsTask();
        if(Utils.hasHoneycomb()) {
            mLoadSummaryStatisticsTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, mBusinessID);
        }
        else {
            mLoadSummaryStatisticsTask.execute(mLocationID);
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

        // without business statistics permission, proceed to analytics
        if(DataManager.getInstance().hasBusinessAnalyticsPermission() &&
                !DataManager.getInstance().hasBusinessStatisticsPermission()) {
            ((ImageView) mParent.findViewById(R.id.imageViewStatisticSummaryBusinessAnalytics))
                    .performClick();
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
            case STATISTICS_LIST:
                ((MainActivity) getActivity()).setFragmentID(fID);
                ((MainActivity) getActivity()).setParentFragmentID(mMenuID);
                StatisticsListFragment slf = DataManager.getInstance().getStatisticsListFragment();
                slf.setParent(mMenuID);
                slf.fragmentActive(true);
                if(args.isEmpty() == false) {
                    slf.setArguments(args);
                }
                transaction.add(R.id.frameLayoutMiddle, slf, fID.toString());
                break;
            case ANALYTICS_BUSINESS:
                ((MainActivity) getActivity()).setFragmentID(fID);
                ((MainActivity) getActivity()).setParentFragmentID(mMenuID);
                AnalyticsBusinessFragment abf = DataManager.getInstance().getAnalyticsBusinessFragment();
                abf.setParent(mMenuID);
                abf.fragmentActive(true);
                if(args.isEmpty() == false) {
                    abf.setArguments(args);
                }
                transaction.add(R.id.frameLayoutMiddle, abf, fID.toString());
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
        cancelStatisticsTask();
    }

    public void cancelStatisticsTask() {
        mLoadSummaryStatisticsTask.cancel(true);
    }

    private void updateStatisticsPeriod() {
        TextView tv;
        for(int i = 0; i < PERIOD_NAMES.length; i++) {
            if(mSummaryStats != null && i < mSummaryStats.size()) {
                SummaryStats stats = mSummaryStats.get(i);

                tv = (TextView) mParent.findViewById(STAT_VIEW_IDS[i]);
                switch(mPeriod)
                {
                    case ALL_TIME:
                        tv.setText(String.valueOf(stats.getAllTime()));
                        break;
                    case TODAY:
                        tv.setText(String.valueOf(stats.getToday()));
                        break;
                    case PAST_WEEK:
                        tv.setText(String.valueOf(stats.getPastWeek()));
                        break;
                    case THIS_PERIOD:
                        tv.setText(String.valueOf(stats.getThisPeriod()));
                        break;
                    case LAST_PERIOD:
                        tv.setText(String.valueOf(stats.getLastPeriod()));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private class AnalyticsClickListener implements View.OnClickListener {

        public Bundle mArguments;

        public AnalyticsClickListener(Bundle args) {
            mArguments = args;
        }

        @Override
        public void onClick(View v) {
            if(DataManager.getInstance().hasBusinessAnalyticsPermission()) {
                replaceSubmenuFragment(Constants.MenuID.ANALYTICS_BUSINESS, mArguments, true);
            }
            else {
                AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
                builder.setTitle("Access Denied");
                builder.setMessage("Contact us for access to business analytics");
                builder.setPositiveButton(R.string.dialog_warning_ok, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        // do nothing
                    }
                });
                builder.create().show();
            }
        }
    }

    private class LoadSummaryStatisticsTask extends AsyncTask<Integer, Void, Integer> {
        protected Integer doInBackground(Integer... params) {
            if((MainActivity) getActivity() == null) {
                Logger.verbose(mName, "main == null in LoadSummaryStatisticsTask");
                mStatisticsRetrievalError = "An error occurred when retrieving " +
                        "summary statistics information";
                return 0;
            }

            // bail if this task was canceled early
            if(isCancelled() == true) {
                Logger.verbose(mName, "isCancelled == true in LoadSummaryStatisticsTask");
                mStatisticsRetrievalError = "An error occurred when retrieving " +
                        "summary statistics information";
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setSummaryStats(params[0]);

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setStatRedemptions(params[0], mPeriod.getCode());

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setStatCheckIns(params[0], mPeriod.getCode());

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setStatFavorites(params[0]);

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setStatRatings(params[0]);

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            mStatisticsRetrievalError = StatisticsParser.setStatTips(params[0]);

            if(mStatisticsRetrievalError != null) {
                return 0;
            }

            return null;
        }

        protected void onPostExecute(Integer error) {

            if(mStatisticsRetrievalError != null) {
                Toast.makeText(mParent.getContext(), mStatisticsRetrievalError, Toast.LENGTH_SHORT).show();
                return;
            }

            DataManager dm = DataManager.getInstance();
            mSummaryStats = dm.getSummaryStats();
            updateStatisticsPeriod();

            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }
}
