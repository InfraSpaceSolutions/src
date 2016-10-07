//
//  UnifiedViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@class ScanditSDKBarcodePicker;
@interface UnifiedViewController : GAITrackedViewController <ScanditSDKOverlayControllerDelegate>
{
    ScanditSDKBarcodePicker* picker;
    IBOutlet UIImageView* memberQRCode;
    IBOutlet UIImageView* logoImage;
    IBOutlet UILabel* businessLabel;
}

-(IBAction) clickCheckin:(id)sender;
-(IBAction) clickClose:(id)sender;
-(IBAction) clickRedeem:(id)sender;

@end
