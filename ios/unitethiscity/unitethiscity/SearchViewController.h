//
//  SearchViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate>
{
    NSArray* categories;
}
@property (nonatomic, strong) IBOutlet UITableView* tableView;

@end
