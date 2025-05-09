using System;
using System.Collections.Generic;

namespace WorkCloneCS
{
    class sync
    {
        public static (int min, int max) catagoryIdRange { get; set; }
        public static List<staff> allStaff { get; set; }
        public static List<catagory> catagories { get; set; }


        public static void syncAll()
        {
            catagories = new List<catagory>();
            catagoryIdRange = SQL.getRangeOfCatagoryID();
            Logger.Log(catagoryIdRange.Item1.ToString());
            allStaff = SQL.getStaffData();
            Logger.Log($"max? {catagoryIdRange.min}, min? {catagoryIdRange.max}");
            //catagories section
            for (int i = 1; i <= catagoryIdRange.max; i++)
            {
                var x = SQL.getCatagory(i);
                if (x != null) catagories.Add((x));
                Logger.Log($"currently going through: {i}");
            }
            foreach (catagory cat in catagories)
            {
                if (catagories.Count > 1)
                {
                    Logger.Log($"{cat.catName}");
                }

            }
        }
    }
}