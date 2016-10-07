package com.unitethiscity.data;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Stack;

import android.content.Context;
import android.location.Location;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.os.SystemClock;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;
import com.unitethiscity.ui.AccountFragment;
import com.unitethiscity.ui.AnalyticsBusinessFragment;
import com.unitethiscity.ui.AnalyticsSummaryFragment;
import com.unitethiscity.ui.BusinessFragment;
import com.unitethiscity.ui.EventFragment;
import com.unitethiscity.ui.FavoriteFragment;
import com.unitethiscity.ui.InboxFragment;
import com.unitethiscity.ui.MainActivity;
import com.unitethiscity.ui.MainFragment;
import com.unitethiscity.ui.RedeemFragment;
import com.unitethiscity.ui.SearchBusinessFragment;
import com.unitethiscity.ui.SearchCategoriesFragment;
import com.unitethiscity.ui.SearchEventFragment;
import com.unitethiscity.ui.SearchFragment;
import com.unitethiscity.ui.StatisticsListFragment;
import com.unitethiscity.ui.StatisticsSummaryFragment;
import com.unitethiscity.ui.SubscribeFragment;
import com.unitethiscity.ui.UTCFragment;
import com.unitethiscity.ui.WalletFragment;
import com.unitethiscity.ui.WebFragment;

import org.json.JSONException;
import org.json.JSONObject;

public class DataManager {

	private String mName = Constants.DATA_MANAGER;
	private static DataManager mInstance = null;

	private Context mMainActivityContext;
	private boolean mMainActivityActive;
	private int mMenuFromNotification;

	private Stack<Constants.MenuID> mMenuStack;
	private Constants.MenuID mLastMenuPopped;

	private APIVersion mAPIVersion;

	// fragment instances
	private MainFragment mMain;
	private WalletFragment mWallet;
	private FavoriteFragment mFavorite;
	private UTCFragment mUTC;
	private InboxFragment mInbox;
	private SearchFragment mSearch;
	private SearchBusinessFragment mSearchBusiness;
	private SearchEventFragment mSearchEvent;
	private SearchCategoriesFragment mSearchCategories;
	private RedeemFragment mRedeem;
	private EventFragment mEvent;
	private AccountFragment mAccount;
	private WebFragment mWeb;
	private BusinessFragment mBusiness;
	private SubscribeFragment mSubscribe;
	private StatisticsSummaryFragment mStatisticsSummary;
	private StatisticsListFragment mStatisticsList;
	private AnalyticsSummaryFragment mAnalyticsSummary;
	private AnalyticsBusinessFragment mAnalyticsBusiness;

	// UTC-specific data structures containing information from API calls
	private HashMap<Integer, UTCLocation> mLocations;
	private UTCLocation mLocationContext;
	private HashMap<Integer, UTCEvent> mEvents;
	private ArrayList<Integer> mEventIDs;
	private UTCEvent mEventContext;
	private HashMap<Integer, UTCPushToken> mPushTokens;
	private HashMap<String, String> mWalletData;
	private ArrayList<Integer> mLocationIDs;
	private ArrayList<Integer> mFavorites;
	private ArrayList<UTCMessage> mMessages;
	private UTCMessage mCurrentMessage;
	private ArrayList<SummaryStats> mSummaryStats;
	private ArrayList<StatCheckIn> mStatCheckIns;
	private ArrayList<StatFavorite> mStatFavorites;
	private ArrayList<StatRating> mStatRatings;
	private ArrayList<StatRedemption> mStatRedemptions;
	private ArrayList<StatTip> mStatTips;
	private ArrayList<SummaryAnalytics> mSummaryAnalytics;
	private ArrayList<SummaryAnalytics> mSummaryAnalyticsBusiness;
	private ArrayList<SummaryRedemption> mSummaryRedemptions;
	private ArrayList<SummaryCheckIn> mSummaryCheckIns;
	private int mBusinessScanLocationID;
	private UTCDataRevision mRevisionLocationInfo;
	private UTCDataRevision mRevisionCategories;
	private UTCDataRevision mRevisionDeals;
	private UTCDataRevision mRevisionEventInfo;
	private boolean mHasGlobalStatistics;
	private boolean mHasGlobalAnalytics;
	private boolean mHasBusinessStatistics;
	private boolean mHasBusinessAnalytics;

	// check-in/redeem related
	private AsyncTask<String, Void, Boolean> mCheckInOrRedeemTask;
	private boolean mRequestingPIN;
	private String mPIN;

	// location-related objects and primitives
	private Location mGPSLocation;
	private Location mNetworkLocation;
	private Location mPassiveLocation;
	private Location mLastProximityLocation;
	private boolean mLocationReady;
	private boolean mNeedLocationUpdate;
	private boolean mNeedLocalUpdate;
	private boolean mWalletNeedsUpdate;

	private long mGPSDwell;
	private long mCategoryTimestamp;
	private long mMessagesTimestamp;
	private long mEventsTimestamp;

	// scanner-related
	private String mScanData;
	private ScanTask mScanTask;

	// category-related
	private CategoryContext mCategoryContext;
	private boolean mCategoryFilterUpdate;

	// Google analytics-related
	private boolean mAnalyticsEnabled;

	// GCM-related
	private boolean mUnregistering;
	private long mUnregisterTimestamp;

	// social-related
	private int mReminderCount;

	// context of UTC fragment
	private boolean mFromBigButton;

	public class APIVersion {

		public final static String MAJOR = "Major";
		public final static String MINOR = "Minor";
		public final static String PATCH = "Patch";

		private int mMajor;
		private int mMinor;
		private int mPatch;

		public APIVersion(int major, int minor, int patch) {
			mMajor = major;
			mMinor = minor;
			mPatch = patch;
		}

		public boolean versionCheck(int major, int minor) {
			return mMajor >= major && mMinor >= minor;
		}

		public int getMajor() {
			return mMajor;
		}

		public int getMinor() {
			return mMinor;
		}

		public int getPatch() {
			return mPatch;
		}
	}

	public enum ScanTask {
		REDEEM(0), CHECKIN(1), UNIFIED_ACTION(2);

		private final int value;

		private ScanTask(int v) {
			value = v;
		}

		public int getValue() {
			return value;
		}
	}

	public DataManager() {
		mMainActivityContext = null;
		mMainActivityActive = true;
		mMenuFromNotification = -2;
		mMenuStack = new Stack<Constants.MenuID>();
		mLastMenuPopped = Constants.MenuID.EMPTY;
		mGPSLocation = new Location(LocationManager.GPS_PROVIDER);
		mNetworkLocation = new Location(LocationManager.NETWORK_PROVIDER);
		mPassiveLocation = new Location(LocationManager.PASSIVE_PROVIDER);
		mLocationReady = false;
		mNeedLocationUpdate = true;
		mNeedLocalUpdate = false;
		mWalletNeedsUpdate = true;
		mGPSDwell = 0L;
		mCategoryTimestamp = 0L;
		mMessagesTimestamp = 0L;
		mLocations = new HashMap<Integer, UTCLocation>();
		mLocationIDs = new ArrayList<Integer>();
		mLocationContext = new UTCLocation();
		mEvents = new HashMap<Integer, UTCEvent>();
		mEventIDs = new ArrayList<Integer>();
		mEventContext = new UTCEvent();
		mPushTokens = new HashMap<Integer, UTCPushToken>();
		mWalletData = new HashMap<String, String>();
		mFavorites = new ArrayList<Integer>();
		mMessages = new ArrayList<UTCMessage>();
		mCurrentMessage = null;
		mSummaryStats = new ArrayList<SummaryStats>();
		mStatCheckIns = new ArrayList<StatCheckIn>();
		mStatFavorites = new ArrayList<StatFavorite>();
		mStatRatings = new ArrayList<StatRating>();
		mStatRedemptions = new ArrayList<StatRedemption>();
		mStatTips = new ArrayList<StatTip>();
		mSummaryAnalytics = new ArrayList<SummaryAnalytics>();
		mSummaryAnalyticsBusiness = new ArrayList<SummaryAnalytics>();
		mSummaryRedemptions = new ArrayList<SummaryRedemption>();
		mSummaryCheckIns = new ArrayList<SummaryCheckIn>();
		mBusinessScanLocationID = -1;
		mRevisionLocationInfo = new UTCDataRevision();
		mRevisionCategories = new UTCDataRevision();
		mRevisionDeals = new UTCDataRevision();
		mRevisionEventInfo = new UTCDataRevision();
		mHasGlobalStatistics = false;
		mHasGlobalAnalytics = false;
		mHasBusinessStatistics = false;
		mHasBusinessAnalytics = false;
		mRequestingPIN = false;
		mPIN = "";
		mScanTask = ScanTask.REDEEM;
		mCategoryContext = new CategoryContext();
		mCategoryFilterUpdate = false;
		mAnalyticsEnabled = true;
		mUnregistering = false;
		mUnregisterTimestamp = 0L;
		mReminderCount = 0;
		mFromBigButton = false;
	}

	public static DataManager getInstance() {
		if (mInstance == null) {
			mInstance = new DataManager();
		}
		return mInstance;
	}

	public static void destroyInstance() {
		 mInstance = null;
	}

	public void cleanup() {
		mMainActivityContext = null;
		mMainActivityActive = true;
		mMenuFromNotification = -2;
		mMenuStack = null;
		mLastMenuPopped = null;
		mGPSLocation = null;
		mNetworkLocation = null;
		mPassiveLocation = null;
		mLocationReady = false;
		mNeedLocationUpdate = true;
		mNeedLocalUpdate = false;
		mWalletNeedsUpdate = true;
		mGPSDwell = 0L;
		mCategoryTimestamp = 0L;
		mMessagesTimestamp = 0L;
		mLocations = null;
		mLocationIDs = null;
		mLocationContext = null;
		mEvents = null;
		mEventIDs = null;
		mEventContext = null;
		mPushTokens = null;
		mWalletData = null;
		mFavorites = null;
		mMessages = null;
		mCurrentMessage = null;
		mSummaryStats = null;
		mStatCheckIns = null;
		mStatFavorites = null;
		mStatRatings = null;
		mStatRedemptions = null;
		mStatTips = null;
		mSummaryAnalytics = null;
		mSummaryAnalyticsBusiness = null;
		mSummaryRedemptions = null;
		mSummaryCheckIns = null;
		mBusinessScanLocationID = -1;
		mRequestingPIN = false;
		mPIN = "";
		mScanTask = null;
		mCategoryContext = null;
		mCategoryFilterUpdate = false;
		mAnalyticsEnabled = true;
		mUnregistering = false;
		mUnregisterTimestamp = 0L;
		mReminderCount = 0;
		mFromBigButton = false;
	}

	public void setMainActivityContext(Context c) {
		mMainActivityContext = c;
	}

	public Context getMainActivityContext() {
		return mMainActivityContext;
	}

	public void setMainActivityActive(boolean active) {
		mMainActivityActive = active;
	}

	public boolean isMainActivityActive() {
		return mMainActivityActive;
	}

	public void setMenuFromNotification(int menu) {
		mMenuFromNotification = menu;
	}

	public int getMenuFromNotification() {
		return mMenuFromNotification;
	}

	public void setAPIVersion(int major, int minor, int patch) {
		mAPIVersion = new APIVersion(major, minor, patch);
	}

	public APIVersion getAPIVersion() {
		return mAPIVersion;
	}

	public boolean fragmentsValid() {
		return mMain != null && mWallet != null && mFavorite != null &&
				mUTC != null && mInbox != null && mSearch != null &&
				mRedeem != null && mAccount != null && mWeb != null &&
				mStatisticsSummary != null && mStatisticsList != null &&
				mAnalyticsSummary != null && mAnalyticsBusiness != null;
	}

	public void initializeFragments() {
		mMain = new MainFragment();
		mWallet = new WalletFragment();
		mFavorite = new FavoriteFragment();
		mUTC = new UTCFragment();
		mInbox = new InboxFragment();
		mSearch = new SearchFragment();
		mSearchBusiness = new SearchBusinessFragment();
		mSearchEvent = new SearchEventFragment();
		mSearchCategories = new SearchCategoriesFragment();
		mRedeem = new RedeemFragment();
		mEvent = new EventFragment();
		mAccount = new AccountFragment();
		mWeb = new WebFragment();
		mBusiness = new BusinessFragment();
		mSubscribe = new SubscribeFragment();
		mStatisticsSummary = new StatisticsSummaryFragment();
		mStatisticsList = new StatisticsListFragment();
		mAnalyticsSummary = new AnalyticsSummaryFragment();
		mAnalyticsBusiness = new AnalyticsBusinessFragment();
	}

	public void destroyFragments() {
		mMain = null;
		mWallet = null;
		mFavorite = null;
		mUTC = null;
		mInbox = null;
		mSearch = null;
		mSearchBusiness = null;
		mSearchEvent = null;
		mSearchCategories = null;
		mRedeem = null;
		mEvent = null;
		mAccount = null;
		mWeb = null;
		mBusiness = null;
		mSubscribe = null;
		mStatisticsSummary = null;
		mStatisticsList = null;
		mAnalyticsSummary = null;
		mAnalyticsBusiness = null;
	}

	public void pushToMenuStack(Constants.MenuID fID) {
		mMenuStack.push(fID);
	}

	public void popFromMenuStack() {
		if (mMenuStack.isEmpty()) {
			mLastMenuPopped = Constants.MenuID.EMPTY;
		} else {
			mLastMenuPopped = mMenuStack.pop();
		}
	}

	public Constants.MenuID lastMenuPopped() {
		return mLastMenuPopped;
	}

	public int menuStackSize() {
		return mMenuStack.size();
	}

	public boolean menuStackEmpty() {
		return mMenuStack.isEmpty();
	}

	public Constants.MenuID currentMenu() {
		Constants.MenuID menu;

		if (mMenuStack.isEmpty()) {
			menu = Constants.MenuID.EMPTY;
		} else {
			menu = mMenuStack.peek();
		}

		return menu;
	}

	public MainFragment getMainFragment() {
		return mMain;
	}

	public WalletFragment getWalletFragment() {
		return mWallet;
	}

	public FavoriteFragment getFavoriteFragment() {
		return mFavorite;
	}

	public UTCFragment getUTCFragment() {
		return mUTC;
	}

	public InboxFragment getInboxFragment() {
		return mInbox;
	}

	public SearchFragment getSearchFragment() {
		return mSearch;
	}

	public SearchBusinessFragment getSearchBusinessFragment() {
		return mSearchBusiness;
	}

	public SearchEventFragment getSearchEventFragment() {
		return mSearchEvent;
	}

	public SearchCategoriesFragment getSearchCategoriesFragment() {
		return mSearchCategories;
	}

	public RedeemFragment getRedeemFragment() {
		return mRedeem;
	}

	public EventFragment getEventFragment() {
		return mEvent;
	}

	public AccountFragment getAccountFragment() {
		return mAccount;
	}

	public WebFragment getWebFragment() {
		return mWeb;
	}

	public BusinessFragment getBusinessFragment() {
		return mBusiness;
	}

	public SubscribeFragment getSubscribeFragment() {
		return mSubscribe;
	}

	public StatisticsSummaryFragment getStatisticsSummaryFragment() { return mStatisticsSummary; }

	public StatisticsListFragment getStatisticsListFragment() { return mStatisticsList; }

	public AnalyticsSummaryFragment getAnalyticsSummaryFragment() { return mAnalyticsSummary; }

	public AnalyticsBusinessFragment getAnalyticsBusinessFragment() { return mAnalyticsBusiness; }

	public void executeCheckInOrRedeemTask(MainActivity main, String scanData, int role, int locID) {
		mCheckInOrRedeemTask = new CheckInOrRedeemTask(main, role, locID);
		// task object could be corrupted or canceled
		// when switching menus
		if (mCheckInOrRedeemTask != null) {
			if(Utils.hasHoneycomb()) {
				mCheckInOrRedeemTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, scanData);
			}
			else {
				mCheckInOrRedeemTask.execute(scanData);
			}
		}
	}

	public void executeCheckInOrRedeemTask(MainActivity main, String scanData, int role, int locID,
					boolean checkIn, boolean redeem) {
		mCheckInOrRedeemTask = new CheckInOrRedeemTask(main, role, locID, checkIn, redeem);
		// task object could be corrupted or canceled
		// when switching menus
		if (mCheckInOrRedeemTask != null) {
			if(Utils.hasHoneycomb()) {
				mCheckInOrRedeemTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, scanData);
			}
			else {
				mCheckInOrRedeemTask.execute(scanData);
			}
		}
	}

	public boolean cancelCheckInOrRedeemTask() {
		boolean result = false;
		if (mCheckInOrRedeemTask != null) {
			Logger.info(mName, "CheckInOrRedeemTask cancelled");
			result = mCheckInOrRedeemTask.cancel(true);
		} else {
			Logger.verbose(mName,
					"CheckInOrRedeemTask was null when cancelling");
		}
		return result;
	}

	public void setRequestingPIN(boolean state) {
		mRequestingPIN = state;
	}

	public boolean requestingPIN() {
		return mRequestingPIN;
	}

	public void setPIN(String pin) {
		mPIN = pin;
	}

	public String getPIN() {
		return mPIN;
	}

	public void setScanData(String scan) {
		mScanData = scan;
	}

	public String getScanData() {
		return mScanData;
	}

	public boolean isLocationReady() {
		return mLocationReady;
	}

	public void setLocationReady(boolean state) {
		mLocationReady = state;
	}

	public void setLastGPSLocation(Location loc) {
		mGPSLocation = loc;
	}

	public void setLastNetworkLocation(Location loc) {
		mNetworkLocation = loc;
	}

	public void setLastPassiveLocation(Location loc) {
		mPassiveLocation = loc;
	}

	public Location getLastGPSLocation() {
		return mGPSLocation;
	}

	public Location getLastNetworkLocation() {
		return mNetworkLocation;
	}

	public Location getLastPassiveLocation() {
		return mPassiveLocation;
	}

	public void setLastProximityLocation(Location loc) {
		mLastProximityLocation = loc;
	}

	public Location getLastProximityLocation() {
		return mLastProximityLocation;
	}

	public void setGPSDwellTime(long time) {
		mGPSDwell = time;
	}

	public long getGPSDwellTime() {
		return mGPSDwell;
	}

	public boolean doLocationsNeedUpdated() {
		// saved current revision
		Integer rev = mRevisionLocationInfo.getRevision();
		Logger.info(mName, "current revision - " + String.valueOf(rev));

		// retrieve updated revision
		DataRevisionParser.setRevisionLocationInfo();

		Logger.info(mName, "updated revision - " + String.valueOf(mRevisionLocationInfo.getRevision()));

		// compare revisions
		mNeedLocationUpdate = !mRevisionLocationInfo.revisionMatches(rev);

		return mNeedLocationUpdate;
	}

	public void setLocationsNeedUpdated(boolean state) {
		mNeedLocationUpdate = state;
	}

	public void forceLocationUpdate() { mRevisionLocationInfo = new UTCDataRevision(); }

	public boolean needLocalUpdate() {
		return mNeedLocalUpdate;
	}

	public void setLocalUpdateNeeded(boolean state) {
		mNeedLocalUpdate = state;
	}

	public boolean doesWalletNeedUpdate() {
		return mWalletNeedsUpdate;
	}

	public void setCategoryTimestamp(long time) {
		mCategoryTimestamp = time;
	}

	public boolean doCategoriesNeedUpdated() {
		return (SystemClock.elapsedRealtime() - mCategoryTimestamp) > Constants.CATEGORIES_UPDATE_DWELL;
	}

	public void setMessagesTimestamp(long time) {
		mMessagesTimestamp = time;
	}

	public void setEventsTimestamp(long time) {
		mEventsTimestamp = time;
	}

	public boolean doMessagesNeedUpdated() {
		return (SystemClock.elapsedRealtime() - mMessagesTimestamp) > Constants.MESSAGES_UPDATE_DWELL;
	}

	public boolean doEventsNeedUpdated() {
		return (SystemClock.elapsedRealtime() - mEventsTimestamp) > Constants.EVENTS_UPDATE_DWELL;
	}

	public void setWalletNeedsUpdate(boolean state) {
		mWalletNeedsUpdate = state;
	}

	public void addToWalletData(String key, String value) {
		if (mWalletData.containsKey(key)) {
			mWalletData.remove(key);
		}
		mWalletData.put(key, value);
	}

	public String getFromWalletData(String key) {
		return mWalletData.get(key);
	}

	public void addLocation(UTCLocation utcLoc) {
		if (mLocations.containsKey(utcLoc.id())) {
			Logger.verbose(mName, "location " + utcLoc.id().toString()
					+ " already added, replacing");
			mLocations.remove(utcLoc.id());
			mLocations.put(utcLoc.id(), utcLoc);
		} else {
			mLocations.put(utcLoc.id(), utcLoc);
			mLocationIDs.add(utcLoc.id());
		}
	}

	public HashMap<Integer, UTCLocation> getLocations() {
		return mLocations;
	}

	public UTCLocation getLocationContext() {
		return mLocationContext;
	}

	public void addToLocationContext(String key, String value) {
		if (mLocationContext.containsKey(key)) {
			mLocationContext.remove(key);
		}
		mLocationContext.put(key, value);
	}

	public void initializeLocationContext(Integer id) {
		mLocationContext = new UTCLocation(id);
	}

	public UTCLocation getLocation(Integer id) {
		return mLocations.get(id);
	}

	public void setLocations(HashMap<Integer, UTCLocation> locations) {
		mLocations = locations;
	}

	public int numberOfLocations() {
		return mLocations.size();
	}

	public void addEvent(UTCEvent utcEvt) {
		if (mEvents.containsKey(utcEvt.id())) {
			Logger.verbose(mName, "event " + utcEvt.id().toString()
					+ " already added, replacing");
			mEvents.remove(utcEvt.id());
			mEvents.put(utcEvt.id(), utcEvt);
		} else {
			mEvents.put(utcEvt.id(), utcEvt);
			mEventIDs.add(utcEvt.id());
		}
	}

	public HashMap<Integer, UTCEvent> getEvents() {
		return mEvents;
	}

	public UTCEvent getEventContext() {
		return mEventContext;
	}

	public void addToEventContext(String key, String value) {
		if (mEventContext.containsKey(key)) {
			mEventContext.remove(key);
		}
		mEventContext.put(key, value);
	}

	public void initializeEventContext(Integer id) {
		mEventContext = new UTCEvent(id);
	}

	public UTCEvent getEvent(Integer id) {
		return mEvents.get(id);
	}

	public void setEvents(HashMap<Integer, UTCEvent> events) {
		mEvents = events;
	}

	public int numberOfEvents() {
		return mEvents.size();
	}

	public ArrayList<Integer> getIDs() {
		return mLocationIDs;
	}

	public ArrayList<Integer> getEventIDs() {
		return mEventIDs;
	}

	public void addPushToken(UTCPushToken utcPt) {
		if (mPushTokens.containsKey(utcPt.id())) {
			Logger.verbose(mName, "push token " + utcPt.id().toString()
					+ " already added, replacing");
			mPushTokens.remove(utcPt);
			mPushTokens.put(utcPt.id(), utcPt);
		} else {
			mPushTokens.put(utcPt.id(), utcPt);
		}
	}

	public void clearPushTokens() {
		mPushTokens.clear();
	}

	public void addFavorite(Integer id) {
		mFavorites.add(id);
	}

	public void removeFavorite(Integer id) {
		mFavorites.remove(id);
	}

	public ArrayList<Integer> getFavorites() {
		return mFavorites;
	}

	public void clearFavorites() {
		mFavorites.clear();
	}

	public void addMessage(UTCMessage msg) {
		mMessages.add(msg);
	}

	public void addMessage(int index, UTCMessage msg) {
		mMessages.add(index, msg);
	}

	public void setCurrentMessage(UTCMessage msg) {
		mCurrentMessage = msg;
	}

	public UTCMessage getCurrentMessage() {
		return mCurrentMessage;
	}

	public void clearCurrentMessage() {
		mCurrentMessage = null;
	}

	public void removeFavorite(int index) {
		if(index >= mFavorites.size()) {
			mFavorites.remove(index);
		}
	}

	public ArrayList<UTCMessage> getMessages() {
		return mMessages;
	}

	public void clearMessages() {
		mMessages.clear();
	}

	public void setSummaryStats(ArrayList<SummaryStats> summaryStats) {
		mSummaryStats = summaryStats;
	}

	public ArrayList<SummaryStats> getSummaryStats() {
		return mSummaryStats;
	}

	public void clearSummaryStats() { mSummaryStats.clear(); }

	public void setStatCheckIns(ArrayList<StatCheckIn> checkIns) {
		mStatCheckIns = checkIns;
	}

	public ArrayList<StatCheckIn> getStatCheckIns() {
		return mStatCheckIns;
	}

	public void clearStatCheckIns() { mStatCheckIns.clear(); }

	public void setStatFavorites(ArrayList<StatFavorite> favorites) {
		mStatFavorites = favorites;
	}

	public ArrayList<StatFavorite> getStatFavorites() {
		return mStatFavorites;
	}

	public void clearStatFavorites() { mStatFavorites.clear(); }

	public void setStatRatings(ArrayList<StatRating> ratings) {
		mStatRatings = ratings;
	}

	public ArrayList<StatRating> getStatRatings() {
		return mStatRatings;
	}

	public void clearStatRatings() { mStatRatings.clear(); }

	public void setStatRedemptions(ArrayList<StatRedemption> redemptions) {
		mStatRedemptions = redemptions;
	}

	public ArrayList<StatRedemption> getStatRedemptions() {
		return mStatRedemptions;
	}

	public void clearStatRedemptions() { mStatRedemptions.clear(); }

	public void setStatTips(ArrayList<StatTip> tips) {
		mStatTips = tips;
	}

	public ArrayList<StatTip> getStatTips() {
		return mStatTips;
	}

	public void clearStatTips() { mStatTips.clear(); }

	public void setSummaryAnalytics(ArrayList<SummaryAnalytics> summaryAnalytics) {
		mSummaryAnalytics = summaryAnalytics;
	}

	public ArrayList<SummaryAnalytics> getSummaryAnalytics() {
		return mSummaryAnalytics;
	}

	public void clearSummaryAnalytics() {
		mSummaryAnalytics.clear();
	}

	public void setSummaryAnalyticsBusiness(ArrayList<SummaryAnalytics> summaryAnalytics) {
		mSummaryAnalyticsBusiness = summaryAnalytics;
	}

	public ArrayList<SummaryAnalytics> getSummaryAnalyticsBusiness() {
		return mSummaryAnalyticsBusiness;
	}

	public void clearSummaryAnalyticsBusiness() {
		mSummaryAnalyticsBusiness.clear();
	}

	public void setSummaryRedemptions(ArrayList<SummaryRedemption> summaryRedemptions) {
		mSummaryRedemptions = summaryRedemptions;
	}

	public ArrayList<SummaryRedemption> getSummaryRedemptions() {
		return mSummaryRedemptions;
	}

	public void clearSummaryRedemptions() {
		mSummaryRedemptions.clear();
	}

	public void setSummaryCheckIns(ArrayList<SummaryCheckIn> summaryCheckIns) {
		mSummaryCheckIns = summaryCheckIns;
	}

	public ArrayList<SummaryCheckIn> getSummaryCheckIns() {
		return mSummaryCheckIns;
	}

	public void clearSummaryCheckIns() {
		mSummaryCheckIns.clear();
	}

	public void setBusinessScanLocationID(int id) {
		mBusinessScanLocationID = id;
	}

	public int getBusinessScanLocationID() {
		return mBusinessScanLocationID;
	}

	public void setLocationInfoDataRevision(Integer drvID, String name, Integer rev, String ts) {
		mRevisionLocationInfo = new UTCDataRevision(drvID, name, rev, ts);
	}

	public UTCDataRevision getLocationInfoDataRevision() {
		return mRevisionLocationInfo;
	}

	public void clearLocationInfoDataRevision() {
		mRevisionLocationInfo = new UTCDataRevision();
	}

	public void setCategoriesDataRevision(Integer drvID, String name, Integer rev, String ts) {
		mRevisionCategories = new UTCDataRevision(drvID, name, rev, ts);
	}

	public UTCDataRevision getCategoriesDataRevision() {
		return mRevisionCategories;
	}

	public void clearCategoriesDataRevision() {
		mRevisionCategories = new UTCDataRevision();
	}

	public void setDealsDataRevision(Integer drvID, String name, Integer rev, String ts) {
		mRevisionDeals = new UTCDataRevision(drvID, name, rev, ts);
	}

	public UTCDataRevision getDealsDataRevision() {
		return mRevisionDeals;
	}

	public void clearDealsDataRevision() {
		mRevisionDeals = new UTCDataRevision();
	}

	public void setEventInfoDataRevision(Integer drvID, String name, Integer rev, String ts) {
		mRevisionEventInfo = new UTCDataRevision(drvID, name, rev, ts);
	}

	public UTCDataRevision getEventInfoDataRevision() {
		return mRevisionEventInfo;
	}

	public void clearEventInfoDataRevision() {
		mRevisionEventInfo = new UTCDataRevision();
	}

	public void setStatPermissions(JSONObject permissions) {

		// first clear existing permissions
		mHasGlobalStatistics = false;
		mHasGlobalAnalytics = false;
		mHasBusinessStatistics = false;
		mHasBusinessAnalytics = false;

		try {
			if (permissions.has("HasGlobalStatistics")) {
				mHasGlobalStatistics = permissions.getBoolean("HasGlobalStatistics");
			}
			if (permissions.has("HasGlobalAnalytics")) {
				mHasGlobalAnalytics = permissions.getBoolean("HasGlobalAnalytics");
			}
			if (permissions.has("HasBusinessStatistics")) {
				mHasBusinessStatistics = permissions.getBoolean("HasBusinessStatistics");
			}
			if (permissions.has("HasBusinessAnalytics")) {
				mHasBusinessAnalytics = permissions.getBoolean("HasBusinessAnalytics");
			}
		}
		catch (JSONException jsone) {
			Logger.error(mName, "JSON Exception when setting stat permissions");
		}
	}

	public boolean hasGlobalStatisticsPermission() {
		return mHasGlobalStatistics;
	}

	public boolean hasGlobalAnalyticsPermission() {
		return mHasGlobalAnalytics;
	}

	public boolean hasBusinessStatisticsPermission() {
		return mHasBusinessStatistics;
	}

	public boolean hasBusinessAnalyticsPermission() {
		return mHasBusinessAnalytics;
	}

	public void setRating(int locID, int rating) {
		// mLocations.get(locID).put(Constants.JSON_TAG_RATING,
		// String.valueOf(Utils.fiveRatingToTenRating(rating)));
		mLocations.get(locID).put(LocationParser.JSON_TAG_RATING,
				String.valueOf(rating));
	}

	public CategoryContext getCategoryContext() {
		return mCategoryContext;
	}

	public void setCategoryContext(CategoryContext cc) {
		mCategoryContext = cc;
	}

	public void setCategoryFilterUpdate(boolean state) {
		mCategoryFilterUpdate = state;
	}

	public boolean doesCategoryFilteringNeedUpdating() {
		return mCategoryFilterUpdate;
	}

	public void addCategory(UTCCategory cat) {
		mCategoryContext.addCategory(cat);
	}

	public void addCategory(int index, UTCCategory cat) {
		mCategoryContext.addCategory(index, cat);
	}

	public void replaceCategory(int index, UTCCategory cat) {
		mCategoryContext.replaceCategory(index, cat);
	}

	public void clearCategories() {
		mCategoryContext.clearCategories();
	}

	public CategoryAntifilters getCategoryAntifilters() {
		return mCategoryContext.getAntifilters();
	}

	public void setCategoryAntifilters(CategoryAntifilters ca) {
		mCategoryContext.setAntifilters(ca);
	}

	public void addCategoryToAntifilters(Integer id) {
		if(!mCategoryContext.getAntifilters()
				.isCategoryAntifiltered(id)) {
			mCategoryContext.getAntifilters()
			.addCategoryToAntifilters(id);
		}
	}

	public void addGroupToAntifilters(Integer id) {
		if(!mCategoryContext.getAntifilters()
				.isGroupAntifiltered(id)) {
			mCategoryContext.getAntifilters()
			.addGroupToAntifilters(id);
		}
	}

	public void removeCategoryFromAntifilters(Integer id) {
		if(mCategoryContext.getAntifilters()
				.isCategoryAntifiltered(id)) {
			mCategoryContext.getAntifilters()
			.removeCategoryFromAntifilters(id);
		}
	}

	public void removeGroupFromAntifilters(Integer id) {
		if(mCategoryContext.getAntifilters()
				.isGroupAntifiltered(id)) {
			mCategoryContext.getAntifilters()
			.removeGroupFromAntifilters(id);
		}
	}

	public boolean isCategoryAntifiltered(Integer id) {
		return mCategoryContext.getAntifilters()
				.isCategoryAntifiltered(id);
	}

	public boolean isGroupAntifiltered(Integer id) {
		return mCategoryContext.getAntifilters()
				.isGroupAntifiltered(id);
	}

	public void setScanTask(ScanTask taskType) {
		mScanTask = taskType;
	}

	public ScanTask getScanTask() {
		return mScanTask;
	}

	public void sortLocationsByDistance() {
		int locationNum = mLocations.size();

		// something is wrong if locations 0 or less, therefore bail
		if (locationNum <= 0) {
			return;
		}

		Logger.verbose(mName,
				"sorting " + locationNum
						+ " locations by distance (ID array list size: "
						+ mLocationIDs.size() + ")");

		int left = 0;
		int right = locationNum - 1;

		int i = 0;

		float[] distances = new float[locationNum];
		Integer[] IDs = new Integer[locationNum];

		for (Integer id : mLocations.keySet()) {
			distances[i] = Float.valueOf(mLocations.get(id).get(
					LocationParser.JSON_TAG_DISTANCE));
			IDs[i++] = id;
			Logger.verbose(mName,
					"distance value - " + String.valueOf(distances[i - 1]));
		}

		quickSort(distances, IDs, left, right);

		mLocationIDs.clear();
		for (i = 0; i < locationNum; i++) {
			mLocationIDs.add(IDs[i]);
		}
	}

	private int partition(float arr[], Integer ids[], int left, int right) {
		int i = left, j = right;
		float tmp;
		Integer tmpi;
		float pivot = arr[(left + right) / 2];

		while (i <= j) {
			while (arr[i] < pivot)
				i++;
			while (arr[j] > pivot)
				j--;
			if (i <= j) {
				tmp = arr[i];
				tmpi = ids[i];
				arr[i] = arr[j];
				ids[i] = ids[j];
				arr[j] = tmp;
				ids[j] = tmpi;
				i++;
				j--;
			}
		}
		;

		return i;
	}

	private void quickSort(float arr[], Integer ids[], int left, int right) {
		int index = partition(arr, ids, left, right);
		if (left < index - 1)
			quickSort(arr, ids, left, index - 1);
		if (index < right)
			quickSort(arr, ids, index, right);
	}

	public void sortLocationsByName() {
		int locationNum = mLocations.size();

		// something is wrong if locations 0 or less, therefore bail
		if (locationNum <= 0) {
			return;
		}

		Logger.verbose(mName,
				"sorting " + locationNum
						+ " locations by name (ID array list size: "
						+ mLocationIDs.size() + ")");

		int i = 0;

		float previousDistance = (float) -1.0;
		boolean inRange = false;
		int[][] ranges = new int[(locationNum / 2) + 1][2];
		int rangeNum = 0;

		// determine sections, or a range of indices, that share a distance
		// value
		for (i = 0; i < mLocationIDs.size(); i++) {
			if(mLocations.get(mLocationIDs.get(i)) == null) {
				return;
			}
			float distance = Float.valueOf(mLocations.get(mLocationIDs.get(i))
					.get(LocationParser.JSON_TAG_DISTANCE));
			if (previousDistance != -1.0) {
				if (previousDistance == distance) {
					if (inRange == false) {
						inRange = true;
						ranges[rangeNum][0] = i - 1;
					}
				} else if (inRange == true) {
					inRange = false;
					ranges[rangeNum++][1] = i - 1;
				}
			}
			previousDistance = distance;
		}

		if (inRange == true) {
			ranges[rangeNum++][1] = mLocationIDs.size() - 1;
		}

		ArrayList<String> names = new ArrayList<String>();
		// iterate through each section of locations that have the same distance
		for (i = 0; i < rangeNum; i++) {
			int first = ranges[i][0];
			int last = ranges[i][1];
			// knowing the index range, go through names and sort their
			// associated IDs
			for (int j = first; j <= last; j++) {
				for (int k = first; k <= last; k++) {
					String name1 = mLocations.get(mLocationIDs.get(j)).get(
							LocationParser.JSON_TAG_NAME);
					String name2 = mLocations.get(mLocationIDs.get(k)).get(
							LocationParser.JSON_TAG_NAME);
					Integer tmp;

					if (name1.compareToIgnoreCase(name2) < 0) {
						tmp = mLocationIDs.get(j);
						mLocationIDs.set(j, mLocationIDs.get(k));
						mLocationIDs.set(k, tmp);
					}
				}
			}
			names.clear();
		}
	}

	public void sortEventsByDate() {
		int eventsNum = mEvents.size();

		// something is wrong if events 0 or less, therefore bail
		if (eventsNum <= 0) {
			return;
		}

		Logger.verbose(mName,
				"sorting " + eventsNum
						+ " events by date (ID array list size: "
						+ mEventIDs.size() + ")");

		int left = 0;
		int right = eventsNum - 1;

		int i = 0;

		int[] dates = new int[eventsNum];
		Integer[] IDs = new Integer[eventsNum];

		for (Integer id : mEvents.keySet()) {
			dates[i] = Integer.valueOf(mEvents.get(id).get(
					EventsParser.JSON_TAG_SORTABLE_DATE).replace("-", ""));
			IDs[i++] = id;
			Logger.verbose(mName,
					"date value - " + String.valueOf(dates[i - 1]));
		}

		quickSort(dates, IDs, left, right);

		mEventIDs.clear();
		for (i = 0; i < eventsNum; i++) {
			mEventIDs.add(IDs[i]);
		}
	}

	private int partition(int arr[], Integer ids[], int left, int right) {
		int i = left, j = right;
		int tmp;
		Integer tmpi;
		int pivot = arr[(left + right) / 2];

		while (i <= j) {
			while (arr[i] < pivot)
				i++;
			while (arr[j] > pivot)
				j--;
			if (i <= j) {
				tmp = arr[i];
				tmpi = ids[i];
				arr[i] = arr[j];
				ids[i] = ids[j];
				arr[j] = tmp;
				ids[j] = tmpi;
				i++;
				j--;
			}
		}
		;

		return i;
	}

	private void quickSort(int arr[], Integer ids[], int left, int right) {
		int index = partition(arr, ids, left, right);
		if (left < index - 1)
			quickSort(arr, ids, left, index - 1);
		if (index < right)
			quickSort(arr, ids, index, right);
	}

	public void setAnalyticsState(boolean state) {
		mAnalyticsEnabled = state;
	}

	public boolean getAnalyticsState() {
		return mAnalyticsEnabled;
	}

	public void setUnregistering(boolean state) {
		mUnregistering = state;
	}

	public boolean getUnregistering() {
		return mUnregistering;
	}

	public void setUnregisterTimestamp(long timestamp) {
		mUnregisterTimestamp = timestamp;
	}

	public long getUnregisterTimestamp() {
		return mUnregisterTimestamp;
	}

	public void incrementReminder() {
		mReminderCount++;
	}

	public boolean reminderLimitHit() {
		return mReminderCount >= Constants.REMINDER_COUNT_LIMIT - 1;
	}

	public void resetReminder() {
		mReminderCount = 0;
	}

	public boolean fromBigButton() {
		return mFromBigButton;
	}

    public void setFromBigButton(boolean fromBig) {
    	mFromBigButton = fromBig;
    }
}