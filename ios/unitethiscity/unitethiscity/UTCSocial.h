//
//  UTCSocial.h
//  unitethiscity
//
//  Created by Michael Terry on 5/21/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UTCSocial : NSObject

+(UTCSocial*) sharedInstance;

@property (readwrite) int busID;
@property (readwrite) int sptID;

-(void) openSLFacebook:(UIViewController*)vc withMessage:(NSString*)msg andLink:(NSString*)link;
-(void) openSLTwitter:(UIViewController*)vc withMessage:(NSString*)msg andLink:(NSString*)link;

-(void) openInstagram:(UIViewController*)vc withMessage:(NSString*)msg andLink:(NSString*)link;

-(NSString*) messageForLocation:(LocationContext*)lc;
-(NSString*) messageForName:(NSString*)name;
-(NSString*) linkForLocation:(LocationContext*)lc;
-(NSString*) linkForLocID:(int)locid;

-(NSDictionary*) postParamForLocation:(LocationContext*)lc andAction:(NSString*)action;
-(NSDictionary*) postParamForName:(NSString*)name Location:(int)locID andAction:(NSString*)action;
-(void) requestPushToFacebookFeed:(NSDictionary*)postParams;
-(void) pushToFacebookFeed:(NSDictionary*)postParams;
-(void) creditSocialPost;

@end
