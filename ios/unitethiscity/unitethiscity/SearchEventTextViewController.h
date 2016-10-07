//
//  SearchEventTextViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 3/27/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchEventTextViewController : GAITrackedViewController

@property (readwrite) BOOL popOnGo;
@property (nonatomic, strong) NSString* eventType;
@property (nonatomic, strong) IBOutlet UILabel* titleLabel;
@property (nonatomic, strong) IBOutlet UIButton* goButton;
@property (nonatomic, strong) IBOutlet UIButton* cancelButton;
@property (nonatomic, strong) IBOutlet UIButton* categoryButton;
@property (nonatomic, strong) IBOutlet UIButton* selectDateButton;
@property (nonatomic, strong) IBOutlet UITextField* searchTextField;
@property (nonatomic, strong) IBOutlet UITextField* searchDateTextField;
@property (nonatomic, strong) IBOutlet UIView* searchView;

-(IBAction) clickGo:(id)sender;
-(IBAction) clickCancel:(id)sender;
-(IBAction) clickCategory:(id)sender;
-(IBAction) clickBack:(id)sender;
-(IBAction) clickSelectDate:(id)sender;

@end
