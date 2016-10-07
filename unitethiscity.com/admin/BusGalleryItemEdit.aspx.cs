/******************************************************************************
 * Filename: BusGalleryItemEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit business gallery items.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Sancsoft.Web;

public partial class admin_BusGalleryItemEdit : System.Web.UI.Page
{
    int id;
    int busID;
    long imageTicks = DateTime.Now.Ticks;
    WebDBContext db = new WebDBContext( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        SubmitButton.Click += SubmitButton_Click;

        // Get the target id
        id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

        // Verify id exists
        if ( id == 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }
        
        // Get the image
        TblGalleryItems rsGal = db.TblGalleryItems.SingleOrDefault( target => target.GalID == id );

        // Verify target record exists
        if ( rsGal == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        busID = rsGal.BusID;

        if ( !Page.IsPostBack )
        {
            // Get the record
            VwBusinesses rs = db.VwBusinesses.SingleOrDefault( target => target.BusID == busID );

            // Verify target record exists
            if ( rs == null )
            {
                throw new WebException( RC.TargetDNE );
            }

            // Populate the page
            BusIDLiteral.Text = rs.BusID.ToString( );
            BusGuidLiteral.Text = rs.BusGuid.ToString( );
            BusNameHyperLink.Text = rs.BusName;
            BusNameHyperLink.ToolTip = "View Business";
            BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rs.BusID.ToString( );
            BusFormalNameLiteral.Text = rs.BusFormalName;

            GalleryImage.ImageUrl = String.Format( "/BusinessGallery/{0}.png?t={1}", rsGal.GalGuid.ToString( ), imageTicks );
            ThumbnailImage.ImageUrl = String.Format( "/BusinessGallery/{0}.thumb.png?t={1}", rsGal.GalGuid.ToString( ), imageTicks );
        }
    }

    void SubmitButton_Click( object sender, EventArgs e )
    {
        bool fileError;
        HttpPostedFileBase imageFile = new HttpPostedFileWrapper( GalleryFileUpload.PostedFile );
        ImageManager.CreateUpdateBusinessGalleryItem( imageFile, busID, id, out fileError );
        if ( fileError )
        {
            throw new WebException( RC.UploadError );
        }

        Response.Redirect( "BusGalleryItems.aspx?ID=" + busID.ToString( ) );
    }
}