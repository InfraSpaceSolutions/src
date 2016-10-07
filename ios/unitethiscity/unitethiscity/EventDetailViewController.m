//
//  EventDetailViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "EventInfo.h"
#import "EventDetailViewController.h"

@interface EventDetailViewController ()

@end

@implementation EventDetailViewController

@synthesize busNameLabel;
@synthesize summaryLabel;
@synthesize eventTypeLabel;
@synthesize dateLabel;
@synthesize bodyView;
@synthesize busPicture;
@synthesize eventCrumbLabel;
@synthesize moreButton;

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

    [self setScreenName:@"Event Detail"];

    EventInfo* evt = [[UTCApp sharedInstance] selectedEvent];
    
    [busNameLabel setText:evt.busName];
    [summaryLabel setText:evt.summary];
    [eventTypeLabel setText:[evt.eventType uppercaseString]];
    [dateLabel setText:evt.dateAsString];
    [bodyView setText:@""];
    [eventCrumbLabel setText:[evt.eventType uppercaseString]];
    [moreButton setHidden:([[evt eventLink] length] == 0)];
    [self reloadContext];

    // load the location picture
    [busPicture setImageWithURL:[NSURL URLWithString:[evt businessImageURI]]
               placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    
    // Do any additional setup after loading the view from its nib.
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

-(void) reloadContext
{
    // get the event info from the dictionary
    EventInfo* evt = [[UTCApp sharedInstance] selectedEvent];
    
    // start the spinner
    [[UTCApp sharedInstance] startActivity];
    
    // load the location context information for the active user
    [[UTCAPIClient sharedClient] getEventContextFor:evt.evtID withBlock:^(NSDictionary* attr, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            NSString* bodyString = [attr valueForKeyPath:@"Body"];
            [bodyView setText:bodyString];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

-(IBAction) clickMore:(id)sender
{
    EventInfo* evt = [[UTCApp sharedInstance] selectedEvent];
    NSURL* url = [NSURL URLWithString:[evt eventLink]];
    [[UIApplication sharedApplication] openURL:url];
}

@end
