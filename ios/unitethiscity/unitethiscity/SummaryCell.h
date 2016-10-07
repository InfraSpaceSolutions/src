//
//  SummaryCell.h
//  unitethiscity
//
//  Created by Michael Terry on 3/19/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kSummaryCellHeight     75.0

@interface SummaryCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UIImageView* locPicture;
@property (nonatomic, strong) IBOutlet UILabel* nameLabel;
@property (nonatomic, strong) IBOutlet UILabel* countLabel;
@property (nonatomic, strong) IBOutlet UILabel* sumLabel;
@property (nonatomic, strong) IBOutlet UILabel* totalLabel;

@end
