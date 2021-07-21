namespace ServiceHost.Config
{
    public class JwtConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public int SecondsToExpire { get; set; }
        public int CacheSecondsToExpire { get; set; }
    }
}
