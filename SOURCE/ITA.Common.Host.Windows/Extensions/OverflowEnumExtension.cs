using System.ComponentModel;
using System.Diagnostics;

namespace ITA.Common.Host.Windows.Extensions
{
    public static class OverflowEnumExtension
    {
        public static System.Diagnostics.OverflowAction ToOverflowAction(this OverflowAction overflowAction)
        {
            switch (overflowAction)
            {
                case OverflowAction.DoNotOverwrite: return System.Diagnostics.OverflowAction.DoNotOverwrite;
                case OverflowAction.OverwriteAsNeeded: return System.Diagnostics.OverflowAction.OverwriteAsNeeded;
                case OverflowAction.OverwriteOlder: return System.Diagnostics.OverflowAction.OverwriteOlder;
            }

            throw new InvalidEnumArgumentException(nameof(overflowAction));
        }
    }
}
