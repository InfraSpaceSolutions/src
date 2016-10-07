/******************************************************************************
 * Filename: CatView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an category.
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

public partial class admin_CatView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		EditButton.Click += new EventHandler( EditButton_Click );
		DeleteButton.Click += new EventHandler( DeleteButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // Get the record
        VwCategories rs = db.VwCategories.SingleOrDefault(target => target.CatID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Bind data to children pages repeater
        RefreshChildrenCategories();

        if (rs.CatParentID != 0)
        {
            AssignedBusinessesPanel.Visible = true;
            // Bind data to assigned businesses repeater
            RefreshBusinesses();
        }
        else
        {
            AssignedBusinessesPanel.Visible = false;
        }

		if ( !Page.IsPostBack )
		{
			// Populate the page
            CatIDLiteral.Text = rs.CatID.ToString();
            CatParentNameLiteral.Text = WebConvert.ToString( rs.CatParentName, "None" );
            CatNameLiteral.Text = rs.CatName;
            CatDescriptionLiteral.Text = WebConvert.PreserveBreaks( rs.CatDescription );
		}
	}

    void RefreshChildrenCategories()
    {
        // Get children pages to populate repeater
        ChildrenCategoriesRepeater.DataSource = db.TblCategories.Where(target => target.CatParentID == id).OrderBy(target => target.CatName);
        ChildrenCategoriesRepeater.DataBind();

        // Show empty row if there are no children pages
        if (db.TblCategories.Count(target => target.CatParentID == id) == 0)
        {
            NoChildrenRow.Visible = true;
        }
    }

    void RefreshBusinesses()
    {
        // Get children pages to populate repeater
        BusinessesRepeater.DataSource = db.VwBusinessCategories.Where(target => target.CatID == id).OrderBy(target => target.BusName);
        BusinessesRepeater.DataBind();

        // Show empty row if there are no children pages
        if (db.VwBusinessCategories.Count(target => target.CatID == id) == 0)
        {
            NoBusinessesRow.Visible = true;
        }
    }

	void DeleteButton_Click( object sender, EventArgs e )
	{
        // Does this category have any children cateogies( if so, then dont allow removal of category )
        if (db.TblCategories.Count(target => target.CatParentID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

        // Is this category currently assigned to a business?( if so, then dont allow removal of category )
        if (db.VwBusinessCategories.Count(target => target.CatID == id) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

		// Get the record
        TblCategories rs = db.TblCategories.Single( Target => Target.CatID == id );

		// Get account
        string name = rs.CatName + "(" + rs.CatID.ToString() +")";

		// Delete the record
		db.TblCategories.DeleteOnSubmit( rs );
		db.SubmitChanges();

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.Categories);

        // Redirect to list page
        Response.Redirect( "CatList.aspx?Name=" + HttpUtility.UrlEncode(name));
	}

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect( "CatEdit.aspx?ID=" + id.ToString( ) );
	}
}