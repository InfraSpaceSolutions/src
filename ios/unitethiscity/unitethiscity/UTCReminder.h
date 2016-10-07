//
//  UTCReminder.h
//  unitethiscity
//
//  Created by Michael Terry on 5/28/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UTCReminder : NSObject <UIAlertViewDelegate>

@property (nonatomic,strong) UIAlertView* reminderAlert;

+(UTCReminder*) sharedInstance;
+(void) countOpportunity;

@end
