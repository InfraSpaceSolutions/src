//
//  SplashViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "SplashViewController.h"
#import "AccountInfo.h"
#import "UTCReminder.h"

@interface SplashViewController ()
-(void) initializeTips;
-(void) animateTips;
-(void) displayTip;
@end

#define kTipDwellTime                   2.5

@implementation SplashViewController

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
    [self setScreenName:@"Splash"];
    if ([[[UTCApp sharedInstance] locationDictionary] count] <= 0)
    {
        [self loadLocations];
    }
}

-(void)viewWillDisappear:(BOOL)animated
{
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
}

-(void) viewWillAppear:(BOOL)animated
{
    // fix up placement of the bottom tip
    if (![UTCSettings isScreenTall])
    {
        [tipImageA setFrame:CGRectMake(0, 350, 320, 73)];
    }
    // Do any additional setup after loading the view from its nib.
    // TIPS ARE DISABLED UNTIL WE GET NEW GRAPHICS
    [tipImageA setHidden:YES];
    //[self initializeTips];
    
    [UTCReminder countOpportunity];

    [self initializeLocations];
}

-(void) dealloc
{
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) initializeLocations
{
    for (int i=0; i < [_businessLabels count]; i++)
    {
        UILabel* l = (UILabel*)[_businessLabels objectAtIndex:i];
        NSString* busName = [[UTCApp sharedInstance] randomBusinessName];
        busName = [busName uppercaseString];
        [l setText:busName];
    }
}

// create the tip graphics
-(void) initializeTips
{
    // initialize the index to the first image for display - strangely, the last
    tipIndex = 6;
    [self displayTip];
    // animate the different tips on a delay
    [self performSelector:@selector(animateTips) withObject:nil afterDelay:kTipDwellTime];
}

// display the active tip graphic
-(void) displayTip
{
    NSString* imgName = [NSString stringWithFormat:@"splashTip_%d.png", tipIndex];
    switch (tipIndex)
    {
        case 5:
            [tipImageB setImage:[UIImage imageNamed:imgName]];
            [tipImageA setHidden:YES];
            [tipImageB setHidden:NO];
        case 6:
            if ([[[UTCApp sharedInstance] account] isSignedIn])
            {
                [tipImageB setImage:[UIImage imageNamed:imgName]];
            }
            else
            {
                [tipImageB setImage:[UIImage imageNamed:@"splashTip_A.png"]];
            }
            [tipImageA setHidden:YES];
            [tipImageB setHidden:NO];
            break;
        default:
            [tipImageA setImage:[UIImage imageNamed:imgName]];
            [tipImageA setHidden:NO];
            [tipImageB setHidden:YES];
            break;
    }
}


// cycle through the tip images
-(void) animateTips
{
    tipIndex = (tipIndex < (kNumSplashTips-1)) ? tipIndex + 1 : 0;
 
    [self displayTip];
    // set the tip animation up to show the next tip
    [self performSelector:@selector(animateTips) withObject:nil afterDelay:kTipDwellTime];
}

// user clicked the big button - let's do the default action
-(void) clickBig:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openUnite];
}

// load both the locations and the favorites to update the cache
-(void) loadLocations
{
    [[UTCAPIClient sharedClient] getAllLocationsWithBlock:^(NSArray *locations, NSError *error) {
        if (error) {
            NSLog(@"Failed to load locations");
        } else {
            [[UTCApp sharedInstance] updateMemberFavorites:nil];
            [[UTCApp sharedInstance] reloadLocationsFromJSON:locations];
            [self initializeLocations];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

@end
