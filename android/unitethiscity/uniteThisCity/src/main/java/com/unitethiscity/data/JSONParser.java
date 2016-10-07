package com.unitethiscity.data;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class JSONParser {

	private static String mName = Constants.JSON_PARSER;

    public static String USER_AGENT = "ANDROID";
	
	public static enum HttpMethod {
		GET,
		POST,
		DELETE,
		PUT
	}
	
    // constructor
    public JSONParser() {
    	
    }
 
    public static JSONObject getJSONObjectFromURL(String url, HttpMethod method, JSONObject body, boolean expectResponse) {
        InputStream is = null;
        JSONObject jObj = null;
        String json = "";
        String userAgent = USER_AGENT;
        
        HttpParams httpParams = new BasicHttpParams();
        HttpConnectionParams.setConnectionTimeout(httpParams, Constants.JSON_CONNECTION_TIMEOUT);
        HttpConnectionParams.setSoTimeout(httpParams, Constants.JSON_SOCKET_TIMEOUT);
        
        // Making HTTP request
        try {
            // defaultHttpClient
        	DefaultHttpClient httpclient = new DefaultHttpClient(httpParams);
        	HttpResponse httpResponse = null;
        	switch(method) {
        		case GET:
                    HttpGet httpget = new HttpGet(url);
                    httpget.setHeader("Accept", "application/json");
                    httpget.setHeader("Content-type", "application/json");
                    httpget.setHeader("User-Agent", userAgent);
                    httpResponse = httpclient.execute(httpget);
        			break;
        		case POST:
        			HttpPost httppost = new HttpPost(url);
        			if(body != null) {
        				Logger.verbose(mName, "request body - " + body.toString());
        				httppost.setEntity(new StringEntity(body.toString()));
        			}
    			    httppost.setHeader("Accept", "application/json");
    			    httppost.setHeader("Content-type", "application/json");
                    httppost.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httppost);
        			break;
        		case DELETE:
        			HttpDelete httpdel = new HttpDelete(url);
        			httpdel.setHeader("Accept", "application/json");
        			httpdel.setHeader("Content-type", "application/json");
                    httpdel.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httpdel);
        			break;
        		case PUT:
        			HttpPut httpput = new HttpPut(url);
        			httpput.setHeader("Accept", "application/json");
        			httpput.setHeader("Content-type", "application/json");
                    httpput.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httpput);
        		default:
        			Logger.error(mName, "Invalid HTTP request");
        			break;
        	}

        	if(httpResponse != null && expectResponse) {
                HttpEntity httpEntity = httpResponse.getEntity();
                if(httpEntity != null) {
                	is = httpEntity.getContent();
                }
        	}
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (ClientProtocolException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
 
        if(expectResponse) {
            try {
            	if(is != null) {
                    BufferedReader reader = new BufferedReader(new InputStreamReader(
                            is, "UTF-8"), 8);
                    StringBuilder sb = new StringBuilder();
                    String line = null;
                    while ((line = reader.readLine()) != null) {
                        sb.append(line + "\n");
                    }
                    is.close();
                    json = sb.toString();

        			Logger.verbose(mName, json);
            	}
            	else {
            		Logger.warn(mName, "expected response but entity was null");
            	}
            } catch (Exception e) {
                Logger.error(mName, "Error converting result " + e.toString());
            }
     
            if(json.compareTo("") != 0) {
                // try parse the string to a JSON object
                try {
                    jObj = new JSONObject(json);
                } catch (JSONException e) {
                	Logger.error(mName, "Error parsing data for object " + e.toString());
                }
            }
        }
 
        // return JSON String
        return jObj;
    }
    
    public static JSONArray getJSONArrayFromURL(String url, HttpMethod method, JSONObject body, boolean expectResponse) {
        InputStream is = null;
        JSONArray jArray = null;
        String json = "";
        String userAgent = USER_AGENT;
        
        HttpParams httpParams = new BasicHttpParams();
        HttpConnectionParams.setConnectionTimeout(httpParams, Constants.JSON_CONNECTION_TIMEOUT);
        HttpConnectionParams.setSoTimeout(httpParams, Constants.JSON_SOCKET_TIMEOUT);

        // Making HTTP request
        try {
        	DefaultHttpClient httpclient = new DefaultHttpClient(httpParams);
        	HttpResponse httpResponse = null;
        	switch(method) {
        		case GET:
                    HttpGet httpget = new HttpGet(url);
                    httpget.setHeader("Accept", "application/json");
                    httpget.setHeader("Content-type", "application/json");
                    httpget.setHeader("User-Agent", userAgent);
                    httpResponse = httpclient.execute(httpget);
        			break;
        		case POST:
        			HttpPost httppost = new HttpPost(url);
        			if(body != null) {
        				Logger.verbose(mName, "request body - " + body.toString());
        				httppost.setEntity(new StringEntity(body.toString()));
        			}
    			    httppost.setHeader("Accept", "application/json");
    			    httppost.setHeader("Content-type", "application/json");
                    httppost.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httppost);
        			break;
        		case DELETE:
        			HttpDelete httpdel = new HttpDelete(url);
        			httpdel.setHeader("Accept", "application/json");
        			httpdel.setHeader("Content-type", "application/json");
                    httpdel.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httpdel);
        			break;
        		default:
        			Logger.error(mName, "Invalid HTTP request");
        			break;
        	}

        	if(httpResponse != null && expectResponse) {
                HttpEntity httpEntity = httpResponse.getEntity();
                if(httpEntity != null) {
                	is = httpEntity.getContent();
                }
        	}
        } catch (UnknownHostException e) {
        	e.printStackTrace();
    	} catch (SocketTimeoutException e) {
        	e.printStackTrace();
    	} catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (ClientProtocolException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        if(expectResponse) {
            try {
            	if(is != null) {
                    BufferedReader reader = new BufferedReader(new InputStreamReader(
                            is, "UTF-8"), 8);
                    StringBuilder sb = new StringBuilder();
                    String line = null;
                    while ((line = reader.readLine()) != null) {
                        sb.append(line + "\n");
                    }
                    is.close();
                    json = sb.toString();

        			Logger.verbose(mName, json);
            	}
            	else {
            		Logger.warn(mName, "expected response but entity was null");
            	}
            } catch (Exception e) {
            	Logger.error(mName, "Error converting result " + e.toString());
            }

            if(json.compareTo("") != 0) {
                try {
        			jArray = new JSONArray(json);
        		} catch (JSONException e) {
        			Logger.error(mName, "Error parsing data for array " + e.toString());
        		}
            }
        }
        
        return jArray;
    }
    
    public static int getIntegerFromURL(String url, HttpMethod method, JSONObject body, boolean expectResponse) {
        InputStream is = null;
        int intReply = 0;
        String userAgent = USER_AGENT;
        
        HttpParams httpParams = new BasicHttpParams();
        HttpConnectionParams.setConnectionTimeout(httpParams, Constants.JSON_CONNECTION_TIMEOUT);
        HttpConnectionParams.setSoTimeout(httpParams, Constants.JSON_SOCKET_TIMEOUT);
        
        // Making HTTP request
        try {
            // defaultHttpClient
        	DefaultHttpClient httpclient = new DefaultHttpClient(httpParams);
        	HttpResponse httpResponse = null;
        	switch(method) {
        		case GET:
                    HttpGet httpget = new HttpGet(url);
                    httpget.setHeader("Accept", "application/json");
                    httpget.setHeader("Content-type", "application/json");
                    httpget.setHeader("User-Agent", userAgent);
                    httpResponse = httpclient.execute(httpget);
        			break;
        		case POST:
        			HttpPost httppost = new HttpPost(url);
        			if(body != null) {
        				Logger.verbose(mName, "request body - " + body.toString());
        				httppost.setEntity(new StringEntity(body.toString()));
        			}
    			    httppost.setHeader("Accept", "application/json");
    			    httppost.setHeader("Content-type", "application/json");
                    httppost.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httppost);
        			break;
        		case DELETE:
        			HttpDelete httpdel = new HttpDelete(url);
        			httpdel.setHeader("Accept", "application/json");
        			httpdel.setHeader("Content-type", "application/json");
                    httpdel.setHeader("User-Agent", userAgent);
        			httpResponse = httpclient.execute(httpdel);
        			break;
        		default:
        			Logger.error(mName, "Invalid HTTP request");
        			break;
        	}

        	if(httpResponse != null && expectResponse) {
                HttpEntity httpEntity = httpResponse.getEntity();
                if(httpEntity != null) {
                	is = httpEntity.getContent();
                }
        	}
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (ClientProtocolException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
 
        if(expectResponse) {
            try {
            	if(is != null) {
                    BufferedReader reader = new BufferedReader(new InputStreamReader(
                            is, "UTF-8"), 8);
                    StringBuilder sb = new StringBuilder();
                    String line = null;
                    while ((line = reader.readLine()) != null) {
                        sb.append(line + "\n");
                    }
                    is.close();
                    intReply = Integer.valueOf(sb.toString());
            	}
            	else {
            		Logger.warn(mName, "expected response but entity was null");
            	}
            } catch (Exception e) {
                Logger.error(mName, "Error converting result " + e.toString());
            }
        }
 
        // return integer reply
        return intReply;
    }
}