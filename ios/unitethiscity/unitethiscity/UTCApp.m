//
//  UTCApp.m
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "EventInfo.h"
#import "TutorialInfo.h"
#import "SocialInfo.h"
#import "UTCAPIClient.h"

static UTCApp* sharedInstance;

@implementation UTCApp

@synthesize selectedLocation;
@synthesize selectedInboxMessage;
@synthesize selectedEvent;
@synthesize selectedReview;
@synthesize memberCoordinates = _memberCoordinates;
@synthesize memberFavorites = _memberFavorites;
@synthesize locationDictionary = _locationDictionary;
@synthesize orderedLocations = _orderedLocations;
@synthesize favoriteLocations = _favoriteLocations;
@synthesize matchingLocations = _matchingLocations;
@synthesize businessLocations = _businessLocations;
@synthesize orderedEntertainers = _orderedEntertainers;
@synthesize matchingEntertainers = _matchingEntertainers;
@synthesize favoriteEntertainers = _favoriteEntertainers;
@synthesize eventDictionary = _eventDictionary;
@synthesize orderedEvents = _orderedEvents;
@synthesize matchingEvents = _matchingEvents;
@synthesize isRetina = _isRetina;
@synthesize account = _account;
@synthesize searchText = _searchText;
@synthesize pendingAction = _pendingAction;
@synthesize locationsRevision = _locationsRevision;
@synthesize devicePushToken = _devicePushToken;
@synthesize facebookStatus = _facebookStatus;
@synthesize searchRefined = _searchRefined;
@synthesize tutorialInfo;
@synthesize socialInfo;
@synthesize searchDate;
@synthesize selectedRange;
@synthesize selectedAccount;
@synthesize hasGlobalAnalytics;
@synthesize hasGlobalStatistics;
@synthesize hasBusinessAnalytics;
@synthesize hasBusinessStatistics;

// perform one time initialization of the application object
-(id) init
{
    self = [super init];
    if (self != nil)
    {
        selectedLocation = 0;
        
        // figure out if we are retina enabled  - do it once and serve the results
        _isRetina = ([[UIScreen mainScreen] respondsToSelector:@selector(scale)] && [[UIScreen mainScreen] scale] >= 2.0);

        // initialize the member position to downtown Cleveland
        _memberCoordinates = [[CLLocation alloc] initWithLatitude:kLocationDefLat longitude:kLocationDefLong];

        // configure the location manager
        locationManager = [[CLLocationManager alloc] init];
        [locationManager setDesiredAccuracy:kLocationAccuracy];
        [locationManager setDistanceFilter:kLocationFilter];
        [locationManager setDelegate:[self appDelegate]];
        
        // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
        if ([locationManager respondsToSelector:@selector(requestAlwaysAuthorization)]) {
            [locationManager requestAlwaysAuthorization];
        }
        CLAuthorizationStatus status = [CLLocationManager authorizationStatus];
        if (status == kCLAuthorizationStatusAuthorizedAlways) {
            [locationManager startMonitoringSignificantLocationChanges];
        } else {
            [locationManager startUpdatingLocation];
        }
        
        // default the account to guest
        _account = [[AccountInfo alloc] init];
        _memberFavorites = [[NSArray alloc] init];
        _accountAntiFilters = [[NSMutableArray alloc] init];
        _businessLocations = [[NSArray alloc] init];
        _searchText = @"";
        
        tutorialInfo = [[TutorialInfo alloc] init];
        socialInfo = [[SocialInfo alloc] init];

        // clear the loaded revison for caching support
        _locationsRevision = 0;
        
        // clear out the device token
        _devicePushToken = @"";
        
        _facebookStatus = kFacebookUnknown;
        
        // flag the search as unrefined
        _searchRefined = NO;
        
        if (![_account loadFromDisk])
        {
            [_account loadDefaults];
            [_account writeToDisk];
        }
        
        if (![self loadAntiFiltersFromDisk])
        {
            _accountAntiFilters = [[NSMutableArray alloc] init];
            [self writeAntiFiltersToDisk];
        }
        
        if (![tutorialInfo loadFromDisk])
        {
            [tutorialInfo loadDefaults];
            [tutorialInfo writeToDisk];
        }
        
        if (![socialInfo loadFromDisk])
        {
            [socialInfo loadDefaults];
            [socialInfo writeToDisk];
        }

#ifdef DEBUG_ACCOUNTINFO_INITWITHTEST
        // load the account information with the test member to skip authentication
        //_account = [[AccountInfo alloc] initWithTestMember];
#endif
        
    }
    return self;
}

// retrieve the application delegate; a helper function for managing retrieval
-(UTCAppDelegate*) appDelegate
{
    UTCAppDelegate* appDelegate = (UTCAppDelegate*)[[UIApplication sharedApplication] delegate];
    return appDelegate;
}

// retrieve the root view controller - all views are navigated through it
-(UTCRootViewController*) rootViewController
{
    UTCAppDelegate* appDelegate = (UTCAppDelegate*)[[UIApplication sharedApplication] delegate];
    UTCRootViewController* rootViewController = [appDelegate viewController];
    return rootViewController;
}

// start the activity spinner
-(void) startActivity
{
    [[self rootViewController] startActivityDisplay];
}

// stop the activity spinner
-(void) stopActivity
{
    [[self rootViewController] stopActivityDisplay];
}

// reload the location dictionary from json and precalc the rest
-(void) reloadLocationsFromJSON:(NSArray*)locationsFromJSON
{
    int oldCount = (int)[_locationDictionary count];
    
    // empty out the location dictionary
    [_locationDictionary removeAllObjects];
    // initialize the dictionary to hold all of the locations
    _locationDictionary = [NSMutableDictionary dictionaryWithCapacity:[locationsFromJSON count]];
    // create a location info and add it to the dictionary for each receieved location
    for (NSDictionary* attributes in locationsFromJSON)
    {
        LocationInfo* loc = [[LocationInfo alloc] initWithAttributes:attributes];
        [loc updateDistanceToMember:_memberCoordinates];
        [_locationDictionary setObject:loc forKey:[NSNumber numberWithInt:loc.locID]];
    }
    [self orderAllLocations];
    [self filterMatchingLocations];
    [self filterFavoriteLocations];
    [self filterBusinessLocations];
    NSLog(@"reloaded fresh locations (%d to %lu total)", oldCount, (unsigned long)[_locationDictionary count]);
}

// sort all of the locations based on the current distance
-(void) orderAllLocations
{
    NSArray *unorderedLocations = [_locationDictionary allValues];
    NSArray* sortedLocations = [unorderedLocations sortedArrayUsingSelector:@selector(compareByDistance:)];
    NSMutableArray* buildLocations =[[NSMutableArray alloc] init];
    NSMutableArray* buildEntertainers = [[NSMutableArray alloc] init];
    
    for (LocationInfo* loc in sortedLocations) {
        if ([loc isEntertainer]) {
            [buildEntertainers addObject:loc];
        } else {
            [buildLocations addObject:loc];
        }
    }
    _orderedLocations = buildLocations;
    _orderedEntertainers = buildEntertainers;
}

// filter the matching locations - fill an array with those matching active criteria
-(void) filterMatchingLocations
{
    // for now, lets just go with everything.
    NSMutableArray* buildMatches = [[NSMutableArray alloc] initWithCapacity:[_locationDictionary count]];
    for (LocationInfo* loc in _orderedLocations)
    {
        if (![self accountAntiFilterContainsCategory:[loc catID]])
        {
            if ([_searchText length] == 0)
            {
                // no search text - just add the location
                [buildMatches addObject:loc];
            }
            else
            {
                // apply the search text
                NSString* locString = [NSString stringWithFormat:@"%@ %@ %@", [loc name], [loc address], [loc concatTags]];
                if ([locString rangeOfString:_searchText options:NSCaseInsensitiveSearch].location != NSNotFound)
                {
                    [buildMatches addObject:loc];
                }
            }
        }
    }
    _matchingLocations = buildMatches;
    buildMatches = [[NSMutableArray alloc] initWithCapacity:[_locationDictionary count]];
    for (LocationInfo* loc in _orderedEntertainers)
    {
        if (![self accountAntiFilterContainsCategory:[loc catID]])
        {
            if ([_searchText length] == 0)
            {
                // no search text - just add the location
                [buildMatches addObject:loc];
            }
            else
            {
                // apply the search text
                NSString* locString = [NSString stringWithFormat:@"%@ %@ %@", [loc name], [loc address], [loc concatTags]];
                if ([locString rangeOfString:_searchText options:NSCaseInsensitiveSearch].location != NSNotFound)
                {
                    [buildMatches addObject:loc];
                }
            }
        }
    }
    _matchingEntertainers = buildMatches;
}

// make a list of just the favorite locations
-(void) filterFavoriteLocations
{
    NSMutableArray* buildFavorites = [[NSMutableArray alloc] init];
    for (LocationInfo* loc in _orderedLocations)
    {
        if ([self favoritesContainsLocation:[loc locID]])
        {
            [buildFavorites addObject:loc];
        }
    }
    _favoriteLocations = buildFavorites;
    buildFavorites = [[NSMutableArray alloc] init];
    for (LocationInfo* loc in _orderedEntertainers)
    {
        if ([self favoritesContainsLocation:[loc locID]])
        {
            [buildFavorites addObject:loc];
        }
    }
    _favoriteEntertainers = buildFavorites;
}

// make a list of just my business locations
-(void) filterBusinessLocations
{
    NSMutableArray* buildBusinesses = [[NSMutableArray alloc] init];
    for (LocationInfo* loc in _orderedLocations)
    {
        // if the location is in our account management set, add it to the array of business locations
        if ([self businessesContainsLocation:[loc busID]])
        {
            [buildBusinesses addObject:loc];
        }
    }
    for (LocationInfo* loc in _orderedEntertainers)
    {
        // if the location is in our account management set, add it to the array of business locations
        if ([self businessesContainsLocation:[loc busID]])
        {
            [buildBusinesses addObject:loc];
        }
    }
    _businessLocations = buildBusinesses;
}

// reload the event dictionary from json and precalc the rest
-(void) reloadEventsFromJSON:(NSArray*)eventsFromJSON
{
    [self reloadEventsFromJSON:eventsFromJSON withType:@""];
}

// reload the event dictionary from json and precalc the rest
-(void) reloadEventsFromJSON:(NSArray*)eventsFromJSON withType:(NSString*)eventType
{
    int oldCount = (int)[_eventDictionary count];
    NSLog(@"events = %@", eventsFromJSON);
    // empty out the location dictionary
    [_eventDictionary removeAllObjects];
    // initialize the dictionary to hold all of the locations
    _eventDictionary = [NSMutableDictionary dictionaryWithCapacity:[eventsFromJSON count]];
    // create an event info and add it to the dictionary for each receieved location
    for (NSDictionary* attributes in eventsFromJSON)
    {
        EventInfo* evt = [[EventInfo alloc] initWithAttributes:attributes];
        // if an event type is defined - limit events to it
        if ([eventType length]) {
            if (!([[evt eventType] isEqualToString:eventType])) {
                //NSLog(@"Discarding event of type %@", [evt eventType]);
                continue;
            }
        }
        [_eventDictionary setObject:evt forKey:[NSNumber numberWithInt:evt.evtID]];
    }
    [self orderAllEvents];
    [self filterMatchingEvents];
    NSLog(@"reloaded fresh events (%d to %lu total)", oldCount, (unsigned long)[_eventDictionary count]);
}

// sort all of the locations based on the current distance
-(void) orderAllEvents
{
    NSArray *unorderedEvents = [_eventDictionary allValues];
    _orderedEvents = [unorderedEvents sortedArrayUsingSelector:@selector(compareByDate:)];
}

// filter the matching events - fill an array with those matching active criteria
-(void) filterMatchingEvents
{
    NSDate* filterDate = searchDate;
    // default to now if we don't have a search date
    if (filterDate == nil)
    {
        filterDate = [NSDate date];
    }
    
    NSDateFormatter *dateFormater = [[NSDateFormatter alloc] init];
    [dateFormater setDateFormat:@"yyyy-MM-dd"];
    NSString *dateFilterString = [dateFormater stringFromDate:filterDate];

    // for now, lets just go with everything.
    NSMutableArray* buildMatches = [[NSMutableArray alloc] initWithCapacity:[_eventDictionary count]];
    for (EventInfo* evt in _orderedEvents)
    {
        // skip dates that are older than the filter date
        if ([dateFilterString caseInsensitiveCompare:[evt endDate]] == NSOrderedDescending)
        {
            NSLog(@"Skipping %@ before %@", [evt endDate], dateFilterString);
            continue;
        }
        if (![self accountAntiFilterContainsCategory:[evt catID]])
        {
            if ([_searchText length] == 0)
            {
                // no search text - just add the location
                [buildMatches addObject:evt];
            }
            else
            {
                // apply the search text
                NSString* evtString = [NSString stringWithFormat:@"%@ %@ %@ %@", [evt busName], [evt summary], [evt concatTags], [evt eventType]];
                if ([evtString rangeOfString:_searchText options:NSCaseInsensitiveSearch].location != NSNotFound)
                {
                    [buildMatches addObject:evt];
                }
            }
        }
    }
    _matchingEvents = buildMatches;
}

// set the text used for searches
-(void) setSearchText:(NSString *)searchText
{
    _searchText = searchText;
    NSLog(@"search text = %@", _searchText);
}

-(void) setPendingAction:(int)pendingAction
{
    _pendingAction = pendingAction;
}
// update the member's physical location and associated distances
-(void) updateMemberCoordinates:(CLLocation *)newCoordinates
{
    NSLog(@"Member has a new physical location - store but dont update distances until needed");
    _memberCoordinates = newCoordinates;
    freshCoordinates = YES;
    
    [self updateDeviceProximity];
}

-(void) updateDeviceProximity
{
    // handle posting the device location to the server to support notifications
    NSMutableDictionary* dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys:_devicePushToken, @"PutToken",
                                 [NSNumber numberWithDouble:_memberCoordinates.coordinate.latitude], @"Latitude",
                                 [NSNumber numberWithDouble:_memberCoordinates.coordinate.longitude], @"Longitude",
                                 nil];
    
    NSDateFormatter* dateToLog;
    dateToLog = [[NSDateFormatter alloc] init];
    [dateToLog setDateStyle:NSDateFormatterShortStyle];

    NSString* logMsg = [NSString stringWithFormat:@"%@ : %@\n", [NSDate date], dict];
    [self logProximityActivity:logMsg];

    if (![_account isSignedIn]) {
        [self logProximityActivity:@"Update Proximity - discard not signed in\n"];
        return;
    }
    if ( [_devicePushToken length] <= 0) {
        [self logProximityActivity:@"Update Proximity - no device token\n"];
        return;
    }
    
    [[UTCAPIClient sharedClient] postProximityUpdate:dict withBlock:^(NSError* error ) {
        if (error) {
            NSLog(@"Unable to send proximity update");
        } else {
            NSLog(@"Sent proximity update");
        }
    }];
 }

// update the member favorites
-(void) updateMemberFavorites:(NSArray*)freshFavorites;
{
    _memberFavorites = [[NSArray alloc] initWithArray:freshFavorites];
}

// refresh the member favorites - store new list of favorites and update the list now
-(void) refreshMemberFavorites:(NSArray *)freshFavorites
{
    _memberFavorites = [[NSArray alloc] initWithArray:freshFavorites];
    [self filterFavoriteLocations];
}

// chec to see if the specified location
-(BOOL) favoritesContainsLocation:(int)locID
{
    return ([_memberFavorites containsObject:[NSNumber numberWithInt:locID]]);
}

// check to see if this business is one we manage
-(BOOL) businessesContainsLocation:(int)busID
{
    return [[_account businessRoles] containsObject:[NSNumber numberWithInt:busID]];
}

// check to see if this is a filtered category
-(BOOL) accountAntiFilterContainsCategory:(int)catID
{
    return ([_accountAntiFilters containsObject:[NSNumber numberWithInt:catID]]);
}

// send the push token for the device to the server
-(void) postPushToken:(BOOL)enabled;
{
    // build the push token dictionary for submission
    NSMutableDictionary* dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[NSNumber numberWithInt:0], @"PutId",
                                 [NSNumber numberWithInt:[_account accID]], @"AccId",
                                 [NSNumber numberWithInt:kPushDeviceTypeID], @"PdtId",
                                 _devicePushToken, @"PutToken",
                                 (enabled) ? @"true": @"false", @"PutEnabled",
                                  nil];
    //NSLog(@"PushToken %@", dict);
    
    // attempt to set the push token
    [[UTCAPIClient sharedClient] postPushToken:dict withBlock:^(NSError* error ) {
        if (error) {
            NSLog(@"Unable to update push token status %@",[UTCAPIClient getMessageFromError:error]);
        } else {
            NSLog(@"Updated push token status %d - token = %@", enabled, _devicePushToken);
            // if we enabled the push toke successfully, and it is enabled, update the device proximity
            if (enabled) {
                [self updateDeviceProximity];
            }
        }
    }];
    
}


// login the user with the dictionary information
-(void) accountLogin:(NSDictionary*)attr
{
    // switch to the member account
    [_account loadAttributes:attr];
    [_account writeToDisk];
    
    // clear out the favorites that may have been loaded for the user
    _memberFavorites = [[NSArray alloc] init];
    _favoriteLocations = [[NSArray alloc] init];
    
    // clear out the filters
    _accountAntiFilters = [[NSMutableArray alloc] init];
    
    // update the avatar in the root controller
    [[self rootViewController] loadAccountImage];
    
    // post the push token for the application
    [self postPushToken:YES];
}

// logout the user
-(void) accountLogout
{
    // post the push token for the application
    [self postPushToken:NO];
    
    // switch to the default guest account
    [_account loadDefaults];
    [_account writeToDisk];
    
    // clear out the favorites that may have been loaded for the user
    _memberFavorites = [[NSArray alloc] init];
    _favoriteLocations = [[NSArray alloc] init];
    
    // clear out the filters
    _accountAntiFilters = [[NSMutableArray alloc] init];

    // update the avatar in the root controller
    [[self rootViewController] loadAccountImage];
}

-(void) createAccountFromFacebook:(NSDictionary*)user
{
    [[UTCApp sharedInstance] startActivity];
    
    // build the parameters to send to create the new account
    NSMutableDictionary* param = [[NSMutableDictionary alloc] init];
    
    [param setObject:[user objectForKey:@"first_name"] forKey:@"AccFName"];
    [param setObject:[user objectForKey:@"last_name"] forKey:@"AccLName"];
    NSString* email = [user objectForKey:@"email"];
    // if no email address was supplied, manufacture a fake one to use
    if (!email) {
        email = [NSString stringWithFormat:@"%@@www.unitethiscity.com", [user objectForKey:@"id"]];
    }
    [param setObject:email forKey:@"AccEMail"];
    [param setObject:@"00000" forKey:@"AccZip"];
    NSString* gender = [user objectForKey:@"gender"];
    [param setObject:@"?" forKey:@"AccGender"];
    if ([gender compare:@"male" options:NSCaseInsensitiveSearch] == NSOrderedSame) {
        [param setObject:@"M" forKey:@"AccGender"];
    }
    if ([gender compare:@"female" options:NSCaseInsensitiveSearch] == NSOrderedSame) {
        [param setObject:@"F" forKey:@"AccGender"];
    }
    [param setObject:@"!nopromocode" forKey:@"PromoCode"];
    [param setObject:[NSNumber numberWithInt:2] forKey:@"PrdID"];
    NSString* birthday = [user objectForKey:@"birthday"];
    if (!birthday) {
        birthday = @"01/01/1900";
    }
    [param setObject:birthday forKey:@"AccBirthdate"];
    [param setObject:[user objectForKey:@"id"] forKey:@"AccFacebookIdentifier"];
    
    
    NSLog(@"Param: %@", param);
    
    // attempt the subscription
    [[UTCAPIClient sharedClient] postFreeSignUp:param withBlock:^(NSDictionary* dic, NSError *error) {
        if (error) {
            NSLog(@"Error: %@", error);
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // build the credentials for submission
            NSString* hashword = [AccountInfo passwordEncryption:[dic objectForKey:@"Password"]];
            NSMutableDictionary* cred = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[dic objectForKey:@"Account"], @"Account", hashword, @"Password", nil];
            // attempt the login
            [[UTCAPIClient sharedClient] postLogin:cred withBlock:^(NSDictionary* dic, NSError *error) {
                if (error) {
                    [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                } else {
                    // perform a login with the attributes
                    [[UTCApp sharedInstance] accountLogin:dic];
                    [[[UTCApp sharedInstance] rootViewController] openSplash];
                }
            }];
        }
    }];
    [[UTCApp sharedInstance] stopActivity];
}

// toggle between the test user and a guest user
-(void) toggleTestUser
{
    if ([_account isMember])
    {
        _account = [[AccountInfo alloc] init];
        // clear out any favorites
        [self refreshMemberFavorites:[[NSArray alloc] init]];
    }
    else
    {
        _account = [[AccountInfo alloc] initWithTestMember];
    }
    // store the active account information for the next start up
    [_account writeToDisk];
    
    [[self rootViewController] loadAccountImage];
}

// store the account information to disk
-(void) writeAntiFiltersToDisk
{
    NSLog(@"UTCApp: WriteAntiFiltersToDisk");
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kAntiFiltersFilename];
    [[self accountAntiFilters] writeToFile:filePath atomically:YES];
}

// load the account info as a dictionary from disk
-(BOOL) loadAntiFiltersFromDisk
{
    NSLog(@"UTCApp: LoadAntiFiltersFromDisk");
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kAntiFiltersFilename];
    _accountAntiFilters = [NSMutableArray arrayWithContentsOfFile:filePath];
    return (_accountAntiFilters!=nil);
}

-(NSString*) randomBusinessName
{
    int count = (int)[_orderedLocations count];
    if ( count <= 0) return @"";
    int r = (arc4random() % count);
    LocationInfo* loc = [_orderedLocations objectAtIndex:r];
    //LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    return loc.name;
}

-(void) logProximityActivity:(NSString *)msg
{
#ifdef PROXIMITY_LOCAL_LOGGING
    //Get the file path
    NSString *documentsDirectory = [NSSearchPathForDirectoriesInDomains (NSDocumentDirectory, NSUserDomainMask, YES) objectAtIndex:0];
    NSString *fileName = [documentsDirectory stringByAppendingPathComponent:kProximityLogFilename];
    
    //create file if it doesn't exist
    if(![[NSFileManager defaultManager] fileExistsAtPath:fileName])
        [[NSFileManager defaultManager] createFileAtPath:fileName contents:nil attributes:nil];
    
    //append text to file (you'll probably want to add a newline every write)
    NSFileHandle *file = [NSFileHandle fileHandleForUpdatingAtPath:fileName];
    [file seekToEndOfFile];
    [file writeData:[msg dataUsingEncoding:NSUTF8StringEncoding]];
    
    [file closeFile];
#endif
}


// determine if we should show the tutorial
-(BOOL) shouldShowTutorial
{
    BOOL ret = YES;
    if (tutorialInfo)
    {
        ret = ![tutorialInfo hasDisplayed];
    }
    return ret;
}

// mark the tutorial as shown
-(void) markTutorialShown
{
    // mark the tutorial as having been displaed
    [tutorialInfo setHasDisplayed:YES];
    // write to disk to prevent showing in the future
    [tutorialInfo writeToDisk];
}

// mark the tutorial as unshown
-(void) markTutorialUnshown
{
    // mark the tutorial as having been displaed
    [tutorialInfo setHasDisplayed:NO];
    // write to disk to prevent showing in the future
    [tutorialInfo writeToDisk];
}

-(NSString*) galleryImageURI:(NSString *)galleryGuid
{
    BOOL isRetina = NO;
    NSString* uri = [NSString stringWithFormat:@"%@%@%@.png", kUTCGalleryImageURL, galleryGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}

-(NSString*) galleryThumbURI:(NSString *)galleryGuid
{
    BOOL isRetina = NO;
    NSString* uri = [NSString stringWithFormat:@"%@%@.thumb%@.png", kUTCGalleryImageURL, galleryGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}


#pragma mark Singleton implementation

// static access to the application object singleton
+ (UTCApp *)sharedInstance
{
    if (!sharedInstance)
    {
        sharedInstance = [[UTCApp alloc] init];
    }
    return sharedInstance;
}

// override zone allocations to utilize singleton behavior
+ (id)allocWithZone:(NSZone*)zone
{
    if (!sharedInstance)
    {
        sharedInstance = [super allocWithZone:zone];
        return sharedInstance;
    }
    else
    {
        return nil;
    }
}

// override zone copying to enforce singleton behavior
- (id)copyWithZone:(NSZone*)zone
{
    return self;
}

-(void) assignStatPermissions:(NSDictionary *)dic {
    [self clearStatPermissions];
    if (dic) {
        hasGlobalStatistics = [[dic objectForKey:@"HasGlobalStatistics"] boolValue];
        hasGlobalAnalytics = [[dic objectForKey:@"HasGlobalAnalytics"] boolValue];
        hasBusinessStatistics = [[dic objectForKey:@"HasBusinessStatistics"] boolValue];
        hasBusinessAnalytics = [[dic objectForKey:@"HasBusinessAnalytics"] boolValue];
    }
}

-(void) clearStatPermissions {
    hasGlobalStatistics = NO;
    hasGlobalAnalytics = NO;
    hasBusinessStatistics = NO;
    hasBusinessAnalytics = NO;
}

-(int) defaultSocialChannel
{
    NSUserDefaults* defaults = [NSUserDefaults standardUserDefaults];
    int value = (int)[defaults integerForKey:@"defaultSocialChannel"];
    return value;
}

-(void) setDefaultSocialChannel:(int)value
{
    NSUserDefaults* defaults = [NSUserDefaults standardUserDefaults];
    NSString* valueString = [NSString stringWithFormat:@"%d",value];
    [defaults setObject:valueString forKey:@"defaultSocialChannel"];
    [defaults synchronize];
}

@end
