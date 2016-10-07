//
//  SearchEventListViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "SearchEventListViewController.h"
#import "EventCell.h"
#import "EventInfo.h"
#import "AccountInfo.h"

#define kSearchBarTag       1

@implementation SearchEventListViewController

@synthesize tableView = _tableView;
@synthesize textSearchBar = _textSearchBar;
@synthesize eventType;
@synthesize eventTypeLabel;
@synthesize eventDate;

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
    [self setScreenName:@"Search Event List"];
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    if ([eventType isEqualToString:@"Entertainer"]) {
        [eventTypeLabel setText:@"ENTERTAINERS"];
    }
    if ([eventType isEqualToString:@"Special"]) {
        [eventTypeLabel setText:@"SPECIALS"];
    }
    eventDate = [[UTCApp sharedInstance] searchDate];
    if (!eventDate) {
        eventDate = [NSDate date];
    }
    
    // initialize the search bar text to the saved value
    [_textSearchBar setText:[[UTCApp sharedInstance] searchText]];
    
    // refresh the user's favorites
    [[UTCApp sharedInstance] startActivity];
    
    [[UTCAPIClient sharedClient] getAllEventsWithBlock:^(NSArray *events, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            [[UTCApp sharedInstance] reloadEventsFromJSON:events withType:eventType];
            [_tableView reloadData];
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
    return [[[UTCApp sharedInstance] matchingEvents] count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kEventCellHeight;
}

// fill out the cell with the data from the dictionary
-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* eventCellIdentifier = @"EventCellIdentifier";
    
    EventCell* cell = (EventCell*)[tableView dequeueReusableCellWithIdentifier:eventCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"EventCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    // get the location info from the dictionary of matching
    EventInfo* evtInfo = [[[UTCApp sharedInstance] matchingEvents] objectAtIndex:[indexPath row]];
    
    // fill out the cell with our location information
    cell.busNameLabel.text = evtInfo.busName;
    cell.dateLabel.text = evtInfo.dateAsString;
    cell.summaryLabel.text = evtInfo.summary;
    cell.backgroundLabel.backgroundColor = ([indexPath row] % 2) ? [UTCSettings msgBackColorB] :  [UTCSettings msgBackColorA];
    cell.tag = -evtInfo.evtID;
    [cell.locPicture setImageWithURL:[NSURL URLWithString:[evtInfo businessImageURI]]
                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // get the event info from the dictionary
    EventInfo* evtInfo = [[[UTCApp sharedInstance] matchingEvents] objectAtIndex:[indexPath row]];
    [[UTCApp sharedInstance] setSelectedEvent:evtInfo];
    
    [[[UTCApp sharedInstance] rootViewController] openEventDetail];
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
    [[[UTCApp sharedInstance] rootViewController] openSearchCategories:YES withType:eventType];
}

-(IBAction) clickSearchByKeyword:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchEventText:YES withType:eventType];
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
    [[UTCApp sharedInstance] filterMatchingEvents];
    [_tableView reloadData];
    [searchBar resignFirstResponder]; // if you want the keyboard to go away
}

- (void)searchBarCancelButtonClicked:(UISearchBar *) searchBar {
    [searchBar setShowsCancelButton:NO];
    [searchBar setText:@""];
    [[UTCApp sharedInstance] setSearchText:@""];
    [[UTCApp sharedInstance] filterMatchingEvents];
    [_tableView reloadData];
    [searchBar resignFirstResponder]; // if you want the keyboard to go away
}

- (void)searchBarTextDidBeginEditing:(UISearchBar *)searchBar
{
    [searchBar setShowsCancelButton:YES];
}

@end