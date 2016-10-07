/******************************************************************************
 * Filename: BusLocLprEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit a locations's properties.
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

public partial class admin_BusLocLprEdit : System.Web.UI.Page
{
	int id;
    int locid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);
        locid = WebConvert.ToInt32(Request.QueryString["LocID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }
        if (locid == 0)
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
            BusNameHyperLink.Text = rs.BusName;
            BusNameHyperLink.ToolTip = "View Business";
            BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rs.BusID.ToString();
            BusFormalNameLiteral.Text = rs.BusFormalName;

            // Get the target location record
            TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == locid);
            if (rsLoc == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            LocNameHyperLink.Text = rsLoc.LocName;
            LocNameHyperLink.ToolTip = "View Location";
            LocNameHyperLink.NavigateUrl = "BusLocView.aspx?ID=" + rs.BusID.ToString() + "&LocID=" + rsLoc.LocID;


            // Bind all of the properties from the properties table to the checkboxlist
            if (db.TblProperties.Count() == 0)
            {
                PrpIDCheckBoxList.Visible = false;
                NoPropertiesLiteral.Visible = true;
            }
            else
            {
                PrpIDCheckBoxList.Visible = true;
                PrpIDCheckBoxList.DataSource = db.TblProperties.OrderBy( target => target.PrpName );
                PrpIDCheckBoxList.DataBind();
                NoPropertiesLiteral.Visible = false;

                // Pre-check the properties that are already tied to this business
                foreach (TblLocationProperties lpr in db.TblLocationProperties.Where(target => target.LocID == locid))
                {
                    PrpIDCheckBoxList.Items.FindByValue(lpr.PrpID.ToString()).Selected = true;
                }
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

        // Delete existing links between the location and properties
        foreach (TblLocationProperties lpr in db.TblLocationProperties.Where(target => target.LocID == locid))
        {
            db.TblLocationProperties.DeleteOnSubmit(lpr);
            db.SubmitChanges();
        }

        // Create links between the location and properties selected
        int numberOfProperties = db.TblProperties.Count();
        for (int i = 0; i < numberOfProperties; i++)
        {
            if (PrpIDCheckBoxList.Items[i].Selected)
            {
                // Create a new location property link record
                TblLocationProperties rsLpr = new TblLocationProperties();

                // Populate the record
                rsLpr.LocID = locid;
                rsLpr.PrpID = WebConvert.ToInt32(PrpIDCheckBoxList.Items[i].Value, 0);

                // Insert record into table
                db.TblLocationProperties.InsertOnSubmit(rsLpr);

                // Sync to database
                db.SubmitChanges();
            }
        }

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to the location view page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }
}