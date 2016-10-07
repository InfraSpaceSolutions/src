//
//  RatingViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 12/27/15.
//  Copyright © 2015 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCApp.h"
#import "UTCAPIClient.h"
#import "UTCRootViewController.h"
#import "TutorialViewController.h"
#import "MediaPlayer/MediaPlayer.h"

#define NUM_TUTORIAL_IMAGES         8

@interface TutorialViewController ()

@property (nonatomic) NSInteger imageIndex;

@end

@implementation TutorialViewController

@synthesize currentImage;
@synthesize animatedImage;
@synthesize closeButton;
@synthesize watchVideoButton;
@synthesize imageIndex;


- (void)viewDidLoad {
    [super viewDidLoad];
    
    UISwipeGestureRecognizer *swipe;
    
    // by the way, if not using ARC, make sure to add `autorelease` to
    // the following alloc/init statements
    
    swipe = [[UISwipeGestureRecognizer alloc] initWithTarget:self action:@selector(handleSwipe:)];
    swipe.direction = UISwipeGestureRecognizerDirectionLeft;
    [currentImage addGestureRecognizer:swipe];
    
    swipe = [[UISwipeGestureRecognizer alloc] initWithTarget:self action:@selector(handleSwipe:)];
    swipe.direction = UISwipeGestureRecognizerDirectionRight;
    [currentImage addGestureRecognizer:swipe];
    
    UITapGestureRecognizer *tap = [[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(handleTap:)];
    tap.numberOfTapsRequired = 1;
    [currentImage addGestureRecognizer:tap];
    
    imageIndex = 1;
    [self updateImageDisplayed:kNilOptions];
}

- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) clickWatchVideo:(id)sender {
    // pick a video from the documents directory
    NSURL *video = [[NSURL alloc] initWithString:@"http://www.unitethiscity.com/video/tutorial-video.mp4"];
    
    // create a movie player view controller
    MPMoviePlayerViewController * controller = [[MPMoviePlayerViewController alloc]initWithContentURL:video];
    [controller.moviePlayer prepareToPlay];
    [controller.moviePlayer play];
    
    // and present it
    [self presentMoviePlayerViewControllerAnimated:controller];
}


-(void) clickClose:(id)sender {
    [[UTCApp sharedInstance] markTutorialShown];
    [self dismissViewControllerAnimated:NO completion:nil];
}

-(void) clickNext:(id)sender {
}

- (void)updateImageDisplayed:(UIViewAnimationOptions)option {
    NSString* imageName = [NSString stringWithFormat:@"tutorial-%d", (int)imageIndex];
    [UIView transitionWithView:self.view
                      duration:0.5
                       options:option
                    animations:^{
                        currentImage.image = [UIImage imageNamed:imageName];
                    }
                    completion:nil];
    
    [watchVideoButton setHidden:(imageIndex != NUM_TUTORIAL_IMAGES)];
}

- (void)handleSwipe:(UISwipeGestureRecognizer *)gesture
{
    UIViewAnimationOptions option = kNilOptions;
    
    if (gesture.direction == UISwipeGestureRecognizerDirectionRight)
    {
        // if we're at the first card, don't do anything
        
        if (imageIndex == 1)
            return;
        
        // if swiping to the right, maybe it's like putting a card
        // back on the top of the deck of cards
        
        option = UIViewAnimationOptionTransitionCurlDown;
        
        // adjust the index of the next card to be shown
        
        self.imageIndex--;
    }
    else if (gesture.direction == UISwipeGestureRecognizerDirectionLeft)
    {
        // if we're at the last card, don't do anything
        
        if (imageIndex == NUM_TUTORIAL_IMAGES)
            return;
        
        // if swiping to the left, it's like pulling a card off the
        // top of the deck of cards
        
        option = UIViewAnimationOptionTransitionCurlUp;
        
        // adjust the index of the next card to be shown
        
        imageIndex++;
    }
    
    // now animate going to the next card; the view you apply the
    // animation to is the container view that is holding the image
    // view. In this example, I have the image view on the main view,
    // but if you want to constrain the animation to only a portion of
    // the screen, you'd define a simple `UIView` that is the dimensions
    // that you want to animate and then put the image view inside
    // that view, and replace the `self.view` reference below with the
    // view that contains the image view.
    [self updateImageDisplayed:option];
}

- (void)handleTap:(UISwipeGestureRecognizer *)gesture
{
    if (imageIndex == NUM_TUTORIAL_IMAGES)
        return;
    imageIndex += 1;
    
    [self updateImageDisplayed:UIViewAnimationOptionTransitionCurlUp];
}
@end
