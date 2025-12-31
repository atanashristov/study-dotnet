# FilterAttribute_ResponseErrorFormat_Prompt

When `Shirt_ValidateShirtIdFilterAttribute` validation fails, the controller returns error with this body format:

```json
HTTP/1.1 404 Not Found
Connection: close
Content-Type: application/json; charset=utf-8
Date: Fri, 28 Nov 2025 16:21:28 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": {
    "rawValue": "-1",
    "attemptedValue": "-1",
    "errors": [
      {
        "exception": null,
        "errorMessage": "Invalid shirt ID."
      },
      {
        "exception": null,
        "errorMessage": "Shirt does not exist."
      }
    ],
    "validationState": 1,
    "isContainerNode": false,
    "children": null
  }
}
```

Normally we return errors in this different body format (see `Shirt_EnsureCorrectSizingAttribute`):

```json
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Fri, 28 Nov 2025 16:22:00 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Size": [
      "Invalid size for female shirt. Minimum size is 6."
    ]
  },
  "traceId": "00-b5abbed87db46a75b50e0af8ad05e2fb-9788ce5b84aa771a-00"
}
```

Can we make `Shirt_ValidateShirtIdFilterAttribute` use the same format?

## Part 2

`Shirt_ValidateShirtIdFilterAttribute` does not return "type" and "traceId" like the Validators return.

We want to fully support the ASP.NET Core [RFC 7807](https://www.rfc-editor.org/rfc/rfc7807.html) tandard that defines a standardized format for conveying machine-readable details about errors in HTTP responses.

Key features of RFC 7807:

- Problem Details Object: The core of the RFC is the "Problem Details Object," typically represented in JSON or XML format.
- Standardized Fields: It defines several standard fields within this object to provide comprehensive error details:
  - type: A URI reference identifying the problem type, ideally linking to human-readable documentation.
  - title: A short, human-readable summary of the problem type.
  - status: The HTTP status code associated with the problem.
  - detail: A human-readable explanation specific to the particular occurrence of the problem.
  - instance: A URI reference identifying the specific occurrence of the problem.
