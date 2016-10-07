//
//  UTCSettings.h
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

// debug feature flags
#define DEBUG_ACCOUNTINFO_INITWITHTEST

// account information persistence settings
#define kAccountInfoVersion         1
#define kAccountInfoFilename        @"AccountInfo.dat"

// anti-filter persistence settings
#define kAntiFiltersFilename        @"AntiFilters.dat"

// proximity log persistence settings
#define kProximityLogFilename       @"ProximityLog.txt"
// #define PROXIMITY_LOCAL_LOGGING

// tutorial info persistence
#define kTutorialInfoVersion        1
#define kTutorialInfoFilename       @"TutorialInfo.dat"

// social info persistence
#define kSocialInfoVersion          1
#define kSocialInfoFilename         @"SocialInfo.dat"

// api version the app is designed to work with; if the major versions don't match, the application
// will throw an error on start up
#define kAPIVersionMajor            1
#define kAPIVersionMinor            15
#define kAPIVersionPatch            0

// hash keys
#define kAccountHashKey             @"063a64a2-92ed-4953-bb27-dde61238ca5e"
#define kMemberHashKey              @"ef0cd82a-8be1-4208-b6ae-0eaf97e7e14b"
#define kBusinessHashKey            @"4dc01879-29b6-4212-9654-fe4e6069bc2d"

// QURL - QR Code base urls
#define kMemberIdentifierQURL       @"https://www.unitethiscity.com/qr/m/"
#define kBusinessIdentifierQURL     @"https://www.unitethiscity.com/qr/b/"

// link to join UTC
#define kMemberJoinURL              @"https://www.unitethiscity.com/app/join"
#define kMemberReadMoreURL          @"https://www.unitethiscity.com/app/more"

// terms and conditions
#define kSocialDealTermsURL         @"https://www.unitethiscity.com/app/terms/embedded2?l=%d"
#define kLoyaltyDealTermsURL        @"https://www.unitethiscity.com/app/terms/embeddedLoyalty2?l=%d"
#define kGeneralTermsURL            @"https://www.unitethiscity.com/TermsAndConditions"

// member tips
#define kMemberTipsURL              @"https://www.unitethiscity.com/app/tips/?loc=%d"

// link to view a message
#define kMemberViewMessageURL       @"https://www.unitethiscity.com/app/message/default2?inb=%d&tok=%@"

// link to forgot password interface
#define kForgotPasswordURL          @"https://www.unitethiscity.com/mobileresetpassword"

// link to social media pages
#define kTwitterURL                 @"http://twitter.com/UniteThisCity"
#define kFacebookURL                @"http://www.facebook.com/UniteThisCity"

// social platform identifiers
#define kSptFacebookA               1
#define kSptFacebook                2
#define kSptTwitter                 3

// define the best guess at facebook status
#define kFacebookUnknown            0
#define kFacebookOn                 1
#define kFacebookOff                2

// define the properties of the facebook reminder
#define kReminderPeriod             10
#define kKeyReminderCount           @"UTCReminderCounter"

// define the supported scan actions - intentions of qr code scans
#define kScanActionNone             0
#define kScanActionMemberCheckin    1
#define kScanActionMemberRedeem     2
#define kScanActionBusinessCheckin  3
#define kScanActionBusinessRedeem   4

// stat ranges
#define kRangeToday         0
#define kRangePastWeek      1
#define kRangeThisMonth     2
#define kRangeLastMonth     3
#define kRangeAllTime       4

// default social channel selection
#define kDefaultSocialUndefined     0
#define kDefaultSocialNone          1
#define kDefaultSocialFacebook      2
#define kDefaultSocialTwitter       3
#define kDefaultSocialInstagram     4

// configuration of the location manager
#define kLocationAccuracy       kCLLocationAccuracyHundredMeters
#define kLocationFilter         100.0

// the default location for the member is downtown cleveland
#define kLocationDefLat         41.499495
#define kLocationDefLong        -81.695409

// webapi configuration and endpoint settings
#define kUTCAPIBaseURLString    @"https://api.unitethiscity.com/api";
//#define kUTCAPIBaseURLString    @"http://utcapi.dev.sancsoft.net/api";

// the base url where business images are available to the software
#define kUTCBusinessImageURL    @"https://www.unitethiscity.com/businessimages/"
#define kUTCGalleryImageURL     @"https://www.unitethiscity.com/businessgallery/"

// caching - for use by AFNetworking
#define kUTCInMemoryCacheSpace  (4 * 1024 * 1024)
#define kUTCOnDiskCacheSpace    (20 * 1024 * 1024)

// Scandit SDK
#define kScanditSDKAppKey       @"rFXz/JDtEeKUByE5g8ULslvQvx3H+dRLcw6qXT6y79I"

// Google App Analytics
#define kGAITrackingID          @"UA-37748134-2"
#define kGAIEnabled             YES
#define kGAITrackExceptions     NO
#define kGAIDebug               NO
#define kGAIDispatchInterval    20


// Location distance parameters
#define kMetersToMiles          0.000621371192
#define kTooFarLimit            3000.0

// everybody is in Cleveland in the current release
#define kDefaultCitID           1

// define the device type for push notifications
#define kPushDeviceTypeID       1

// default color - used to mark non-favs, etc. in lists
#define kDefaultColorR          178 //230
#define kDefaultColorG          178 //230
#define kDefaultColorB          178 //230
#define kDefaultColorA          255


// favorite color - used to mark favorites in lists
#define kFavoriteColorR         255
#define kFavoriteColorG         255
#define kFavoriteColorB         255
#define kFavoriteColorA         255

// general list alternate color A
#define kListAColorR            0xff
#define kListAColorG            0xff
#define kListAColorB            0xff
#define kListAColorA            0xff

// general list alternate color B
#define kListBColorR            0xb2
#define kListBColorG            0xb2
#define kListBColorB            0xb2
#define kListBColorA            0xff

// defines the size of the avatar graphic
#define kAvatarSize             40

// actions that can be denied for non-members
#define kGuestDeniedDefault         0
#define kGuestDeniedCheckIn         1
#define kGuestDeniedRedeem          2
#define kGuestDeniedFavorite        3
#define kGuestDeniedRate            4
#define kGuestDeniedTip             5
#define kGuestDeniedInbox           6

// roles for action in the system and accounts
#define kRoleUndefined              0
#define kRoleAdministrator          1
#define kRoleMember                 2
#define kRoleBusiness               3
#define kRoleCharity                4
#define kRoleSalesRep               5
#define kRoleAssociate              6

@interface UTCSettings : NSObject

+(CGSize) getScreenSize;
+(float) getScreenHeight;
+(float) getScreenWidth;
+(BOOL) isScreenTall;
+(UIColor*) defaultBackColor;
+(UIColor*) favoriteBackColor;
+(UIColor*) msgBackColorA;
+(UIColor*) msgBackColorB;
+(UIColor*) evtBackColorA;
+(UIColor*) evtBackColorB;
+(UIColor*) statBackColorA;
+(UIColor*) statBackColorB;


+(BOOL) isFacebookAvailable;
+(BOOL) isTwitterAvailable;

@end

