//
//  LocationInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface LocationInfo : NSObject

@property (readonly) int locID;
@property (readonly) int busID;
@property (readonly) int citID;
@property (readonly) int catID;
@property (readonly) NSString* busGuid;
@property (readonly) NSString* name;
@property (readonly) NSString* address;
@property (readonly) double rating;
@property (readonly) double distance;
@property (readonly) CLLocation* coordinates;
@property (readonly) NSString* catName;
@property (readonly) NSArray* locTags;
@property (readonly) BOOL isEntertainer;

@property (readonly) int dealID;
@property (readonly) double dealAmount;
@property (readwrite) BOOL myIsRedeemed;
@property (readwrite) BOOL myIsCheckedIn;
@property (readwrite) NSString* myCheckInTime;
@property (readwrite) NSString* myRedeemDate;

-(id) initWithAttributes:(NSDictionary*) attributes;
-(void) updateDistanceToMember:(const CLLocation*)coord;
-(NSString*) formatDistanceAsString;
-(UIImage*) ratingImage;
-(NSString*) businessImageURI;
-(NSComparisonResult) compareByDistance:(LocationInfo*) otherObject;
-(NSString*) concatTags;

+(NSString*) businessImageURIFromGuid:(NSString*)busGuid;
+(UIImage*) ratingImage:(double)rating;

@end
