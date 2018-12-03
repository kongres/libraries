namespace Kongrevsky.Infrastructure.Impersonator
{
    #region << Using >>

    using System;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.Win32.SafeHandles;

    #endregion

    /// <summary>
    ///     Provides ability to run code within the context of a specific user.
    /// </summary>
    public static class Impersonation
    {
        /// <summary>
        ///     Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        public static void RunAsUser(UserCredentials credentials, Action action, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                RunImpersonated(tokenHandle, _ => action());
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The action to perform.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        public static async Task RunAsUserAsync(UserCredentials credentials, Func<Task> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                await RunImpersonatedAsync(tokenHandle, _ => function());
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">
        ///     The action to perform, which accepts a <see cref="SafeAccessTokenHandle" /> to the user account as
        ///     its only parameter.
        /// </param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        public static void RunAsUser(UserCredentials credentials, Action<SafeAccessTokenHandle> action, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">
        ///     The action to perform, which accepts a <see cref="SafeAccessTokenHandle" /> to the user account
        ///     as its only parameter.
        /// </param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        public static async Task RunAsUserAsync(UserCredentials credentials, Func<SafeAccessTokenHandle, Task> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                await RunImpersonatedAsync(tokenHandle, function);
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">
        ///     The function to execute, which accepts a <see cref="SafeAccessTokenHandle" /> to the user
        ///     account as its only parameter.
        /// </param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, Func<T> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return RunImpersonated(tokenHandle, _ => function());
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">
        ///     The function to execute, which accepts a <see cref="SafeAccessTokenHandle" /> to the user
        ///     account as its only parameter.
        /// </param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <returns>The result of executing the function.</returns>
        public static async Task<T> RunAsUserAsync<T>(UserCredentials credentials, Func<Task<T>> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return await RunImpersonatedAsync(tokenHandle, _ => function());
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, Func<SafeAccessTokenHandle, T> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return RunImpersonated(tokenHandle, function);
            }
        }

        /// <summary>
        ///     Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <returns>The result of executing the function.</returns>
        public static async Task<T> RunAsUserAsync<T>(UserCredentials credentials, Func<SafeAccessTokenHandle, Task<T>> function, LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return await RunImpersonatedAsync(tokenHandle, function);
            }
        }

        private static void RunImpersonated(SafeAccessTokenHandle tokenHandle, Action<SafeAccessTokenHandle> action)
        {
            WindowsIdentity.RunImpersonated(tokenHandle, () => action(tokenHandle));
        }

        private static async Task RunImpersonatedAsync(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, Task> function)
        {
            await WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
        }

        private static T RunImpersonated<T>(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, T> function)
        {
            return WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
        }

        private static async Task<T> RunImpersonatedAsync<T>(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, Task<T>> function)
        {
            return await WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
        }
    }
}