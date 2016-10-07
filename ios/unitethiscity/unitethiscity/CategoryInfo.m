//
//  CategoryInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 4/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "CategoryInfo.h"

@implementation CategoryInfo

// create an instance from a dictionary of attributes (key/value pairs)
-(id) init
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _grpID = 0;
    _catID = 0;
    _grpName = @"";
    _catName = @"";
    _locCount = 0;
    
    return self;
}

// create an instance from a dictionary of attributes (key/value pairs)
-(id) initWithAttributes:(NSDictionary *)attributes
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _grpID = (int)[[attributes valueForKeyPath:@"GroupId"] integerValue];
    _catID = (int)[[attributes valueForKeyPath:@"CatId"] integerValue];
    _grpName = [attributes valueForKeyPath:@"GroupName"];
    _catName = [attributes valueForKeyPath:@"CatName"];
    _locCount = (int)[[attributes valueForKeyPath:@"Count"] integerValue];
   
    return self;
}

@end
