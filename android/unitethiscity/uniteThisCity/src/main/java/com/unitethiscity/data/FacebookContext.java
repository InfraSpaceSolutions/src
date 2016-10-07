package com.unitethiscity.data;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

/**
 * Created by Donald on 1/13/2016.
 */
public class FacebookContext {

    private String mName = Constants.FACEBOOK_CONTEXT;

    String mFirstName;
    String mLastName;
    String mEmail;
    String mGender;
    String mBirthday;
    String mUserID;
    String mAccount;
    String mPassword;

    public FacebookContext() {
        Logger.verbose(mName, "empty account context created");
        mFirstName = null;
        mLastName = null;
        mEmail = null;
        mGender = null;
        mBirthday = null;
        mUserID = null;
        String mAccount;
        String mPassword;
    }

    public FacebookContext(String firstName, String lastName, String email, String gender, String birthday, String userID) {
        mFirstName = firstName;
        mLastName = lastName;
        mEmail = email;
        mGender = gender;
        mBirthday = birthday;
        mUserID = userID;
    }

    public String getFirstName() { return mFirstName; }
    public String getLastName() { return mLastName; }
    public String getEmail() { return mEmail; }
    public String getGender() { return mGender; }
    public String getBirthday() { return mBirthday; }
    public String getUserID() { return mUserID; }
    public String getAccount() { return mAccount; }
    public void setAccount(String account) {
        mAccount = account;
    }
    public String getPassword() { return mPassword; }
    public void setPassword(String password) {
        mPassword = password;
    }
}
