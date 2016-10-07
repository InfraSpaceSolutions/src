/******************************************************************************
 * Filename: BusGalleryItemSequence.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Sequence business gallery items.
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

public partial class admin_BusGalleryItemSequence : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        DoneButton.Click += DoneButton_Click;
        GalleryItemsRepeater.ItemDataBound += new RepeaterItemEventHandler( GalleryItemsRepeater_ItemDataBound );

        // Get the target id
        id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

        // Verify id exists
        if ( id == 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }

        // Get the gallery items and bind to the repeater
        IEnumerable<TblGalleryItems> rsGal = db.TblGalleryItems.Where( rows => rows.BusID == id ).OrderBy( rows => rows.GalSeq );
        NoItemsRow.Visible = ( !rsGal.Any( ) );
        GalleryItemsRepeater.DataSource = rsGal;
        GalleryItemsRepeater.DataBind( );

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
        }
    }

    void GalleryItemsRepeater_ItemDataBound( object sender, RepeaterItemEventArgs e )
    {
        // Get sequence link button controls
        LinkButton upSequence = (LinkButton)e.Item.FindControl( "UpSequenceLinkButton" );
        LinkButton downSequence = (LinkButton)e.Item.FindControl( "DownSequenceLinkButton" );

        // Get underlying datasource
        TblGalleryItems rs = (TblGalleryItems)e.Item.DataItem;

        // Wire sequence link button controls
        upSequence.CommandArgument = rs.GalID.ToString( );
        upSequence.Command += new CommandEventHandler( upSequence_Command );
        downSequence.CommandArgument = rs.GalID.ToString( );
        downSequence.Command += new CommandEventHandler( downSequence_Command );
    }

    void downSequence_Command( object sender, CommandEventArgs e )
    {
        TblGalleryItems rsSwap = null;

        // Get target page record
        TblGalleryItems rs = db.TblGalleryItems.SingleOrDefault( target => target.GalID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

        // Verify record exists
        if ( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Get list of all children pages for swap
        List<TblGalleryItems> items = db.TblGalleryItems.Where( target => target.BusID == id ).OrderBy( target => target.GalSeq ).ToList( );

        // Move the selected page down in sequence
        if ( rs.GalSeq < items.Count )
        {
            // Get the item to swap with
            rsSwap = db.TblGalleryItems.SingleOrDefault( target => target.GalID == items[rs.GalSeq].GalID );
        }

        if ( rsSwap != null )
        {
            // Swap the target with the one below it
            int temp = rs.GalSeq;
            rs.GalSeq = rsSwap.GalSeq;
            rsSwap.GalSeq = temp;

            // Sync to database
            db.SubmitChanges( );
        }

        Response.Redirect( "BusGalleryItemSequence.aspx?ID=" + id.ToString( ) );
    }

    void upSequence_Command( object sender, CommandEventArgs e )
    {
        TblGalleryItems rsSwap = null;

        // Get the target page record
        TblGalleryItems rs = db.TblGalleryItems.SingleOrDefault( target => target.GalID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

        // Verify record exists
        if ( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Create list of all children pages
        List<TblGalleryItems> items = db.TblGalleryItems.Where( target => target.BusID == id ).OrderBy( target => target.GalSeq ).ToList( );

        // Move the page up
        if ( rs.GalSeq > 1 )
        {
            // Swap with the next page
            rsSwap = db.TblGalleryItems.SingleOrDefault( target => target.GalID == items[rs.GalSeq - 2].GalID );
        }

        if ( rsSwap != null )
        {
            // Swap the target page with the one above it
            int temp = rs.GalSeq;
            rs.GalSeq = rsSwap.GalSeq;
            rsSwap.GalSeq = temp;

            // Sync to database
            db.SubmitChanges( );
        }

        Response.Redirect( "BusGalleryItemSequence.aspx?ID=" + id.ToString( ) );
    }

    void DoneButton_Click( object sender, EventArgs e )
    {
        Response.Redirect( "BusGalleryItems.aspx?ID=" + id.ToString( ) );
    }
}