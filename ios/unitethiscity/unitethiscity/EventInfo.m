//
//  EventInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "EventInfo.h"

@implementation EventInfo

@synthesize evtID = _evtID;
@synthesize ettID = _ettID;
@synthesize busID = _busID;
@synthesize busGuid = _busGuid;
@synthesize citID = _citID;
@synthesize busName = _busName;
@synthesize dateAsString = _dateAsString;
@synthesize sortableDate = _sortableDate;
@synthesize summary = _summary;
@synthesize eventType = _eventType;
@synthesize catID = _catID;
@synthesize catName = _catName;
@synthesize evtTags = _evtTags;
@synthesize startDate = _startDate;
@synthesize endDate = _endDate;

// create an instance from a dictionary of attributes (key/value pairs)
-(id) initWithAttributes:(NSDictionary *)attributes
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _evtID = (int)[[attributes valueForKeyPath:@"Id"] integerValue];
    _ettID = (int)[[attributes valueForKeyPath:@"EttId"] integerValue];
    _busID = (int)[[attributes valueForKeyPath:@"BusId"] integerValue];
    _busGuid = [attributes valueForKeyPath:@"BusGuid"];
    _citID = (int)[[attributes valueForKeyPath:@"CitId"] integerValue];
    _busName = [attributes valueForKeyPath:@"BusName"];
    _dateAsString = [attributes valueForKey:@"DateAsString"];
    _sortableDate = [attributes valueForKey:@"SortableDate"];
    _summary = [attributes valueForKey:@"Summary"];
    _eventType = [attributes valueForKey:@"EventType"];
    _catID = (int)[[attributes valueForKeyPath:@"CatId"] integerValue];
    _catName = [attributes valueForKeyPath:@"CatName"];
    _evtTags = [[NSArray alloc] initWithArray:[attributes valueForKeyPath:@"Properties"]];
    _startDate = [attributes valueForKey:@"StartDate"];
    _endDate = [attributes valueForKey:@"EndDate"];
    _eventLink = [attributes valueForKey:@"EventLink"];
    return self;
}

// get the appropriate resource url for a business by guid; handles retina selection
-(NSString*) businessImageURI
{
    BOOL isRetina = [[UTCApp sharedInstance] isRetina];
    NSString* uri = [NSString stringWithFormat:@"%@%@%@.png", kUTCBusinessImageURL, _busGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}

// compare this event to another to support sorting;  sorts by date, then id
-(NSComparisonResult) compareByDate:(EventInfo *)otherObject
{
    NSComparisonResult dateCompare = [_sortableDate caseInsensitiveCompare:otherObject.sortableDate];
    
    if( dateCompare != NSOrderedSame )
    {
        return dateCompare;
    }
    if (self.evtID < otherObject.evtID)
    {
        return NSOrderedAscending;
    }
    else if (self.evtID > otherObject.evtID)
    {
        return NSOrderedDescending;
    }
    return NSOrderedSame;
}

// generated a comma separated list of tags suitable for display
-(NSString*) concatTags
{
    return [[NSString stringWithFormat:@"%@%@%@",_catName, ([_evtTags count] > 0 ) ? @"," : @"", [_evtTags componentsJoinedByString:@", "]] uppercaseString];
}

@end
