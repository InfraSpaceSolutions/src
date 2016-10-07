/******************************************************************************
 * Filename: BusEvtNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a business event.
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

public partial class admin_BusEvtNew : System.Web.UI.Page
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

            // Load the event types dropdown list
            List<TblEventTypes> rsEtt =
                (from ett in db.TblEventTypes
                 orderby ett.EttName
                 select ett).ToList();

            EttIDDropDownList.DataSource = rsEtt;
            EttIDDropDownList.DataBind();

            // default the date picker to todays date
            ChkStartDateEdit.Value = DateTime.Now;
            ChkEndDateEdit.Value = DateTime.Now;
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
        TblEvents rs = new TblEvents();

        // Populate fields
        rs.BusID = id;
        rs.EvjID = 0;
        rs.EttID = WebConvert.ToInt32(EttIDDropDownList.SelectedValue, 0);
        rs.EvtStartDate = WebConvert.ToDateTime(ChkStartDateEdit.Value, DateTime.Today);
        rs.EvtEndDate = WebConvert.ToDateTime(ChkEndDateEdit.Value, WebConvert.ToDateTime( ChkStartDateEdit.Value, DateTime.Today));
        rs.EvtSummary = WebConvert.Truncate(EvtSummaryTextBox.Text.Trim(), 140);
        rs.EvtBody = WebConvert.ToString(EvtBodyTextBox.Text.Trim(), "");

        // Submit to the db
        db.TblEvents.InsertOnSubmit(rs);
        db.SubmitChanges();

        // add the event link if needed
        string eventLink = EventLinkTextBox.Text.Trim();
        if (eventLink.Length > 0)
        {
            TblEventLinks rsEventLink = new TblEventLinks();
            rsEventLink.EvtID = rs.EvtID;
            db.TblEventLinks.InsertOnSubmit(rsEventLink);
            rsEventLink.EvtLinkName = eventLink;
            db.SubmitChanges();
        }

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}