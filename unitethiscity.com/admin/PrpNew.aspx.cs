/******************************************************************************
 * Filename: PrpNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new property.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_PrpNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

		// Create the record
		TblProperties rs = new TblProperties();

		// Populate fields
        rs.PrpName = WebConvert.Truncate( PrpNameTextBox.Text.Trim( ), 50 );
        rs.PrpDescription = PrpDescriptionTextBox.Text.Trim();

		// Submit to the db
		db.TblProperties.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect("PrpView.aspx?ID=" + rs.PrpID.ToString());
	}
}