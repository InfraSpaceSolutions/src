//
//  SearchBusinessListViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "SearchBusinessListViewController.h"
#import "LocationCell.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "AccountInfo.h"

#define kSearchBarTag       1

@implementation SearchBusinessListViewController

@synthesize tableView = _tableView;
@synthesize textSearchBar = _textSearchBar;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // make the search bar transparent
        UISearchBar* searchBar = (UISearchBar*)[self.view viewWithTag:kSearchBarTag];
        [searchBar setBackgroundColor:[UIColor clearColor]];
        [searchBar setBackgroundImage:[UIImage new]];
        [searchBar setTranslucent:YES];
        [searchBar setShowsCancelButton:NO];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    [self setScreenName:@"Search Business List"];
    
}

-(void) viewWillAppear:(BOOL)animated
{
    // initialize the search bar text to the saved value
    [_textSearchBar setText:[[UTCApp sharedInstance] searchText]];
    
    // make sure we have location services turned on
    [[[UTCApp sharedInstance] rootViewController] confirmLocationServices];

    // refresh the user's favorites
    [[UTCApp sharedInstance] startActivity];
    
    [[UTCAPIClient sharedClient] getAllLocationsWithBlock:^(NSArray *locations, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            if ([[[UTCApp sharedInstance] account] isMember]) {
                // a member is active - get the favorites
                [[UTCAPIClient sharedClient] getFavoritesWithBlock:^(NSArray* attr, NSError* error) {
                    if (error) {
                        [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                    } else {
                        // we have updated locations (and favs if apply) - regenerate the dictionary
                        [[UTCApp sharedInstance] updateMemberFavorites:attr];
                        [[UTCApp sharedInstance] reloadLocationsFromJSON:locations];
                        [_tableView reloadData];
                    }
                }];
            }
            else
            {
                [[UTCApp sharedInstance] updateMemberFavorites:nil];
                [[UTCApp sharedInstance] reloadLocationsFromJSON:locations];
                [_tableView reloadData];
            }
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


#pragma mark - Table Data Source Methods -

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [[[UTCApp sharedInstance] matchingLocations] count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kLocationCellHeight;
}

// fill out the cell with the data from the location dictionary
-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* locationCellIdentifier = @"LocationCellIdentifier";
    
    LocationCell* cell = (LocationCell*)[tableView dequeueReusableCellWithIdentifier:locationCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"LocationCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    // get the location info from the dictionary of matching locations
    LocationInfo* locInfo = [[[UTCApp sharedInstance] matchingLocations] objectAtIndex:[indexPath row]];
    
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
    
    // connect the buttons
    [cell.redeemButton setTag:[indexPath row]];
    [cell.redeemButton addTarget:self action:@selector(redeemButtonClicked:) forControlEvents:UIControlEventTouchUpInside];
    [cell.detailsButton setTag:[indexPath row]];
    [cell.detailsButton addTarget:self action:@selector(detailsButtonClicked:) forControlEvents:UIControlEventTouchUpInside];

    // update with details and context - v2
    [cell.dealValueLabel setText:[LocationContext formatDeal:[locInfo dealAmount]]];
    [cell.redeemedBarView setHidden:![locInfo myIsRedeemed]];
    [cell.redeemedWhenLabel setText:[locInfo myRedeemDate]];
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // get the location info from the dictionary of all locations
    LocationInfo* locInfo = [[[UTCApp sharedInstance] matchingLocations] objectAtIndex:[indexPath row]];
    [[UTCApp sharedInstance] setSelectedLocation:locInfo.locID];
    
    [[[UTCApp sharedInstance] rootViewController] openUniteDetail];
}

-(void) tableView:(UITableView*)tableView willDisplayCell:(UITableViewCell *)cell forRowAtIndexPath:(NSIndexPath *)indexPath
{
    //cell.backgroundColor = [UIColor lightGrayColor];
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickSearchByCategory:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchCategories:YES];
}

-(IBAction) clickSearchByKeyword:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchBusinessText:YES];
}


- (void)searchBarSearchButtonClicked:(UISearchBar *)searchBar {
    [self handleSearch:searchBar];
}

- (void)searchBarTextDidEndEditing:(UISearchBar *)searchBar {
    //[self handleSearch:searchBar];
}

- (void)handleSearch:(UISearchBar *)searchBar {
    [searchBar setShowsCancelButton:NO];
    [[UTCApp sharedInstance] setSearchText:searchBar.text];
    [[UTCApp sharedInstance] filterMatchingLocations];
    [_tableView reloadData];
    [searchBar resignFirstResponder]; // if you want the keyboard to go away
}

- (void)searchBarCancelButtonClicked:(UISearchBar *) searchBar {
    [searchBar setShowsCancelButton:NO];
    [searchBar setText:@""];
    [[UTCApp sharedInstance] setSearchText:@""];
    [[UTCApp sharedInstance] filterMatchingLocations];
    [_tableView reloadData];
    [searchBar resignFirstResponder]; // if you want the keyboard to go away
}

- (void)searchBarTextDidBeginEditing:(UISearchBar *)searchBar
{
    [searchBar setShowsCancelButton:YES];
}

-(void) detailsButtonClicked:(UIButton*)sender
{
    NSLog(@"details button clicked");
    // get the location info from the dictionary of all locations
    LocationInfo* locInfo = [[[UTCApp sharedInstance] matchingLocations] objectAtIndex:[sender tag]];
    [[UTCApp sharedInstance] setSelectedLocation:locInfo.locID];
    [[[UTCApp sharedInstance] rootViewController] openSocialTermsAndConditions];
}

-(void) redeemButtonClicked:(UIButton*)sender
{
    NSLog(@"redeem button clicked");
    // get the location info from the dictionary of all locations
    LocationInfo* locInfo = [[[UTCApp sharedInstance] matchingLocations] objectAtIndex:[sender tag]];
    [[UTCApp sharedInstance] setSelectedLocation:locInfo.locID];
    [[[UTCApp sharedInstance] rootViewController] openUniteRedeem];
}

@end
