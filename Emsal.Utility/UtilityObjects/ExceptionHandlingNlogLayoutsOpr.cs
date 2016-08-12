using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emsal.Utility.UtilityObjects
{

    [LayoutRenderer("guid")]
    public class guidLayoutRenderer : LayoutRenderer
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ExceptionHandlingOperation.guid.ToString());
        }

    }
    [LayoutRenderer("LogType")]
    public class LogTypeLayoutRenderer : LayoutRenderer
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ExceptionHandlingOperation.LogType.ToString());
        }

    }
    [LayoutRenderer("ChannelId")]
    public class ChannelIdLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ExceptionHandlingOperation.ChannelId);
        }

    }
    [LayoutRenderer("RequestId")]
    public class RequestIdLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ExceptionHandlingOperation.RequestId);
        }

    }
    [LayoutRenderer("LineNumber")]
    public class LineNumberLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ExceptionHandlingOperation.LineNumber.ToString());
        }

    }



}
