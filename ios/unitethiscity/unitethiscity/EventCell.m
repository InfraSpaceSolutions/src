//
//  EventCell.m
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "EventCell.h"

@implementation EventCell

@synthesize busNameLabel;
@synthesize dateLabel;
@synthesize summaryLabel;
@synthesize backgroundLabel;
@synthesize locPicture;

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

-(void) setImageByType:(NSString *)eventType
{
    NSLog(@"Event type = %@", eventType);
    if ([eventType isEqualToString:@"Special"])
    {
        [locPicture setImage:[UIImage imageNamed:@"eventSpecial"]];
    }
    else if ([eventType isEqualToString:@"Event"])
    {
        [locPicture setImage:[UIImage imageNamed:@"eventEvent"]];
        
    }
    else if ([eventType isEqualToString:@"Entertainer"])
    {
        [locPicture setImage:[UIImage imageNamed:@"eventEntertainer"]];
    }
    else
    {
        [locPicture setImage:[UIImage imageNamed:@"eventBusiness"]];
    }
}

@end
