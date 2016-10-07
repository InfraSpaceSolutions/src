//
//  InboxCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kInboxCellHeight    115.0

@interface InboxCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel* fromLabel;
@property (nonatomic, strong) IBOutlet UILabel* dateLabel;
@property (nonatomic, strong) IBOutlet UILabel* subjectLabel;
@property (nonatomic, strong) IBOutlet UILabel* backgroundLabel;
@property (nonatomic, strong) IBOutlet UIImageView* messageIcon;
@property (nonatomic, strong) IBOutlet UIImageView* fromPicture;

@end
