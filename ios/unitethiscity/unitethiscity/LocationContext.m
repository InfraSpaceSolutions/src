//
//  LocationContext.m
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "LocationContext.h"
#import "LocationInfo.h"

@implementation LocationContext

@synthesize locID = _locID;
@synthesize busID = _busID;
@synthesize citID = _citID;
@synthesize catID = _catID;
@synthesize busGuid = _busGuid;
@synthesize name = _name;
@synthesize address = _address;
@synthesize rating = _rating;
@synthesize coordinates = _coordinates;
@synthesize catName = _catName;
@synthesize summary = _summary;
@synthesize phone = _phone;
@synthesize webSite = _webSite;
@synthesize facebookLink = _facebookLink;
@synthesize facebookId = _facebookId;
@synthesize requiresPIN = _requiresPIN;
@synthesize dealID = _dealID;
@synthesize dealAmount = _dealAmount;
@synthesize dealName = _dealName;
@synthesize dealDescription = _dealDescription;
@synthesize customTerms = _customTerms;
@synthesize accID = _accID;
@synthesize myTip = _myTip;
@synthesize myRating = _myRating;
@synthesize myIsFavorite = _myIsFavorite;
@synthesize myIsRedeemed = _myIsRedeemed;
@synthesize myIsCheckedIn = _myIsCheckedIn;
@synthesize myCheckInTime = _myCheckInTime;
@synthesize myRedeemDate = _myRedeemDate;
@synthesize numMenuItems = _numMenuItems;
@synthesize numGalleryItems = _numGalleryItems;
@synthesize numEvents = _numEvents;
@synthesize loyaltySummary = _loyaltySummary;
@synthesize pointsCollected = _pointsCollected;
@synthesize pointsNeeded = _pointsNeeded;
@synthesize menuLink = _menuLink;
@synthesize isEntertainer = _isEntertainer;

// initialize to default & safe values
-(id) init
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _locID = 0;
    _busID = 0;
    _citID = 0;
    _catID = 0;
    _catName = @"";
    _busGuid = @"";
    _name = @"";
    _address = @"";
    _rating = 0;
    _coordinates = [[CLLocation alloc] init];
    _summary = @"";
    _phone = @"";
    _webSite = @"";
    _facebookLink = @"";
    _facebookId = @"";
    _requiresPIN = NO;
    _dealID = 0;
    _dealAmount = 0;
    _dealName = @"";
    _dealDescription = @"";
    _customTerms = @"";
    _accID = 0;
    _myTip = @"";
    _myRating = 0.0;
    _myIsFavorite = NO;
    _myIsRedeemed = NO;
    _myIsCheckedIn = NO;
    _myCheckInTime = @"";
    _myRedeemDate = @"";
    _numMenuItems = 0;
    _numGalleryItems = 0;
    _numEvents = 0;
    _pointsNeeded = 0;
    _pointsCollected = 0;
    _loyaltySummary = @"ENTER TO WIN";
    _menuLink = @"";
    _isEntertainer = NO;
    return self;
}

// initialize to default & safe values
-(id) initWithLocationInfo:(LocationInfo*)li
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _locID = [li locID];
    _busID = [li busID];
    _citID = [li citID];
    _catID = [li catID];
    _catName = [li catName];
    _busGuid = [li busGuid];
    _name = [li name];
    _address = [li address];
    _rating = [li rating];
    _coordinates = [li coordinates];
    _summary = @"";
    _phone = @"";
    _webSite = @"";
    _facebookLink = @"";
    _facebookId = @"";
    _requiresPIN = NO;
    _dealID = 0;
    _dealAmount = 0;
    _dealName = @"";
    _dealDescription = @"";
    _customTerms = @"";
    _accID = 0;
    _myTip = @"";
    _myRating = 0.0;
    _myIsFavorite = NO;
    _myIsRedeemed = NO;
    _myIsCheckedIn = NO;
    _myCheckInTime = @"";
    _myRedeemDate = @"";
    _numMenuItems = 0;
    _numGalleryItems = 0;
    _numEvents = 0;
    _pointsCollected = 0;
    _pointsNeeded = 0;
    _loyaltySummary = @"ENTER TO WIN";
    _menuLink = @"";
    _isEntertainer = [li isEntertainer];
    
    return self;
}



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
    _coordinates = [[CLLocation alloc] initWithLatitude:[[attributes valueForKeyPath:@"Latitude"] doubleValue]
                                              longitude:[[attributes valueForKeyPath:@"Longitude"] doubleValue]];
    _summary = [attributes valueForKeyPath:@"Summary"];
    _phone = [attributes valueForKeyPath:@"Phone"];
    _webSite = [attributes valueForKeyPath:@"Website"];
    _facebookLink = [attributes valueForKeyPath:@"FacebookLink"];
    _facebookId = [attributes valueForKeyPath:@"FacebookId"];
    _requiresPIN = [[attributes valueForKeyPath:@"RequiresPIN"] boolValue];
    _dealID = [[attributes valueForKeyPath:@"DealId"] intValue];
    _dealAmount = [[attributes valueForKeyPath:@"DealAmount"] doubleValue];
    _dealName = [attributes valueForKeyPath:@"DealName"];
    _dealDescription = [attributes valueForKeyPath:@"DealDescription"];

    _customTerms = [attributes valueForKeyPath:@"CustomTerms"];
    _accID = (int)[[attributes valueForKeyPath:@"AccId"] integerValue];
    _myTip = [attributes valueForKeyPath:@"MyTip"];
    _myRating = [[attributes valueForKeyPath:@"MyRating"] doubleValue];
    _myIsFavorite = [[attributes valueForKeyPath:@"MyIsFavorite"] boolValue];
    _myIsRedeemed = [[attributes valueForKeyPath:@"MyIsRedeemed"] boolValue];
    _myIsCheckedIn = [[attributes valueForKeyPath:@"MyIsCheckedIn"] boolValue];
    _myCheckInTime = [attributes valueForKeyPath:@"MyCheckInTime"];
    _myRedeemDate = [attributes valueForKeyPath:@"MyRedeemDate"];
    _numMenuItems = (int)[[attributes valueForKeyPath:@"NumMenuItems"] integerValue];
    _numGalleryItems = (int)[[attributes valueForKeyPath:@"NumGalleryItems"] integerValue];
    _numEvents = (int)[[attributes valueForKeyPath:@"NumEvents"] integerValue];
   
    _pointsNeeded = (int)[[attributes valueForKeyPath:@"PointsNeeded"] integerValue];
    _pointsCollected = (int)[[attributes valueForKeyPath:@"PointsCollected"] integerValue];
    _loyaltySummary = [attributes valueForKeyPath:@"LoyaltySummary"];
    _menuLink = [attributes valueForKeyPath:@"MenuLink"];
    
    _isEntertainer = [[attributes valueForKeyPath:@"Entertainer"] boolValue];
    
    return self;
}

// get the image for the location rating
-(UIImage*) ratingImage
{
    double rat = (_rating * 2) + 0.5;
    rat = MAX(0.0, MIN(rat,10.0));
    return [UIImage imageNamed:[NSString stringWithFormat:@"rating%d.png", (int)rat]];
}

// format the deal string based on the supplied amount
+(NSString*) formatDeal:(double)amount
{
    NSString* fmt;
    int intAmount = (int)amount;
    if ( amount <= 0.0)
    {
        // no deal defined or not a valid deal number
        fmt = @"N/A";
    }
    else if ((double)intAmount != amount)
    {
        // show dollars and cents
        fmt = [NSString stringWithFormat:@"$%.2f", amount];
    }
    else
    {
        // its an even dollar- just show dollars
        fmt = [NSString stringWithFormat:@"$%.0f",amount];
    }
    return fmt;
}

// get the formatted string for the deal for this location context
-(NSString*) formatDeal
{
    return [LocationContext formatDeal:_dealAmount];
}

@end
