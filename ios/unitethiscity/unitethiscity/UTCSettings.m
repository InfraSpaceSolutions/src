//
//  UTCSettings.m
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCSettings.h"

@implementation UTCSettings

// get the size of the device screen
+(CGSize) getScreenSize
{
    return [[UIScreen mainScreen] bounds].size;
}

// get the size of the device screen
+(float) getScreenHeight
{
    return [self getScreenSize].height;
}

// get the size of the device screen
+(float) getScreenWidth
{
    return [self getScreenSize].width;
}

// see if the screen is tall (IPhone 5)
+(BOOL) isScreenTall
{
    float h = [self getScreenHeight];
    NSAssert( (h == 480) || (h == 568), @"Unexpected screen height %g",h );
    return ( h == 568 );
}

// get the color that is used to flag favorites
+(UIColor*) defaultBackColor
{
    return [UIColor colorWithRed:kDefaultColorR/255.0 green:kDefaultColorG/255.0 blue:kDefaultColorB/255.0 alpha:kDefaultColorA/255.0];
}


// get the color that is used to flag favorites
+(UIColor*) favoriteBackColor
{
    return [UIColor colorWithRed:kFavoriteColorR/255.0 green:kFavoriteColorG/255.0 blue:kFavoriteColorB/255.0 alpha:kFavoriteColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) evtBackColorA
{
    return [UIColor colorWithRed:kListAColorR/255.0 green:kListAColorG/255.0 blue:kListAColorB/255.0 alpha:kListAColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) evtBackColorB
{
    return [UIColor colorWithRed:kListBColorR/255.0 green:kListBColorG/255.0 blue:kListBColorB/255.0 alpha:kListBColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) msgBackColorA
{
    return [UIColor colorWithRed:kListAColorR/255.0 green:kListAColorG/255.0 blue:kListAColorB/255.0 alpha:kListAColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) msgBackColorB
{
    return [UIColor colorWithRed:kListBColorR/255.0 green:kListBColorG/255.0 blue:kListBColorB/255.0 alpha:kListBColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) statBackColorA
{
    return [UIColor colorWithRed:kListAColorR/255.0 green:kListAColorG/255.0 blue:kListAColorB/255.0 alpha:kListAColorA/255.0];
}

// get the color that is used for alternating messages
+(UIColor*) statBackColorB
{
    return [UIColor colorWithRed:220/255.0 green:220/255.0 blue:220/255.0 alpha:255/255.0];
}



// see if built in facebook posting through IOS is available
+(BOOL) isFacebookAvailable
{
    // check to see if we have the social framework
    Class sfClass = NSClassFromString(@"SLComposeViewController");
    return (sfClass != nil);
}

// see if built in twitter posting through IOS5/IOS6 is available
+(BOOL) isTwitterAvailable
{
    // check to see if we have the social framework
    Class sfClass = NSClassFromString(@"SLComposeViewController");
    if ( sfClass != nil)
    {
        return YES;
    }
    
    // lets try falling back to the tweet sheet from IOS5
    Class twClass = NSClassFromString(@"TWTweetComposeViewController");
    return (twClass != nil);
}

@end
