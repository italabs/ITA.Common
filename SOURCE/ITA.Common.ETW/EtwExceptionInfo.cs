using System.Diagnostics.Tracing;

namespace ITA.Common.ETW
{
    [EventData]
    public class EtwExceptionInfo
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public override string ToString()
        {
            return string.Format("Message=\"{0}\" StackTrace=\"{1}\"", Message, StackTrace);
        }
    }
}
