//
//  MoreViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 12/14/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface MoreViewController : UIViewController <UICollectionViewDataSource, UICollectionViewDelegateFlowLayout>
{
}

@property (nonatomic, strong) IBOutlet UILabel* businessLabel;
@property (nonatomic, strong) IBOutlet UIButton* galleryButton;
@property (nonatomic, strong) IBOutlet UIButton* menuButton;
@property (nonatomic, strong) IBOutlet UIButton* calendarButton;
@property (nonatomic, strong) IBOutlet UIView* calendarView;
@property (nonatomic, strong) IBOutlet UIView* menuView;
@property (nonatomic, strong) IBOutlet UIView* galleryView;
@property (nonatomic, strong) IBOutlet UITableView* menuTable;
@property (nonatomic, strong) IBOutlet UICollectionView* galleryCollection;
@property (nonatomic, strong) IBOutlet UITableView* calendarTable;
@property (nonatomic, strong) IBOutlet UIButton* menuOnlineButton;

@property (nonatomic, strong) LocationContext* locationContext;
@property (nonatomic, strong) NSArray* menuItems;
@property (nonatomic, strong) NSArray* galleryItems;
@property (nonatomic, strong) NSArray* calendarItems;

@end
