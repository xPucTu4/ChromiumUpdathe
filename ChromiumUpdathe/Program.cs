using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChromiumUpdathe
{
    class Program
    {
        public static string ChromiumUrl = "https://download-chromium.appspot.com/rev/Win_x64?type=snapshots";
        public static string ArchiveUrl = "https://commondatastorage.googleapis.com/chromium-browser-snapshots/Win_x64/__PENIS_LAST_COMMIT__/chrome-win.zip";
        public static string InstallDirectory = "C:\\Program Files\\";
        static void Main(string[] args)
        {
            Console.WriteLine("Updating chromium!");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en-US", 0.8f));
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("DickHTTPLib", "0.2.5"));
            Task<string> jsonText = client.GetStringAsync(ChromiumUrl);
            jsonText.Wait();
            var result = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(jsonText.Result, new { content = "" });
            Console.WriteLine("The last version commit is {0}.", result.content);
            if (result.content != GetMyVersion())
            {
                var zipAsync = client.GetStreamAsync(ArchiveUrl.Replace("__PENIS_LAST_COMMIT__", result.content));
                zipAsync.Wait();
                Console.WriteLine("Writing to disk");
                string tempName = Path.GetTempFileName();
                FileStream fs = File.OpenWrite(tempName);
                zipAsync.Result.CopyToAsync(fs).Wait();
                fs.FlushAsync().Wait();
                fs.Close();
                try
                {
                    DeleteRecursive(Path.Combine(InstallDirectory, "chrome-win"));
                    Console.WriteLine("The old directory is deleted");
                }
                catch (Exception any)
                {
                    Console.WriteLine("Error: ", any.Message);
                }
                Console.WriteLine("Extracting archive");
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(tempName, InstallDirectory);
                    Console.WriteLine("The new version is extracted");
                }
                catch (Exception any)
                {
                    Console.WriteLine("Error: ", any.Message);
                }
                File.Delete(tempName);

                // Set my version
                File.WriteAllText(Path.Combine(InstallDirectory, "dick.ver"), result.content);

                Console.WriteLine("Update completed");
            }
            else
            {
                Console.WriteLine("You already have the latest version");
            }
            Console.WriteLine("Press any key to continue, or any other key to quit.");
            Console.ReadKey(true);
        }

        private static string GetMyVersion()
        {
            try
            {
                return File.ReadAllText(Path.Combine(InstallDirectory, "dick.ver"));
            }
            catch { }

            DirectoryInfo di = new DirectoryInfo(Path.Combine(InstallDirectory,"chrome-win"));
            var a = di.GetFiles("*.manifest");
            return a[0].Name.Replace(".manifest", "");
        }

        private static void DeleteRecursive(string v)
        {
            DirectoryInfo di = new DirectoryInfo(v);
            foreach(DirectoryInfo di2 in di.GetDirectories())
            {
                DeleteRecursive(di2.FullName);
            }
            foreach(FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }
        }
    }
}
