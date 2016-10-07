//
//  SearchMenuViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCRootViewController.h"
#import "SearchMenuViewController.h"

@interface SearchMenuViewController ()

@end

@implementation SearchMenuViewController

@synthesize businessesButton;
@synthesize eventsButton;
@synthesize specialsButton;
@synthesize entertainersButton;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    [self setScreenName:@"Search Menu"];
    if (![UTCSettings isScreenTall]) {
        CGRect btFrame;
        btFrame = businessesButton.frame;
        btFrame.origin.y -= 32;
        [businessesButton setFrame:btFrame];
        
        btFrame = eventsButton.frame;
        btFrame.origin.y -= 32;
        [eventsButton setFrame:btFrame];
        
        btFrame = specialsButton.frame;
        btFrame.origin.y -= 37;
        [specialsButton setFrame:btFrame];
        
        btFrame = entertainersButton.frame;
        btFrame.origin.y -= 37;
        [entertainersButton setFrame:btFrame];
        
    }
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickSearchBusinesss:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchBusinessText:NO];
}

-(void) clickSearchEvents:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchEventText:NO withType:@"Event"];
}

-(void) clickSearchEntertainers:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchEntertainersText:NO];
    //[[[UTCApp sharedInstance] rootViewController] openSearchEventText:NO withType:@"Entertainer"];
}

-(void) clickSearchSpecials:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchEventText:NO withType:@"Special"];
}

@end
