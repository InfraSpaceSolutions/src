//
//  SearchEventListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchEventListViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UITableView* tableView;
@property (nonatomic, strong) IBOutlet UISearchBar* textSearchBar;
@property (nonatomic, strong) IBOutlet NSString* eventType;
@property (nonatomic, strong) IBOutlet UILabel* eventTypeLabel;
@property (nonatomic, strong) IBOutlet NSDate* eventDate;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickSearchByKeyword:(id)sender;
-(IBAction) clickSearchByCategory:(id)sender;

@end
