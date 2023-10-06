using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Caffeine
{
    internal static class SimpleLogger
    {
        public static void Error(Exception ex = null, string message = null)
        {
            StringBuilder sb = new StringBuilder();
            if (message != null)
                sb.AppendLine(message);

            while (ex != null)
            {
                sb.AppendLine(DateTime.Now.ToString("o"));
                sb.AppendLine(ex.GetType().ToString());
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }

            if (sb.Length == 0)
                return;

            sb.Insert(0, "ERR: ");
            WriteToFile(sb.ToString());
        }

        public static void Info(string message)
        {
            message = "INF: " + message;
            WriteToFile(message);
        }

        private static void WriteToFile(string data)
        {
            Debug.WriteLine(data);
            File.AppendAllText("exception.log", data);
        }
    }
}
