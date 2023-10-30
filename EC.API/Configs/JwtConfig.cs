namespace EC.API.Configs
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string auth_key { get; set; } = string.Empty;
    }
}
