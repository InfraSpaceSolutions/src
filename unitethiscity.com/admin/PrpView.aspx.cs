/******************************************************************************
 * Filename: PrpView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an property.
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

public partial class admin_PrpView : System.Web.UI.Page
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
        TblProperties rs = db.TblProperties.SingleOrDefault(target => target.PrpID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

		if ( !Page.IsPostBack )
		{
			// Populate the page
            PrpIDLiteral.Text = rs.PrpID.ToString();
            PrpNameLiteral.Text = rs.PrpName;
            PrpDescriptionLiteral.Text = WebConvert.PreserveBreaks(rs.PrpDescription);
		}
	}

	void DeleteButton_Click( object sender, EventArgs e )
	{
        // DEPENDENCIES
        // are any businesses using this property
        if (db.TblBusinessProperties.Count(target => target.PrpID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

        // are any locations using this propery
        if (db.TblLocationProperties.Count(target => target.PrpID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }
		// Get the record
        TblProperties rs = db.TblProperties.Single( Target => Target.PrpID == id );

		// Get account
        string name = rs.PrpName;

		// Delete the record
		db.TblProperties.DeleteOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to list page
        Response.Redirect("PrpList.aspx?Name=" + name);
	}

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect("PrpEdit.aspx?ID=" + id.ToString());
	}
}