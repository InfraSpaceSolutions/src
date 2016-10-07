//
//  UnifiedResultsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "UnifiedResultsViewController.h"
#import "UTCSocial.h"

@interface UnifiedResultsViewController ()
-(void) performAction;
@end

@implementation UnifiedResultsViewController

@synthesize headlineLabel = _headlineLabel;
@synthesize actionLabel = _actionLabel;
@synthesize logoImageView = _logoImageView;
@synthesize activityView = _activityView;
@synthesize pinNumberTextBox = _pinNumberTextBox;
@synthesize twitterButton = _twitterButton;
@synthesize facebookButton = _facebookButton;
@synthesize locationId = _locationId;
@synthesize locationName = _locationName;

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
    
    // init the properties
    _locationName = @"";
    _locationId = 0;
    
    [self setScreenName:@"Unified Results"];
    
    // hide the logo to save space on 3.5 screens
    [_logoImageView setHidden:(![UTCSettings isScreenTall])];
    
    // kickstart the action process
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

-(void) clickTwitter:(id)sender
{
    [[UTCSocial sharedInstance] openSLTwitter:self
                                   withMessage:[[UTCSocial sharedInstance] messageForName:_locationName]
                                       andLink:[[UTCSocial sharedInstance] linkForLocID:_locationId]];
}

-(void) clickFacebook:(id)sender
{
    [[UTCSocial sharedInstance] openSLFacebook:self
                                   withMessage:[[UTCSocial sharedInstance] messageForName:_locationName]
                                       andLink:[[UTCSocial sharedInstance] linkForLocID:_locationId]];
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
    
    // get the context information to define the redemption
    AccountInfo* acc = [[UTCApp sharedInstance] account];
    
    // build the dictionary to submit for unified action
    NSMutableDictionary* uac = [[NSMutableDictionary alloc] init];
    
    [uac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"AccId"];
    [uac setObject:[NSNumber numberWithInt:kRoleMember] forKey:@"RolId"];
    [uac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"MemberAccId"];
    [uac setObject:[[UTCApp sharedInstance] lastQurl] forKey:@"Qurl"];
    [uac setObject:[_pinNumberTextBox text] forKey:@"PinNumber"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [uac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [uac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];

    // add in the desired action if known
    [uac setObject:[NSNumber numberWithBool:[[UTCApp sharedInstance] pendingAction] == kScanActionMemberCheckin] forKey:@"RequestCheckin"];
    [uac setObject:[NSNumber numberWithBool:[[UTCApp sharedInstance] pendingAction] == kScanActionMemberRedeem] forKey:@"RequestRedeem"];
    
    NSLog(@"request = %@",uac);
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postUnifiedAction:uac withBlock:^(NSDictionary* uar, NSError *error) {
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
                    [_messageLabel setText:@"This offer requires approval.  Please have a staff member confirm your UTC redemption."];
                    [_resultLabel setText:[errDict objectForKey:@"Message"]];
                    [_pinNumberTextBox setHidden:NO];
                    [_retryButton setHidden:NO];
                }
            }
            [_rewardLabel setHidden:YES];
        } else {
            // set the properties
            _locationName = [uar objectForKey:@"BusName"];
            _locationId = (int)[[uar valueForKey:@"LocId"] integerValue];
            // mark the business for applying credit
            [[UTCSocial sharedInstance] setBusID:(int)[[uar objectForKey:@"BusId"] integerValue]];
            
            // use the results label to tell us the business
            [_resultLabel setText:[uar objectForKey:@"BusName"]];
            // turn on the social sharing buttons
            [_facebookButton setHidden:NO];
            [_twitterButton setHidden:NO];
            // if we checked in set the outputs
            if ( [[uar objectForKey:@"CheckedIn"] boolValue] )
            {
                [_headlineLabel setText:@"CHECK IN"];
                [_messageLabel setText:@"Congratulations! You just checked in for more chances to win."];
                [_rewardLabel setText:@""];
                [_rewardLabel setHidden:YES];
                [[UTCSocial sharedInstance] requestPushToFacebookFeed:[[UTCSocial sharedInstance] postParamForName:_locationName
                                                                                                          Location:_locationId
                                                                                                         andAction:@"Check In"]];
            }
            
            // if we redeemed set the outputs
            if ( [[uar objectForKey:@"Redeemed"] boolValue] )
            {
                [_headlineLabel setText:@"REDEEM"];
                [_messageLabel setText:@"Congratulations! You just redeemed UTC cash for savings up to:"];
                double dealAmount = [[uar objectForKey:@"DealAmount"] doubleValue];
                NSString* reward = [LocationContext formatDeal:dealAmount];
                [_rewardLabel setText:reward];
                [_rewardLabel setHidden:NO];
                [[UTCSocial sharedInstance] requestPushToFacebookFeed:[[UTCSocial sharedInstance] postParamForName:_locationName
                                                                                                          Location:_locationId
                                                                                                         andAction:@"Redeemed"]];
            }
        }
        [_activityView stopAnimating];
    }];
}


@end

