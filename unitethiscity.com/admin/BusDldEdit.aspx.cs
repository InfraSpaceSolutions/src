/******************************************************************************
 * Filename: BusDldEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a business.
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

public partial class admin_BusDldEdit : System.Web.UI.Page
{
	int id;
    int dldid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target bus id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        // Get the target dldID to update
        dldid = WebConvert.ToInt32(Request.QueryString["DldID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
        }
        // Verify dldid exists
        if (dldid == 0)
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

            // get the target deal definition
            TblDealDefinitions rsDld = db.TblDealDefinitions.SingleOrDefault(target => target.DldID == dldid);

            // Verify target records exits
            if (rsDld == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            DldNameTextBox.Text = rsDld.DldName;
            DldAmountTextBox.Text = String.Format("{0:N}", rsDld.DldAmount);
            DldDescriptionTextBox.Text = rsDld.DldDescription;
            DldCustomTermsTextBox.Text = rsDld.DldCustomTerms;
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
        TblDealDefinitions rs = db.TblDealDefinitions.SingleOrDefault( target => target.BusID == id && target.DldID == dldid );
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Populate fields
        rs.DldName =  WebConvert.Truncate( DldNameTextBox.Text.Trim(), 50 );
        rs.DldAmount = WebConvert.ToDecimal(DldAmountTextBox.Text.Trim(), 0);
        rs.DldDescription = WebConvert.Truncate(DldDescriptionTextBox.Text.Trim(), 255);
        rs.DldCustomTerms = WebConvert.Truncate(DldCustomTermsTextBox.Text.Trim(), 255);

        // Submit to the db
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString());
    }
}