//
//  SignInViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 12/27/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "SignInViewController.h"
#import "UTCAppDelegate.h"
#import <FacebookSDK/FacebookSDK.h>

@interface SignInViewController ()

@end

@implementation SignInViewController

@synthesize dialogView;
@synthesize emailLoginView;
@synthesize closeButtonShort;
@synthesize closeButtonTall;
@synthesize accountTextBox;
@synthesize passwordTextBox;

- (void)viewDidLoad {
    [super viewDidLoad];
    // round the corners of the background view
    dialogView.layer.cornerRadius = 5;
    dialogView.layer.masksToBounds = YES;
    [closeButtonTall setHidden:![UTCSettings isScreenTall]];
    [closeButtonShort setHidden:[UTCSettings isScreenTall]];
    [emailLoginView setHidden:YES];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(BOOL) textFieldShouldReturn:(UITextField *)textField{
    
    [textField resignFirstResponder];
    return YES;
}

-(void) clickSignInEmail:(id)sender
{
    [emailLoginView setHidden:NO];
}

-(void) clickSignInFacebook:(id)sender
{
    // open the active session with explicit request for email address permissions
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
                                                  NSLog(@"cred = %@", cred);
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

-(void) clickSignUp:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSignUpSplashViewController];
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickLogin:(id)sender
{
    // dont bother if the form is empty
    if (([[accountTextBox text] length] == 0) || ([[passwordTextBox text] length] == 0))
    {
        return;
    }
    
    // build the credentials for submission
    NSString* hashword = [AccountInfo passwordEncryption:[passwordTextBox text]];
    NSMutableDictionary* cred = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[accountTextBox text], @"Account", hashword, @"Password", nil];
    // attempt the login
    [[UTCAPIClient sharedClient] postLogin:cred withBlock:^(NSDictionary* dic, NSError *error) {
        if (error) {
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            // perform a login with the attributes
            [[UTCApp sharedInstance] accountLogin:dic];
            [self dismissViewControllerAnimated:NO completion:nil];
        }
    }];
}

-(void) clickCancel:(id)sender
{
    [emailLoginView setHidden:YES];
}

-(void) clickForgotPassword:(id)sender
{
    // dismiss the login form if its open
    NSURL* url = [NSURL URLWithString:kForgotPasswordURL];
    [[UIApplication sharedApplication] openURL:url];
}

@end
