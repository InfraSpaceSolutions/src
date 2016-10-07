//
//  BusinessListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface BusinessListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
}

@property (nonatomic, strong) IBOutlet UITableView* tableView;

@end
