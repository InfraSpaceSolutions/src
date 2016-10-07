/******************************************************************************
 * Filename: ProEdit.aspx.cs
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

public partial class admin_ProEdit : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {

        // Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);
        ProNameDuplicate.ServerValidate += new ServerValidateEventHandler(ProNameDuplicate_ServerValidate);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        // Verify id exists
        if (id == 0)
        {
            throw new WebException(RC.DataIncomplete);
        }

        if (!Page.IsPostBack)
        {
            // Get the target record
            TblPromotions rs = db.TblPromotions.SingleOrDefault(target => target.ProID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            ProIDLiteral.Text = rs.ProID.ToString();
            ProNameTextBox.Text = rs.ProName;
            ProTitleTextBox.Text = rs.ProTitle;
            ProTextTextBox.Text = rs.ProText;
            ProEnabledDropDownList.SelectedValue = rs.ProEnabled.ToString();
        }
    }

    void ProNameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Check for duplicate referral code
        args.IsValid = (db.TblPromotions.Count(target => target.ProID != id && target.ProName == args.Value.Trim()) == 0);
    }

    void OkButton_Click(object sender, EventArgs e)
    {
        // Validate the page
        if (!Page.IsValid)
        {
            return;
        }

        // Get the record
        TblPromotions rs = db.TblPromotions.Single(target => target.ProID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Update the record
        rs.ProName = WebConvert.Truncate(ProNameTextBox.Text.Trim(), 50);
        rs.ProTitle = WebConvert.Truncate(ProTitleTextBox.Text.Trim(), 128);
        rs.ProText = ProTextTextBox.Text;
        rs.ProEnabled= WebConvert.ToBoolean(ProEnabledDropDownList.SelectedValue, false);

        // Sync to database
        db.SubmitChanges();

        // Redirect to target view page
        Response.Redirect("ProView.aspx?ID=" + id.ToString());
    }
}