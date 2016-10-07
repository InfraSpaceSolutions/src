//
//  SearchCategoryListViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 5/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "SearchCategoryListViewController.h"
#import "CategoryCell.h"
#import "CategoryInfo.h"

@interface SearchCategoryListViewController ()

@end

@implementation SearchCategoryListViewController

@synthesize popOnApply;
@synthesize eventType;
@synthesize tableView = _tableView;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        categories = [[NSArray alloc] init];
        editAntiFilters = [[NSMutableArray alloc] init];
        [editAntiFilters addObjectsFromArray:[[UTCApp sharedInstance] accountAntiFilters]];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    [self setScreenName:@"Search Category List"];
}

-(void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    // Do any additional setup after loading the view from its nib.
    [[UTCApp sharedInstance] startActivity];
    
    [[UTCAPIClient sharedClient] getCategoriesWithBlock:^(NSArray *cat, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            int lastGroupId = 0;
            NSMutableArray* makeCats = [[NSMutableArray alloc] init];
            for (NSDictionary* rawcat in cat)
            {
                CategoryInfo* ci = [[CategoryInfo alloc] initWithAttributes:rawcat];
                if ([ci locCount] == 0)
                {
                    continue;
                }
                if ( [[rawcat objectForKey:@"GroupId"] integerValue]!= lastGroupId)
                {
                    CategoryInfo* grpcat = [[CategoryInfo alloc] initWithAttributes:rawcat];
                    grpcat.catID = 0;
                    [makeCats addObject:grpcat];
                    lastGroupId = grpcat.grpID;
                }
                [makeCats addObject:ci];
            }
            // categories loaded
            categories = makeCats;
            // initialize the anti-filters based on the loaded categories
            editAntiFilters = [[NSMutableArray alloc] init];
            if ([[UTCApp sharedInstance] searchRefined])
            {
                // search has been filtered, preserve it
                [editAntiFilters addObjectsFromArray:[[UTCApp sharedInstance] accountAntiFilters]];
            }
            else
            {
                // filters have not yet been applied, de-select everything
                // add all of the categories (not groups) to antifilters
                for (CategoryInfo* ci in categories)
                {
                    if ( [ci catID] != 0)
                    {
                        [editAntiFilters addObject:[NSNumber numberWithInt:[ci catID]]];
                    }
                }
            }
            
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

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickSelectAll:(id)sender
{
    // remove all anti-filters
    NSMutableArray* anti = editAntiFilters;
    [anti removeAllObjects];
    [_tableView reloadData];
}

-(IBAction) clickSelectNone:(id)sender
{
    // remove all anti-filters
    NSMutableArray* anti = editAntiFilters;
    [anti removeAllObjects];
    
    // add all of the categories (not groups) to antifilters
    for (CategoryInfo* ci in categories)
    {
        if ( [ci catID] != 0)
        {
            [anti addObject:[NSNumber numberWithInt:[ci catID]]];
        }
    }
    
    [_tableView reloadData];
}

-(void) applyFilter
{
    NSMutableArray* anti = [[UTCApp sharedInstance] accountAntiFilters];
    [anti removeAllObjects];
    [anti addObjectsFromArray:editAntiFilters];
    // flag the search as refined to preserve selections
    [[UTCApp sharedInstance] setSearchRefined:YES];
    
    if (popOnApply) {
        [[[UTCApp sharedInstance] rootViewController] popActiveView];
    } else {
        if (!eventType) {
            [[[UTCApp sharedInstance] rootViewController] openSearchBusinesses];
        } else if ([eventType caseInsensitiveCompare:@"Entertainers"]) {
            [[[UTCApp sharedInstance] rootViewController] openSearchEntertainers];
        } else {
            [[[UTCApp sharedInstance] rootViewController] openSearchEventsWithType:eventType];
        }
    }
}

- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == 1)
    {
        [self applyFilter];
    }
}

-(IBAction) clickApply:(id)sender
{
    // count the number of categories selected
    int numActive = 0;
    for (CategoryInfo* ci in categories)
    {
        if ( [ci catID] != 0)
        {
            if (![editAntiFilters containsObject:[NSNumber numberWithInt:ci.catID]])
            {
                numActive++;
            }
        }
    }
    if (numActive > 0)
    {
        [self applyFilter];
    }
    else
    {
        UIAlertView* alert = [[UIAlertView alloc] initWithTitle:@"No Categories Selected" message:@"" delegate:self cancelButtonTitle:@"Cancel" otherButtonTitles:@"OK", nil];
        [alert show];
    }
    
}

#pragma mark - Table Data Source Methods -

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [categories count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kCategoryCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* categoryCellIdentifier = @"CategoryCellIdentifier";
    
    CategoryCell* cell = (CategoryCell*)[tableView dequeueReusableCellWithIdentifier:categoryCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"CategoryCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    CategoryInfo* catInfo = [categories objectAtIndex:[indexPath row]];
    BOOL isAntiFilter = [editAntiFilters containsObject:[NSNumber numberWithInt:catInfo.catID]];
    
    if ( catInfo.catID == 0 )
    {
        BOOL isOn = YES;
        int i = (int)[indexPath row] + 1;
        while (i < (int)[categories count])
        {
            CategoryInfo* c = [categories objectAtIndex:i];
            // see if we're at the end of the set
            if (([c catID] == 0) || ([c grpID] != [catInfo grpID]))
            {
                break;
            }
            // if it is in the anti-filters, the full set is not on
            if ([editAntiFilters containsObject:[NSNumber numberWithInt:c.catID]])
            {
                isOn = NO;
                break;
            }
            i++;
        }
        
        cell.catNameLabel.text = catInfo.grpName;
        [cell.catNameLabel setTextColor:[UIColor whiteColor]];
        [cell.cellView setBackgroundColor:[UIColor darkGrayColor]];
        [cell.catOn setHidden:YES];
        [cell.catOff setHidden:YES];
        [cell.selectOn setHidden:!isOn];
        [cell.selectOff setHidden:isOn];
    }
    else
    {
        cell.catNameLabel.text = catInfo.catName;
        [cell.catNameLabel setTextColor:[UIColor grayColor]];
        [cell.cellView setBackgroundColor:[UIColor whiteColor]];
        [cell.catOn setHidden:isAntiFilter];
        [cell.catOff setHidden:!isAntiFilter];
        [cell.selectOn setHidden:YES];
        [cell.selectOff setHidden:YES];
    }
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    CategoryInfo* ci = [categories objectAtIndex:row];
    NSMutableArray* anti = editAntiFilters;
    if ([ci catID] != 0)
    {
        if ([anti containsObject:[NSNumber numberWithInt:[ci catID]]])
        {
            [anti removeObject:[NSNumber numberWithInt:[ci catID]]];
        }
        else
        {
            [anti addObject:[NSNumber numberWithInt:[ci catID]]];
        }
    }
    else
    {
        // see if we should add or remove from the anti-filter list based on the state of the first following category's state
        // if we cant find a follower - there shouldn't be anything to do ...
        BOOL remove = (row < [categories count]-1) ? [anti containsObject:[NSNumber numberWithInt:[[categories objectAtIndex:row+1] catID]]] : NO;
        for (int i=(int)row+1; i < [categories count]; i++)
        {
            if ( [[categories objectAtIndex:i] grpID] != [ci grpID])
            {
                break;
            }
            if (remove)
            {
                [anti removeObject:[NSNumber numberWithInt:[[categories objectAtIndex:i] catID]]];
            }
            else
            {
                [anti addObject:[NSNumber numberWithInt:[[categories objectAtIndex:i] catID]]];
            }
        }
    }
    
    [_tableView reloadData];
}

@end
