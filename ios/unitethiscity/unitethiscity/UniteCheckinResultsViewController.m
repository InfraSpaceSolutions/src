//
//  UniteCheckinResultsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/2/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCSocial.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "UniteCheckinResultsViewController.h"

@interface UniteCheckinResultsViewController ()

@end

@implementation UniteCheckinResultsViewController

@synthesize activityView;
@synthesize logoImageView;
@synthesize closeButton;
@synthesize businessNameLabel;
@synthesize successLabel;
@synthesize successMessageLabel;
@synthesize errorMessageLabel;
@synthesize shareView;


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
    
    [self setScreenName:@"Unite Checkin Results"];

    // hide the logo to save space on 3.5 screens
    [logoImageView setHidden:(![UTCSettings isScreenTall])];
    
    [activityView setHidesWhenStopped:YES];
    [activityView startAnimating];
    
    // hide stuff while we are working
    [shareView setHidden:YES];
    [successLabel setHidden:YES];
    [successMessageLabel setHidden:YES];
    [errorMessageLabel setHidden:YES];
    
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    // build the dictionary to submit for redemption
    NSMutableDictionary* checkin = [[NSMutableDictionary alloc] init];
  
    [businessNameLabel setText:[lc name]];
    
    [checkin setObject:[NSNumber numberWithInt:lc.accID] forKey:@"AccId"];
    [checkin setObject:[NSNumber numberWithInt:kRoleMember] forKey:@"RolId"];
    [checkin setObject:[NSNumber numberWithInt:lc.locID] forKey:@"LocId"];
    [checkin setObject:[NSNumber numberWithInt:lc.accID] forKey:@"MemberAccId"];
    [checkin setObject:@"N/A" forKey:@"Qurl"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [checkin setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [checkin setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postCheckIn:checkin withBlock:^(NSError *error) {
        if (error) {
            // default the error to the localized description in case we don't get something better from the response
            [errorMessageLabel setHidden:NO];
            [errorMessageLabel setText:[error localizedDescription]];
            NSData* jsonData = [[error localizedRecoverySuggestion] dataUsingEncoding:NSUTF8StringEncoding];
            NSError* jsonError;
            NSDictionary* errDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&jsonError];
            if (errDict) {
                [errorMessageLabel setText:[errDict objectForKey:@"Message"]];
            }
        } else {
            [shareView setHidden:NO];
            [successLabel setHidden:NO];
            [successMessageLabel setHidden:NO];
            [errorMessageLabel setHidden:YES];
        }
        [activityView stopAnimating];
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

-(void) clickTwitter:(id)sender
{
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [[UTCSocial sharedInstance] openSLTwitter:self
                                  withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                                      andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
}

-(void) clickFacebook:(id)sender
{
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [[UTCSocial sharedInstance] openSLFacebook:self
                                   withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                                       andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
}

-(void) clickInstagram:(id)sender
{
    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    
    [[UTCSocial sharedInstance] openInstagram:self
            withMessage:[[UTCSocial sharedInstance] messageForLocation:lc]
                andLink:[[UTCSocial sharedInstance] linkForLocation:lc]];
}


@end
