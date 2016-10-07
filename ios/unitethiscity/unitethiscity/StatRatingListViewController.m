//
//  StatRatingListViewController
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
#import "StatCell.h"
#import "StatRatingListViewController.h"

@interface StatRatingListViewController ()

@property (strong, nonatomic) NSArray* stats;
@property (strong, nonatomic) LocationInfo* loc;

@end

@implementation StatRatingListViewController

@synthesize stats;
@synthesize loc;
@synthesize tableView = _tableView;

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
    
    [self setScreenName:@"Stat Ratings"];
    stats = [[NSArray alloc] init];
    
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    // load the summary statistics
    [[UTCApp sharedInstance] startActivity];
    [[UTCAPIClient sharedClient] getStatRatingFor:[loc busID] withBlock:^(NSArray* attr, NSError *error) {
        if (error) {
            NSLog(@"rating stats load error at view controller");
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
    return kStatCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* inboxCellIdentifier = @"StatCellIdentifier";
    
    StatCell* cell = (StatCell*)[tableView dequeueReusableCellWithIdentifier:inboxCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"StatCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    NSUInteger row = [indexPath row];
    NSDictionary* rec = [stats objectAtIndex:row];
    
    [[cell nameLabel] setText:[rec objectForKey:@"Name"]];
    [[cell timestampLabel] setText:[rec objectForKey:@"TimestampAsString"]];
    [[cell summaryLabel] setText:[rec objectForKey:@"LocationName"]];
    [[cell alternateLabel] setText:@"RATING"];
    [[cell optionalLabel] setHidden:YES];
    [cell.locPicture setImageWithURL:[NSURL URLWithString:[loc businessImageURI]]
                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
    [[cell ratingImage] setHidden:NO];
    double rating = [[rec objectForKey:@"Rating"] doubleValue];
    cell.ratingImage.image = [LocationInfo ratingImage:rating];

    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    NSLog(@"Selected Row #%lu", (unsigned long)row);
}

-(IBAction) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

@end
