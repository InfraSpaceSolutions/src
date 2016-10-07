/******************************************************************************
 * Filename: PerDelView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an period deal.
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

public partial class admin_PerDelView : System.Web.UI.Page
{
	int id;
    int delid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        EditButton.Click += new EventHandler(EditButton_Click);
        DeleteButton.Click += new EventHandler(DeleteButton_Click);

		// Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);
        delid = WebConvert.ToInt32(Request.QueryString["DelID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }

        // Verify id exists
        if (delid == 0)
        {
            throw new WebException(RC.DataIncomplete);
        }

        // Get the record
        TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

		if ( !Page.IsPostBack )
		{
			// Populate the page
            PerIDLiteral.Text = rs.PerID.ToString();
            PerNameHyperLink.Text = rs.PerName;
            PerNameHyperLink.NavigateUrl = "PerView.aspx?ID=" + id.ToString();
            PerNameHyperLink.ToolTip = "View Period";
            PerStartDateLiteral.Text = rs.PerStartDate.ToShortDateString();
            PerEndDateLiteral.Text = rs.PerEndDate.ToShortDateString();
        }

        // Get the target deal record
        VwDeals rsDel = db.VwDeals.SingleOrDefault(target => target.DelID == delid);
        if (rsDel == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Location info
        DelNameLiteral.Text = rsDel.DelName;
        DelAmountLiteral.Text = String.Format( "{0:C}", rsDel.DelAmount );
        DelDescriptionLiteral.Text = WebConvert.PreserveBreaks( rsDel.DelDescription );
        DelCustomTermsLiteral.Text = WebConvert.PreserveBreaks( rsDel.DelCustomTerms );
        BusNameHyperLink.Text = rsDel.BusName;
        BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rsDel.BusID.ToString();
        BusNameHyperLink.ToolTip = "View Business";
	}

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the record
        TblDeals rs = db.TblDeals.Single(Target => Target.DelID == delid);

        // Get account
        string name = rs.DelName;

        // Delete the record
        db.TblDeals.DeleteOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("PerView.aspx?ID=" + id.ToString() + "&Name=" + name);
    }

    void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("PerDelEdit.aspx?ID=" + id.ToString() + "&DelID=" + delid.ToString());
    }
}