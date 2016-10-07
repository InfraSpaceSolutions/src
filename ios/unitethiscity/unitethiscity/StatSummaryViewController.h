//
//  StatSummaryViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/4/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface StatSummaryViewController : GAITrackedViewController
{
}

@property (nonatomic, strong) IBOutlet UIScrollView* scrollView;
@property (nonatomic, strong) IBOutlet UIView* contentView;
@property (nonatomic, strong) IBOutlet UIView* backgroundView;
@property (nonatomic, strong) IBOutlet UILabel* businessNameLabel;

@property (nonatomic, strong) IBOutlet UILabel* summaryTitleLabel;
@property (nonatomic, strong) IBOutlet UILabel* socialPointsLabel;
@property (nonatomic, strong) IBOutlet UILabel* loyaltyPointsLabel;
@property (nonatomic, strong) IBOutlet UILabel* favoritesLabel;
@property (nonatomic, strong) IBOutlet UILabel* ratingsLabel;
@property (nonatomic, strong) IBOutlet UILabel* reviewsLabel;

-(IBAction) clickSocialDeals:(id)sender;
-(IBAction) clickLoyaltyDeals:(id)sender;
-(IBAction) clickFavorites:(id)sender;
-(IBAction) clickRatings:(id)sender;
-(IBAction) clickReviews:(id)sender;
-(IBAction) clickAnalytics:(id)sender;
@end
