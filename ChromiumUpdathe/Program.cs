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
        public static string InstallDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
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

            int versionDiff = 0;
            try
            {
                versionDiff = Int32.Parse(result.content) - Int32.Parse(GetMyVersion());

            }
            catch { }

            Console.WriteLine("The last version commit is {0}, and your is {1}, {2} versions behind.", result.content, GetMyVersion(), versionDiff);
            if (result.content != GetMyVersion())
            {
                var zipAsync = client.GetStreamAsync(ArchiveUrl.Replace("__PENIS_LAST_COMMIT__", result.content));
                zipAsync.Wait();
                Console.WriteLine("Writing to disk");
                string tempName = Path.GetTempFileName();
                FileStream fs = File.OpenWrite(tempName);
                CopyWithProgress(zipAsync.Result, fs);
                
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
                File.WriteAllText(Path.Combine(InstallDirectory, "chrome-win", "dick.ver"), result.content);

                Console.WriteLine("Update completed");
            }
            else
            {
                Console.WriteLine("You already have the latest version");
            }
            Console.WriteLine("Press any key to continue, or any other key to quit.");
            Console.ReadKey(true);
        }

        private static void CopyWithProgress(Stream src, Stream dst)
        {
            Console.WriteLine();
            byte[] buffer = new byte[1024];
            int totalReaded = 0;
            int readCount = 0;
            do
            {
                readCount = src.Read(buffer, 0, buffer.Length);
                dst.Write(buffer, 0, readCount);
                totalReaded++;
                
                // Report progress
                if (totalReaded % 1024 == 0)
                {
                    Console.Write(String.Concat(Enumerable.Repeat("\b \b", 64)));
                    Console.Write("Downloaded: {0} KB.".PadRight(32, ' '), totalReaded);
                }
            }
            while (readCount > 0);
            Console.WriteLine();
        }

        private static string GetMyVersion()
        {
            try
            {
                return File.ReadAllText(Path.Combine(InstallDirectory, "chrome-win", "dick.ver"));
            }
            catch { }
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(InstallDirectory, "chrome-win"));
                var a = di.GetFiles("*.manifest");
                return a[0].Name.Replace(".manifest", "");
            }
            catch
            {
                return "0";
            }
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
