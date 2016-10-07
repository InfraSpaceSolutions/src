//
//  BusinessTipViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/7/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "BusinessTipViewController.h"
#import "AccountInfo.h"

@interface BusinessTipViewController ()

@end

@implementation BusinessTipViewController

@synthesize reviewerLabel;
@synthesize reviewText;

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
    
    [self setScreenName:@"Business Review Detail"];
    
    NSDictionary* tip = [[UTCApp sharedInstance] selectedReview];
    if (tip) {
        [reviewerLabel setText:[tip valueForKey:@"Name"]];
        [reviewText setText:[tip valueForKey:@"TipText"]];
    }
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