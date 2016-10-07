//
//  InboxListViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "InboxListViewController.h"
#import "InboxCell.h"
#import "AccountInfo.h"

@interface InboxListViewController ()

@end

@implementation InboxListViewController

@synthesize noContentView;

NSDateFormatter* dateFromJSON;
NSDateFormatter* dateToScreen;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        inboxMessages = [[NSArray alloc] init];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self setScreenName:@"Inbox List"];
}

-(void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    [[UTCApp sharedInstance] startActivity];
    dateFromJSON = [[NSDateFormatter alloc] init];
    [dateFromJSON setTimeZone:[NSTimeZone timeZoneWithName:@"UTC"]];
    [dateFromJSON setDateFormat:@"yyyy-MM-ddHH:mm:ss"];
    dateToScreen = [[NSDateFormatter alloc] init];
    [dateToScreen setDateStyle:NSDateFormatterShortStyle];
    [dateToScreen setTimeStyle:NSDateFormatterNoStyle];
    
    [noContentView setHidden:YES];
    
    [[UTCAPIClient sharedClient] getAllInboxMessagesWithBlock:^(NSArray *msg, NSError *error) {
        if (error) {
            if ([[[UTCApp sharedInstance] account] isMember]) {
                [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
            }
        } else {
            inboxMessages = msg;
            [noContentView setHidden:([inboxMessages count] != 0)];
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
    return [inboxMessages count];
}

// specify the custom height of our cells
-(CGFloat) tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return kInboxCellHeight;
}

-(UITableViewCell*) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString* inboxCellIdentifier = @"InboxCellIdentifier";
    
    InboxCell* cell = (InboxCell*)[tableView dequeueReusableCellWithIdentifier:inboxCellIdentifier];
    if ( cell == nil )
    {
        NSArray* nib = [[NSBundle mainBundle] loadNibNamed:@"InboxCell" owner:self options:nil];
        cell = [nib objectAtIndex:0];
    }
    
    NSUInteger row = [indexPath row];
    NSDictionary* msg = [inboxMessages objectAtIndex:row];
    cell.fromLabel.text = [msg valueForKey:@"FromName"];
    NSDate* msgDate = [dateFromJSON dateFromString:[msg valueForKey:@"MsgTSAsStr"]];
    cell.dateLabel.text = [NSString stringWithFormat:@"RECEIVED %@", [dateToScreen stringFromDate:msgDate]];
    cell.subjectLabel.text = [msg valueForKey:@"Summary"];
    cell.backgroundLabel.backgroundColor = (row % 2) ? [UTCSettings msgBackColorB] :  [UTCSettings msgBackColorA];
    cell.messageIcon.hidden = [[msg valueForKey:@"InbRead"] boolValue];
    // load the location picture
    [cell.fromPicture setImageWithURL:[NSURL URLWithString:[self messageImageURI:[msg valueForKey:@"BusGuid"]]]
                    placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];

    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSUInteger row = [indexPath row];
    NSLog(@"Selected Row #%lu", (unsigned long)row);
    NSDictionary* msg = [inboxMessages objectAtIndex:row];
    [[UTCApp sharedInstance] setSelectedInboxMessage:msg];
    [[[UTCApp sharedInstance] rootViewController] openInboxDetail];
}

// get the appropriate resource url for a business by guid; handles retina selection
-(NSString*) messageImageURI:(NSString*)busGuid
{
    BOOL isRetina = [[UTCApp sharedInstance] isRetina];
    NSString* uri = [NSString stringWithFormat:@"%@%@%@.png", kUTCBusinessImageURL, busGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}


@end
