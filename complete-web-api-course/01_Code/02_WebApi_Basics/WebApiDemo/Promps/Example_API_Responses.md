# Examples of API responses

## Examples of errors API responses

### GET {{WebApiDemo_BaseUrl}}/api/shirts/{{firstItemId}}

```json
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application / problem + json; charset = utf-8
Date: Wed, 24 Dec 2025 22: 46: 17 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "<https://tools.ietf.org/html/rfc7807#section-3.1>",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "instance": "/api/shirts/AA11",
  "errors": {
    "id": [
      "The value 'AA11' is not valid."
    ]
  },
  "traceId": "00-4e9f32dca61d2a34afe2d1799fadf393-7ba2d364d1c86054-00"
}
```

### GET {{WebApiDemo_BaseUrl}}/api/shirts/1{{firstItemId}}

```json
HTTP/1.1 404 Not Found
Connection: close
Content-Type: application / problem + json; charset = utf-8
Date: Wed, 24 Dec 2025 22: 40:03 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "<https://tools.ietf.org/html/rfc9110#section-15.5.5>",
  "title": "One or more validation errors occurred.",
  "status": 404,
  "instance": "/api/shirts/11",
  "errors": {
    "id": [
      "Shirt does not exist."
    ]
  },
  "traceId": "00-7d8ca1921ec581a7f23a5ad91fdfa8c0-e506211eec8fcdda-00"
}
```

### POST {{WebApiDemo_BaseUrl}}/api/shirts

```json
{
  "brand": "Nike",
  "color": "Yellow",
  "size": 9,
  "gender": "Male",
  "price": 29.99
}
```

```json
HTTP/1.1 409 Conflict
Connection: close
Content-Type: application / problem + json; charset = utf-8
Date: Wed, 24 Dec 2025 22: 49: 11 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "<https://tools.ietf.org/html/rfc9110#section-15.5.1>",
  "title": "One or more validation errors occurred.",
  "status": 409,
  "instance": "/api/shirts",
  "errors": {
    "createShirtDto": [
      "A shirt with the same brand, color, size, and gender already exists."
    ]
  },
  "traceId": "00-5d7762ef782d570c178f9f0a6e5fa5a9-5f3c5c16efc661cd-00"
}
```

### PUT {{WebApiDemo_BaseUrl}}/api/shirts/{{newItemId}}

```json
{
  "brand": "Nike",
  "color": "Yellow",
  "size": 8,
  "gender": "Male",
  "price": 29.99
}
```

```json
HTTP/1.1 409 Conflict
Connection: close
Content-Type: application / problem + json; charset = utf-8
Date: Wed, 24 Dec 2025 22: 59: 33 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "<https://tools.ietf.org/html/rfc9110#section-15.5.10>",
  "title": "One or more validation errors occurred.",
  "status": 409,
  "instance": "/api/shirts/18",
  "errors": {
    "updateShirtDto": [
      "A shirt with the same brand, color, size, and gender already exists."
    ]
  },
  "traceId": "00-589b498443c2b45acf2328252a6eb54a-7473e58975f26856-00"
}
```

```json
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Wed, 24 Dec 2025 23:19:44 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7807#section-3.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "instance": "/api/shirts/18",
  "errors": {
    "size": [
      "Invalid size for male shirt. Minimum size is 8."
    ]
  },
  "traceId": "00-08e7305c11eded02554599f186c93598-14005217aa88c42f-00"
}
```
