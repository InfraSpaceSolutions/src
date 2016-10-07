/******************************************************************************
 * Filename: CatNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new category.
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

public partial class admin_CatNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );

        if (!Page.IsPostBack)
        {
            LoadEligibleParentCategories();
        }
	}

    void LoadEligibleParentCategories()
    {
        // Get the eligible parent pages
        List<VwCategories> rsCat =
            (from cat in db.VwCategories.Where( target => target.CatParentID == 0 )
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

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        int catParentID = WebConvert.ToInt32(CatParentIDDropDownList.SelectedValue, 0);

		// Create the record
		TblCategories rs = new TblCategories();

		// Populate fields
        rs.CatParentID = catParentID;
        rs.CatName = WebConvert.Truncate( CatNameTextBox.Text.Trim( ), 50 );
        rs.CatDescription = CatDescriptionTextBox.Text.Trim();

		// Submit to the db
		db.TblCategories.InsertOnSubmit( rs );
		db.SubmitChanges();

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.Categories);

        // Redirect to the view page
        Response.Redirect( "CatView.aspx?ID=" + rs.CatID.ToString( ) );
	}
}