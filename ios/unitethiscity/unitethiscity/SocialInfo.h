//
//  SocialInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 1/24/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SocialInfo : NSObject

@property (readwrite) BOOL connectFacebook;
@property (readwrite) BOOL connectTwitter;

-(id) init;
-(id) initWithAttributes:(NSDictionary*) attributes;

-(void) loadDefaults;
-(void) loadAttributes:(NSDictionary*)attributes;

-(BOOL) loadFromDisk;
-(void) writeToDisk;

@end
