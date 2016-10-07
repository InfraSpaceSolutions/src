package com.unitethiscity.data;

import java.util.ArrayList;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;
import com.unitethiscity.general.Utils;

public class UTCWebAPI {

	private static String mName = Constants.UTC_WEB_API;
	
	// using API major version 1
	public 	final static int	API_MAJOR_VERSION	= 1;
	public 	final static int	API_MINOR_VERSION	= 15;
	private final static String UTC_PRODUCTION_API	= String.valueOf(API_MAJOR_VERSION) + ".thinkm.co/api/";
	private final static String UTC_DEV_API			= ".dev.sancsoft.net/api/";
	private final static String UTC_SECURE_API		= "https://api.unitethiscity.com/api/";
	private final static String UTC_WEB_API_URL 	//= "http://utcapi" + UTC_PRODUCTION_API;
													//= "http://utcapi" + UTC_DEV_API;
													= UTC_SECURE_API;

	private final static String	WALLET 				= UTC_WEB_API_URL + "Wallet";
	private final static String	RATING 				= UTC_WEB_API_URL + "Rating";
	private final static String	TIP 				= UTC_WEB_API_URL + "Tip";
	private final static String	FAVORITE 			= UTC_WEB_API_URL + "Favorite";
	private final static String	INBOX_MESSAGE 		= UTC_WEB_API_URL + "InboxMessage";
	private final static String INBOX_READ			= UTC_WEB_API_URL + "InboxRead";
	private final static String INBOX_DELETE		= UTC_WEB_API_URL + "InboxDelete";
	private final static String	CHECK_IN 			= UTC_WEB_API_URL + "CheckIn";
	private final static String	REDEEM 				= UTC_WEB_API_URL + "Redeem";
	private final static String	UNIFIED_ACTION		= UTC_WEB_API_URL + "UnifiedAction";
	private final static String	LOCATION_CONTEXT 	= UTC_WEB_API_URL + "LocationContext";
	private final static String	EVENT_CONTEXT 		= UTC_WEB_API_URL + "EventContext";
	private final static String	LOGOUT 				= UTC_WEB_API_URL + "Logout";
	private final static String	CATEGORY 			= UTC_WEB_API_URL + "Category";
	private final static String	VERSION 			= UTC_WEB_API_URL + "Version";
	private final static String	LOGIN 				= UTC_WEB_API_URL + "Login";
	private final static String	LOGIN_FACEBOOK		= UTC_WEB_API_URL + "FacebookLogin";
	private final static String	LOCATION_INFO 		= UTC_WEB_API_URL + "LocationInfo";
	private final static String	LOCATION_SUMMARY	= UTC_WEB_API_URL + "LocationSummary";
	private final static String	EVENT_INFO 			= UTC_WEB_API_URL + "EventInfo";
	private final static String PUSH_TOKEN			= UTC_WEB_API_URL + "PushToken";
	private final static String SOCIAL_POST			= UTC_WEB_API_URL + "SocialPost";
	private final static String DATA_REVISION		= UTC_WEB_API_URL + "DataRevision";
	private final static String SUBSCRIPTION		= UTC_WEB_API_URL + "Subscription";
	private final static String FREE_SIGN_UP		= UTC_WEB_API_URL + "FreeSignUp";
	private final static String FREE_SIGN_UP2		= UTC_WEB_API_URL + "FreeSignUp2";
	private final static String FREE_SIGN_UP3		= UTC_WEB_API_URL + "FreeSignUp3";
	private final static String	PROXIMITY			= UTC_WEB_API_URL + "Proximity";
	private final static String	GALLERY				= UTC_WEB_API_URL + "Gallery";
	private final static String	MENU				= UTC_WEB_API_URL + "Menu";
	private final static String OPT_OUT				= UTC_WEB_API_URL + "OptOut";
	private final static String SUMMARY_STATS		= UTC_WEB_API_URL + "SummaryStats";
	private final static String STAT_FAVORITE		= UTC_WEB_API_URL + "StatFavorite";
	private final static String STAT_REDEMPTION		= UTC_WEB_API_URL + "StatRedemption";
	private final static String STAT_CHECK_IN		= UTC_WEB_API_URL + "StatCheckIn";
	private final static String STAT_RATING			= UTC_WEB_API_URL + "StatRating";
	private final static String STAT_TIP			= UTC_WEB_API_URL + "StatTip";
	private final static String SUMMARY_ANALYTICS	= UTC_WEB_API_URL + "SummaryAnalytics";
	private final static String SUMMARY_REDEMPTIONS	= UTC_WEB_API_URL + "SummaryRedemptions";
	private final static String SUMMARY_CHECK_INS	= UTC_WEB_API_URL + "SummaryCheckIns";
	private final static String STAT_PERMISSIONS	= UTC_WEB_API_URL + "StatPermissions";
	
	public static JSONObject getWallet() {
		Logger.verbose(mName, "getWallet");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(WALLET, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getUserWallet(String token) {
		Logger.verbose(mName, "getUserWallet");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(WALLET + "?token=" + token, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static int getRating(String token, Integer id) {
		Logger.verbose(mName, "getRating");
		
		int reply = 0;
		
		reply = JSONParser.getIntegerFromURL(RATING + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return reply;
	}
	
	public static JSONObject postRating(String token, Integer id, int rating) {
		Logger.verbose(mName, "postRating");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(RATING + "/" + id.toString() + "?token=" + token + "&rating=" + String.valueOf(rating),
				JSONParser.HttpMethod.POST,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONArray deleteRating(String token, Integer id) {
		Logger.verbose(mName, "deleteRating");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(RATING + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.DELETE,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONObject getTip(String token, Integer id) {
		Logger.verbose(mName, "getTip");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(TIP + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject postTip(String token, Integer id, int accid, String text, String signature, 
			String timestamp, String timestampasstr) {
		Logger.verbose(mName, "postTip");
		
		JSONObject jsonReply = null;
		
		JSONObject tipBody = null;
		try {
			tipBody = new JSONObject();
			tipBody.put("LocId", id.intValue());
			tipBody.put("AccId", accid);
			tipBody.put("Text", text);
			tipBody.put("Signature", signature);
			tipBody.put("Timestamp", timestamp);
			tipBody.put("TimestampAsStr", timestampasstr);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}

		jsonReply = JSONParser.getJSONObjectFromURL(TIP + "?token=" + token, 
				JSONParser.HttpMethod.POST,
				tipBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject deleteTip(String token, Integer id) {
		Logger.verbose(mName, "deleteTip");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(TIP + "/" + id.toString() + "?token=" + token, 
				JSONParser.HttpMethod.DELETE,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONArray getFavorites(String token) {
		Logger.verbose(mName, "getFavorites");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(FAVORITE + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject postFavorite(String token, Integer id) {
		Logger.verbose(mName, "postFavorite");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(FAVORITE + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.POST,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONObject deleteFavorite(String token, Integer id) {
		Logger.verbose(mName, "deleteFavorite");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(FAVORITE + "/" + id.toString() + "?token=" + token, 
				JSONParser.HttpMethod.DELETE,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONArray getAllMessages(String token) {
		Logger.verbose(mName, "getAllMessages");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(INBOX_MESSAGE + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getMessage(String token, Integer id) {
		Logger.verbose(mName, "getMessage");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(INBOX_MESSAGE + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}

	public static JSONObject readMessage(String token, Integer id) {
		Logger.verbose(mName, "readMessage");

		JSONObject jsonReply = null;

		jsonReply = JSONParser.getJSONObjectFromURL(INBOX_READ + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.POST,
				null,
				false);

		return jsonReply;
	}

	public static JSONObject deleteMessage(String token, Integer id) {
		Logger.verbose(mName, "deleteMessage");

		JSONObject jsonReply = null;

		jsonReply = JSONParser.getJSONObjectFromURL(INBOX_DELETE + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.POST,
				null,
				false);

		return jsonReply;
	}

	public static JSONObject optOut(String token, Integer id) {
		Logger.verbose(mName, "optOut");

		JSONObject jsonReply = null;

		jsonReply = JSONParser.getJSONObjectFromURL(OPT_OUT + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.POST,
				null,
				false);

		return jsonReply;
	}

	public static JSONObject checkIn(String token, int accid, int rolid, Integer id, int memberaccid, 
			String qurl, double latitude, double longitude) {
		Logger.verbose(mName, "checkIn");
		
		JSONObject jsonReply = null;
		
		JSONObject checkInBody = null;
		try {
			checkInBody = new JSONObject();
			checkInBody.put("AccId", accid);
			checkInBody.put("RolId", rolid);
			checkInBody.put("LocId", id.intValue());
			checkInBody.put("MemberAccId", memberaccid);
			checkInBody.put("Qurl", qurl);
			checkInBody.put("Latitude", latitude);
			checkInBody.put("Longitude", longitude);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(CHECK_IN + "?token=" + token, 
				JSONParser.HttpMethod.POST,
				checkInBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject redeem(String token, int accid, int rolid, Integer id, int dealid, int memberaccid, 
			String qurl, String pinnumber, double latitude, double longitude) {
		Logger.verbose(mName, "redeem");
		
		JSONObject jsonReply = null;
		
		JSONObject redeemBody = null;
		try {
			redeemBody = new JSONObject();
			redeemBody.put("AccId", accid);
			redeemBody.put("RolId", rolid);
			redeemBody.put("LocId", id.intValue());
			redeemBody.put("DealId", dealid);
			redeemBody.put("MemberAccId", memberaccid);
			redeemBody.put("Qurl", qurl);
			redeemBody.put("PinNumber", pinnumber);
			redeemBody.put("Latitude", latitude);
			redeemBody.put("Longitude", longitude);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(REDEEM + "?token=" + token, 
				JSONParser.HttpMethod.POST,
				redeemBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject unifiedAction(String token, int accid, int rolid, int memberaccid, 
			String qurl, String pinnumber, double latitude, double longitude, boolean checkIn, boolean redeem) {
		Logger.verbose(mName, "unifiedAction");
		
		JSONObject jsonReply = null;
		
		int checkInRequest = 0;
		int redeemRequest = 0;
		if(checkIn) checkInRequest++;
		if(redeem) redeemRequest++;
		
		JSONObject unifiedActionBody = null;
		try {
			unifiedActionBody = new JSONObject();
			unifiedActionBody.put("AccId", accid);
			unifiedActionBody.put("RolId", rolid);
			unifiedActionBody.put("MemberAccId", memberaccid);
			unifiedActionBody.put("Qurl", qurl);
			unifiedActionBody.put("PinNumber", pinnumber);
			unifiedActionBody.put("Latitude", latitude);
			unifiedActionBody.put("Longitude", longitude);
			unifiedActionBody.put("RequestCheckin", checkInRequest);
			unifiedActionBody.put("RequestRedeem", redeemRequest);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(UNIFIED_ACTION + "?token=" + token, 
				JSONParser.HttpMethod.POST,
				unifiedActionBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getLocationContext(Integer id) {
		Logger.verbose(mName, "getLocationContext");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(LOCATION_CONTEXT + "/" + id.toString(), 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getEventContext(Integer id) {
		Logger.verbose(mName, "getEventContext");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(EVENT_CONTEXT + "/" + id.toString(), 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getUserLocationContext(String token, Integer id) {
		Logger.verbose(mName, "getUserLocationContext");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(LOCATION_CONTEXT + "/" + id.toString() + "?token=" + token, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getUserEventContext(String token, Integer id) {
		Logger.verbose(mName, "getUserEventContext");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(EVENT_CONTEXT + "/" + id.toString() + "?token=" + token, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject logout(ArrayList<Integer> businessroles, ArrayList<Integer> charityroles, ArrayList<Integer> associateroles, int accid, 
			Integer id, String accguid, String token, String accemail, String accfname, String acclname, 
			boolean isadmin, boolean issalesrep, boolean ismember) {
		Logger.verbose(mName, "logout");
		
		JSONObject jsonReply = null;
		
		JSONObject logoutBody = null;
		try {
			logoutBody = new JSONObject();
			JSONArray businessRoles = new JSONArray();
			for(int i = 0; i < businessroles.size(); i++) {
				businessRoles.put(businessroles.get(i).intValue());
			}
			logoutBody.put("BusinessRoles", businessRoles);
			JSONArray charityRoles = new JSONArray();
			for(int i = 0; i < charityroles.size(); i++) {
				charityRoles.put(charityroles.get(i).intValue());
			}
			logoutBody.put("CharityRoles", charityRoles);
			JSONArray associateRoles = new JSONArray();
			for(int i = 0; i < associateroles.size(); i++) {
				associateRoles.put(associateroles.get(i).intValue());
			}
			logoutBody.put("AssociateRoles", businessRoles);
			logoutBody.put("AccId", accid);
			logoutBody.put("CitId", id.toString());
			logoutBody.put("AccGuid", accguid);
			logoutBody.put("Token", token);
			logoutBody.put("AccEMail", accemail);
			logoutBody.put("AccFName", accfname);
			logoutBody.put("AccLName", acclname);
			logoutBody.put("IsAdmin", isadmin);
			logoutBody.put("IsSalesRep", issalesrep);
			logoutBody.put("IsMember", ismember);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(LOGOUT,
				JSONParser.HttpMethod.POST,
				logoutBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONArray getCategories() {
		Logger.verbose(mName, "getCategories");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(CATEGORY,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getVersion() {
		Logger.verbose(mName, "getVersion");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(VERSION,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject login(String account, String password) {
		Logger.verbose(mName, "login");
		
		JSONObject jsonReply = null;
		
		JSONObject loginBody = null;
		try {
			loginBody = new JSONObject();
			loginBody.put("Account", account);
			loginBody.put("Password", Utils.md5(password + "-" + Constants.ACCOUNT_HASH_KEY));
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(LOGIN, 
				JSONParser.HttpMethod.POST,
				loginBody,
				true);
		
		return jsonReply;
	}

	public static JSONObject loginFacebook(String account, String id) {
		Logger.verbose(mName, "loginFacebook");

		JSONObject jsonReply = null;

		JSONObject loginBody = null;
		try {
			loginBody = new JSONObject();
			loginBody.put("Account", account);
			loginBody.put("Passcode", Constants.FACEBOOK_LOGIN_PASSCODE);
			loginBody.put("FacebookId", id);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}

		jsonReply = JSONParser.getJSONObjectFromURL(LOGIN_FACEBOOK,
				JSONParser.HttpMethod.POST,
				loginBody,
				true);

		return jsonReply;
	}
	
	public static JSONArray getLocations() {
		Logger.verbose(mName, "getLocations");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(LOCATION_INFO,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}

	public static JSONArray getLocationsSummary(String token) {
		Logger.verbose(mName, "getLocationsSummary");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(LOCATION_SUMMARY + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getLocationsSummary() {
		Logger.verbose(mName, "getLocationsSummary");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(LOCATION_SUMMARY,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}
	
	public static JSONObject getLocation(Integer id) {
		Logger.verbose(mName, "getLocation");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(LOCATION_INFO + "/" + id.toString(),
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONArray getEvents() {
		Logger.verbose(mName, "getEvents");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(EVENT_INFO,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getEvent(Integer id) {
		Logger.verbose(mName, "getEvent");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(EVENT_INFO + "/" + id.toString(),
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject createPushToken(String token, int putID, int accountID, int pdtID, String registrationID, boolean enabled) {
		Logger.verbose(mName, "createPushToken");
		
		JSONObject jsonReply = null;
		
		JSONObject createRecordBody = null;
		try {
			createRecordBody = new JSONObject();
			createRecordBody.put("PutId", putID);
			createRecordBody.put("AccId", accountID);
			createRecordBody.put("PdtId", pdtID);
			createRecordBody.put("PutToken", registrationID);
			createRecordBody.put("PutEnabled", enabled);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(PUSH_TOKEN + "?token=" + token,
				JSONParser.HttpMethod.POST,
				createRecordBody,
				false);
		
		return jsonReply;
	}
	
	public static JSONArray getPushTokens(String token) {
		Logger.verbose(mName, "getPushTokens");
		
		JSONArray jsonReply = null;
		
		jsonReply = JSONParser.getJSONArrayFromURL(PUSH_TOKEN + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getPushTokenByID(Integer id, String token) {
		Logger.verbose(mName, "getPushTokenByID");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(PUSH_TOKEN + "/" + id.toString() + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getPushTokenByToken(String token, String registrationID) {
		Logger.verbose(mName, "getPushTokenByID");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(PUSH_TOKEN + "?token=" + token + "&pt=" + registrationID, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject deletePushToken(String token, int accountID, String registrationID) {
		Logger.verbose(mName, "deletePushToken");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(PUSH_TOKEN + "/" + String.valueOf(accountID) + "?token=" + token, 
				JSONParser.HttpMethod.DELETE,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONObject updatePushToken(String token, int putID, int accountID, int pdtID, String registrationID, boolean enabled) {
		Logger.verbose(mName, "updatePushToken");
		
		JSONObject jsonReply = null;
		
		JSONObject createRecordBody = null;
		try {
			createRecordBody = new JSONObject();
			createRecordBody.put("PutId", putID);
			createRecordBody.put("AccId", accountID);
			createRecordBody.put("PdtId", pdtID);
			createRecordBody.put("PutToken", registrationID);
			createRecordBody.put("PutEnabled", enabled);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(PUSH_TOKEN + "?token=" + token,
				JSONParser.HttpMethod.PUT,
				createRecordBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject createSocialPost(String token, int accountID, int sptID, int busID) {
		Logger.verbose(mName, "createPushToken");
		
		JSONObject jsonReply = null;
		
		JSONObject createRecordBody = null;
		try {
			createRecordBody = new JSONObject();
			createRecordBody.put("AccId", accountID);
			createRecordBody.put("SptId", sptID); // social post type - 1:Facebook-A, 2:Facebook, 3:Twitter
			createRecordBody.put("BusId", busID);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(SOCIAL_POST + "?token=" + token,
				JSONParser.HttpMethod.POST,
				createRecordBody,
				false);
		
		return jsonReply;
	}
	
	public static JSONObject getDataRevisionByID(String id) {
		Logger.verbose(mName, "getDataRevisionByID");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(DATA_REVISION + "/" + id, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject getDataRevisionByName(String name) {
		Logger.verbose(mName, "getDataRevisionByName");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(DATA_REVISION + "?name=" + name, 
				JSONParser.HttpMethod.GET,
				null,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject subscription(String firstName, String lastName, String email, String phone, 
										  String billFirstName, String billLastName, String billAddress, 
										  String billCity, String billState, String billZip, String cardNumber, 
										  int cardCode, int cardExpireMonth, int cardExpireYear, String promoCode, 
										  int prdID) {
		Logger.verbose(mName, "subscription");
		
		JSONObject jsonReply = null;
		
		JSONObject subscriptionBody = null;
		try {
			subscriptionBody = new JSONObject();
			subscriptionBody.put("AccFName", firstName);
			subscriptionBody.put("AccLName", lastName);
			subscriptionBody.put("AccEMail", email);
			subscriptionBody.put("AccPhone", phone);
			subscriptionBody.put("BillFName", billFirstName);
			subscriptionBody.put("BillLName", billLastName);
			subscriptionBody.put("BillAddress", billAddress);
			subscriptionBody.put("BillCity", billCity);
			subscriptionBody.put("BillState", billState);
			subscriptionBody.put("BillZip", billZip);
			subscriptionBody.put("CardNumber", cardNumber);
			subscriptionBody.put("CardCode", cardCode);
			subscriptionBody.put("CardExpMonth", cardExpireMonth);
			subscriptionBody.put("CardExpYear", cardExpireYear);
			subscriptionBody.put("PromoCode", promoCode);
			subscriptionBody.put("PrdID", prdID);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		// upon success, we expect a user account and password
		jsonReply = JSONParser.getJSONObjectFromURL(SUBSCRIPTION,
				JSONParser.HttpMethod.POST,
				subscriptionBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject cancelSubscription(String account, String password, String token) {
		Logger.verbose(mName, "cancelSubscription");
		
		JSONObject jsonReply = null;
		
		JSONObject cancelBody = null;
		try {
			cancelBody = new JSONObject();
			cancelBody.put("Account", account);
			cancelBody.put("Password", Utils.md5(password + "-" + Constants.ACCOUNT_HASH_KEY));
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(SUBSCRIPTION + "?token=" + token, 
				JSONParser.HttpMethod.DELETE,
				cancelBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject freeSignUp(String firstName, String lastName, String email, String phone, 
			  String gender, String birthdate, String promoCode) {
		Logger.verbose(mName, "freesignup");
		
		JSONObject jsonReply = null;
		
		JSONObject freeSignUpBody = null;
		try {
			freeSignUpBody = new JSONObject();
			freeSignUpBody.put("AccFName", firstName);
			freeSignUpBody.put("AccLName", lastName);
			freeSignUpBody.put("AccEMail", email);
			freeSignUpBody.put("AccPhone", phone);
			freeSignUpBody.put("AccGender", gender);
			freeSignUpBody.put("AccBirthdate", birthdate);
			freeSignUpBody.put("PromoCode", promoCode);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		// upon success, we expect a user account and password
		jsonReply = JSONParser.getJSONObjectFromURL(FREE_SIGN_UP,
				JSONParser.HttpMethod.POST,
				freeSignUpBody,
				true);
		
		return jsonReply;
	}
	
	public static JSONObject freeSignUp2(String firstName, String lastName, String email, String zip, 
			  String gender, String birthdate, String promoCode) {
		Logger.verbose(mName, "freesignup2");
		
		JSONObject jsonReply = null;
		
		JSONObject freeSignUpBody = null;
		try {
			freeSignUpBody = new JSONObject();
			freeSignUpBody.put("AccFName", firstName);
			freeSignUpBody.put("AccLName", lastName);
			freeSignUpBody.put("AccEMail", email);
			freeSignUpBody.put("AccZip", zip);
			freeSignUpBody.put("AccGender", gender);
			freeSignUpBody.put("AccBirthdate", birthdate);
			freeSignUpBody.put("PromoCode", promoCode);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		// upon success, we expect a user account and password
		jsonReply = JSONParser.getJSONObjectFromURL(FREE_SIGN_UP2,
				JSONParser.HttpMethod.POST,
				freeSignUpBody,
				true);
		
		return jsonReply;
	}

	public static JSONObject freeSignUp3(String firstName, String lastName, String email, String zip,
										 String gender, String birthdate, String promoCode, String facebookID) {
		Logger.verbose(mName, "freesignup3");

		JSONObject jsonReply = null;

		JSONObject freeSignUpBody = null;
		try {
			freeSignUpBody = new JSONObject();
			freeSignUpBody.put("AccFName", firstName);
			freeSignUpBody.put("AccLName", lastName);
			freeSignUpBody.put("AccEMail", email);
			freeSignUpBody.put("AccZip", zip);
			freeSignUpBody.put("AccGender", gender);
			freeSignUpBody.put("AccBirthdate", birthdate);
			freeSignUpBody.put("PromoCode", promoCode);
			freeSignUpBody.put("AccFacebookIdentifier", facebookID);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}

		// upon success, we expect a user account and password
		jsonReply = JSONParser.getJSONObjectFromURL(FREE_SIGN_UP3,
				JSONParser.HttpMethod.POST,
				freeSignUpBody,
				true);

		return jsonReply;
	}
	
	public static JSONObject requestPassword(String email) {
		Logger.verbose(mName, "requestPassword");
		
		JSONObject jsonReply = null;
		
		jsonReply = JSONParser.getJSONObjectFromURL(SUBSCRIPTION + "?emailAddress=" + email, 
				JSONParser.HttpMethod.GET,
				null,
				false);
		
		return jsonReply;
	}
	
	public static JSONObject proximity(String token, String pushToken, double latitude, double longitude) {
		Logger.verbose(mName, "proximity");
		
		JSONObject jsonReply = null;
		
		JSONObject proximityBody = null;
		try {
			proximityBody = new JSONObject();
			proximityBody.put("PutToken", pushToken);
			proximityBody.put("Latitude", latitude);
			proximityBody.put("Longitude", longitude);
		} catch (JSONException e) {
			throw new RuntimeException(e);
		}
		
		jsonReply = JSONParser.getJSONObjectFromURL(PROXIMITY + "?token=" + token,
				JSONParser.HttpMethod.POST,
				proximityBody,
				false);
		
		return jsonReply;
	}

	public static JSONArray getGallery(int busID) {
		Logger.verbose(mName, "getGallery");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(GALLERY + "/" + String.valueOf(busID),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getMenu(int busID) {
		Logger.verbose(mName, "getMenu");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(MENU + "/" + String.valueOf(busID),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getSummaryStats(String token, int id) {
		Logger.verbose(mName, "getSummaryStats");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(SUMMARY_STATS + "/" + String.valueOf(id) + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatFavorite(String token, int id) {
		Logger.verbose(mName, "getStatFavorite");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_FAVORITE + "/" + String.valueOf(id) + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getSummaryAnalytics(String token, int id) {
		Logger.verbose(mName, "getSummaryAnalytics");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(SUMMARY_ANALYTICS + "/" + String.valueOf(id) + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getSummaryAnalytics(String token, int id, int busId) {
		Logger.verbose(mName, "getSummaryAnalytics (business)");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(SUMMARY_ANALYTICS + "/" + String.valueOf(id) +
						"?token=" + token + "&busID=" + String.valueOf(busId),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatRedemption(String token, int id, int scopeid) {
		Logger.verbose(mName, "getStatRedemption");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_REDEMPTION + "/" + String.valueOf(id)
						+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatRedemption(String token, int id, int scopeid, int accid) {
		Logger.verbose(mName, "getStatRedemption (accid)");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_REDEMPTION + "/" + String.valueOf(id)
						+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid)
						+ "&accID=" + String.valueOf(accid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatCheckIn(String token, int id, int scopeid) {
		Logger.verbose(mName, "getStatCheckIn");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_CHECK_IN + "/" + String.valueOf(id)
					+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatCheckIn(String token, int id, int scopeid, int accid) {
		Logger.verbose(mName, "getStatCheckIn (accid)");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_CHECK_IN + "/" + String.valueOf(id)
						+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid)
						+ "&accID=" + String.valueOf(accid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatRating(String token, int id) {
		Logger.verbose(mName, "getStatRating");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_RATING + "/" + String.valueOf(id) + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getStatTip(String token, int id) {
		Logger.verbose(mName, "getStatTip");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(STAT_TIP + "/" + String.valueOf(id) + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getSummaryRedemptions(String token, int id, int scopeid) {
		Logger.verbose(mName, "getSummaryRedemptions");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(SUMMARY_REDEMPTIONS + "/" + String.valueOf(id)
						+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONArray getSummaryCheckIns(String token, int id, int scopeid) {
		Logger.verbose(mName, "getSummaryCheckIns");

		JSONArray jsonReply = null;

		jsonReply = JSONParser.getJSONArrayFromURL(SUMMARY_CHECK_INS + "/" + String.valueOf(id)
						+ "?token=" + token + "&scopeid=" + String.valueOf(scopeid),
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}

	public static JSONObject getStatPermissions(String token) {
		Logger.verbose(mName, "getStatPermissions");

		JSONObject jsonReply = null;

		jsonReply = JSONParser.getJSONObjectFromURL(STAT_PERMISSIONS + "?token=" + token,
				JSONParser.HttpMethod.GET,
				null,
				true);

		return jsonReply;
	}
}
