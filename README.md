This app can be used to get the total number of Inserts/Updates on a collection from a given point of time.
1. Start the app
  >dotnet CosmosChangefeed.dll
  
  >Once a checkpoint is created the app will display the same on console
  
  >> run your Inserts/Updates
  
  >hit enter - the app should display all the operations on a collection
 
2. To compile
   >Use visual studio to compile
   >Add app.config file to the project - add the following contents to the file (update the values)

<configuration>
  <appSettings>
    <add key="EndPointUrl" value="https://XXX.documents.azure.com"/>
    <add key="AuthorizationKey" value="ZZZ"/>
    <add key="DatabaseName" value="{DB_NAME}"/>
    <add key="CollectionName" value="{COLLECTION_NAME"/>
  </appSettings>
</configuration>

3. to run
