using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for ResourceFileManager
/// </summary>
public class ResourceFileModel
{
    public string Filename { get; set; }
    public long Length { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }

    /// <summary>
    /// Relative download url.
    /// </summary>
    public string DownloadUrl
    {
        get
        {
            return String.Format( "/Resources/{0}", this.Filename );
        }
    }

    /// <summary>
    /// Relative admin delete url.
    /// </summary>
    public string DeleteUrl
    {
        get
        {
            return String.Format( "/admin/ResDelete.ashx?filename={0}", this.Filename );
        }
    }

    /// <summary>
    /// File size display based on length calculated in B, KB, MB, GB, or TB.
    /// </summary>
    public string FileSize
    {
        get
        {
            int unit = 1024;

            // Default to bytes
            double size = (double)this.Length;
            string suffix = "B";

            // Evaluate the B size and
            // convert to KB accordingly
            if ( size > unit )
            {
                // Calculate the kilobytes
                size = Math.Floor( size / unit );
                suffix = "KB";
            }

            // Evaluate the KB size and
            // convert to MB accordingly
            if ( size > unit )
            {
                // Calculate the megabytes
                size = Math.Floor( size / unit );
                suffix = "MB";
            }

            // Evaluate the MB size and
            // convert to GB accordingly
            if ( size > unit )
            {
                // Calculate the gigabytes
                size = Math.Floor( size / unit );
                suffix = "GB";
            }

            // Evaluate the GB size and
            // convert to TB accordingly
            if ( size > unit )
            {
                // Calculate the gigabytes
                size = Math.Floor( size / unit );
                suffix = "TB";
            }

            // Return the result
            return String.Format( "{0} {1}", size, suffix );
        }
    }

    public ResourceFileModel( )
    {
        this.Filename = "";
        this.Length = 0;
        this.CreationTime = DateTime.Now;
        this.LastWriteTime = DateTime.Now;
    }


    public ResourceFileModel( FileInfo fileInfo )
    {
        this.Filename = fileInfo.Name;
        this.Length = fileInfo.Length;
        this.CreationTime = fileInfo.CreationTime;
        this.LastWriteTime = fileInfo.LastWriteTime;
    }
}