//
//  GuestDeniedViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCRootViewController.h"
#import "GuestDeniedViewController.h"

@interface GuestDeniedViewController ()

@end

@implementation GuestDeniedViewController

@synthesize headlineLabel = _headlineLabel;
@synthesize actionLabel = _actionLabel;
@synthesize logoImageView = _logoImageView;
@synthesize deniedAction = _deniedAction;

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

    [self setScreenName:@"Guest Denied"];

    // update the messaging based on the denied action
    switch (_deniedAction) {
        case kGuestDeniedCheckIn:
            [_headlineLabel setText:@"WANT A CHANCE TO WIN GREAT PRIZES?"];
            [_actionLabel setText:@"CHECK IN"];
            break;
        case kGuestDeniedRedeem:
            [_headlineLabel setText:@"WANT TO REDEEM YOUR UTC CASH?"];
            [_actionLabel setText:@"REDEEM"];
            break;
        case kGuestDeniedFavorite:
            [_headlineLabel setText:@"WANT UPDATES FROM LOCAL BUSINESSES?"];
            [_actionLabel setText:@"FAVORITES"];
            break;
        case kGuestDeniedRate:
            [_headlineLabel setText:@"WANT YOUR OPINION TO BE HEARD?"];
            [_actionLabel setText:@"RATINGS"];
            break;
        case kGuestDeniedTip:
            [_headlineLabel setText:@"WANT TO HELP US SPREAD THE WORD?"];
            [_actionLabel setText:@"TIPS"];
            break;
        case kGuestDeniedInbox:
            [_headlineLabel setText:@"WANT THE LATEST NEWS ON GREAT DEALS?"];
            [_actionLabel setText:@"NOTIFICATIONS"];
            break;
      default:
            [_headlineLabel setText:@"WANT TO SUPPORT LOCAL CHARITIES?"];
            [_actionLabel setText:@"JOIN US"];
            break;
    }
    
    // hide the logo to save space on 3.5 screens
    [_logoImageView setHidden:(![UTCSettings isScreenTall])];
                               
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

-(void) clickJoinNow:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openAccount];
}

-(void) clickSignIn:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openAccount];
}

@end
