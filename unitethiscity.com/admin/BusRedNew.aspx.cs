/******************************************************************************
 * Filename: BusRedNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a business redemption.
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

public partial class admin_BusRedNew : System.Web.UI.Page
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

            // Populate the accounts drop down list
            List<TblAccounts> rsAcc =
                (from acc in db.TblAccounts
                 where !(from red in db.TblRedemptions select red.AccID).Contains(acc.AccID)
                 orderby acc.AccEMail
                 select acc).ToList();

            foreach (var acc in rsAcc)
            {
                ListItem li = new ListItem(acc.AccEMail, acc.AccID.ToString());
                AccIDDropDownList.Items.Add(li);
            }

            // Populate the deals drop down list
            List<TblDeals> rsDel =
                (from del in db.TblDeals
                 where del.BusID == id
                 orderby del.DelName
                 select del).ToList();

            foreach (var del in rsDel)
            {
                ListItem li = new ListItem(del.DelName, del.DelID.ToString());
                DelIDDropDownList.Items.Add(li);
            }

            if (rs.BusRequirePin)
            {
                // Populate the deals drop down list
                List<TblPins> rsPin =
                    (from pin in db.TblPins
                     where pin.BusID == id && pin.PinEnabled == true
                     orderby pin.PinName
                     select pin).ToList();

                foreach (var pin in rsPin)
                {
                    ListItem li = new ListItem(pin.PinName + " (" + pin.PinNumber + ")", pin.PinID.ToString());
                    PinIDDropDownList.Items.Add(li);
                }

                PinRow.Visible = true;
            }
            else
            {
                PinRow.Visible = false;
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
        TblRedemptions rs = new TblRedemptions();

        // Populate fields
        rs.AccID = WebConvert.ToInt32( AccIDDropDownList.SelectedValue, 0 );
        rs.DelID = WebConvert.ToInt32(DelIDDropDownList.SelectedValue, 0);
        rs.RedTS = DateTime.Now;
        rs.PinID = WebConvert.ToInt32(PinIDDropDownList.SelectedValue, 0);

        // Submit to the db
        db.TblRedemptions.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }
}