/******************************************************************************
 * Filename: BusLocChkNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new rating for this location.
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

public partial class admin_BusLocChkNew : System.Web.UI.Page
{
	int id;
    int locid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );
        locid = WebConvert.ToInt32(Request.QueryString["LocID"], 0); 

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

            // Load the accounts dropdown list
            List<TblAccounts> rsAcc =
                (from acc in db.TblAccounts
                 orderby acc.AccEMail
                 select acc).ToList();

            AccIDDropDownList.DataSource = rsAcc;
            AccIDDropDownList.DataBind();

            // Load the periods dropdown list
            List<TblPeriods> rsPer =
                (from per in db.TblPeriods
                 orderby per.PerName
                 select per).ToList();

            PerIDDropDownList.DataSource = rsPer;
            foreach (var per in rsPer)
            {
                ListItem li = new ListItem(per.PerName + " ( " + per.PerStartDate.ToShortDateString() + " - " + per.PerEndDate.ToShortDateString() + " )", per.PerID.ToString());
                PerIDDropDownList.Items.Add(li);
            }

            // default the date picker to todays date
            ChkDateEdit.Value = DateTime.Now;
		}
	}

    void SubmitButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        DateTime chkTSDate = (DateTime)WebConvert.ToDateTime(ChkDateEdit.Value, null);
        DateTime chkTime = (DateTime)ChkTimeEdit.Value;

        // Create the record
        TblCheckIns rs = new TblCheckIns();

        // Populate fields
        rs.AccID = WebConvert.ToInt32(AccIDDropDownList.SelectedValue, 0);
        rs.LocID = locid;
        rs.PerID = WebConvert.ToInt32(PerIDDropDownList.SelectedValue, 0);
        rs.ChkTS = WebConvert.ToDateTime(chkTSDate.ToShortDateString() + " " + chkTime.ToShortTimeString(), DateTime.Now);

        // Submit to the db
        db.TblCheckIns.InsertOnSubmit(rs);
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&locID=" + locid.ToString());
    }
}