/******************************************************************************
 * Filename: PerDelNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create an period deal.
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

public partial class admin_PerDelNew : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        BusIDDropDownList.SelectedIndexChanged += new EventHandler(BusIDDropDownList_SelectedIndexChanged);

		// Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
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

            // load the businesses dropdownlist
            List<TblBusinesses> rsBus =
                (from bus in db.TblBusinesses
                 where bus.BusAssignedDldID != 0 && !(from del in db.TblDeals where del.BusID == bus.BusID select del.BusID).Contains(bus.BusID)
                 orderby bus.BusName
                 select bus).ToList();

            foreach (var bus in rsBus)
            {
                ListItem li = new ListItem(bus.BusName, bus.BusID.ToString());
                BusIDDropDownList.Items.Add(li);
            }
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
        TblDeals rs = new TblDeals();

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Populate fields
        rs.BusID = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);
        rs.PerID = id;
        rs.DelName = WebConvert.Truncate(DelNameTextBox.Text.Trim(), 50);
        rs.DelAmount = WebConvert.ToDecimal(DelAmountTextBox.Text.Trim(), 0);
        rs.DelDescription = WebConvert.Truncate(DelDescriptionTextBox.Text.Trim(), 255);
        rs.DelCustomTerms = WebConvert.Truncate(DelCustomTermsTextBox.Text.Trim(), 255);

        // Insert the new page record
        db.TblDeals.InsertOnSubmit(rs);

        // Submit to the db
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("PerDelView.aspx?ID=" + id.ToString() + "&DelID=" + rs.DelID.ToString());
    }

    void BusIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int busID = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);
        int busAssignedDldID = 0;

        // check business for an assigned deal definition
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == busID);
        if (rsBus != null)
        {
            busAssignedDldID = rsBus.BusAssignedDldID;
        }

        // update the form values based on the business deal definition if any
        // Get the target deal record
        TblDealDefinitions rsDld = db.TblDealDefinitions.SingleOrDefault(target => target.DldID == busAssignedDldID);
        if (rsDld != null)
        {
            // assigned deal definition information
            DelNameTextBox.Text = rsDld.DldName;
            DelAmountTextBox.Text = String.Format("{0:N}", rsDld.DldAmount);
            DelDescriptionTextBox.Text = rsDld.DldDescription;
            DelCustomTermsTextBox.Text = rsDld.DldCustomTerms;
        }
        else
        {
            DelNameTextBox.Text = "";
            DelAmountTextBox.Text = "";
            DelDescriptionTextBox.Text = "";
            DelCustomTermsTextBox.Text = "";
        }
    }
}