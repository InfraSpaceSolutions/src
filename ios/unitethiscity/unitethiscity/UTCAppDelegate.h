//
//  UTCAppDelegate.h
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import <UIKit/UIKit.h>

extern NSString* const FBSessionStateChangedNotification;

@class UTCRootViewController;

@interface UTCAppDelegate : UIResponder <UIApplicationDelegate,CLLocationManagerDelegate>

- (BOOL)openFacebookSessionWithAllowLoginUI:(BOOL)allowLoginUI;
- (void)closeFacebookSession;

@property (strong, nonatomic) NSString* facebookId;

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) UTCRootViewController *viewController;

@end
