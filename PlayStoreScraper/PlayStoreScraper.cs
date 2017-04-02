using System;
using System.Linq;
//using WebUtilsLib;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Security;
using System.ComponentModel;
using System.Reflection;
using PlayStoreScraper.Exporters;
using PlayStoreScraper.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayStoreScraper
{
    class PlayStoreScraper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Response Parser
        private static PlayStoreParser parser = new PlayStoreParser();

	
		public static async Task<AppModel> ParseAppUrls(string url, int downloadDelay = 0, IExporter exporter = null)
        {
			AppModel parsedApp = null;

            log.Info("Parsing App URLs...");

            // Creating Instance of Web Requests Server
			HttpClient httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });
            httpClient.DefaultRequestHeaders.Add("User-Agent", Consts.USER_AGENT);
            httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", Consts.ACCEPT_LANGUAGE);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            CultureInfo lang = new CultureInfo("ru");
            Thread.CurrentThread.CurrentCulture = lang;

            try
            {
                // Building APP URL
                string appUrl = Consts.APP_URL_PREFIX + url + string.Format(Consts.APP_URL_LANGUAGE, lang.TwoLetterISOLanguageName);

				string response = await httpClient.GetStringAsync(appUrl);

                // Configuring server and Issuing Request
//                server.Headers.Add(Consts.ACCEPT_LANGUAGE);
//                server.Host = Consts.HOST;
//                server.Encoding = "utf-8";
//                server.EncodingDetection = WebRequests.CharsetDetection.DefaultCharset;
//                string response = server.Get(appUrl);

                // Sanity Check
//                if (String.IsNullOrEmpty(response) || server.StatusCode != System.Net.HttpStatusCode.OK)
//                {
//                    log.Info("Error opening app page : " + appUrl);
//
//                    // Renewing WebRequest Object to get rid of Cookies
//                   // server = new WebRequests();
//
//                    // Inc. retry counter
//                    retryCounter++;
//
//                    log.Info("Retrying:" + retryCounter);
//
//                    // Checking for maximum retry count
//                    double waitTime;
//                    if (retryCounter >= 11)
//                    {
//                        waitTime = TimeSpan.FromMinutes(35).TotalMilliseconds;
//                    }
//                    else
//                    {
//                        // Calculating next wait time ( 2 ^ retryCounter seconds)
//                        waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryCounter)).TotalMilliseconds;
//                    }
//
//                    // Hiccup to avoid google blocking connections in case of heavy traffic from the same IP
//                    Thread.Sleep(Convert.ToInt32(waitTime));
//                }
//                else
                {
                    

                    // Parsing App Data
                    parsedApp = parser.ParseAppPage(response, appUrl);

                    // Export the App Data
                    if (exporter != null)
                    {
                        log.Info("Parsed App: " + parsedApp.Name);

                        exporter.Write(parsedApp);
					}
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

			return parsedApp;
        }
        
        /// <summary>
        /// Get Page Token for play store streaming search result.
        /// </summary>
        /// <param name="response">Response body</param>
        /// <returns>Page Token</returns>
        protected static string getPageToken(string response)
        {
            string pagTok = "";
            string regex = @"'\[.*\\42((?:.(?!\\42))*:S:.*?)\\42.*\]\\n'";
            Match match = Regex.Match(response, regex);
            if (match.Success)
            {
                pagTok = DecodeEncodedNonAsciiCharacters(match.Groups[1].Value, true);
            }
            return pagTok;
        }

        protected static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        protected static string DecodeEncodedNonAsciiCharacters(string value, bool isDoubleSlash = false)
        {
            string regex = @"\\u(?<Value>[a-zA-Z0-9]{4})";
            if (isDoubleSlash)
            {
                regex = @"\\" + regex;
            }

            return Regex.Replace(
                value,
                regex,
                m =>
                {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
