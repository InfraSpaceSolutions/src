//
//  BusinessListViewController
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "BusinessListViewController.h"
#import "BusinessCell.h"
#import "LocationInfo.h"
#import "AccountInfo.h"

#define kSearchBarTag       1

@interface BusinessListViewController ()

@end

@implementation BusinessListViewController

@synthesize tableView = _tableView;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self != nil) {
        // make the search bar transparent
        UISearchBar* searchBar = (UISearchBar*)[self.view viewWithTag:kSearchBarTag];
        [searchBar setBackgroundColor:[UIColor clearColor]];
        [searchBar setBackgroundImage:[UIImage new]];
        [searchBar setTranslucent:YES];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];

    [self setScreenName:@"Business List"];
    
    // refresh the user's managed businesses
    [[UTCApp sharedInstance] startActivity];

    [[UTCAPIClient sharedClient] getStatPermissionsWithBlock:^(NSDictionary *perms, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            NSLog(@"statperms = %@", perms);
            [[UTCApp sharedInstance] assignStatPermissions:perms];
            [[UTCAPIClient sharedClient] getAllLocationsWithBlock:^(NSArray *locations, NSError *error) {
                if (error) {
                    [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                } else {
                    [[UTCApp sharedInstance] reloadLocationsFromJSON:locations];
                    [_tableView reloadData];
                }
                [_tableView reloadData];
            }];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


-(IBAction) clickAnalytics:(id)sender
{
    NSLog(@"clicked Analytics");
    
    if ([[UTCApp sharedInstance] hasGlobalAnalytics]) {
        [[[UTCApp sharedInstance] rootViewController] openAnalyticsViewController:0];
    } else {
        [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Access Denied", nil) message:@"Contact us for access to global analytics." delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
    }
}

#pragma mark - Table Data Source Methods -

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if ([[UTCApp sharedInstance] hasGlobalStatistics]) {
        return [[[UTCApp sharedInstance] orderedLocations] count];
    } else if (([[UTCApp sharedInstance] hasBusinessStatistics]) || ([[UTCApp sharedInstance] hasBusinessAnalytics])) {
        return [[[UTCApp sharedInstance] businessLocations] count];
    }
    return 0;
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kBusinessCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* businessCellIdentifier = @"BusinessCellIdentifier";
    
    BusinessCell* cell = (BusinessCell*)[tableView dequeueReusableCellWithIdentifier:businessCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"BusinessCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    // get the location info from the dictionary of business locations
    LocationInfo* locInfo = nil;
    if ([[UTCApp sharedInstance] hasGlobalStatistics]) {
        locInfo = [[[UTCApp sharedInstance] orderedLocations] objectAtIndex:[indexPath row]];
    } else if (([[UTCApp sharedInstance] hasBusinessStatistics]) || ([[UTCApp sharedInstance] hasBusinessAnalytics])) {
        locInfo = [[[UTCApp sharedInstance] businessLocations] objectAtIndex:[indexPath row]];
    }

    // fill out the cell with our location information
    cell.nameLabel.text = locInfo.name;
    cell.distanceLabel.text = [locInfo formatDistanceAsString];
    cell.addressLabel.text = locInfo.address;
    cell.tagsLabel.text = [locInfo concatTags];
    cell.ratingImage.image = [locInfo ratingImage];
    BOOL isFavorite = [[[UTCApp sharedInstance] favoriteLocations] containsObject:locInfo];
    cell.backgroundLabel.backgroundColor = (isFavorite) ? [UTCSettings favoriteBackColor]: [UTCSettings defaultBackColor];
    [cell.markFavorite setHidden:!isFavorite];
    cell.tag = -locInfo.locID;
    
    // load the location picture
    [cell.locPicture setImageWithURL:[NSURL URLWithString:[locInfo businessImageURI]]
                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];

    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // get the location info from the dictionary of all locations
    LocationInfo* locInfo = nil;
    if ([[UTCApp sharedInstance] hasGlobalStatistics]) {
        locInfo = [[[UTCApp sharedInstance] orderedLocations] objectAtIndex:[indexPath row]];
    } else if (([[UTCApp sharedInstance] hasBusinessStatistics]) || ([[UTCApp sharedInstance] hasBusinessAnalytics])) {
        locInfo = [[[UTCApp sharedInstance] businessLocations] objectAtIndex:[indexPath row]];
    }
    [[UTCApp sharedInstance] setSelectedLocation:locInfo.locID];
    
    if (([[UTCApp sharedInstance] hasGlobalStatistics]) || ([[UTCApp sharedInstance] hasBusinessStatistics])) {
        [[[UTCApp sharedInstance] rootViewController] openStatSummaryViewController];
        
    } else if ([[UTCApp sharedInstance] hasBusinessAnalytics]) {
        [[[UTCApp sharedInstance] rootViewController] openAnalyticsViewController:[locInfo busID]];
    }
}

-(void) tableView:(UITableView*)tableView willDisplayCell:(UITableViewCell *)cell forRowAtIndexPath:(NSIndexPath *)indexPath
{
    //cell.backgroundColor = [UIColor lightGrayColor];
}

@end
