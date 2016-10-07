//
//  AccountInfo.m
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "AccountInfo.h"
#import "LocationInfo.h"
#import "LocationContext.h"
#import "Guid.h"
#import <CommonCrypto/CommonDigest.h>

@implementation AccountInfo

@synthesize accID = _accID;
@synthesize citID = _citID;
@synthesize accGuid = _accGuid;
@synthesize apiToken = _apiToken;
@synthesize email = _email;
@synthesize firstName = _firstName;
@synthesize lastName = _lastName;
@synthesize isAdmin = _isAdmin;
@synthesize isSalesRep = _isSalesRep;
@synthesize isMember = _isMember;
@synthesize businessRoles = _businessRoles;
@synthesize charityRoles = _charityRoles;
@synthesize associateRoles = _associateRoles;

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

// create an instance with default values
-(id) initWithTestMember
{
    if (!(self = [super init]))
    {
        return nil;
    }
    _accID = 1;
    _citID = kDefaultCitID;
    _accGuid = @"b8c8e11a-52df-4249-a88d-732ee8bfa072";
    _apiToken = @"20fab834-b421-48c1-9e6b-0b97630b8844";
    _email = @"mterry@sancsoft.com";
    _firstName = @"Michael";
    _lastName = @"Terry";
    _isAdmin = NO;
    _isSalesRep = NO;
    _isMember = YES;
    _businessRoles = [[NSArray alloc] init];
    _charityRoles = [[NSArray alloc] init];
    _associateRoles = [[NSArray alloc] init];
    return self;
}

// checks to see if the user is currently signed in as any role
-(BOOL) isSignedIn
{
    return (_accID != 0);
}

// calculate the md5 hash of a string
+(NSString *)md5:(NSString *)str
{
    const char *cStr = [str UTF8String];
    unsigned char result[16];
    CC_MD5( cStr, (int)strlen(cStr), result );
    return [NSString stringWithFormat:
            @"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
            result[0], result[1], result[2], result[3],
            result[4], result[5], result[6], result[7],
            result[8], result[9], result[10], result[11],
            result[12], result[13], result[14], result[15]
            ];
}

// generate the checkin query string
-(NSString*) memberIdentifierQueryString
{
    NSString* raw = [[NSString stringWithFormat:@"%@-%@", _accGuid, kMemberHashKey] lowercaseString];
    NSString* hash = [AccountInfo md5:raw];
    NSString* qs = [NSString stringWithFormat:@"a=%d&h=%@",_accID, hash];
    return qs;
}

// generate the QURI (QR code - URI) for identifying the member
-(NSString*) memberIdentifierQURI
{
    return [NSString stringWithFormat:@"%@?%@", kMemberIdentifierQURL, [self memberIdentifierQueryString]];
}

// generate the location context identifier
-(NSString*) businessIdentifierQueryString
{
    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    
    NSString* raw = [[NSString stringWithFormat:@"%@-%@", loc.busGuid, kBusinessHashKey] lowercaseString];
    NSString* hash = [AccountInfo md5:raw];
    NSString* qs = [[NSString stringWithFormat:@"b=%d&h=%@",loc.busID, hash] lowercaseString];
    return qs;
}

// generate the QURI (QR code - URI) for identifying the member
-(NSString*) businessIdentifierQURI
{
    return [NSString stringWithFormat:@"%@?%@", kBusinessIdentifierQURL, [self businessIdentifierQueryString]];
}

// generate the encrypted version of the password in MD5
+(NSString*) passwordEncryption:(NSString *)password
{
    NSString* raw = [NSString stringWithFormat:@"%@-%@", password, kAccountHashKey];
    NSString* hash = [AccountInfo md5:raw];
    return hash;
}

// load default values
-(void) loadDefaults
{
    NSLog(@"AccountInfo: LoadDefaults");
    _accID = 0;
    _citID = kDefaultCitID;
    _accGuid = [[Guid emptyGuid] stringValueWithFormat:GuidFormatDashed];
    _apiToken = [[Guid emptyGuid] stringValueWithFormat:GuidFormatDashed];
    _email = @"guest@unitethiscity.com";
    _firstName = @"Guest";
    _lastName = @"User";
    _isAdmin = NO;
    _isSalesRep = NO;
    _isMember = NO;
    _businessRoles = [[NSArray alloc] init];
    _charityRoles = [[NSArray alloc] init];
    _associateRoles = [[NSArray alloc] init];
}

// load from a dictionary of attributes
-(void) loadAttributes:(NSDictionary*)attributes
{
    NSLog(@"AccountInfo: LoadAttributes");
    _accID = (int)[[attributes valueForKeyPath:@"AccId"] integerValue];
    _citID = (int)[[attributes valueForKeyPath:@"CitID"] integerValue];
    _accGuid = [attributes valueForKeyPath:@"AccGuid"];
    _apiToken = [attributes valueForKeyPath:@"Token"];
    _email = [attributes valueForKeyPath:@"AccEMail"];
    _firstName = [attributes valueForKeyPath:@"AccFName"];
    _lastName = [attributes valueForKeyPath:@"AccLName"];
    _isAdmin = [[attributes valueForKeyPath:@"IsAdmin"] boolValue];
    _isSalesRep = [[attributes valueForKeyPath:@"IsSalesRep"] boolValue];
    _isMember = [[attributes valueForKeyPath:@"IsMember"] boolValue];
    _businessRoles = [[NSArray alloc] initWithArray:[attributes valueForKeyPath:@"BusinessRoles"]];
    _charityRoles = [[NSArray alloc] initWithArray:[attributes valueForKeyPath:@"CharityRoles"]];
    _associateRoles = [[NSArray alloc] initWithArray:[attributes valueForKeyPath:@"AssociateRoles"]];
}

// make a dictionary based on the current properties
-(NSDictionary*) makeAttributes
{
    NSMutableDictionary* dic = [[NSMutableDictionary alloc] init];
    [dic setObject:[NSNumber numberWithInt:kAccountInfoVersion] forKey:@"Version"];
    [dic setObject:[NSNumber numberWithInt:_accID] forKey:@"AccId"];
    [dic setObject:[NSNumber numberWithInt:_citID] forKey:@"CitId"];
    [dic setObject:_accGuid forKey:@"AccGuid"];
    [dic setObject:_apiToken forKey:@"Token"];
    [dic setObject:_email forKey:@"AccEMail"];
    [dic setObject:_firstName forKey:@"AccFName"];
    [dic setObject:_lastName forKey:@"AccLName"];
    [dic setObject:[NSNumber numberWithBool:_isAdmin] forKey:@"IsAdmin"];
    [dic setObject:[NSNumber numberWithBool:_isSalesRep] forKey:@"IsSalesRep"];
    [dic setObject:[NSNumber numberWithBool:_isMember] forKey:@"IsMember"];
    [dic setObject:_businessRoles forKey:@"BusinessRoles"];
    [dic setObject:_charityRoles forKey:@"CharityRoles"];
    [dic setObject:_associateRoles forKey:@"AssociateRoles"];

    return dic;
}

// store the account information to disk
-(void) writeToDisk
{
    NSLog(@"AccountInfo: WriteToDisk");
    NSDictionary* attr = [self makeAttributes];
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kAccountInfoFilename];
    [attr writeToFile:filePath atomically:YES];
}

// load the account info as a dictionary from disk
-(BOOL) loadFromDisk
{
    NSLog(@"AccountInfo: LoadFromDisk");
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:kAccountInfoFilename];
    NSDictionary *attrFromFile = [NSDictionary dictionaryWithContentsOfFile:filePath];

    // NSLog(@"Loaded account info = %@", attrFromFile);
    
    if ([[attrFromFile valueForKey:@"Version"] intValue] == kAccountInfoVersion)
    {
        [self loadAttributes:attrFromFile];
        return YES;
    }
    return NO;
}


@end

