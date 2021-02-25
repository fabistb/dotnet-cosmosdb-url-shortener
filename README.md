# dotnet cosmosdb url shortener
Simple URL shortener test implementation.

The application relies on Azure resources.
Has never been tested on a scale or productive environment.

### Setup
#### Local
Shut theoretically also work with the Cosmos Db local emulator but not tested.

https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/cosmos-db/local-emulator.md

Create a new Namespace with the name _CosmosNamespace_ and a container with the name _ShortUrlContainer_.
Add to local secrets or launchSettings the cosmos connection string as _cosmos-connectionstring_.

#### Azure
To run the application in Azure the following services are required.

- Azure Web App 
- Cosmos DB
- Azure KeyVault

Enable managed identity for the web app and grant secret read and list access in the key vault.
Add the cosmos db connection string as secret with the name _cosmos-connectionstring_ to Azure Key Vault.

Create the container and namespace as described above to your cosmos db.
### HTTP Requests
**Create Short Url**

``curl
curl --location --request POST 'http://localhost:5000/' \
--header 'Content-Type: application/json' \
--data-raw '{
"LongUrl": "https://www.google.com"
}'
``

**Redirect to Url**

``curl
curl --location --request GET 'http://localhost:5000/api/v1.0/UrlShortener/dpl3npem'
``
