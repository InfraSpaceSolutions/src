//
//  UniteListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UniteListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
    int dataRevision;
}

@property (nonatomic, strong) IBOutlet UITableView* tableView;

@end
