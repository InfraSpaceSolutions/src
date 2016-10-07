//
//  UTCApp.h
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

@class UTCAppDelegate;
@class UTCRootViewController;
@class AccountInfo;
@class TutorialInfo;
@class SocialInfo;
@class LocationContext;
@class EventInfo;

@interface UTCApp : NSObject
{
    CLLocationManager* locationManager;
    BOOL freshCoordinates;
}

@property (readwrite) int selectedLocation;
@property (readwrite) NSDictionary* selectedInboxMessage;
@property (readwrite) EventInfo* selectedEvent;
@property (readwrite) NSDictionary* selectedReview;
@property (readwrite) int locationsRevision;
@property (readwrite) int selectedRange;
@property (readwrite) int selectedAccount;

@property(readwrite) NSString* devicePushToken;

@property (readonly) AccountInfo* account;
@property (readwrite) LocationContext* locContext;
@property (readwrite) NSString* lastQurl;

@property (readonly) CLLocation* memberCoordinates;
@property (readonly) NSArray* memberFavorites;
@property (readonly) NSMutableArray* accountAntiFilters;
@property (readwrite) BOOL searchRefined;

@property (readonly) NSMutableDictionary* locationDictionary;
@property (readonly) NSArray* orderedLocations;
@property (readonly) NSArray* matchingLocations;
@property (readonly) NSArray* favoriteLocations;
@property (readonly) NSArray* businessLocations;

@property (readonly) NSArray* orderedEntertainers;
@property (readonly) NSArray* matchingEntertainers;
@property (readonly) NSArray* favoriteEntertainers;

@property (readonly) NSMutableDictionary* eventDictionary;
@property (readonly) NSArray* orderedEvents;
@property (readonly) NSArray* matchingEvents;

@property (readonly) NSString* searchText;
@property (readonly) int pendingAction;

@property (readwrite) NSDate* searchDate;

@property (readonly) BOOL isRetina;

@property (readwrite) int facebookStatus;

@property (readonly) TutorialInfo* tutorialInfo;
@property (readonly) SocialInfo* socialInfo;

@property (readwrite) BOOL hasGlobalStatistics;
@property (readwrite) BOOL hasGlobalAnalytics;
@property (readwrite) BOOL hasBusinessStatistics;
@property (readwrite) BOOL hasBusinessAnalytics;

@property (readwrite) int defaultSocialChannel;

+(UTCApp*) sharedInstance;
-(UTCAppDelegate*) appDelegate;
-(UTCRootViewController*) rootViewController;

-(void) reloadLocationsFromJSON:(NSArray*)locationsFromJSON;
-(void) filterMatchingLocations;
-(void) filterFavoriteLocations;
-(void) updateMemberCoordinates:(CLLocation*)newCoordinates;
-(void) updateMemberFavorites:(NSArray*)freshFavorites;
-(void) refreshMemberFavorites:(NSArray*)freshFavorites;

-(void) reloadEventsFromJSON:(NSArray*)locationsFromJSON;
-(void) reloadEventsFromJSON:(NSArray*)eventsFromJSON withType:(NSString*)eventType;
-(void) filterMatchingEvents;
-(void) orderAllEvents;

-(void) accountLogin:(NSDictionary*)accountDictionary;
-(void) accountLogout;
-(void) postPushToken:(BOOL)enabled;

-(BOOL) favoritesContainsLocation:(int)locID;
-(BOOL) accountAntiFilterContainsCategory:(int)catID;
-(BOOL) businessesContainsLocation:(int)busID;
-(void) startActivity;
-(void) stopActivity;

-(void) toggleTestUser;
-(void) writeAntiFiltersToDisk;
-(BOOL) loadAntiFiltersFromDisk;

-(void) setSearchText:(NSString *)searchText;
-(void) setPendingAction:(int)pendingAction;

-(NSString*) randomBusinessName;

-(void) createAccountFromFacebook:(NSDictionary*)user;

-(BOOL) shouldShowTutorial;
-(void) markTutorialShown;
-(void) markTutorialUnshown;

-(NSString*) galleryImageURI:(NSString *)galleryGuid;
-(NSString*) galleryThumbURI:(NSString *)galleryGuid;
-(void) assignStatPermissions:(NSDictionary*)dic;
-(void) clearStatPermissions;

@end
