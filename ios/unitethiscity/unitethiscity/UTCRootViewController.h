//
//  UTCRootViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import <UIKit/UIKit.h>

@class ScanditSDKBarcodePicker;
@class LocationContext;

@interface UTCRootViewController : UIViewController <ScanditSDKOverlayControllerDelegate>
{
    UILabel* titleLabel;
    
    UIButton* walletButton;
    UIButton* favoriteButton;
    UIButton* uniteButton;
    UIButton* inboxButton;
    UIButton* searchButton;
    UIButton* accountButton;
    UIActivityIndicatorView* activitySpinner;
    
    UIViewController* activeViewController;
    UIViewController* modalViewController;
    
    ScanditSDKBarcodePicker* picker;
}

@property (nonatomic,strong) IBOutlet UILabel* titleLabel;
@property (nonatomic,strong) IBOutlet UIButton* walletButton;
@property (nonatomic,strong) IBOutlet UIButton* favoriteButton;
@property (nonatomic,strong) IBOutlet UIButton* uniteButton;
@property (nonatomic,strong) IBOutlet UIButton* inboxButton;
@property (nonatomic,strong) IBOutlet UIButton* searchButton;
@property (nonatomic,strong) IBOutlet UIButton* accountButton;
@property (nonatomic,strong) IBOutlet UIImageView* accountPicture;
@property (nonatomic,strong) IBOutlet UIActivityIndicatorView* activitySpinner;
@property (readwrite) BOOL firstDisplay;

-(IBAction)clickWallet:(id)sender;
-(IBAction)clickFavorite:(id)sender;
-(IBAction)clickUnite:(id)sender;
-(IBAction)clickInbox:(id)sender;
-(IBAction)clickSearch:(id)sender;
-(IBAction)clickHome:(id)sender;
-(IBAction)clickAccount:(id)sender;

-(void) popActiveView;
-(void) popActiveView:(BOOL)anim;

-(void) openAccount;
-(void) openFavorite;
-(void) openFavoriteDetail;
-(void) openInbox;
-(void) openInboxDetail;
-(void) openSearch;
-(void) openSplash;
-(void) openUnite;
-(void) openUniteDetail;
-(void) openUniteRedeem;
-(void) openUniteRedeemResults;
-(void) openUniteCheckInResults;
-(void) openWallet;
-(void) openGuestDenied:(int)deniedAction;
-(void) openLeaveATip;
-(void) openSocialTermsAndConditions;
-(void) openLoyaltyTermsAndConditions;
-(void) openBusinesses;
-(void) openBusinessRedeem;
-(void) openBusinessRedeemResults;
-(void) openBusinessCheckInResults;
-(void) openUnifiedAction;
-(void) openUnifiedResults;
-(void) openSearchMenu;
-(void) openSearchBusinesses;
-(void) openSearchEntertainers;
-(void) openSearchEvents;
-(void) openSearchEventsWithType:(NSString*)eventType;
-(void) openSearchCategories:(BOOL)popOnApply;
-(void) openSearchCategories:(BOOL)popOnApply withType:(NSString*)eventType;
-(void) openEventDetail;
-(void) openSearchBusinessText:(BOOL)popOnGo;
-(void) openSearchEntertainersText:(BOOL)popOnGo;
-(void) openSearchEventText:(BOOL)popOnGo withType:(NSString*)eventType;
-(void) openSubscribe;
-(void) openSubscribeWithPreloads:(NSDictionary*)dic;
-(void) openProximityList;
-(void) openProximityResults;
-(void) openMoreInfo:(LocationContext*)lc;
-(void) openSignUpSplashViewController;
-(void) openTutorialViewController;

-(void) openStatSummaryViewController;
-(void) openStatRedemptionListViewController;
-(void) openStatCheckInListViewController;
-(void) openStatFavoriteListViewController;
-(void) openStatRatingListViewController;
-(void) openStatTipListViewController;
-(void) openBusinessTipViewController;
-(void) openGalleryImageViewController:(int)index withArray:(NSArray*)items;
-(void) openAnalyticsViewController:(int)bus;
-(void) openSummaryRedemptionListViewController;
-(void) openSummaryCheckInListViewController;

-(void) openDefaultSocialViewController:(int)defaultSocial;

-(void) startActivityDisplay;
-(void) stopActivityDisplay;

-(void) loadAccountImage;
-(NSString*) accountImageURLWithDimensions:(int)size;

-(void) confirmLocationServices;

@end
