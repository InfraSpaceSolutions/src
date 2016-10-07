//
//  SearchCategoryListViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchCategoryListViewController : GAITrackedViewController <UITableViewDataSource, UITableViewDelegate, UIAlertViewDelegate>
{
    NSArray* categories;
    NSMutableArray* editAntiFilters;
}

@property (nonatomic, strong) NSString* eventType;
@property (readwrite) BOOL popOnApply;
@property (nonatomic, strong) IBOutlet UITableView* tableView;

-(IBAction) clickBack:(id)sender;
-(IBAction) clickSelectAll:(id)sender;
-(IBAction) clickSelectNone:(id)sender;
-(IBAction) clickApply:(id)sender;

@end
