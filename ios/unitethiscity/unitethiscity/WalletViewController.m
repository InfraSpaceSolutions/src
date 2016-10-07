//
//  WalletViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//
#import "UTCApp.h"
#import "WalletViewController.h"
#import "AccountInfo.h"
#import "UTCAPIClient.h"

@interface WalletViewController ()

@end


@implementation WalletViewController

@synthesize greetingLabel = _greetingLabel;
@synthesize cashLabel = _cashLabel;
@synthesize pointsLabel = _pointsLabel;
@synthesize scrollView =  _scrollView;
@synthesize contentView = _contentView;
@synthesize backgroundView = _backgroundView;


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
    
    [self setScreenName:@"Wallet"];

    
    // Do any additional setup after loading the view from its nib.
    [_scrollView addSubview:_contentView];
    
    // configure the scrollview size
    [_scrollView setContentSize:_contentView.frame.size];
    
    // round the corners of the background view 
    _backgroundView.layer.cornerRadius = 5;
    _backgroundView.layer.masksToBounds = YES;
    
    AccountInfo* accInfo = [[UTCApp sharedInstance] account];
    // Do any additional setup after loading the view from its nib.
    if ( [[[UTCApp sharedInstance] account] isSignedIn])
    {
        [_greetingLabel setText:[NSString stringWithFormat:@"HEY %@", [[accInfo firstName] uppercaseString]]];
    }
    else
    {
        [_greetingLabel setText:@"HEY YOU"];
    }
    
    // start the spinner
    [[UTCApp sharedInstance] startActivity];
    
    // load the wallet information for the active user
    [[UTCAPIClient sharedClient] getWalletWithBlock:^(NSDictionary* attr, NSError *error) {
        if (error) {
            NSLog(@"location load error at view controller");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            double cashAvailable = [[attr valueForKeyPath:@"CashAvailable"] doubleValue];
            double cashRedeemed = [[attr valueForKeyPath:@"CashRedeemed"] doubleValue];
            int points = [[attr valueForKeyPath:@"PointsAllTime"] intValue];
            //int numCheckins = [[attr valueForKeyPath:@"NumCheckins"] intValue];
            cashAvailable -= cashRedeemed;
            cashAvailable = MAX(cashAvailable, 0);
            
            double cashRedeemedAllTime = [[attr valueForKeyPath:@"CashRedeemedAllTime"] doubleValue];
            
            
            // brute force formatted results for the wallet money
            NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
            [formatter setNumberStyle:NSNumberFormatterCurrencyStyle];
            [formatter setLocale:[NSLocale currentLocale]];
            [formatter setCurrencyCode:@"USD"];
            [_cashLabel setText:[NSString stringWithFormat:@"%@",[formatter stringFromNumber:[NSNumber numberWithDouble:cashAvailable]]]];
            
            [_savingsThisMonth setText:[NSString stringWithFormat:@"%@", [formatter stringFromNumber:[NSNumber numberWithDouble:cashRedeemed]]]];
            [_savingsAllTime setText:[NSString stringWithFormat:@"%@", [formatter stringFromNumber:[NSNumber numberWithDouble:cashRedeemedAllTime]]]];
            
            if ([accInfo isMember])
            {
                [_pointsLabel setText:[NSString stringWithFormat:@"%d POINT%@", points, (points != 1) ? @"S" : @""]];
            }
            else
            {
                [_pointsLabel setText:@"BECOME A MEMBER TO EARN POINTS"];
            }
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
