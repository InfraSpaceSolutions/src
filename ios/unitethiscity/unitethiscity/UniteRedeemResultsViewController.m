//
//  UniteRedeemResultsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCSocial.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "UniteRedeemResultsViewController.h"

@interface UniteRedeemResultsViewController ()
-(void) performAction;
@end

@implementation UniteRedeemResultsViewController

@synthesize logoImageView;
@synthesize headlineLabel;
@synthesize successHeadlineLabel;
@synthesize errorHeadlineLabel;
@synthesize businessNameLabel;
@synthesize activityView;
@synthesize rewardLabel;
@synthesize rewardSummaryLabel;
@synthesize errorMesssageLabel;
@synthesize pinNumberTextBox;
@synthesize shareView;
@synthesize errorView;

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

    [self setScreenName:@"Unite Redeem Results"];

    // hide the logo to save space on 3.5 screens
    [logoImageView setHidden:(![UTCSettings isScreenTall])];
    
    [self performAction];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(BOOL) textFieldShouldReturn:(UITextField *)textField{
    
    [textField resignFirstResponder];
    return YES;
}

-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickOk:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickRetry:(id)sender
{
    [pinNumberTextBox resignFirstResponder];
    [self performAction];
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


// attempt the action with the currently available information; split out to support
// multiple attempts with retry for PIN entry
-(void) performAction
{
    [headlineLabel setHidden:NO];
    [successHeadlineLabel setHidden:YES];
    [errorHeadlineLabel setHidden:YES];
    [shareView setHidden:YES];
    [errorView setHidden:YES];
    [pinNumberTextBox setHidden:YES];
    
    [rewardLabel setHidden:YES];
    [rewardSummaryLabel setHidden:YES];
    
    [activityView setHidesWhenStopped:YES];
    [activityView startAnimating];

    // get the context information to define the redemption
    LocationContext* lc = [[UTCApp sharedInstance] locContext];

    [businessNameLabel setText:[lc name]];
    [rewardLabel setText:[lc formatDeal]];
    [rewardSummaryLabel setText:@""];
    
    // build the dictionary to submit for redemption
    NSMutableDictionary* redeem = [[NSMutableDictionary alloc] init];
    
    [redeem setObject:[NSNumber numberWithInt:lc.accID] forKey:@"AccId"];
    [redeem setObject:[NSNumber numberWithInt:kRoleMember] forKey:@"RolId"];
    [redeem setObject:[NSNumber numberWithInt:lc.locID] forKey:@"LocId"];
    [redeem setObject:[NSNumber numberWithInt:lc.dealID] forKey:@"DealId"];
    [redeem setObject:[NSNumber numberWithInt:lc.accID] forKey:@"MemberAccId"];
    [redeem setObject:@"N/A" forKey:@"Qurl"];
    [redeem setObject:[pinNumberTextBox text] forKey:@"PinNumber"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [redeem setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [redeem setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // NSLog(@"REDEEM SUBMISSION %@",redeem);
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postRedeem:redeem withBlock:^(NSError *error) {
        if (error) {
            [errorHeadlineLabel setHidden:NO];
            [errorView setHidden:NO];
            [errorMesssageLabel setText:[error localizedDescription]];
            NSData* jsonData = [[error localizedRecoverySuggestion] dataUsingEncoding:NSUTF8StringEncoding];
            NSError* jsonError;
            NSDictionary* errDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&jsonError];
            if (errDict) {
                [errorMesssageLabel setText:[errDict objectForKey:@"Message"]];
                // if we got a PIN error, let them try again
                if ([[errorMesssageLabel text] hasPrefix:@"PIN"])
                {
                    [errorMesssageLabel setText:@"This offer requires approval.  Please have a staff member confirm your UTC redemption."];
                    [pinNumberTextBox setHidden:NO];
                }
            }
        } else {
            NSLog(@"Redeem in %@", redeem);
            [successHeadlineLabel setHidden:NO];
            [shareView setHidden:NO];
            [rewardLabel setHidden:NO];
            [rewardSummaryLabel setHidden:NO];
        }
        [activityView stopAnimating];
    }];
}


@end
