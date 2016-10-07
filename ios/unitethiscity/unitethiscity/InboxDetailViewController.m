//
//  InboxDetailViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "InboxDetailViewController.h"
#import "AccountInfo.h"

@interface InboxDetailViewController ()

@end

@implementation InboxDetailViewController

@synthesize subjectLabel;
@synthesize webView;
@synthesize deleteCrumbButton;

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
    
    [self setScreenName:@"Inbox Detail"];
    
    NSDictionary* msg = [[UTCApp sharedInstance] selectedInboxMessage];

    //[deleteCrumbButton setHidden:[UTCSettings isScreenTall]];
    
    [subjectLabel setText:[msg valueForKey:@"Summary"]];

    
    // Do any additional setup after loading the view from its nib.
    NSString *urlAddress = [NSString stringWithFormat:kMemberViewMessageURL, [[msg valueForKey:@"Id"] intValue],
                            [[[UTCApp sharedInstance] account] apiToken]];
    [webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:urlAddress]]];
    
    [[UTCApp sharedInstance] startActivity];
    // rating view is open and clicked the rate button - save changes
    [[UTCAPIClient sharedClient] postInboxReadWithBlock:[[msg valueForKey:@"Id"] intValue] withBlock:^( NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];

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

-(IBAction) clickDelete:(id)sender
{
    NSDictionary* msg = [[UTCApp sharedInstance] selectedInboxMessage];

    [[UTCApp sharedInstance] startActivity];
    // rating view is open and clicked the rate button - save changes
    [[UTCAPIClient sharedClient] postInboxDeleteWithBlock:[[msg valueForKey:@"Id"] intValue] withBlock:^( NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];

    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickOptOut:(id)sender
{
    NSDictionary* msg = [[UTCApp sharedInstance] selectedInboxMessage];
    
    [[UTCApp sharedInstance] startActivity];
    // rating view is open and clicked the rate button - save changes
    [[UTCAPIClient sharedClient] postInboxOptOutWithBlock:[[msg valueForKey:@"Id"] intValue] withBlock:^( NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end