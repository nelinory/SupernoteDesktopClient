using SupernoteDesktopClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Core
{
    public static class HttpManager
    {
        private static readonly Regex _sourceLocationRegex = new Regex(@"^http:\/\/\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _jsonRegex = new Regex(@"'({.*?})'", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static async Task<(bool isValid, string message)> IsSourceLocationValidAsync(string sourceLocation)
        {
            bool isValid = false;
            string message = String.Empty;

            try
            {
                // validate Url
                if (_sourceLocationRegex.IsMatch(sourceLocation) == false)
                    return (false, "\nWeb address must be like: http://192.168.77.11:8089\nPlease correct it and try again.");

                // check if connection can be established
                using HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(sourceLocation) };
                using HttpResponseMessage response = await httpClient.GetAsync("/");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    isValid = true;
                else
                    throw new Exception("Cannot connect...");
            }
            catch (Exception)
            {
                // failed to connect
                message = "Cannot establish a connection to Supernote. Ensure you have entered the correct receiving address shown on Supernote 'Browse & Access' dialog.";
            }

            return (isValid, message);
        }

        public static async Task GetWebFolderAsync(HttpClient httpClient, List<string> webFileItems, string path)
        {
            using HttpResponseMessage response = await httpClient.GetAsync(path);

            var htmlResponse = await response.Content.ReadAsStringAsync();

            FileSystem fsl = await GetJsonPayloadAsync(htmlResponse);

            // get all files
            var files = fsl.FileList.Where(p => p.IsDirectory == false).ToList();
            foreach (FileSystemItem file in files)
            {
                webFileItems.Add(file.Path.Substring(1));
            }

            // walk all folders
            var folders = fsl.FileList.Where(p => p.IsDirectory == true).ToList();
            foreach (FileSystemItem folder in folders)
            {
                await GetWebFolderAsync(httpClient, webFileItems, folder.Path);
            }
        }

        public static async Task DownloadFileAsync(HttpClient httpClient, string destination, string filePath)
        {
            using var fileStream = await httpClient.GetStreamAsync(httpClient.BaseAddress + filePath);
            string destPath = Path.Combine(destination, Path.GetDirectoryName(filePath));

            Directory.CreateDirectory(destPath);

            using var fs = new FileStream(Path.Combine(destPath, Path.GetFileName(filePath)), FileMode.OpenOrCreate);
            await fileStream.CopyToAsync(fs);
        }

        private static async Task<FileSystem> GetJsonPayloadAsync(string htmlResponse)
        {
            string input = htmlResponse.Substring(htmlResponse.IndexOf("json = '{"));
            Match match = _jsonRegex.Matches(input)[0];

            FileSystem fsl = new FileSystem();
            if (match.Success == true)
            {
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(match.Groups[1].Value));
                return await JsonSerializer.DeserializeAsync<FileSystem>(memoryStream, options);
            }

            return fsl;
        }
    }
}
