//
//  FavoriteListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface FavoriteListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
}

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (nonatomic, strong) IBOutlet UIView* noContentView;

@end
