//
//  SearchBusinessTextViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 3/27/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCRootViewController.h"
#import "SearchBusinessTextViewController.h"

@interface SearchBusinessTextViewController ()

@end

@implementation SearchBusinessTextViewController

@synthesize popOnGo;
@synthesize isEntertainer;
@synthesize titleLabel;
@synthesize goButton;
@synthesize cancelButton;
@synthesize categoryButton;
@synthesize searchTextField;
@synthesize searchView;

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
    [self setScreenName:@"Search Business Text"];
    
    if (isEntertainer) {
        [titleLabel setText:@"SEARCH ENTERTAINERS"];
    } else {
        [titleLabel setText:@"SEARCH BUSINESSES"];
    }
        
    if ([UTCSettings isScreenTall]) {
        CGRect btFrame;
        btFrame = searchView.frame;
        btFrame.origin.y += 32;
        [searchView setFrame:btFrame];
    }
}

- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [categoryButton setHidden:popOnGo];
    [searchTextField setText:[[UTCApp sharedInstance] searchText]];
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


-(void) clickGo:(id)sender
{
    [[UTCApp sharedInstance] setSearchText:[searchTextField text]];
    if (popOnGo) {
        [[[UTCApp sharedInstance] rootViewController] popActiveView];
    } else {
        if ( isEntertainer ) {
            [[[UTCApp sharedInstance] rootViewController] openSearchEntertainers];
        } else {
            [[[UTCApp sharedInstance] rootViewController] openSearchBusinesses];
        }
    }
}

-(void) clickCancel:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(void) clickCategory:(id)sender
{
    if ( isEntertainer ) {
        [[[UTCApp sharedInstance] rootViewController] openSearchCategories:NO withType:@"Entertainer"];
    } else {
        [[[UTCApp sharedInstance] rootViewController] openSearchCategories:NO];
    }
}

-(void) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end
