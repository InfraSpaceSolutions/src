//
//  ProximityResultsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 7/15/14.
//  Copyright (c) 2014 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ProximityResultsViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UILabel* headlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* resultLabel;
@property (nonatomic, strong) IBOutlet UILabel* messageLabel;
@property (nonatomic, strong) IBOutlet UILabel* rewardLabel;
@property (nonatomic, strong) IBOutlet UILabel* redeemedValueLabel;


@property (nonatomic, strong) IBOutlet UILabel* actionLabel;
@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;
@property (nonatomic, strong) IBOutlet UIButton* okButton;
@property (nonatomic, strong) IBOutlet UIButton* retryButton;
@property (nonatomic, strong) IBOutlet UIButton* redeemButton;
@property (nonatomic, strong) IBOutlet UIButton* checkInButton;
@property (nonatomic, strong) IBOutlet UIActivityIndicatorView* activityView;
@property (nonatomic, strong) IBOutlet UIButton* twitterButton;
@property (nonatomic, strong) IBOutlet UIButton* facebookButton;

@property (nonatomic, strong) IBOutlet UIView* actionView;
@property (nonatomic, strong) IBOutlet UIView* enterPinView;
@property (nonatomic, strong) IBOutlet UIView* checkInView;
@property (nonatomic, strong) IBOutlet UIView* redeemView;

@property (nonatomic, strong) IBOutlet UIImageView* nackImage;

@property (nonatomic, strong) IBOutlet UITextField* pinNumberTextBox;

@property (nonatomic, strong) NSString* locationName;
@property (readwrite) int locationId;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickOk:(id)sender;
-(IBAction) clickRetry:(id)sender;
-(IBAction) clickCheckIn:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickRedeem:(id)sender;

@end
