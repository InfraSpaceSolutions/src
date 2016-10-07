//
//  TermsViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 5/6/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TermsViewController : GAITrackedViewController
{
}

@property (readwrite) BOOL isSocialDeal;
@property (nonatomic,strong) IBOutlet UIWebView* webView;
@property (nonatomic,strong) IBOutlet UILabel* dealTypeLabel;

-(IBAction) clickBack:(id)sender;

@end
