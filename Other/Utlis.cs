using System;
using System.IO;

namespace SortMasterCLI.Other
{
    public static class Utils
    {
        public static string GetUserName()
        {

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return userProfile;
        }
    }
}