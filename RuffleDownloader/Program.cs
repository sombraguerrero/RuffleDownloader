// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Text;

string filePath = @"D:\users\rsett\Downloads\ruffle_desktop.zip";
Task<byte[]> getRuffle = GetRuffle();
getRuffle.Wait();
byte[] stageResults = getRuffle.Result;
if (stageResults.Length  <= 1024)
{
    filePath = @"D:\users\rsett\Downloads\download_error.log";
}

File.WriteAllBytes(filePath, getRuffle.Result);
if (File.Exists(filePath))
{
    ZipFile.ExtractToDirectory(filePath, @"D:\run\ruffle", true);
}
static async Task<byte[]> GetRuffle()
{
    int daysBack = 0;
    HttpClient httpClient = new HttpClient();
    string targetdir = DateTime.Now.ToString("yyyy-MM-dd");
    string targetfile = DateTime.Now.ToString("yyyy_MM_dd");
    string BaseAddress = @"https://github.com/ruffle-rs/ruffle/releases/download/";

    HttpResponseMessage response = await httpClient.GetAsync($"{BaseAddress}nightly-{targetdir}/ruffle-nightly-{targetfile}-windows-x86_64.zip");
    try
    {
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            bool success = false;
            while (!success)
            {
                daysBack--;
                targetdir = DateTime.Now.AddDays(daysBack).ToString("yyyy-MM-dd");
                targetfile = DateTime.Now.AddDays(daysBack).ToString("yyyy_MM_dd");
                response = await httpClient.GetAsync($"{BaseAddress}nightly-{targetdir}/ruffle-nightly-{targetfile}-windows-x86_64.zip");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    success = true;
                    Console.WriteLine("Success on " + targetdir);
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine(ex.Message);
    }
    return Encoding.UTF8.GetBytes("The release server appears to be in error!");
}
