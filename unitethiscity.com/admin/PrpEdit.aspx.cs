/******************************************************************************
 * Filename: PrpEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an property.
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

public partial class admin_PrpEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
		SubmitButton.Click += new EventHandler( OkButton_Click );

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
            TblProperties rs = db.TblProperties.SingleOrDefault(target => target.PrpID == id);

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            PrpIDLiteral.Text = rs.PrpID.ToString();
            PrpNameTextBox.Text = rs.PrpName;
            PrpDescriptionTextBox.Text = rs.PrpDescription;
		}
	}

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

		// Get the record
        TblProperties rs = db.TblProperties.SingleOrDefault(target => target.PrpID == id);

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Update the record
        rs.PrpName = WebConvert.Truncate(PrpNameTextBox.Text.Trim(), 50);
        rs.PrpDescription = PrpDescriptionTextBox.Text.Trim();

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
        Response.Redirect("PrpView.aspx?ID=" + id.ToString());
	}
}