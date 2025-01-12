﻿
using Microsoft.AspNetCore.Components.Forms;

namespace MinimalAPIsMovies.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // This is the code that will execute before the endpoint
            var result = await next(context);
            // This is the code the will execute after the endpoint
            return result;
        }
    }
}
