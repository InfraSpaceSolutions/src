//
//  EventInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface EventInfo : NSObject

@property (readonly) int evtID;
@property (readonly) int ettID;
@property (readonly) int busID;
@property (readonly) NSString* busGuid;
@property (readonly) int citID;
@property (readonly) NSString* busName;
@property (readonly) NSString* dateAsString;
@property (readonly) NSString* sortableDate;
@property (readonly) NSString* summary;
@property (readonly) NSString* eventType;
@property (readonly) int catID;
@property (readonly) NSString* catName;
@property (readonly) NSArray* evtTags;
@property (readonly) NSString* startDate;
@property (readonly) NSString* endDate;
@property (readonly) NSString* eventLink;


-(id) initWithAttributes:(NSDictionary*) attributes;
-(NSString*) businessImageURI;
-(NSComparisonResult) compareByDate:(EventInfo*) otherObject;
-(NSString*) concatTags;

@end
