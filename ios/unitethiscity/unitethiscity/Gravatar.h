//
//  Gravatar.h
//  unitethiscity
//
//  Created by Michael Terry on 3/28/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Gravatar : NSObject

+(NSString*) imageUrlForEmail:(NSString*)anEmail withSize:(int)dim withSSL:(BOOL)secure;

@end
