/******************************************************************************
 * Filename: CatEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an category.
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

public partial class admin_CatEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
		SubmitButton.Click += new EventHandler( OkButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		if ( !Page.IsPostBack )
		{
			// Get the target record
            VwCategories rs = db.VwCategories.SingleOrDefault( target => target.CatID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

            LoadEligibleParentCategories();

			// Populate the page
            CatIDLiteral.Text = rs.CatID.ToString();
            CatParentIDDropDownList.SelectedValue = rs.CatParentID.ToString();
            CatNameTextBox.Text = rs.CatName;
            CatDescriptionTextBox.Text = rs.CatDescription;
		}
	}

    void LoadEligibleParentCategories()
    {
        // Get the eligible parent pages
        List<VwCategories> rsCat =
            (from cat in db.VwCategories.Where(target => target.CatParentID == 0)
             orderby cat.CatParentName, cat.CatName
             select cat).ToList();

        CatParentIDDropDownList.Items.Clear();
        CatParentIDDropDownList.Items.Add(new ListItem("Select...", ""));
        CatParentIDDropDownList.Items.Add(new ListItem("None[no parent]", "0"));
        foreach (var cat in rsCat)
        {
            if (cat.CatParentID != 0)
            {
                ListItem li = new ListItem(cat.CatParentName + ": " + cat.CatName, cat.CatID.ToString());
                CatParentIDDropDownList.Items.Add(li);
            }
            else
            {
                ListItem li = new ListItem(cat.CatName, cat.CatID.ToString());
                CatParentIDDropDownList.Items.Add(li);
            }
        }
    }

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

        int catParentID = WebConvert.ToInt32(CatParentIDDropDownList.SelectedValue, 0);

		// Get the record
        TblCategories rs = db.TblCategories.SingleOrDefault( target => target.CatID == id  );

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Update the record
        rs.CatParentID = catParentID;
        rs.CatName = WebConvert.Truncate(CatNameTextBox.Text.Trim(), 50);
        rs.CatDescription = CatDescriptionTextBox.Text.Trim();

		// Sync to database
		db.SubmitChanges();

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.Categories);

        // Redirect to target view page
        Response.Redirect( "CatView.aspx?ID=" + id.ToString( ) );
	}
}