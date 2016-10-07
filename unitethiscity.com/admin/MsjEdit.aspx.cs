/******************************************************************************
 * Filename: MsjEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an referral code.
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

public partial class admin_MsjEdit : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);

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
            VwMessageJobs rs = db.VwMessageJobs.SingleOrDefault(target => target.MsjID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            MjsIDDropDownList.DataSource = db.TblMessageJobStates;
            MjsIDDropDownList.DataBind();
            RolIDDropDownList.DataSource = db.TblRoles;
            RolIDDropDownList.DataBind();
            BusIDDropDownList.DataSource = db.TblBusinesses.OrderBy(target => target.BusName);
            BusIDDropDownList.DataBind();

            // create the hours dropdown
            for (int i=0; i < 24; i++)
            {
                DateTime dt = DateTime.Today.AddHours(i);
                SendHourDropDownList.Items.Add(new ListItem(dt.ToShortTimeString(), i.ToString()));
            }

            // Populate the page
            MsjIDLiteral.Text = rs.MsjID.ToString();
            AccEMailLiteral.Text = rs.AccEMail;
            MjsIDDropDownList.SelectedValue = rs.MjsID.ToString();
            SendDateTextBox.Text = rs.MsjSendTS.ToShortDateString();
            SendHourDropDownList.SelectedValue = rs.MsjSendTS.Hour.ToString();
            RolIDDropDownList.SelectedValue = rs.RolID.ToString();
            BusIDDropDownList.SelectedValue = rs.BusID.ToString();
            MsgFromNameTextBox.Text = rs.MsgFromName;
            MsgSummaryTextBox.Text = rs.MsgSummary;
            MsgBodyTextBox.Text = rs.MsgBody;
        }
    }

    void OkButton_Click(object sender, EventArgs e)
    {
        // Validate the page
        if (!Page.IsValid)
        {
            return;
        }

        TblMessageJobs rsMsj = db.TblMessageJobs.Single(target => target.MsjID == id);
        TblMessages rsMsg = db.TblMessages.Single(target => target.MsgID == rsMsj.MsgID);

        rsMsj.MjsID = WebConvert.ToInt32(MjsIDDropDownList.SelectedValue, 0);
        rsMsj.MsjSendTS = WebConvert.ToDateTime(SendDateTextBox.Text, DateTime.Today);
        rsMsj.MsjSendTS = rsMsj.MsjSendTS.AddHours(WebConvert.ToInt32(SendHourDropDownList.SelectedValue, 0));
        rsMsj.RolID = WebConvert.ToInt32(RolIDDropDownList.SelectedValue, 0);
        rsMsj.BusID = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);

        rsMsg.MsgFromName = MsgFromNameTextBox.Text;
        rsMsg.MsgSummary = MsgSummaryTextBox.Text;
        rsMsg.MsgBody = MsgBodyTextBox.Text;
        rsMsg.MsgTS = DateTime.Now;

        db.SubmitChanges();

        // Redirect to target view page
        Response.Redirect("MsjView.aspx?ID=" + id.ToString());
    }
}