//
//  LocationCell.m
//  unitethiscity
//
//  Created by Michael Terry on 2/9/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//
#import "UTCApp.h"
#import "LocationCell.h"

@implementation LocationCell

@synthesize nameLabel;
@synthesize distanceLabel;
@synthesize addressLabel;
@synthesize tagsLabel;
@synthesize backgroundLabel;
@synthesize ratingImage;

@synthesize locPicture;
@synthesize markFavorite;
@synthesize actionBarView;
@synthesize dealValueLabel;
@synthesize detailsButton;
@synthesize redeemButton;
@synthesize redeemedBarView;
@synthesize redeemedWhenLabel;

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:style reuseIdentifier:reuseIdentifier];
    if (self) {
        // Initialization code
    }
    return self;
}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated
{
    [super setSelected:selected animated:animated];

    // Configure the view for the selected state
}

@end
