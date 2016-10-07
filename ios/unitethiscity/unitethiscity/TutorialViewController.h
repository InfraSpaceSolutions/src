//
//  TutorialViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 1/16/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TutorialViewController : UIViewController

@property (nonatomic, strong) IBOutlet UIImageView* currentImage;
@property (nonatomic, strong) IBOutlet UIImageView* animatedImage;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;
@property (nonatomic, strong) IBOutlet UIButton* watchVideoButton;

-(IBAction) clickClose:(id)sender;
-(IBAction) clickWatchVideo:(id)sender;

@end
