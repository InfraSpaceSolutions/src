/******************************************************************************
 * Filename: ProNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit a promotional code
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

public partial class admin_ProNew : System.Web.UI.Page
{
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {

        // Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);
        ProNameDuplicate.ServerValidate += new ServerValidateEventHandler(ProNameDuplicate_ServerValidate);
    }

    void ProNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Check for duplicate referral code
        args.IsValid = (db.TblPromotions.Count(target=>target.ProName == args.Value.Trim()) == 0);
    }

    void OkButton_Click(object sender, EventArgs e)
    {
        // Validate the page
        if (!Page.IsValid)
        {
            return;
        }

        // Get the record
        TblPromotions rs = new TblPromotions();

        // Update the record
        rs.ProName = WebConvert.Truncate(ProNameTextBox.Text.Trim(), 50);
        rs.ProTitle = WebConvert.Truncate(ProTitleTextBox.Text.Trim(), 128);
        rs.ProText = ProTextTextBox.Text;
        rs.ProEnabled = WebConvert.ToBoolean(ProEnabledDropDownList.SelectedValue, false);

        db.TblPromotions.InsertOnSubmit(rs);

        // Sync to database
        db.SubmitChanges();

        // Redirect to target view page
        Response.Redirect("ProView.aspx?ID=" + rs.ProID.ToString());
    }
}