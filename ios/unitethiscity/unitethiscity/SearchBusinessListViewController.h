//
//  SearchBusinessListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchBusinessListViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (nonatomic, strong) IBOutlet UISearchBar* textSearchBar;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickSearchByKeyword:(id)sender;
-(IBAction) clickSearchByCategory:(id)sender;

@end
