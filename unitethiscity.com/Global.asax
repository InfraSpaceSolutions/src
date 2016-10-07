<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        // Debug setting
        bool isDebug = WebConvert.ToBoolean( ConfigurationManager.AppSettings["IS_DEBUG"], false );
        if ( isDebug )
        {
            // Friendly errors turned OFF in debug
            return;
        }
        
		// Code that runs when an unhandled error occurs
		string rc = "InternalError";

		// Get the exception and evaluate if its a web exception
		Exception ex = Server.GetLastError().GetBaseException();
		if ( ex is Sancsoft.Web.WebException )
		{
            Sancsoft.Web.WebException wex = (Sancsoft.Web.WebException)ex;
			rc = wex.ReturnCode.ToString();
		}
		else if ( ex is HttpException )
		{
			HttpException httpex = (HttpException)ex;

			if ( httpex.GetHttpCode() == 404 )
			{
				rc = "PageNotFound";
			}
		}

		// Redirect to a friendly error page
		Server.ClearError();
		Response.Clear();
		Response.Redirect( "ErrorDisplay.aspx?rc=" + rc );
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
