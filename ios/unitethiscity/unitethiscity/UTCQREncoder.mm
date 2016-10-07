//
//  UTCQREncoder.m
//  unitethiscity
//
//  Created by Michael Terry on 3/28/13.
//  Copyright (c) 2013 Sanctuary Software Studio, Inc. All rights reserved.
//

#import "UTCQREncoder.h"
#import "QREncoder.h"

@implementation UTCQREncoder

// encode a qrcode image from the supplied string; image is square based on supplied dimension
+(UIImage*) fromString:(NSString*)str withSize:(int)dim;
{
    //first encode the string into a matrix of bools, TRUE for black dot and FALSE for white. Let the encoder decide the error correction level and version
    DataMatrix* qrMatrix = [QREncoder encodeWithECLevel:QR_ECLEVEL_AUTO version:QR_VERSION_AUTO string:str];
    
    //then render the matrix
    UIImage* qrcodeImage = [QREncoder renderDataMatrix:qrMatrix imageDimension:dim];
    
    return qrcodeImage;
}

@end
