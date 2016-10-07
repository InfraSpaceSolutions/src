//
//  BusinessTipViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/7/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface BusinessTipViewController : GAITrackedViewController
{
}

@property (nonatomic,strong) IBOutlet UILabel* reviewerLabel;
@property (nonatomic,strong) IBOutlet UITextView* reviewText;

-(IBAction) clickBack:(id)sender;

@end
