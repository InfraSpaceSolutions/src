<%@ WebHandler Language="C#" Class="GalleryItemDelete" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Sancsoft.Web;

public class GalleryItemDelete : IHttpHandler, IRequiresSessionState
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
        int galID = WebConvert.ToInt32( context.Request.QueryString["galID"], 0 );
        if ( galID <= 0 )
        {
            throw new WebException( RC.DataIncomplete );
        }
        
        WebDBContext db = new WebDBContext( );

        // Get/validate the item
        TblGalleryItems rsGal = db.TblGalleryItems.SingleOrDefault( target => target.BusID == siteBusiness.BusID && target.GalID == galID );
        if ( rsGal == null )
        {
            throw new WebException( RC.TargetDNE );
        }
        
        // Update the sequence
        IEnumerable<TblGalleryItems> rsSeq = db.TblGalleryItems.Where( rows => rows.BusID == siteBusiness.BusID && rows.GalSeq > 1 && rows.GalSeq > rsGal.GalSeq );
        foreach ( var row in rsSeq )
        {
            row.GalSeq = row.GalSeq - 1;
        }
        
        // Delete the file on disk if it exists
        string filePath = SiteSettings.GetValue( "BusinessGalleryPath" ) + rsGal.GalGuid.ToString( ) + ".png";
        if ( File.Exists( filePath ) )
        {
            File.Delete( filePath );
        }
        
        // Delete the item
        db.TblGalleryItems.DeleteOnSubmit( rsGal );
        
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