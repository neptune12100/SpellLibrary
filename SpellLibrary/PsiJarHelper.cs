using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpellLibrary
{
    /// <summary>
    /// Helper class for downloading/updating Psi
    /// </summary>
    class PsiJarHelper
    {

        private static WebClient wc = new WebClient();
        //private static readonly string PsiVersionUriBase = "https://raw.githubusercontent.com/Vazkii/Psi/master/version/";
        private static readonly string PsiVersionUriBase = "file://C:\\Users\\Levi\\Documents\\mcmod\\Psi-master\\Psi-master\\version\\";
        private static readonly string PsiFileUriBase = "http://psi.vazkii.us/files/";
        public static string GetLatestPsiVersion(string mcVersion)
        {
            string versionFileUri = PsiVersionUriBase + mcVersion + ".txt";
            byte[] versionBytes = wc.DownloadData(versionFileUri);
            string version = Encoding.ASCII.GetString(versionBytes);
            return version;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">The Psi version e.g. "beta-31" gets you Psi-beta-31.jar</param>
        /// <returns></returns>
        public static bool DownloadPsiVersion(string version)
        {
            string jarFileName = "Psi-" + version + ".jar";
            string fileUri = PsiFileUriBase + jarFileName;
            try {
                wc.DownloadFile(fileUri, jarFileName);
            } catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
