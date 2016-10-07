//
//  UTCAPIClient.h
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "AFHTTPClient.h"

@interface UTCAPIClient : AFHTTPClient

+(UTCAPIClient*) sharedClient;

-(void) getVersionWithBlock:(void (^)(NSDictionary* dic, NSError* error))block;
-(void) getAllLocationsWithBlock:(void (^)(NSArray *locations, NSError* error))block;
-(void) getCategoriesWithBlock:(void (^)(NSArray *dic, NSError* error))block;
-(void) getAllEventsWithBlock:(void (^)(NSArray* events, NSError* error))block;
-(void) getWalletWithBlock:(void (^)(NSDictionary* dictWallet, NSError* error))block;
-(void) getLocationContextFor:(int)locID withBlock:(void (^)(NSDictionary* locContext, NSError* error))block;
-(void) getEventContextFor:(int)evtID withBlock:(void (^)(NSDictionary *evtContext, NSError* error))block;
-(void) getFavoritesWithBlock:(void (^)(NSArray* attr, NSError* error))block;
-(void) getAllInboxMessagesWithBlock:(void (^)(NSArray *messages, NSError* error))block;
-(void) getInboxMessageWithBlock:(int)inbID withBlock:(void (^)(NSDictionary *, NSError *))block;
-(void) getDataRevisionWithBlock:(int)drvID withBlock:(void (^)(NSDictionary*, NSError*))block;
-(void) getStatSummaryFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatSummaryRedemptionsFor:(int)busID andScope:(int)scopeid withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatSummaryCheckInsFor:(int)busID andScope:(int)scopeid withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatRedemptionFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatRedemptionFor:(int)busID andUser:(int)accID andScope:(int)scopeID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatCheckInFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatCheckInFor:(int)busID andUser:(int)accID andScope:(int)scopeID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatFavoriteFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatRatingFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatTipFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getAnalyticsSummaryFor:(int)range withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getAnalyticsSummaryFor:(int)range andBusiness:(int)bus withBlock:(void (^)(NSArray *stats, NSError* error))block;
-(void) getStatPermissionsWithBlock:(void (^)(NSDictionary* dic, NSError* error))block;
-(void) getGalleryItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block;
-(void) getMenuItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block;
-(void) getCalendarItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block;
-(void) postFavoriteWithBlock:(int)locID withBlock:(void (^)(NSError* error))block;
-(void) deleteFavoriteWithBlock:(int)locID withBlock:(void (^)(NSError* error))block;
-(void) postLogin:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block;
-(void) postFacebookLogin:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block;
-(void) postRedeem:(NSDictionary*)param withBlock:(void (^)(NSError* error))block;
-(void) postCheckIn:(NSDictionary*)param withBlock:(void (^)(NSError* error))block;
-(void) postRating:(int)rating forLocation:(int)loc withBlock:(void (^)(NSError* error))block;
-(void) postTip:(NSDictionary*)tip withBlock:(void (^)(NSError* error))block;
-(void) postUnifiedAction:(NSDictionary*)param withBlock:(void (^)(NSDictionary*, NSError*)) block;
-(void) postProximityAction:(NSDictionary*)param withBlock:(void (^)(NSDictionary*, NSError*)) block;
-(void) postPushToken:(NSDictionary*)param withBlock:(void(^)(NSError* error))block;
-(void) postSocialPost:(NSDictionary*)param withBlock:(void(^)(NSError* error))block;
-(void) postSubscription:(NSDictionary*)param withBlock:(void(^)(NSDictionary *, NSError*))block;
-(void) postFreeSignUp:(NSDictionary*)param withBlock:(void(^)(NSDictionary *, NSError*))block;
-(void) postProximityUpdate:(NSDictionary*)param withBlock:(void(^)(NSError* error))block;
-(void) postInboxReadWithBlock:(int)inbID withBlock:(void (^)(NSError* error))block;
-(void) postInboxDeleteWithBlock:(int)inbID withBlock:(void (^)(NSError *))block;
-(void) postInboxOptOutWithBlock:(int)inbID withBlock:(void (^)(NSError *))block;
+(NSString*) getMessageFromError:(NSError*)err;

@end
