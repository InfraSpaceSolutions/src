//
//  RatingViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 1/14/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@class AMRatingControl;

@interface RatingViewController : UIViewController

@property (nonatomic, strong) IBOutlet UIView* dialogView;
@property (nonatomic, strong) IBOutlet UILabel* businessName;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonShort;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonTall;
@property (nonatomic, strong) AMRatingControl* ratingControl;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickDone:(id)sender;

@end
