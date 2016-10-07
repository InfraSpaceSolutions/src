//
//  EventCell.h
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kEventCellHeight        115.0

@interface EventCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel* busNameLabel;
@property (nonatomic, strong) IBOutlet UILabel* dateLabel;
@property (nonatomic, strong) IBOutlet UILabel* summaryLabel;
@property (nonatomic, strong) IBOutlet UILabel* backgroundLabel;
@property (nonatomic, strong) IBOutlet UIImageView* locPicture;

-(void) setImageByType:(NSString*)eventType;

@end
