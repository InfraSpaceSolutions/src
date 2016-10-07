//
//  Gravatar.m
//  unitethiscity
//
//  Created by Michael Terry on 3/28/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "Gravatar.h"
#import <CommonCrypto/CommonDigest.h>

#define HTTP_GRAVATAR_URL       @"http://www.gravatar.com"
#define HTTPS_GRAVATAR_URL      @"https://secure.gravatar.com"
#define DEFAULT_GRAVATAR        @"&d=mm"

@interface Gravatar (private)
+(NSString *)md5:(NSString *)str;
+(NSString *)calculateGravatarId:(NSString *)anEmail;
@end

@implementation Gravatar

// calculate the md5 hash of a string
+(NSString *)md5:(NSString *)str
{
    const char *cStr = [str UTF8String];
    unsigned char result[16];
    CC_MD5( cStr, (int)strlen(cStr), result );
    return [NSString stringWithFormat:
            @"%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X",
            result[0], result[1], result[2], result[3],
            result[4], result[5], result[6], result[7],
            result[8], result[9], result[10], result[11],
            result[12], result[13], result[14], result[15]
            ];
}

// create a gravatar hash from an email
+(NSString *)calculateGravatarId:(NSString *)anEmail
{
    return [[Gravatar md5:[[anEmail stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]] lowercaseString]] lowercaseString];
}

// get the url for a gravatar image
+(NSString*) imageUrlForEmail:(NSString*)anEmail withSize:(int)dim withSSL:(BOOL)secure
{
    return [NSString stringWithFormat:@"%@/avatar/%@.png?s=%d%@",
            (secure) ? HTTPS_GRAVATAR_URL : HTTP_GRAVATAR_URL,
            [Gravatar calculateGravatarId:anEmail], dim, DEFAULT_GRAVATAR];
}

@end
