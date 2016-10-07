//
//  UTCAPIClient.m
//  unitethiscity
//
//  Created by Michael Terry on 3/30/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "AccountInfo.h"
#import "UTCAPIClient.h"
#import "AFJSONRequestOperation.h"

@implementation UTCAPIClient

static NSString * const kUTCAPIClientBase = kUTCAPIBaseURLString;

+ (UTCAPIClient *)sharedClient
{
    static UTCAPIClient *_sharedClient = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedClient = [[UTCAPIClient alloc] initWithBaseURL:[NSURL URLWithString:kUTCAPIClientBase]];
    });
    
    return _sharedClient;
}

- (id)initWithBaseURL:(NSURL *)url
{
    self = [super initWithBaseURL:url];
    if (!self) {
        return nil;
    }
    
    [self registerHTTPOperationClass:[AFJSONRequestOperation class]];
    
    // Accept HTTP Header; see http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.1
	[self setDefaultHeader:@"Accept" value:@"application/json"];
    
    return self;
}

-(void) getVersionWithBlock:(void (^)(NSDictionary* dic, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/Version", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAllLocationsWithBlock:Error %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getDataRevisionWithBlock:(int)drvID withBlock:(void (^)(NSDictionary *, NSError *))block
{
    NSString* getpath = [NSString stringWithFormat:@"datarevision/%d", drvID];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getDataRevisionWithBlock %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}
-(void) getAllLocationsWithBlock:(void (^)(NSArray *locations, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/locationsummary", [[[UTCApp sharedInstance] account] apiToken]];
    
    //[[UTCAPIClient sharedClient] getPath:@"locationinfo" parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAllLocationsWithBlock:Error %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getCategoriesWithBlock:(void (^)(NSArray *dic, NSError* error))block {
    [[UTCAPIClient sharedClient] getPath:@"category" parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getCategoriesWithBlock %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getAllEventsWithBlock:(void (^)(NSArray *events, NSError* error))block {
    [[UTCAPIClient sharedClient] getPath:@"eventinfo" parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getEventsWithBlock %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getWalletWithBlock:(void (^)(NSDictionary* dictWallet, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/Wallet", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAllLocationsWithBlock:Error %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getLocationContextFor:(int)locID withBlock:(void (^)(NSDictionary *, NSError *))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/LocationContext/%d", [[[UTCApp sharedInstance] account] apiToken], locID];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAllLocationsWithBlock:%d Error %@", locID, error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getEventContextFor:(int)evtID withBlock:(void (^)(NSDictionary *, NSError *))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/EventContext/%d", [[[UTCApp sharedInstance] account] apiToken], evtID];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getEventContextFor:%d Error %@", evtID, error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getFavoritesWithBlock:(void (^)(NSArray*, NSError*))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/Favorite", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSArray*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getFavoritesWithBlock Error %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatSummaryFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/summarystats/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatSummaryFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatSummaryRedemptionsFor:(int)busID andScope:(int)scopeid withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/summaryredemptions/%d?scopeid=%d", [[[UTCApp sharedInstance] account] apiToken], busID, scopeid];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatSummaryRedemptionsFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatSummaryCheckInsFor:(int)busID andScope:(int)scopeid withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/summarycheckins/%d?scopeid=%d", [[[UTCApp sharedInstance] account] apiToken], busID, scopeid];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatSummaryCheckInsFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatRedemptionFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statredemption/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatRedemptionFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatRedemptionFor:(int)busID andUser:(int)accID andScope:(int)scopeID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statredemption/%d?scopeid=%d&accid=%d", [[[UTCApp sharedInstance] account] apiToken],
                         busID, scopeID, accID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatRedemptionFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}


-(void) getStatCheckInFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statcheckin/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatCheckInFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatCheckInFor:(int)busID andUser:(int)accID andScope:(int)scopeID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statcheckin/%d?scopeid=%d&accid=%d", [[[UTCApp sharedInstance] account] apiToken],
                         busID, scopeID, accID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatCheckInFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}


-(void) getStatFavoriteFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statfavorite/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatFavoriteFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatRatingFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/statrating/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatRatingFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}


-(void) getStatTipFor:(int)busID withBlock:(void (^)(NSArray *stats, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/stattip/%d", [[[UTCApp sharedInstance] account] apiToken], busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatTipFor %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getAnalyticsSummaryFor:(int)range withBlock:(void (^)(NSArray *, NSError *))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/summaryanalytics/%d", [[[UTCApp sharedInstance] account] apiToken], range];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAnalyticsSumamryFor` %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getAnalyticsSummaryFor:(int)range andBusiness:(int)bus withBlock:(void (^)(NSArray *, NSError *))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/summaryanalytics/%d?busid=%d", [[[UTCApp sharedInstance] account] apiToken], range, bus];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAnalyticsSumamryForBusiness` %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getStatPermissionsWithBlock:(void (^)(NSDictionary* dic, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/StatPermissions", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getStatPermissionsWithBlock %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getGalleryItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"gallery/%d", busID];

    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getGalleryItems %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getMenuItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"menu/%d", busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getMenuItems %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getCalendarItemsFor:(int)busID withBlock:(void (^)(NSArray *items, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"calendar/%d", busID];
    
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getCalendarItems %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}



-(void) postFavoriteWithBlock:(int)locID withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/Favorite/%d", [[[UTCApp sharedInstance] account] apiToken], locID];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postFavoriteWithBlock:%d Error %@", locID, error.localizedDescription);
            block(error);
        }
    }];
}

-(void) deleteFavoriteWithBlock:(int)locID withBlock:(void (^)(NSError *))block {
    NSString* path = [NSString stringWithFormat:@"%@/Favorite/%d", [[[UTCApp sharedInstance] account] apiToken], locID];
    [[UTCAPIClient sharedClient] deletePath:path parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"deleteFavoritesWithBlock:%d Error %@", locID, error.localizedDescription);
            block(error);
        }
    }];
}

-(void) getAllInboxMessagesWithBlock:(void (^)(NSArray *locations, NSError* error))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/inboxmessage/", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getAllInboxMessagesWithBlock %@", error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) getInboxMessageWithBlock:(int)inbID withBlock:(void (^)(NSDictionary *, NSError *))block {
    NSString* getpath = [NSString stringWithFormat:@"%@/inboxmessage/%d", [[[UTCApp sharedInstance] account] apiToken], inbID];
    [[UTCAPIClient sharedClient] getPath:getpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block((NSDictionary*)JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"getInboxMessageWithBlock:%d Error %@", inbID, error.localizedDescription);
            block(nil,error);
        }
    }];
}

-(void) postLogin:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    [[UTCAPIClient sharedClient] postPath:@"Login" parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postLogin %@", error);
            block(nil, error);
        }
    }];
}

-(void) postFacebookLogin:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    [[UTCAPIClient sharedClient] postPath:@"FacebookLogin" parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postFacebookLogin %@", error);
            block(nil, error);
        }
    }];
}


-(void) postRedeem:(NSDictionary*)param withBlock:(void (^)( NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/Redeem", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postRedeem:Error %@", error);
            block(error);
        }
    }];
}

-(void) postCheckIn:(NSDictionary*)param withBlock:(void (^)( NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/CheckIn", [[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postCheckin:Error %@", error);
            block(error);
        }
    }];
}

-(void) postRating:(int)rating forLocation:(int)loc withBlock:(void (^)(NSError* error))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/Rating/%d?rating=%d",[[[UTCApp sharedInstance] account] apiToken],loc,rating];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postRatingError %@", error);
            block(error);
        }
    }];
}

-(void) postTip:(NSDictionary*)param withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/Tip",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postTipError %@", error);
            block(error);
        }
    }];
}

-(void) postUnifiedAction:(NSDictionary *)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/UnifiedAction",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postUnifiedActionError %@", error);
            block(nil, error);
        }
    }];
}

-(void) postProximityAction:(NSDictionary *)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/ProximityAction",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            block(nil, error);
        }
    }];
}

-(void) postPushToken:(NSDictionary *)param withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/PushToken",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postPushTokenError %@", error);
            block(error);
        }
    }];
}

-(void) postSocialPost:(NSDictionary *)param withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/SocialPost",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postSocialPostError %@", error);
            block(error);
        }
    }];
}


-(void) postSubscription:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    [[UTCAPIClient sharedClient] postPath:@"Subscription" parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postSubscriptionError %@", error);
            block(nil, error);
        }
    }];
}

-(void) postFreeSignUp:(NSDictionary*)param withBlock:(void (^)(NSDictionary *, NSError *))block {
    [[UTCAPIClient sharedClient] postPath:@"FreeSignUp3" parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(JSON,nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postFreeSignUpError %@", error);
            block(nil, error);
        }
    }];
}

-(void) postProximityUpdate:(NSDictionary *)param withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/Proximity",[[[UTCApp sharedInstance] account] apiToken]];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:param success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postProximityError %@", error);
            block(error);
        }
    }];
}


// get best match error from an error response
+(NSString*) getMessageFromError:(NSError *)error
{
    NSString* msg = @"Internal application error 1066";
    
    if (error)
    {
        // default to the localized error description
        msg = [error localizedDescription];
        // try to extract a better message from the response
        NSData* jsonData = [[error localizedRecoverySuggestion] dataUsingEncoding:NSUTF8StringEncoding];
        NSError* jsonError;
        if (jsonData)
        {
            NSDictionary* errDict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&jsonError];
            if (errDict) {
                msg = [errDict objectForKey:@"Message"];
            }
        }
    }
    return msg;
}

-(void) postInboxReadWithBlock:(int)inbID withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/InboxRead/%d",[[[UTCApp sharedInstance] account] apiToken],inbID];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postInboxRead Error %@", error);
            block(error);
        }
    }];
}

-(void) postInboxDeleteWithBlock:(int)inbID withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/InboxDelete/%d",[[[UTCApp sharedInstance] account] apiToken],inbID];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postInboxDelete Error %@", error);
            block(error);
        }
    }];
}

-(void) postInboxOptOutWithBlock:(int)inbID withBlock:(void (^)(NSError *))block {
    NSString* postpath = [NSString stringWithFormat:@"%@/OptOut/%d",[[[UTCApp sharedInstance] account] apiToken],inbID];
    [[UTCAPIClient sharedClient] postPath:postpath parameters:nil success:^(AFHTTPRequestOperation *operation, id JSON) {
        if (block) {
            block(nil);
        }
    } failure:^(AFHTTPRequestOperation* operation, NSError* error) {
        if (block) {
            NSLog(@"postInboxOptOut Error %@", error);
            block(error);
        }
    }];
}

@end
