//
//  SummaryCheckInListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 3/28/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SummaryCheckInListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, strong) IBOutlet UITableView* tableView;

-(IBAction) clickBack:(id)sender;

@end
