using System.Text.Json;
namespace WorkCloneCS;

static partial class SQL
{
    // only used in getCategory when catch is called
    private static category errorCallGC(Exception ex, int categoryChosen)
    {
        Logger.Log("\ncouldn't connect so am resorting to backup errorCallGC  \n");
        Logger.Log(ex.Message);
        Logger.Here();
        if (categoriesFromFile != null)
        {
            Logger.Log("tbf i think it worked just have a quick look tbf");
            foreach (category cat in categoriesFromFile)
            {
                Logger.Log($"catID {cat.categoryId}, chosen cat: {categoryChosen}");
                if (cat.categoryId == categoryChosen) return cat;
            }
        }

        Logger.Log("categories file or cat doesnt exist so :( errorcallGC ");
        return null;
    }

    //only using in getRangeOfCategoryID when catch is called
    private static (int, int) errorCallCI(Exception ex)
    {
        int min = 0;
        int max = 0;
        Logger.Log($"{ex.Message} errorCallCI");
        List<int> d = new List<int>();
        //couldnt connect or something so 
        if (categoriesFromFile != null)
        {
            foreach (category cat in categoriesFromFile)
            {
                d.Add(cat.categoryId);
            }

            min = d.Min();
            max = d.Max();
        }
        // else basically means we are screwed icl
        else
        {
            min = 0;
            max = 0;
        }

        return (min, max);
    }

    //only used in getStaffData when catch is called
    private static List<staff> errorCallSD(Exception ex)
    {
        Logger.Log($"{ex.Message} errorCallSD");
        Console.WriteLine("Error: " + ex.Message);
        string file =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt";
        return staffreturnthing(file);
    }

    private static List<staff> getStaffFromFile()
    {
        
            if (!File.Exists(jsonstaffDir)) return null;
            return JsonSerializer.Deserialize<List<staff>>(
                    File.ReadAllText(jsonstaffDir));
        
    }
    
    private static void ErrorCallIS(Exception ex)
    {
        if (ex != null) Logger.Log(ex.Message);
        if (!created)
        {
            created = true;
            Task.Run(() => MessageBox.Show(
                "it is recomened for you to go through the config settings," +
                "\n you are currently using the backup database on your local device," +
                "\n you will not be able to send an order through in this state"));
        }
    }

    
}
