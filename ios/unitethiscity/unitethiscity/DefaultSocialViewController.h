//
//  DefaultSocialViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 4/5/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DefaultSocialViewController : UIViewController

@property (nonatomic, strong) IBOutlet UIView* dialogView;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonShort;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonTall;
@property (readwrite) int defaultSocial;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickYes:(id)sender;
-(IBAction) clickNo:(id)sender;

@end
