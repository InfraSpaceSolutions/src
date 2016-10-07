/******************************************************************************
 * Filename: BusLocFavNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new account favorite for this location.
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
using Sancsoft.Web;

public partial class admin_BusLocFavNew : System.Web.UI.Page
{
	int id;
    int locid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        locid = WebConvert.ToInt32(Request.QueryString["LocID"], 0); 

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
            BusIDLiteral.Text = rs.BusID.ToString();
            BusGuidLiteral.Text = rs.BusGuid.ToString();
            BusNameLiteral.Text = rs.BusName;
            BusFormalNameLiteral.Text = rs.BusFormalName;

            // Load the accounts dropdown list
            List<TblAccounts> rsAcc =
                (from acc in db.TblAccounts
                 where !(from fav in db.TblFavorites where fav.LocID == locid select fav.AccID).Contains(acc.AccID)
                 orderby acc.AccEMail
                 select acc).ToList();

            AccIDDropDownList.DataSource = rsAcc;
            AccIDDropDownList.DataBind();
		}
	}

    void SubmitButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        // Create the record
        TblFavorites rs = new TblFavorites();

        // Populate fields
        rs.AccID = WebConvert.ToInt32(AccIDDropDownList.SelectedValue, 0); ;
        rs.LocID = locid;
        rs.FavTS = DateTime.Now;

        // Submit to the db
        db.TblFavorites.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&locID=" + locid.ToString());
    }
}