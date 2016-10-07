<%@ WebHandler Language="C#" Class="OptOutDelete" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Sancsoft.Web;

public class OptOutDelete : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest( HttpContext context )
    {
        // make sure the user is logged in
        if ( !SiteAccount.IsLoggedIn( ) )
        {
            context.Response.Redirect( "/AccountLogin", false );
            return;
        }

        SiteAccount siteAccount = new SiteAccount();

        // Get/validate the item Id
        int optID = WebConvert.ToInt32( context.Request.QueryString["optID"], 0 );
        if ( optID <= 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }

        WebDBContext db = new WebDBContext( );

        // Get/validate the item
        TblOptOuts rsOpt = db.TblOptOuts.SingleOrDefault( target => target.AccID == siteAccount.AccID && target.OptID == optID );
        if ( rsOpt == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Delete the item
        db.TblOptOuts.DeleteOnSubmit( rsOpt );

        // Submit changes to the database
        db.SubmitChanges( );

        context.Response.Redirect( "OptOut" );
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}