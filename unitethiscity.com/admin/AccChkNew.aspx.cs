/******************************************************************************
 * Filename: AccChkNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new checkin for an account.
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

public partial class admin_AccChkNew : System.Web.UI.Page
{
    int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        if (!Page.IsPostBack)
        {
            // Get the record
            VwAccounts rs = db.VwAccounts.SingleOrDefault(target => target.AccID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            AccIDLiteral.Text = rs.AccID.ToString();
            AccGuidLiteral.Text = rs.AccGuid.ToString();
            AccFNameLiteral.Text = rs.AccFName;
            AccLNameLiteral.Text = rs.AccLName;
            AccEMailHyperLink.NavigateUrl = "mailto:" + rs.AccEMail;
            AccEMailHyperLink.Text = rs.AccEMail;
            AccEMailHyperLink.ToolTip = "Send E-mail";

            // Load the locations dropdown list
            List<TblLocations> rsLoc =
                (from loc in db.TblLocations
                 orderby loc.LocName
                 select loc).ToList();

            LocIDDropDownList.DataSource = rsLoc;
            LocIDDropDownList.DataBind();

            // default the date picker to todays date
            ChkDateEdit.Value = DateTime.Today;
            ChkTimeEdit.Value = DateTime.Now;
        }
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        DateTime chkTSDate = (DateTime)WebConvert.ToDateTime(ChkDateEdit.Value, DateTime.Today);
        DateTime chkTSTime = (DateTime)WebConvert.ToDateTime(ChkTimeEdit.Value, DateTime.Today);
        DateTime chkTS = chkTSDate + new TimeSpan(chkTSTime.Hour, chkTSTime.Minute, 0);

        Period period = new Period(chkTS);

        // Create the record
		TblCheckIns rs = new TblCheckIns();


        // Populate fields
        rs.AccID = id;
        rs.LocID = WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0);
        rs.ChkTS = chkTS;
        rs.PerID = period.PerID;

		// Submit to the db
		db.TblCheckIns.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
	}
}