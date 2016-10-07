//
//  CategoryCell.h
//  unitethiscity
//
//  Created by Michael Terry on 4/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kCategoryCellHeight     36

@interface CategoryCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UIView* cellView;
@property (nonatomic, strong) IBOutlet UILabel* catNameLabel;
@property (nonatomic, strong) IBOutlet UIImageView* catOn;
@property (nonatomic, strong) IBOutlet UIImageView* catOff;
@property (nonatomic, strong) IBOutlet UIImageView* selectOn;
@property (nonatomic, strong) IBOutlet UIImageView* selectOff;

@end
