namespace MOVEitFileUpload.Common
{
    public static class Endpoints
    {
        public const string GetTokenEndpoint = "/api/v1/token";
        public const string GetUserInfoEndpoint = "/api/v1/users/self";
        public const string PostFileToFolder = "/api/v1/folders/{0}/files";
    }
}
