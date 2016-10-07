//
//  UniteDetailViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "UniteDetailViewController.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "AccountInfo.h"
#import "AMRatingControl.h"
#import "RatingViewController.h"

#define kContentViewBaseHeight      565
#define kWebViewBaseHeight          100

@interface UniteDetailViewController ()

@property (strong, nonatomic) LocationContext* lc;

@end

@implementation UniteDetailViewController

@synthesize lc;
@synthesize locName;
@synthesize locAddress;
@synthesize locRating;
@synthesize locTags;
@synthesize locInfo;
@synthesize locCash;
@synthesize locPicture;
@synthesize availableLabel;
@synthesize moreInfoCrumbButton;
@synthesize socialRedeemedView;
@synthesize tipsWebView;
@synthesize checkInButton;
@synthesize loyaltySummaryLabel;
@synthesize loyaltyPointsLabel;
@synthesize crumbLabel;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self setScreenName:@"Unite Detail"];
    
    lc = nil;

    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];

    [locName setText:loc.name];
    [locAddress setText:loc.address];
    [locRating setImage:[loc ratingImage]];
    [locTags setText:[loc concatTags]];
    [locInfo setText:@""];
    [crumbLabel setText:(([loc isEntertainer] == YES) ? @"ENTERTAINER" : @"BUSINESS INFO")];
    
    // default offer to n/a until we get a deal in the context
    [locCash setText:[LocationContext formatDeal:0]];

    // init the context related data
    webSite = @"";
    facebookLink = @"";
    phoneNumber = @"";
    mapAddress = @"";
    
    // map button is always enabled - we have a location
    [mapButton setEnabled:YES];
    // enable facebook if we get a link in the context
    [facebookButton setEnabled:NO];
    // enable phone if we get a number in the context
    [phoneButton setEnabled:NO];
    // enable web link if we get a link in the context
    [webButton setEnabled:NO];
    // always enable the more information button
    [moreButton setHidden:![self hasMoreInfo]];
    [moreInfoCrumbButton setHidden:![self hasMoreInfo]];
    
    [self updateFavoritesButton];
    // load the location picture
    [locPicture setImageWithURL:[NSURL URLWithString:[loc businessImageURI]]
               placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];

    // Do any additional setup after loading the view from its nib.
    [scrollView addSubview:contentView];
    
    // configure the scrollview size
    [scrollView setContentSize:contentView.frame.size];
}

-(void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [self reloadContext];
    
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) reloadContext
{
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    // start the spinner
    [[UTCApp sharedInstance] startActivity];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] getLocationContextFor:loc.locID withBlock:^(NSDictionary* attr, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // parse the context from the response
            lc = [[LocationContext alloc] initWithAttributes:attr];
            // assign the context to the application
            [[UTCApp sharedInstance] setLocContext:lc];
            // update the fields from the context
            [locInfo setText:lc.summary];
            [locCash setText:[lc formatDeal]];
            [locRating setImage:[lc ratingImage]];

            webSite = lc.webSite;
            facebookLink = lc.facebookId;
            phoneNumber = lc.phone;
            
            // enable buttons from the context
            [webButton setEnabled:(webSite.length >0)];
            [facebookButton setEnabled:(facebookLink.length >0)];
            [phoneButton setEnabled:(phoneNumber.length >0)];
            
            // indicate if the deal is available or redeemed
            if ([lc myIsRedeemed])
            {
                [socialRedeemedView setHidden:NO];
                [availableLabel setText:@"REDEEMED"];
            }
            else
            {
                [socialRedeemedView setHidden:YES];
                [availableLabel setText:@"AVAILABLE"];
            }
            
            if ([lc myIsCheckedIn])
            {
                [checkInButton setHidden:YES];
            }
            else
            {
                [checkInButton setHidden:NO];
            }
            // Do any additional setup after loading the view from its nib.
            NSString *urlAddress = [NSString stringWithFormat:kMemberTipsURL, (int)lc.locID];
            [tipsWebView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:urlAddress]]];
            
            [loyaltySummaryLabel setText:[lc loyaltySummary]];
            int pointsNeeded = [lc pointsNeeded] - [lc pointsCollected];
            if (pointsNeeded <= 0)
            {
                [loyaltyPointsLabel setText:@"LOYALTY REWARDED"];
                [checkInButton setHidden:YES];
            }
            else
            {
                [loyaltyPointsLabel setText:[NSString stringWithFormat:@"%d POINT%@ NEEDED", pointsNeeded, (pointsNeeded != 1) ? @"S" : @""]];
            }
            [moreButton setHidden:![self hasMoreInfo]];
            [moreInfoCrumbButton setHidden:![self hasMoreInfo]];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickRedeem:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openUniteRedeem];
}

-(IBAction) clickCheckIn:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openUniteCheckInResults];
}


-(IBAction) clickTip:(id)sender
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [[[UTCApp sharedInstance] rootViewController] openGuestDenied:kGuestDeniedTip];
        return;
    }
    
    [[[UTCApp sharedInstance] rootViewController] openLeaveATip];
}

-(IBAction) clickSocialTerms:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSocialTermsAndConditions];
}

-(IBAction) clickLoyaltyTerms:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openLoyaltyTermsAndConditions];
}


-(IBAction) clickRate:(id)sender
{
    RatingViewController* dvc = [[RatingViewController alloc] initWithNibName:@"RatingViewController" bundle:nil];
    UTCRootViewController* rvc = [[UTCApp sharedInstance] rootViewController];
    [rvc presentViewController:dvc animated:NO completion:nil];
}

-(IBAction) clickFavorite:(id)sender
{
    if (![[[UTCApp sharedInstance] account] isMember])
    {
        [[[UTCApp sharedInstance] rootViewController] openGuestDenied:kGuestDeniedFavorite];
        return;
    }
    
    // start the spinner
    [[UTCApp sharedInstance] startActivity];
    
    // load the location context information for the active user
    int locID = [[UTCApp sharedInstance] selectedLocation];

    if (![[UTCApp sharedInstance] favoritesContainsLocation:locID]) {
        [[UTCAPIClient sharedClient] postFavoriteWithBlock:[[UTCApp sharedInstance] selectedLocation] withBlock:^(NSError *error) {
            if (error) {
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            } else {
                [[UTCAPIClient sharedClient] getFavoritesWithBlock:^(NSArray* attr, NSError* error) {
                    if (error) {
                        [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                    } else {
                        [[UTCApp sharedInstance] refreshMemberFavorites:attr];
                        [self updateFavoritesButton];
                    }
                }];
            }
            [[UTCApp sharedInstance] stopActivity];
        }];
    } else {
        [[UTCAPIClient sharedClient] deleteFavoriteWithBlock:[[UTCApp sharedInstance] selectedLocation] withBlock:^(NSError *error) {
            if (error) {
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            } else {
                [[UTCAPIClient sharedClient] getFavoritesWithBlock:^(NSArray* attr, NSError* error) {
                    if (error) {
                        [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                    } else {
                        [[UTCApp sharedInstance] refreshMemberFavorites:attr];
                        [self updateFavoritesButton];
                   }
                }];
            }
            [[UTCApp sharedInstance] stopActivity];
        }];
    }
}

// update the image to show the message for changing the state
-(void) updateFavoritesButton
{
    int locID = [[UTCApp sharedInstance] selectedLocation];
    BOOL fav = [[UTCApp sharedInstance] favoritesContainsLocation:locID];
    UIImage* img = [UIImage imageNamed:(fav) ? @"btnRemoveFavorite.png" : @"btnAddToFavorites.png"];
    [favoriteButton setImage:img forState:UIControlStateNormal];
}

// user clicked the map icon
-(void) clickMap:(id)sender
{
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];

    // generate a map query to the address of the destination
    NSString *mapUrl = [NSString stringWithFormat:@"http://maps.apple.com/?q=%@",
                        [loc.address stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]];
    // open the map url - this will launch the mapping app on the phone
    NSURL* url = [NSURL URLWithString:mapUrl];
    [[UIApplication sharedApplication] openURL:url];
}

// user clicked the faceboook icon
-(void) clickFacebook:(id)sender
{
    // attempt to open the profile in the native facebook application
    NSString *stringUrl = [NSString stringWithFormat:@"fb://profile/%@", facebookLink];
    NSURL *url = [NSURL URLWithString:stringUrl];
    if ( [[UIApplication sharedApplication] canOpenURL:url])
    {
        [[UIApplication sharedApplication] openURL:url];
        return;
    }
    
    // couldn't open with the native scheme, go to the website with the profile id
    stringUrl = [NSString stringWithFormat:@"http://www.facebook.com/%@", facebookLink];
    url = [NSURL URLWithString:stringUrl];
    [[UIApplication sharedApplication] openURL:url];
}

// user clicked the phone icon
-(void) clickPhone:(id)sender
{
    NSString* title = [NSString stringWithFormat:@"Call %@", locName.text];
    [[[UIAlertView alloc] initWithTitle:title message:phoneNumber delegate:self
                      cancelButtonTitle:@"Cancel" otherButtonTitles:@"OK", nil] show];
}

// catch the response to the dial alert
-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    NSString *title = [alertView buttonTitleAtIndex:buttonIndex];
    if([title isEqualToString:@"OK"])
    {
        NSURL *url = [NSURL URLWithString:[NSString stringWithFormat:@"tel://%@", phoneNumber]];
        [[UIApplication sharedApplication] openURL:url];
    }
}
// user clicked the website icon
-(void) clickWeb:(id)sender
{
    NSURL *url = [NSURL URLWithString:webSite];
    [[UIApplication sharedApplication] openURL:url];
}

// user clicked the more button
-(void) clickMore:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openMoreInfo:(LocationContext*)lc];
}

-(BOOL) hasMoreInfo
{
    return (lc != nil) ? (([lc numEvents] >0 ) || ([lc numGalleryItems] >0 ) || ([lc numMenuItems] >0)) : NO;
}

- (void)webViewDidFinishLoad:(UIWebView *)aWebView {
    NSLog(@"web view did finish load");
    CGRect frame = aWebView.frame;
    frame.size.height = 1;
    aWebView.frame = frame;
    CGSize fittingSize = [aWebView sizeThatFits:CGSizeZero];
    if (fittingSize.height < kWebViewBaseHeight) {
        fittingSize.height = kWebViewBaseHeight;
    }
    frame.size = fittingSize;
    aWebView.frame = frame;
    double fullContentHeight = kContentViewBaseHeight + frame.size.height - kWebViewBaseHeight;
    CGRect contentFrame = contentView.frame;
    contentFrame.size.height = fullContentHeight;
    contentView.frame = contentFrame;
    [scrollView setContentSize:contentView.frame.size];

}

@end
