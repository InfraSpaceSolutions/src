//
//  UTCReminder.m
//  unitethiscity
//
//  Created by Michael Terry on 5/28/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCSettings.h"
#import "AccountInfo.h"
#import "UTCReminder.h"


@interface UTCReminder (private)
-(void) showReminderAlert;
@end

@implementation UTCReminder

@synthesize reminderAlert;

-(id) init
{
    self = [super init];
    if (self != nil)
    {
    }
    return self;
}

+(UTCReminder*) sharedInstance
{
    static UTCReminder* sharedInstance = nil;
    if (sharedInstance == nil)
    {
        sharedInstance = [[UTCReminder alloc] init];
    }
    return sharedInstance;
}

+(void) countOpportunity
{
    //NSLog(@"facebook = %d", [[UTCApp sharedInstance] facebookStatus]);

    // if facebook is on or we can't tell, do nothing
    if ([[UTCApp sharedInstance] facebookStatus] != kFacebookOff)
    {
        return;
    }
    
    // if we aren't logged in - do nothing
    if (![[[UTCApp sharedInstance] account] isSignedIn])
    {
        return;
    }
    
    BOOL promptNow = NO;
    NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
    int reminderCount = (int)[userDefaults integerForKey:kKeyReminderCount];
    if (++reminderCount >= kReminderPeriod)
    {
        promptNow = YES;
        reminderCount = 0;
    }
    [userDefaults setInteger:reminderCount forKey:kKeyReminderCount];
    [userDefaults synchronize];
    if ( promptNow )
    {
        //[[UTCReminder sharedInstance] showReminderAlert];
    }
}

-(void) showReminderAlert
{
    UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"Connect to Facebook"
                                                        message:@"Earn more chances to win by automatically posting when you redeem offers or check in."
                                                       delegate:self
                                              cancelButtonTitle:@"Not Now"
                                              otherButtonTitles:@"Ok", nil];
    [self setReminderAlert:alertView];
    [alertView show];
}


- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
	switch (buttonIndex) {
		case 0:
		{
			// not now, maybe later
			break;
		}
		case 1:
		{
			// connect to facebook
            UTCAppDelegate *appDelegate = (UTCAppDelegate*)[[UIApplication sharedApplication] delegate];
            [appDelegate openFacebookSessionWithAllowLoginUI:YES];
			break;
		}
		default:
			break;
	}
}

@end
