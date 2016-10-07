//
//  RatingViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 12/27/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "RatingViewController.h"
#import "UTCAppDelegate.h"
#import "AMRatingControl.h"
#import "LocationInfo.h"
#import "LocationContext.h"

@interface RatingViewController ()

@end

@implementation RatingViewController

@synthesize dialogView;
@synthesize closeButtonShort;
@synthesize closeButtonTall;
@synthesize ratingControl;
@synthesize businessName;

- (void)viewDidLoad {
    [super viewDidLoad];
    // round the corners of the background view
    dialogView.layer.cornerRadius = 5;
    dialogView.layer.masksToBounds = YES;
    [closeButtonTall setHidden:![UTCSettings isScreenTall]];
    [closeButtonShort setHidden:[UTCSettings isScreenTall]];
    
    ratingControl = [[AMRatingControl alloc] initWithLocation:CGPointMake(120-75, 120)
                                                   emptyImage:[UIImage imageNamed:@"ratingStarOff.png"]
                                                   solidImage:[UIImage imageNamed:@"ratingStarOn.png"]
                                                 andMaxRating:5];
    [ratingControl setStarWidthAndHeight:32];
    [ratingControl setRating:3];
    [dialogView addSubview:ratingControl];

}

- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    [ratingControl setRating:[lc myRating]];
    [businessName setText:[lc name]];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)  clickDone:(id)sender
{
    [[UTCApp sharedInstance] startActivity];
    LocationContext* lc = [[UTCApp sharedInstance] locContext];
    [lc setMyRating:(int)[ratingControl rating]];
    // rating view is open and clicked the rate button - save changes
    [[UTCAPIClient sharedClient] postRating:(int)[ratingControl rating] forLocation:[[UTCApp sharedInstance] selectedLocation] withBlock:^( NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];

    [self dismissViewControllerAnimated:NO completion:nil];
    
}
-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

@end
