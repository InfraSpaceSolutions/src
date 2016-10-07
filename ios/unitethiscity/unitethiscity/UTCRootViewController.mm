//
//  UTCRootViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCAPIClient.h"
#import "UTCSettings.h"
#import "Gravatar.h"
#import "AccountInfo.h"

#import "UTCRootViewController.h"
#import "SplashViewController.h"
#import "AccountViewController.h"
#import "WalletViewController.h"
#import "FavoriteViewController.h"
#import "SearchViewController.h"

#import "FavoriteListViewController.h"
#import "FavoriteDetailViewController.h"

#import "InboxListViewController.h"
#import "InboxDetailViewController.h"

#import "UniteListViewController.h"
#import "UniteDetailViewController.h"
#import "UniteRedeemViewController.h"
#import "UniteRedeemResultsViewController.h"
#import "UniteCheckinResultsViewController.h"
#import "MoreViewController.h"

#import "SearchMenuViewController.h"
#import "SearchBusinessListViewController.h"
#import "SearchEventListViewController.h"
#import "SearchCategoryListViewController.h"
#import "SearchEntertainerListViewController.h"

#import "SearchBusinessTextViewController.h"
#import "SearchEventTextViewController.h"

#import "EventDetailViewController.h"

#import "GuestDeniedViewController.h"
#import "TipViewController.h"
#import "TermsViewController.h"

#import "UnifiedViewController.h"
#import "UnifiedResultsViewController.h"

#import "ProximityViewController.h"
#import "ProximityResultsViewController.h"

#import "BusinessListViewController.h"
#import "BusinessRedeemViewController.h"
#import "BusinessRedeemResultsViewController.h"
#import "BusinessCheckinResultsViewController.h"

#import "SubscribeViewController.h"

#import "ScanditSDKBarcodePicker.h"

#import "SignUpSplashViewController.h"
#import "TutorialViewController.h"
#import "StatSummaryViewController.h"
#import "StatRedemptionListViewController.h"
#import "StatCheckInListViewController.h"
#import "StatFavoriteListViewController.h"
#import "StatRatingListViewController.h"
#import "StatTipListViewController.h"
#import "BusinessTipViewController.h"
#import "GalleryImageViewController.h"
#import "AnalyticsViewController.h"
#import "SummaryRedemptionListViewController.h"
#import "SummaryCheckInListViewController.h"

#import "DefaultSocialViewController.h"

@interface UTCRootViewController ()
-(void) markSelected:(UIButton*)selected;
-(void) switchViewController:(UIViewController*)vc;
@end

@implementation UTCRootViewController

@synthesize titleLabel;
@synthesize walletButton, favoriteButton, uniteButton, inboxButton, searchButton, accountButton;
@synthesize activitySpinner;
@synthesize accountPicture;
@synthesize firstDisplay;

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
    [titleLabel setText:@""];
    [activitySpinner setHidden:YES];
    [activitySpinner stopAnimating];
    
    // start with the splash screen if logged in or the account screen if not logged in
    UIViewController* vc = nil;
    vc = [[SplashViewController alloc] initWithNibName:@"SplashViewController" bundle:nil];
    [[self view] insertSubview:vc.view atIndex:1];
    activeViewController = vc;

    // load the user avatar
    [self loadAccountImage];
    
    // check the api version
    [[UTCAPIClient sharedClient] getVersionWithBlock:^(NSDictionary* attr, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // parse the version out of the response
            int major =[[attr valueForKeyPath:@"Major"] intValue];
            int minor =[[attr valueForKeyPath:@"Minor"] intValue];
            int patch =[[attr valueForKeyPath:@"Patch"] intValue];
            NSLog(@"Connected to API version %d.%d.%d - designed for %d.%d.%d", major, minor, patch, kAPIVersionMajor, kAPIVersionMinor, kAPIVersionPatch);
            // if major versions don't match, tell the user to update their app
            if (major != kAPIVersionMajor) {
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Not Compatible", nil) message:@"API not supported. Please update your App"
                                           delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            }
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
    
}

-(void) viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    if (firstDisplay)
    {
        firstDisplay = NO;
        AccountInfo* ai = [[UTCApp sharedInstance] account];
        if (![ai isSignedIn])
        {
            [self openSignUpSplashViewController];
        }
        else if ([[UTCApp sharedInstance] shouldShowTutorial])
        {
            [self openTutorialViewController];
        }
    }
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


// update the button bar to indicated the selected "tab"
-(void) markSelected:(UIButton*)selected
{
    [walletButton setSelected:(selected == walletButton)];
    [favoriteButton setSelected:(selected == favoriteButton)];
    [uniteButton setSelected:(selected == uniteButton)];
    [inboxButton setSelected:(selected == inboxButton)];
    [searchButton setSelected:(selected == searchButton)];
}

-(IBAction)clickWallet:(id)sender
{
    [self openWallet];
}

-(IBAction)clickFavorite:(id)sender
{
    [self openFavorite];
}

-(IBAction)clickUnite:(id)sender
{
    [self openUnite];
}

-(IBAction)clickInbox:(id)sender
{
    [self openInbox];
}

-(IBAction)clickSearch:(id)sender
{
    [self openSearchMenu];
}

-(IBAction)clickHome:(id)sender
{
    [self openSplash];
}

-(IBAction)clickAccount:(id)sender
{
    [self openAccount];
    //[[UTCApp sharedInstance] toggleTestUser];
}

#pragma mark - Activity display

-(void) startActivityDisplay
{
    [activitySpinner setHidden:NO];
    [activitySpinner startAnimating];
}

-(void) stopActivityDisplay
{
    [activitySpinner setHidden:YES];
    [activitySpinner stopAnimating];
}

// load the authenticated user's avatar image and tie to the button
-(void) loadAccountImage
{
    int avatarDim = ([[UTCApp sharedInstance] isRetina]) ? kAvatarSize * 2 : kAvatarSize;
    NSString* imageUrl = [self accountImageURLWithDimensions:avatarDim];
    [accountPicture setImageWithURL:[NSURL URLWithString:imageUrl] placeholderImage:[UIImage imageNamed:@"mysteryMan.png"]];
}

// get the applicable url for the image for the current user
-(NSString*) accountImageURLWithDimensions:(int)size
{
    NSString* email =[[[UTCApp sharedInstance] account] email];
    NSString* gravatarURI = [Gravatar imageUrlForEmail:email withSize:size withSSL:NO];
    
    // attempt to get an image from facebook
    UTCAppDelegate* appDelegate = [[UTCApp sharedInstance] appDelegate];
    if (appDelegate) {
        if ([[appDelegate facebookId] length] > 0 ) {
            gravatarURI = [NSString stringWithFormat:@"https://graph.facebook.com/%@/picture?width=%d", [appDelegate facebookId], size];
        }
    }
    return gravatarURI;
}

#pragma mark - View Controller Navigation -

// the rootviewcontroller provides a set of tabbed navigation controllers; tabs are selected using buttons on the
// rootviewcontroller nib which switch the activeViewController to the selected display; the switch views are
// generally navigation controllers which provide independent view stacks providing access to the screens that
// comprise the application

// switch the active view controller - only switch if the specified view is not already active
-(void) switchViewController:(UIViewController*)vc
{
    // don't do anything unless we want to switch views
    if (activeViewController != vc)
    {
        // remove the currently active view
        if ( activeViewController.view.superview != nil)
        {
            [activeViewController.view removeFromSuperview];
        }
        // insert the new view in place
        [[self view] insertSubview:vc.view atIndex:1];
        // track the newly active view
        activeViewController = vc;
    }
}

// pop the current view controller off of the active navigation controller
-(void) popActiveView
{
    [self popActiveView:YES];
}

// pop the current view controller off of the active navigation
-(void) popActiveView:(BOOL)anim
{
    UINavigationController* activeNavController = (UINavigationController*)activeViewController;
    [activeNavController popViewControllerAnimated:anim];
}

// open the account nav controller
-(void) openAccount
{
    [self markSelected:nil];
    [self switchViewController:[[AccountViewController alloc] initWithNibName:@"AccountViewController" bundle:nil]];
}

// open the splash nav controller
-(void) openSplash
{
    [self markSelected:nil];
    [self switchViewController:[[SplashViewController alloc] initWithNibName:@"SplashViewController" bundle:nil]];
}

// open the wallet view controller
-(void) openWallet
{
    [self markSelected:walletButton];
    [self switchViewController:[[WalletViewController alloc] initWithNibName:@"WalletViewController" bundle:nil]];
}

// open the tab's nav controller
-(void) openFavorite
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedFavorite];
        return;
    }

    // indicate the active tab
    [self markSelected:favoriteButton];

    // create the root view controller for the navigation
    FavoriteListViewController* lvc = [[FavoriteListViewController alloc] initWithNibName:@"FavoriteListViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc = [[UINavigationController alloc] initWithRootViewController:lvc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];

    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
}

// open the view on the nav controller
-(void) openFavoriteDetail
{
    UniteDetailViewController* dvc = [[UniteDetailViewController alloc] initWithNibName:@"UniteDetailViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the tab's nav controller
-(void) openUnite
{
    // indicate the active tab
    [self markSelected:uniteButton];
    
    // create the root view controller for the navigation
    UniteListViewController* lvc = [[UniteListViewController alloc] initWithNibName:@"UniteListViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc = [[UINavigationController alloc] initWithRootViewController:lvc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];

    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
}

// open the view on the nav controller
-(void) openUniteDetail
{
    UniteDetailViewController* dvc = [[UniteDetailViewController alloc] initWithNibName:@"UniteDetailViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the more info view controller
-(void) openMoreInfo:(LocationContext*)lc
{
    MoreViewController* vc = [[MoreViewController alloc] initWithNibName:@"MoreViewController" bundle:nil];
    [vc setLocationContext:lc];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}

// open the view on the nav controller
-(void) openLeaveATip
{
    TipViewController* dvc = [[TipViewController alloc] initWithNibName:@"TipViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the redeem screen
-(void) openUniteRedeem
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedRedeem];
        return;
    }
    
    UniteRedeemViewController* dvc = [[UniteRedeemViewController alloc] initWithNibName:@"UniteRedeemViewController" bundle:nil];
    [self presentViewController:dvc animated:YES completion:nil];
}

-(void) openSignUpSplashViewController
{
    SignUpSplashViewController* dvc = [[SignUpSplashViewController alloc] initWithNibName:@"SignUpSplashViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
    
}

-(void) openTutorialViewController
{
    TutorialViewController* dvc = [[TutorialViewController alloc] initWithNibName:@"TutorialViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
    
}

// open the redeem results screen
-(void) openUniteRedeemResults
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedRedeem];
        return;
    }
    
    UniteRedeemResultsViewController* dvc = [[UniteRedeemResultsViewController alloc] initWithNibName:@"UniteRedeemResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
}

// open the checkin results screen
-(void) openUniteCheckInResults
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedCheckIn];
        return;
    }
    
    UniteCheckinResultsViewController* dvc = [[UniteCheckinResultsViewController alloc] initWithNibName:@"UniteCheckinResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
}


// open the tab's nav controller
-(void) openInbox
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedInbox];
        return;
    }
    
    // indicate the active tab
    [self markSelected:inboxButton];

    // create the root view controller for the navigation
    InboxListViewController* lvc = [[InboxListViewController alloc] initWithNibName:@"InboxListViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc= [[UINavigationController alloc] initWithRootViewController:lvc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];

    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
}

// open the view on the nav controller
-(void) openInboxDetail
{
    InboxDetailViewController* dvc = [[InboxDetailViewController alloc] initWithNibName:@"InboxDetailViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the tab's nav controller
-(void) openSearchMenu
{
    // indicate the active tab
    [self markSelected:searchButton];
    
    // we are entering search - initialize the functions
    // clear the anti-filters
    [[[UTCApp sharedInstance] accountAntiFilters] removeAllObjects];
    // mark the search as unrefined so we initialize to no selections
    [[UTCApp sharedInstance] setSearchRefined:NO];
    
    // create the root view controller for the navigation
    SearchMenuViewController* vc = [[SearchMenuViewController alloc] initWithNibName:@"SearchMenuViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc= [[UINavigationController alloc] initWithRootViewController:vc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];
    
    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
}

// open the list of matching businesses
-(void) openSearchBusinesses
{
    SearchBusinessListViewController* vc = [[SearchBusinessListViewController alloc] initWithNibName:@"SearchBusinessListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}

// open the list of matching entertainers
-(void) openSearchEntertainers
{
    SearchEntertainerListViewController* vc = [[SearchEntertainerListViewController alloc] initWithNibName:@"SearchEntertainerListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}


// open the list of matching events
-(void) openSearchEvents
{
    [self openSearchEventsWithType:@""];
}

// open the list of matching events
-(void) openSearchEventsWithType:(NSString*)eventType
{
    SearchEventListViewController* vc = [[SearchEventListViewController alloc] initWithNibName:@"SearchEventListViewController" bundle:nil];
    [vc setEventType:eventType];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}

// open the list of categories for filtering
-(void) openSearchCategories:(BOOL)popOnApply
{
    [self openSearchCategories:popOnApply withType:nil];
}

-(void) openSearchCategories:(BOOL)popOnApply withType:(NSString*)eventType
{
    SearchCategoryListViewController* vc = [[SearchCategoryListViewController alloc] initWithNibName:@"SearchCategoryListViewController" bundle:nil];
    [vc setPopOnApply:popOnApply];
    [vc setEventType:eventType];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
    
}

-(void) openSearchBusinessText:(BOOL)popOnGo
{
    SearchBusinessTextViewController* vc = [[SearchBusinessTextViewController alloc] initWithNibName:@"SearchBusinessTextViewController" bundle:nil];
    [vc setPopOnGo:popOnGo];
    [vc setIsEntertainer:NO];
    if (!popOnGo ) {
        [[UTCApp sharedInstance] setSearchText:@""];
    }
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}

-(void) openSearchEntertainersText:(BOOL)popOnGo
{
    SearchBusinessTextViewController* vc = [[SearchBusinessTextViewController alloc] initWithNibName:@"SearchBusinessTextViewController" bundle:nil];
    [vc setPopOnGo:popOnGo];
    [vc setIsEntertainer:YES];
    if (!popOnGo ) {
        [[UTCApp sharedInstance] setSearchText:@""];
    }
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}


-(void) openSearchEventText:(BOOL)popOnGo withType:(NSString*)eventType
{
    SearchEventTextViewController* vc = [[SearchEventTextViewController alloc] initWithNibName:@"SearchEventTextViewController" bundle:nil];
    [vc setPopOnGo:popOnGo];
    [vc setEventType:eventType];
    if (!popOnGo ) {
        [[UTCApp sharedInstance] setSearchText:@""];
        [[UTCApp sharedInstance] setSearchDate:[NSDate date]];
    }
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:vc animated:YES];
}

// open the view on the nav controller
-(void) openEventDetail
{
    EventDetailViewController* dvc = [[EventDetailViewController alloc] initWithNibName:@"EventDetailViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the search view controller
-(void) openSearch
{
    [self markSelected:searchButton];
    [self switchViewController:[[SearchViewController alloc] initWithNibName:@"SearchViewController" bundle:nil]];
}


// open the modal access denied dialog with the specified action
-(void) openGuestDenied:(int)deniedAction
{
    GuestDeniedViewController* dvc = [[GuestDeniedViewController alloc] initWithNibName:@"GuestDeniedViewController" bundle:nil];
    [dvc setDeniedAction:deniedAction];
    [self presentViewController:dvc animated:YES completion:nil];
}

// open the webview of the terms and conditions for the current business
-(void) openSocialTermsAndConditions
{
    TermsViewController* dvc = [[TermsViewController alloc] initWithNibName:@"TermsViewController" bundle:nil];
    [dvc setIsSocialDeal:YES];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

// open the webview of the terms and conditions for the current business
-(void) openLoyaltyTermsAndConditions
{
    TermsViewController* dvc = [[TermsViewController alloc] initWithNibName:@"TermsViewController" bundle:nil];
    [dvc setIsSocialDeal:NO];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}


-(void) openUnifiedAction
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedRedeem];
        return;
    }

    UnifiedViewController* dvc = [[UnifiedViewController alloc] initWithNibName:@"UnifiedViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];

}

-(void) openUnifiedResults
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedRedeem];
        return;
    }
    
    UnifiedResultsViewController* dvc = [[UnifiedResultsViewController alloc] initWithNibName:@"UnifiedResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
}

-(void) openProximityList
{
    // create the root view controller for the navigation
    ProximityViewController* lvc = [[ProximityViewController alloc] initWithNibName:@"ProximityViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc = [[UINavigationController alloc] initWithRootViewController:lvc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];
    
    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
//    ProximityViewController* dvc = [[ProximityViewController alloc] initWithNibName:@"ProximityViewController" bundle:nil];
//    [self presentViewController:dvc animated:NO completion:nil];
}

-(void) openProximityResults
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [self openGuestDenied:kGuestDeniedRedeem];
        return;
    }
    
    ProximityResultsViewController* dvc = [[ProximityResultsViewController alloc] initWithNibName:@"ProximityResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
}

// open the business interface
-(void) openBusinesses
{
    // indicate the active tab
    [self markSelected:nil];
    
    // create the root view controller for the navigation
    BusinessListViewController* lvc = [[BusinessListViewController alloc] initWithNibName:@"BusinessListViewController" bundle:nil];
    // create a nav controller for the app section with the initial view for the section
    UINavigationController* nc = [[UINavigationController alloc] initWithRootViewController:lvc];
    // hide the navigation bar - we don't use it
    [nc setNavigationBarHidden:YES animated:NO];
    
    // switch the active view to this section's navigation controller
    [self switchViewController:nc];
}
-(void) openGalleryImageViewController:(int)index withArray:(NSArray*)items
{
    GalleryImageViewController* dvc = [[GalleryImageViewController alloc] initWithNibName:@"GalleryImageViewController" bundle:nil];
    [dvc setGalleryIndex:index];
    [dvc setGalleryItems:items];
    [self presentViewController:dvc animated:YES completion:nil];
}

// open the business action interface
-(void) openBusinessRedeem
{
    BusinessRedeemViewController* dvc = [[BusinessRedeemViewController alloc] initWithNibName:@"BusinessRedeemViewController" bundle:nil];
    [self presentViewController:dvc animated:YES completion:nil];
}

// open the business action interface
-(void) openBusinessRedeemResults
{
    BusinessRedeemResultsViewController* dvc = [[BusinessRedeemResultsViewController alloc] initWithNibName:@"BusinessRedeemResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:YES completion:nil];
}


// open the business action interface
-(void) openBusinessCheckInResults
{
    BusinessCheckinResultsViewController* dvc = [[BusinessCheckinResultsViewController alloc] initWithNibName:@"BusinessCheckinResultsViewController" bundle:nil];
    [self presentViewController:dvc animated:YES completion:nil];
}

// run the subscribe interface
-(void) openSubscribe
{
    [self markSelected:nil];
    [self switchViewController:[[SubscribeViewController alloc] initWithNibName:@"SubscribeViewController" bundle:nil]];
}

-(void) openSubscribeWithPreloads:(NSDictionary *)dic
{
    [self markSelected:nil];
    SubscribeViewController* vc = [[SubscribeViewController alloc] initWithNibName:@"SubscribeViewController" bundle:nil];
    
    // fill out the fields from the dictionary data provided
    [vc setPreloads:dic];

    [self switchViewController:vc];
}

-(void) openStatSummaryViewController
{
    StatSummaryViewController* dvc = [[StatSummaryViewController alloc] initWithNibName:@"StatSummaryViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openStatRedemptionListViewController
{
    StatRedemptionListViewController* dvc = [[StatRedemptionListViewController alloc] initWithNibName:@"StatRedemptionListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openStatCheckInListViewController
{
    StatCheckInListViewController* dvc = [[StatCheckInListViewController alloc] initWithNibName:@"StatCheckInListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openStatFavoriteListViewController
{
    StatFavoriteListViewController* dvc = [[StatFavoriteListViewController alloc] initWithNibName:@"StatFavoriteListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openStatRatingListViewController
{
    StatRatingListViewController* dvc = [[StatRatingListViewController alloc] initWithNibName:@"StatRatingListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openStatTipListViewController
{
    StatTipListViewController* dvc = [[StatTipListViewController alloc] initWithNibName:@"StatTipListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openBusinessTipViewController
{
    BusinessTipViewController* dvc = [[BusinessTipViewController alloc] initWithNibName:@"BusinessTipViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openAnalyticsViewController:(int)bus
{
    AnalyticsViewController* dvc = [[AnalyticsViewController alloc] initWithNibName:@"AnalyticsViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [dvc setBusID:bus];
    [nc pushViewController:dvc animated:YES];
}

-(void) openSummaryRedemptionListViewController
{
    SummaryRedemptionListViewController* dvc = [[SummaryRedemptionListViewController alloc] initWithNibName:@"SummaryRedemptionListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openSummaryCheckInListViewController
{
    SummaryCheckInListViewController* dvc = [[SummaryCheckInListViewController alloc] initWithNibName:@"SummaryCheckInListViewController" bundle:nil];
    UINavigationController* nc = (UINavigationController*)activeViewController;
    [nc pushViewController:dvc animated:YES];
}

-(void) openDefaultSocialViewController:(int)defaultSocial
{
    DefaultSocialViewController* dvc = [[DefaultSocialViewController alloc] initWithNibName:@"DefaultSocialViewController" bundle:nil];
    [dvc setDefaultSocial:defaultSocial];
    [self presentViewController:dvc animated:YES completion:nil];
}


//////
// ScanditSDKOverlayControllerDelegate
//////

// scan completed
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didScanBarcode:(NSDictionary *)barcodeResult
{
	[picker stopScanning];
    [picker dismissViewControllerAnimated:NO completion:nil];
    
	if (barcodeResult == nil)
    {
        NSLog(@"barcodeResult is nil");
        return;
    }
    
    [self dismissViewControllerAnimated:NO completion:nil];
    
    NSLog(@"barcodeResult is %@",[barcodeResult objectForKey:@"barcode"]);
    
    // store the captured bar code
    [[UTCApp sharedInstance] setLastQurl:[barcodeResult objectForKey:@"barcode"]];
    [self openUnifiedResults];
}

// user canceled the scan
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didCancelWithStatus:(NSDictionary *)status
{
    [picker dismissViewControllerAnimated:NO completion:nil];
    [self dismissViewControllerAnimated:NO completion:nil];
}

// user entered a bar code manually
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didManualSearch:(NSString *)input
{
}

// confirm that location services are enabled or prompt to turn them on
-(void) confirmLocationServices
{
    CLAuthorizationStatus status = [CLLocationManager authorizationStatus];
    if ((status != kCLAuthorizationStatusAuthorized) && (status != kCLAuthorizationStatusAuthorizedAlways) && (status != kCLAuthorizationStatusAuthorizedWhenInUse)) {
        NSString* title;
        title = (status == kCLAuthorizationStatusDenied) ? @"Location services are off" : @"Location is not enabled";
        NSString *message = @"Set location to 'Always' or 'While Using the App' in Location Services Settings to sort results by distance";
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:title
                                                            message:message
                                                           delegate:self
                                                  cancelButtonTitle:@"Cancel"
                                                  otherButtonTitles:@"Settings", nil];
        [alertView show];
    }
}

// handle the request to view the settings from the alert box
- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == 1) {
        // Send the user to the Settings for this app
        NSURL *settingsURL = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
        [[UIApplication sharedApplication] openURL:settingsURL];
    }
}


@end
