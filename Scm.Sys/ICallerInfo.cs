namespace Scm.Sys
{
    public interface ICallerInfo
    {
        string CallerMemberName { get; }
        string CallerFilePath { get; }
        int CallerLineNumber { get; }
    }
}