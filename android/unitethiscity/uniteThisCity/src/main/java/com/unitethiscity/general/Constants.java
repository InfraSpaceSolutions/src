package com.unitethiscity.general;

import android.graphics.Color;

import com.unitethiscity.R;

public class Constants {
	public final static boolean TESTING = false;
	public final static boolean TESTING_LOCATION_FIELDS = false;
	public final static boolean TESTING_JSON = false;
	
	public final static long	FRAGMENT_SWITCH_DWELL					= 500; 					// milliseconds
	public final static long	CATEGORIES_UPDATE_DWELL					= 5 * (1000 * 60); 		// milliseconds
	public final static long	MESSAGES_UPDATE_DWELL					= 10 * (1000 * 60); 	// milliseconds
	public final static long	EVENTS_UPDATE_DWELL						= 10 * (1000 * 60); 	// milliseconds
	public final static long	ACTION_DWELL							= 1000; 				// milliseconds
	
	public final static long	LAYOUT_ADDITION_DELAY					= 25;					// milliseconds
	
	public final static String	UTILS									= "Utils";

	public final static int		PERMISSION_REQUEST_FINE_LOCATION			= 10;
	public final static int		PERMISSION_REQUEST_COARSE_LOCATION			= 11;
	public final static int		PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE_1	= 12;
	public final static int		PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE_2	= 13;

	public final static String	SHARED_PREFERENCES_NAME					= "UTCSharedPreferences";

	public final static String	SHARED_PREFERENCES_VERSION				= "Version";
	public final static String	SHARED_PREFERENCES_ACCTOKEN				= "AccountToken";
	public final static String	SHARED_PREFERENCES_NOTIFICATIONS		= "NotificationsEnabled";
	public final static String  SHARED_PREFERENCES_TUTORIAL_SHOWN		= "TutorialShown";
	public final static String	SHARED_PREFERENCES_DEFAULT_SOCIAL		= "DefaultSocial";
	
	public final static int		PUSH_TOKEN_DEVICE_ID					= 2;
	
	public final static String	SAVED_INSTANCE_FRAGMENT					= "SavedInstanceFragmentMenuID";
	public final static String	SAVED_INSTANCE_FRAGMENT_PARENT			= "SavedInstanceFragmentParentMenuID";

	public final static String 	SIGNUP_ACTIVITY							= "SignupActivity";
	public final static String 	FAVORITE_FRAGMENT 						= "FavoriteFragment";
	public final static String 	INBOX_FRAGMENT 							= "InboxFragment";
	public final static String 	MAIN_ACTIVITY 							= "MainActivity";
	public final static String 	TUTORIAL_ACTIVITY						= "TutorialActivity";
	public final static String 	MAIN_FRAGMENT 							= "MainFragment";
	public final static String 	UTC_DIALOG_FRAGMENT 					= "UTCDialogFragment";
	public final static String  UTC_SIGN_IN_DIALOG						= "SignInDialog";
	public final static String	UTC_RATE_DIALOG							= "RateDialog";
	public final static String  UTC_IMAGE_VIEWER_DIALOG					= "ImageViewerDialog";
	public final static String	UTC_REVIEW_VIEWER_DIALOG				= "ReviewViewerDialog";
	public final static String  UTC_LOYALTY_DIALOG						= "LoyaltyDialog";
	public final static String	UTC_REDEEM_REWARD_DIALOG				= "RedeemRewardDialog";
	public final static String 	REDEEM_FRAGMENT 						= "RedeemFragment";
	public final static String 	EVENT_FRAGMENT 							= "EventFragment";
	public final static String 	SCANNER_ACTIVITY 						= "ScannerActivity";
	public final static String 	SEARCH_FRAGMENT 						= "SearchFragment";
	public final static String 	SEARCH_BUSINESS_FRAGMENT 				= "SearchBusinessFragment";
	public final static String 	SEARCH_EVENT_FRAGMENT 					= "SearchEventFragment";
	public final static String 	SEARCH_CATEGORIES_FRAGMENT 				= "SearchCategoriesFragment";
	public final static String 	UNITE_THIS_CITY 						= "UniteThisCity";
	public final static String 	UTC_FRAGMENT 							= "UTCFragment";
	public final static String 	WALLET_FRAGMENT 						= "WalletFragment";
	public final static String 	ACCOUNT_FRAGMENT 						= "AccountFragment";
	public final static String 	BUSINESS_FRAGMENT 						= "BusinessFragment";
	public final static String  SUBSCRIBE_FRAGMENT						= "SubscribeFragment";
	public final static String 	WEB_FRAGMENT 							= "WebFragment";
	public final static String	STATISTICS_SUMMARY						= "StatisticsSummaryFragment";
	public final static String	STATISTICS_LIST							= "StatisticsListFragment";
	public final static String	ANALYTICS_SUMMARY						= "AnalyticsSummaryFragment";
	public final static String	ANALYTICS_BUSINESS						= "AnalyticsBusinessFragment";
	
	public final static String 	GEOLOCATION_MANAGER 					= "UTCGeolocationManager";
	public final static String 	JSON_PARSER 							= "JSONParser";
	public final static String 	IMAGE_DOWNLOADER 						= "ImageDownloader";
	public final static String 	DATA_MANAGER 							= "DataManager";
	public final static String 	LOGIN_MANAGER 							= "LoginManager";
	public final static String 	UTC_WEB_API 							= "UTCWebAPI";
	public final static String 	LOCATION_PARSER 						= "LocationParser";
	public final static String 	LOCATION_CONTEXT_PARSER 				= "LocationContextParser";
	public final static String 	FAVORITES_PARSER 						= "FavoritesParser";
	public final static String 	WALLET_PARSER 							= "WalletParser";
	public final static String 	CATEGORIES_PARSER 						= "CategoriesParser";
	public final static String 	INBOX_PARSER 							= "InboxParser";
	public final static String 	EVENTS_PARSER 							= "EventsParser";
	public final static String	EVENT_CONTEXT_PARSER					= "EventContextParser";
	public final static String	PUSH_TOKEN_PARSER						= "PushTokenParser";
	public final static String  DATA_REVISION_PARSER					= "DataRevisionParser";
	public final static String	STATISTICS_PARSER						= "StatisticsParser";
	public final static String	ANALYTICS_PARSER						= "AnalyticsParser";
	public final static String 	CHECK_IN_OR_REDEEM_TASK					= "CheckInOrRedeemTask";
	public final static String	TWITTER_SIGN_IN							= "TwitterSignIn";
	
	public final static String	ACCOUNT_CONTEXT							= "AccountContext";
	public final static String	FACEBOOK_CONTEXT						= "FacebookContext";
	public final static String	CATEGORY_CONTEXT						= "CategoryContext";
	public final static String	CATEGORY_ANTIFILTERS					= "CategoryAntifilters";

	public final static String 	WEB_FRAGMENT_URL_ARG					= "WebFragmentURL";
	public final static String 	WEB_FRAGMENT_TITLE						= "WebFragmentTitle";

	public final static String  INBOX_FRAGMNENT_ID_ARG					= "MessageID";
	public final static String	INBOX_FRAGMENT_SUMMARY_ARG				= "MessageSummary";
	public final static String	INBOX_FRAGMENT_BODY_ARG					= "MessageBody";
	public final static String	INBOX_FRAGMENT_READ_ARG					= "MessageRead";

	public final static String	MY_BROADCAST_RECEIVER					= "MyBroadcastReceiver";
	public final static String	MY_INTENT_SERVICE					  	= "MyIntentService";
	public final static String  PROXIMITY_LOCATION_SERVICE				= "ProximityLocationService";
	
	public final static boolean ENABLE_BUSINESS_BUTTON					= true;
	
	public final static int		FAVORITE_LOCATION_COLOR					= Color.argb(0xFF, 0xFF, 0xFF, 0xFF);
	
	public final static int 	TWO_MINUTES 							= 1000 * 60 * 2;
	public final static int 	FIFTEEN_MINUTES 						= 1000 * 60 * 15;
	public final static int 	FRAGMENT_WINDOW 						= 0;
	public final static int 	VIBRATE_LENGTH 							= 10;
	public final static int 	TIP_LENGTH 								= 3000;
	public final static int 	NUM_MAIN_MENU_ITEMS 					= 5;
	public final static int 	BASE_CUSTOM_IDS 						= 0x7e000000;
	public final static int 	BASE_OFFSET_LOCATION_DETAILS_CHILD 		= 100;
	public final static float 	METERS_PER_MILE 						= 1609.34f; // meters per miles conversion factor
	public final static int 	LOCATION_GPS_UPDATE_INTERVAL 			= 10*1000; // milliseconds
	public final static int 	LOCATION_NETWORK_UPDATE_INTERVAL 		= 5*1000; // milliseconds
	public final static int 	LOCATION_MIN_DISTANCE_UPDATE 			= 100; // meters
	public final static int 	LOCATION_MIN_GPS_SATS 					= 3;
	public final static long 	LOCATION_GPS_TURNOFF 					= 45*1000L;
	public final static long 	LOCATION_SINGLE_UPDATE_INTERVAL 		= 45*1000L;
	public final static long 	LOCATION_GPS_DWELL 						= 60*1000L;
	public final static int 	LOCATION_GPS_TIMEOUT 					= 5000;
	public final static int 	LOCATION_NETWORK_TIMEOUT 				= 3000;
	public final static int 	LOCATION_ACCURACY_LIMIT 				= 100; // meters
	public final static int 	LOCATION_SINGLE_UPDATE_GPS 				= 1;
	public final static int 	LOCATION_SINGLE_UPDATE_NET 				= 2;
	public final static int 	LOCATION_SINGLE_UPDATE_BOTH 			= 3;
	public final static String 	LOCATION_MISSING_TAG 					= "9999";
	public final static int		LOCATION_PROXIMITY_INTERVAL				= 2*1000; // milliseconds
	public final static int 	LOCATION_CHUNK_SIZE 					= 25;
	// downtown Cleveland, OH
	public final static double	LOCATION_DEFAULT_LATITUDE				= 41.504959;
	public final static double	LOCATION_DEFAULT_LONGITUDE				= -81.685603;
	public final static String 	LOCATION_INFO_IMAGE	 					= "http://www.unitethiscity.com/businessimages";
	public final static String	LOCATION_INFO_REVIEWS					= "https://www.unitethiscity.com/app/tips/?loc=";
	
	public final static String	UNITE_THIS_CITY_HOME_URL				= "http://www.unitethiscity.com/";
	public final static String	UNITE_THIS_CITY_JOIN_US_URL				= "http://www.unitethiscity.com/JoinUs";
	public final static String	UNITE_THIS_CITY_TERMS_AND_CONDITIONS	= "http://www.unitethiscity.com/TermsAndConditions";
	public final static String	UNITE_THIS_CITY_FORGOT_PASSWORD			= "https://www.unitethiscity.com/mobileresetpassword";

	public final static String	MEMBER_IDENTIFIER_URL					= "http://www.unitethiscity.com/qr/m/?a=";
	public final static String	BUSINESS_IDENTIFIER_URL					= "http://www.unitethiscity.com/qr/b/?b=";
	
	public final static String	BUSINESS_DIRECTORY						= "http://www.unitethiscity.com/directory/business/"; // followed by <locid>
	public final static String	BUSINESS_DIRECTORY_NO_MAPS				= "?nomap=1";
	
	public final static String	MESSAGE_VIEW_URL						= "http://www.unitethiscity.com/app/message/?inb=";  // followed by &tok=<token>
																		  //"http://unitethiscity.dev.sancsoft.net/app/message/?inb=";
	public final static String	LOCATION_TERMS_SOCIAL_DEAL_URL			= "https://www.unitethiscity.com/app/terms/embedded2?l=";
	public final static String	LOCATION_TERMS_LOYALTY_DEAL_URL			= "https://www.unitethiscity.com/app/terms/embeddedLoyalty2?l=";
	public final static String	LOCATION_TERMS_DEFAULT_URL				= "https://www.unitethiscity.com/TermsAndConditions";


	public final static String	GRAVATAR_AVATAR_URL						= "http://www.gravatar.com/avatar/";
	public final static String	FACEBOOK_AVARTAR_URL					= "https://graph.facebook.com/"; // Facebook user ID and then followed by /picture and possibly ?width=<n>

	public final static String	TUTORIAL_VIDEO_URL						= "http://www.unitethiscity.com/video/tutorial-video.mp4";

	public final static String	BUSINESS_GALLERY_URL					= "http://www.unitethiscity.com/BusinessGallery/";

    // JSON-related
    public final static int 	JSON_CONNECTION_TIMEOUT 				= 5*1000; // milliseconds
    public final static int 	JSON_SOCKET_TIMEOUT 					= 10*1000; // milliseconds

	public final static String  FACEBOOK_LOGIN_PASSCODE					= "xxx-xxxxxxxxxx-xxxxxxxx";

	public final static String	EMPTY_UTC_TOKEN							= "00000000-0000-0000-0000-000000000000";

    public final static String	ACCOUNT_HASH_KEY 						= "063a64a2-92ed-4953-bb27-dde61238ca5e";
    public final static String	MEMBER_HASH_KEY 						= "ef0cd82a-8be1-4208-b6ae-0eaf97e7e14b";
    public final static String	BUSINESS_HASH_KEY 						= "4dc01879-29b6-4212-9654-fe4e6069bc2d";
    
    // linked to Sanctuary Software
	public final static String 	SCANDIT_SDK_APP_KEY 					= "rFXz/JDtEeKUByE5g8ULslvQvx3H+dRLcw6qXT6y79I";
	
	// Google API information for UTC
	public final static String	GOOGLE_API_PROJECT_NUMBER				= "937943914086";
	public final static String	GOOGLE_API_KEY							= "AIzaSyDnaYXAKcKh6LU-ygbk6uKDcu8Nt19LwRA";
	
	// GCM-related
	public final static String	GCM_REGISTRATION_ID						= "RegistrationID";
	public final static String	GCM_EXPONENTIAL_BACKOFF					= "ExponentialBackoff";
	public final static int		GCM_UNREGISTER_TIMEOUT					= 5*1000; // milliseconds
	
	// Social-posting related
	public final static String	DEFAULT_FB_POST_IMAGE					= "http://www.unitethiscity.com/facebook/utc128.png";
	public final static String	DEFAULT_DESCRIPTION_PREFIX				= "Supporting local through Unite This City";
	public final static int		REMINDER_COUNT_LIMIT					= 10;
    
	public final static Integer[] ratingIds = {
			R.drawable.rating_zero,
			R.drawable.rating_one,
			R.drawable.rating_two,
			R.drawable.rating_three,
			R.drawable.rating_four,
			R.drawable.rating_five,
			R.drawable.rating_six,
			R.drawable.rating_seven,
			R.drawable.rating_eight,
			R.drawable.rating_nine,
			R.drawable.rating_ten
	};
    
	public final static int[] menuImageViews = {
		R.id.imageViewWallet,
		R.id.imageViewFavorite,
		R.id.imageViewUTC,
		R.id.imageViewInbox,
		R.id.imageViewSearch
	};
	public final static int[] menuImageResouces = {
		R.drawable.btn_wallet,
		R.drawable.btn_favorite,
		R.drawable.btn_utc,
		R.drawable.btn_inbox,
		R.drawable.btn_search
	};
	public final static int[] menuOnImageResouces = {
		R.drawable.btn_wallet_on,
		R.drawable.btn_favorite_on,
		R.drawable.btn_utc_on,
		R.drawable.btn_inbox_on,
		R.drawable.btn_search_on
	};
	
    public enum MenuType {
        MAIN,
        SUB
    }
    
	public final static String[] FRAGMENT_NAMES = {
		MAIN_FRAGMENT,
		WALLET_FRAGMENT,
		FAVORITE_FRAGMENT,
		UTC_FRAGMENT,
		INBOX_FRAGMENT,
		SEARCH_FRAGMENT,
		SEARCH_BUSINESS_FRAGMENT,
		SEARCH_EVENT_FRAGMENT,
		SEARCH_CATEGORIES_FRAGMENT,
		REDEEM_FRAGMENT,
		EVENT_FRAGMENT,
		ACCOUNT_FRAGMENT,
		WEB_FRAGMENT,
		BUSINESS_FRAGMENT,
		SUBSCRIBE_FRAGMENT,
		STATISTICS_SUMMARY,
		STATISTICS_LIST,
		ANALYTICS_SUMMARY,
		ANALYTICS_BUSINESS
	};
	
    // main menu items must be grouped together at front of values
    // first main menu item starts at value 0
    // last main menu item value must end at NUM_MAIN_MENU_ITEMS - 1
    public enum MenuID {
    	EMPTY(-2),
    	HOME(-1),
    	WALLET(0),
        FAVORITE(1),
        UTC(2),
        INBOX(3),
        SEARCH(4),
        SEARCH_BUSINESS(5),
        SEARCH_EVENT(6),
        SEARCH_CATEGORIES(7),
        REDEEM(8),
        EVENT(9),
        ACCOUNT(10),
        WEB(11),
        BUSINESS(12),
        SUBSCRIBE(13),
		STATISTICS_SUMMARY(14),
		STATISTICS_LIST(15),
		ANALYTICS_SUMMARY(16),
		ANALYTICS_BUSINESS(17);

        private final int value;
        
        private MenuID(int v)
        {
            value = v;
        }
        
        public static MenuID fromValue(int v) {
        	for(MenuID id : MenuID.values()) {
        		if(id.value == v) {
        			return id;
        		}
        	}
        	
        	return HOME;
        }
        
        public String getName() {
        	return FRAGMENT_NAMES[value + 1];
        }

        public int getValue() {
            return value;
        }
    }

    public enum Role {
        MEMBER(2),
        BUSINESS(3);

        private final int value;
        
        private Role(int v)
        {
            value = v;
        }
        
        static Role fromValue(int v) {
        	for(Role id : Role.values()) {
        		if(id.value == v) {
        			return id;
        		}
        	}
        	
        	return MEMBER;
        }

        public int getValue() {
            return value;
        }
    }
    
    public enum SocialPostType {
        FACEBOOK_AUTOMATIC(0),
        FACEBOOK(1),
        TWITTER(2),
		INSTAGRAM(3);

        private final int value;
        
        private SocialPostType(int v)
        {
            value = v;
        }
        
        static SocialPostType fromValue(int v) {
        	for(SocialPostType id : SocialPostType.values()) {
        		if(id.value == v) {
        			return id;
        		}
        	}
        	
        	return FACEBOOK_AUTOMATIC;
        }

        public int getValue() {
            return value;
        }
    }
}
