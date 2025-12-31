# FilterAttribute_ResponseErrorFormat_Answer

 Yes! Let me find and update the Shirt_ValidateShirtIdFilterAttribute to return the ProblemDetails format:

Read Shirt_ValidateShirtIdFilterAttribute.cs

Shirt_ValidateShirtIdFilterAttribute.cs+12-3
Perfect! Now Shirt_ValidateShirtIdFilterAttribute returns the same ProblemDetails format:

Changes made:

✅ Wrap ModelState in ValidationProblemDetails object
✅ Set appropriate Status code (400 or 404)
✅ Return as BadRequestObjectResult or NotFoundObjectResult
✅ Added early return after first error to avoid both errors showing
Now both errors return the same format:

Benefits:

✅ Consistent error format across all validation
✅ RFC 7807 compliant
✅ Proper content-type: application/problem+json
✅ Easier for clients to parse

## Part 2

The issue is that ValidationProblemDetails in filters doesn't automatically populate all fields. You need to get them from the controller context or set them manually:

Shirt_ValidateShirtIdFilterAttribute.cs+11-2
Perfect! Now it returns the complete ProblemDetails format with all fields:

What was added:

✅ Type - RFC 9110 reference URLs
400: <https://tools.ietf.org/html/rfc9110#section-15.5.1>
404: <https://tools.ietf.org/html/rfc9110#section-15.5.5>
✅ Title - Standard validation error title
✅ Instance - Request path that caused the error
✅ traceId - Distributed tracing identifier from Activity or HttpContext
Now returns:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "instance": "/api/shirts/-1",
  "errors": {
    "id": [
      "Invalid shirt ID."
    ]
  },
  "traceId": "00-b5abbed87db46a75b50e0af8ad05e2fb-9788ce5b84aa771a-00"
}
```

Now both validation approaches return identical, RFC 7807-compliant responses! 🎉
