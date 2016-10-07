//
//  FavoriteViewController.m
//  unitethiscity
//
//  Created by Michael Terry on 2/3/13.
//  Copyright (c) 2013 Sanctuary Software Studio, INc. All rights reserved.
//

#import "FavoriteViewController.h"
#import "FavoriteListViewController.h"

@interface FavoriteViewController ()

@end

@implementation FavoriteViewController

@synthesize navController;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        UIViewController* rootOfNav = [[FavoriteListViewController alloc] initWithNibName:@"FavoriteListViewController" bundle:nil];
        navController = [[UINavigationController alloc] initWithRootViewController:rootOfNav];
        [rootOfNav release];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
