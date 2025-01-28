using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YemekTarifleriUygulamasi.Helpers
{
    public static class CustomHtmlHelpers
    {
        public static IHtmlContent DeleteConfirmation(this IHtmlHelper htmlHelper, int id, string title, string deleteAction, string cancelAction, string controller)
        {
            string confirmationHtml = $@"
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f9f9f9;
                        padding: 20px;
                        margin-top: 160px;

                    }}
                    .delete-confirmation {{
                        background-color: #fff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        max-width: 500px;
                        margin: 0 auto;
                    }}
                    .delete-confirmation p {{
                        font-size: 16px;
                        color: #333;
                    }}
                    .btn {{
                        padding: 10px 20px;
                        margin: 10px 5px;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                    .btn-danger {{
                        background-color: #d9534f;
                        color: white;
                    }}
                    .btn-secondary {{
                        background-color: #6c757d;
                        color: white;
                    }}
                </style>
                <div class='delete-confirmation'>
                    <p>Are you sure you want to delete <strong>{title}</strong>?</p>
                    <form action='/{controller}/{deleteAction}' method='post'>
                        <input type='hidden' name='id' value='{id}' />
                        <button type='submit' class='btn btn-danger'>Delete</button>
                        <a href='/{controller}/{cancelAction}' class='btn btn-secondary'>Cancel</a>
                    </form>
                </div>";

            return new HtmlString(confirmationHtml);
        }
    }
}
