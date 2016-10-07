//
//  UTCAppDelegate.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "AFNetworkActivityIndicatorManager.h"
#import <FacebookSDK/FacebookSDK.h>

NSString* const FBSessionStateChangedNotification = @"com.unitethiscity.unitethiscity:FBSessionStateChangedNotificaiton";

@implementation UTCAppDelegate


- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    // force creation of the application object singleton
    [UTCApp sharedInstance];
    
    // configure our caching for use by AF
    NSURLCache *URLCache = [[NSURLCache alloc] initWithMemoryCapacity:kUTCInMemoryCacheSpace diskCapacity:kUTCOnDiskCacheSpace diskPath:nil];
    [NSURLCache setSharedURLCache:URLCache];

    // force creation of the AFNetork activity manager
    [[AFNetworkActivityIndicatorManager sharedManager] setEnabled:YES];
    
    // prepare Google Analytics
    [[GAI sharedInstance] setTrackUncaughtExceptions:kGAITrackExceptions];
    [[GAI sharedInstance] setDispatchInterval:kGAIDispatchInterval];
    [[GAI sharedInstance] trackerWithTrackingId:kGAITrackingID];
    
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    // Override point for customization after application launch.
    self.viewController = [[UTCRootViewController alloc] initWithNibName:@"UTCRootViewController" bundle:nil];
    self.window.rootViewController = self.viewController;
    [self.window makeKeyAndVisible];
    [self.viewController setFirstDisplay:YES];
    
    // configure notifications based on the IOS version in use - IOS8 requires different settings than <= IOS7
    if ([application respondsToSelector:@selector(registerUserNotificationSettings:)]) {
        // use registerUserNotificationSettings
        [application registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:(UIUserNotificationTypeSound | UIUserNotificationTypeAlert | UIUserNotificationTypeBadge) categories:nil]];
        [application registerForRemoteNotifications];
    } else {
        // use registerForRemoteNotificationTypes:
        [[UIApplication sharedApplication] registerForRemoteNotificationTypes:(UIRemoteNotificationTypeSound)|(UIRemoteNotificationTypeAlert)|(UIRemoteNotificationTypeBadge)];
    }
    
    
    NSLog(@"Device screen size %g x %g %@", [UTCSettings getScreenWidth], [UTCSettings getScreenHeight], ([UTCSettings isScreenTall]) ? @"IPhone5" : @"NotIPhone5");
    
    // clear the facebook id
    _facebookId = @"";
    
    // restore the facebook session
    if (FBSession.activeSession.state == FBSessionStateCreatedTokenLoaded)
    {
        // Yes, so just open the session (this won't display any UX).
        [FBSession openActiveSessionWithReadPermissions:[[NSArray alloc] initWithObjects:@"email", nil]
                                           allowLoginUI:YES
                                      completionHandler:
         ^(FBSession *session,
           FBSessionState state, NSError *error) {
             [self sessionStateChanged:session state:state error:error];
         }];
    }
    else
    {
        NSLog(@"Unable to restore facebook session");
        [[UTCApp sharedInstance] setFacebookStatus:kFacebookOff];
    }
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later. 
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
    
    // We need to properly handle activation of the app with regards to Facebook Login
    // (e.g., returning from iOS 6.0 Login Dialog or from fast app switching).
    [FBSession.activeSession handleDidBecomeActive];
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
    // Don't close the facebook session - this disconnects it from Facebook and requires reconnection to restore the session
    // Version 1.8.0
    //[FBSession.activeSession close];
}

#pragma mark Notifications

// callback when the registration for remote notifications completes
-(void) application:(UIApplication *)app didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    NSLog(@"Device token is %@, was %@", deviceToken, [[UTCApp sharedInstance] devicePushToken]);
    NSString* deviceTokenString = [NSString stringWithFormat:@"%@",deviceToken];
    [[UTCApp sharedInstance] setDevicePushToken:deviceTokenString];
    // if we are signed in (automatically), post the push token with an enable
    if ([[[UTCApp sharedInstance] account] isSignedIn])
    {
        [[UTCApp sharedInstance] postPushToken:YES];
    }
}

// callback when the registration for remote notifications fails
-(void) application:(UIApplication *)app didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
    NSString* str = [NSString stringWithFormat:@"Error: %@", error];
    NSLog(@"Did Fail to register with error: %@ = %@", error, str);
}

// callback for when a remote notification is received while we are in the application
-(void) application:(UIApplication *)app didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    NSLog(@"Received notification %@", userInfo);
}

#pragma mark CLLocationManagerDelegate

-(void)locationManager:(CLLocationManager *)manager didUpdateToLocation:(CLLocation *)newLocation fromLocation:(CLLocation *)oldLocation
{
    [[UTCApp sharedInstance] updateMemberCoordinates:newLocation];
}

-(void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    if (error.code != 0) {
        NSLog(@"Location error %ld = %@", (long)error.code, error.description);
    }
}

#pragma mark FacebookSDK

/*
 * Callback for session changes.
 */
- (void)sessionStateChanged:(FBSession *)session
                      state:(FBSessionState) state
                      error:(NSError *)error
{
    switch (state) {
        case FBSessionStateOpen:
            if (!error) {
                // We have a valid session
                NSLog(@"User session found");
                [[UTCApp sharedInstance] setFacebookStatus:kFacebookOn];
            }
            else {
                NSLog(@"User session error %@", error);
                [[UTCApp sharedInstance] setFacebookStatus:kFacebookUnknown];
            }
            break;
        case FBSessionStateClosed:
        case FBSessionStateClosedLoginFailed:
            [FBSession.activeSession closeAndClearTokenInformation];
            NSLog(@"User session NOT found");
            [[UTCApp sharedInstance] setFacebookStatus:kFacebookOff];
            break;
            
        default:
            NSLog(@"User session WTF");
            [[UTCApp sharedInstance] setFacebookStatus:kFacebookUnknown];
            break;
    }
    
    [[NSNotificationCenter defaultCenter]
     postNotificationName:FBSessionStateChangedNotification
     object:session];
    
    if (error) {
        UIAlertView *alertView = [[UIAlertView alloc]
                                  initWithTitle:@"Error"
                                  message:error.localizedDescription
                                  delegate:nil
                                  cancelButtonTitle:@"OK"
                                  otherButtonTitles:nil];
        [alertView show];
    }
    else if (session.isOpen) {
        // attempt to get the account info
        [[FBRequest requestForMe] startWithCompletionHandler:
         ^(FBRequestConnection *connection,
           NSDictionary<FBGraphUser> *user,
           NSError *error) {
             if (!error) {
                 NSLog(@"USER = %@", user);
                 _facebookId = user.objectID;
                 NSLog(@"Identified Facebook user identifier %@", _facebookId);
                 [self.viewController loadAccountImage];
             }
         }];
    }
}

/*
 * Opens a Facebook session and optionally shows the login UX.
 */
- (BOOL)openFacebookSessionWithAllowLoginUI:(BOOL)allowLoginUI {
    return [FBSession openActiveSessionWithReadPermissions:[[NSArray alloc] initWithObjects:@"email", nil]
                                              allowLoginUI:allowLoginUI
                                         completionHandler:^(FBSession *session,
                                                             FBSessionState state,
                                                             NSError *error) {
                                             [self sessionStateChanged:session
                                                                 state:state
                                                                 error:error];
                                         }];
}

/*
 * If we have a valid session at the time of openURL call, we handle
 * Facebook transitions by passing the url argument to handleOpenURL
 */
- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
  sourceApplication:(NSString *)sourceApplication
         annotation:(id)annotation {
    // attempt to extract a token from the url
    return [FBSession.activeSession handleOpenURL:url];
}

-(void) closeFacebookSession
{
    [FBSession.activeSession closeAndClearTokenInformation];
    _facebookId = @"";
}

@end
