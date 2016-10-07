/******************************************************************************
 * Filename: BusGalleryItems.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View business gallery items.
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

public partial class admin_BusGalleryItems : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext( );
    public string ImageTicks = DateTime.Now.Ticks.ToString( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        UploadButton.Click += UploadButton_Click;
        SequenceButton.Click += SequenceButton_Click;
        GalleryItemsRepeater.ItemCommand += new RepeaterCommandEventHandler( GalleryItemsRepeater_ItemCommand );

        // Get the target id
        id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

        // Verify id exists
        if ( id == 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }

        if ( !Page.IsPostBack )
        {
            // Get the record
            VwBusinesses rs = db.VwBusinesses.SingleOrDefault( target => target.BusID == id );

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

            // Get the gallery items and bind to the repeater
            IEnumerable<TblGalleryItems> rsGal = db.TblGalleryItems.Where( rows => rows.BusID == id ).OrderBy( rows => rows.GalSeq );
            NoItemsRow.Visible = ( !rsGal.Any( ) );
            GalleryItemsRepeater.DataSource = rsGal;
            GalleryItemsRepeater.DataBind( );
        }
    }

    void GalleryItemsRepeater_ItemCommand( object source, RepeaterCommandEventArgs e )
    {
        int galID = WebConvert.ToInt32( e.CommandArgument, 0 );

        TblGalleryItems rsGal = db.TblGalleryItems.SingleOrDefault( target => target.GalID == galID );
        if ( rsGal == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        IEnumerable<TblGalleryItems> seqItems = db.TblGalleryItems
            .Where( rows => rows.BusID == id && rows.GalSeq > 1 && rows.GalSeq > rsGal.GalSeq )
            .OrderBy( rows => rows.GalSeq );

        foreach ( var row in seqItems )
        {
            row.GalSeq = row.GalSeq - 1;
        }

        // Delete the file on disk if it exists
        string filePath = SiteSettings.GetValue( "BusinessGalleryPath" ) + rsGal.GalGuid.ToString( ) + ".png";
        if ( File.Exists( filePath ) )
        {
            File.Delete( filePath );
        }

        db.TblGalleryItems.DeleteOnSubmit( rsGal );
        db.SubmitChanges( );

        Response.Redirect( "BusGalleryItems.aspx?ID=" + id.ToString( ) );
    }

    void UploadButton_Click( object sender, EventArgs e )
    {
        bool fileError;
        HttpPostedFileBase imageFile = new HttpPostedFileWrapper( GalleryFileUpload.PostedFile );
        ImageManager.CreateUpdateBusinessGalleryItem( imageFile, id, 0, out fileError );
        if ( fileError )
        {
            throw new WebException( RC.UploadError );
        }

        Response.Redirect( "BusGalleryItems.aspx?ID=" + id.ToString( ) );
    }
    
    void SequenceButton_Click( object sender, EventArgs e )
    {
        Response.Redirect( "BusGalleryItemSequence.aspx?ID=" + id.ToString( ) );
    }
}