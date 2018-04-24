namespace Kongrevsky.Infrastructure.ActiveDirectoryManager.Models
{
    public class ADUser
    {
        #region Properties

        public bool IsActive { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        #endregion
    }
}