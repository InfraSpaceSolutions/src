//
//  TipViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "TipViewController.h"
#import "LocationContext.h"

@interface TipViewController ()

@end

@implementation TipViewController

@synthesize postButton;
@synthesize cancelButton;
@synthesize crumbDoneButton;

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

    [self setScreenName:@"Tip Modify"];

    // set the location name
    [_locationLabel setText:[[[UTCApp sharedInstance] locContext] name]];
    // load any existing tip text from the context
    [_tipText setText:[[[UTCApp sharedInstance] locContext] myTip]];
    
    // Do any additional setup after loading the view from its nib.
    [_tipText becomeFirstResponder];
    
    if ([UTCSettings isScreenTall]) {
        [crumbDoneButton setHidden:YES];
        [postButton setHidden:NO];
        [cancelButton setHidden:NO];
        
    } else {
        [crumbDoneButton setHidden:NO];
        [postButton setHidden:YES];
        [cancelButton setHidden:YES];
    }
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(void) clickPost:(id)sender
{
    // build the dictionary to submit for redemption
    NSMutableDictionary* tip = [[NSMutableDictionary alloc] init];
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    [tip setObject:[NSNumber numberWithInt:lc.accID] forKey:@"AccId"];
    [tip setObject:[NSNumber numberWithInt:lc.locID] forKey:@"LocId"];
    [tip setObject:[_tipText text] forKey:@"Text"];

    [[UTCApp sharedInstance] startActivity];

    // load the location context information for the active user
    [[UTCAPIClient sharedClient] postTip:tip withBlock:^(NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            LocationContext* lc = [[UTCApp sharedInstance] locContext];
            lc.myTip = [_tipText text];
            [[[UTCApp sharedInstance] rootViewController] popActiveView];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
}

-(void) clickCancel:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end
