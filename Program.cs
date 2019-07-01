using System;
using System.Configuration;
using System.Threading.Tasks;

namespace CosmosChangefeed
{
    class Program
    {
        static void Main(string[] args)
        {
            String databaseName = ConfigurationManager.AppSettings["DatabaseName"];
            String collectionName = ConfigurationManager.AppSettings["CollectionName"];
            String endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
            String authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
            ChangeFeed changeFeed = new ChangeFeed(endpointUrl, databaseName, collectionName, authorizationKey);
            changeFeed.Connect();
            Run(changeFeed).Wait();

            Console.Read();
        }
        private static async Task Run(ChangeFeed feed)
        {
            long changes = await feed.SetCheckpoint();
            Console.WriteLine("A checkpoint is setup, Once your load is completed hit enter");
            Console.Read();
            changes = await feed.SetCheckpoint();
            Console.WriteLine("The total number of documents updated during this run are: " + feed.Changes.UpdatedDocuments.Count.ToString());
            
        }
    }
}
