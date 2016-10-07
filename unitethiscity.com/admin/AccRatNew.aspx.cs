/******************************************************************************
 * Filename: AccRatNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new rating for an account.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_AccRatNew : System.Web.UI.Page
{
    int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        if (!Page.IsPostBack)
        {
            // Get the record
            VwAccounts rs = db.VwAccounts.SingleOrDefault(target => target.AccID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            AccIDLiteral.Text = rs.AccID.ToString();
            AccGuidLiteral.Text = rs.AccGuid.ToString();
            AccFNameLiteral.Text = rs.AccFName;
            AccLNameLiteral.Text = rs.AccLName;
            AccEMailHyperLink.NavigateUrl = "mailto:" + rs.AccEMail;
            AccEMailHyperLink.Text = rs.AccEMail;
            AccEMailHyperLink.ToolTip = "Send E-mail";

            // Load the locations dropdown list
            List<TblLocations> rsLoc =
                (from loc in db.TblLocations
                 where !(from rat in db.TblRatings where rat.AccID == id select rat.LocID).Contains(loc.LocID)
                 orderby loc.LocName
                 select loc).ToList();

            LocIDDropDownList.DataSource = rsLoc;
            LocIDDropDownList.DataBind();
        }
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

		// Create the record
		TblRatings rs = new TblRatings();

        // Populate fields
        rs.AccID = id;
        rs.LocID = WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0);
        rs.RatRating = WebConvert.ToInt32(RatRatingDropDownList.SelectedValue, 0); 
        rs.RatTS = DateTime.Now;

		// Submit to the db
		db.TblRatings.InsertOnSubmit( rs );
		db.SubmitChanges();

        //Update the locations rating
        TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0));
        // Verify target record exists
        if (rsLoc != null)
        {
            rsLoc.LocRating = CalculateLocationsRating(WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0));
            // Submit to the db
            db.SubmitChanges();
        }

        //Update the businesses rating
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == rsLoc.BusID);
        // Verify target record exists
        if (rsBus != null)
        {
            rsBus.BusRating = CalculateBusinessesRating( rsBus.BusID );
            // Submit to the db
            db.SubmitChanges();
        }

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
    }

    double CalculateBusinessesRating( int busid )
    {
        int lngTotalAccountRatings = 0;
        double dblSumRatings = 0;
        double dblRating = 0;

        // get the total amount of ratings for this business( all business locations )
        lngTotalAccountRatings = db.VwRatings.Count(target => target.BusID == busid);
        lngTotalAccountRatings = WebConvert.ToInt32(lngTotalAccountRatings, 0);

        // get the total ratings amount from all locations for this business
        dblSumRatings = db.VwRatings.Where(target => target.BusID == busid).ToList().Sum(target => target.RatRating);
        dblSumRatings = WebConvert.ToDouble(dblSumRatings, 0);

        // get the average rating
        dblRating = WebConvert.ToDouble(dblSumRatings / lngTotalAccountRatings, 0);

        dblRating = Math.Round(dblRating, 2);

        if (double.IsNaN(dblRating))
        {
            dblRating = 0;
        }

        // return the average rating
        return dblRating;
    }

    double CalculateLocationsRating( int locid )
    {
        int lngTotalAccountRatings = 0;
        double dblSumRatings = 0;
        double dblRating = 0;

        // get the total amount of ratings for this location( number of ratings for this location )
        lngTotalAccountRatings = db.TblRatings.Count(target => target.LocID == locid);
        lngTotalAccountRatings = WebConvert.ToInt32(lngTotalAccountRatings, 0);

        // get the total ratings amount from all accounts( sum of ratings for this location )
        dblSumRatings = db.TblRatings.Where(target => target.LocID == locid).ToList().Sum(target => target.RatRating);
        dblSumRatings = WebConvert.ToDouble(dblSumRatings, 0);

        // get the average rating
        dblRating = WebConvert.ToDouble(dblSumRatings / lngTotalAccountRatings, 0);

        dblRating = Math.Round(dblRating, 2);

        if (double.IsNaN(dblRating))
        {
            dblRating = 0;
        }

        // return the average rating
        return dblRating;
    }
}