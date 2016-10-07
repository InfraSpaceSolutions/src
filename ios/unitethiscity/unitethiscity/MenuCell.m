//
//  MenuCell.m
//  unitethiscity
//
//  Created by Michael Terry on 2/13/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "MenuCell.h"

@implementation MenuCell

@synthesize nameLabel;
@synthesize priceLabel;

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
