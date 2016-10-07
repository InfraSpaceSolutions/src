package com.unitethiscity.ui;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.unitethiscity.R;
import com.unitethiscity.data.DataManager;
import com.unitethiscity.data.LocationContextParser;
import com.unitethiscity.data.UTCLocation;
import com.unitethiscity.data.DataManager.ScanTask;
import com.unitethiscity.general.Constants;
import com.unitethiscity.general.Logger;

public class UTCDialogFragment extends DialogFragment {
	
	private String mName = Constants.UTC_DIALOG_FRAGMENT;
	
	private boolean mUnified = false;
	private boolean mCheckIn = false;
	private boolean mRedeem = false;
    
    public static UTCDialogFragment newInstance(String name, int locationID, int businessID, String data, 
    		String title, String description, String dealAmount, boolean success) {
    	UTCDialogFragment frag = new UTCDialogFragment();
        Bundle args = new Bundle();
        args.putString("name", name);
        if(locationID != -1) {
        	args.putInt("locid", locationID);
        }
        if(businessID != -1) {
        	args.putInt("busid", businessID);
        }
        if(name.equals("RedeemConfirmDialog") && success) {
        	args.putString("dealamount", data);
        }
        if((name.equals("RedeemDialog") || name.equals("UnifiedRedeemDialog") || 
        		name.equals("BusinessDialog")) && success) {
        	// no longer using QR codes
        	//args.putString("qrcodedata", data);
        }
        if(!success) {
        	args.putString("error", data);
        }
        if(title != null) {
        	args.putString("title", title);
        }
        if(description != null) {
        	args.putString("description", description);
        }
        if(dealAmount != null && dealAmount != "") {
        	args.putString("dealamount", dealAmount);
        }
        frag.setArguments(args);
        return frag;
    }
    
    public static UTCDialogFragment newInstance(String name, int locationID, int businessID, String businessImage,
    		String data, String title, String description, String dealAmount, boolean success) {
    	UTCDialogFragment frag = newInstance(name, locationID, businessID, data, title, description, dealAmount, success);
    	frag.getArguments().putString("busimage", businessImage);
        return frag;
    }
    
    // guest dialog constructor
    public static UTCDialogFragment newInstance(String name) {
    	UTCDialogFragment frag = new UTCDialogFragment();
        Bundle args = new Bundle();
        args.putString("name", name);
        frag.setArguments(args);
        return frag;
    }
    
    // warning dialog constructor
    public static UTCDialogFragment newInstance(String name, String title, String message) {
    	UTCDialogFragment frag = new UTCDialogFragment();
        Bundle args = new Bundle();
        args.putString("name", name);
        args.putString("title", title);
        args.putString("message", message);
        frag.setArguments(args);
        return frag;
    }
    
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {	
        AlertDialog ad = new AlertDialog.Builder(getActivity())
        	.setTitle("")
        	.show();
        ad.getWindow().setLayout(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        return ad;
    }
    
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
    	View view;
    	
    	String name = getArguments().getString("name");
    	Logger.verbose(mName, name + " created");
    	
    	if(name.equals("RedeemDialog") || name.equals("UnifiedRedeemDialog")) {
    		view = inflater.inflate(R.layout.fragment_dialog_redeem, container);
    	}
    	else if(name.equals("BusinessDialog")) {
    		view = inflater.inflate(R.layout.fragment_dialog_business, container);
    	}
    	else if(name.contains("Guest")) {
    		view = inflater.inflate(R.layout.fragment_dialog_guest, container);
    	}
    	else if(name.equals("PINDialog") || name.equals("BusinessPINDialog")) {
    		view = inflater.inflate(R.layout.fragment_dialog_pin, container);
    	}
    	else if(name.equals("WarningDialog")) {
    		view = inflater.inflate(R.layout.fragment_dialog_warning, container);
    	}
    	else {
    		view = inflater.inflate(R.layout.fragment_dialog_confirm, container);
    	}
        
    	if(name.equals("RedeemDialog") || name.equals("UnifiedRedeemDialog")) {
        	if(getArguments().containsKey("title")) {
        		((TextView) view.findViewById(R.id.redeemDialogName))
        			.setText(getArguments().getString("title"));
        	}
    	}
    	else if(name.equals("BusinessDialog")) {
        	if(getArguments().containsKey("title")) {
        		((TextView) view.findViewById(R.id.businessDialogName))
        			.setText(getArguments().getString("title"));
        	}
    	}
    	else if(name.equals("WarningDialog")) {
        	if(getArguments().containsKey("title")) {
        		String title = getArguments().getString("title");
        		if(title.equals("")) {
        			view.findViewById(R.id.linearLayoutHeaderWarning).setVisibility(LinearLayout.GONE);
        			view.findViewById(R.id.textViewHeaderWarning).setVisibility(TextView.GONE);
        		}
        		else {
        			((TextView) view.findViewById(R.id.textViewHeaderWarning))
        				.setText(title);
        		}
        	}
        	if(getArguments().containsKey("message")) {
        		((TextView) view.findViewById(R.id.textViewDescriptionWarning))
        			.setText(getArguments().getString("message"));
        	}
    	}
    	else {
        	if(getArguments().containsKey("title")) {
        		((TextView) view.findViewById(R.id.textViewSuccessStateConfirm))
        			.setText(getArguments().getString("title"));
        	}
    	}
    	
    	// redeem dialog - lets user scan for redemption or check-in
        if(name.equals("RedeemDialog") || name.equals("UnifiedRedeemDialog")) {
        	// no longer using QR code
        	/*
        	int dimension = 125;
        	
        	/*
            ImageView qrImage = (ImageView) view.findViewById(R.id.redeemDialogQRCode);
            QRCodeEncoder qrEncoder = new QRCodeEncoder(getArguments().getString("qrcodedata"), 
            		null, 
            		Contents.Type.TEXT,
            		BarcodeFormat.QR_CODE.toString(),
            		dimension);
            Bitmap qrBitmap = Bitmap.createBitmap(dimension, dimension, Bitmap.Config.ARGB_8888);
    		try {
    			qrBitmap = qrEncoder.encodeAsBitmap();
    		} catch (WriterException e) {
    			e.printStackTrace();
    		}
    		
            qrImage.setImageBitmap(qrBitmap);
            */
        	
            // button listeners - we need to set up our unified action if we are currently 
            // inside a unified redeem dialog
            if(name.equals("RedeemDialog")) {
            	
                final Button redeemButton = (Button) view.findViewById(R.id.buttonRedeemRedeem);
                redeemButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	// no longer using scanner
                    	/*
                    	DataManager.getInstance().setScanTask(ScanTask.REDEEM);
                    	DataManager.getInstance().setScanData(null);
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    	
                    	DataManager.getInstance().setScanTask(ScanTask.REDEEM);
                    	DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
                				null, Constants.Role.MEMBER.getValue(), -1, false, true);
                    }
                });
                
                final Button checkInButton = (Button) view.findViewById(R.id.buttonRedeemCheckIn);
                checkInButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	// no longer using scanner
                    	/*
                    	DataManager.getInstance().setScanTask(ScanTask.CHECKIN);
                    	DataManager.getInstance().setScanData(null);
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    	
                    	DataManager.getInstance().setScanTask(ScanTask.CHECKIN);
                    	DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(),
                				null, Constants.Role.MEMBER.getValue(), -1, true, false);
						dismiss();
                    }
                });
            }
            else if(name.equals("UnifiedRedeemDialog")) {
            	
                final Button redeemButton = (Button) view.findViewById(R.id.buttonRedeemRedeem);
                redeemButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	DataManager.getInstance().setScanTask(ScanTask.UNIFIED_ACTION);
                    	DataManager.getInstance().setScanData(null);
                    	DataManager.getInstance().getMainFragment().setUnifiedActionType(false, true);
                    	/* scanner removed
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    }
                });
                
                final Button checkInButton = (Button) view.findViewById(R.id.buttonRedeemCheckIn);
                checkInButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	DataManager.getInstance().setScanTask(ScanTask.UNIFIED_ACTION);
                    	DataManager.getInstance().setScanData(null);
                    	DataManager.getInstance().getMainFragment().setUnifiedActionType(true, false);
                    	/* scanner removed
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    }
                });
                
            }
            
            if(name.equals("RedeemDialog")) {
            	if(getArguments().containsKey("title")) {
            		((TextView) view.findViewById(R.id.redeemDialogName))
            			.setText(getArguments().getString("title"));
            	}
            	
            	if(getArguments().containsKey("dealamount")) {
            		((TextView) view.findViewById(R.id.redeemDialogAmount))
            			.setText(getArguments().getString("dealamount"));
            	}
            	
            	TextView redeemDescription = (TextView) view.findViewById(R.id.redeemDialogDescription);
                UTCLocation thisLocation = DataManager.getInstance().getLocationContext();
                String newText = "";
                if(thisLocation.containsKey(LocationContextParser.JSON_TAG_MY_IS_REDEEMED)) {
                	if(Boolean.valueOf(thisLocation.get(LocationContextParser.JSON_TAG_MY_IS_REDEEMED)) == true) {
                		newText += "Redeemed " + thisLocation.get(LocationContextParser.JSON_TAG_MY_REDEEM_DATE);
                	}
                }
                if(thisLocation.containsKey(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN)) {
                	if(Boolean.valueOf(thisLocation.get(LocationContextParser.JSON_TAG_MY_IS_CHECKED_IN)) == true) {
                		newText += "\nChecked In " + thisLocation.get(LocationContextParser.JSON_TAG_MY_CHECK_IN_TIME);
                	}
                }
                
                if(!newText.equals("")) {
                    if(!newText.contains("Redeemed")) {
                    	newText = redeemDescription.getText().toString() + newText;
                    }
                	redeemDescription.setText(newText);
                }
            }
            else {
            	((TextView) view.findViewById(R.id.redeemDialogName))
            		.setVisibility(TextView.GONE);
            }
        }
        else if(name.equals("BusinessDialog")) {
            	int dimension = 125;
            	
                ImageView qrImage = (ImageView) view.findViewById(R.id.businessDialogQRCode);
                Bitmap qrBitmap = Bitmap.createBitmap(dimension, dimension, Bitmap.Config.ARGB_8888);
                /* previous QR code generation
                QRCodeEncoder qrEncoder = new QRCodeEncoder(getArguments().getString("qrcodedata"), 
                		null, 
                		Contents.Type.TEXT,
                		BarcodeFormat.QR_CODE.toString(),
                		dimension);
                Bitmap qrBitmap = Bitmap.createBitmap(dimension, dimension, Bitmap.Config.ARGB_8888);
        		try {
        			qrBitmap = qrEncoder.encodeAsBitmap();
        		} catch (WriterException e) {
        			e.printStackTrace();
        		}
        		*/
                
                qrImage.setImageBitmap(qrBitmap);
                
                final Button redeemButton = (Button) view.findViewById(R.id.buttonBusinessRedeem);
                redeemButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	DataManager.getInstance().setScanTask(ScanTask.REDEEM);
                    	DataManager.getInstance().setScanData(null);
                    	/* scanner removed
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    }
                });
                
                final Button checkInButton = (Button) view.findViewById(R.id.buttonBusinessCheckIn);
                checkInButton.setOnClickListener(new View.OnClickListener() {
                    public void onClick(View v) {
                        // Perform action on click
                    	DataManager.getInstance().setScanTask(ScanTask.CHECKIN);
                    	DataManager.getInstance().setScanData(null);
                    	/* scanner removed
                    	Intent scanner = new Intent(getActivity(), ScannerActivity.class);
	    				try {
	    					startActivity(scanner);
	    				}
	    				catch(ActivityNotFoundException anfe) {
	    					anfe.printStackTrace();
	    				}
	    				*/
                    }
                });
                
            	if(getArguments().containsKey("title")) {
            		((TextView) view.findViewById(R.id.businessDialogName))
            			.setText(getArguments().getString("title"));
            	}
        }
        else if(name.contains("Guest")) { // guest-specific dialogs, such as when one tries to post areview
        	
            final Button cancelButton = (Button) view.findViewById(R.id.buttonSignInGuest);
            cancelButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					UTCDialogFragment.this.dismiss();
					((MainActivity) getActivity()).setAccountFragment();
				}
			});
        	
            final Button okButton = (Button) view.findViewById(R.id.buttonJoinNowGuest);
            okButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					UTCDialogFragment.this.dismiss();
					((MainActivity) getActivity()).setSubscribeFragment();
				}
			});
            
            TextView header = (TextView) view.findViewById(R.id.textViewHeaderGuest);
            TextView dialogName = (TextView) view.findViewById(R.id.textViewGuest);
            Resources res = getActivity().getResources();
        	if(name.equals("GuestRedeemDialog")) {
        		header.setText(res.getString(R.string.dialog_redeem_header));
        		dialogName.setText(res.getString(R.string.dialog_redeem));
        	}
        	else if(name.equals("GuestReviewDialog")) {
        		header.setText(res.getString(R.string.dialog_reviews_header));
        		dialogName.setText(res.getString(R.string.dialog_reviews));
        	}
        	else if(name.equals("GuestRateDialog")) {
        		header.setText(res.getString(R.string.dialog_ratings_header));
        		dialogName.setText(res.getString(R.string.dialog_ratings));
        	}
        	else if(name.equals("GuestFavoriteDialog")) {
        		header.setText(res.getString(R.string.dialog_favorites_header));
        		dialogName.setText(res.getString(R.string.dialog_favorites));
        	}
        	else if(name.equals("GuestNotificationsDialog")) {
        		header.setText(res.getString(R.string.dialog_notifications_header));
        		dialogName.setText(res.getString(R.string.dialog_notifications));
        	}
        }
        else if(name.equals("PINDialog") || name.equals("BusinessPINDialog")) { // PIN request dialog for redemption
        	
            final EditText et = (EditText) view.findViewById(R.id.editTextPIN);
            et.addTextChangedListener(new TextWatcher() {

				@Override
				public void afterTextChanged(Editable s) {
					
				}

				@Override
				public void beforeTextChanged(CharSequence s, int start,
						int count, int after) {
					
				}

				@Override
				public void onTextChanged(CharSequence s, int start,
						int before, int count) {
					DataManager.getInstance().setPIN(s.toString());
				}
            	
            });
            // Show soft keyboard automatically
            et.requestFocus();
            getDialog().getWindow().clearFlags(
                    WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE
                    | WindowManager.LayoutParams.FLAG_ALT_FOCUSABLE_IM);
            getDialog().getWindow().setSoftInputMode(
                    WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);

            final Button submitButton = (Button) view.findViewById(R.id.buttonSubmitPIN);
            if(name.equals("PINDialog")) {
                submitButton.setOnClickListener(new View.OnClickListener() {
    				
    				@Override
    				public void onClick(View v) {
    					/* not using the scanner
    			    	String scanData = DataManager.getInstance().getScanData();
    			    	if(scanData != null) {
    			    		Logger.verbose(mName, "reusing scan data " + scanData);

    			    		if(!mUnified) {
    				    		DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(), 
    				    				scanData, Constants.Role.MEMBER.getValue(), -1);
    			    		}
    			    		else {
    				    		DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(), 
    				    				scanData, Constants.Role.MEMBER.getValue(), -1, mCheckIn, mRedeem);
    			    		}
    			    	}
    			    	else {
    			    		Toast.makeText(getActivity(), "Corrupted scan data, please rescan QR code", Toast.LENGTH_SHORT).show();
    			    	}
    			    	*/

						DataManager.getInstance().setPIN(et.getText().toString());
    					DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(), 
			    				"N/A", Constants.Role.MEMBER.getValue(), -1);
    				}
    			});
            }
            else if(name.equals("BusinessPINDialog")) {
                submitButton.setOnClickListener(new View.OnClickListener() {
    				
    				@Override
    				public void onClick(View v) {
    			    	String scanData = DataManager.getInstance().getScanData();
    			    	if(scanData != null) {
    			    		Logger.verbose(mName, "reusing scan data " + scanData);

    			    		DataManager.getInstance().executeCheckInOrRedeemTask((MainActivity) getActivity(), 
    				    				scanData, Constants.Role.BUSINESS.getValue(),
    				    				DataManager.getInstance().getBusinessScanLocationID());
    			    	}
    			    	else {
    			    		Toast.makeText(getActivity(), "Corrupted scan data, please rescan QR code", Toast.LENGTH_SHORT).show();
    			    	}
    				}
    			});
            }
            
            final Button cancelButton = (Button) view.findViewById(R.id.buttonCancelPIN);
            cancelButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					if(DataManager.getInstance().getMainFragment().isAdded()) {
						DataManager.getInstance().getMainFragment().startImageSwitcherTask();
					}
					DataManager.getInstance().setPIN("");
					UTCDialogFragment.this.dismiss();
				}
			});
        }
        else if(name.equals("WarningDialog")) {
            final Button okButton = (Button) view.findViewById(R.id.buttonOKWarning);
            okButton.setOnClickListener(new View.OnClickListener() {
				
				@Override
				public void onClick(View v) {
					UTCDialogFragment.this.dismiss();
				}
			});
        }
        else { // success or fail confirmation dialogs
            Resources res = getActivity().getResources();

            // business name
            ((TextView) view.findViewById(R.id.textViewConfirmBusinessName))
				.setText(getArguments().getString("title"));

            ((TextView) view.findViewById(R.id.textViewSuccessStateConfirm)).setVisibility(TextView.GONE);

            if(name.equals("RedeemConfirmDialog")) {
            	// hide the social images if we're confirming redemption
            	//((ImageView) view.findViewById(R.id.imageViewPostFacebook)).setVisibility(ImageView.GONE);
            	//((ImageView) view.findViewById(R.id.imageViewPostTwitter)).setVisibility(ImageView.GONE);
            	// don't hide anymore

            	if(getArguments().containsKey("dealamount") && getArguments().containsKey("error") == false)
            	{
            		((TextView) view.findViewById(R.id.textViewSavingsAmount))
            			.setText(getArguments().getString("dealamount"));
            	}
            	else if(getArguments().containsKey("error"))
            	{
            		((TextView) view.findViewById(R.id.textViewSavingsAmount)).setVisibility(TextView.INVISIBLE);
            		((ImageView) view.findViewById(R.id.imageViewNack)).setVisibility(ImageView.VISIBLE);
            	}
            }
            else {
            	((TextView) view.findViewById(R.id.textViewSavingsRedeemed)).setVisibility(TextView.INVISIBLE);
            	((TextView) view.findViewById(R.id.textViewSavingsAmount)).setVisibility(TextView.INVISIBLE);

            	((TextView) view.findViewById(R.id.textViewDescriptionConfirm)).setVisibility(TextView.VISIBLE);
            	if(getArguments().containsKey("error")) {
            		((TextView) view.findViewById(R.id.textViewDescriptionConfirm)).setText(getArguments().getString("error"));
            	}

            	((TextView) view.findViewById(R.id.textViewHeaderConfirm))
            		.setText(res.getString(R.string.dialog_check_in));
            }

            if(getArguments().containsKey("locid")) {
            	// set up social posting images
            	final ImageView twitter = ((ImageView) view.findViewById(R.id.imageViewPostTwitter));
            	twitter.setOnClickListener(new View.OnClickListener() {

            		@Override
            		public void onClick(View v) {

            			String message = getArguments().getString("title");

            			if(getArguments().containsKey("dealamount"))
            			{
            				message += ": " + getArguments().getString("dealamount") + " Savings";
            			}

            			message += ", " + Constants.DEFAULT_DESCRIPTION_PREFIX + " ";

            			((MainActivity) getActivity()).publishTweet(message +
            					Constants.BUSINESS_DIRECTORY + String.valueOf(getArguments().getInt("locid")) + 
            					Constants.BUSINESS_DIRECTORY_NO_MAPS,
            					getArguments().getInt("busid"));
            		}
            	});

            	final ImageView facebook = ((ImageView) view.findViewById(R.id.imageViewPostFacebook));
            	facebook.setOnClickListener(new View.OnClickListener() {

            		@Override
            		public void onClick(View v) {
            			String caption = getArguments().getString("title");

            			if(getArguments().containsKey("dealamount"))
            			{
            				caption += ": " + getArguments().getString("dealamount") + " Savings";
            			}

            			String image = null;
            			if(getArguments().containsKey("busimage"))
            			{
            				image = getArguments().getString("busimage");
            			}

            			((MainActivity) getActivity()).publishFeedDialog(null,
            					caption,
            					null, 
            					Constants.BUSINESS_DIRECTORY + String.valueOf(getArguments().getInt("locid")) + Constants.BUSINESS_DIRECTORY_NO_MAPS, 
            					image,
            					Constants.DEFAULT_DESCRIPTION_PREFIX,
            					getArguments().getInt("busid"),
								null);
            		}
            	});
            }
        }
        
        return view;
    }
    
    @Override
    public void onStop() {
    	super.onStop();
    	Logger.verbose(mName, "onStop()");
    }
    
    public void setUnifiedSettings(boolean unified, boolean checkIn, boolean redeem) {
    	mUnified = unified;
    	mCheckIn = checkIn;
    	mRedeem = redeem;
    }
}