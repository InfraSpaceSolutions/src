//
//  CategoryInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 4/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface CategoryInfo : NSObject

@property (readwrite) int catID;
@property (readwrite) NSString* catName;
@property (readonly) int grpID;
@property (readonly) NSString* grpName;
@property (readwrite) int locCount;

-(id) init;
-(id) initWithAttributes:(NSDictionary*) attributes;

@end
