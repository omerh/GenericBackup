using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;

namespace GenericBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(".\\log4net.config"));
            BackupHelper.CleanOldFiles(int.Parse(ConfigurationManager.AppSettings["AllowFile"]));
            BackupHelper.StartBackup();

        }
    }
}
