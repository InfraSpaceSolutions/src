/******************************************************************************
 * Filename: PerDelEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an period deal.
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

public partial class admin_PerDelEdit : System.Web.UI.Page
{
	int id;
    int delid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

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

		if ( !Page.IsPostBack )
        {

            // Get the record
            TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }
			// Populate the page
            PerIDLiteral.Text = rs.PerID.ToString();
            PerNameHyperLink.Text = rs.PerName;
            PerNameHyperLink.NavigateUrl = "PerView.aspx?ID=" + id.ToString();
            PerNameHyperLink.ToolTip = "View Period";
            PerStartDateLiteral.Text = rs.PerStartDate.ToShortDateString();
            PerEndDateLiteral.Text = rs.PerEndDate.ToShortDateString();

            // Get the target deal record
            VwDeals rsDel = db.VwDeals.SingleOrDefault(target => target.DelID == delid);
            if (rsDel == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Location info
            DelNameTextBox.Text = rsDel.DelName;
            DelAmountTextBox.Text = String.Format("{0:N}", rsDel.DelAmount);
            DelDescriptionTextBox.Text = rsDel.DelDescription;
            DelCustomTermsTextBox.Text = rsDel.DelCustomTerms;
            BusNameHyperLink.Text = rsDel.BusName;
            BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rsDel.BusID.ToString();
            BusNameHyperLink.ToolTip = "View Business";
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
        TblDeals rs = db.TblDeals.SingleOrDefault(target => target.PerID == id && target.DelID == delid);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Populate fields
        rs.DelName = WebConvert.Truncate(DelNameTextBox.Text.Trim(), 50);
        rs.DelAmount = WebConvert.ToDecimal(DelAmountTextBox.Text.Trim(), 0);
        rs.DelDescription = WebConvert.Truncate(DelDescriptionTextBox.Text.Trim(), 255);
        rs.DelCustomTerms = WebConvert.Truncate(DelCustomTermsTextBox.Text.Trim(), 255);

        // Submit to the db
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("PerDelView.aspx?ID=" + rs.PerID.ToString() + "&DelID=" + rs.DelID.ToString());
    }
}