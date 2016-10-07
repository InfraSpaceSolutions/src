/******************************************************************************
 * Filename: BusLocNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new business location.
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

public partial class admin_BusLocNew : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

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
            BusIDLiteral.Text = rs.BusID.ToString();
            BusGuidLiteral.Text = rs.BusGuid.ToString();
            BusNameLiteral.Text = rs.BusName;
            BusFormalNameLiteral.Text = rs.BusFormalName;

            // Load the states dropdown list
            List<TblStates> rsStates = (from rows in db.TblStates where rows.CtrID == 186 select rows).ToList();
            StaNameDropDownList.DataSource = rsStates;
            StaNameDropDownList.DataBind();
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
        TblLocations rs = new TblLocations();

        // Populate fields
        rs.BusID = id;
        rs.LocName = WebConvert.Truncate(LocNameTextBox.Text.Trim(), 50);
        rs.LocAddress = WebConvert.Truncate(LocAddressTextBox.Text.Trim(), 50);
        rs.LocCity = WebConvert.Truncate(LocCityTextBox.Text.Trim(), 50);
        rs.LocState = WebConvert.ToString(StaNameDropDownList.SelectedValue, "");
        rs.LocZip = WebConvert.Truncate(LocZipTextBox.Text.Trim(), 50);
        rs.LocPhone = Phone.Clean(LocPhoneTextBox.Text);
        rs.LocLatitude = WebConvert.ToDouble( LocLatitudeTextBox.Text.Trim(), 0 );
        rs.LocLongitude = WebConvert.ToDouble( LocLongitudeTextBox.Text.Trim(), 0 );

        // Submit to the db
        db.TblLocations.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to the view page
        Response.Redirect("BusLocView.aspx?ID=" + rs.BusID.ToString() + "&locID=" + rs.LocID.ToString());
    }
}