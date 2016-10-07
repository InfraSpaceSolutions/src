//
//  WalletViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface WalletViewController : GAITrackedViewController
{
}

@property (nonatomic, strong) IBOutlet UIScrollView* scrollView;
@property (nonatomic, strong) IBOutlet UIView* contentView;
@property (nonatomic, strong) IBOutlet UIView* backgroundView;
@property (nonatomic, strong) IBOutlet UILabel* greetingLabel;
@property (nonatomic, strong) IBOutlet UILabel* cashLabel;
@property (nonatomic, strong) IBOutlet UILabel* pointsLabel;
@property (nonatomic, strong) IBOutlet UILabel* savingsThisMonth;
@property (nonatomic, strong) IBOutlet UILabel* savingsAllTime;


@end
