using System;

using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace WorkCloneCS;
class sync
{
    public static (int min, int max) categoryIdRange { get; set; }
    public static List<staff> allStaff { get; set; }
    public static List<category> categories { get; set; }

    public static void syncStaff()
    {
        try
        {
            database.updateStaff();
            Logger.Log("staff synced");
        } catch(Exception ex) { Logger.Log(ex.Message); }
    }

    public static void syncCategory()
    {
        Logger.Log("entered sync category");
        categories = new List<category>();
        categoryIdRange = SQL.getRangeOfCategoryID();
        Logger.Log(categoryIdRange.Item1.ToString());
        
        Logger.Log($"max? {categoryIdRange.min}, min? {categoryIdRange.max}");
        
        // Create a temporary list to store categories
        List<category> tempCategories = new List<category>();
        
        //categories section
        for (int i = 1; i <= categoryIdRange.max; i++)
        {
            var x = SQL.getCategory(i);
            if (x != null) tempCategories.Add(x);
            Logger.Log($"currently going through: {i}");
        }

        // After collecting all categories, assign them to the main list
        categories = tempCategories;
        
        // Log the categories
        if (categories.Count > 1)
        {
            foreach (category cat in categories)
            {
                Logger.Log($"{cat.catName}");
            }
        }
    }

    public static bool checkCatFile()
    {
        string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workCloneCs/sql/categoryJson.txt";
        if (!File.Exists(path)) return false;
        try
        {
            string json = File.ReadAllText(path); // Read file contents
            List<category> fileJson = JsonSerializer.Deserialize<List<category>>(json); // Deserialize JSON text
            if (fileJson[0].categoryId == 0 || fileJson[0].catName == null || fileJson[0].items == null)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"error in checkCaatFile {ex.Message}");
            return false;
        }

        return true;
    }
    public static void getFiles()
    {
        string dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workCloneCs";
        string sqlDir = dir + "/sql/";
        if (Directory.Exists(sqlDir))
        {
            if (checkCatFile())
            {
                try
                {
                    List<category> j = SQL.pullCatFile();
                    categories = j;
                    int min = j[0].categoryId;
                    int max = j[0].categoryId;
                    foreach (category cat in j)
                    {
                        if (cat.categoryId < min)
                        {
                            min = cat.categoryId;
                        }

                        if (cat.categoryId > max)
                        {
                            max = cat.categoryId;
                        }
                    }

                    categoryIdRange = (min, max);
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
                    allStaff = SQL.staffreturnthing(null);
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
            Logger.Log("just about to go into syncStaff");
            syncStaff();
            Logger.Log("synced staff");
            
            Logger.Log("just about to go into syncCategory");
            syncCategory();
            Logger.Log("synced category");
            
            Logger.Log($"sync took {(DateTime.Now - start).TotalSeconds:F5} seconds");
            int cloudV = SQL.getDatabaseVNum();
            File.WriteAllText($@"{SQL.dir}\sql\DBvNum.txt", $"{cloudV}");
            Logger.Log("db version updated in sync all");
        });
    }


    
    
}
