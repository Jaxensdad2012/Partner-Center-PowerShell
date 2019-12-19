﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Store.PartnerCenter.PowerShell.Utilities
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides share utility operations.
    /// </summary>
    internal static class SharedUtilities
    {
        private static readonly string s_homeEnvVar = Environment.GetEnvironmentVariable("HOME");
        private static readonly string s_lognameEnvVar = Environment.GetEnvironmentVariable("LOGNAME");
        private static readonly string s_userEnvVar = Environment.GetEnvironmentVariable("USER");
        private static readonly string s_lNameEnvVar = Environment.GetEnvironmentVariable("LNAME");
        private static readonly string s_usernameEnvVar = Environment.GetEnvironmentVariable("USERNAME");

        /// <summary>
        /// Generate the default file location
        /// </summary>
        /// <returns>Root directory</returns>
        internal static string GetUserRootDirectory()
        {
            return !IsWindowsPlatform()
                ? GetUserHomeDirOnUnix()
                : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        /// <summary>
        /// Gets a flag indicating whether the platform is running Linux or not.
        /// </summary>
        /// <returns>A value indicating the platform is running Linux or not.</returns>
        public static bool IsLinuxPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
        }

        /// <summary>
        /// Gets a flag indicating whether the platform is running OSX or not.
        /// </summary>
        /// <returns>A value indicating the platform is running OSX or not.</returns>
        public static bool IsMacPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
        }

        /// <summary>
        /// Gets a flag indicating whether the platform is running Windows or not.
        /// </summary>
        /// <returns>A value indicating the platform is running Windows or not.</returns>
        public static bool IsWindowsPlatform()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private static string GetUserHomeDirOnUnix()
        {
            if (IsWindowsPlatform())
            {
                throw new NotSupportedException();
            }

            if (!string.IsNullOrEmpty(s_homeEnvVar))
            {
                return s_homeEnvVar;
            }

            string username = null;
            if (!string.IsNullOrEmpty(s_lognameEnvVar))
            {
                username = s_lognameEnvVar;
            }
            else if (!string.IsNullOrEmpty(s_userEnvVar))
            {
                username = s_userEnvVar;
            }
            else if (!string.IsNullOrEmpty(s_lNameEnvVar))
            {
                username = s_lNameEnvVar;
            }
            else if (!string.IsNullOrEmpty(s_usernameEnvVar))
            {
                username = s_usernameEnvVar;
            }

            if (IsMacPlatform())
            {
                return !string.IsNullOrEmpty(username) ? Path.Combine("/Users", username) : null;
            }
            else if (IsLinuxPlatform())
            {
                if (LinuxNativeMethods.getuid() == LinuxNativeMethods.RootUserId)
                {
                    return "/root";
                }
                else
                {
                    return !string.IsNullOrEmpty(username) ? Path.Combine("/home", username) : null;
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}