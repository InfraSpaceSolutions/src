/******************************************************************************
 * Filename: PerView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an period.
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

public partial class admin_PerView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		EditButton.Click += new EventHandler( EditButton_Click );
        DeleteButton.Click += new EventHandler(DeleteButton_Click);
        AddDealButton.Click += new EventHandler(AddDealButton_Click);

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // Get the record
        TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Bind data to repeaters
        RefreshDeals();

		if ( !Page.IsPostBack )
		{
			// Populate the page
            PerIDLiteral.Text = rs.PerID.ToString();
            PerNameLiteral.Text = rs.PerName;
            PerStartDateLiteral.Text = rs.PerStartDate.ToShortDateString();
            PerEndDateLiteral.Text = rs.PerEndDate.ToShortDateString();
		}
	}

	void DeleteButton_Click( object sender, EventArgs e )
	{
		// Get the record
        TblPeriods rs = db.TblPeriods.Single( Target => Target.PerID == id );

		// Get account
        string name = rs.PerName;

		// Delete the record
		db.TblPeriods.DeleteOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to list page
        Response.Redirect("PerList.aspx?Name=" + name);
	}

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect("PerEdit.aspx?ID=" + id.ToString());
	}

    void RefreshDeals()
    {
        // Get this businesses deal definitions to populate repeater
        DealsRepeater.DataSource = db.VwDeals.Where(target => target.PerID == id).OrderBy(target => target.DelName);
        DealsRepeater.DataBind();

        // Show empty row if there are no deals for this period
        if (db.TblDeals.Count(target => target.PerID == id) == 0)
        {
            NoDealsRow.Visible = true;
        }
    }

    void AddDealButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("PerDelNew.aspx?ID=" + id.ToString() );
    }
}