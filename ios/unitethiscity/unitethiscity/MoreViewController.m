//
//  MoreViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 12/14/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "EventInfo.h"
#import "EventCell.h"
#import "MenuCell.h"
#import "GalleryCell.h"
#import "MoreViewController.h"

@interface MoreViewController ()

@property (strong, nonatomic) LocationInfo* loc;
@property (strong, nonatomic) NSNumberFormatter* priceFormatter;
@end

@implementation MoreViewController

@synthesize businessLabel;
@synthesize galleryButton;
@synthesize menuButton;
@synthesize calendarButton;
@synthesize galleryView;
@synthesize menuView;
@synthesize calendarView;
@synthesize galleryCollection;
@synthesize menuTable;
@synthesize calendarTable;
@synthesize menuOnlineButton;

@synthesize locationContext;
@synthesize loc;
@synthesize priceFormatter;
@synthesize galleryItems;
@synthesize menuItems;
@synthesize calendarItems;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    galleryItems = [[NSArray alloc] init];
    menuItems = [[NSArray alloc] init];
    calendarItems = [[NSArray alloc] init];

    priceFormatter = [[NSNumberFormatter alloc] init];
    [priceFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [priceFormatter setLocale:[NSLocale currentLocale]];
    [priceFormatter setCurrencyCode:@"USD"];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    [self.galleryCollection registerNib:[UINib nibWithNibName:@"GalleryCell" bundle:[NSBundle mainBundle]] forCellWithReuseIdentifier:@"GalleryCellIdentifier"];
    
    // get the locaiton info from the dictionary of all locations
    loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    [businessLabel setText:[loc name]];
   
    // hide the menu link if it is empty
    [menuOnlineButton setHidden:[[locationContext menuLink] length] == 0];
    
    // find a place to start
    [galleryButton setHidden:([locationContext numGalleryItems] <= 0)];
    [menuButton setHidden:([locationContext numMenuItems] <= 0)];
    [calendarButton setHidden:([locationContext numEvents] <= 0)];
    if ([locationContext numGalleryItems] > 0 ) {
        [self showGallery];
    } else if ([locationContext numMenuItems] > 0) {
        [self showMenu];
    } else if ([locationContext numEvents] > 0) {
        [self showCalendar];
    }
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) showGallery
{
    [galleryButton setSelected:YES];
    [menuButton setSelected:NO];
    [calendarButton setSelected:NO];
    [galleryView setHidden:NO];
    [menuView setHidden:YES];
    [calendarView setHidden:YES];
    
    // load the gallery items
    [[UTCApp sharedInstance] startActivity];
    [[UTCAPIClient sharedClient] getGalleryItemsFor:[loc busID] withBlock:^(NSArray* items, NSError *error) {
        if (error) {
            NSLog(@"loading gallery items failed");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            galleryItems = items;
            NSLog(@"Gallery items = %@", galleryItems);
            [galleryCollection reloadData];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
}

-(void) showMenu
{
    [galleryButton setSelected:NO];
    [menuButton setSelected:YES];
    [calendarButton setSelected:NO];
    [galleryView setHidden:YES];
    [menuView setHidden:NO];
    [calendarView setHidden:YES];

    // load the menu items
    [[UTCApp sharedInstance] startActivity];
    [[UTCAPIClient sharedClient] getMenuItemsFor:[loc busID] withBlock:^(NSArray* items, NSError *error) {
        if (error) {
            NSLog(@"loading menu items failed");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            menuItems = items;
            NSLog(@"Menu items = %@", menuItems);
            [menuTable reloadData];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
}

-(void) showCalendar
{
    [galleryButton setSelected:NO];
    [menuButton setSelected:NO];
    [calendarButton setSelected:YES];
    [galleryView setHidden:YES];
    [menuView setHidden:YES];
    [calendarView setHidden:NO];
    
    // load the calendar items
    [[UTCApp sharedInstance] startActivity];
    [[UTCAPIClient sharedClient] getCalendarItemsFor:[loc busID] withBlock:^(NSArray* items, NSError *error) {
        if (error) {
            NSLog(@"loading calendar items failed");
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // update the fields with our data
            // reload the event dictionary from json and precalc the rest
            NSMutableArray* newEvents = [NSMutableArray arrayWithCapacity:[items count]];
            for (NSDictionary* attributes in items)
            {
                EventInfo* evt = [[EventInfo alloc] initWithAttributes:attributes];
                [newEvents addObject:evt];
            }
            calendarItems = newEvents;
            [calendarTable reloadData];
        }
        [[UTCApp sharedInstance] stopActivity];
    }];
    
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(IBAction) clickGallery:(id)sender
{
    [self showGallery];
}

-(IBAction) clickMenu:(id)sender
{
    [self showMenu];

}

-(IBAction) clickCalendar:(id)sender
{
    [self showCalendar];
}

-(IBAction) clickMenuOnlineButton:(id)sender
{
    NSURL* url = [NSURL URLWithString:[locationContext menuLink]];
    [[UIApplication sharedApplication] openURL:url];

}

-(NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (tableView == menuTable) {
        return [menuItems count];
    }
    if (tableView == calendarTable) {
        return [calendarItems count];
    }
    return 0;
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (tableView == menuTable) {
        return kMenuCellHeight;
    }
    if (tableView == calendarTable) {
        return kEventCellHeight;
    }
    return 0;
}

-(UITableViewCell*) menuTableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* cellIdentifier = @"MenuCellIdentifier";
    MenuCell* cell = (MenuCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"MenuCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    NSUInteger row = [indexPath row];
    NSDictionary* item = [menuItems objectAtIndex:row];
    [[cell nameLabel] setText:[item valueForKey:@"Name"]];
    NSNumber* price = [item valueForKey:@"Price"];
    [[cell priceLabel] setText:[priceFormatter stringFromNumber:price]];
    return cell;
}

-(UITableViewCell*) calendarTableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* cellIdentifier = @"EventCellIdentifier";
    
    EventCell* cell = (EventCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"EventCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    // get the location info from the dictionary of matching
    EventInfo* evtInfo = [calendarItems objectAtIndex:[indexPath row]];
    
    // fill out the cell with our location information
    cell.busNameLabel.text = evtInfo.busName;
    cell.dateLabel.text = evtInfo.dateAsString;
    cell.summaryLabel.text = evtInfo.summary;
    cell.backgroundLabel.backgroundColor = ([indexPath row] % 2) ? [UTCSettings msgBackColorB] :  [UTCSettings msgBackColorA];
    cell.tag = -evtInfo.evtID;
//    [cell.locPicture setImageWithURL:[NSURL URLWithString:[evtInfo businessImageURI]]
//                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    [cell setImageByType:[evtInfo eventType]];
    return cell;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (tableView == menuTable) {
        return [self menuTableView:tableView cellForRowAtIndexPath:indexPath];
    }
    if (tableView == calendarTable) {
        return [self calendarTableView:tableView cellForRowAtIndexPath:indexPath];
    }
    return nil;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    NSLog(@"Selected Row #%lu", (unsigned long)row);
    
    if (tableView == menuTable) {
        NSLog(@"Clicked on menu item");
    }
    if (tableView == calendarTable) {
        NSLog(@"Clicked on calendar item");
        
        // get the event info from the dictionary
        EventInfo* evtInfo = [calendarItems objectAtIndex:[indexPath row]];
        [[UTCApp sharedInstance] setSelectedEvent:evtInfo];
        
        [[[UTCApp sharedInstance] rootViewController] openEventDetail];
    }
    
}


- (NSInteger)collectionView:(UICollectionView *)view numberOfItemsInSection:(NSInteger)section
{
    return [galleryItems count];
}

- (NSInteger)numberOfSectionsInCollectionView: (UICollectionView *)collectionView
{
    return 1;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)cv cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    GalleryCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"GalleryCellIdentifier" forIndexPath:indexPath];
    [cell setBackgroundColor:[UIColor whiteColor]];
    NSDictionary* galleryAttr = (NSDictionary*)[galleryItems objectAtIndex:[indexPath row]];
    
    NSString* uri = [[UTCApp sharedInstance] galleryThumbURI:[galleryAttr objectForKey:@"ImageId"]];
    [cell.galleryImageView setImageWithURL:[NSURL URLWithString:uri] placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    return cell;
}

- (void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath
{
    NSLog(@"selected image %d", (int)[indexPath row]);
    [[[UTCApp sharedInstance] rootViewController] openGalleryImageViewController:(int)[indexPath row] withArray:galleryItems];
}

- (void)collectionView:(UICollectionView *)collectionView didDeselectItemAtIndexPath:(NSIndexPath *)indexPath
{
}

- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout sizeForItemAtIndexPath:(NSIndexPath *)indexPath
{
    CGSize retval = CGSizeMake(95, 95);
    return retval;
}

- (UIEdgeInsets)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout insetForSectionAtIndex:(NSInteger)section
{
    return UIEdgeInsetsMake(5, 8, 5, 8);
}

@end
