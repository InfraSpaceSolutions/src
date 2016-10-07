/*
 * Copyright (C) 2012 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.unitethiscity.general;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutput;
import java.io.ObjectOutputStream;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.List;

import com.unitethiscity.ui.MainActivity;
import com.unitethiscity.ui.UniteThisCity;

import android.annotation.TargetApi;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.res.AssetManager;
import android.graphics.Typeface;
import android.os.Build;
import android.os.StrictMode;
import android.widget.TextView;

/**
 * Class containing some static utility methods.
 */
public class Utils {
    private Utils() {};

    private static String mName = Constants.UTILS;
    
    public static Typeface mFontsStyle;
    
    @TargetApi(11)
    public static void enableStrictMode() {
        if (Utils.hasGingerbread()) {
            StrictMode.ThreadPolicy.Builder threadPolicyBuilder =
                    new StrictMode.ThreadPolicy.Builder()
                            .detectAll()
                            .penaltyLog();
            StrictMode.VmPolicy.Builder vmPolicyBuilder =
                    new StrictMode.VmPolicy.Builder()
                            .detectAll()
                            .penaltyLog();

            if (Utils.hasHoneycomb()) {
                threadPolicyBuilder.penaltyFlashScreen();
                vmPolicyBuilder
                        .setClassInstanceLimit(MainActivity.class, 1)
                        .setClassInstanceLimit(UniteThisCity.class, 1);
            }
            StrictMode.setThreadPolicy(threadPolicyBuilder.build());
            StrictMode.setVmPolicy(vmPolicyBuilder.build());
        }
    }

    public static boolean hasFroyo() {
        // Can use static final constants like FROYO, declared in later versions
        // of the OS since they are inlined at compile time. This is guaranteed behavior.
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.FROYO;
    }

    public static boolean hasGingerbread() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.GINGERBREAD;
    }

    public static boolean hasHoneycomb() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB;
    }

    public static boolean hasHoneycombMR1() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB_MR1;
    }

    public static boolean hasJellyBean() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN;
    }
    
    public static boolean hasJellyBeanMR1() {
    	return Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1;
    }

    public static byte[] serializeObject(Object o) { 
    	ByteArrayOutputStream bos = new ByteArrayOutputStream(); 

    	try { 
    		ObjectOutput out = new ObjectOutputStream(bos); 
    		out.writeObject(o); 
    		out.close(); 

    		// get the bytes of the serialized object 
    		byte[] buf = bos.toByteArray(); 

    		return buf; 
    	} catch(IOException ioe) { 
    		Logger.error(mName, "serializeObject error - " + ioe.toString());

    		return null; 
    	} 
    }
    
    public static Object deserializeObject(byte[] b) { 
    	try { 
    		ObjectInputStream in = new ObjectInputStream(new ByteArrayInputStream(b)); 
    		Object object = in.readObject(); 
    		in.close(); 

    		return object; 
    	} catch(ClassNotFoundException cnfe) { 
    		Logger.error(mName, "deserializeObject class not found error - " + cnfe.toString());

    		return null; 
    	} catch(IOException ioe) { 
    		Logger.error(mName, "deserializeObject IO error - " + ioe.toString());

    		return null; 
    	} 
    }
    
	public static boolean saveExists(Context ctx, String name) {
		String[] files = ctx.fileList();
		for(int i = 0; i < files.length; i++) {
			if(files[i].equals(name)) {
				return true;
			}
		}
		
		return false;
	}
	
	public static void save(Context ctx, String name, Object obj) throws IOException {
		// delete serialize object file if it exists
		delete(ctx, name);
		// save serialized object to new file
		FileOutputStream fos = ctx.openFileOutput(name, Context.MODE_PRIVATE);
		byte[] buffer = Utils.serializeObject(obj);
		if(buffer != null) {
			fos.write(buffer);
			fos.close();
		}
	}
	
	public static Object open(Context ctx, String name) throws IOException, ClassNotFoundException {
		// open serialized object file and deserialize it
		FileInputStream fis = ctx.openFileInput(name);

		ObjectInputStream is = new ObjectInputStream(fis);
		Object obj = is.readObject();
		is.close();
		return obj;
	}
	
	public static void delete(Context ctx, String name) {
		// delete serialize object file if it exists
		if(saveExists(ctx, name)) {
			ctx.deleteFile(name);
		}
	}
    
    public static final String md5(final String s) {
        try {
            // Create MD5 Hash
            MessageDigest digest = java.security.MessageDigest
                    .getInstance("MD5");
            digest.update(s.getBytes());
            byte messageDigest[] = digest.digest();

            // Create Hex String
            StringBuffer hexString = new StringBuffer();
            for (int i = 0; i < messageDigest.length; i++) {
                String h = Integer.toHexString(0xFF & messageDigest[i]);
                while (h.length() < 2)
                    h = "0" + h;
                hexString.append(h);
            }
            return hexString.toString();

        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
        return "";
    }
    
    public static int fiveRatingToTenRating(double five) {
    	int ten = 0;
    	
    	if(five >= 0.5 && five < 1.0) {
    		ten = 1;
    	}
    	else if(five >= 1.0 && five < 1.5) {
    		ten = 2;
    	}
    	else if(five >= 1.5 && five < 2.0) {
    		ten = 3;
    	}
    	else if(five >= 2.0 && five < 2.5) {
    		ten = 4;
    	}
    	else if(five >= 2.5 && five < 3.0) {
    		ten = 5;
    	}
    	else if(five >= 3.0 && five < 3.5) {
    		ten = 6;
    	}
    	else if(five >= 3.5 && five < 4.0) {
    		ten = 7;
    	}
    	else if(five >= 4.0 && five < 4.5) {
    		ten = 8;
    	}
    	else if(five >= 4.5 && five < 5.0) {
    		ten = 9;
    	}
    	else if(five == 5.0) {
    		ten = 10;
    	}
    	
    	return ten;
    }
    
    public static Constants.MenuType menuType(Constants.MenuID id) {
    	boolean mainMenu = false;
    	
    	mainMenu  = id == Constants.MenuID.HOME;
    	mainMenu |= id == Constants.MenuID.WALLET;
    	mainMenu |= id == Constants.MenuID.FAVORITE;
    	mainMenu |= id == Constants.MenuID.UTC;
    	mainMenu |= id == Constants.MenuID.INBOX;
    	mainMenu |= id == Constants.MenuID.SEARCH;
    	mainMenu |= id == Constants.MenuID.ACCOUNT;
    	
    	return mainMenu ? Constants.MenuType.MAIN : Constants.MenuType.SUB;
    }
    
    public static void TypeFace(TextView tv, AssetManager asm, String font){
        mFontsStyle = Typeface.createFromAsset(asm, font); 
        tv.setTypeface(mFontsStyle);
    }

	public static Intent createExplicitFromImplicitIntent(Context context, Intent implicitIntent) {
		// Retrieve all services that can match the given intent
		PackageManager pm = context.getPackageManager();
		List<ResolveInfo> resolveInfo = pm.queryIntentServices(implicitIntent, 0);
		if (resolveInfo == null ) {
			return null;
		}

		// Get component info and create ComponentName
		ResolveInfo serviceInfo = resolveInfo.get(0);
		String packageName = serviceInfo.serviceInfo.packageName;
		String className = serviceInfo.serviceInfo.name;
		ComponentName component = new ComponentName(packageName, className);

		// Create a new intent. Use the old one for extras and such reuse
		Intent explicitIntent = new Intent(implicitIntent);

		// Set the component to be explicit
		explicitIntent.setComponent(component);

		return explicitIntent;
	}

	public static boolean convertToBoolean(String value) {
		boolean returnValue = false;

		if ("1".equalsIgnoreCase(value) || "yes".equalsIgnoreCase(value) ||
				"true".equalsIgnoreCase(value) || "on".equalsIgnoreCase(value)) {
			returnValue = true;
		}

		return returnValue;
	}
}
