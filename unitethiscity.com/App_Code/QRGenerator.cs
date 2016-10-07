/******************************************************************************
 * Filename: QRGnerator.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Utility class for generation of QR images using Google chart api
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Generate QR codes using the Google chart api
/// </summary>
public static class QRGenerator
{
    public static string ImageURL(string data, int size=100, int margin=4, string errorLevel="Low")
    {
        string url = string.Format("http://chart.apis.google.com/chart?cht=qr&chld={2}|{3}&chs={0}x{0}&chl={1}", size, HttpUtility.UrlEncode(data), errorLevel, margin);
        return url;
    }
}