//
//  AnalyticsCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/15/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kAnalyticsCellHeight    44.0

@interface AnalyticsCell : UITableViewCell

@property (strong, nonatomic) IBOutlet UIView* backgroundView;
@property (strong, nonatomic) IBOutlet UILabel* nameLabel;
@property (strong, nonatomic) IBOutlet UILabel* totalLabel;
@property (strong, nonatomic) IBOutlet UILabel* percentLabel;

@end
