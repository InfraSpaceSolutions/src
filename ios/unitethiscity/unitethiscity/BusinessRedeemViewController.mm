//
//  BusinessRedeemViewController.mm
//  unitethiscity
//
//  Created by Michael Terry on 2/18/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "BusinessRedeemViewController.h"
#import "ScanditSDKBarcodePicker.h"
#import "UTCQREncoder.h"
#import "AccountInfo.h"
#import "LocationInfo.h"

@interface BusinessRedeemViewController ()

@end

@implementation BusinessRedeemViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self)
    {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.

    [self setScreenName:@"Business Redeem"];
    
    // hide the logo to save space on 3.5 screens
    [logoImageView setHidden:(![UTCSettings isScreenTall])];

    // clear any pending scan actions
    [[UTCApp sharedInstance] setPendingAction:kScanActionNone];
    
    // create the member specific qr code for display so the business can scan
    NSString* busquri = [[[UTCApp sharedInstance] account] businessIdentifierQURI];
    
    [businessQRCode setImage:[UTCQREncoder fromString:busquri withSize:512]];

    // get the location id as a key for accessing the dictionary
    NSNumber *key = [NSNumber numberWithInt:[[UTCApp sharedInstance] selectedLocation]];
    
    // get the locaiton info from the dictionary of all locations
    LocationInfo* loc = [[[UTCApp sharedInstance] locationDictionary] objectForKey:key];
    [businessLabel setText:[loc name]];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickCheckin:(id)sender
{
    // track that we are doing a checkin
    [[UTCApp sharedInstance] setPendingAction:kScanActionMemberCheckin];
    
    picker = [[ScanditSDKBarcodePicker alloc] initWithAppKey:kScanditSDKAppKey];
    
	// Show the search bar for manual entry of a barcode.
    [picker.overlayController showSearchBar:NO];
	
	// Show the tool bar with cancel button.
    [picker.overlayController showToolBar:YES];
	
    // Set this class as the delegate for the overlay controller. It will now receive events when
    // a barcode was successfully scanned, manually entered or the cancel button was pressed.
	picker.overlayController.delegate = self;
    
	// Set the center of the area where barcodes are detected successfully.
    // The default is the center of the screen (0.5, 0.5)
    [picker setScanningHotSpotToX:0.5 andY:0.5];
    [picker.overlayController setBannerImageWithResource:@"headerLogo" ofType:@"png"];
    
    [self presentViewController:picker animated:YES completion:nil];
    [picker startScanning];
}

-(void) clickRedeem:(id)sender
{
    // track that we are doing a redeem
    [[UTCApp sharedInstance] setPendingAction:kScanActionMemberRedeem];
    
    picker = [[ScanditSDKBarcodePicker alloc] initWithAppKey:kScanditSDKAppKey];
    
	// Show the search bar for manual entry of a barcode.
    [picker.overlayController showSearchBar:NO];
	
	// Show the tool bar with cancel button.
    [picker.overlayController showToolBar:YES];
	
    // Set this class as the delegate for the overlay controller. It will now receive events when
    // a barcode was successfully scanned, manually entered or the cancel button was pressed.
	picker.overlayController.delegate = self;
    
	// Set the center of the area where barcodes are detected successfully.
    // The default is the center of the screen (0.5, 0.5)
    [picker setScanningHotSpotToX:0.5 andY:0.5];
    [picker.overlayController setBannerImageWithResource:@"headerLogo" ofType:@"png"];
    
    [self presentViewController:picker animated:YES completion:nil];
    [picker startScanning];
}

-(void) clickClose:(id)sender
{
    [self dismissViewControllerAnimated:NO completion:nil];
}

//////
// ScanditSDKOverlayControllerDelegate
//////

// scan completed
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didScanBarcode:(NSDictionary *)barcodeResult
{
	[picker stopScanning];
    [picker dismissViewControllerAnimated:NO completion:nil];
    
	if (barcodeResult == nil)
    {
        NSLog(@"barcodeResult is nil");
        [[UTCApp sharedInstance] setPendingAction:kScanActionNone];
        return;
    }
    
    [self dismissViewControllerAnimated:NO completion:nil];
    
    // store the captured bar code
    [[UTCApp sharedInstance] setLastQurl:[barcodeResult objectForKey:@"barcode"]];
    
    // process the request
    switch ([[UTCApp sharedInstance] pendingAction])
    {
        case kScanActionMemberRedeem:
            [[[UTCApp sharedInstance] rootViewController] openBusinessRedeemResults];
            break;
            
        case kScanActionMemberCheckin:
            [[[UTCApp sharedInstance] rootViewController] openBusinessCheckInResults];
            break;
            
        default:
            break;
    }
}

// user canceled the scan
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didCancelWithStatus:(NSDictionary *)status
{
    [picker dismissViewControllerAnimated:YES completion:nil];
    [[UTCApp sharedInstance] setPendingAction:kScanActionNone];
}

// user entered a bar code manually
- (void)scanditSDKOverlayController:(ScanditSDKOverlayController *)scanditSDKOverlayController didManualSearch:(NSString *)input
{
}

@end
