using System;

using System.Collections.Generic;

namespace WorkCloneCS
{
    class sync
    {
        public static (int min, int max) catagoryIdRange { get; set; }
        public static List<staff> allStaff { get; set; }
        public static List<catagory> catagories { get; set; }

        public static void syncStaff()
        {
            try
            {
                allStaff = SQL.getStaffData();
            } catch(Exception ex) { Logger.Log(ex.Message); }
        }

        public static void syncCatagory()
        {
            catagories = new List<catagory>();
            catagoryIdRange = SQL.getRangeOfCatagoryID();
            Logger.Log(catagoryIdRange.Item1.ToString());
            
            Logger.Log($"max? {catagoryIdRange.min}, min? {catagoryIdRange.max}");
            
            // Create a temporary list to store categories
            List<catagory> tempCategories = new List<catagory>();
            
            //catagories section
            for (int i = 1; i <= catagoryIdRange.max; i++)
            {
                var x = SQL.getCatagory(i);
                if (x != null) tempCategories.Add(x);
                Logger.Log($"currently going through: {i}");
            }

            // After collecting all categories, assign them to the main list
            catagories = tempCategories;
            
            // Log the categories
            if (catagories.Count > 1)
            {
                foreach (catagory cat in catagories)
                {
                    Logger.Log($"{cat.catName}");
                }
            }
        }
        
        public static void syncAll()
        {
            Task.Run(() =>
            {
                DateTime start = DateTime.Now;
                SQL.initSQL();
                syncStaff();
                syncCatagory();
                Logger.Log($"sync took {(DateTime.Now - start).TotalSeconds:F5} seconds");
            });
        }
    }
}