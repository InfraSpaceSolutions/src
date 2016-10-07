//
//  AnalyticsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/15/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AnalyticsViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (readwrite) int range;
@property (readwrite) int busID;
@property (nonatomic, strong) IBOutlet UILabel* rangeLabel;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickSelectRange:(id)sender;

@end