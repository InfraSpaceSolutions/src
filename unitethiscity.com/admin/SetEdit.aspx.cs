/******************************************************************************
 * Filename: SetEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit the value for a website setting.
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

public partial class admin_SetEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );

		// Get target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // Configure the Cute Editor
        CuteEditorConfig.SetToolbarDefaultAdmin(SetHtmlEditor);

		if ( !Page.IsPostBack )
		{ 
			// Get target record
			TblSettings rs = db.TblSettings.SingleOrDefault( target => target.SetID == id );

			// Verify record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate page
			SetIDLiteral.Text = rs.SetID.ToString();
			SetNameLiteral.Text = rs.SetName;
			
			// Display correct input control for the setting type
			switch ( rs.SetType )
			{
				case 1:
                    SingleLinePanel.Visible = true;
					SetValueSingleTextBox.Text = rs.SetValue;
					break;
				case 2:
                    MultipleLinePanel.Visible = true;
					SetValueMultiTextBox.Text = rs.SetValue;
					break;
				case 3:
                    HTMLPanel.Visible = true;
					SetHtmlEditor.Text = rs.SetValue;
					break;
				default:
                    SingleLinePanel.Visible = true;
                    SetValueSingleTextBox.Text = rs.SetValue;
                    break;
            }
		}
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Get setting record
		TblSettings rs = db.TblSettings.SingleOrDefault( target => target.SetID == id );

		// Verify record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Update the setting value
		switch ( rs.SetType )
		{
			case 1: 
                rs.SetValue = SetValueSingleTextBox.Text.Trim();
				break;
			case 2: 
                rs.SetValue = SetValueMultiTextBox.Text.Trim();
				break;
			case 3:
                rs.SetValue = SetHtmlEditor.Text;
				break;
			default:
                rs.SetValue = SetValueSingleTextBox.Text.Trim();
				break;
		} 

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
		Response.Redirect( "SetList.aspx" );
	}
}