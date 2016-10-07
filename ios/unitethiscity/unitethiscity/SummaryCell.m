//
//  SummaryCell.m
//  unitethiscity
//
//  Created by Michael Terry on 3/19/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "SummaryCell.h"

@implementation SummaryCell

@synthesize locPicture;
@synthesize nameLabel;
@synthesize countLabel;
@synthesize sumLabel;
@synthesize totalLabel;

- (void)awakeFromNib {
    // Initialization code
}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated {
    [super setSelected:selected animated:animated];
    
    // Configure the view for the selected state
}

@end
