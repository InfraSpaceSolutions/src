<%@ WebHandler Language="C#" Class="ResDelete" %>
using System;
using System.Web;
using System.Web.SessionState;
using Sancsoft.Web;

public class ResDelete : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest( HttpContext context )
    {
        // Force login
        if ( CookieManager.SesAccID == "" )
		{
			// Redirect to the login screen
			context.Response.Redirect( "Login.aspx" );
		}
        
        // Get the filename to be deleted
        string filename = WebConvert.ToString( context.Request["filename"], "" );
        if ( filename == "" )
        {
            throw new WebException( RC.DataIncomplete );
        }

        ResourceFileManager manager = new ResourceFileManager( );
        string error;
        if ( manager.Delete( filename, out error ) )
        {
            context.Response.Redirect( String.Format( "/admin/ResList.aspx?deleted={0}", filename ) );
            return;
        }

        throw new Exception( error );
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}