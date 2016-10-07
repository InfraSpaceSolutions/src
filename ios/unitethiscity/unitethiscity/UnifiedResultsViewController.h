//
//  UnifiedResultsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UnifiedResultsViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UILabel* headlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* resultLabel;
@property (nonatomic, strong) IBOutlet UILabel* messageLabel;
@property (nonatomic, strong) IBOutlet UILabel* rewardLabel;

@property (nonatomic, strong) IBOutlet UILabel* actionLabel;
@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;
@property (nonatomic, strong) IBOutlet UIButton* okButton;
@property (nonatomic, strong) IBOutlet UIButton* retryButton;
@property (nonatomic, strong) IBOutlet UIActivityIndicatorView* activityView;
@property (nonatomic, strong) IBOutlet UIButton* twitterButton;
@property (nonatomic, strong) IBOutlet UIButton* facebookButton;

@property (nonatomic, strong) IBOutlet UITextField* pinNumberTextBox;

@property (nonatomic, strong) NSString* locationName;
@property (readwrite) int locationId;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickOk:(id)sender;
-(IBAction) clickRetry:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickFacebook:(id)sender;

@end
