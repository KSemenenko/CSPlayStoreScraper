using PlayStoreScraper.Exporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStoreScraper.Models;

namespace PlayStoreScraper
{
    class Program
    {
		static void Main(string[] args)
		{

			AppModel appModel = PlayStoreScraper.ParseAppUrls ("/store/apps/details?id=com.NNGames.starchef_android").Result;


			Console.WriteLine($"App Name: {appModel.Name}, App URL: {appModel.Url}, {appModel.CurrentVersion}, {appModel.CoverImageUrl}");
		
		}

//        static void Main2(string[] args)
//        {
//            string[] keywords = { "game", "browser" };
//
//            string[] fieldNames = {
//                "Url", "ScrapedDate", "Name", "Developer", "IsTopDeveloper", "DeveloperURL", "PublicationDate", 
//                "Category", "IsFree", "Price", "CoverImageUrl", "Description", "ReviewScore", "ReviewTotal", 
//                "FiveStarsReviews", "FourStarsReviews", "ThreeStarsReviews", "TwoStarsReviews", 
//                "OneStarReviewCount", "AppSize", "Installs", "CurrentVersion", "MinimumOSVersion", "ContentRating", 
//                "HaveInAppPurchases", "InAppPriceRange", "DeveloperEmail", "DeveloperWebsite", "DeveloperPrivacyPolicy"
//            };
//
//            string outputFilePath = @"./result";
//
//            IExporter exporter = null;
//
//            // Export to CSV file.
//            // NOTE: You can also implement your own Exporter class.
//            // Please see IExporter and CSVExporter class for example.
//            exporter = new CSVExporter(outputFilePath + ".csv", fieldNames);
//
//            // Start crawling for the keywords with maximum 30 Apps result and download delay 1 second.
//            PlayStoreScraper.Crawl(keywords, exporter, 30, 1000);
//
//
//            // Same example with exporting to JSON file
//            exporter = new JSONExporter(outputFilePath + ".json", fieldNames);
//            PlayStoreScraper.Crawl(keywords, exporter, 30, 1000);
//
//
//            // Same example with callback method
//            PlayStoreScraper.Crawl(keywords, null, 30, 1000, appModel =>
//            {
//                Console.WriteLine("App Name: {0}, App URL: {1}", appModel.Name, appModel.Url);
//            });
//        }
    }
}
