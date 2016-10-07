//
//  Guid.h
//  unitethiscity
//
//  Created by Michael Terry on 3/26/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum
{
    GuidFormatCompact = 0,
    GuidFormatIncludeDashes = 1,
    GuidFormatIncludeBraces = 2,
    GuidFormatIncludeParenthesis = 4,
    GuidFormatUpperCase = 8,
    GuidFormatDashed = GuidFormatIncludeDashes,
    GuidFormatBraces = GuidFormatIncludeDashes | GuidFormatIncludeBraces,
    GuidFormatParenthesis = GuidFormatIncludeDashes | GuidFormatIncludeParenthesis
} GuidFormat;

@interface Guid : NSObject<NSCoding>
{
@private
    UInt8 data[16];
}

-(id) initWithString:(NSString*)string;
-(id) initWithBytes:(UInt8[16])bytes;
-(NSString*) stringValue;
-(CFUUIDBytes) uuidBytes;
-(void) byteDataToBuffer:(UInt8*)bytes;
-(NSString*) stringValueWithFormat:(GuidFormat)format;
-(BOOL) isEmpty;
-(BOOL) isEqual:(id)object;
-(NSUInteger) hash;
-(NSString*) description;

+(Guid*) randomGuid;
+(Guid*) emptyGuid;
+(Guid*) guidFromString:(NSString*)guidString;

@end