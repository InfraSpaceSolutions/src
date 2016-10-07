<%@ WebHandler Language="C#" Class="MenuItemDelete" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Sancsoft.Web;

public class MenuItemDelete : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest( HttpContext context )
    {
        // make sure the user is logged in
        if ( !SiteAccount.IsLoggedIn( ) )
        {
            context.Response.Redirect( "/AccountLogin", false );
            return;
        }
        
        // Get the business
        SiteBusiness siteBusiness = new SiteBusiness( );

        // Get/validate the item Id
        int menID = WebConvert.ToInt32( context.Request.QueryString["menID"], 0 );
        if ( menID <= 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }
        
        WebDBContext db = new WebDBContext( );

        // Get/validate the item
        TblMenuItems rsMen = db.TblMenuItems.SingleOrDefault( target => target.BusID == siteBusiness.BusID && target.MenID == menID );
        if ( rsMen == null )
        {
            throw new WebException( RC.TargetDNE );
        }
        
        // Update the sequence
        IEnumerable<TblMenuItems> rsSeq = db.TblMenuItems.Where( rows => rows.BusID == siteBusiness.BusID && rows.MenSeq > 1 && rows.MenSeq > rsMen.MenSeq );
        foreach ( var row in rsSeq )
        {
            row.MenSeq = row.MenSeq - 1;
        }
        
        // Delete the item
        db.TblMenuItems.DeleteOnSubmit( rsMen );
        
        // Submit changes to the database
        db.SubmitChanges( );
        
        context.Response.Redirect( "/Business/Dashboard?msg=Modify" );
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}