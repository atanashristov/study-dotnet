# Add API Versioning

## Summary

I want to add API versioning to the Asp.Net Core API project that we have in folder "01_Code/02_WebApi_Basics/WebApiDemo".

## Details

It looks like I have to add 2 packages, but please make sure this is correct considering we are on Dot.Net 10:

- Asp.Versioning.Mvc
- Asp.Versioning.Mvc.ApiExplorer

Create a V2 of the ShirtsController. The default version on the path "shirts/" should be the current ShirtsController (the V1).

Do not rename ShirtsController, do not change the name to ShirtsV1Controller for example.

The clients can request V2 ShirtsController by specifying one of:

- Header: x-api-version
- Query string param: api-version
- Url segment

Make sure we can see the versions of the API in the Open API specification and in the Scalar UI.
