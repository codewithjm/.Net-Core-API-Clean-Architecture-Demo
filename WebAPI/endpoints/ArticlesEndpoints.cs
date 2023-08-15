
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Shared.Commons.Consts;
using WebAPI.Extensions;

namespace WebAPI.Endpoints;



public static class ArticlesEndpoints
{

    public static WebApplication MapArticleEndpoints(this WebApplication app)
    {
        const string route = ApiRouteConst.ApiPath;

        _ = app.MapGet(route + "test/article",[AllowAnonymous] async () =>
        {
            return Results.Ok(true);
        }).AddSummary<int>(SwaggerTagsConst.ARTICLE_TAG, "Article List", "\n GET /test articles");

        return app;
    }
}
