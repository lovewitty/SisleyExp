using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Windows.Forms;
using NLog.Config;
using System.IO;
using NLog.Targets;

namespace ExpNewOrd.Common
{
   public class AppCommon
    {
       public static string Connstring
       {
           get
           {
               return "Server=JEFF-PC;UId=william5;Pwd=william5;Database=SVR_SisleyCommnuity_NewDB";
           }
       }

       // Static constructor
       static AppCommon()
    {
        InitLog();
    }

       private static Logger _NLoger;

       public static Logger Log { get { return _NLoger; } }

       public static  Logger InitLog()
       {
           string NLogModuleName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
           LoggingConfiguration config = new LoggingConfiguration();
           FileTarget fileTarget = new FileTarget();

           //string ver = Guid.NewGuid().ToString();
           config.RemoveTarget("file");
           //config.RemoveTarget(ver);
           config.AddTarget("file", fileTarget);
           //config.AddTarget(ver, fileTarget);

           fileTarget.FileName = string.Format(@"${{basedir}}\{0}.txt", NLogModuleName);
           fileTarget.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss}${newline}" +
                               @"stacktrace:${newline}${stacktrace}${newline}" +
                               @"exception:${newline}${exception}${newline}" +
                               @"message:${newline}${message}";
           LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
           config.LoggingRules.Clear();
           config.LoggingRules.Add(rule2);
           LogManager.Configuration = config;
           //应用以上步骤配置后得到的logger,若此代码提前执行则将会应用本地配置文件中的配置
           var lg = _NLoger = NLog.LogManager.GetLogger(NLogModuleName);
           
           return lg;
       }
    }
}
