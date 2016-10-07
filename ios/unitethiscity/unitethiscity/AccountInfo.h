//
//  AccountInfo.h
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface AccountInfo : NSObject

@property (readonly) int accID;
@property (readonly) int citID;
@property (readonly) NSString* accGuid;
@property (readonly) NSString* apiToken;
@property (readonly) NSString* email;
@property (readonly) NSString* firstName;
@property (readonly) NSString* lastName;
@property (readonly) BOOL isAdmin;
@property (readonly) BOOL isSalesRep;
@property (readonly) BOOL isMember;
@property (readonly) NSArray* businessRoles;
@property (readonly) NSArray* charityRoles;
@property (readonly) NSArray* associateRoles;

-(id) init;
-(id) initWithAttributes:(NSDictionary*) attributes;
-(id) initWithTestMember;

-(BOOL) isSignedIn;

+(NSString *)md5:(NSString *)str;
-(NSString*) memberIdentifierQueryString;
-(NSString*) memberIdentifierQURI;
-(NSString*) businessIdentifierQueryString;
-(NSString*) businessIdentifierQURI;
+(NSString*) passwordEncryption:(NSString*)password;

-(void) loadDefaults;
-(void) loadAttributes:(NSDictionary*)attributes;

-(BOOL) loadFromDisk;
-(void) writeToDisk;

@end
