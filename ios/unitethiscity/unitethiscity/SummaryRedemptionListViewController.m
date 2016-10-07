//
//  SummaryRedemptionListViewController.m
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
#import "SummaryCell.h"
#import "SummaryRedemptionListViewController.h"

@interface SummaryRedemptionListViewController ()

@property (strong, nonatomic) NSArray* stats;
@property (strong, nonatomic) LocationInfo* loc;
@end

@implementation SummaryRedemptionListViewController

@synthesize stats;
@synthesize tableView = _tableView;
@synthesize loc;

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
    
    [self setScreenName:@"Summary Redemption"];
    stats = [[NSArray alloc] init];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    // load the summary statistics
    [[UTCApp sharedInstance] startActivity];

    [[UTCAPIClient sharedClient] getStatSummaryRedemptionsFor:[loc busID] andScope:([[UTCApp sharedInstance] selectedRange] +1) withBlock:^(NSArray* attr, NSError *error) {
        if (error) {
            NSLog(@"redemption summary load error at view controller");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            stats = attr;
            [_tableView reloadData];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
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


#pragma mark - Table Data Source Methods -

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [stats count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kSummaryCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* inboxCellIdentifier = @"SummaryCellIdentifier";
    
    SummaryCell* cell = (SummaryCell*)[tableView dequeueReusableCellWithIdentifier:inboxCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"SummaryCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    NSUInteger row = [indexPath row];
    NSDictionary* rec = [stats objectAtIndex:row];
    
    [[cell nameLabel] setText:[rec objectForKey:@"Name"]];
    NSString* summaryText = [NSString stringWithFormat:@"%@ (%@)",[LocationContext formatDeal:[[rec objectForKey:@"Sum"] doubleValue]],[rec objectForKey:@"Count"]];
    
    
    [[cell totalLabel] setHidden:NO];
    [[cell totalLabel] setText:summaryText];
    
    [[cell countLabel] setText:[NSString stringWithFormat:@"%@",[rec objectForKey:@"Count"]]];
    [[cell countLabel] setHidden:YES];
    [[cell sumLabel] setText:[LocationContext formatDeal:[[rec objectForKey:@"Sum"] doubleValue]]];
    [[cell sumLabel] setHidden:YES];
    [cell.locPicture setImageWithURL:[NSURL URLWithString:[loc businessImageURI]]
                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    NSDictionary* rec = [stats objectAtIndex:row];
    int accid = [[rec objectForKey:@"AccID"] intValue];
    [[UTCApp sharedInstance] setSelectedAccount:accid];
    [[[UTCApp sharedInstance] rootViewController] openStatRedemptionListViewController];
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end
