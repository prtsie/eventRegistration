namespace EventRegistration.Services.MailService
{
    public class MailOptions
    {
        public required string OrganizationName { get; set; }

        public required string OrganizationMail { get; set; }

        public required  int Port { get; set; }

        public required  string SmtpServer { get; set; }

        public required  string SmtpServerLogin { get; set; }

        public required  string SmtpServerPassword { get; set; }

        public bool UseSsl { get; set; }
    }
}
