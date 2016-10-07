//
//  UniteCheckinResultsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/2/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UniteCheckinResultsViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;
@property (nonatomic, strong) IBOutlet UIActivityIndicatorView* activityView;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;

@property (nonatomic, strong) IBOutlet UILabel* businessNameLabel;
@property (nonatomic, strong) IBOutlet UILabel* successLabel;
@property (nonatomic, strong) IBOutlet UILabel* successMessageLabel;
@property (nonatomic, strong) IBOutlet UILabel* errorMessageLabel;

@property (nonatomic, strong) IBOutlet UIView* shareView;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickInstagram:(id)sender;

@end
