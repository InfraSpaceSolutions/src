package com.unitethiscity.general;

import android.util.Log;

public class Logger {

	// enable or disable verbose logging
	// (in addition to info, debug, warnings, and errors)
	private final static boolean VERBOSE = false;
	
	private final static boolean ALL_FLAG = true;
	
	private final static boolean UTILS						= true & ALL_FLAG;
	
	// UI classes
	private final static boolean FAVORITE_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean INBOX_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean MAIN_ACTIVITY 				= true & ALL_FLAG;
	private final static boolean SIGNUP_ACTIVITY			= true & ALL_FLAG;
	private final static boolean MAIN_FRAGMENT 				= true & ALL_FLAG;
	private final static boolean UTC_DIALOG_FRAGMENT 		= true & ALL_FLAG;
	private final static boolean LOYALTY_DIALOG			 	= true & ALL_FLAG;
	private final static boolean REDEEM_REWARD_DIALOG	 	= true & ALL_FLAG;
	private final static boolean REDEEM_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean EVENT_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean SCANNER_ACTIVITY 			= true & ALL_FLAG;
	private final static boolean SEARCH_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean SEARCH_BUSINESS_FRAGMENT 	= true & ALL_FLAG;
	private final static boolean SEARCH_EVENT_FRAGMENT 		= true & ALL_FLAG;
	private final static boolean SEARCH_CATEGORIES_FRAGMENT = true & ALL_FLAG;
	private final static boolean UNITE_THIS_CITY 			= true & ALL_FLAG;
	private final static boolean UTC_FRAGMENT 				= true & ALL_FLAG;
	private final static boolean WALLET_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean ACCOUNT_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean WEB_FRAGMENT 				= true & ALL_FLAG;
	private final static boolean BUSINESS_FRAGMENT 			= true & ALL_FLAG;
	private final static boolean SUBSCRIBE_FRAGMENT 		= true & ALL_FLAG;
	private final static boolean STATISTICS_SUMMARY_FRAGMENT = true & ALL_FLAG;
	private final static boolean STATISTICS_LIST_FRAGMENT   = true & ALL_FLAG;
	private final static boolean ANALYTICS_SUMMARY_FRAGMENT = true & ALL_FLAG;
	private final static boolean ANALYTICS_BUSINESS_FRAGMENT = true & ALL_FLAG;
	// other classes
	private final static boolean GEOLOCATION_MANAGER 		= true & ALL_FLAG;
	private final static boolean JSON_PARSER 				= true & ALL_FLAG;
	private final static boolean IMAGE_DOWNLOADER 			= true & ALL_FLAG;
	private final static boolean DATA_MANAGER 				= true & ALL_FLAG;
	private final static boolean LOGIN_MANAGER 				= true & ALL_FLAG;
	private final static boolean UTC_WEB_API 				= true & ALL_FLAG;
	private final static boolean LOCATION_PARSER 			= true & ALL_FLAG;
	private final static boolean LOCATION_CONTEXT_PARSER 	= true & ALL_FLAG;
	private final static boolean FAVORITES_PARSER 			= true & ALL_FLAG;
	private final static boolean WALLET_PARSER				= true & ALL_FLAG;
	private final static boolean CATEGORIES_PARSER 			= true & ALL_FLAG;
	private final static boolean INBOX_PARSER 				= true & ALL_FLAG;
	private final static boolean EVENTS_PARSER				= true & ALL_FLAG;
	private final static boolean EVENT_CONTEXT_PARSER		= true & ALL_FLAG;
	private final static boolean PUSH_TOKEN_PARSER			= true & ALL_FLAG;
	private final static boolean DATA_REVISION_PARSER		= true & ALL_FLAG;
	private final static boolean STATISTICS_PARSER			= true & ALL_FLAG;
	private final static boolean ANALYTICS_PARSER			= true & ALL_FLAG;
	private final static boolean CHECK_IN_OR_REDEEM_TASK	= true & ALL_FLAG;
	private final static boolean TWITTER_SIGN_IN			= true & ALL_FLAG;
	// Google Cloud Messaing
	private final static boolean MY_BROADCAST_RECEIVER		= true & ALL_FLAG;
	private final static boolean MY_INTENT_SERVICE			= true & ALL_FLAG;
	private final static boolean PROXIMITY_LOCATION_SERVICE = true & ALL_FLAG;
	
	private final static String[] NAMES = {
		"Utils",
		"FavoriteFragment",
		"InboxFragment",
		"MainActivity",
		"SignupActivity",
		"MainFragment",
		"UTCDialogFragment",
		"LoyaltyDialog",
		"RedeemRewardDialog",
		"RedeemFragment",
		"EventFragment",
		"ScannerActivity",
		"SearchFragment",
		"SearchBusinessFragment",
		"SearchEventFragment",
		"SearchCategoriesFragment",
		"UniteThisCity",
		"UTCFragment",
		"WalletFragment",
		"AccountFragment",
		"WebFragment",
		"BusinessFragment",
		"SubscribeFragment",
		"StatisticsSummaryFragment",
		"StatisticsListFragment",
		"AnalyticsSummaryFragment",
		"AnalyticsBusinessFragment",
		"UTCGeolocationManager",
		"ImageDownloader",
		"JSONParser",
		"DataManager",
		"UTCWebAPI",
		"LoginManager",
		"LocationParser",
		"LocationContextParser",
		"FavoritesParser",
		"WalletParser",
		"CategoriesParser",
		"InboxParser",
		"EventsParser",
		"EventContextParser",
		"PushTokenParser",
		"DataRevisionParser",
		"StatisticsParser",
		"AnalyticsParser",
		"CheckInOrRedeemTask",
		"TwitterSignIn",
		"MyBroadcastReceiver",
		"MyIntentService",
		"ProximityLocationService"
	};
	
	private final static boolean[] LOG = {
		UTILS,
		FAVORITE_FRAGMENT,
		INBOX_FRAGMENT,
		MAIN_ACTIVITY,
		SIGNUP_ACTIVITY,
		MAIN_FRAGMENT,
		UTC_DIALOG_FRAGMENT,
		LOYALTY_DIALOG,
		REDEEM_REWARD_DIALOG,
		REDEEM_FRAGMENT,
		EVENT_FRAGMENT,
		SCANNER_ACTIVITY,
		SEARCH_FRAGMENT,
		SEARCH_BUSINESS_FRAGMENT,
		SEARCH_EVENT_FRAGMENT,
		SEARCH_CATEGORIES_FRAGMENT,
		UNITE_THIS_CITY,
		UTC_FRAGMENT,
		WALLET_FRAGMENT,
		ACCOUNT_FRAGMENT,
		WEB_FRAGMENT,
		BUSINESS_FRAGMENT,
		SUBSCRIBE_FRAGMENT,
		STATISTICS_SUMMARY_FRAGMENT,
		STATISTICS_LIST_FRAGMENT,
		ANALYTICS_SUMMARY_FRAGMENT,
		ANALYTICS_BUSINESS_FRAGMENT,
		GEOLOCATION_MANAGER,
		JSON_PARSER,
		IMAGE_DOWNLOADER,
		DATA_MANAGER,
		LOGIN_MANAGER,
		UTC_WEB_API,
		LOCATION_PARSER,
		LOCATION_CONTEXT_PARSER,
		FAVORITES_PARSER,
		WALLET_PARSER,
		CATEGORIES_PARSER,
		INBOX_PARSER,
		EVENTS_PARSER,
		EVENT_CONTEXT_PARSER,
		PUSH_TOKEN_PARSER,
		DATA_REVISION_PARSER,
		STATISTICS_PARSER,
		ANALYTICS_PARSER,
		CHECK_IN_OR_REDEEM_TASK,
		TWITTER_SIGN_IN,
		MY_BROADCAST_RECEIVER,
		MY_INTENT_SERVICE,
		PROXIMITY_LOCATION_SERVICE
	};
	
	public static void info(String tag, String msg) {
		if(loggingForTag(tag)) {
			Log.i(tag, msg);
		}
	}
	
	public static void debug(String tag, String msg) {
		if(loggingForTag(tag)) {
			Log.d(tag, msg);
		}
	}
	
	public static void verbose(String tag, String msg) {
		if(VERBOSE) {
			Log.v(tag, msg);
		}
	}
	
	public static void warn(String tag, String msg) {
		Log.w(tag, msg);
	}
	
	public static void error(String tag, String msg) {
		Log.e(tag, msg);
	}
	
	private static boolean loggingForTag(String tag) {
		int size = NAMES.length;
		
		if(size != LOG.length) {
			Log.w("Logger", "NAMES and LOG lengths DO NOT MATCH!");
			return false;
		}
		
		for(int i = 0; i < size; i++) {
			if(tag.compareTo(NAMES[i]) == 0) {
				return LOG[i];
			}
		}
			
		return false;
	}
}
