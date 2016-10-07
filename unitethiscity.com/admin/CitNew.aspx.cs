/******************************************************************************
 * Filename: CitNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new city.
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

public partial class admin_CitNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        CitNameDuplicate.ServerValidate += new ServerValidateEventHandler(CitNameDuplicate_ServerValidate);
	}

    void CitNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (db.TblCities.Count(target => target.CitName == args.Value.Trim()) == 0);
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

		// Create the record
		TblCities rs = new TblCities();

		// Populate fields
        rs.CitName = WebConvert.Truncate(CitNameTextBox.Text.Trim(), 50);

		// Submit to the db
		db.TblCities.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect("CitView.aspx?ID=" + rs.CitID.ToString());
	}
}