using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using System.Text;

namespace Event.API.Helpers
{
    public static class ControllerHelpers
    {
        /// <summary>
        /// Encode a string in UTF8 format
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncodeString(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            string dataEncoded = WebEncoders.Base64UrlEncode(dataBytes);

            return dataEncoded;
        }

        /// <summary>
        /// Decode a string in UTF8 format
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecodeString(string data)
        {
            var dataBytes = WebEncoders.Base64UrlDecode(data);
            string dataDecoded = Encoding.UTF8.GetString(dataBytes);

            return dataDecoded;
        }

        /// <summary>
        /// Returns the absolute path for all mail templates
        /// </summary>
        /// <param name="webRoot"></param>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static string GetMailTemplatesPath(IWebHostEnvironment webRoot, string file_name) => $"{webRoot.WebRootPath}{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}MailTemplates{Path.DirectorySeparatorChar}{file_name}";
    }
}
