using System;
using System.Collections;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;

namespace Magicube.WebServer.Internal {
    public static class EventLogHelper {
        public static bool UploadEventLog(string applicationName, string errorMessage) {
            bool successfullyWroteToEventLog = true;

            try {
                //todo:  upload eventLog to server
            } catch (Exception) {
                successfullyWroteToEventLog = false;
            }

            if (Environment.UserInteractive)
                Console.WriteLine(errorMessage);

            return successfullyWroteToEventLog;
        }

        public static void GenerateTextErrorMessage(Exception exception, StringBuilder sb, int recursionLevel = 0) {
            if (recursionLevel > 25)
                return;

            string prefix = "";

            if (recursionLevel > 0) {
                for (int i = 0; i < recursionLevel; i++)
                    prefix += "  ";

                prefix += "Inner ";
            }

            sb.AppendSection(prefix, "Exception Information");
            sb.AppendDetails(prefix, "Exception Type", exception.GetType().ToString());
            sb.AppendDetails(prefix, "Exception Message", exception.Message);
            sb.AppendLine(exception.StackTrace).AppendLine();

            using (var currentProcess = Process.GetCurrentProcess()) {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

                sb.AppendSection(prefix, "Application Information");
                sb.AppendDetails(prefix, "Machine Name", ipGlobalProperties.HostName + "." + ipGlobalProperties.DomainName);
                sb.AppendDetails(prefix, "Domain User", Environment.UserDomainName + "\\" + Environment.UserName);
                sb.AppendDetails(prefix, "Application Domain Name", AppDomain.CurrentDomain.FriendlyName);
                sb.AppendDetails(prefix, "Process Name", currentProcess.ProcessName);
                sb.AppendDetails(prefix, "Process Id", currentProcess.Id.ToString());
            }

            if (exception.Data.Keys.Count > 0) {
                sb.AppendLine().AppendSection(prefix, "Additional Information");

                foreach (DictionaryEntry dictionaryEntry in exception.Data) {
                    sb.AppendDetails(prefix, dictionaryEntry.Key.ToString(), dictionaryEntry.Value == null ? "" : dictionaryEntry.Value.ToString());
                }

                sb.AppendLine();
            }

            if (exception.InnerException != null)
                GenerateTextErrorMessage(exception.InnerException, sb, ++recursionLevel);
        }

        private static StringBuilder AppendSection(this StringBuilder sb, string prefix, string sectionName) {
            return sb.AppendFormat("{0}{1}:", prefix, sectionName).AppendLine();
        }

        private static StringBuilder AppendDetails(this StringBuilder sb, string prefix, string detailName, string detailValue) {
            return sb.AppendFormat("{0}  {1}: {2}", prefix, detailName, detailValue).AppendLine();
        }
    }
}
