﻿/******************************************************************************
 * Filename: BusLocRatNew.aspx.cs
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

public partial class admin_BusLocRatNew : System.Web.UI.Page
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
                 where !(from rat in db.TblRatings where rat.LocID == locid select rat.AccID).Contains(acc.AccID)
                 orderby acc.AccEMail
                 select acc).ToList();

            AccIDDropDownList.DataSource = rsAcc;
            AccIDDropDownList.DataBind();
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
        TblRatings rs = new TblRatings();

        // Populate fields
        rs.AccID = WebConvert.ToInt32(AccIDDropDownList.SelectedValue, 0);
        rs.LocID = locid;
        rs.RatRating = WebConvert.ToInt32(RatRatingDropDownList.SelectedValue, 0);
        rs.RatTS = DateTime.Now;

        // Submit to the db
        db.TblRatings.InsertOnSubmit(rs);
        db.SubmitChanges();

        //Update the businesses rating
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == id);
        // Verify target record exists
        if (rsBus != null)
        {
            rsBus.BusRating = CalculateBusinessesRating();
            // Submit to the db
            db.SubmitChanges();
        }

        //Update the locations rating
        TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == locid);
        // Verify target record exists
        if (rsLoc != null)
        {
            rsLoc.LocRating = CalculateLocationsRating();
            // Submit to the db
            db.SubmitChanges();
        }

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to the view page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&locID=" + locid.ToString());
    }

    double CalculateBusinessesRating()
    {
        int lngTotalAccountRatings = 0;
        double dblSumRatings = 0;
        double dblRating = 0;

        // get the total amount of ratings for this business( all business locations )
        lngTotalAccountRatings = db.VwRatings.Count(target => target.BusID == id);
        lngTotalAccountRatings = WebConvert.ToInt32(lngTotalAccountRatings, 0);

        // get the total ratings amount from all locations for this business
        dblSumRatings = db.VwRatings.Where(target => target.BusID == id).ToList().Sum(target => target.RatRating);
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

    double CalculateLocationsRating()
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