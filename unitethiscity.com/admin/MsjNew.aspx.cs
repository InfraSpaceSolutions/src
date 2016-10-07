/******************************************************************************
 * Filename: MsjNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * New message job
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

public partial class admin_MsjNew : System.Web.UI.Page
{
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);

        if (!Page.IsPostBack)
        {
            MjsIDDropDownList.DataSource = db.TblMessageJobStates;
            MjsIDDropDownList.DataBind();
            RolIDDropDownList.DataSource = db.TblRoles;
            RolIDDropDownList.DataBind();
            BusIDDropDownList.DataSource = db.TblBusinesses.OrderBy(target=>target.BusName);
            BusIDDropDownList.DataBind();
            // create the hours dropdown
            for (int i = 0; i < 24; i++)
            {
                DateTime dt = DateTime.Today.AddHours(i);
                SendHourDropDownList.Items.Add(new ListItem(dt.ToShortTimeString(), i.ToString()));
            }

            SendDateTextBox.Text = DateTime.Today.AddDays(1).ToShortDateString();
            SendHourDropDownList.SelectedValue = DateTime.Now.Hour.ToString();
        }
    }

    void OkButton_Click(object sender, EventArgs e)
    {
        // Validate the page
        if (!Page.IsValid)
        {
            return;
        }

        TblMessages rsMsg = new TblMessages();
        rsMsg.MsgFromID = WebConvert.ToInt32(CookieManager.SesAccID,0);
        rsMsg.MsgFromName = MsgFromNameTextBox.Text;
        rsMsg.MsgSummary = MsgSummaryTextBox.Text;
        rsMsg.MsgBody = MsgBodyTextBox.Text;
        rsMsg.MsgTS = DateTime.Now;
        rsMsg.MsgExpires = DateTime.Now.AddDays(WebConvert.ToInt32(SiteSettings.GetValue("DaysToExpireMessages"), 31));

        db.TblMessages.InsertOnSubmit(rsMsg);
        db.SubmitChanges();

        TblMessageJobs rsMsj = new TblMessageJobs();
        rsMsj.MsgID = rsMsg.MsgID;
        rsMsj.MjsID = WebConvert.ToInt32(MjsIDDropDownList.SelectedValue, 0);
        rsMsj.RolID = WebConvert.ToInt32(RolIDDropDownList.SelectedValue, 0);
        rsMsj.BusID = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);
        rsMsj.MsjSendTS = WebConvert.ToDateTime(SendDateTextBox.Text, DateTime.Today);
        rsMsj.MsjSendTS = rsMsj.MsjSendTS.AddHours(WebConvert.ToInt32(SendHourDropDownList.SelectedValue, 0));

        db.TblMessageJobs.InsertOnSubmit(rsMsj);
        db.SubmitChanges();

        // Redirect to target view page
        Response.Redirect("MsjView.aspx?ID=" + rsMsj.MsjID.ToString());
    }
}