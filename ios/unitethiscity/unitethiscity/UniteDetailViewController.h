//
//  UniteDetailViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UniteDetailViewController : GAITrackedViewController <UIAlertViewDelegate>
{
    IBOutlet UIScrollView* scrollView;
    IBOutlet UIView* contentView;
    IBOutlet UIButton* tipButton;
    IBOutlet UIButton* rateButton;
    IBOutlet UIButton* favoriteButton;
    IBOutlet UIView* ratingView;
    
    IBOutlet UIButton* mapButton;
    IBOutlet UIButton* facebookButton;
    IBOutlet UIButton* phoneButton;
    IBOutlet UIButton* webButton;
    IBOutlet UIButton* moreButton;
    
    NSString* webSite;
    NSString* facebookLink;
    NSString* phoneNumber;
    NSString* mapAddress;
 }

@property (nonatomic, strong) IBOutlet UILabel* locName;
@property (nonatomic, strong) IBOutlet UILabel* locAddress;
@property (nonatomic, strong) IBOutlet UIImageView* locRating;
@property (nonatomic, strong) IBOutlet UILabel* locTags;
@property (nonatomic, strong) IBOutlet UILabel* locInfo;
@property (nonatomic, strong) IBOutlet UILabel* locCash;
@property (nonatomic, strong) IBOutlet UIImageView* locPicture;
@property (nonatomic, strong) IBOutlet UILabel* availableLabel;
@property (nonatomic, strong) IBOutlet UIButton* moreInfoCrumbButton;
@property (nonatomic, strong) IBOutlet UIView* socialRedeemedView;
@property (nonatomic, strong) IBOutlet UIWebView* tipsWebView;
@property (nonatomic, strong) IBOutlet UIButton* checkInButton;

@property (nonatomic, strong) IBOutlet UILabel* loyaltySummaryLabel;
@property (nonatomic, strong) IBOutlet UILabel* loyaltyPointsLabel;

@property (nonatomic, strong) IBOutlet UILabel* crumbLabel;

-(IBAction) clickTip:(id)sender;
-(IBAction) clickRate:(id)sender;
-(IBAction) clickFavorite:(id)sender;
-(IBAction) clickBack:(id)sender;
-(IBAction) clickRedeem:(id)sender;
-(IBAction) clickSocialTerms:(id)sender;
-(IBAction) clickLoyaltyTerms:(id)sender;
-(IBAction) clickCheckIn:(id)sender;

-(IBAction) clickMap:(id)sender;
-(IBAction) clickFacebook:(id)sender;
-(IBAction) clickPhone:(id)sender;
-(IBAction) clickWeb:(id)sender;
-(IBAction) clickMore:(id)sender;

-(void) updateFavoritesButton;

@end
