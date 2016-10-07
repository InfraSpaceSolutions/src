//
//  MenuCell.h
//  unitethiscity
//
//  Created by Michael Terry on 2/13/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

#define kMenuCellHeight     44.0

@interface MenuCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel* nameLabel;
@property (nonatomic, strong) IBOutlet UILabel* priceLabel;

@end
