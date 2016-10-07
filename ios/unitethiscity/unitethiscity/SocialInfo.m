//
//  SocialInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 1/24/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//


#import "UTCApp.h"
#import "SocialInfo.h"

@implementation SocialInfo

@synthesize connectFacebook;
@synthesize connectTwitter;

// create an instance with default values
-(id) init
{
    if (!(self = [super init]))
    {
        return nil;
    }
    
    [self loadDefaults];
    
    return self;
}

// create an instance from a dictionary of attributes (key/value pairs)
-(id) initWithAttributes:(NSDictionary *)attributes
{
    if (!(self = [super init]))
    {
        return nil;
    }
    
    [self loadAttributes:attributes];
    
    return self;
}


// load default values
-(void) loadDefaults
{
    connectFacebook = NO;
    connectTwitter = NO;
    
}

// load from a dictionary of attributes
-(void) loadAttributes:(NSDictionary*)attributes
{
    connectFacebook = [[attributes valueForKeyPath:@"ConnectFacebook"] boolValue];
    connectTwitter = [[attributes valueForKeyPath:@"ConnectTwitter"] boolValue];
}

// make a dictionary based on the current properties
-(NSDictionary*) makeAttributes
{
    NSMutableDictionary* dic = [[NSMutableDictionary alloc] init];
    [dic setObject:[NSNumber numberWithInt:kSocialInfoVersion] forKey:@"Version"];
    [dic setObject:[NSNumber numberWithBool:connectFacebook] forKey:@"ConnectFacebook"];
    [dic setObject:[NSNumber numberWithBool:connectTwitter] forKey:@"ConnectTwitter"];
    return dic;
}

// store the information to disk
-(void) writeToDisk
{
    NSDictionary* attr = [self makeAttributes];
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kSocialInfoFilename];
    [attr writeToFile:filePath atomically:YES];
}

// load the info as a dictionary from disk
-(BOOL) loadFromDisk
{
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kSocialInfoFilename];
    NSDictionary *attrFromFile = [NSDictionary dictionaryWithContentsOfFile:filePath];
    
    if ([[attrFromFile valueForKey:@"Version"] intValue] == kSocialInfoVersion)
    {
        [self loadAttributes:attrFromFile];
        return YES;
    }
    return NO;
}


@end

