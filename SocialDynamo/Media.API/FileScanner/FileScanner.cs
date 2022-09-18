using Media.API.Exceptions;
using nClam;

namespace Media.API.FileScanner
{
    public static class FileScanner
    {
        static readonly string serverName = "localhost";
        static readonly int serverPort = 3310;

        public static void Run(Stream blob)
        {
            var clam = new ClamClient("localhost", 3310);
            var scanResult = clam.SendAndScanFileAsync(blob).Result;

            if (scanResult.Result != ClamScanResults.Clean)
                throw new FileVirusDetectedException("File contains virus");
        }
    }
}
