using System;

using System.Collections.Generic;

namespace WorkCloneCS;
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
            Logger.Log("staff synced");
        } catch(Exception ex) { Logger.Log(ex.Message); }
    }

    public static void syncCatagory()
    {
        Logger.Log("entered sync catagory");
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

    public static void getFiles()
    {
        string dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workCloneCs";
        string sqlDir = dir + "/sql/";
        if (Directory.Exists(sqlDir))
        {
            if (File.Exists(sqlDir + "catagoryJson.txt"))
            {
                try
                {
                    List<catagory> j = SQL.pullCatFile();
                    catagories = j;
                    int min = j[0].catagoryId;
                    int max = j[0].catagoryId;
                    foreach (catagory cat in j)
                    {
                        if (cat.catagoryId < min)
                        {
                            min = cat.catagoryId;
                        }

                        if (cat.catagoryId > max)
                        {
                            max = cat.catagoryId;
                        }
                    }

                    catagoryIdRange = (min, max);
                }
                catch (Exception ex)
                {
                    Logger.Log($"ex {ex}");
                }
            }
           
            if (File.Exists(sqlDir+"staff.txt"))
            {
                try
                {
                    allStaff = SQL.errorCallSD(null);
                }
                catch (Exception ex)
                {
                    Logger.Log($"excetion in the getFiles function {ex}");
                }
            }

        }
        else
        {
            Logger.Log("this shouldnt have ever ran but i dont really know what to say probs " +
                       "something to do with permissions so i would start there");
        }
        
    }
    
    public static void syncAll()
    {
        Task.Run(() =>
        {
            DateTime start = DateTime.Now;
            SQL.initSQL();
            syncStaff();
            Logger.Log("just about to go into syncCatagory");
            syncCatagory();
            
            Logger.Log($"sync took {(DateTime.Now - start).TotalSeconds:F5} seconds");
        });
    }
}
