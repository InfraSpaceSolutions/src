//
//  GuestDeniedViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/1/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface GuestDeniedViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UILabel* headlineLabel;
@property (nonatomic, strong) IBOutlet UILabel* actionLabel;
@property (nonatomic, strong) IBOutlet UIImageView* logoImageView;
@property (readwrite) int deniedAction;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickJoinNow:(id)sender;
-(IBAction) clickSignIn:(id)sender;

@end
