namespace Kongrevsky.Infrastructure.HelpManager.Models
{
    public class CreateHelpRequest
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }
    }
}