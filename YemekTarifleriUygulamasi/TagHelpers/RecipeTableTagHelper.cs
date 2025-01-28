using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.TagHelpers
{
    [HtmlTargetElement("recipe-table")]
    public class RecipeTableTagHelper : TagHelper
    {
        private readonly IUrlHelper _urlHelper;

        // Constructor ile IUrlHelper enjekte ediliyor.
        public RecipeTableTagHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public List<Recipe> Recipes { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (Recipes == null || Recipes.Count == 0)
            {
                output.Content.SetHtmlContent("<p>Henüz bir tarif eklemediniz.</p>");
                return;
            }

            var table = "<table class=\"table\">" +
                            "<thead>" +
                                "<tr>" +
                                    "<th>Başlık</th>" +
                                    "<th>Kategori</th>" +
                                    "<th>Açıklama</th>" +
                                    "<th>Görsel</th>" +
                                    "<th>İşlemler</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>";

            foreach (var recipe in Recipes)
            {
                table += "<tr>" +
                            $"<td>{recipe.Title}</td>" +
                            $"<td>{recipe.Category}</td>" +
                            $"<td>{recipe.Description}</td>" +
                            "<td>";

                if (!string.IsNullOrEmpty(recipe.ImagePath))
                {
                    table += $"<img src=\"{recipe.ImagePath}\" alt=\"{recipe.Title}\" style=\"width: 100px; height: auto;\" />";
                }
                else
                {
                    table += "<span>Görsel yok</span>";
                }

                table += "</td>" +
                         "<td>" +
                            // URL'yi Recipe/Index olarak değiştirdik.
                            $"<a href=\"{_urlHelper.Action("Index", "Recipe")}\" class=\"btn btn-warning\">Düzenle</a>" +
                            $"<a href=\"{_urlHelper.Action("Delete", "Recipe", new { id = recipe.Id })}\" class=\"btn btn-danger\">Sil</a>" +
                         "</td>" +
                        "</tr>";
            }

            table += "</tbody></table>";

            output.Content.SetHtmlContent(table);
        }
    }
}
