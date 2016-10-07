using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for ResourceFileManager
/// </summary>
public class ResourceFileManager
{
    private readonly string RootFilePath;

    public ResourceFileManager( string rootFilePath = null )
    {
        this.RootFilePath = rootFilePath ?? Path.Combine( HttpContext.Current.Server.MapPath( "~" ), "Resources" );
    }

    public IEnumerable<ResourceFileModel> GetFiles( )
    {
        List<ResourceFileModel> files = new List<ResourceFileModel>( );
        foreach ( string file in Directory.EnumerateFiles( this.RootFilePath ) )
        {
            files.Add( new ResourceFileModel( new FileInfo( file ) ) );
        }
        return files;
    }

    public bool Upload( HttpPostedFile file, out string filename, out string error, bool overwrite = false )
    {
        // Clean up the filename
        filename = this.CleanFilename( file.FileName );

        // Define the path
        string path = Path.Combine( this.RootFilePath, filename );
        error = "";
        if ( !overwrite && File.Exists( path ) )
        {
            error = String.Format( "A file already exists at: {0}", path );
            return false;
        }

        // Attempt to save the file to disk
        try
        {
            file.SaveAs( path );
        }
        catch ( Exception ex )
        {
            error = ex.Message;
            return false;
        }

        return true;
    }

    public bool Delete( string filename, out string error, bool existsCheck = false )
    {
        string path = Path.Combine( this.RootFilePath, filename );
        bool exists = File.Exists( path );
        error = "";
        if ( existsCheck && !exists )
        {
            error = String.Format( "A file was not found at: {0}", path );
            return false;
        }
        else if ( !existsCheck && !exists )
        {
            return true;
        }

        // Attempt to delete the file
        try
        {
            File.Delete( path );
        }
        catch ( Exception ex )
        {
            error = ex.Message;
            return false;
        }

        return true;
    }

    private string RemoveAccent( string txt )
    {
        byte[] bytes = System.Text.Encoding.GetEncoding( "Cyrillic" ).GetBytes( txt );
        return System.Text.Encoding.ASCII.GetString( bytes );
    }

    private string CleanFilename( string origFilename )
    {
        string clean = RemoveAccent( origFilename ).ToLower( );

        // Remove all non valid chars
        clean = System.Text.RegularExpressions.Regex.Replace( clean, @"[^a-z0-9\s-\.]", "" );

        // Convert multiple spaces into one space
        clean = System.Text.RegularExpressions.Regex.Replace( clean, @"\s+", " " ).Trim( );

        // Replace spaces by dashes
        clean = System.Text.RegularExpressions.Regex.Replace( clean, @"\s", "-" );

        // Return the result
        return clean;
    }
}