//
//  AnalyticsCell.m
//  unitethiscity
//
//  Created by Michael Terry on 2/15/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "AnalyticsCell.h"

@implementation AnalyticsCell

@synthesize backgroundView;
@synthesize nameLabel;
@synthesize totalLabel;
@synthesize percentLabel;

- (void)awakeFromNib {
    // Initialization code
}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated {
    [super setSelected:selected animated:animated];

    // Configure the view for the selected state
}

@end
