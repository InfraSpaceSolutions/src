/******************************************************************************
 * Filename: BusDldNew.aspx.cs
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

public partial class admin_BusDldNew : System.Web.UI.Page
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
        TblDealDefinitions rs = new TblDealDefinitions();

        // Populate fields
        rs.BusID = id;
        rs.DldName =  WebConvert.Truncate( DldNameTextBox.Text.Trim(), 50 );
        rs.DldAmount = WebConvert.ToDecimal(DldAmountTextBox.Text.Trim(), 0);
        rs.DldDescription = WebConvert.Truncate(DldDescriptionTextBox.Text.Trim(), 255);
        rs.DldCustomTerms = WebConvert.Truncate(DldCustomTermsTextBox.Text.Trim(), 255);

        // Submit to the db
        db.TblDealDefinitions.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}