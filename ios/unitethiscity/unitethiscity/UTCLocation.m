//
//  UTCLocation.m
//  unitethiscity
//
//  Created by Michael Terry on 2/25/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCLocation.h"

@implementation UTCLocation

@synthesize name;
@synthesize address;
@synthesize tags;
@synthesize info;
@synthesize phone;
@synthesize website;
@synthesize rating;
@synthesize distance;
@synthesize cash;

-(id) init
{
    name = @"{Location Name}";
    address = @"{Location Address}";
    tags = @"{Location Tags}";
    info = @"{Location Info}";
    phone = @"{Location Phone}";
    website = @"{Location Website}";
    rating = 1;
    distance = 1.0;
    cash = 10;

    return self;
}

-(id) initWithName:(NSString*)locName
           address:(NSString*)locAddress
              tags:(NSString*)locTags
              info:(NSString*)locInfo
             phone:(NSString*)locPhone
           website:(NSString*)locWebsite
            rating:(int)locRating
          distance:(double)locDistance
              cash:(int)locCash
{
    name = [locName copy];
    address = [locAddress copy];
    tags = [locTags copy];
    info = [locInfo copy];
    phone = [locPhone copy];
    website = [locWebsite copy];
    rating = locRating;
    distance = locDistance;
    cash = locCash;
    
    return self;
}

-(id) initWithAttributes:(NSDictionary *)attributes
{
    
    return self;
}

@end
