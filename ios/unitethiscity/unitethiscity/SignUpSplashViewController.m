//
//  SignUpSplashViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 12/27/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "SignUpSplashViewController.h"
#import "SignInViewController.h"
#import "UTCAppDelegate.h"
#import <FacebookSDK/FacebookSDK.h>


@interface SignUpSplashViewController ()

@end

@implementation SignUpSplashViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void) viewDidAppear:(BOOL)animated {
    AccountInfo* ai = [[UTCApp sharedInstance] account];
    if ([ai isSignedIn])
    {
        [self dismissViewControllerAnimated:NO completion:nil];
    }
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

-(void) clickSkipStep:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickAlreadyJoined:(id)sender
{
    SignInViewController* dvc = [[SignInViewController alloc] initWithNibName:@"SignInViewController" bundle:nil];
    [self presentViewController:dvc animated:NO completion:nil];
}

-(void) clickSignInEmail:(id)sender
{
    NSLog(@"sign in email");
    [self dismissViewControllerAnimated:NO completion:nil];
    [[[UTCApp sharedInstance] rootViewController] openSubscribe];
}

-(void) clickSignInFacebook:(id)sender
{
    NSLog(@"sign in facebook");
    [FBSession openActiveSessionWithReadPermissions:[[NSArray alloc] initWithObjects:@"email", nil]
                                       allowLoginUI:YES
                                  completionHandler:^(FBSession *session, FBSessionState state, NSError *error) {
                                      if (state == FBSessionStateOpen)
                                      {
                                          // attempt to get the account info
                                          [[FBRequest requestForMe] startWithCompletionHandler:^(FBRequestConnection *connection, NSDictionary<FBGraphUser> *user, NSError *error) {
                                               if (!error) {
                                                   NSLog(@"USER! = %@", user);
                                                   NSMutableDictionary* cred = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[user objectForKey:@"email"], @"Account", @"helloworld", @"Passcode",
                                                                                [user objectForKey:@"id"], @"FacebookId", nil];
                                                   NSLog(@"CRED = %@", cred);
                                                   [[UTCAPIClient sharedClient] postFacebookLogin:cred withBlock:^(NSDictionary* dic, NSError *error) {
                                                       if (error) {
                                                           NSLog(@"Error on facebook login = %@", [UTCAPIClient getMessageFromError:error]);
                                                           [self dismissViewControllerAnimated:NO completion:nil];
                                                           [[UTCApp sharedInstance] createAccountFromFacebook:user];
                                                       } else {
                                                           // perform a login with the attributes
                                                           [[UTCApp sharedInstance] accountLogin:dic];
                                                           [self dismissViewControllerAnimated:NO completion:nil];
                                                       }
                                                   }];
                                               }
                                           }];
                                      }
                                  }];
}

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
                 NSLog(@"USER! = %@", user);
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


@end
