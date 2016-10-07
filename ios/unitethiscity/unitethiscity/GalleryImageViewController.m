//
//  GalleryImageViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/13/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCAppDelegate.h"
#import "UTCRootViewController.h"
#import "GalleryImageViewController.h"

@interface GalleryImageViewController ()

@end

@implementation GalleryImageViewController

@synthesize galleryIndex;
@synthesize galleryItems;
@synthesize closeButton;
@synthesize galleryImageView;

- (void)viewDidLoad {
    [super viewDidLoad];

    [self reloadImageView:kNilOptions];
    
    UISwipeGestureRecognizer *swipe;
    
    // by the way, if not using ARC, make sure to add `autorelease` to
    // the following alloc/init statements
    
    swipe = [[UISwipeGestureRecognizer alloc] initWithTarget:self action:@selector(handleSwipe:)];
    swipe.direction = UISwipeGestureRecognizerDirectionLeft;
    [galleryImageView addGestureRecognizer:swipe];
    
    swipe = [[UISwipeGestureRecognizer alloc] initWithTarget:self action:@selector(handleSwipe:)];
    swipe.direction = UISwipeGestureRecognizerDirectionRight;
    [galleryImageView addGestureRecognizer:swipe];
    
    UITapGestureRecognizer *tap = [[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(handleTap:)];
    tap.numberOfTapsRequired = 1;
    [galleryImageView addGestureRecognizer:tap];
    
}

- (void) reloadImageView:(UIViewAnimationOptions)option  {
    NSDictionary* galleryAttr = (NSDictionary*)[galleryItems objectAtIndex:galleryIndex];
    
    NSString* uri = [[UTCApp sharedInstance] galleryImageURI:[galleryAttr objectForKey:@"ImageId"]];
    [UIView transitionWithView:self.view
                      duration:0.5
                       options:option
                    animations:^{
                        [galleryImageView setImageWithURL:[NSURL URLWithString:uri] placeholderImage:[UIImage imageNamed:@"locationPicture.png"]];
                    }
                    completion:nil];
}


- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(IBAction) clickClose:(id)sender {
    [self dismissViewControllerAnimated:NO completion:nil];
}

- (void)handleSwipe:(UISwipeGestureRecognizer *)gesture
{
    UIViewAnimationOptions option = kNilOptions;

    if (gesture.direction == UISwipeGestureRecognizerDirectionRight)
    {
        // if we're at the first card, don't do anything
        
        if (galleryIndex-- == 0) {
            galleryIndex = (int)[galleryItems count] -1;
        }
        option = UIViewAnimationOptionTransitionCurlDown;

    }
    else if (gesture.direction == UISwipeGestureRecognizerDirectionLeft)
    {
        // if we're at the last card, don't do anything
        
        if (++galleryIndex == [galleryItems count]) {
            galleryIndex = 0;
        }
        option = UIViewAnimationOptionTransitionCurlUp;
    }
    
    [self reloadImageView:option];
}

- (void)handleTap:(UISwipeGestureRecognizer *)gesture
{
    if (++galleryIndex >= [galleryItems count]) {
        galleryIndex = 0;
    }
    [self reloadImageView:UIViewAnimationOptionTransitionCurlUp];
}

@end
