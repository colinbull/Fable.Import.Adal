# Active Directory for Javascript Fable sample.

This sample requires an Azure Active Directory Tenant with two applications registered, one for the client application and one for the API.

For the client this sample follows the Azure Active Directory javascript sample for more complete details see,  https://azure.microsoft.com/en-gb/resources/samples/active-directory-javascript-singlepageapp-dotnet-webapi/. 

The API, is created using Giraffe ands secured using JWT Authenticated by Azure Active Directory, it is based on the the Azure ASPNETCore 2 JWT sample,  https://blogs.msdn.microsoft.com/webdev/2017/04/06/jwt-validation-and-authorization-in-asp-net-core/ ** NOTE - this article is a little out of date and the API for ASPNETCORE 2 hsa changed a little but the concept remains the same. 

## Running the sample 

### To run the client 

You need to enter the Azure Active Directory `tenantid`, `clientid` and `tokenuri` from the Application registration into `src/Client/App.fs` 
Then open a shell and navigate to `src/Client` and Run `dotnet fable yarn-start`

### To run the API 

You need to enter the details for the JWT Bearer into `src/Server/Program.fs` 
Navigate to `src/Server` and Run `dotnet run`

### Testing the API
Once both are running you can navigate to `http://localhost:8080` and then click login to authenticate with azure active driectory. 

As an extension the sample also supports Role based authentication via Azure appRoles. See https://docs.microsoft.com/en-us/azure/architecture/multitenant-identity/app-roles for more details.