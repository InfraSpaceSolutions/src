using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_ErrorDisplay : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		string title;
		string message;
		string rc = WebConvert.ToString( Request.QueryString["RC"], "" );

		if ( rc.Length == 0 )
		{
			rc = "InternalError";
		}

		WebException ex = new WebException( (RC)Enum.Parse( typeof( RC ), rc, true ) );

		ex.GetError( out title, out message );
		ErrHeaderLabel.Text = title;
		ErrMessage.Text = message;
    }
}
