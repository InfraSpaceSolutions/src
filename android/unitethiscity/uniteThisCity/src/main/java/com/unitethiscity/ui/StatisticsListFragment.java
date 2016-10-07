package com.unitethiscity.ui;

import android.annotation.TargetApi;
import android.content.Context;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.view.InflateException;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LocationParser;
import com.unitethiscity.data.LoginManager;
import com.unitethiscity.data.StatCheckIn;
import com.unitethiscity.data.StatFavorite;
import com.unitethiscity.data.StatRating;
import com.unitethiscity.data.StatRedemption;
import com.unitethiscity.data.StatTip;
import com.unitethiscity.data.StatisticsParser;
import com.unitethiscity.data.SummaryCheckIn;
import com.unitethiscity.data.SummaryRedemption;
import com.unitethiscity.data.UTCWebAPI;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

import org.json.JSONArray;

import java.text.DecimalFormat;
import java.text.NumberFormat;

public class StatisticsListFragment extends Fragment {

    private String mName = Constants.STATISTICS_LIST;

    public Constants.MenuType mMenuType = Constants.MenuType.SUB;
    public Constants.MenuID mMenuID = Constants.MenuID.STATISTICS_LIST;
    private Constants.MenuID mParentID;

    private ViewGroup mContainer;
    private View mParent;

    private boolean mFragmentActive = false;

    private int mLocationID = 0;
    private int mBusinessID = 0;
    private int mAccountID = 0;
    private String mBusinessGUID = "";

    private int mScope = 0;

    private StatisticsSummaryFragment.StatType mStatType = StatisticsSummaryFragment.StatType.SOCIAL_DEALS;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Logger.verbose(mName, "onCreate()");
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Logger.verbose(mName, "onCreateView()");

        mContainer = container;

        // Inflate the layout for this fragment
        mParent = inflater.inflate(R.layout.fragment_statistics_list, container, false);

        return mParent;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Logger.verbose(mName, "onActivityCreated()");

        mLocationID = getArguments().getInt(LocationParser.JSON_TAG_ID);
        mBusinessID = getArguments().getInt(LocationParser.JSON_TAG_BUSID);
        mBusinessGUID = getArguments().getString(LocationParser.JSON_TAG_BUSGUID);

        ImageView analytics = (ImageView) mParent.findViewById(R.id.imageViewStatisticListBusinessAnalytics);
        analytics.setClickable(true);
        Bundle args = new Bundle();
        args.putInt(LocationParser.JSON_TAG_BUSID, mBusinessID);
        analytics.setOnClickListener(new AnalyticsClickListener(args));

        if(mFragmentActive == true) {
            ((MainActivity) getActivity()).clearBackPressed();
            if(isAdded()) ((MainActivity) getActivity()).showSpinner();
            LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
            statsList.removeAllViewsInLayout();
            LinearLayout usersList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);
            usersList.removeAllViewsInLayout();
            if(mStatType == StatisticsSummaryFragment.StatType.SOCIAL_DEALS || mStatType == StatisticsSummaryFragment.StatType.LOYALTY_POINTS) {
                LoadUsersTask loadUsers = new LoadUsersTask();
                if (Utils.hasHoneycomb()) {
                    loadUsers.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                } else {
                    loadUsers.execute();
                }
            }
            else {
                LoadStatisticsTask loadStats = new LoadStatisticsTask();
                if (Utils.hasHoneycomb()) {
                    loadStats.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                } else {
                    loadStats.execute();
                }
            }
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
        if(fID != Constants.MenuID.HOME) {
            final Vibrator vib = (Vibrator) getActivity().getSystemService(Context.VIBRATOR_SERVICE);
            // Vibrate for LocationParser.VIBRATE_LENGTH milliseconds
            vib.vibrate(Constants.VIBRATE_LENGTH);
        }

        FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

        mFragmentActive = false;
        hide();

        // Replace whatever is in the frameLayout view with this fragment
        switch (fID) {
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

    public void removeAllStatisticResults() {
        LinearLayout results = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
        results.removeAllViews();
    }

    public void setStatType(StatisticsSummaryFragment.StatType type) {
        mStatType = type;
    }

    public void setScope(int scope) { mScope = scope; }

    public StatisticsSummaryFragment.StatType getStatType() {
        return mStatType;
    }

    private class AnalyticsClickListener implements View.OnClickListener {

        public Bundle mArguments;

        public AnalyticsClickListener(Bundle args) {
            mArguments = args;
        }

        @Override
        public void onClick(View v) {
            replaceSubmenuFragment(Constants.MenuID.ANALYTICS_BUSINESS, mArguments, true);
        }
    }

    private class LoadStatisticsTask extends AsyncTask<Void, View, Void> {
        protected Void doInBackground(Void... params) {
            if(mParent != null) {

                DataManager dm = DataManager.getInstance();
                int numOfStats = 0;

                String url = Constants.LOCATION_INFO_IMAGE + "/" + mBusinessGUID + "@2x.png";

                View child = null;
                TextView name;
                TextView nameExtra;
                TextView info1;
                TextView info2;
                Button bottomInfo1;
                Button bottomInfo2;
                ImageView logo;
                ImageView ratingBar;

                LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);

                if(mStatType.equals(StatisticsSummaryFragment.StatType.SOCIAL_DEALS)) {
                    StatisticsParser.setStatRedemptions(mBusinessID, mScope, mAccountID);

                    numOfStats = dm.getStatRedemptions().size();
                    Logger.verbose(mName, "number of social deals - " + String.valueOf(numOfStats));
                    for(int i = 0; i < numOfStats; i++) {
                        try {
                            child = inflater.inflate(R.layout.statistic_details, null, false);
                            logo = (ImageView) child.findViewById(R.id.imageViewStatisticDetail);
                            name = (TextView) child.findViewById(R.id.statisticName);
                            nameExtra = (TextView) child.findViewById(R.id.statisticNameExtra);
                            info1 = (TextView) child.findViewById(R.id.statisticInformation1);
                            info2 = (TextView) child.findViewById(R.id.statisticInformation2);
                            bottomInfo1 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation1);
                            bottomInfo2 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation2);

                            StatRedemption redemption = dm.getStatRedemptions().get(i);

                            addImage(url, logo);

                            String dealAmountStr;
                            double dealAmount = redemption.getAmount();
                            if(dealAmount <= 0) {
                                dealAmountStr = "N/A";
                            }
                            else {
                                int dealAmountInt = (int) dealAmount;
                                if(dealAmountInt == dealAmount) {
                                    dealAmountStr = String.valueOf(dealAmountInt);
                                    dealAmountStr = "$" + dealAmountStr;
                                }
                                else {
                                    dealAmountStr = NumberFormat.getCurrencyInstance().format(dealAmount);
                                }
                            }

                            name.setText(redemption.getName());
                            nameExtra.setText(redemption.getPin());
                            info1.setText(redemption.getDeal());
                            bottomInfo1.setText(redemption.getTimestampAsString());
                            bottomInfo2.setText(dealAmountStr);

                            publishProgress(child);
                        } catch (InflateException ie) {
                            Logger.error(mName, "could not inflate child view");
                            throw new RuntimeException(ie);
                        }
                    }
                }
                else if(mStatType.equals(StatisticsSummaryFragment.StatType.LOYALTY_POINTS)) {
                    StatisticsParser.setStatCheckIns(mBusinessID, mScope, mAccountID);

                    numOfStats = dm.getStatCheckIns().size();
                    Logger.verbose(mName, "number of loyalty points - " + String.valueOf(numOfStats));
                    for(int i = 0; i < numOfStats; i++) {
                        try {
                            child = inflater.inflate(R.layout.statistic_details, null, false);
                            logo = (ImageView) child.findViewById(R.id.imageViewStatisticDetail);
                            name = (TextView) child.findViewById(R.id.statisticName);
                            nameExtra = (TextView) child.findViewById(R.id.statisticNameExtra);
                            info1 = (TextView) child.findViewById(R.id.statisticInformation1);
                            info2 = (TextView) child.findViewById(R.id.statisticInformation2);
                            bottomInfo1 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation1);
                            bottomInfo2 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation2);

                            StatCheckIn checkIn = dm.getStatCheckIns().get(i);

                            addImage(url, logo);

                            name.setText(checkIn.getName());
                            info1.setText(checkIn.getLocationName());
                            bottomInfo1.setText(checkIn.getTimestampAsString());
                            bottomInfo2.setText("1 POINT");

                            publishProgress(child);
                        } catch (InflateException ie) {
                            Logger.error(mName, "could not inflate child view");
                            throw new RuntimeException(ie);
                        }
                    }
                }
                else if(mStatType.equals(StatisticsSummaryFragment.StatType.FAVORITES)) {
                    ((TextView)mParent.findViewById(R.id.crumbStatisticsList)).setText("FAVORITES");
                    numOfStats = dm.getStatFavorites().size();
                    Logger.verbose(mName, "number of favorites - " + String.valueOf(numOfStats));
                    for(int i = 0; i < numOfStats; i++) {
                        try {
                            child = inflater.inflate(R.layout.statistic_details, null, false);
                            logo = (ImageView) child.findViewById(R.id.imageViewStatisticDetail);
                            name = (TextView) child.findViewById(R.id.statisticName);
                            nameExtra = (TextView) child.findViewById(R.id.statisticNameExtra);
                            info1 = (TextView) child.findViewById(R.id.statisticInformation1);
                            info2 = (TextView) child.findViewById(R.id.statisticInformation2);
                            bottomInfo1 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation1);
                            bottomInfo2 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation2);

                            StatFavorite favorite = dm.getStatFavorites().get(i);

                            addImage(url, logo);

                            name.setText(favorite.getName());
                            info1.setText(favorite.getLocationName());
                            bottomInfo1.setText(favorite.getTimestampAsString());
                            bottomInfo2.setText("FAVORITE");

                            publishProgress(child);
                        } catch (InflateException ie) {
                            Logger.error(mName, "could not inflate child view");
                            throw new RuntimeException(ie);
                        }
                    }
                }
                else if(mStatType.equals(StatisticsSummaryFragment.StatType.RATINGS)) {
                    ((TextView)mParent.findViewById(R.id.crumbStatisticsList)).setText("RATINGS");
                    numOfStats = dm.getStatRatings().size();
                    Logger.verbose(mName, "number of ratings - " + String.valueOf(numOfStats));
                    for(int i = 0; i < numOfStats; i++) {
                        try {
                            child = inflater.inflate(R.layout.statistic_details, null, false);
                            logo = (ImageView) child.findViewById(R.id.imageViewStatisticDetail);
                            name = (TextView) child.findViewById(R.id.statisticName);
                            nameExtra = (TextView) child.findViewById(R.id.statisticNameExtra);
                            info1 = (TextView) child.findViewById(R.id.statisticInformation1);
                            info2 = (TextView) child.findViewById(R.id.statisticInformation2);
                            bottomInfo1 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation1);
                            bottomInfo2 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation2);
                            ratingBar = (ImageView) child.findViewById(R.id.imageViewStatisticRating);

                            StatRating rating = dm.getStatRatings().get(i);

                            addImage(url, logo);

                            name.setText(rating.getName());
                            info1.setText(rating.getLocationName());
                            bottomInfo1.setText(rating.getTimestampAsString());
                            bottomInfo2.setText(String.valueOf(rating.getRating()));
                            bottomInfo2.setText("RATING");
                            ratingBar.setVisibility(ImageView.VISIBLE);
                            int ratingNumber = rating.getRating() * 2;
                            if (ratingNumber > 10) {
                                ratingNumber = 10;
                            }
                            Drawable ratingDrawable = getResources().getDrawable(
                                    Constants.ratingIds[ratingNumber]
                            );
                            ratingBar.setImageDrawable(ratingDrawable);

                            publishProgress(child);
                        } catch (InflateException ie) {
                            Logger.error(mName, "could not inflate child view");
                            throw new RuntimeException(ie);
                        }
                    }
                }
                else if(mStatType.equals(StatisticsSummaryFragment.StatType.REVIEWS)) {
                    ((TextView)mParent.findViewById(R.id.crumbStatisticsList)).setText("REVIEWS");
                    numOfStats = dm.getStatTips().size();
                    Logger.verbose(mName, "number of reviews - " + String.valueOf(numOfStats));
                    for(int i = 0; i < numOfStats; i++) {
                        try {
                            child = inflater.inflate(R.layout.statistic_details, null, false);
                            logo = (ImageView) child.findViewById(R.id.imageViewStatisticDetail);
                            name = (TextView) child.findViewById(R.id.statisticName);
                            nameExtra = (TextView) child.findViewById(R.id.statisticNameExtra);
                            info1 = (TextView) child.findViewById(R.id.statisticInformation1);
                            info2 = (TextView) child.findViewById(R.id.statisticInformation2);
                            bottomInfo1 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation1);
                            bottomInfo2 = (Button) child.findViewById(R.id.buttonStatisticBottomInformation2);

                            StatTip tip = dm.getStatTips().get(i);

                            addImage(url, logo);

                            final String nameStr = tip.getName();
                            final String tipStr = tip.getTipText();

                            name.setText(nameStr);
                            info1.setText(tip.getTipText());
                            bottomInfo1.setText(tip.getTimestampAsString());
                            bottomInfo2.setText("REVIEW");

                            child.setClickable(true);
                            child.setOnClickListener(new View.OnClickListener() {
                                @Override
                                public void onClick(View v) {
                                    ReviewViewerDialog.newInstance(nameStr, tipStr).show(getFragmentManager());
                                }
                            });

                            publishProgress(child);
                        } catch (InflateException ie) {
                            Logger.error(mName, "could not inflate child view");
                            throw new RuntimeException(ie);
                        }
                    }
                }
            }

            return null;
        }

        @Override
        protected void onProgressUpdate(View... child) {
            LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);

            if(isAdded()) {
                statsList.addView(child[0]);
            }
        }

        protected void onPostExecute(Void param) {
            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }

    private class LoadUsersTask extends AsyncTask<Void, View, Void> {
        protected Void doInBackground(Void... params) {
            if(mParent != null) {

                DataManager dm = DataManager.getInstance();

                final String url = Constants.LOCATION_INFO_IMAGE + "/" + mBusinessGUID + "@2x.png";

                final LayoutInflater inflater = (LayoutInflater) mParent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);

                if(mStatType.equals(StatisticsSummaryFragment.StatType.SOCIAL_DEALS)) {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ((TextView) mParent.findViewById(R.id.crumbStatisticsList)).setText("SOCIAL DEALS");

                            LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
                            statsList.setVisibility(LinearLayout.GONE);
                            LinearLayout usersList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);
                            usersList.setVisibility(LinearLayout.VISIBLE);
                        }
                    });

                    StatisticsParser.setSummaryRedemptions(mBusinessID, mScope);

                    int numOfSumRedemptions = dm.getSummaryRedemptions().size();
                    for(int i = 0; i < numOfSumRedemptions; i++) {
                        View summaryChild = inflater.inflate(R.layout.summary_item, null, false);

                        ImageView summaryLogo = (ImageView) summaryChild.findViewById(R.id.imageViewSummaryLogo);
                        TextView summaryName = (TextView) summaryChild.findViewById(R.id.textViewSummaryName);
                        TextView summaryValue = (TextView) summaryChild.findViewById(R.id.textViewSummaryValue);

                        addImage(url, summaryLogo);

                        SummaryRedemption sumRedemption = dm.getSummaryRedemptions().get(i);

                        summaryName.setText(sumRedemption.getName());
                        String sumAmountStr;
                        double sumAmount = sumRedemption.getSum();
                        int sumAmountInt = (int) sumAmount;
                        if(sumAmountInt == sumAmount) {
                            sumAmountStr = String.valueOf(sumAmountInt);
                            sumAmountStr = "$" + sumAmountStr;
                        }
                        else {
                            sumAmountStr = NumberFormat.getCurrencyInstance().format(sumAmount);
                        }
                        summaryValue.setText(sumAmountStr + " (" + sumRedemption.getCount() + ")");

                        final int accID = sumRedemption.getAccountID();
                        summaryChild.setClickable(true);
                        summaryChild.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                mAccountID = accID;

                                LinearLayout usersList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);
                                usersList.setVisibility(LinearLayout.GONE);
                                LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
                                statsList.setVisibility(LinearLayout.VISIBLE);

                                getActivity().runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        LoadStatisticsTask loadStats = new LoadStatisticsTask();
                                        if (Utils.hasHoneycomb()) {
                                            loadStats.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                                        } else {
                                            loadStats.execute();
                                        }
                                    }
                                });
                            }
                        });

                        publishProgress(summaryChild);
                    }
                }
                else if(mStatType.equals(StatisticsSummaryFragment.StatType.LOYALTY_POINTS)) {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ((TextView) mParent.findViewById(R.id.crumbStatisticsList)).setText("LOYALTY DEALS");

                            LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
                            statsList.setVisibility(LinearLayout.GONE);
                            LinearLayout usersList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);
                            usersList.setVisibility(LinearLayout.VISIBLE);
                        }
                    });

                    StatisticsParser.setSummaryCheckIns(mBusinessID, mScope);

                    int numOfSumCheckIns = dm.getSummaryCheckIns().size();
                    for(int i = 0; i < numOfSumCheckIns; i++) {
                        View summaryChild = inflater.inflate(R.layout.summary_item, null, false);

                        ImageView summaryLogo = (ImageView) summaryChild.findViewById(R.id.imageViewSummaryLogo);
                        TextView summaryName = (TextView) summaryChild.findViewById(R.id.textViewSummaryName);
                        TextView summaryValue = (TextView) summaryChild.findViewById(R.id.textViewSummaryValue);

                        addImage(url, summaryLogo);

                        SummaryCheckIn sumCheckIn = dm.getSummaryCheckIns().get(i);

                        summaryName.setText(sumCheckIn.getName());
                        summaryValue.setText(String.valueOf(sumCheckIn.getCount()));

                        final int accID = sumCheckIn.getAccountID();
                        summaryChild.setClickable(true);
                        summaryChild.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                mAccountID = accID;

                                LinearLayout usersList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);
                                usersList.setVisibility(LinearLayout.GONE);
                                LinearLayout statsList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsList);
                                statsList.setVisibility(LinearLayout.VISIBLE);

                                getActivity().runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        LoadStatisticsTask loadStats = new LoadStatisticsTask();
                                        if (Utils.hasHoneycomb()) {
                                            loadStats.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                                        } else {
                                            loadStats.execute();
                                        }
                                    }
                                });
                            }
                        });

                        publishProgress(summaryChild);
                    }
                }
            }

            return null;
        }

        @Override
        protected void onProgressUpdate(View... child) {
            LinearLayout userList = (LinearLayout) mParent.findViewById(R.id.linearLayoutStatisticsUsers);

            if(isAdded()) {
                userList.addView(child[0]);
            }
        }

        protected void onPostExecute(Void param) {
            if(isAdded()) ((MainActivity) getActivity()).hideSpinner();
        }
    }

    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
    private void addImage(String url, ImageView iv) {
        AddImageTask ait = new AddImageTask(iv);
        if(Utils.hasHoneycomb()) {
            ait.executeOnExecutor(AsyncTask.SERIAL_EXECUTOR, url);
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
}
