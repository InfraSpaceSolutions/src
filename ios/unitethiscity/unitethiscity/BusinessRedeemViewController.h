//
//  BusinessRedeemViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/7/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@class ScanditSDKBarcodePicker;
@interface BusinessRedeemViewController : GAITrackedViewController <ScanditSDKOverlayControllerDelegate>
{
    ScanditSDKBarcodePicker* picker;
    IBOutlet UIImageView* businessQRCode;
    IBOutlet UILabel* businessLabel;
    IBOutlet UIImageView* logoImageView;
}

-(IBAction) clickCheckin:(id)sender;
-(IBAction) clickClose:(id)sender;
-(IBAction) clickRedeem:(id)sender;

@end

