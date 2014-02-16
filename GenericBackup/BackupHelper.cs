using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using log4net;
using System.Configuration;


namespace GenericBackup
{
    internal class BackupHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (BackupHelper));

        internal static void StartBackup()
        {
            var fileName = Environment.MachineName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            log.Debug("File name " + fileName);
            var backupPath = ConfigurationManager.AppSettings["target"] + "\\" + fileName + ".zip";
            log.Debug("Backup path and name" + backupPath);
            var source = ConfigurationManager.AppSettings["source"];
            try
            {
                log.Info("Starting backup...");
                using (var zip = new ZipFile())
                {
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    zip.AddDirectory(source, fileName);
                    zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                    zip.Save(backupPath);
                }
                log.Info("Done... ");
                var file = new FileInfo(backupPath);
                var size = ConvertBytesToMegabytes(file.Length).ToString("0.00");


                var body = new StringBuilder();
                body.AppendLine("Backup Directory name: " + fileName);
                body.AppendLine("Back located at: " + backupPath);
                body.AppendLine("File backup size is " + size + "MB");

                var reportSucess = Boolean.Parse(ConfigurationManager.AppSettings["ReportSucess"]);
                if ( reportSucess == true)
                    MailHelper.SendEmail("Success: Back to " + Environment.MachineName + " Completed Succesfully", body.ToString());
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                if (ex.InnerException != null)
                    log.Fatal(ex.InnerException);
                MailHelper.SendEmail("Error: Failed to run back to " + Environment.MachineName, ex.Message);
            }
        }

        private static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes/1024f)/1024f;
        }

        internal static void CleanOldFiles(int AllowFile)
        {
            var backupPath = ConfigurationManager.AppSettings["target"];
            var filesOnTarget = new DirectoryInfo(backupPath).GetFiles().OrderBy(x => x.LastAccessTime);
            if (filesOnTarget.Count() > AllowFile)
            {
                log.Info("Need to delete files");
                var f = filesOnTarget.ToArray();
                var take = filesOnTarget.Count() - AllowFile;
                var filesToDelete = f.Take(take);

                foreach (var file in filesToDelete)
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
