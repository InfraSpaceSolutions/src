//
//  SearchEntertainerListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/21/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchEntertainerListViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (nonatomic, strong) IBOutlet UISearchBar* textSearchBar;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickSearchByKeyword:(id)sender;
-(IBAction) clickSearchByCategory:(id)sender;

@end
