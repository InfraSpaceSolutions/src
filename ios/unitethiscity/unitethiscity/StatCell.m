//
//  StatCell.m
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "StatCell.h"

@implementation StatCell

@synthesize nameLabel;
@synthesize timestampLabel;
@synthesize optionalLabel;
@synthesize summaryLabel;
@synthesize alternateLabel;
@synthesize locPicture;
@synthesize ratingImage;

- (void)awakeFromNib {
    // Initialization code
}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated {
    [super setSelected:selected animated:animated];

    // Configure the view for the selected state
}

@end
