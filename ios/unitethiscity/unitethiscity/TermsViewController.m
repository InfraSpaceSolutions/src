//
//  TermsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "TermsViewController.h"
#import "LocationINfo.h"

@interface TermsViewController ()

@end

@implementation TermsViewController

@synthesize webView = _webView;
@synthesize dealTypeLabel;
@synthesize isSocialDeal;

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
    
    [self setScreenName:@"Terms"];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locatIon info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];

    [dealTypeLabel setText:((isSocialDeal == YES) ? @"UTC SOCIAL DEAL" : @"UTC LOYALTY DEAL")];
    
    // open the terms and conditions in a web view
    NSURL* url;
    if (isSocialDeal)
    {
        url = [NSURL URLWithString:[NSString stringWithFormat:kSocialDealTermsURL, loc.locID]];
    }
    else
    {
        url = [NSURL URLWithString:[NSString stringWithFormat:kLoyaltyDealTermsURL, loc.locID]];
    }
    [_webView loadRequest:[NSURLRequest requestWithURL:url]];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end