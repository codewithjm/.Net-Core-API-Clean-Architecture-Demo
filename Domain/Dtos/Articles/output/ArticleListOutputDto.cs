using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Articles.output;

public class ArticleListOutputDto
{
    public int Ident { get; set; }
    public string title { get; set; }
    public DateTime date_created { get; set; }
}
