package com.unitethiscity.data;

import java.io.Serializable;
import java.util.ArrayList;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class AccountContext implements Serializable {

	private static final long serialVersionUID = 5122122248468643096L;

	private String mName = Constants.ACCOUNT_CONTEXT;
	
	private ArrayList<Integer> mBusinessRoles;
	private ArrayList<Integer> mCharityRoles;
	private ArrayList<Integer> mAssociateRoles;
	private Integer mAccountID;
	private Integer mCityID;
	private String mAccountGUID;
	private String mToken;
	private String mAccountEmail;
	private String mAccountFirstName;
	private String mAccountLastName;
	private boolean mIsAdmin;
	private boolean mIsSalesRep;
	private boolean mIsMember;
	private boolean mTwitterEnabled;
	private boolean mFacebookEnabled;
	
	public AccountContext() {
		Logger.verbose(mName, "empty account context created");
		mBusinessRoles = new ArrayList<Integer>();
		mCharityRoles = new ArrayList<Integer>();
		mAssociateRoles = new ArrayList<Integer>();
		mAccountID = 0;
		mCityID = 0;
		mAccountGUID = null;
		mToken = null;
		mAccountEmail = null;
		mAccountFirstName = null;
		mAccountLastName = null;
		mIsAdmin = false;
		mIsSalesRep = false;
		mIsMember = false;
		mTwitterEnabled = false;
		mFacebookEnabled = false;
	}
	
	public void addBusinessRole(Integer role) {
		mBusinessRoles.add(role);
	}
	
	public ArrayList<Integer> getBusinessRoles() {
		return mBusinessRoles;
	}
	
	public void clearBusinessRoles() {
		mBusinessRoles.clear();
	}
	
	public boolean hasBusinessRoles() {
		return !mBusinessRoles.isEmpty();
	}
	
	public void addCharityRole(Integer role) {
		mCharityRoles.add(role);
	}
	
	public ArrayList<Integer> getCharityRoles() {
		return mCharityRoles;
	}
	
	public void clearCharityRoles() {
		mCharityRoles.clear();
	}
	
	public boolean hasCharityRoles() {
		return !mCharityRoles.isEmpty();
	}
	
	public void addAssociateRole(Integer role) {
		mAssociateRoles.add(role);
	}
	
	public ArrayList<Integer> getAssociateRoles() {
		return mAssociateRoles;
	}
	
	public void clearAssociateRoles() {
		mAssociateRoles.clear();
	}
	
	public boolean hasAssociateRoles() {
		return !mAssociateRoles.isEmpty();
	}
	
	public void setAccountID(Integer accountID) {
		mAccountID = accountID;
	}

	public Integer getAccountID() {
		return mAccountID;
	}

	public void setCityID(Integer id) {
		mCityID = id;
	}
	
	public Integer getCityID() {
		return mCityID;
	}
	
	public void setAccountGUID(String guid) {
		mAccountGUID = guid;
	}
	
	public String getAccountGUID() {
		return mAccountGUID;
	}
	
	public void setToken(String token) {
		mToken = token;
	}
	
	public String getToken() {
		return mToken;
	}
	
	public void setAccountEmail(String email) {
		mAccountEmail = email;
	}
	
	public String getAccountEmail() {
		return mAccountEmail;
	}
	
	public void setAccountFirstName(String name) {
		mAccountFirstName = name;
	}
	
	public String getAccountFirstName() {
		return mAccountFirstName;
	}
	
	public void setAccountLastName(String name) {
		mAccountLastName = name;
	}
	
	public String getAccountLastName() {
		return mAccountLastName;
	}
	
	public String getAccountFullName() {
		return mAccountFirstName + " " + mAccountLastName;
	}
	
	public String getSignature() {
		return mAccountFirstName + " " + mAccountLastName.substring(0, 1) + ".";
	}
	
	public void setAdminStatus(boolean status) {
		mIsAdmin = status;
	}
	
	public boolean getAdminStatus() {
		return mIsAdmin;
	}
	
	public void setSalesRepStatus(boolean status) {
		mIsSalesRep = status;
	}
	
	public boolean getSalesRepStatus() {
		return mIsSalesRep;
	}
	
	public void setMemberStatus(boolean status) {
		mIsMember = status;
	}
	
	public boolean getMemberStatus() {
		return mIsMember;
	}
	
	public void setTwitter(boolean state) {
		mTwitterEnabled = state;
	}
	
	public boolean getTwitter() {
		return mTwitterEnabled;
	}
	
	public void setFacebook(boolean state) {
		mFacebookEnabled = state;
	}
	
	public boolean getFacebook() {
		return mFacebookEnabled;
	}
}
