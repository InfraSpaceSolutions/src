//
//  StatRedemptionListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface StatRedemptionListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, strong) IBOutlet UITableView* tableView;

-(IBAction) clickBack:(id)sender;

@end
