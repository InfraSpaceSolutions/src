//
//  DefaultSocialViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/5/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "DefaultSocialViewController.h"

@interface DefaultSocialViewController ()

@end

@implementation DefaultSocialViewController

@synthesize dialogView;
@synthesize closeButtonShort;
@synthesize closeButtonTall;
@synthesize defaultSocial;

- (void)viewDidLoad {
    [super viewDidLoad];
    [super viewDidLoad];
    // round the corners of the background view
    dialogView.layer.cornerRadius = 5;
    dialogView.layer.masksToBounds = YES;
    [closeButtonTall setHidden:![UTCSettings isScreenTall]];
    [closeButtonShort setHidden:[UTCSettings isScreenTall]];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openUniteRedeemResults];

}

-(void) clickNo:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openUniteRedeemResults];
}

-(void) clickYes:(id)sender
{
    [[UTCApp sharedInstance] setDefaultSocialChannel:defaultSocial];
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openUniteRedeemResults];
}

@end
