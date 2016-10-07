/******************************************************************************
 * Filename: CuteEditorConfig.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Configuration class for various toolbar configurations of the Cute Editor 
 * HTML Editor.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CuteEditor;

/// <summary>
/// Configuration class for various toolbar configurations of the Cute Editor textbox.
/// </summary>
public class CuteEditorConfig
{
	/// <summary>
	/// Standard configuration settings for the Cute Editor textbox used on administration website.
	/// </summary>
	/// <param name="objCE">Cute Editor textbox control.</param>
	public static void SetToolbarDefaultAdmin( Editor objCE, string ImagePathSetting = "ManagedImagesPath" )
	{
        // Load the public website stylesheet
        objCE.EditorWysiwygModeCss = "~/Styles/admin.css";

        // Set the css class for the admin page body editor
        objCE.EditorBodyClass = "PageBodyEditor";
        objCE.EditorBodyStyle = "background-color:#ffffff";
        objCE.EditorBodyStyle = "background-image:none";

        // Turn off the context menu
        objCE.EnableContextMenu = false;

        // Set up the managed images gallery file path location
        string managedImagesPath = SiteSettings.GetValue( ImagePathSetting );
        objCE.SetSecurityImageGalleryPath( managedImagesPath );
        objCE.SetSecurityImageBrowserPath( managedImagesPath );

        // Turn off insert image toolbar items
        objCE.SetSecurityAllowModify( false );
        objCE.SetSecurityAllowCreateFolder( false );
        objCE.SetSecurityAllowCopy( false );
        objCE.SetSecurityAllowMove( false );
        objCE.SetSecurityAllowRename( false );

        // Define and add the available tools on the standard toolbar
		string tools = "";
		tools += "{CssClass,FormatBlock,FontSize";
		tools += " | netspell,Cut,Copy,Paste,Undo,Redo,Find";
		tools += " | Bold,Italic,Underline,JustifyLeft,JustifyCenter,JustifyRight";
		tools += " | Forecolor,Backcolor,InsertOrderedList,InsertUnorderedList,Indent,Outdent,InsertChars,InsertLink";
		tools += " | InsertImage}";

		// Add the standard toolbar
		objCE.TemplateItemList = tools;

		// Add the supported CSS classes
        string cssClasses = "imgRight,image right; imgLeft,image left";
		ToolControl cssTools = objCE.ToolControls["CssClass"];
		if ( cssTools != null )
		{
			RichDropDownList cssDdl = (RichDropDownList)cssTools.Control;
			cssDdl.RenderItemBorder = false;
			RichListItem itmCss = cssDdl.Items[0];
			cssDdl.Items.Clear();
			cssDdl.Items.Add( itmCss );

			char[] cssDelimeter = { ';' };
			char[] cssPairDelimeter = { ',' };
			foreach ( var cssNamePair in cssClasses.Split( cssDelimeter ) )
			{
				ArrayList arrCSS = new ArrayList( cssNamePair.Split( cssPairDelimeter ) );
				if ( arrCSS.Capacity == 2 )
				{
					cssDdl.Items.Add( arrCSS[1].ToString(), arrCSS[0].ToString() );
				}
			}
		}

        // Configure the font-size dropdown to use pixels instead of points
        string fontSizes = "10;11;12;14;16;18;20;22;24;26";
        ToolControl fontSizeTools = objCE.ToolControls["FontSize"];
        if ( fontSizeTools != null )
        {
            RichDropDownList fontSizeDdl = (RichDropDownList)fontSizeTools.Control;
            RichListItem itmFontSize = fontSizeDdl.Items[0];
            fontSizeDdl.Items.Clear( );
            fontSizeDdl.Items.Add( itmFontSize );

            char[] fontSizeDelimeter = { ';' };
            foreach ( var fontSize in fontSizes.Split( fontSizeDelimeter ) )
            {
                fontSizeDdl.Items.Add( "<span style=\"font-size:" + fontSize + "px;\">" + fontSize + "px" + "</span>", fontSize + "px", fontSize );
            }
        }
	}
}