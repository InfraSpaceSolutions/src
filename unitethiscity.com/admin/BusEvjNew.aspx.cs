/******************************************************************************
 * Filename: BusEvjNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a business recurring event.
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

public partial class admin_BusEvjNew : System.Web.UI.Page
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

            // default values set up
            EvjIntervalDailyTextBox.Text = "7";
            EvjIntervalMonthlyTextBox.Text = "1";
            EvjEnabledDropDownList.SelectedValue = "True";
            EvjDurationTextBox.Text = "1";

            // Load the event job types dropdown list
            List<TblEventJobTypes> rsEjt =
                (from ejt in db.TblEventJobTypes
                 orderby ejt.EjtID
                 select ejt).ToList();

            EjtIDDropDownList.DataSource = rsEjt;
            EjtIDDropDownList.DataBind();

            // Load the event types dropdown list
            List<TblEventTypes> rsEtt =
                (from ett in db.TblEventTypes
                 orderby ett.EttName
                 select ett).ToList();

            EttIDDropDownList.DataSource = rsEtt;
            EttIDDropDownList.DataBind();

            // default the date picker to todays date
            ChkBeginDateEdit.Value = DateTime.Now;
            ChkStopDateEdit.Value = DateTime.Now;
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
        TblEventJobs rs = new TblEventJobs();

        // Populate fields
        rs.BusID = id;
        rs.EttID = WebConvert.ToInt32(EttIDDropDownList.SelectedValue, 0);
        rs.EjtID = WebConvert.ToInt32(EjtIDDropDownList.SelectedValue, 0);
        switch (WebConvert.ToInt32(EjtIDDropDownList.SelectedValue, 0))
        {
            case 1:
                rs.EvjInterval = WebConvert.ToInt32(EvjIntervalDailyTextBox, 0);
                break;
            case 2:
                rs.EvjInterval = WebConvert.ToInt32(EvjIntervalWeeklyDropDownList.SelectedValue, 0);
                break;
            case 3:
                rs.EvjInterval = WebConvert.ToInt32(EvjIntervalMonthlyTextBox, 0);
                break;
            default:
                rs.EvjInterval = WebConvert.ToInt32(EvjIntervalDailyTextBox, 0);
                break;
        }
        rs.EvjName = WebConvert.Truncate(EvjNameTextBox.Text.Trim(), 50);
        rs.EvjEnabled = WebConvert.ToBoolean(EvjEnabledDropDownList.SelectedValue, false);
        rs.EvjBeginDate = WebConvert.ToDateTime(ChkBeginDateEdit.Value, DateTime.Today);
        rs.EvjStopDate = WebConvert.ToDateTime(ChkStopDateEdit.Value, DateTime.Today);
        rs.EvjDuration = WebConvert.ToInt32(EvjDurationTextBox.Text.Trim(), 0);
        rs.EvjDaysPublished = WebConvert.ToInt32(SiteSettings.GetValue("RecurringDaysPublished"), 0);
        rs.EvjSummary = WebConvert.Truncate(EvjSummaryTextBox.Text.Trim(), 140);
        rs.EvjBody = WebConvert.ToString(EvjBodyTextBox.Text.Trim(), "");

        // Submit to the db
        db.TblEventJobs.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}