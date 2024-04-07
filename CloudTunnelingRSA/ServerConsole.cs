partial class WebSocketServer
{
    public class ServerConsole
    {
        public static void WriteLine(string message)
        {
            if (AppConfig.Configuration.m_useConsolePrint) {
                    Console.WriteLine(message);
            }
        }
        public static void WriteLine()
        {
            if (AppConfig.Configuration.m_useConsolePrint) { 
                Console.WriteLine();
            }
        }
    }
}
