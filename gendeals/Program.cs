/******************************************************************
 *  Filename : PROGRAM.CS
 *  Project  : GENDEALS.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Commmand line program for generating deals in the UTC database
 *  based on the currently assigned deals to businesses.
 *  
 *  Usage:      GENDEALS.EXE
 *  Example:    GENDEALS.ESE
 *  
 ******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace com.unitethiscity
{
    class Program
    {
        protected static LogConsole log;
        protected static WebDBContext db;
        protected static int perID;
        protected static DateTime perDate;

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">command line arguments</param>
        static int Main(string[] args)
        {
            // create the output logfile
            log = new LogConsole();
            log.Open();

            // output the version and operational information
            // output program version information
            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
            log.WriteLine(assemblyName.Name + " Version " + assemblyName.Version.ToString());
            log.WriteLine("Copyright (c) 2013 - Sanctuary Software Studio, Inc.");

            // create a connection to the target database
            db = new WebDBContext();

            // work with current date
            perDate = DateTime.Now;
            perID = IdentifyPeriod(perDate);
            log.WriteLine(String.Format("Period #{0} for {1:yyyy-MM-dd}", perID, perDate));
            if (perID == 0)
            {
                log.WriteLine("ERROR - Period undefined. Cannot generate deals for undefined period");
                return -1;
            }

            // process the deal information for each business in the database
            log.WriteLine(String.Format("Processing {0} businesses.", db.TblBusinesses.Count()));
            IEnumerable<TblBusinesses> rsBus = db.TblBusinesses.OrderBy(target=>target.BusID);

            // analyze and process each business
            foreach (TblBusinesses bus in rsBus)
            {
                log.Write( bus.BusID.ToString("0000 ") + bus.BusName + ": ");
                UpdateDeal(bus);
            }
            log.Close();
            return 0;
        }

        /// <summary>
        /// Identify the period for the 
        /// </summary>
        /// <param name="dt">date within period</param>
        /// <returns>period id, 0 if not found</returns>
        public static int IdentifyPeriod(DateTime dt)
        {
            int ret = 0;

            // back up the end date by 24 hours so that we get an inclusive comparison
            DateTime enddt = dt.AddDays(-1);
            TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerStartDate <= dt && target.PerEndDate > enddt);
            if (rs != null)
            {
                ret = rs.PerID;
            }
            return ret;
        }

        /// <summary>
        /// Update the deal based on the associated deal definition for this business
        /// </summary>
        /// <param name="rsBus">target business</param>
        public static void UpdateDeal(TblBusinesses rsBus)
        {
            // get the currently assigned deal definition if one exists that is assigned to the business
            TblDealDefinitions dealDef = db.TblDealDefinitions.SingleOrDefault(target => target.DldID == rsBus.BusAssignedDldID);

            // get the currently assigned deal if one exists
            TblDeals deal = db.TblDeals.SingleOrDefault(target => target.PerID == perID && target.BusID == rsBus.BusID);

            // no deal defined for this business - there should be no live deal
            if (dealDef == null)
            {
                // if we have a deal, we need to remove it? Let's just 0 out the number
                if (deal != null)
                {
                    deal.DelAmount = 0;
                    db.SubmitChanges();
                    log.WriteLine(String.Format("ZEROED delid#{0} - deal no longer defined", deal.DelID));
                    return;
                }

                log.WriteLine("IGNORED - no deal defined for period");
                return;
            }

            // we have a deal defined - lets make (or update) a deal
            
            // handle the case where we don't have an existing deal
            if (deal == null)
            {
                // make a new deal record and insert it into the database
                deal = new TblDeals();
                deal.BusID = dealDef.BusID;
                deal.PerID = perID;
                deal.DelName = dealDef.DldName;
                deal.DelAmount = dealDef.DldAmount;
                deal.DelDescription = dealDef.DldDescription;
                deal.DelCustomTerms = dealDef.DldCustomTerms;
                db.TblDeals.InsertOnSubmit(deal);
                db.SubmitChanges();
                log.WriteLine(String.Format("CREATED delid#{0} dld#{1} {2:F2}", deal.DelID, dealDef.DldID, deal.DelAmount));
                return;
            }

            // handle the case where a deal is already defined - needs to be updated
            if (deal != null)
            {
                bool changed = false;
                // update the existing deal with the new value
                if (deal.DelName != dealDef.DldName)
                {
                    deal.DelName = dealDef.DldName;
                    changed = true;
                }
                if ( deal.DelAmount != dealDef.DldAmount )
                {
                    deal.DelAmount = dealDef.DldAmount;
                    changed = true;
                }
                if (deal.DelDescription != dealDef.DldDescription)
                {
                    deal.DelDescription = dealDef.DldDescription;
                    changed = true;
                }
                if (deal.DelCustomTerms != dealDef.DldCustomTerms)
                {
                    deal.DelCustomTerms = dealDef.DldCustomTerms;
                    changed = true;
                }

                // if we found a change - update the record and indicate in the log output
                if (changed)
                {
                    db.SubmitChanges();
                    log.WriteLine(String.Format("UPDATED delid#{0} dld#{1} {2:F2}", deal.DelID, dealDef.DldID, deal.DelAmount));
                }
                else
                {
                    log.WriteLine(String.Format("SKIPPED delid#{0} dld#{1} {2:F2}", deal.DelID, dealDef.DldID, deal.DelAmount));
                }
                return;
            }

            log.WriteLine("ERROR - Unexpected or unprocessed state for the deal on this business");
        }
    }
}
