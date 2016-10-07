/******************************************************************************
 * Project:  unitethiscity.com/App_Code
 * 
 * Revision History:
 * $Log: $
 * 
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Sancsoft.Web;

/// <summary>
/// Image manager class for uploaded images
/// </summary>
public class ImageManager
{
    // business logos/images
    const int LARGE_LOGO_WIDTH = 200;
    const int LARGE_LOGO_HEIGHT = 200;
    const int SMALL_LOGO_WIDTH = 100;
    const int SMALL_LOGO_HEIGHT = 100;

    // business gallery items
    const int GALLERY_ITEM_MAX_WIDTH = 1024;
    const int GALLERY_THUMB_WIDTH_HEIGHT = 160;

    public static void CreateUpdateBusinessImage( FileUpload imageFile, int busID, out bool fileError )
    {
        fileError = false;

        // Make sure there is a file
        if ( !imageFile.HasFile )
        {
            return;
        }

        // Validate for image/* file types only
        if ( !imageFile.PostedFile.ContentType.Contains( "image/" ) )
        {
            // Log the error
            fileError = true;
            return;
        }

        WebDBContext db = new WebDBContext( );

        // Create/update the small image
        CreateImageFile( db, imageFile, SMALL_LOGO_WIDTH, busID, out fileError );
        CreateImageFile( db, imageFile, LARGE_LOGO_WIDTH, busID, out fileError );
    }

    public static bool CreateDefaultBusinessImage( int busID, out bool fileError )
    {
        fileError = false;

        WebDBContext db = new WebDBContext( );

        // get the businesses id
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault( target => target.BusID == busID );
        if ( rsBus == null )
        {
            fileError = true;
            return fileError;
        }

        // Create the default small/large image
        string defaultLargeFilePath = Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), "default@2x.png" );
        string defaultSmallFilePath = Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), "default.png" );

        // Remove the current files
        File.Delete( Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), rsBus.BusGuid + "@2x.png" ) );
        File.Delete( Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), rsBus.BusGuid + ".png" ) );

        File.Copy( defaultLargeFilePath, Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), rsBus.BusGuid + "@2x.png" ) );
        File.Copy( defaultSmallFilePath, Path.Combine( SiteSettings.GetValue( "BusinessImagesPath" ), rsBus.BusGuid + ".png" ) );

        return fileError;
    }

    static bool CreateImageFile( WebDBContext db, FileUpload imageFile, int imgWidth, int busID, out bool fileError )
    {
        bool logosmall = false;

        // get the businesses id
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault( target => target.BusID == busID );
        if ( rsBus == null )
        {
            fileError = true;
            return fileError;
        }

        // creating a small logo
        if ( imgWidth == SMALL_LOGO_WIDTH )
        {
            logosmall = true;
        }
        else
        {
            // large logo
            logosmall = false;
        }

        // Get a clean content type
        string contentType = NormalizedContentType( imageFile.PostedFile.ContentType );
        string filename = Path.GetFileName( imageFile.FileName );
        string fileextension = Path.GetExtension( imageFile.FileName );

        // Create the image stream
        Stream imgStream = imageFile.PostedFile.InputStream;

        // Create an image object from the stream
        System.Drawing.Image img = System.Drawing.Image.FromStream( imgStream );

        // Determine the type of scaling based on the image orientation
        int datWidth = imgWidth;
        // Calculate the HEIGHT of the image
        int datHeight = img.Height * datWidth / img.Width;

        // Create the new RGB bitmap with the calculated width and height
        Bitmap bmp = new Bitmap( datWidth, datHeight, PixelFormat.Format24bppRgb );

        // Anti-alias formatting
        Graphics grPhoto = Graphics.FromImage( bmp );
        grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
        grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
        grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
        grPhoto.DrawImage( img, new Rectangle( 0, 0, datWidth, datHeight ) );

        // Set up the encoding info
        ImageCodecInfo bmpCodec = GetEncoderInfo( contentType );
        Encoder bmpEncoder = Encoder.Quality;
        EncoderParameters bmpParams = new EncoderParameters( 1 );
        EncoderParameter bmpParam = new EncoderParameter( bmpEncoder, 80L );
        bmpParams.Param[0] = bmpParam;

        string filePath = "";

        // Save the file to disk
        if ( logosmall )
        {
            // small logo do not add the @2x
            filePath = SiteSettings.GetValue( "BusinessImagesPath" ) + rsBus.BusGuid.ToString( ) + ".png";
        }
        else
        {
            // add the @2x to large logo filenames
            filePath = SiteSettings.GetValue( "BusinessImagesPath" ) + rsBus.BusGuid.ToString( ) + "@2x.png";
        }

        bmp.Save( filePath, bmpCodec, bmpParams );

        // Release the bitmap from memory
        bmp.Dispose( );

        // Release the image from memory
        img.Dispose( );

        // Release the graphic from memory
        grPhoto.Dispose( );

        fileError = false;
        return fileError;
    }

    public static void CreateUpdateBusinessGalleryItem( HttpPostedFileBase imageFile, int busID, int galID, out bool fileError )
    {
        fileError = false;

        // Make sure there is a file
        if ( imageFile.ContentLength <= 0 )
        {
            return;
        }

        // Validate for image/* file types only
        if ( !imageFile.ContentType.Contains( "image/" ) )
        {
            // Log the error
            fileError = true;
            return;
        }

        WebDBContext db = new WebDBContext( );

        // Create/update the gallery item
        CreateUpdateGalleryItemFile( db, imageFile, busID, galID, out fileError );
    }

    static bool CreateUpdateGalleryItemFile( WebDBContext db, HttpPostedFileBase imageFile, int busID, int galID, out bool fileError )
    {
        // get/create the gallery item
        TblGalleryItems rsGal = db.TblGalleryItems.SingleOrDefault( target => target.BusID == busID && target.GalID == galID );
        if ( rsGal == null )
        {
            // Get the next sequence
            var maxSeq = db.TblGalleryItems.Where( target => target.BusID == busID );
            int galSeq = 1;
            if ( maxSeq.Any( ) )
            {
                galSeq += maxSeq.Max( target => target.GalSeq );
            }
            // Create the item
            rsGal = new TblGalleryItems( );
            rsGal.BusID = busID;
            rsGal.GalSeq = galSeq;
            rsGal.GalGuid = Guid.NewGuid( );

            // Sync to the database
            db.TblGalleryItems.InsertOnSubmit( rsGal );
            db.SubmitChanges( );
        }

        // Create the image stream
        Stream imgStream = imageFile.InputStream;

        // Create an image object from the stream
        System.Drawing.Image img = System.Drawing.Image.FromStream( imgStream );

        // Default the width and height
        int datWidth = img.Width;
        int datHeight = img.Height;

        // Check the max width
        if ( datWidth > GALLERY_ITEM_MAX_WIDTH )
        {
            // Calculate the HEIGHT of the image
            datWidth = GALLERY_ITEM_MAX_WIDTH;
            datHeight = img.Height * datWidth / img.Width;
        }

        // Create the new RGB bitmap with the calculated width and height
        Bitmap bmp = new Bitmap( datWidth, datHeight, PixelFormat.Format24bppRgb );

        // Anti-alias formatting
        Graphics grPhoto = Graphics.FromImage( bmp );
        grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
        grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
        grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
        grPhoto.DrawImage( img, new Rectangle( 0, 0, datWidth, datHeight ) );

        // Set up the encoding info
        ImageCodecInfo bmpCodec = GetEncoderInfo( "image/png" );
        Encoder bmpEncoder = Encoder.Quality;
        EncoderParameters bmpParams = new EncoderParameters( 1 );
        EncoderParameter bmpParam = new EncoderParameter( bmpEncoder, 80L );
        bmpParams.Param[0] = bmpParam;

        // Create the filepath
        string baseFilePath = SiteSettings.GetValue( "BusinessGalleryPath" );
        string fileName = String.Format( "{0}.png", rsGal.GalGuid.ToString( ) );
        string filePath = String.Format( "{0}{1}", baseFilePath, fileName );

        // Save the large image to disk
        bmp.Save( filePath, bmpCodec, bmpParams );

        //
        //== Create/update the thumbnail
        //

        // Thumbnail canvas size
        int thumbCanvasWidth = GALLERY_THUMB_WIDTH_HEIGHT;
        int thumbCanvasHeight = thumbCanvasWidth;

        // Default the thumbnail size
        int thumbWidth = datWidth;
        int thumbHeight = datHeight;

        // Check the max width
        if ( thumbWidth > thumbCanvasWidth )
        {
            // Calculate the HEIGHT of the thumbnail image
            thumbWidth = thumbCanvasWidth;
            thumbHeight = datHeight * thumbWidth / datWidth;
        }

        // Check the calculated height
        if ( thumbHeight < thumbCanvasHeight )
        {
            // Calculating height instead
            // Calculate the WIDTH of the thumbnail image
            thumbHeight = thumbCanvasHeight;
            thumbWidth = datWidth * thumbHeight / datHeight;
        }

        // Calculate the x/y centers
        int canvasX = ( thumbCanvasWidth / 2 );
        int canvasY = ( thumbCanvasHeight / 2 );
        int thumbX = ( thumbWidth / 2 );
        int thumbY = ( thumbHeight / 2 );
        
        // Calculate the drawn X axis
        int thumbDrawX = 0;
        if ( thumbCanvasWidth > thumbWidth )
        {
            thumbDrawX = ( canvasX > thumbX ) ? canvasX - thumbX : thumbX - canvasX;
        }
        else
        {
            // Negative
            thumbDrawX = -( ( canvasX > thumbX ) ? canvasX - thumbX : thumbX - canvasX );
        }

        // Calculate the drawn Y axis
        int thumbDrawY = 0;
        if ( thumbCanvasHeight > thumbHeight )
        {
            thumbDrawY = ( canvasY > thumbY ) ? canvasY - thumbY : thumbY - canvasY;
        }
        else
        {
            // Negative
            thumbDrawY = -( ( canvasY > thumbY ) ? canvasY - thumbY : thumbY - canvasY );
        }

        // Create the thumbnail bitmap
        Bitmap thumbBmp = new Bitmap( thumbCanvasWidth, thumbCanvasHeight, PixelFormat.Format24bppRgb );

        // Anti-alias formatting
        Graphics thumbGrPhoto = Graphics.FromImage( thumbBmp );
        thumbGrPhoto.SmoothingMode = SmoothingMode.AntiAlias;
        thumbGrPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
        thumbGrPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
        thumbGrPhoto.DrawImage( img, new Rectangle( thumbDrawX, thumbDrawY, thumbWidth, thumbHeight ) );

        // Thumbnail filename and path
        string thumbFileName = String.Format( "{0}.thumb.png", rsGal.GalGuid.ToString( ) );
        string thumbFilePath = String.Format( "{0}{1}", baseFilePath, thumbFileName );

        // Save the thumbnail to disk
        thumbBmp.Save( thumbFilePath, bmpCodec, bmpParams );

        // Release the bitmaps from memory
        bmp.Dispose( );
        thumbBmp.Dispose( );

        // Release the graphics from memory
        grPhoto.Dispose( );
        thumbGrPhoto.Dispose( );

        // Release the image from memory
        img.Dispose( );

        fileError = false;
        return fileError;
    }

    static string NormalizedContentType( string contentTypeIn )
    {
        string contentTypeOut = contentTypeIn;

        // translate mime types to standard encoders
        switch ( contentTypeIn )
        {
            case "image/pjpeg":
                contentTypeOut = "image/jpeg";
                break;
            case "image/x-png":
                contentTypeOut = "image/png";
                break;
        }

        return contentTypeOut;
    }

    static ImageCodecInfo GetEncoderInfo( string mimeType )
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders( );
        for ( j = 0; j < encoders.Length; ++j )
        {
            if ( encoders[j].MimeType == mimeType )
            {
                return encoders[j];
            }
        }
        return null;
    }

    public static void CalculateImageDimensions( int targetWidth, int targetHeight, int imageWidth, int imageHeight, out int calcWidth, out int calcHeight )
    {
        int datWidth = 0;
        int datHeight = 0;
        if ( imageWidth >= imageHeight )
        {
            // Landscape image
            datWidth = targetWidth;
        }
        else
        {
            // Portrait image
            datHeight = targetHeight;
        }

        if ( datWidth != 0 )
        {
            // Calculate the new HEIGHT of the image
            datHeight = imageHeight * datWidth / imageWidth;
        }

        if ( datWidth == 0 )
        {
            // Calculate the new WIDTH of the image
            datWidth = imageWidth * datHeight / imageHeight;
        }


        calcWidth = datWidth;
        calcHeight = datHeight;
    }
}