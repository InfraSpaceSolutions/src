/******************************************************************************
 * Filename: BusBrpEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Eidt a business's properties.
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

public partial class admin_BusBrpEdit : System.Web.UI.Page
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
            BusNameHyperLink.Text = rs.BusName;
            BusNameHyperLink.ToolTip = "View Business";
            BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rs.BusID.ToString();
            BusFormalNameLiteral.Text = rs.BusFormalName;

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
                foreach (TblBusinessProperties bpr in db.TblBusinessProperties.Where(target => target.BusID == id))
                {
                    PrpIDCheckBoxList.Items.FindByValue(bpr.PrpID.ToString()).Selected = true;
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

        // Delete existing links between the businesses and properties
        foreach (TblBusinessProperties bpr in db.TblBusinessProperties.Where(target => target.BusID == id))
        {
            db.TblBusinessProperties.DeleteOnSubmit(bpr);
            db.SubmitChanges();
        }

        // Create links between the business and properties selected
        int numberOfProperties = db.TblProperties.Count();
        for (int i = 0; i < numberOfProperties; i++)
        {
            if (PrpIDCheckBoxList.Items[i].Selected)
            {
                // Create a new business property link record
                TblBusinessProperties rsBpr = new TblBusinessProperties();

                // Populate the record
                rsBpr.BusID = id;
                rsBpr.PrpID = WebConvert.ToInt32(PrpIDCheckBoxList.Items[i].Value, 0);

                // Insert record into table
                db.TblBusinessProperties.InsertOnSubmit(rsBpr);

                // Sync to database
                db.SubmitChanges();
            }
        }

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }
}