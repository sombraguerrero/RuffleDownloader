// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
string filePath = @"D:\users\rsett\Downloads\ruffle_desktop.zip";
Task<byte[]> getRuffle = GetRuffle();
getRuffle.Wait();
File.WriteAllBytes(filePath, getRuffle.Result);
if (File.Exists(filePath))
{
    ZipFile.ExtractToDirectory(filePath, @"D:\run\ruffle");
}
static async Task<byte[]> GetRuffle()
{
    int daysBack = 0;
    int targetSize = 1048576 * 4;
    HttpClient httpClient = new HttpClient();
    string targetdir = DateTime.Now.ToString("yyyy-MM-dd");
    string targetfile = DateTime.Now.ToString("yyyy_MM_dd");
    string BaseAddress = @"https://github.com/ruffle-rs/ruffle/releases/download/";

    byte[] response = await httpClient.GetByteArrayAsync($"{BaseAddress}nightly-{targetdir}/ruffle-nightly-{targetfile}-windows-x86_64.zip");
    try
    {
        if (response.Length > targetSize)
        {
            return response;
        }
        else
        {
            bool success = false;
            while (!success)
            {
                daysBack--;
                targetdir = DateTime.Now.AddDays(daysBack).ToString("yyyy-MM-dd");
                targetfile = DateTime.Now.AddDays(daysBack).ToString("yyyy_MM_dd");
                response = await httpClient.GetByteArrayAsync($"{BaseAddress}nightly-{targetdir}/ruffle-nightly-{targetfile}-windows-x86_64.zip");
                if (response.Length > targetSize)
                {
                    success = true;
                    return response;
                }
            }
        }
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine(ex.Message);
    }
    byte[] responseBytes = new byte[response.Length];
    Random random = new Random();
    random.NextBytes(responseBytes);
    return responseBytes;
}
