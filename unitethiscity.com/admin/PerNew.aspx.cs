/******************************************************************************
 * Filename: PerNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new period.
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

public partial class admin_PerNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        PerNameDuplicate.ServerValidate += new ServerValidateEventHandler(PerNameDuplicate_ServerValidate);
	}

    void PerNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (db.TblPeriods.Count(target => target.PerName == args.Value.Trim()) == 0);
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        DateTime perStartDate = (DateTime)WebConvert.ToDateTime(PerStartDateEdit.Value, null);
        DateTime perEndDate = (DateTime)WebConvert.ToDateTime(PerEndDateEdit.Value, null);

		// Create the record
		TblPeriods rs = new TblPeriods();

		// Populate fields
        rs.PerName = WebConvert.Truncate(PerNameTextBox.Text.Trim(), 16);
        rs.PerStartDate = WebConvert.ToDateTime(perStartDate.ToShortDateString(), DateTime.Now );
        rs.PerEndDate = WebConvert.ToDateTime(perEndDate.ToShortDateString(), DateTime.Now);

		// Submit to the db
		db.TblPeriods.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect("PerView.aspx?ID=" + rs.PerID.ToString());
	}
}