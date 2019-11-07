using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Middleware
{
    public class RestSharpMiddleware
    {
        private readonly RequestDelegate _next;

        public RestSharpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext, IRestClient client)
        {
            await _next(httpContext);
        }
    }
}
