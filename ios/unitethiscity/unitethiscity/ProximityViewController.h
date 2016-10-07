//
//  ProximityViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 7/15/14.
//  Copyright (c) 2014 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ProximityViewController :  GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
    int dataRevision;
}

@property (nonatomic, strong) IBOutlet UITableView* tableView;

@end
