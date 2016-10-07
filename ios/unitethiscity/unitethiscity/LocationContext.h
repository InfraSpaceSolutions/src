//
//  LocationContext.h
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LocationInfo;

@interface LocationContext : NSObject

@property (readonly) int locID;
@property (readonly) int busID;
@property (readonly) int citID;
@property (readonly) int catID;
@property (readonly) NSString* busGuid;
@property (readonly) NSString* name;
@property (readonly) NSString* address;
@property (readonly) double rating;
@property (readonly) CLLocation* coordinates;
@property (readonly) NSString* catName;
@property (readonly) NSString* summary;
@property (readonly) NSString* phone;
@property (readonly) NSString* webSite;
@property (readonly) NSString* facebookLink;
@property (readonly) NSString* facebookId;
@property (readonly) BOOL requiresPIN;
@property (readonly) int dealID;
@property (readonly) double dealAmount;
@property (readonly) NSString* dealName;
@property (readonly) NSString* dealDescription;
@property (readonly) NSString* customTerms;
@property (readonly) int accID;
@property (readwrite) NSString* myTip;
@property (readwrite) double myRating;
@property (readwrite) BOOL myIsFavorite;
@property (readwrite) BOOL myIsRedeemed;
@property (readwrite) BOOL myIsCheckedIn;
@property (readwrite) NSString* myCheckInTime;
@property (readwrite) NSString* myRedeemDate;
@property (readonly) int numMenuItems;
@property (readonly) int numGalleryItems;
@property (readonly) int numEvents;
@property (readonly) int pointsNeeded;
@property (readonly) int pointsCollected;
@property (readonly) NSString* loyaltySummary;
@property (readonly) NSString* menuLink;
@property (readonly) BOOL isEntertainer;

-(id) init;
-(id) initWithLocationInfo:(LocationInfo*)li;
-(id) initWithAttributes:(NSDictionary*) attributes;

-(NSString*) formatDeal;
-(UIImage*) ratingImage;

+(NSString*) formatDeal:(double)amount;

@end
