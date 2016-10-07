//
//  SplashViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "GAITrackedViewController.h"

#define kNumSplashTips      7

@interface SplashViewController : GAITrackedViewController
{
    IBOutlet UIImageView* tipImageA;
    IBOutlet UIImageView* tipImageB;
    int tipIndex;
}

@property (nonatomic,strong) IBOutlet UIImageView* tipImageView;
@property (nonatomic, retain) IBOutletCollection(UILabel) NSArray *businessLabels;

-(IBAction) clickBig:(id)sender;

@end
