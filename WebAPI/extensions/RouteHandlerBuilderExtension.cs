using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using WebAPI.Middlewares;
using Microsoft.AspNetCore.Http.Json;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace WebAPI.Extensions;

public static class RouteHandlerBuilderExtension
{
    public static RouteHandlerBuilder AddSummary<TResponse>(this RouteHandlerBuilder builder, string tags = "", string title = "", string description = "")
    {
        _ = builder
            .WithTags(tags)
            .WithMetadata(new SwaggerOperationAttribute(title, description))
            .Produces<TResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces<ApiErrorException>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

        return builder;
    }
}
