using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosChangefeed
{
    public class ChangeFeed
    {
        private DocumentClient DocumentClient { get; set; }
        private Uri ConnectionUri { get; set; }
        public String Endpoint { get; set; } 
        public String AuthorizationKey { get; set; }
        public Changes Changes { get; set; }

        public ChangeFeed(String endPoint, String databaseName, String collectionName, String authKey)
        {
            this.Endpoint = endPoint;
            this.AuthorizationKey = authKey;
            this.ConnectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
        }

        public void Connect()
        {
            this.DocumentClient = new DocumentClient(
                new Uri(this.Endpoint),
                this.AuthorizationKey,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
            
        }

        public async Task<long> SetCheckpoint()
        {
            if(this.Changes == null)
            {
                this.Changes = new Changes();
                this.Changes.Checkpoints = new Dictionary<string, string>();
                this.Changes.UpdatedDocuments = new List<Document>();
            }
            List<PartitionKeyRange> partitionKeyRanges = await GetPartionKeyRange();
            long changes = GetChanges(partitionKeyRanges);
            return changes;
        }
        public async Task<List<PartitionKeyRange>> GetPartionKeyRange()
        {
            string continuationToken = null;
            List<PartitionKeyRange> partitionKeyRange = new List<PartitionKeyRange>();
            do
            {
                FeedResponse<PartitionKeyRange> partitionKeyRangesResponse = await DocumentClient.ReadPartitionKeyRangeFeedAsync(
                    ConnectionUri,
                    new FeedOptions { RequestContinuation = continuationToken });

                partitionKeyRange.AddRange(partitionKeyRangesResponse);
                continuationToken = partitionKeyRangesResponse.ResponseContinuation;
            }
            while (continuationToken != null);
            return partitionKeyRange;
        }
        public long GetChanges(List<PartitionKeyRange> partitionKeyRanges)
        {
            foreach (PartitionKeyRange pKeyRange in partitionKeyRanges)
            {
                string continuation = null;
                this.Changes.Checkpoints.TryGetValue(pKeyRange.Id, out continuation);
                IDocumentQuery<Document> query = DocumentClient.CreateDocumentChangeFeedQuery(
                    ConnectionUri,
                    new ChangeFeedOptions
                    {
                        PartitionKeyRangeId = pKeyRange.Id,
                        //StartFromBeginning = true,
                        RequestContinuation = continuation,
                        MaxItemCount = -1,
                        // Set reading time: only show change feed results modified since StartTime
                        //StartTime = DateTime.Now - TimeSpan.FromSeconds(9000)
                    });

                while (query.HasMoreResults)
                {
                    FeedResponse<Document> readChangesResponse = query.ExecuteNextAsync<Document>().Result;
                    foreach (Document updatedDocument in readChangesResponse)
                    {
                        this.Changes.UpdatedDocuments.Add(updatedDocument);
                    }
                    this.Changes.Checkpoints[pKeyRange.Id] = readChangesResponse.ResponseContinuation;
                }
            }
            return this.Changes.UpdatedDocuments.Count;
        }
    }
}
