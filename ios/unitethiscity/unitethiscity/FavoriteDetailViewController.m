//
//  FavoriteDetailViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "FavoriteDetailViewController.h"

@interface FavoriteDetailViewController ()

@end

@implementation FavoriteDetailViewController

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
    // Do any additional setup after loading the view from its nib.
    [self setScreenName:@"Favorite Detail"];
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
