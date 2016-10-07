//
//  AccountViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 4/2/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "AccountViewController.h"
#import "Gravatar.h"
#import "UTCAppDelegate.h"
#import "SignInViewController.h"
#import <FacebookSDK/FacebookSDK.h>
#import "SocialInfo.h"

#import "GoogleAnalytics/GAITrackedViewController.h"

@interface AccountViewController ()
-(void) loadAccountInfo:(AccountInfo*)ai;
-(void) showAnonView;
-(void) showAuthView;
@end

@implementation AccountViewController

@synthesize anonView = _anonView;
@synthesize authView = _authView;
@synthesize businessButton = _businessButton;
@synthesize referralButton = _referralButton;
@synthesize joinNowButton = _joinNowButton;
@synthesize readMoreButton = _readMoreButton;
@synthesize twitterButton;
@synthesize facebookButton;
@synthesize instagramButtom;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self setScreenName:@"Account"];

    // Do any additional setup after loading the view from its nib.
    AccountInfo* ai = [[UTCApp sharedInstance] account];
    [self loadAccountInfo:ai];
    
    // update the displayed version from the bundle
    [_versionLabel setText:[NSString stringWithFormat:@"v%@.%@",
                            [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"],
                            [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"]]];

}

-(void) viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    AccountInfo* ai = [[UTCApp sharedInstance] account];
    SocialInfo* si = [[UTCApp sharedInstance] socialInfo];
    [self loadAccountInfo:ai];
    [FBSession openActiveSessionWithReadPermissions:[[NSArray alloc] initWithObjects:@"email", nil] allowLoginUI:NO completionHandler:^(FBSession *session, FBSessionState state, NSError *error) {
        if (state == FBSessionStateOpen) {
            [si setConnectFacebook:YES];
        } else {
            [si setConnectFacebook:NO];
        }
    }];
}

-(void) loadAccountInfo:(AccountInfo*)ai
{
    // display the current user's name
    [_userNameLabel setText:[NSString stringWithFormat:@"%@ %@", [ai firstName], [ai lastName]]];
    
    // update the sign in/ sign out button based on the account info
    [_signInOutButton setTitle:([ai isSignedIn] ? @"SIGN OUT" : @"SIGN IN") forState:UIControlStateNormal];
    
    // load the gravatar for the active user
    [_avatarImageView setImageWithURL:[NSURL URLWithString:[[[UTCApp sharedInstance] rootViewController] accountImageURLWithDimensions:150]]];
    
    // show the business button if the user has business roles
    [_businessButton setHidden:([[ai businessRoles] count] <= 0)];
    
    // show the referral button if the user has referral codes
    [_referralButton setHidden:YES];
    
    // reload the button on the root view controller
    [[[UTCApp sharedInstance] rootViewController] loadAccountImage];

    // set the states of the social buttons
    int defaultSocial = [[UTCApp sharedInstance] defaultSocialChannel];
    [twitterButton setSelected:(defaultSocial == kDefaultSocialTwitter)];
    [facebookButton setSelected:(defaultSocial == kDefaultSocialFacebook)];
    [instagramButtom setSelected:(defaultSocial == kDefaultSocialInstagram)];
    
    // show the interface view based on the login state
    if ([ai isSignedIn])
    {
        [self showAuthView];
    }
    else
    {
        [self showAnonView];
    }
}

-(void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

-(BOOL) textFieldShouldReturn:(UITextField *)textField{
    
    [textField resignFirstResponder];
    return YES;
}
-(IBAction) clickJoinNow:(id)sender
{
    // dismiss the login form if its open
    [[[UTCApp sharedInstance] rootViewController] openSubscribe];
}

-(IBAction) clickSignUpEmail:(id)sender
{
    // dismiss the login form if its open
    [[[UTCApp sharedInstance] rootViewController] openSubscribe];
}

-(IBAction) clickSignUpFacebook:(id)sender
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
                                                  NSLog(@"USER$ = %@", user);
                                                  NSMutableDictionary* cred = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[user objectForKey:@"email"], @"Account", @"helloworld", @"Passcode",
                                                                               [user objectForKey:@"id"], @"FacebookId", nil];
                                                  [[UTCAPIClient sharedClient] postFacebookLogin:cred withBlock:^(NSDictionary* dic, NSError *error) {
                                                      if (error) {
                                                          NSLog(@"Error on facebook login = %@", [UTCAPIClient getMessageFromError:error]);
                                                          [self dismissViewControllerAnimated:NO completion:nil];
                                                          [[UTCApp sharedInstance] createAccountFromFacebook:user];
                                                      } else {
                                                          // perform a login with the attributes
                                                          [[UTCApp sharedInstance] accountLogin:dic];
                                                          AccountInfo* ai = [[UTCApp sharedInstance] account];
                                                          [self loadAccountInfo:ai];
                                                      }
                                                  }];
                                              }
                                          }];
                                      }
                                  }];
}

-(IBAction) clickReadMore:(id)sender
{
    // dismiss the login form if its open
    NSURL* url = [NSURL URLWithString:kMemberReadMoreURL];
    [[UIApplication sharedApplication] openURL:url];
    
}

-(IBAction) clickSignInOut:(id)sender
{
    // if we are logged in, log out
    if ([[[UTCApp sharedInstance] account] isSignedIn])
    {
        [[UTCApp sharedInstance] accountLogout];
        if (FBSession.activeSession.isOpen)
        {
            UTCAppDelegate *appDelegate = (UTCAppDelegate*)[[UIApplication sharedApplication] delegate];
            [appDelegate closeFacebookSession];
        }
    }
    else
    {
        SignInViewController* dvc = [[SignInViewController alloc] initWithNibName:@"SignInViewController" bundle:nil];
        [self presentViewController:dvc animated:NO completion:nil];
    }
    [self loadAccountInfo:[[UTCApp sharedInstance] account]];
}

-(IBAction) clickBusiness:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openBusinesses];
}

-(IBAction) clickReferral:(id)sender
{
    NSLog(@"Referral support not implemented");
    NSURL *settingsURL = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
    [[UIApplication sharedApplication] openURL:settingsURL];

}

-(void) clickViewTutorial:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openTutorialViewController];
}

-(IBAction) clickFacebook:(id)sender
{
    // if we are already this soical then turn off, otherwise assign this social choice
    int origSocial = [[UTCApp sharedInstance] defaultSocialChannel];
    int newSocial = (origSocial == kDefaultSocialFacebook) ? kDefaultSocialNone : kDefaultSocialFacebook;
    [[UTCApp sharedInstance] setDefaultSocialChannel:newSocial];
    
    if (newSocial == kDefaultSocialFacebook) {
        [FBSession openActiveSessionWithReadPermissions:[[NSArray alloc] initWithObjects:@"email", nil] allowLoginUI:YES completionHandler:^(FBSession *session, FBSessionState state, NSError *error) {
            [self loadAccountInfo:[[UTCApp sharedInstance] account]];
        }];
    } else {
        [self loadAccountInfo:[[UTCApp sharedInstance] account]];
    }
}

-(IBAction) clickTwitter:(id)sender
{
    // if we are already this soical then turn off, otherwise assign this social choice
    int origSocial = [[UTCApp sharedInstance] defaultSocialChannel];
    int newSocial = (origSocial == kDefaultSocialTwitter) ? kDefaultSocialNone : kDefaultSocialTwitter;
    [[UTCApp sharedInstance] setDefaultSocialChannel:newSocial];
    [self loadAccountInfo:[[UTCApp sharedInstance] account]];
}

-(IBAction) clickInstagram:(id)sender
{
    // if we are already this soical then turn off, otherwise assign this social choice
    int origSocial = [[UTCApp sharedInstance] defaultSocialChannel];
    int newSocial = (origSocial == kDefaultSocialInstagram) ? kDefaultSocialNone : kDefaultSocialInstagram;
    [[UTCApp sharedInstance] setDefaultSocialChannel:newSocial];
    [self loadAccountInfo:[[UTCApp sharedInstance] account]];
}


// show the view appropriate for when user is not logged in
-(void) showAnonView
{
    [_anonView setHidden:NO];
    [_authView setHidden:YES];
}


// show the view appropriate for when the user is logged in
-(void) showAuthView
{
    [_anonView setHidden:YES];
    [_authView setHidden:NO];
}

@end
