using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace Emsal.Utility.UtilityObjects
{
    public class ExceptionHandlingOperation
    {
        public static Int64 guid;
        public static string RequestId;
        public static string ChannelId;
        public static int LineNumber;
        public static string LogType;


        private static Logger logger;


        static ExceptionHandlingOperation()
        {

            GlobalDiagnosticsContext.Set("ConnectionString", ConfigurationManager.AppSettings["PWD"]);
            //  GlobalDiagnosticsContext.Set("ConnectionString", "Data Source=AZKKDB;User Id=OC_YKB;Password=OCYKBOCYKB123");
            //GlobalDiagnosticsContext.Set("ConnectionString", "Data Source=AZKKUAT;User Id=OC_YKB;Password=YKB_OC");
            //connectionString="${gdc:item=ConnectionString}"
            //LayoutRendererFactory.AddLayoutRenderer("hour", typeof(EmailDeneme.HourLayoutRenderer));

        }


        public static void ExceptionYaz(string ex, string activityID, string channelID, Exception excp)
        {
            guid = IOUtil.GetGuid();
            ChannelId = channelID;
            LineNumber = GetLineNumber(excp);
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            logger.Info("Soap header exception, Date={0}, Message={1}", DateTime.Now.ToString(), ex);

        }
        public static void SaveInputInformation(string inputValue, string requestId, int lineNumber, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "INPUT";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = lineNumber;
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            logger.Info(inputValue);

        }
        public static void SaveOutputInformation(string outputValue, string requestId, int lineNumber, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "OUTPUT";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = lineNumber;
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            logger.Info(outputValue);

        }

        public static void HandleSoapHeaderExeption(SoapException excp, string requestId, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "ERROR";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = GetLineNumber(excp);
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            //logger.Info("Soap header exception, Date={0}, Message={1}", DateTime.Now.ToString(), excp.Message);
            logger.FatalException("", excp);
        }
        public static void HandleSoapException(SoapException excp, string requestId, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "ERROR";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = GetLineNumber(excp);
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);

            //logger.Info("Soap header exception, Date={0}, Message={1}", DateTime.Now.ToString(), excp.Message);
            logger.FatalException("", excp);
        }

        public static void HandleNullReferenceExeption(NullReferenceException excp, string requestId, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "ERROR";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = GetLineNumber(excp);
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            //logger.Info("Soap header exception, Date={0}, Message={1}", DateTime.Now.ToString(), excp.Message);
            logger.FatalException("", excp);
        }

        public static void HandleException(Exception excp, string requestId, ChannelEnum channel)
        {
            guid = IOUtil.GetGuid();
            LogType = "ERROR";
            RequestId = requestId;
            ChannelId = channel.ToString();
            LineNumber = GetLineNumber(excp);
            logger = NLog.LogManager.GetLogger(ConfigurationManager.AppSettings["logger_name"]);
            //logger.Info("Soap header exception, Date={0}, Message={1}", DateTime.Now.ToString(), excp.Message);
            logger.FatalException("", excp);
        }

        public static int GetLineNumber(Exception ex)
        {

            int linenum = 0;
            try
            {
                linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(":line") + 5));
            }
            catch
            {
                //Stack trace is not available!
            }
            return linenum;
        }

    }
}
