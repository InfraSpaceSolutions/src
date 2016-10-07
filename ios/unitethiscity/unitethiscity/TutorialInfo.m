//
//  TutorialInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 1/24/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//


#import "UTCApp.h"
#import "TutorialInfo.h"

@implementation TutorialInfo

@synthesize hasDisplayed;

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
    NSLog(@"TutorialInfo: LoadDefaults");
    hasDisplayed = NO;
}

// load from a dictionary of attributes
-(void) loadAttributes:(NSDictionary*)attributes
{
    NSLog(@"TutorialInfo: LoadAttributes");
    hasDisplayed = [[attributes valueForKeyPath:@"HasDisplayed"] boolValue];
}

// make a dictionary based on the current properties
-(NSDictionary*) makeAttributes
{
    NSMutableDictionary* dic = [[NSMutableDictionary alloc] init];
    [dic setObject:[NSNumber numberWithInt:kTutorialInfoVersion] forKey:@"Version"];
    [dic setObject:[NSNumber numberWithBool:hasDisplayed] forKey:@"HasDisplayed"];
    return dic;
}

// store the tutorial information to disk
-(void) writeToDisk
{
    NSLog(@"TutorialInfo: WriteToDisk");
    NSDictionary* attr = [self makeAttributes];
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kTutorialInfoFilename];
    [attr writeToFile:filePath atomically:YES];
}

// load the tutorial info as a dictionary from disk
-(BOOL) loadFromDisk
{
    NSLog(@"TutorialInfo: LoadFromDisk");
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kTutorialInfoFilename];
    NSDictionary *attrFromFile = [NSDictionary dictionaryWithContentsOfFile:filePath];
    
    if ([[attrFromFile valueForKey:@"Version"] intValue] == kTutorialInfoVersion)
    {
        [self loadAttributes:attrFromFile];
        return YES;
    }
    return NO;
}


@end

