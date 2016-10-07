//
//  SearchEventTextViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 3/27/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCRootViewController.h"
#import "SearchEventTextViewController.h"
#import "AbstractActionSheetPicker.h"
#import "ActionSheetStringPicker.h"
#import "ActionSheetDatePicker.h"

@interface SearchEventTextViewController ()

@end

@implementation SearchEventTextViewController

@synthesize popOnGo;
@synthesize eventType;
@synthesize titleLabel;
@synthesize goButton;
@synthesize cancelButton;
@synthesize categoryButton;
@synthesize searchTextField;
@synthesize searchDateTextField;
@synthesize selectDateButton;
@synthesize searchView;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    [self setScreenName:@"Search Event Text"];
    if ([UTCSettings isScreenTall]) {
        CGRect btFrame;
        btFrame = searchView.frame;
        btFrame.origin.y += 32;
        [searchView setFrame:btFrame];
    }
}

- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [searchTextField setText:[[UTCApp sharedInstance] searchText]];
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:@"MM/dd/yyyy"];
    if ([[UTCApp sharedInstance] searchDate]) {
        [searchDateTextField setText:[dateFormatter stringFromDate:[[UTCApp sharedInstance] searchDate]]];
    } else {
        [searchDateTextField setText:@""];
    }
    NSString* title = [NSString stringWithFormat:@"SEARCH %@S", [eventType uppercaseString]];
    [titleLabel setText:title];
    [categoryButton setHidden:popOnGo];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(BOOL) textFieldShouldReturn:(UITextField *)textField{
    
    [textField resignFirstResponder];
    return YES;
}


-(void) clickGo:(id)sender
{
    NSDate* currentDate = [NSDate date];
    NSDateFormatter* df = [[NSDateFormatter alloc] init];
    [df setDateFormat:@"MM/dd/yyyy"];
    
    NSDate* parseDate = [df dateFromString:[searchDateTextField text]];
    if (parseDate != nil) {
        currentDate = parseDate;
    }
    [[UTCApp sharedInstance] setSearchDate:currentDate];
    
    [[UTCApp sharedInstance] setSearchText:[searchTextField text]];
    if (popOnGo) {
        [[[UTCApp sharedInstance] rootViewController] popActiveView];
    } else {
        [[[UTCApp sharedInstance] rootViewController] openSearchEventsWithType:eventType];
    }
}

-(void) clickCancel:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

-(void) clickCategory:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] openSearchCategories:NO withType:eventType];
}

-(void) clickBack:(id)sender
{
    [[[UTCApp sharedInstance] rootViewController] popActiveView];
}

// choose the date of birth using the action sheet picker
-(void) clickSelectDate:(id)sender {
    NSDate* currentDate = [NSDate date];
    NSDateFormatter* df = [[NSDateFormatter alloc] init];
    [df setDateFormat:@"MM/dd/yyyy"];
    
    NSDate* parseDate = [df dateFromString:[searchDateTextField text]];
    if (parseDate != nil) {
        currentDate = parseDate;
    }
    
    ActionSheetDatePicker* datePicker = [[ActionSheetDatePicker alloc] initWithTitle:@"Event Date"
                                                                      datePickerMode:UIDatePickerModeDate
                                                                        selectedDate:currentDate
                                                                              target:self
                                                                              action:@selector(searchDateSelected:element:)
                                                                              origin:sender];
    [datePicker setHideCancel:NO];
    [datePicker showActionSheetPicker];
}

// callback for the selected date of birth - update the text field
- (void) searchDateSelected:(NSDate *)selectedDate element:(id)element {
    NSDateFormatter* df = [[NSDateFormatter alloc] init];
    [df setDateFormat:@"MM/dd/YYYY"];
    [searchDateTextField setText:[df stringFromDate:selectedDate]];
}


@end
