
namespace CloudTunnelingLocalGateRSA
{
    public class ServerConsole
    {
        public static void WriteLine(string message )
        {
            Console.WriteLine(message);
        }
        public static void WriteLine()
        {
            Console.WriteLine("");
        }

        public static void WriteFormatLine(string message, params object[] objects)
        {
            Console.WriteLine(string.Format(message, objects));
        }
    }
}
