namespace HPVTesting.Interfaces.Background
{
    public interface IBackgroundMailerJobs : IBackgroundJobs
    {
        void SendWelcomeEmail(string DisplayName, string RecipientMail);
    }
}