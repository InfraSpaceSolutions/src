//
//  SignInViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 12/27/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SignInViewController : UIViewController

@property (nonatomic, strong) IBOutlet UIView* dialogView;
@property (nonatomic, strong) IBOutlet UIView* emailLoginView;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonShort;
@property (nonatomic, strong) IBOutlet UIButton* closeButtonTall;
@property (nonatomic, strong) IBOutlet UITextField* accountTextBox;
@property (nonatomic, strong) IBOutlet UITextField* passwordTextBox;

-(IBAction) clickSignInEmail:(id)sender;
-(IBAction) clickSignInFacebook:(id)sender;
-(IBAction) clickSignUp:(id)sender;
-(IBAction) clickClose:(id)sender;
-(IBAction) clickCancel:(id)sender;
-(IBAction) clickLogin:(id)sender;
-(IBAction) clickForgotPassword:(id)sender;
@end
