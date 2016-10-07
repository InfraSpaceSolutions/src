//
//  TutorialInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 1/24/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface TutorialInfo : NSObject

@property (readwrite) BOOL hasDisplayed;

-(id) init;
-(id) initWithAttributes:(NSDictionary*) attributes;

-(void) loadDefaults;
-(void) loadAttributes:(NSDictionary*)attributes;

-(BOOL) loadFromDisk;
-(void) writeToDisk;

@end
