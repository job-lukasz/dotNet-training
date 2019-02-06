using System;

namespace rpi_dotnet
{
    class Program
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //Send plaintext
            log.Info("Main program loop");
        }
    }
}
