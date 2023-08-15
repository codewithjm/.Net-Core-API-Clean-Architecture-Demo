using Domain.Dtos.Articles.output;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Features.Articles.Queries;

public class ArticlesQuery : IRequest<ArticleListOutputDto>
{
}

public class ArticlesQueryHandler : IRequestHandler<ArticlesQuery, ArticleListOutputDto>
{


    public async Task<ArticleListOutputDto> Handle(ArticlesQuery request, CancellationToken cancellationToken)
    {
        var result = new ArticleListOutputDto();
        result.title = "Hello World";
        result.Ident = 1;
        result.date_created = DateTime.Now;

        return result;
    }
}
