//
//  StatCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kStatCellHeight     115.0

@interface StatCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel* nameLabel;
@property (nonatomic, strong) IBOutlet UILabel* timestampLabel;
@property (nonatomic, strong) IBOutlet UILabel* optionalLabel;
@property (nonatomic, strong) IBOutlet UILabel* summaryLabel;
@property (nonatomic, strong) IBOutlet UILabel* alternateLabel;
@property (nonatomic, strong) IBOutlet UIImageView* locPicture;
@property (nonatomic, strong) IBOutlet UIImageView* ratingImage;

@end
