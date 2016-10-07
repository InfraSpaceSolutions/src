//
//  InboxDetailViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/10/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface InboxDetailViewController : GAITrackedViewController
{
}

@property (nonatomic,strong) IBOutlet UILabel* subjectLabel;
@property (nonatomic,strong) IBOutlet UIWebView* webView;
@property (nonatomic,strong) IBOutlet UIButton* deleteCrumbButton;

-(IBAction) clickBack:(id)sender;

@end
