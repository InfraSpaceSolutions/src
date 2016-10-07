//
//  SearchMenuViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchMenuViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UIButton* businessesButton;
@property (nonatomic, strong) IBOutlet UIButton* eventsButton;
@property (nonatomic, strong) IBOutlet UIButton* entertainersButton;
@property (nonatomic, strong) IBOutlet UIButton* specialsButton;

-(IBAction) clickSearchBusinesss:(id)sender;
-(IBAction) clickSearchEvents:(id)sender;
-(IBAction) clickSearchSpecials:(id)sender;
-(IBAction) clickSearchEntertainers:(id)sender;

@end
