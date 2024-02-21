namespace İdentityExampleNet70.Models
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
