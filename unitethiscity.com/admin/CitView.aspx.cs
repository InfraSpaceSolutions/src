/******************************************************************************
 * Filename: CitView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an city.
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

public partial class admin_CitView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		EditButton.Click += new EventHandler( EditButton_Click );
		DeleteButton.Click += new EventHandler( DeleteButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // Get the record
        TblCities rs = db.TblCities.SingleOrDefault(target => target.CitID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

		if ( !Page.IsPostBack )
		{
			// Populate the page
            CitIDLiteral.Text = rs.CitID.ToString();
            CitNameLiteral.Text = rs.CitName;
		}
	}

	void DeleteButton_Click( object sender, EventArgs e )
	{
        // DEPENDENCIES
        // are any businesses using this city
        if (db.TblBusinesses.Count(target => target.CitID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }
        // are any accounts using this city
        if (db.TblAccounts.Count(target => target.CitID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

		// Get the record
        TblCities rs = db.TblCities.Single( Target => Target.CitID == id );

		// Get account
        string name = rs.CitName;

		// Delete the record
		db.TblCities.DeleteOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to list page
        Response.Redirect("CitList.aspx?Name=" + name);
	}

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect("CitEdit.aspx?ID=" + id.ToString());
	}
}