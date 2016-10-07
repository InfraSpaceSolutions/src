/******************************************************************************
 * Filename: PagChildSequence.aspx.cs
 * Project:  thebonnotco.com Administration
 * 
 * Description:
 * Manage the sequence number for children pages.
 * 
 * Revision History:
 * $Log: /Caler/Bonnot/thebonnotco.com/website/admin/PagChildSequence.aspx.cs $
 * 
 * 1     3/29/11 10:09a Ncross
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using Sancsoft.Web;

public partial class admin_PagChildSequence : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		ChildrenPagesRepeater.ItemDataBound += new RepeaterItemEventHandler( ChildrenPagesRepeater_ItemDataBound );

		// Get target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		// Bind appropriate children pages to repeater
		RefreshChildrenPages();

		if ( !Page.IsPostBack )
		{
			// Get target page record
			VwPages rs = db.VwPages.SingleOrDefault( target => target.PagID == id );

			// Verify page record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate page labels
			PagNameLiteral.Text = rs.PagName;
		}
	}

	void RefreshChildrenPages()
	{
		// Bind appropriate children pages to repeater
		ChildrenPagesRepeater.DataSource = db.TblPages.Where( target => target.PagParentID == id ).OrderBy( target => target.PagSequence );
		ChildrenPagesRepeater.DataBind();

		// Show empty row if no data in repeater
		if ( db.TblPages.Count( target => target.PagParentID == id ) == 0 )
		{
			NoChildrenRow.Visible = true;
		}
	}

	void ChildrenPagesRepeater_ItemDataBound( object sender, RepeaterItemEventArgs e )
	{
		// Get sequence link button controls
		LinkButton upSequence = (LinkButton)e.Item.FindControl( "UpSequenceLinkButton" );
		LinkButton downSequence = (LinkButton)e.Item.FindControl( "DownSequenceLinkButton" );

		// Get underlying datasource
		TblPages rs = (TblPages)e.Item.DataItem;

		// Wire sequence link button controls
		upSequence.CommandArgument = rs.PagID.ToString();
		upSequence.Command += new CommandEventHandler( upSequence_Command );
		downSequence.CommandArgument = rs.PagID.ToString();
		downSequence.Command += new CommandEventHandler( downSequence_Command );
	}

	void downSequence_Command( object sender, CommandEventArgs e )
	{
		TblPages rsSwap = null;

		// Get target page record
		TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

		// Verify record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Get list of all children pages for swap
		List<TblPages> pages = db.TblPages.Where( target => target.PagParentID == id ).OrderBy( target => target.PagSequence ).ToList();

		// Move the selected page down in sequence
		if ( rs.PagSequence < pages.Count )
		{
			// Get the page to swap with
			rsSwap = db.TblPages.SingleOrDefault( target => target.PagID == pages[rs.PagSequence].PagID );
		}

		if ( rsSwap != null )
		{
			// Swap the target page with the one below it
			int temp = rs.PagSequence;
			rs.PagSequence = rsSwap.PagSequence;
			rsSwap.PagSequence = temp;

			// Sync to database
			db.SubmitChanges();
		}

		// Refresh children pages list
		RefreshChildrenPages();
	}

	void upSequence_Command( object sender, CommandEventArgs e )
	{
		TblPages rsSwap = null;

		// Get the target page record
		TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == WebConvert.ToInt32( e.CommandArgument, 0 ) );

		// Verify record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Create list of all children pages
		List<TblPages> pages = db.TblPages.Where( target => target.PagParentID == id ).OrderBy( target => target.PagSequence ).ToList();

		// Move the page up
		if ( rs.PagSequence > 1 )
		{
			// Swap with the next page
			rsSwap = db.TblPages.SingleOrDefault( target => target.PagID == pages[rs.PagSequence - 2].PagID );
		}

		if ( rsSwap != null )
		{
			// Swap the target page with the one above it
			int temp = rs.PagSequence;
			rs.PagSequence = rsSwap.PagSequence;
			rsSwap.PagSequence = temp;

			// Sync to database
			db.SubmitChanges();
		}

		// Refresh the children pages list
		RefreshChildrenPages();
	}
}