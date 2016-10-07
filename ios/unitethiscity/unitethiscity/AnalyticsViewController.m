//
//  AnalyticsViewController
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "AnalyticsCell.h"
#import "AnalyticsViewController.h"
#import "AbstractActionSheetPicker.h"
#import "ActionSheetStringPicker.h"

@interface AnalyticsViewController ()

@property (strong, nonatomic) NSArray* stats;
@property (strong, nonatomic) NSNumberFormatter* totalFormatter;
@property (strong, nonatomic) NSNumberFormatter* percentFormatter;

@end

@implementation AnalyticsViewController

@synthesize stats;
@synthesize range;
@synthesize tableView = _tableView;
@synthesize totalFormatter;
@synthesize percentFormatter;
@synthesize rangeLabel;
@synthesize busID;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
    }
    range = 1;
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self setScreenName:@"Analytics"];
    stats = [[NSArray alloc] init];
    
    totalFormatter = [[NSNumberFormatter alloc] init];
    [totalFormatter setNumberStyle:NSNumberFormatterDecimalStyle];
    percentFormatter = [[NSNumberFormatter alloc] init];
    [percentFormatter setNumberStyle:NSNumberFormatterPercentStyle];
    [percentFormatter setMinimumFractionDigits:1];
    [percentFormatter setMaximumFractionDigits:1];
    
    [self reloadAnalytics];
    
}

-(void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) reloadAnalytics
{
    switch (range) {
        case 1:
            [rangeLabel setText:@"TODAY"];
            break;
        case 2:
            [rangeLabel setText:@"PAST WEEK"];
            break;
        case 3:
            [rangeLabel setText:@"THIS MONTH"];
            break;
        case 4:
            [rangeLabel setText:@"LAST MONTH"];
            break;
        default:
            [rangeLabel setText:@"ALL TIME"];
            break;
    }
    // load the summary analytics
    [[UTCApp sharedInstance] startActivity];
    
    if (busID > 0) {
        [[UTCAPIClient sharedClient] getAnalyticsSummaryFor:range andBusiness:busID withBlock:^(NSArray* attr, NSError *error) {
            if (error) {
                NSLog(@"error loading analytics");
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            } else {
                // update the fields with our data
                stats = attr;
                [_tableView reloadData];
            }
            [[UTCApp sharedInstance] stopActivity];
        }];
    } else {
        [[UTCAPIClient sharedClient] getAnalyticsSummaryFor:range withBlock:^(NSArray* attr, NSError *error) {
            if (error) {
                NSLog(@"error loading analytics");
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            } else {
                // update the fields with our data
                stats = attr;
                [_tableView reloadData];
            }
            [[UTCApp sharedInstance] stopActivity];
        }];
    }
}

#pragma mark - Table Data Source Methods -

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [stats count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kAnalyticsCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* cellIdentifier = @"AnalyticsCellIdentifier";
    
    AnalyticsCell* cell = (AnalyticsCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"AnalyticsCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    NSUInteger row = [indexPath row];
    NSDictionary* rec = [stats objectAtIndex:row];
    [[cell nameLabel] setText:[rec objectForKey:@"Name"]];
    [[cell totalLabel] setText:[totalFormatter stringFromNumber:[rec objectForKey:@"Total"]]];
    [[cell percentLabel] setText:[percentFormatter stringFromNumber:[rec objectForKey:@"Percent"]]];
    int group = [[rec objectForKey:@"Group"] intValue];
    [[cell backgroundView] setBackgroundColor:((group % 2) == 0) ? [UTCSettings statBackColorB] : [UTCSettings statBackColorA]];
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickSelectRange:(id)sender
{
    NSArray* rangeArray = [NSArray arrayWithObjects:@"Today", @"Past Week", @"This Month", @"Last Month", @"All Time", nil];
    
    [ActionSheetStringPicker showPickerWithTitle:@"Select Range"
                                            rows:rangeArray
                                initialSelection:range - 1
                                       doneBlock:^(ActionSheetStringPicker *picker, NSInteger selectedIndex, id selectedValue) {
                                           range = (int)selectedIndex + 1;
                                           [self reloadAnalytics];
                                       }
                                     cancelBlock:^(ActionSheetStringPicker *picker) {
                                         NSLog(@"Block Picker Canceled");
                                     }
                                          origin:sender];
    
}



@end
