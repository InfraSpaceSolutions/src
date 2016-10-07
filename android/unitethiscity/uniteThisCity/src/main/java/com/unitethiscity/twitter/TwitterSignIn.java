package com.unitethiscity.twitter;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

import twitter4j.Twitter;
import twitter4j.TwitterException;
import twitter4j.TwitterFactory;
import twitter4j.auth.AccessToken;
import twitter4j.auth.RequestToken;

public class TwitterSignIn {

	private String mName = Constants.TWITTER_SIGN_IN;
	
	public String start() {
		String defaultError = "Failed to login to Twitter";
		
		// The factory instance is re-useable and thread safe.
		Twitter twitter = TwitterFactory.getSingleton();
		twitter.setOAuthConsumer("[consumer key]", "[consumer secret]");
		RequestToken requestToken = null;
		try {
			requestToken = twitter.getOAuthRequestToken();
		} catch (TwitterException e1) {
			e1.printStackTrace();
			return defaultError;
		}
		AccessToken accessToken = null;
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
		while (null == accessToken) {
			Logger.info(mName, "Open the following URL and grant access to your account:");
			Logger.info(mName, requestToken.getAuthorizationURL());
			Logger.info(mName, "Enter the PIN (if aviailable) or just hit enter.[PIN]:");
			String pin = null;
			try {
				pin = br.readLine();
			} catch (IOException e) {
				e.printStackTrace();
				return defaultError;
			}
			try {
				accessToken = twitter.getOAuthAccessToken();
			} catch (TwitterException te) {
				if(401 == te.getStatusCode()){
					Logger.error(mName, "Unable to get the access token.");
					return defaultError;
				}
				else {
					te.printStackTrace();
					return defaultError;
				}
			}
		}
		// persist to the accessToken for future reference.
		try {
			storeAccessToken(twitter.verifyCredentials().getId() , accessToken);
		} catch (TwitterException e) {
			e.printStackTrace();
		}

		//Status status = twitter.updateStatus(args[0]);
		//System.out.println("Successfully updated the status to [" + status.getText() + "].");
		
		return null;
	}

	private static void storeAccessToken(long useId, AccessToken accessToken) {
		//store accessToken.getToken()
		//store accessToken.getTokenSecret()
	}
}
