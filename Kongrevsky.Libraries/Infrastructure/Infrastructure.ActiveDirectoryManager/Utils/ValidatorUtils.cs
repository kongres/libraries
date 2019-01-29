namespace Kongrevsky.Infrastructure.ActiveDirectoryManager.Utils
{
    #region << Using >>

    using System.Net.Mail;

    #endregion

    internal class ValidatorUtils
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}