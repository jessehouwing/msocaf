namespace SharePointCustomRules
{
    using System;
    using System.IO;
    using System.Xml;

    public static class Logging
    {
        private static int LogCount = 0;
        public static LogSeverity LogSeverityValue;

        public static void Initialize(string sRuleName)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                if (LogCount.Equals(0))
                {
                    LogSeverityValue = LogSeverity.ERROR;
                }
            }
            catch (Exception exception)
            {
                UpdateLog(exception.Message);
            }
        }

        public static void UpdateLog(string sLog)
        {
            string path = Directory.GetCurrentDirectory() + @"\CustomRulesLog.txt";
            StreamWriter writer = null;
            try
            {
                if (File.Exists(path))
                {
                    writer = File.AppendText(path);
                }
                else
                {
                    writer = File.CreateText(path);
                }
                writer.WriteLine('\n' + sLog);
                writer.Close();
            }
            catch (IOException exception)
            {
                if (writer != null)
                {
                    writer.WriteLine('\n' + exception.Message);
                    writer.Close();
                }
            }
        }

        public enum LogSeverity
        {
            DEBUG,
            ERROR
        }
    }
}

