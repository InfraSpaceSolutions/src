/******************************************************************************
 * Filename: BusMenuItemEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit business menu items.
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

public partial class admin_BusMenuItemEdit : System.Web.UI.Page
{
    int id;
    int busID;
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
        TblMenuItems rsMen = db.TblMenuItems.SingleOrDefault( target => target.MenID == id );

        // Verify target record exists
        if ( rsMen == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        busID = rsMen.BusID;

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

            MenNameTextBox.Text = rsMen.MenName;
            MenPriceTextBox.Text = rsMen.MenPrice.ToString( "N2" );
        }
    }

    void SubmitButton_Click( object sender, EventArgs e )
    {
        // Create the new item
        SiteMenuItem siteMenuItem = new SiteMenuItem( id );
        siteMenuItem.MenName = WebConvert.Truncate( MenNameTextBox.Text, 80 );
        siteMenuItem.MenPrice = WebConvert.ToDecimal( MenPriceTextBox.Text, 0 );
        siteMenuItem.SaveChanges( );

        Response.Redirect( "BusMenuItems.aspx?ID=" + busID.ToString( ) );
    }
}