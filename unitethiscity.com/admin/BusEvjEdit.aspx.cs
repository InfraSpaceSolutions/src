/******************************************************************************
 * Filename: BusEvjEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit a business event.
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

public partial class admin_BusEvjEdit : System.Web.UI.Page
{
	int id;
    int evjid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        PurgeButton.Click += new EventHandler(PurgeButton_Click);

		// Get the target bus id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        // Get the target evtID to update
        evjid = WebConvert.ToInt32(Request.QueryString["EvjID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }
        // Verify evtid exists
        if (evjid == 0)
        {
            throw new WebException(RC.DataIncomplete);
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

            // get the target event
            TblEventJobs rsEvj = db.TblEventJobs.SingleOrDefault(target => target.EvjID == evjid && target.BusID == id);

            // Verify target records exits
            if (rsEvj == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            EvjNameTextBox.Text = rsEvj.EvjName;
            EjtIDDropDownList.SelectedValue = rsEvj.EjtID.ToString();
            EvjIntervalDailyTextBox.Text = rsEvj.EvjInterval.ToString();
            EvjIntervalWeeklyDropDownList.SelectedValue = rsEvj.EvjInterval.ToString();
            EvjIntervalMonthlyTextBox.Text = rsEvj.EvjInterval.ToString();
            ChkBeginDateEdit.Value = rsEvj.EvjBeginDate;
            ChkStopDateEdit.Value = rsEvj.EvjStopDate;
            EvjEnabledDropDownList.SelectedValue = (rsEvj.EvjEnabled) ? "True" : "False";
            EttIDDropDownList.SelectedValue = rsEvj.EttID.ToString();
            EvjDurationTextBox.Text = rsEvj.EvjDuration.ToString();
            EvjSummaryTextBox.Text = rsEvj.EvjSummary;
            EvjBodyTextBox.Text = rsEvj.EvjBody;
		}
	}

    void SubmitButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        // get the target record
        TblEventJobs rs = db.TblEventJobs.SingleOrDefault( target => target.BusID == id && target.EvjID == evjid );
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

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
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }

    void PurgeButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        // get the target record and confirm it exists
        TblEventJobs rs = db.TblEventJobs.SingleOrDefault(target => target.BusID == id && target.EvjID == evjid);
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // delete all associated events
        db.ExecuteCommand("DELETE FROM tblEvents WHERE EvjID=" + evjid);

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}