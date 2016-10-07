//
//  LocationCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kLocationCellHeight     132.0

@interface LocationCell : UITableViewCell
{
    IBOutlet UILabel*       nameLabel;
    IBOutlet UILabel*       distanceLabel;
    IBOutlet UILabel*       addressLabel;
    IBOutlet UILabel*       tagsLabel;
    IBOutlet UILabel*       backgroundLabel;
    IBOutlet UIImageView*   ratingImage;
}

@property (nonatomic, strong) UILabel* nameLabel;
@property (nonatomic, strong) UILabel* distanceLabel;
@property (nonatomic, strong) UILabel* addressLabel;
@property (nonatomic, strong) UILabel* tagsLabel;
@property (nonatomic, strong) UILabel* backgroundLabel;
@property (nonatomic, strong) UIImageView* ratingImage;

@property (nonatomic, strong) IBOutlet UIImageView* locPicture;
@property (nonatomic, strong) IBOutlet UIImageView* markFavorite;
@property (nonatomic, strong) IBOutlet UIView* actionBarView;
@property (nonatomic, strong) IBOutlet UILabel* dealValueLabel;
@property (nonatomic, strong) IBOutlet UIButton* detailsButton;
@property (nonatomic, strong) IBOutlet UIButton* redeemButton;
@property (nonatomic, strong) IBOutlet UIView* redeemedBarView;
@property (nonatomic, strong) IBOutlet UILabel* redeemedWhenLabel;

@end
