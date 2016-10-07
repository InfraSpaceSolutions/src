//
//  TipViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TipViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UILabel* locationLabel;
@property (nonatomic, strong) IBOutlet UITextView* tipText;

@property (nonatomic, strong) IBOutlet UIButton* crumbDoneButton;
@property (nonatomic, strong) IBOutlet UIButton* cancelButton;
@property (nonatomic, strong) IBOutlet UIButton* postButton;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickPost:(id)sender;
-(IBAction) clickCancel:(id)sender;

@end
