//
//  AccountViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/2/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AccountViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UIView* anonView;
@property (nonatomic, strong) IBOutlet UIView* authView;
@property (nonatomic, strong) IBOutlet UIButton* signInOutButton;
@property (nonatomic, strong) IBOutlet UIImageView* avatarImageView;
@property (nonatomic, strong) IBOutlet UILabel* userNameLabel;
@property (nonatomic, strong) IBOutlet UILabel* versionLabel;
@property (nonatomic, strong) IBOutlet UILabel* pitchLabel;
@property (nonatomic, strong) IBOutlet UIButton* businessButton;
@property (nonatomic, strong) IBOutlet UIButton* referralButton;
@property (nonatomic, strong) IBOutlet UIButton* facebookButton;
@property (nonatomic, strong) IBOutlet UIButton* readMoreButton;
@property (nonatomic, strong) IBOutlet UIButton* joinNowButton;
@property (nonatomic, strong) IBOutlet UIButton* twitterButton;
@property (nonatomic, strong) IBOutlet UIButton* instagramButtom;

-(IBAction) clickReadMore:(id)sender;
-(IBAction) clickJoinNow:(id)sender;
-(IBAction) clickSignUpEmail:(id)sender;
-(IBAction) clickSignUpFacebook:(id)sender;
-(IBAction) clickSignInOut:(id)sender;
-(IBAction) clickBusiness:(id)sender;
-(IBAction) clickReferral:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickTwitter:(id)sender;
-(IBAction) clickInstagram:(id)sender;
-(IBAction) clickViewTutorial:(id)sender;

@end
