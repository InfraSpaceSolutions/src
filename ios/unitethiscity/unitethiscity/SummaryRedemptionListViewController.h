//
//  SummaryRedemptionListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 3/19/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SummaryRedemptionListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, strong) IBOutlet UITableView* tableView;

-(IBAction) clickBack:(id)sender;

@end
