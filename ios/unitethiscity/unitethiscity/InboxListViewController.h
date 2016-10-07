//
//  InboxListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface InboxListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
    NSArray* inboxMessages;
}

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (nonatomic, strong) IBOutlet UIView* noContentView;

@end
