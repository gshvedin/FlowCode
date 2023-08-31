using System;
using System.Reflection;

namespace WorkflowEngine.Misc
{
    public static class MethodTimeLogger
    {
        // private static System.Text.StringBuilder logStr = new System.Text.StringBuilder();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Проверьте неиспользуемые параметры", Justification = "<Ожидание>")]
        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
#if DEBUG
            try
            {
                string current = $"{DateTime.Now}|{methodBase?.DeclaringType.Name}|{methodBase.Name}|{milliseconds}|{message}\n";

               // logStr.Append(current);

               // Console.WriteLine(current);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                /*supress*/
            }
#endif
        }
    }
}
