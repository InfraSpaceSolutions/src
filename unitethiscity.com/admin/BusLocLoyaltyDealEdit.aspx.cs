
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_BusLocLoyaltyDealEdit : System.Web.UI.Page
{
    int id;
    int locid;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        DeleteButton.Click += DeleteButton_Click;

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);
        locid = WebConvert.ToInt32(Request.QueryString["LocID"], 0);

        // Verify id exists
        if (id == 0)
        {
            throw new WebException(RC.DataIncomplete);
        }

        if (!Page.IsPostBack)
        {
            // Get the record
            VwBusinesses rs = db.VwBusinesses.SingleOrDefault(target => target.BusID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            BusIDLiteral.Text = rs.BusID.ToString();
            BusGuidLiteral.Text = rs.BusGuid.ToString();
            BusNameLiteral.Text = rs.BusName;
            BusFormalNameLiteral.Text = rs.BusFormalName;

            TblLoyaltyDeals rsLoy = db.TblLoyaltyDeals.SingleOrDefault(target => target.LocID == locid);
            if (rsLoy != null)
            {
                LoyNameTextBox.Text = rsLoy.LoySummary;
                LoyPointsTextBox.Text = rsLoy.LoyPoints.ToString();
                LoyCustomTermsTextBox.Text = rsLoy.LoyTerms;
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

        TblLoyaltyDeals rsLoy = db.TblLoyaltyDeals.SingleOrDefault(target => target.LocID == locid);
        if (rsLoy == null)
        {
            rsLoy = new TblLoyaltyDeals();
            db.TblLoyaltyDeals.InsertOnSubmit(rsLoy);
            rsLoy.LocID = locid;
        }
        rsLoy.LoySummary = LoyNameTextBox.Text;
        rsLoy.LoyPoints = WebConvert.ToInt32(LoyPointsTextBox.Text, 1);
        rsLoy.LoyTerms = LoyCustomTermsTextBox.Text;
        db.SubmitChanges();

        // Redirect to the view page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&locID=" + locid.ToString());
    }
    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the record
        TblLoyaltyDeals rsLoy = db.TblLoyaltyDeals.SingleOrDefault(target => target.LocID == locid);
        if (rsLoy != null)
        {
            // Delete the record
            db.TblLoyaltyDeals.DeleteOnSubmit(rsLoy);
            db.SubmitChanges();
            // Update the revision level of the data set
            DataRevision.Bump(Revisioned.LocationInfo);
        }

        // Redirect to list page
        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid);
    }
}