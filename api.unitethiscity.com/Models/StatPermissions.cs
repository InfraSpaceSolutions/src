/******************************************************************************
 * Filename: StatPermissions.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing statistics permissions for a user account
******************************************************************************/

namespace com.unitethiscity.api.Models
{
    public class StatPermissions
    {
        public int AccId { get; set; }
        public bool HasGlobalStatistics { get; set; }
        public bool HasGlobalAnalytics { get; set; }
        public bool HasBusinessStatistics { get; set; }
        public bool HasBusinessAnalytics { get; set; }
    }
}