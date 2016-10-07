//
//  BusinessCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/7/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kBusinessCellHeight     88.0

@interface BusinessCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel* nameLabel;
@property (nonatomic, strong) IBOutlet UILabel* distanceLabel;
@property (nonatomic, strong) IBOutlet UILabel* addressLabel;
@property (nonatomic, strong) IBOutlet UILabel* tagsLabel;
@property (nonatomic, strong) IBOutlet UILabel* backgroundLabel;
@property (nonatomic, strong) IBOutlet UIImageView* ratingImage;

@property (nonatomic, strong) IBOutlet UIImageView* locPicture;
@property (nonatomic, strong) IBOutlet UIImageView* markFavorite;
@property (nonatomic, strong) IBOutlet UIButton* detailsButton;

@end
