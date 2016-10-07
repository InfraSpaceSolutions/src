//
//  UniteRedeemViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/18/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UniteRedeemViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;

@property (nonatomic, strong) IBOutlet UILabel* businessNameLabel;
@property (nonatomic, strong) IBOutlet UILabel* rewardLabel;
@property (nonatomic, strong) IBOutlet UILabel* rewardSummaryLabel;

@property (nonatomic, strong) IBOutlet UIView* shareView;

@property (readwrite) int selectedSocialChannel;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickInstagram:(id)sender;



@end
