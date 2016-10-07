/******************************************************************************
 * Filename: PerEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an period.
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

public partial class admin_PerEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);
        PerNameDuplicate.ServerValidate += new ServerValidateEventHandler(PerNameDuplicate_ServerValidate);

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
            TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerID == id);

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            PerIDLiteral.Text = rs.PerID.ToString();
            PerNameTextBox.Text = rs.PerName;
            PerStartDateEdit.Value = rs.PerStartDate;
            PerEndDateEdit.Value = rs.PerEndDate;
		}
	}

    void PerNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Check for duplicate period name
        args.IsValid = (db.TblPeriods.Count(target => target.PerName == args.Value.Trim() && target.PerID != id) == 0);
    }

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

		// Get the record
        TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerID == id);

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        DateTime perStartDate = (DateTime)WebConvert.ToDateTime(PerStartDateEdit.Value, null);
        DateTime perEndDate = (DateTime)WebConvert.ToDateTime(PerEndDateEdit.Value, null);

        // Update the record
        rs.PerName = WebConvert.Truncate(PerNameTextBox.Text.Trim(), 15);
        rs.PerStartDate = WebConvert.ToDateTime(perStartDate.ToShortDateString(), DateTime.Now);
        rs.PerEndDate = WebConvert.ToDateTime(perEndDate.ToShortDateString(), DateTime.Now);

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
        Response.Redirect("PerView.aspx?ID=" + id.ToString());
	}
}