//
//  UTCSocial.m
//  unitethiscity
//
//  Created by Michael Terry on 5/21/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCSocial.h"
#import "UTCAPIClient.h"
#import "LocationContext.h"
#import "AccountInfo.h"
#import <FacebookSDK/FacebookSDK.h>

@implementation UTCSocial

+ (UTCSocial *)sharedInstance
{
    static UTCSocial *_sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[UTCSocial alloc] init];
    });
    
    return _sharedInstance;
}

- (id)init
{
    self = [super init];
    if (!self) {
        return nil;
    }
    return self;
}

//////
// Apple built in social framework
//////

// open a modal view to post to twitter or go to twitter site
-(void) openSLTwitter:(UIViewController*)vc withMessage:(NSString *)msg andLink:(NSString *)link
{
    // if we don't have built in support, just go to the page
    if (![UTCSettings isTwitterAvailable])
    {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:kTwitterURL]];
        return;
    }
    
    // do a social framework tweet or a tweet sheet, depending on what is available
    Class sfClass = NSClassFromString(@"SLComposeViewController");
    if (sfClass != nil)
    {
        SLComposeViewController* tweetPostVC = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
        [tweetPostVC setInitialText:msg];
        // use the provided link or default link if not provided
        [tweetPostVC addURL:(link != nil) ? [NSURL URLWithString:link] : [NSURL URLWithString:@"http://www.unitethiscity.com/"]];
        
        // set up the completion handler to credit the post
        tweetPostVC.completionHandler = ^(SLComposeViewControllerResult result) {
            // give benefit of the doubt - at least they tried
            if (result == SLComposeViewControllerResultDone)
            {
                [self creditSocialPost];
            }
        };
        [self setSptID:kSptTwitter];
        [vc presentViewController:tweetPostVC animated:YES completion:nil];
    }
}

// open a modal view to post to facebook or go to facebook site
-(void) openSLFacebook:(UIViewController*)vc withMessage:(NSString *)msg andLink:(NSString *)link
{
    NSLog(@"link = %@", link);
    
    NSMutableDictionary *params = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                   msg, @"name",
                                   @"www.unitethiscity.com", @"caption",
                                   @"Supporting local through Unite This City", @"description",
                                   link, @"link",
                                   nil];
    [FBWebDialogs presentFeedDialogModallyWithSession:nil
                                           parameters:params
                                              handler:
     ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
         if (error) {
             // Error launching the dialog or publishing a story.
             NSLog(@"Error publishing story.");
         } else {
             if (result == FBWebDialogResultDialogNotCompleted) {
                 // User clicked the "x" icon
                 NSLog(@"User canceled story publishing.");
             } else {
                 // Handle the publish feed callback
                 NSLog(@"Published to Facebook");
                 [self creditSocialPost];
             }
         }
     }];
}

-(void) openInstagram:(UIViewController*)vc withMessage:(NSString*)msg andLink:(NSString*)link
{
    NSURL *instagramURL = [NSURL URLWithString:@"instagram://camera"];
    if ([[UIApplication sharedApplication] canOpenURL:instagramURL]) {
        [[UIApplication sharedApplication] openURL:instagramURL];
    } else {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"No Instagram"
                                                        message:@"Instagram is not installed or access is not permitted."
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
    }
}

// generate the default message from a location context
-(NSString*) messageForLocation:(LocationContext*)lc
{
    NSString* msg = [NSString stringWithFormat:@"I just supported the local community through 'Unite This City' by visiting %@.", [lc name]];
    return msg;
}

// generate the default message from a location name
-(NSString*) messageForName:(NSString*)name
{
    NSString* msg = [NSString stringWithFormat:@"I just supported the local community through 'Unite This City' by visiting %@.", name];
    return msg;
}

// generate the default link from a location context
-(NSString*) linkForLocation:(LocationContext *)lc
{
    NSString* link = [NSString stringWithFormat:@"https://www.unitethiscity.com/directory/business/%d?nomap=1", [lc locID]];
    return link;
}

// generate the default link from a location context
-(NSString*) linkForLocID:(int)locid
{
    NSString* link = [NSString stringWithFormat:@"https://www.unitethiscity.com/directory/business/%d?nomap=1", locid];
    return link;
}


//////
// Facebook SDK and Graph API
//////

// create the post parameters for an action
-(NSDictionary*) postParamForLocation:(LocationContext*)lc andAction:(NSString*)action
{
    NSString* link = [NSString stringWithFormat:@"http://www.unitethiscity.com/directory/business/%d?nomap=1", [lc locID]];
    NSString* msg = [NSString stringWithFormat:@"I just supported the local community through 'Unite This City' by visiting %@.", [lc name]];
    NSMutableDictionary* postParams = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                       link, @"link",
                                       msg, @"message",
                                       nil];
    return postParams;
}

// create the post parameters for an action
-(NSDictionary*) postParamForName:(NSString*)name Location:(int)locID andAction:(NSString*)action
{
    NSString* link = [NSString stringWithFormat:@"http://www.unitethiscity.com/directory/business/%d/?nomap=1", locID];
    NSString* msg = [NSString stringWithFormat:@"I just supported the local community through 'Unite This City' by visiting %@.", name];
    NSMutableDictionary* postParams = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                       link, @"link",
                                       msg, @"message",
                                       nil];
    return postParams;
}

// request pushing to the facebook feed - this will attempt to get permission if it doesn't already have it
// if the session is not active, then don't do anything
-(void) requestPushToFacebookFeed:(NSDictionary*)postParams
{
    if (!FBSession.activeSession)
    {
        NSLog(@"No active facebook session");
        return;
    }
    
    if ( !FBSession.activeSession.isOpen)
    {
        NSLog(@"Facebook session not open");
        return;
    }
    
    // mark the target for the social credit
    [self setSptID:kSptFacebookA];
    
    // Ask for publish_actions permissions in context
    if ([FBSession.activeSession.permissions
         indexOfObject:@"publish_actions"] == NSNotFound) {
        // No permissions found in session, ask for it
        [FBSession.activeSession
         requestNewPublishPermissions:
         [NSArray arrayWithObject:@"publish_actions"]
         defaultAudience:FBSessionDefaultAudienceFriends
         completionHandler:^(FBSession *session, NSError *error) {
             if (!error) {
                 // If permissions granted, publish the story
                 [self pushToFacebookFeed:postParams];
             }
         }];
    } else {
        // If permissions present, publish the story
        [self pushToFacebookFeed:postParams];
    }
}

// push a message to the associated member's facebook feed
-(void) pushToFacebookFeed:(NSDictionary*)postParams
{
    [FBRequestConnection startWithGraphPath:@"me/feed" parameters:postParams HTTPMethod:@"POST"
                          completionHandler:^(FBRequestConnection *connection,
                                              id result,
                                              NSError *error) {
                              NSString *alertText;
                              if (error) {
                                  alertText = [NSString stringWithFormat:
                                               @"error: domain = %@, code = %ld",
                                               error.domain, (long)error.code];
                                  // Show the result in an alert
                                  [[[UIAlertView alloc] initWithTitle:@"Facebook Feed Error"
                                                              message:alertText
                                                             delegate:self
                                                    cancelButtonTitle:@"OK"
                                                    otherButtonTitles:nil]
                                   show];
                              } else {
                                  NSLog(@"Facebook Posted action, id: %@",[result objectForKey:@"id"]);
                                  // attempt to credit the user
                                  [self creditSocialPost];
                              }
                          }];
}

// submit credit for the social post to the server
-(void) creditSocialPost
{
    AccountInfo* account = [[UTCApp sharedInstance] account];
    
    if (![account isSignedIn])
    {
        NSLog(@"Cant to social post without an active account");
        return;
    }
    
    NSMutableDictionary* postParams = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                       [NSNumber numberWithInt:[account accID]], @"AccId",
                                       [NSNumber numberWithInt:[self busID]], @"BusId",
                                       [NSNumber numberWithInt:[self sptID]], @"SptID",
                                       nil];
    // attempt to set the push token
    [[UTCAPIClient sharedClient] postSocialPost:postParams withBlock:^(NSError* error ) {
        if (error) {
            NSLog(@"Unable to credit social post %@",[UTCAPIClient getMessageFromError:error]);
        } else {
            NSLog(@"Credited social post");
        }
    }];
    
}

@end
