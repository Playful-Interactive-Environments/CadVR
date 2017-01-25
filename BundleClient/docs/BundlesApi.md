# IO.Swagger.Api.BundlesApi

All URIs are relative to *https://localhost:3001/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetAvailableBundles**](BundlesApi.md#getavailablebundles) | **GET** /bundles | 
[**GetBundle**](BundlesApi.md#getbundle) | **GET** /bundles/{filenameOrUniqueId} | 
[**GetBundleDetailsById**](BundlesApi.md#getbundledetailsbyid) | **GET** /bundles/{uniqueId}/details | 
[**PutBundle**](BundlesApi.md#putbundle) | **PUT** /bundles/{filenameOrUniqueId} | 


<a name="getavailablebundles"></a>
# **GetAvailableBundles**
> BundleList GetAvailableBundles ()



Gets all available file names.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class GetAvailableBundlesExample
    {
        public void main()
        {
            
            // Configure API key authorization: api_key_both
            Configuration.Default.ApiKey.Add("api_key_both", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api_key_both", "Bearer");

            var apiInstance = new BundlesApi();

            try
            {
                BundleList result = apiInstance.GetAvailableBundles();
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BundlesApi.GetAvailableBundles: " + e.Message );
            }
        }
    }
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**BundleList**](BundleList.md)

### Authorization

[api_key_both](../README.md#api_key_both)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getbundle"></a>
# **GetBundle**
> byte[] GetBundle (string filenameOrUniqueId)



Gets a bundle from the server.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class GetBundleExample
    {
        public void main()
        {
            
            // Configure API key authorization: api_key_both
            Configuration.Default.ApiKey.Add("api_key_both", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api_key_both", "Bearer");

            var apiInstance = new BundlesApi();
            var filenameOrUniqueId = filenameOrUniqueId_example;  // string | The unique id of the bundle.

            try
            {
                byte[] result = apiInstance.GetBundle(filenameOrUniqueId);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BundlesApi.GetBundle: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **filenameOrUniqueId** | **string**| The unique id of the bundle. | 

### Return type

**byte[]**

### Authorization

[api_key_both](../README.md#api_key_both)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/octet-stream

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getbundledetailsbyid"></a>
# **GetBundleDetailsById**
> BundleDetails GetBundleDetailsById (string uniqueId)



Gets details about a specific bundle.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class GetBundleDetailsByIdExample
    {
        public void main()
        {
            
            // Configure API key authorization: api_key_client
            Configuration.Default.ApiKey.Add("api_key_client", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api_key_client", "Bearer");

            var apiInstance = new BundlesApi();
            var uniqueId = uniqueId_example;  // string | The unique base64 encoded name of the bundle.

            try
            {
                BundleDetails result = apiInstance.GetBundleDetailsById(uniqueId);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BundlesApi.GetBundleDetailsById: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **uniqueId** | **string**| The unique base64 encoded name of the bundle. | 

### Return type

[**BundleDetails**](BundleDetails.md)

### Authorization

[api_key_client](../README.md#api_key_client)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="putbundle"></a>
# **PutBundle**
> void PutBundle (string filenameOrUniqueId, string bundleFileType, List<string> platforms, List<string> engines, byte[] filecontents)



Uploads a bundle to the server.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PutBundleExample
    {
        public void main()
        {
            
            // Configure API key authorization: api_key_app
            Configuration.Default.ApiKey.Add("api_key_app", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api_key_app", "Bearer");

            var apiInstance = new BundlesApi();
            var filenameOrUniqueId = filenameOrUniqueId_example;  // string | The base64 encoded original display name of the bundle.
            var bundleFileType = bundleFileType_example;  // string | The file extension of the bundle file (e.g. \"pak\" or \"zip\" etc.)
            var platforms = new List<string>(); // List<string> | The os / platform this bundle is built for
            var engines = new List<string>(); // List<string> | The engine/framework this bundle is built for
            var filecontents = BINARY_DATA_HERE;  // byte[] | The contents of the bundle file.

            try
            {
                apiInstance.PutBundle(filenameOrUniqueId, bundleFileType, platforms, engines, filecontents);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BundlesApi.PutBundle: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **filenameOrUniqueId** | **string**| The base64 encoded original display name of the bundle. | 
 **bundleFileType** | **string**| The file extension of the bundle file (e.g. \&quot;pak\&quot; or \&quot;zip\&quot; etc.) | 
 **platforms** | [**List&lt;string&gt;**](string.md)| The os / platform this bundle is built for | 
 **engines** | [**List&lt;string&gt;**](string.md)| The engine/framework this bundle is built for | 
 **filecontents** | **byte[]**| The contents of the bundle file. | 

### Return type

void (empty response body)

### Authorization

[api_key_app](../README.md#api_key_app)

### HTTP request headers

 - **Content-Type**: application/octet-stream
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

