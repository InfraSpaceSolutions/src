//
//  GalleryImageViewController.h
//  unitethiscity
//
//  Created by Michael Terry on 2/13/16.
//  Copyright © 2016 Sanctuary Software Studio, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface GalleryImageViewController : UIViewController

@property (readwrite) int galleryIndex;
@property (nonatomic, strong) NSArray* galleryItems;
@property (nonatomic, strong) IBOutlet UIButton* closeButton;
@property (nonatomic, strong) IBOutlet UIImageView* galleryImageView;

-(IBAction) clickClose:(id)sender;

@end
