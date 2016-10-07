/******************************************************************************
 * Filename: CitEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an city.
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

public partial class admin_CitEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);
        CitNameDuplicate.ServerValidate += new ServerValidateEventHandler(CitNameDuplicate_ServerValidate);

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		if ( !Page.IsPostBack )
		{
			// Get the target record
            TblCities rs = db.TblCities.SingleOrDefault(target => target.CitID == id);

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            CitIDLiteral.Text = rs.CitID.ToString();
            CitNameTextBox.Text = rs.CitName;
		}
	}

    void CitNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Check for duplicate city name
        args.IsValid = (db.TblCities.Count(target => target.CitName == args.Value.Trim()) == 0);
    }

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

		// Get the record
        TblCities rs = db.TblCities.SingleOrDefault(target => target.CitID == id);

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Update the record
        rs.CitName = WebConvert.Truncate(CitNameTextBox.Text.Trim(), 50);

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
        Response.Redirect("CitView.aspx?ID=" + id.ToString());
	}
}