//
//  SubscribeViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 6/21/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "AccountInfo.h"
#import "UTCAppDelegate.h"
#import "SubscribeViewController.h"
#import "AbstractActionSheetPicker.h"
#import "ActionSheetStringPicker.h"
#import "ActionSheetDatePicker.h"

@interface SubscribeViewController ()

@end

@implementation SubscribeViewController

@synthesize preloads;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    if (preloads != nil)
    {
        _accountFirstName.text = [preloads objectForKey:@"first_name"];
        _accountLastName.text = [preloads objectForKey:@"last_name"];
        _accountEMail.text = [preloads objectForKey:@"email"];
        _birthDate.text = [preloads objectForKey:@"birthday"];
        [_genderMale setSelected:NO];
        [_genderFemale setSelected:NO];
        NSString* gender = [preloads objectForKey:@"gender"];
        if ([gender compare:@"male" options:NSCaseInsensitiveSearch] == NSOrderedSame) {
            [_genderMale setSelected:YES];
        }
        if ([gender compare:@"female" options:NSCaseInsensitiveSearch] == NSOrderedSame) {
            [_genderFemale setSelected:YES];
        }
    }
    
    // Do any additional setup after loading the view from its nib.
    [scrollView addSubview:contentView];
    
    // configure the scrollview size
    [scrollView setContentSize:contentView.frame.size];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - UITextFieldDelegate

-(BOOL) textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    return YES;
}

// choose the date of birth using the action sheet picker
- (IBAction)selectDateOfBirth:(UIControl *)sender {
    NSLog(@"Select date of birth");

    NSDate* currentDate = [NSDate date];
    NSDateFormatter* df = [[NSDateFormatter alloc] init];
    [df setDateFormat:@"MM/dd/yyyy"];
    
    NSDate* parseDate = [df dateFromString:[_birthDate text]];
    if (parseDate != nil) {
        currentDate = parseDate;
    }
    
    ActionSheetDatePicker* datePicker = [[ActionSheetDatePicker alloc] initWithTitle:@"Date of Birth"
                                                                      datePickerMode:UIDatePickerModeDate
                                                                        selectedDate:currentDate
                                                                              target:self
                                                                              action:@selector(dateOfBirthSelected:element:)
                                                                              origin:sender];
    [datePicker setHideCancel:NO];
    [datePicker showActionSheetPicker];
}

// callback for the selected date of birth - update the text field
- (void)dateOfBirthSelected:(NSDate *)selectedDate element:(id)element {
    NSLog(@"date of birth selected - %@", selectedDate);
    NSDateFormatter* df = [[NSDateFormatter alloc] init];
    [df setDateFormat:@"MM/dd/YYYY"];
    [_birthDate setText:[df stringFromDate:selectedDate]];
}

-(IBAction) clickTerms:(id)sender
{
    NSURL* url = [NSURL URLWithString:kGeneralTermsURL];
    [[UIApplication sharedApplication] openURL:url];
}

-(void) validationErrorMessage:(NSString*)msg
{
    [[[UIAlertView alloc] initWithTitle:@"Error" message:msg delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
}

-(BOOL) requiredFieldName:(NSString*)name withValue:(NSString*)value
{
    NSString* trimmed = [value stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
    if ( [trimmed length] <=0 )
    {
        [self validationErrorMessage:[NSString stringWithFormat:@"%@ is required.", name]];
        return NO;
    }
    return YES;
}

-(BOOL) validateForm
{
    // check the required fields
    if ( ![self requiredFieldName:@"Account First Name" withValue:[_accountFirstName text]]) return NO;
    if ( ![self requiredFieldName:@"Account Last Name" withValue:[_accountLastName text]]) return NO;
    if ( ![self requiredFieldName:@"Account EMail Address" withValue:[_accountEMail text]]) return NO;

    // gender is now optional
    // birthdate is now optional
    // zipcode is now optional
    
    // must agree to terms and conditions
    if (![_productAgree isSelected])
    {
        [self validationErrorMessage:@"Please agree to terms and conditions"];
        return NO;
    }
    
    return YES;
}

-(IBAction) clickFinish:(id)sender
{
    NSLog(@"clickFinish");
    
    if (![self validateForm])
    {
        return;
    }
    
    [[UTCApp sharedInstance] startActivity];
    
    // build the parameters to send to create the new account
    NSMutableDictionary* param = [[NSMutableDictionary alloc] init];
    [param setObject:[_accountFirstName text] forKey:@"AccFName"];
    [param setObject:[_accountLastName text] forKey:@"AccLName"];
    [param setObject:[_accountEMail text] forKey:@"AccEMail"];
    [param setObject:[_accountZip text] forKey:@"AccZip"];

    // add the specified gender or set to unspecified
    if ( [_genderFemale isSelected]) {
        [param setObject:@"F" forKey:@"AccGender"];
    } else if ( [_genderMale isSelected]) {
        [param setObject:@"M" forKey:@"AccGender"];
    } else {
        [param setObject:@"?" forKey:@"AccGender"];
    }
    
    // add the birthdate
    [param setObject:[_birthDate text] forKey:@"AccBirthdate"];
    
    NSString* promoCode = [_productPromo text];
    if (![promoCode length])
    {
        promoCode = @"!nopromocode";
    }
    
    [param setObject:promoCode forKey:@"PromoCode"];
    [param setObject:[NSNumber numberWithInt:2] forKey:@"PrdID"];
 
    NSLog(@"Param: %@", param);
    
    // attempt the subscription
    [[UTCAPIClient sharedClient] postFreeSignUp:param withBlock:^(NSDictionary* dic, NSError *error) {
        if (error) {
            NSLog(@"Error: %@", error);
            [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
        } else {
            NSLog(@"Account Credentials: %@", dic);
            
            
            // build the credentials for submission
            NSString* hashword = [AccountInfo passwordEncryption:[dic objectForKey:@"Password"]];
            NSMutableDictionary* cred = [[NSMutableDictionary alloc] initWithObjectsAndKeys:[dic objectForKey:@"Account"], @"Account", hashword, @"Password", nil];
            NSLog(@"New cred = %@", cred);
            // attempt the login
            [[UTCAPIClient sharedClient] postLogin:cred withBlock:^(NSDictionary* dic, NSError *error) {
                if (error) {
                    [[[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil) message:[UTCAPIClient getMessageFromError:error] delegate:nil cancelButtonTitle:nil otherButtonTitles:NSLocalizedString(@"OK", nil), nil] show];
                } else {
                    // perform a login with the attributes
                    [[UTCApp sharedInstance] accountLogin:dic];
                    [[[UTCApp sharedInstance] rootViewController] openAccount];
                }
            }];
            
            // perform a login with the attributes
            //[[UTCApp sharedInstance] accountLogin:dic];

            // go to the account screen
        }
    }];
    [[UTCApp sharedInstance] stopActivity];
    

}

-(IBAction) clickAgree:(id)sender
{
    NSLog(@"clickAgree");
 
    [_productAgree setSelected:![_productAgree isSelected]];
    
}

-(IBAction) clickMale:(id)sender
{
    NSLog(@"clickMale");
    
    [_genderMale setSelected:YES];
    [_genderFemale setSelected:NO];
    
}

-(IBAction) clickFemale:(id)sender
{
    NSLog(@"clickFemale");
    [_genderMale setSelected:NO];
    [_genderFemale setSelected:YES];
}




@end
