namespace WorkCloneCS;

static class Logger
{
    private static readonly string logFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "workclonecs", "log.txt");
    private static int logCount;
    private static readonly object _lock = new object();

    static Logger()
    {
        var logDir = Path.GetDirectoryName(logFilePath);
        if (!string.IsNullOrWhiteSpace(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
    }

    public static void Here()
    {
        Log($"here{logCount}");
    }


    public static void Log(string? message)
    {
        lock (_lock)
        {
            try
            {
                if (message == null) message = "logger was called with null message";
                if (!File.Exists(logFilePath)) File.Create(logFilePath).Dispose();
                logCount++;
                string number = logCount.ToString();
                string formattedCount = number.PadLeft(4, '-');
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
                string logEntry = $"{formattedCount}:{timestamp}: {message}";
                Console.WriteLine(logEntry);
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }
    }

}
