//
//  SubscribeViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 6/21/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SubscribeViewController : GAITrackedViewController
{
    IBOutlet UIScrollView* scrollView;
    IBOutlet UIView* contentView;
}

@property (nonatomic, strong) IBOutlet UITextField* accountFirstName;
@property (nonatomic, strong) IBOutlet UITextField* accountLastName;
@property (nonatomic, strong) IBOutlet UITextField* accountEMail;
@property (nonatomic, strong) IBOutlet UITextField* accountZip;
@property (nonatomic, strong) IBOutlet UITextField* birthDate;
@property (nonatomic, strong) IBOutlet UIButton* genderMale;
@property (nonatomic, strong) IBOutlet UIButton* genderFemale;
@property (nonatomic, strong) IBOutlet UITextField* productPromo;
@property (nonatomic, strong) IBOutlet UIButton* productAgree;

@property (nonatomic, strong) IBOutlet UILabel* productLabel;
@property (nonatomic, strong) IBOutlet UITextField* paymentCardNumber;
@property (nonatomic, strong) IBOutlet UITextField* paymentExpMonth;
@property (nonatomic, strong) IBOutlet UITextField* paymentExpYear;
@property (nonatomic, strong) IBOutlet UITextField* paymentCVV;
@property (nonatomic, strong) IBOutlet UITextField* billingFirstName;
@property (nonatomic, strong) IBOutlet UITextField* billingLastName;
@property (nonatomic, strong) IBOutlet UITextField* billingStreet;
@property (nonatomic, strong) IBOutlet UITextField* billingCity;
@property (nonatomic, strong) IBOutlet UITextField* billingState;
@property (nonatomic, strong) IBOutlet UITextField* billingZip;

@property (nonatomic, strong) NSDictionary* preloads;

-(IBAction) clickTerms:(id)sender;
-(IBAction) clickFinish:(id)sender;
-(IBAction) clickAgree:(id)sender;
-(IBAction) clickMale:(id)sender;
-(IBAction) clickFemale:(id)sender;


@end


