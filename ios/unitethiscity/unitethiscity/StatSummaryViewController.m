//
//  StatSummaryViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "StatSummaryViewController.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "AbstractActionSheetPicker.h"
#import "ActionSheetStringPicker.h"

#define kStatsRedemption    0
#define kStatsCheckIns      1
#define kStatsFavorites     2
#define kStatsRatings       3
#define kStatsTips          4
#define kStatsSocial        5

@interface StatSummaryViewController ()

@property (readwrite) int activeRange;
@property (strong, nonatomic) NSArray* summaryStats;

@end

@implementation StatSummaryViewController

@synthesize scrollView;
@synthesize contentView;
@synthesize backgroundView;
@synthesize businessNameLabel;

@synthesize summaryTitleLabel;
@synthesize socialPointsLabel;
@synthesize loyaltyPointsLabel;
@synthesize favoritesLabel;
@synthesize ratingsLabel;
@synthesize reviewsLabel;

@synthesize activeRange;
@synthesize summaryStats;

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
    
    [self setScreenName:@"StatSummary"];
    
    activeRange = kRangeToday;
    [[UTCApp sharedInstance] setSelectedRange:activeRange];
    
    // Do any additional setup after loading the view from its nib.
    [scrollView addSubview:contentView];
    
    // configure the scrollview size
    [scrollView setContentSize:contentView.frame.size];
    
    // round the corners of the background view
    backgroundView.layer.cornerRadius = 5;
    backgroundView.layer.masksToBounds = YES;
    
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    [businessNameLabel setText:[loc name]];
    
    // load the summary statistics
    [[UTCApp sharedInstance] startActivity];
    [[UTCAPIClient sharedClient] getStatSummaryFor:[loc busID] withBlock:^(NSArray* attr, NSError *error) {
        if (error) {
            NSLog(@"summary stats load error at view controller");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            summaryStats = attr;
            [self reloadFromStats];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

-(void) reloadFromStats
{
    NSString* rangeTitle = @"Today";
    NSString* rangeField = @"Today";

    // work with the active range - set us up to map in the right values
    switch (activeRange) {
        case kRangeToday:
            rangeField = @"Today";
            rangeTitle = @"Today";
            break;
        case kRangePastWeek:
            rangeField = @"PastWeek";
            rangeTitle = @"Past Week";
            break;
        case kRangeThisMonth:
            rangeField = @"ThisPeriod";
            rangeTitle = @"This Month";
            break;
        case kRangeLastMonth:
            rangeField = @"LastPeriod";
            rangeTitle = @"Last Month";
            break;
        case kRangeAllTime:
            rangeField = @"AllTime";
            rangeTitle = @"All Time";
            break;
    }
    [summaryTitleLabel setText:rangeTitle];

    [socialPointsLabel setText:[NSString stringWithFormat:@"%d", (int)[[summaryStats[kStatsRedemption] objectForKey:rangeField] intValue]]];
    [loyaltyPointsLabel setText:[NSString stringWithFormat:@"%d", (int)[[summaryStats[kStatsCheckIns] objectForKey:rangeField] intValue]]];
    [favoritesLabel setText:[NSString stringWithFormat:@"%d", (int)[[summaryStats[kStatsFavorites] objectForKey:rangeField] intValue]]];
    [ratingsLabel setText:[NSString stringWithFormat:@"%d", (int)[[summaryStats[kStatsRatings] objectForKey:rangeField] intValue]]];
    [reviewsLabel setText:[NSString stringWithFormat:@"%d", (int)[[summaryStats[kStatsTips] objectForKey:rangeField] intValue]]];
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickSocialDeals:(id)sender
{
//    [[[UTCApp sharedInstance] rootViewController] openStatRedemptionListViewController];
    [[[UTCApp sharedInstance] rootViewController] openSummaryRedemptionListViewController];
}

-(IBAction) clickLoyaltyDeals:(id)sender
{
//    [[[UTCApp sharedInstance] rootViewController] openStatCheckInListViewController];
    [[[UTCApp sharedInstance] rootViewController] openSummaryCheckInListViewController];
}

-(IBAction) clickFavorites:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openStatFavoriteListViewController];
}

-(IBAction) clickRatings:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openStatRatingListViewController];
}

-(IBAction) clickReviews:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openStatTipListViewController];
}

-(IBAction) clickSelectRange:(id)sender
{
    NSArray* rangeArray = [NSArray arrayWithObjects:@"Today", @"Past Week", @"This Month", @"Last Month", @"All Time", nil];
    
    [ActionSheetStringPicker showPickerWithTitle:@"Select Range"
                                            rows:rangeArray
                                initialSelection:activeRange
                                       doneBlock:^(ActionSheetStringPicker *picker, NSInteger selectedIndex, id selectedValue) {
                                           activeRange = (int)selectedIndex;
                                           [[UTCApp sharedInstance] setSelectedRange:activeRange];
                                           [self reloadFromStats];
                                       }
                                     cancelBlock:^(ActionSheetStringPicker *picker) {
                                         NSLog(@"Block Picker Canceled");
                                     }
                                          origin:sender];
    
}

-(void) rangeSelected:(NSString*)selectedRange element:(id)element {
    NSLog(@"RANGE selected %@", selectedRange);
}

-(void) clickAnalytics:(id)sender {
    if ([[UTCApp sharedInstance] hasGlobalAnalytics] || [[UTCApp sharedInstance] hasBusinessAnalytics]) {
        // get the location id as a key for accessing the dictionary
        NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
        // get the locaiton info from the dictionary of all locations
        LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
        [[[UTCApp sharedInstance] rootViewController] openAnalyticsViewController:[loc busID]];
    } else {
        [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Access Denied", nil) message:@"Contact us for access to business analytics." delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
    }
}

@end
