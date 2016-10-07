/******************************************************************************
 * Filename: BusEvtEdit.aspx.cs
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

public partial class admin_BusEvtEdit : System.Web.UI.Page
{
	int id;
    int evtid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target bus id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        // Get the target evtID to update
        evtid = WebConvert.ToInt32(Request.QueryString["EvtID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }
        // Verify evtid exists
        if (evtid == 0)
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

            // Load the event types dropdown list
            List<TblEventTypes> rsEtt =
                (from ett in db.TblEventTypes
                 orderby ett.EttName
                 select ett).ToList();

            EttIDDropDownList.DataSource = rsEtt;
            EttIDDropDownList.DataBind();

            // get the target event
            TblEvents rsEvt = db.TblEvents.SingleOrDefault(target => target.EvtID == evtid && target.BusID == id);

            // Verify target records exits
            if (rsEvt == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            EttIDDropDownList.SelectedValue = rsEvt.EttID.ToString();
            EvtSummaryTextBox.Text = rsEvt.EvtSummary;
            ChkStartDateEdit.Value = rsEvt.EvtStartDate;
            ChkEndDateEdit.Value = rsEvt.EvtEndDate;
            EvtBodyTextBox.Text = rsEvt.EvtBody;

            TblEventLinks rsEventLink = db.TblEventLinks.SingleOrDefault(target => target.EvtID == evtid);
            EventLinkTextBox.Text = (rsEventLink != null) ? rsEventLink.EvtLinkName : "";
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
        TblEvents rs = db.TblEvents.SingleOrDefault( target => target.BusID == id && target.EvtID == evtid );
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Populate fields
        rs.EttID = WebConvert.ToInt32(EttIDDropDownList.SelectedValue, 0);
        rs.EvtStartDate = WebConvert.ToDateTime(ChkStartDateEdit.Value, DateTime.Today);
        rs.EvtEndDate = WebConvert.ToDateTime(ChkEndDateEdit.Value, WebConvert.ToDateTime(ChkStartDateEdit.Value, DateTime.Today));
        rs.EvtSummary = WebConvert.Truncate(EvtSummaryTextBox.Text.Trim(), 140);
        rs.EvtBody = WebConvert.ToString(EvtBodyTextBox.Text.Trim(), "");

        // Submit to the db
        db.SubmitChanges();

        string eventLink = EventLinkTextBox.Text.Trim();
        if (eventLink.Length > 0)
        {
            TblEventLinks rsEventLink = db.TblEventLinks.SingleOrDefault(target => target.EvtID == rs.EvtID);
            if (rsEventLink == null)
            {
                rsEventLink = new TblEventLinks();
                rsEventLink.EvtID = rs.EvtID;
                db.TblEventLinks.InsertOnSubmit(rsEventLink);
            }
            rsEventLink.EvtLinkName = eventLink;
            db.SubmitChanges();
        }
        else
        {
            db.TblEventLinks.DeleteOnSubmit(db.TblEventLinks.SingleOrDefault(target => target.EvtID == rs.EvtID));
            db.SubmitChanges();
        }


        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}