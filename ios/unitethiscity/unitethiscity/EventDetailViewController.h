//
//  EventDetailViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/8/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface EventDetailViewController : GAITrackedViewController

@property (nonatomic, strong) IBOutlet UILabel* busNameLabel;
@property (nonatomic, strong) IBOutlet UILabel* summaryLabel;
@property (nonatomic, strong) IBOutlet UILabel* eventTypeLabel;
@property (nonatomic, strong) IBOutlet UILabel* dateLabel;
@property (nonatomic, strong) IBOutlet UITextView* bodyView;
@property (nonatomic, strong) IBOutlet UIImageView* busPicture;
@property (nonatomic, strong) IBOutlet UILabel* eventCrumbLabel;
@property (nonatomic, strong) IBOutlet UIButton* moreButton;

-(IBAction) clickBack:(id)sender;

@end
