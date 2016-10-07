//
//  LocationInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "LocationInfo.h"

@implementation LocationInfo

@synthesize locID = _locID;
@synthesize busID = _busID;
@synthesize citID = _citID;
@synthesize catID = _catID;
@synthesize busGuid = _busGuid;
@synthesize name = _name;
@synthesize address = _address;
@synthesize rating = _rating;
@synthesize distance = _distance;
@synthesize coordinates = _coordinates;
@synthesize catName = _catName;
@synthesize locTags = _locTags;
@synthesize isEntertainer = _isEntertainer;
@synthesize dealID = _dealID;
@synthesize dealAmount = _dealAmount;
@synthesize myIsRedeemed = _myIsRedeemed;
@synthesize myIsCheckedIn = _myIsCheckedIn;
@synthesize myRedeemDate = _myRedeemDate;
@synthesize myCheckInTime = _myCheckInTime;

// create an instance from a dictionary of attributes (key/value pairs)
-(id) initWithAttributes:(NSDictionary *)attributes
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _locID = (int)[[attributes valueForKeyPath:@"Id"] integerValue];
    _busID = (int)[[attributes valueForKeyPath:@"BusId"] integerValue];
    _citID = (int)[[attributes valueForKeyPath:@"CitId"] integerValue];
    _catID = (int)[[attributes valueForKeyPath:@"CatId"] integerValue];
    _catName = [attributes valueForKeyPath:@"CatName"];
    _busGuid = [attributes valueForKeyPath:@"BusGuid"];
    _name = [attributes valueForKeyPath:@"Name"];
    _address = [attributes valueForKeyPath:@"Address"];
    _rating = [[attributes valueForKeyPath:@"Rating"] doubleValue];
    _distance = kTooFarLimit;
    _coordinates = [[CLLocation alloc] initWithLatitude:[[attributes valueForKeyPath:@"Latitude"] doubleValue]
                                              longitude:[[attributes valueForKeyPath:@"Longitude"] doubleValue]];
    _locTags = [[NSArray alloc] initWithArray:[attributes valueForKeyPath:@"Properties"]];

    _dealID = [[attributes valueForKeyPath:@"DealId"] intValue];
    _dealAmount = [[attributes valueForKeyPath:@"DealAmount"] doubleValue];
    _myIsRedeemed = [[attributes valueForKeyPath:@"MyIsRedeemed"] boolValue];
    _myIsCheckedIn = [[attributes valueForKeyPath:@"MyIsCheckedIn"] boolValue];
    _myCheckInTime = [attributes valueForKeyPath:@"MyCheckInTime"];
    _myRedeemDate = [attributes valueForKeyPath:@"MyRedeemDate"];
    _isEntertainer = [[attributes valueForKeyPath:@"Entertainer"] boolValue];

    return self;
}

// update the distance to the member by calculating distance to this locations coordinates
-(void) updateDistanceToMember:(const CLLocation*)coord;
{
    _distance = [_coordinates distanceFromLocation:coord];
    _distance *= kMetersToMiles;
    _distance = MIN(_distance, kTooFarLimit);
}

// return the distance as a string - blank if it is out of range
-(NSString*) formatDistanceAsString
{
    return (_distance < kTooFarLimit) ? [NSString stringWithFormat:@"%.1f mi", _distance]: @"";
}

// get the image for the location rating
-(UIImage*) ratingImage
{
    double rat = (_rating * 2) + 0.5;
    rat = MAX(0.0, MIN(rat,10.0));
    return [UIImage imageNamed:[NSString stringWithFormat:@"rating%d.png", (int)rat]];
}

// get the image for the location rating
+(UIImage*) ratingImage:(double)rating
{
    double rat = (rating * 2) + 0.5;
    rat = MAX(0.0, MIN(rat,10.0));
    return [UIImage imageNamed:[NSString stringWithFormat:@"rating%d.png", (int)rat]];
}



// get the appropriate resource url for a business by guid; handles retina selection
-(NSString*) businessImageURI
{
    BOOL isRetina = [[UTCApp sharedInstance] isRetina];
    NSString* uri = [NSString stringWithFormat:@"%@%@%@.png", kUTCBusinessImageURL, _busGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}

// compare this location to another to support sorting;  sorts by distance, then id
-(NSComparisonResult) compareByDistance:(LocationInfo *)otherObject
{
    if (self.distance < otherObject.distance)
    {
        return NSOrderedAscending;
    }
    else if (self.distance > otherObject.distance)
    {
        return NSOrderedDescending;
    }
    
    if (self.locID < otherObject.locID)
    {
        return NSOrderedAscending;
    }
    else if (self.locID > otherObject.locID)
    {
        return NSOrderedDescending;
    }
    return NSOrderedSame;
}

// generated a comma separated list of tags suitable for display
-(NSString*) concatTags
{
    return [[NSString stringWithFormat:@"%@%@%@",_catName, ([_locTags count] > 0 ) ? @"," : @"", [_locTags componentsJoinedByString:@", "]] uppercaseString];
}

+(NSString*) businessImageURIFromGuid:(NSString*)busGuid
{
    BOOL isRetina = [[UTCApp sharedInstance] isRetina];
    NSString* uri = [NSString stringWithFormat:@"%@%@%@.png", kUTCBusinessImageURL, busGuid, (isRetina) ? @"@2x" : @""];
    return uri;
}

@end
