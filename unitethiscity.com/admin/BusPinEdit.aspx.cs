/******************************************************************************
 * Filename: BusPinEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a business.
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

public partial class admin_BusPinEdit : System.Web.UI.Page
{
	int id;
    int pinid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target bus id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        // Get the target dldID to update
        pinid = WebConvert.ToInt32(Request.QueryString["PinID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }
        // Verify dldid exists
        if (pinid == 0)
        {
            throw new WebException(RC.DataIncomplete);
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

            // get the target deal definition
            TblPins rsPin = db.TblPins.SingleOrDefault(target => target.PinID == pinid);

            // Verify target records exits
            if (rsPin == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            PinNumberTextBox.Text = rsPin.PinNumber;
            PinNameTextBox.Text = rsPin.PinName;
            PinEnabledDropDownList.SelectedValue = ( rsPin.PinEnabled ) ? "True" : "False";
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
        TblPins rs = db.TblPins.SingleOrDefault( target => target.BusID == id && target.PinID == pinid );
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Populate fields
        rs.PinNumber = WebConvert.Truncate(PinNumberTextBox.Text.Trim(), 8);
        rs.PinName = WebConvert.Truncate(PinNameTextBox.Text.Trim(), 50);
        rs.PinEnabled = WebConvert.ToBoolean(PinEnabledDropDownList.SelectedValue, false);

        // Submit to the db
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}