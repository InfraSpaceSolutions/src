/******************************************************************************
 * Filename: AccAcrNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new account.
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

public partial class admin_AccAcrNew : System.Web.UI.Page
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

            // Load the roles dropdown list
            List<TblRoles> rsRoles = (from rows in db.TblRoles where rows.RolID == (int)Roles.Business || rows.RolID == (int)Roles.Associate select rows).ToList();
            RolIDDropDownList.DataSource = rsRoles;
            RolIDDropDownList.DataBind();

            // Load the businesses dropdown list
            List<TblBusinesses> rsBusinesses = (from rows in db.TblBusinesses select rows).ToList();
            BusIDDropDownList.DataSource = rsBusinesses;
            BusIDDropDownList.DataBind();
        }
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        // get the selected role
        int rolid = WebConvert.ToInt32(RolIDDropDownList.SelectedValue, 0);
 
        // get the selected business
        int busid = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);

        // make sure that our data is reasonable
        if ( rolid == 0 || busid == 0 || id == 0)
        {
            throw new WebException(RC.DataInvalid);
        }

		// create or update the record
        TblAccountRoles rs = db.TblAccountRoles.SingleOrDefault(target => target.AccID == id && target.RolID == rolid && target.BusID == busid);
        if (rs == null)
        {
            rs = new TblAccountRoles();
            rs.AccID = id;
            rs.BusID = busid;
            rs.RolID = rolid;
            db.TblAccountRoles.InsertOnSubmit(rs);
        }

        // assign the access level
        rs.AclID = 100;

		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
	}
}