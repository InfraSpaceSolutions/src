//
//  BusinessCheckinResultsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/2/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "BusinessCheckinResultsViewController.h"

@interface BusinessCheckinResultsViewController ()

@end

@implementation BusinessCheckinResultsViewController

@synthesize headlineLabel = _headlineLabel;
@synthesize actionLabel = _actionLabel;
@synthesize logoImageView = _logoImageView;
@synthesize activityView = _activityView;

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
    
    [self setScreenName:@"Business Checkin Results"];

    // hide the logo to save space on 3.5 screens
    [_logoImageView setHidden:(![UTCSettings isScreenTall])];
    
    [_activityView setHidesWhenStopped:YES];
    [_activityView startAnimating];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    // build the dictionary to submit for redemption
    NSMutableDictionary* checkin = [[NSMutableDictionary alloc] init];
    
    [checkin setObject:[NSNumber numberWithInt:[[[UTCApp sharedInstance] account] accID]] forKey:@"AccId"];
    [checkin setObject:[NSNumber numberWithInt:kRoleBusiness] forKey:@"RolId"];
    [checkin setObject:[NSNumber numberWithInt:loc.locID] forKey:@"LocId"];
    [checkin setObject:[NSNumber numberWithInt:0] forKey:@"MemberAccId"];
    [checkin setObject:[[UTCApp sharedInstance] lastQurl] forKey:@"Qurl"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [checkin setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [checkin setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postCheckIn:checkin withBlock:^(NSError *error) {
        if (error) {
            // default the error to the localized description in case we don't get something better from the response
            [_messageLabel setText:[error localizedDescription]];
            [_resultLabel setText:@"UNSUCCESSFUL"];
            NSData* jsonData = [[error localizedRecoverySuggestion] dataUsingEncoding:NSUTF8StringEncoding];
            NSError* jsonError;
            NSDictionary* errDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&jsonError];
            if (errDict) {
                [_messageLabel setText:[errDict objectForKey:@"Message"]];
            }
            [_rewardLabel setHidden:YES];
        } else {
            [_resultLabel setText:@"SUCCESSFUL"];
            [_messageLabel setText:@"Excellent! You just checked in a Unite This City member."];
            [_rewardLabel setText:@""];
            [_rewardLabel setHidden:YES];
        }
        [_activityView stopAnimating];
    }];
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

-(void) clickOk:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

@end
