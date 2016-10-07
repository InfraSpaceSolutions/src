//
//  UniteRedeemViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/18/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "UTCAPIClient.h"
#import "UTCSocial.h"
#import "UniteRedeemViewController.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import <FacebookSDK/FacebookSDK.h>

@interface UniteRedeemViewController ()

@property (readwrite) BOOL socialPosted;

-(void) socialPostCompleted;

@end

#define kSocialPostDwellTime        2

@implementation UniteRedeemViewController

@synthesize logoImageView;
@synthesize closeButton;
@synthesize businessNameLabel;
@synthesize rewardLabel;
@synthesize rewardSummaryLabel;
@synthesize shareView;

@synthesize socialPosted;
@synthesize selectedSocialChannel;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self)
    {
        // Custom initialization
        
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    
    [self setScreenName:@"Unite Redeem"];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    // get the locaiton info from the dictionary of all locations
    LocationInfo* locInfo = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    [businessNameLabel setText:[locInfo name]];
    [rewardLabel setText:[LocationContext formatDeal:[locInfo dealAmount]]];
    [rewardSummaryLabel setText:@""];
    
    // fix up the display for pre-IPhone5
    if (![UTCSettings isScreenTall] )
    {
        [logoImageView setHidden:YES];
    }
    
    [shareView setHidden:YES];
    [self reloadContext];
    
    socialPosted = NO;
}

-(void) viewWillAppear:(BOOL)animated
{
    [self performSelector:@selector(socialPostCompleted) withObject:nil afterDelay:kSocialPostDwellTime];
}

-(void)viewWillDisappear:(BOOL)animated
{
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
}


- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) doTwitter
{
    selectedSocialChannel = kDefaultSocialTwitter;
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [self openSLTwitter:self
            withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
    
}

-(void) doFacebook
{
    selectedSocialChannel = kDefaultSocialFacebook;
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [self openSLFacebook:self
             withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                 andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
}

-(void) doInstagram
{
    selectedSocialChannel = kDefaultSocialInstagram;
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [self openInstagram:self
            withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
}

-(void) clickTwitter:(id)sender
{
    [self doTwitter];
}

-(void) clickFacebook:(id)sender
{
    [self doFacebook];
}

-(void) clickInstagram:(id)sender
{
    [self doInstagram];
}



// open a modal view to post to twitter or go to twitter site
-(void) openSLTwitter:(UIViewController*)vc withMessage:(NSString *)msg andLink:(NSString *)link
{
    // if we don't have built in support, just go to the page
    if (![UTCSettings isTwitterAvailable])
    {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:kTwitterURL]];
        return;
    }
    
    // do a social framework tweet or a tweet sheet, depending on what is available
    Class sfClass = NSClassFromString(@"SLComposeViewController");
    if (sfClass != nil)
    {
        SLComposeViewController* tweetPostVC = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
        [tweetPostVC setInitialText:msg];
        // use the provided link or default link if not provided
        [tweetPostVC addURL:(link != nil) ? [NSURL URLWithString:link] : [NSURL URLWithString:@"http://www.unitethiscity.com/"]];
        
        // set up the completion handler to credit the post
        tweetPostVC.completionHandler = ^(SLComposeViewControllerResult result) {
            // give benefit of the doubt - at least they tried
            if (result == SLComposeViewControllerResultDone)
            {
                // POSTED TO TWITTER - PROCESS REDEEM!
                socialPosted = YES;
            }
        };
        [vc presentViewController:tweetPostVC animated:YES completion:nil];
    }
}

// open a modal view to post to facebook or go to facebook site
-(void) openSLFacebook:(UIViewController*)vc withMessage:(NSString *)msg andLink:(NSString *)link
{
    NSLog(@"link = %@", link);
    
    NSMutableDictionary *params = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                   msg, @"name",
                                   @"www.unitethiscity.com", @"caption",
                                   @"Supporting local through Unite This City", @"description",
                                   link, @"link",
                                   nil];
    [FBWebDialogs presentFeedDialogModallyWithSession:nil
                                           parameters:params
                                              handler:
     ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
         if (error) {
             // Error launching the dialog or publishing a story.
             NSLog(@"Error publishing story.");
         } else {
             if (result == FBWebDialogResultDialogNotCompleted) {
                 // User clicked the "x" icon
                 NSLog(@"User canceled story publishing.");
             } else {
                 // Handle the publish feed callback
                 NSLog(@"Published to Facebook");
                 // POSTED TO FACEBOOK - PROCESS REDEEM!
                 socialPosted = YES;
             }
         }
     }];
}

-(void) openInstagram:(UIViewController*)vc withMessage:(NSString*)msg andLink:(NSString*)link
{
    NSURL *instagramURL = [NSURL URLWithString:@"instagram://camera"];
    if ([[UIApplication sharedApplication] canOpenURL:instagramURL]) {
        [[UIApplication sharedApplication] openURL:instagramURL];
        socialPosted = YES;
    } else {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"No Instagram"
                                                        message:@"Instagram is not installed or access is not permitted."
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
    }
}



-(void) socialPostCompleted
{
    if (socialPosted) {
        socialPosted = NO;
        [self dismissViewControllerAnimated:NO completion:nil];
        if ([[UTCApp sharedInstance] defaultSocialChannel] <= kDefaultSocialNone) {
            [[[UTCApp sharedInstance] rootViewController] openDefaultSocialViewController:selectedSocialChannel];
        } else {
            [[[UTCApp sharedInstance] rootViewController] openUniteRedeemResults];
        }
    } else {
        [self performSelector:@selector(socialPostCompleted) withObject:nil afterDelay:kSocialPostDwellTime];
    }

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
            LocationContext* lc = [[LocationContext alloc] initWithAttributes:attr];
            // assign the context to the application
            [[UTCApp sharedInstance] setLocContext:lc];
            if (lc) {
                [rewardSummaryLabel setText:[lc dealDescription]];
            }
            [shareView setHidden:NO];
            
            // if there is a default social channel - launch it here
            switch ([[UTCApp sharedInstance] defaultSocialChannel])
            {
                case kDefaultSocialTwitter:
                    [self doTwitter];
                    break;
                case kDefaultSocialFacebook:
                    [self doFacebook];
                    break;
                case kDefaultSocialInstagram:
                    [self doInstagram];
                    break;
                default:
                    NSLog(@"Default social channel is %d; do nothing", [[UTCApp sharedInstance] defaultSocialChannel]);
                    break;
            }
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

@end
