/******************************************************************************
 * Filename: BusMenuItems.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View business menu items.
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

public partial class admin_BusMenuItems : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        SubmitButton.Click += SubmitButton_Click;
        SequenceButton.Click += SequenceButton_Click;
        MenuItemsRepeater.ItemCommand += new RepeaterCommandEventHandler( MenuItemsRepeater_ItemCommand );

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
            IEnumerable<TblMenuItems> rsMen = db.TblMenuItems.Where( rows => rows.BusID == id ).OrderBy( rows => rows.MenSeq );
            NoItemsRow.Visible = ( !rsMen.Any( ) );
            MenuItemsRepeater.DataSource = rsMen;
            MenuItemsRepeater.DataBind( );
        }
    }

    void SubmitButton_Click( object sender, EventArgs e )
    {
        // Create the new item
        SiteMenuItem siteMenuItem = new SiteMenuItem( );
        siteMenuItem.BusID = id;
        siteMenuItem.MenName = WebConvert.Truncate( MenNameTextBox.Text, 80 );
        siteMenuItem.MenPrice = WebConvert.ToDecimal( MenPriceTextBox.Text, 0 );
        siteMenuItem.SaveChanges( );

        Response.Redirect( "BusMenuItems.aspx?ID=" + id.ToString( ) );
    }

    void MenuItemsRepeater_ItemCommand( object source, RepeaterCommandEventArgs e )
    {
        int menID = WebConvert.ToInt32( e.CommandArgument, 0 );

        TblMenuItems rsMen = db.TblMenuItems.SingleOrDefault( target => target.MenID == menID );
        if ( rsMen == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        IEnumerable<TblMenuItems> seqItems = db.TblMenuItems
            .Where( rows => rows.BusID == id && rows.MenSeq > 1 && rows.MenSeq > rsMen.MenSeq )
            .OrderBy( rows => rows.MenSeq );

        foreach ( var row in seqItems )
        {
            row.MenSeq = row.MenSeq - 1;
        }

        db.TblMenuItems.DeleteOnSubmit( rsMen );
        db.SubmitChanges( );

        Response.Redirect( "BusMenuItems.aspx?ID=" + id.ToString( ) );
    }
    
    void SequenceButton_Click( object sender, EventArgs e )
    {
        Response.Redirect( "BusMenuItemSequence.aspx?ID=" + id.ToString( ) );
    }
}