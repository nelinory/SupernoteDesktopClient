using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Core
{
    public static class UpdateManager
    {
        private static bool _updateAvailable = false;
        private static string _updateMessage = String.Empty;
        private static string _updateDetails = String.Empty;

        public static async Task<(bool updateAvailable, string updateMessage, string updateDetails)> CheckForUpdate()
        {
            if (_updateAvailable == true)
                return (_updateAvailable, _updateMessage, _updateDetails);
            else
            {
                _updateDetails = String.Empty;

                using (HttpClient client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }))
                {
                    // github will always redirect releases/latest to a latest version tag
                    using (HttpResponseMessage response = await client.GetAsync(@"https://github.com/nelinory/SupernoteDesktopClient/releases/latest"))
                    {
                        try
                        {
                            string redirect = response.Headers.Location.ToString();
                            if (String.IsNullOrWhiteSpace(redirect) == false)
                            {
                                string githubVersion = redirect.Substring(redirect.LastIndexOf("/") + 1);
                                string currentVersion = ApplicationManager.GetAssemblyVersion();

                                if (String.CompareOrdinal(githubVersion, currentVersion) > 0)
                                {
                                    _updateAvailable = true;
                                    _updateDetails = $"View {githubVersion} release details.";
                                }
                            }
                        }
                        catch (Exception)
                        {
                            _updateAvailable = false;
                        }
                    }
                }

                _updateMessage = (_updateAvailable == true)
                    ? "There is a new release of Supernote Desktop Client available."
                    : "You already have the latest version of Supernote Desktop Client installed.";

                return (_updateAvailable, _updateMessage, _updateDetails);
            }
        }

        public static async Task<(bool updateAvailable, string updateMessage, string updateDetails)> GetUpdateDetails()
        {
            return (_updateAvailable, _updateMessage, _updateDetails);
        }
    }
}
