//
//  SearchViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "SearchViewController.h"
#import "CategoryCell.h"
#import "CategoryInfo.h"

@interface SearchViewController ()

@end

@implementation SearchViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        categories = [[NSArray alloc] init];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self setScreenName:@"Search"];

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

            categories = makeCats;
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

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [categories count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return 44;
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
    BOOL isAntiFilter = [[[UTCApp sharedInstance] accountAntiFilters] containsObject:[NSNumber numberWithInt:catInfo.catID]];
    
    if ( catInfo.catID == 0 )
    {
        cell.catNameLabel.text = @"";
        cell.cellView.backgroundColor = [UIColor blackColor];
    }
    else
    {
        cell.catNameLabel.text = catInfo.catName;
        cell.cellView.backgroundColor = (isAntiFilter) ? [UIColor lightGrayColor] : [UIColor whiteColor];
    }
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    CategoryInfo* ci = [categories objectAtIndex:row];
    NSMutableArray* anti = [[UTCApp sharedInstance] accountAntiFilters];
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
    
    // write our changes to the anti filters
    [[UTCApp sharedInstance] writeAntiFiltersToDisk];
    
    [_tableView reloadData];
}


@end
