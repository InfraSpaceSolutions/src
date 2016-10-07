/******************************************************************************
 * Filename: BusMenuItemSequence.aspx.cs
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

public partial class admin_BusMenuItemSequence : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        DoneButton.Click += DoneButton_Click;
        MenuItemsRepeater.ItemDataBound += new RepeaterItemEventHandler( MenuItemsRepeater_ItemDataBound );

        // Get the target id
        id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

        // Verify id exists
        if ( id == 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }

        // Get the gallery items and bind to the repeater
        IEnumerable<TblMenuItems> rsMen = db.TblMenuItems.Where( rows => rows.BusID == id ).OrderBy( rows => rows.MenSeq );
        NoItemsRow.Visible = ( !rsMen.Any( ) );
        MenuItemsRepeater.DataSource = rsMen;
        MenuItemsRepeater.DataBind( );

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

    void MenuItemsRepeater_ItemDataBound( object sender, RepeaterItemEventArgs e )
    {
        // Get sequence link button controls
        LinkButton upSequence = (LinkButton)e.Item.FindControl( "UpSequenceLinkButton" );
        LinkButton downSequence = (LinkButton)e.Item.FindControl( "DownSequenceLinkButton" );

        // Get underlying datasource
        TblMenuItems rs = (TblMenuItems)e.Item.DataItem;

        // Wire sequence link button controls
        upSequence.CommandArgument = rs.MenID.ToString( );
        upSequence.Command += new CommandEventHandler( upSequence_Command );
        downSequence.CommandArgument = rs.MenID.ToString( );
        downSequence.Command += new CommandEventHandler( downSequence_Command );
    }

    void downSequence_Command( object sender, CommandEventArgs e )
    {
        TblMenuItems rsSwap = null;

        // Get target page record
        TblMenuItems rs = db.TblMenuItems.SingleOrDefault( target => target.MenID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

        // Verify record exists
        if ( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Get list of all children pages for swap
        List<TblMenuItems> items = db.TblMenuItems.Where( target => target.BusID == id ).OrderBy( target => target.MenSeq ).ToList( );

        // Move the selected page down in sequence
        if ( rs.MenSeq < items.Count )
        {
            // Get the item to swap with
            rsSwap = db.TblMenuItems.SingleOrDefault( target => target.MenID == items[rs.MenSeq].MenID );
        }

        if ( rsSwap != null )
        {
            // Swap the target with the one below it
            int temp = rs.MenSeq;
            rs.MenSeq = rsSwap.MenSeq;
            rsSwap.MenSeq = temp;

            // Sync to database
            db.SubmitChanges( );
        }

        Response.Redirect( "BusMenuItemSequence.aspx?ID=" + id.ToString( ) );
    }

    void upSequence_Command( object sender, CommandEventArgs e )
    {
        TblMenuItems rsSwap = null;

        // Get the target page record
        TblMenuItems rs = db.TblMenuItems.SingleOrDefault( target => target.MenID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

        // Verify record exists
        if ( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Create list of all children pages
        List<TblMenuItems> items = db.TblMenuItems.Where( target => target.BusID == id ).OrderBy( target => target.MenSeq ).ToList( );

        // Move the page up
        if ( rs.MenSeq > 1 )
        {
            // Swap with the next page
            rsSwap = db.TblMenuItems.SingleOrDefault( target => target.MenID == items[rs.MenSeq - 2].MenID );
        }

        if ( rsSwap != null )
        {
            // Swap the target page with the one above it
            int temp = rs.MenSeq;
            rs.MenSeq = rsSwap.MenSeq;
            rsSwap.MenSeq = temp;

            // Sync to database
            db.SubmitChanges( );
        }

        Response.Redirect( "BusMenuItemSequence.aspx?ID=" + id.ToString( ) );
    }

    void DoneButton_Click( object sender, EventArgs e )
    {
        Response.Redirect( "BusMenuItems.aspx?ID=" + id.ToString( ) );
    }
}