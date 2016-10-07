//
//  BusinessRedeemResultsViewController
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "BusinessRedeemResultsViewController.h"

@interface BusinessRedeemResultsViewController ()
-(void) performAction;
@end

@implementation BusinessRedeemResultsViewController

@synthesize headlineLabel = _headlineLabel;
@synthesize actionLabel = _actionLabel;
@synthesize logoImageView = _logoImageView;
@synthesize activityView = _activityView;
@synthesize pinNumberTextBox = _pinNumberTextBox;

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
    
    [self setScreenName:@"Business Redeem Results"];

    // hide the logo to save space on 3.5 screens
    [_logoImageView setHidden:(![UTCSettings isScreenTall])];
    
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
    [_pinNumberTextBox resignFirstResponder];
    [self performAction];
}

// attempt the action with the currently available information; split out to support
// multiple attempts with retry for PIN entry
-(void) performAction
{
    [_headlineLabel setText:@"Working..."];
    [_activityView setHidesWhenStopped:YES];
    [_activityView startAnimating];
    [_pinNumberTextBox setHidden:YES];
    [_retryButton setHidden:YES];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    // build the dictionary to submit for redemption
    NSMutableDictionary* redeem = [[NSMutableDictionary alloc] init];
    
    [redeem setObject:[NSNumber numberWithInt:[[[UTCApp sharedInstance] account] accID]] forKey:@"AccId"];
    [redeem setObject:[NSNumber numberWithInt:kRoleBusiness] forKey:@"RolId"];
    [redeem setObject:[NSNumber numberWithInt:loc.locID] forKey:@"LocId"];
    [redeem setObject:[NSNumber numberWithInt:0] forKey:@"DealId"];
    [redeem setObject:[NSNumber numberWithInt:0] forKey:@"MemberAccId"];
    [redeem setObject:[[UTCApp sharedInstance] lastQurl] forKey:@"Qurl"];
    [redeem setObject:[_pinNumberTextBox text] forKey:@"PinNumber"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [redeem setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [redeem setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // NSLog(@"REDEEM SUBMISSION %@",redeem);
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postRedeem:redeem withBlock:^(NSError *error) {
        if (error) {
            [_messageLabel setText:[error localizedDescription]];
            [_resultLabel setText:@"UNSUCCESSFUL"];
            NSData* jsonData = [[error localizedRecoverySuggestion] dataUsingEncoding:NSUTF8StringEncoding];
            NSError* jsonError;
            NSDictionary* errDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&jsonError];
            if (errDict) {
                [_messageLabel setText:[errDict objectForKey:@"Message"]];
                // if we got a PIN error, let them try again
                if ([[_messageLabel text] hasPrefix:@"PIN"])
                {
                    [_headlineLabel setText:@"ENTER PIN"];
                    [_messageLabel setText:@"This offer requires approval.  Please enter your PIN to confirm the UTC redemption."];
                    [_resultLabel setText:[errDict objectForKey:@"Message"]];
                    [_pinNumberTextBox setHidden:NO];
                    [_retryButton setHidden:NO];
                }
            }
            [_rewardLabel setHidden:YES];
        } else {
            [_headlineLabel setText:@"REDEEM"];
            [_resultLabel setText:@"SUCCESSFUL"];
            [_messageLabel setText:@"Excellent! You just helped your customer redeem UTC cash."];
            [_rewardLabel setText:@""];
            [_rewardLabel setHidden:NO];
        }
        [_activityView stopAnimating];
    }];
}


@end
