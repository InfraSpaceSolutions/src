//
//  UniteRedeemResultsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UniteRedeemResultsViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;

@property (nonatomic, strong) IBOutlet UILabel* headlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* successHeadlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* errorHeadlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* businessNameLabel;
@property (nonatomic, strong) IBOutlet UIActivityIndicatorView* activityView;

@property (nonatomic, strong) IBOutlet UILabel* rewardLabel;
@property (nonatomic, strong) IBOutlet UILabel* rewardSummaryLabel;

@property (nonatomic, strong) IBOutlet UILabel* errorMesssageLabel;
@property (nonatomic, strong) IBOutlet UITextField* pinNumberTextBox;

@property (nonatomic, strong) IBOutlet UIView* shareView;
@property (nonatomic, strong) IBOutlet UIView* errorView;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickRetry:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickInstagram:(id)sender;

@end
