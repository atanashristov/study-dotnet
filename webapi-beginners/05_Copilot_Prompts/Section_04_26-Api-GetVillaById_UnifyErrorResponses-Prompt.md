# Section_04_26-Api-GetVillaById_UnifyErrorResponses-Prompt.md

## Summary

I want unified error responses from `GetVillaById`.

## Details

Here are few examples of error responses from `GET api/villa/N`.

- 400 Bad Request

```http
GET {{RoyalVillaApi_HostAddress}}/api/villas/-1

HTTP/1.1 400 Bad Request
Connection: close
Content-Type: text/plain; charset=utf-8
Date: Fri, 02 Jan 2026 22:55:55 GMT
Server: Kestrel
Transfer-Encoding: chunked

Invalid villa ID.
```

- 404 Not Found

```http
GET {{RoyalVillaApi_HostAddress}}/api/villas/100

HTTP/1.1 404 Not Found
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Fri, 02 Jan 2026 22:57:15 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-f7f0666ddf0139f797939f7434451d93-1d99fe78cd524abb-00"
}
```

- 500 Internal Server Error

```http
GET {{RoyalVillaApi_HostAddress}}/api/villas/1

HTTP/1.1 500 Internal Server Error
Connection: close
Content-Type: application/json; charset=utf-8
Date: Fri, 02 Jan 2026 22:58:11 GMT
Server: Kestrel
Transfer-Encoding: chunked

"An error occurred while processing your request."
```

## Request

For the http response codes:

- Client error responses (400 – 499)
- Server error responses (500 – 599)

we want to always return rfc9110 style result, such as above for 404 Not Found.

If _Program.cs_ needs changes, please create a method or a class, but don't pollute the main code in _Program.cs_ with too much setup code.

Any notes, instructions or remarks for me, add to "Section_04_26-Api-GetVillaById_UnifyErrorResponses-Result.md" next to this file.

Also, add a brief summary to the end of "02_Notes/Section_04-Api_Endpoints.md".

In addition to the above, extract base controller class and in it add a method to initialize  the "Problem" result.

Eventually, make these helper Problem methods static, if it makes sense.
