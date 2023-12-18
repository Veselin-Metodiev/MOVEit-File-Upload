using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using MOVEitFileUpload.Services.Interfaces;
using MOVEitFileUpload.ViewModels;

using static MOVEitFileUpload.Common.Endpoints;
using static MOVEitFileUpload.Common.Messages;

namespace MOVEitFileUpload.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient httpClient;

        private const string GrantType = "password";
        private const string AccessTokenKey = "access_token";
        private const string HomeFolderIdKey = "homeFolderID";
        private const string ErrorMessageKey = "message";
        private const string ErrorDescriptionKey = "error_description";
        private const string ErrorDetailKey = "detail";

        public FileUploadService()
        {
            httpClient = new HttpClient();
        }

        public async Task UploadFile(FileUploadViewModel fileUploadViewModel)
        {
            string username = fileUploadViewModel.Username;
            string password = fileUploadViewModel.Password;
            string url = fileUploadViewModel.Url;
            IFormFile file = fileUploadViewModel.File;

            string token = await GetToken(username, password, url);

            string homeFolderId = await GetHomeFolderId(token, url);

            MultipartFormDataContent content = new();
            StreamContent fileContent = new(file.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await httpClient.PostAsync(url + string.Format(PostFileToFolder, homeFolderId), content);

            fileContent.Dispose();
            content.Dispose();

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                throw new WebException(JsonNode.Parse(responseContent)![ErrorDetailKey]!.ToString());
            }

            response.Dispose();
        }

        private async Task<string> GetToken(string username, string password, string url)
        {
            KeyValuePair<string, string>[] data = new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", GrantType),
            };

            HttpContent content = new FormUrlEncodedContent(data);

            HttpResponseMessage response = await httpClient.PostAsync(url + GetTokenEndpoint, content);
            content.Dispose();

            string responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new WebException(JsonNode.Parse(responseContent)![ErrorDescriptionKey]!.ToString());
            }

            response.Dispose();

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new WebException(NoTokenFound);
            }

            string token = JsonNode.Parse(responseContent)![AccessTokenKey]!.ToString();
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new BadHttpRequestException(NoTokenInfoFound);
            }

            return token;
        }

        private async Task<string> GetHomeFolderId(string token, string url)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage response = await httpClient.GetAsync(url + GetUserInfoEndpoint);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new WebException(JsonNode.Parse(responseContent)![ErrorMessageKey]!.ToString());
            }

            response.Dispose();

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new BadHttpRequestException(NoUserInfoFound);
            }

            string homeFolderId = JsonNode.Parse(responseContent)![HomeFolderIdKey]!.ToString();
            if (string.IsNullOrWhiteSpace(homeFolderId))
            {
                throw new BadHttpRequestException(NoHomeFolderIdFound);
            }

            return homeFolderId;
        }

        public void ValidateInput(FileUploadViewModel fileUploadViewModel)
        {
            if (string.IsNullOrWhiteSpace(fileUploadViewModel.Username))
            {
                throw new ArgumentNullException(nameof(fileUploadViewModel.Username));
            }
            if (string.IsNullOrWhiteSpace(fileUploadViewModel.Password))
            {
                throw new ArgumentNullException(nameof(fileUploadViewModel.Password));
            }
            if (string.IsNullOrWhiteSpace(fileUploadViewModel.Url))
            {
                throw new ArgumentNullException(nameof(fileUploadViewModel.Url));
            }
            if (fileUploadViewModel.File == null)
            {
                throw new ArgumentNullException(nameof(fileUploadViewModel.File));
            }
        }
    }
}
