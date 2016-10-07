//
//  ProximityResultsViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 7/15/14.
//  Copyright (c) 2014 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "ProximityResultsViewController.h"
#import "UTCSocial.h"

#define kProximityAction            1
#define kProximityCheckedIn         2
#define kProximityRedeemed          3

@interface ProximityResultsViewController ()
-(void) performActionRedeem;
@end

@implementation ProximityResultsViewController

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
    
    [self setScreenName:@"Proximity Results"];
    
    // hide the logo to save space on 3.5 screens
    [_logoImageView setHidden:(![UTCSettings isScreenTall])];
    
    // kickstart the action process
    //[self performActionRedeem];
    [self prepareAction];
    
    [_okButton setHidden:YES];
    [_checkInButton setHidden:YES];
    [_redeemButton setHidden:YES];
    [_retryButton setHidden:YES];
    [_activityView setHidden:NO];
    [_resultLabel setHidden:YES];
    [_headlineLabel setHidden:NO];
    [_rewardLabel setHidden:YES];
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

-(void) showScreenByIndex:(int)index
{

    [_actionView setHidden:YES];
    [_checkInView setHidden:YES];
    [_redeemView setHidden:YES];
    
    switch (index) {
        case kProximityAction:
            [_actionView setHidden:NO];
            break;
        case kProximityCheckedIn:
            [_checkInView setHidden:NO];
            break;
        case kProximityRedeemed:
            [_redeemView setHidden:NO];
            break;
        default:
            break;
    }
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
    [self performActionRedeem];
}

-(void) clickRedeem:(id)sender
{
    [self performActionRedeem];
}


-(void) clickCheckIn:(id)sender
{
    [self performActionCheckIn];
}

-(void) clickTwitter:(id)sender
{
    [[UTCSocial sharedInstance] openSLTwitter:self
                                  withMessage:[[UTCSocial sharedInstance] messageForName:_locationName]
                                      andLink:[[UTCSocial sharedInstance] linkForLocID:_locationId]];
}

-(void) clickFacebook:(id)sender
{
    NSString* reward = [_rewardLabel text];
    
    NSString* msg = [NSString stringWithFormat:@"%@: %@ Savings", _locationName, reward];
    [[UTCSocial sharedInstance] openSLFacebook:self
                                   withMessage:msg
                                       andLink:[[UTCSocial sharedInstance] linkForLocID:_locationId]];
}


-(void) prepareAction
{
    [self showScreenByIndex:kProximityAction];

    [_headlineLabel setText:@"Working..."];
    [_activityView setHidesWhenStopped:YES];
    [_activityView startAnimating];
    [_pinNumberTextBox setHidden:YES];
    [_retryButton setHidden:YES];
    [_nackImage setHidden:YES];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] getLocationContextFor:[[UTCApp sharedInstance] selectedLocation] withBlock:^(NSDictionary* attr, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // parse the context from the response
            LocationContext* lc = [[LocationContext alloc] initWithAttributes:attr];
            // assign the context to the application
            [[UTCApp sharedInstance] setLocContext:lc];
            [_resultLabel setText:[lc name]];
            [_resultLabel setHidden:NO];
            [_headlineLabel setText:@"SAHRE BELOW TO SAVE"];
            [_headlineLabel setHidden:NO];
            [_rewardLabel setText:[LocationContext formatDeal:[lc dealAmount]]];
            [_rewardLabel setHidden:NO];
            [_okButton setHidden:YES];
            [_retryButton setHidden:YES];
            [_checkInButton setHidden:NO];
            [_redeemButton setHidden:NO];
            
            if ([lc myIsRedeemed]) {
                [_messageLabel setText:[NSString stringWithFormat:@"Redeemed %@", [lc myRedeemDate]]];
            } else {
                [_messageLabel setText:@"Redeem to claim your reward"];
            }
            if ([lc myIsCheckedIn]) {
                [_messageLabel setText:[NSString stringWithFormat:@"%@ Checked In %@", [_messageLabel text], [lc myCheckInTime]]];
            }
            [_messageLabel setHidden:NO];
        }
        [_activityView stopAnimating];
    }];
}


-(void) performActionRedeem
{
    [_headlineLabel setText:@"Working..."];
    [_activityView setHidesWhenStopped:YES];
    [_activityView startAnimating];
    [_pinNumberTextBox setHidden:YES];
    [_retryButton setHidden:YES];
    [_checkInButton setHidden:YES];
    [_redeemButton setHidden:YES];
    
    // get the context information to define the redemption
    AccountInfo* acc = [[UTCApp sharedInstance] account];
    
    // build the dictionary to submit for unified action
    NSMutableDictionary* pac = [[NSMutableDictionary alloc] init];
    
    [pac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"AccId"];
    [pac setObject:[NSNumber numberWithInt:kRoleMember] forKey:@"RolId"];
    [pac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"MemberAccId"];
    [pac setObject:[NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]] forKey:@"LocId"];
    [pac setObject:@"" forKey:@"Qurl"];
    [pac setObject:[_pinNumberTextBox text] forKey:@"PinNumber"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [pac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [pac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // add in the desired action if known
    [pac setObject:[NSNumber numberWithBool:NO] forKey:@"RequestCheckin"];
    [pac setObject:[NSNumber numberWithBool:YES] forKey:@"RequestRedeem"];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postProximityAction:pac withBlock:^(NSDictionary* par, NSError *error) {
        [_headlineLabel setText:@"REDEEM"];
        if (error) {
            [_retryButton setHidden:NO];
            [_checkInButton setHidden:NO];
            [_okButton setHidden:YES];
            [_messageLabel setText:[error localizedDescription]];
            [_rewardLabel setHidden:YES];
            [_nackImage setHidden:NO];

            //[_resultLabel setText:@"UNSUCCESSFUL"];
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
                    [_rewardLabel setHidden:YES];
                    [_nackImage setHidden:YES];
                }
            }
        } else {
            [_retryButton setHidden:YES];
            [_checkInButton setHidden:YES];
            [_okButton setHidden:NO];

            // set the properties
            _locationName = [par objectForKey:@"BusName"];
            _locationId = (int)[[par valueForKey:@"LocId"] integerValue];
            // mark the business for applying credit
            [[UTCSocial sharedInstance] setBusID:(int)[[par objectForKey:@"BusId"] integerValue]];
            
            // use the results label to tell us the business
            [_resultLabel setText:[par objectForKey:@"BusName"]];

            // if we redeemed set the outputs
            if ( [[par objectForKey:@"Redeemed"] boolValue] )
            {
                [_redeemedValueLabel setText:[_rewardLabel text]];

                [self showScreenByIndex:kProximityRedeemed];

//                [_messageLabel setText:@"Congratulations! You just redeemed UTC cash for savings"];
//                double dealAmount = [[par objectForKey:@"DealAmount"] doubleValue];
//                NSString* reward = [LocationContext formatDeal:dealAmount];
//                [_rewardLabel setText:reward];
//                [_rewardLabel setHidden:NO];
                AudioServicesPlayAlertSound(kSystemSoundID_Vibrate);
//                [[UTCSocial sharedInstance] requestPushToFacebookFeed:[[UTCSocial sharedInstance] postParamForName:_locationName
//                                                                                                          Location:_locationId
//                                                                                                         andAction:@"Redeemed"]];
            }
        }
        [_activityView stopAnimating];
    }];
}

-(void) performActionCheckIn
{
    [_headlineLabel setText:@"Working..."];
    [_activityView setHidesWhenStopped:YES];
    [_activityView startAnimating];
    [_pinNumberTextBox setHidden:YES];
    [_retryButton setHidden:YES];
    [_checkInButton setHidden:YES];
    [_redeemButton setHidden:YES];
    [_nackImage setHidden:YES];
    
    // get the context information to define the redemption
    AccountInfo* acc = [[UTCApp sharedInstance] account];
    
    // build the dictionary to submit for unified action
    NSMutableDictionary* pac = [[NSMutableDictionary alloc] init];
    
    [pac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"AccId"];
    [pac setObject:[NSNumber numberWithInt:kRoleMember] forKey:@"RolId"];
    [pac setObject:[NSNumber numberWithInt:acc.accID] forKey:@"MemberAccId"];
    [pac setObject:[NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]] forKey:@"LocId"];
    [pac setObject:@"" forKey:@"Qurl"];
    [pac setObject:[_pinNumberTextBox text] forKey:@"PinNumber"];
    CLLocation* mbrLoc = [[UTCApp sharedInstance] memberCoordinates];
    [pac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.latitude] forKey:@"Latitude"];
    [pac setObject:[NSNumber numberWithDouble:mbrLoc.coordinate.longitude] forKey:@"Longitude"];
    
    // add in the desired action if known
    [pac setObject:[NSNumber numberWithBool:YES] forKey:@"RequestCheckin"];
    [pac setObject:[NSNumber numberWithBool:NO] forKey:@"RequestRedeem"];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postProximityAction:pac withBlock:^(NSDictionary* par, NSError *error) {
        [_headlineLabel setText:@"CHECK IN"];
        if (error) {
            [_retryButton setHidden:YES];
            [_checkInButton setHidden:NO];
            [_redeemButton setHidden:NO];
            [_okButton setHidden:YES];
            [_messageLabel setText:[error localizedDescription]];
            //[_resultLabel setText:@"UNSUCCESSFUL"];
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
                }
            }
        } else {
            //[_retryButton setHidden:YES];
            //[_checkInButton setHidden:YES];
            //[_okButton setHidden:NO];
            
            // set the properties
            _locationName = [par objectForKey:@"BusName"];
            _locationId = (int)[[par valueForKey:@"LocId"] integerValue];
            // mark the business for applying credit
            [[UTCSocial sharedInstance] setBusID:(int)[[par objectForKey:@"BusId"] integerValue]];
            
            // use the results label to tell us the business
            //[_resultLabel setText:[par objectForKey:@"BusName"]];
            
            // if we checked in set the outputs
            if ( [[par objectForKey:@"CheckedIn"] boolValue] )
            {
                [self showScreenByIndex:kProximityCheckedIn];
                AudioServicesPlayAlertSound(kSystemSoundID_Vibrate);
            }
        }
        [_activityView stopAnimating];
    }];
}

@end

