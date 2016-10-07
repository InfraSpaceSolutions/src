/******************************************************************************
 * Filename: LocationContext.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials combined with a location
 * to support the detail view
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class LocationContext
    {
        // locationinfo components
        public int Id { get; set; }
        public int BusId { get; set; }
        public Guid BusGuid { get; set; }
        public int CitId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CatId { get; set; }
        public string CatName { get; set; }
        public List<string> Properties;
        public bool Entertainer { get; set; }


        // location/business details
        public string Summary { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string FacebookLink { get; set; }
        public bool RequiresPIN { get; set; }
        public string FacebookId { get; set; }

        // active deal information
        public int DealId { get; set; }
        public decimal DealAmount { get; set; }
        public string CustomTerms { get; set; }
        public string DealDescription { get; set; }
        public string DealName { get; set; }

        // member context
        public int AccId { get; set; }
        public string MyTip { get; set; }
        public double MyRating { get; set; }
        public bool MyIsFavorite { get; set; }
        public bool MyIsRedeemed { get; set; }

        public bool MyIsCheckedIn { get; set; }

        // action timestamps
        public string MyCheckInTime { get; set; }
        public string MyRedeemDate { get; set; }

        // more info data
        public int NumMenuItems { get; set; }
        public int NumGalleryItems { get; set; }
        public int NumEvents { get; set; }

        public string MenuLink { get; set; }

        // loyalty deal information
        public int PointsNeeded { get; set; }
        public int PointsCollected { get; set; }
        public string LoyaltySummary { get; set; }

        
    }
}