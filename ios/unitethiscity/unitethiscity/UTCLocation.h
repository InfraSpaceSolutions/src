//
//  UTCLocation.h
//  unitethiscity
//
//  Created by Michael Terry on 2/25/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UTCLocation : NSObject
{
    NSString* name;
    NSString* address;
    NSString* tags;
    NSString* info;
    NSString* phone;
    NSString* website;
    int cash;
    int rating;
    double distance;
}

@property (nonatomic, strong) NSString* name;
@property (nonatomic, strong) NSString* address;
@property (nonatomic, strong) NSString* tags;
@property (nonatomic, strong) NSString* info;
@property (nonatomic, strong) NSString* phone;
@property (nonatomic, strong) NSString* website;
@property (nonatomic, assign) int rating;
@property (nonatomic, assign) double distance;
@property (nonatomic, assign) int cash;

-(id) init;
-(id) initWithName:(NSString*)locName
           address:(NSString*)locAddress
              tags:(NSString*)locTags
              info:(NSString*)locInfo
             phone:(NSString*)locPhone
           website:(NSString*)locWebsite
            rating:(int)locRating
          distance:(double)locDistance
              cash:(int)locCash;

-(id) initWithAttributes:(NSDictionary*) attributes;

@end
