using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Scm.Sys
{
    public struct CallerInfo : ICallerInfo
    {
        public string CallerMemberName { get; }
        public string CallerFilePath { get; }
        public int CallerLineNumber { get; }

        public CallerInfo(
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Pass callerinfo to target")]
        public static CallerInfo Here(
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => new CallerInfo(callerMemberName, callerFilePath, callerLineNumber);

        public override string ToString()
            => $"{CallerMemberName}({CallerFilePath}:{CallerLineNumber}";
    }
}