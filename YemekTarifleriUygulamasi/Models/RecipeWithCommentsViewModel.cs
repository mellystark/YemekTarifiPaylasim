namespace YemekTarifleriUygulamasi.Models
{
    public class RecipeWithCommentsViewModel
    {
        public Recipe Recipe { get; set; }
        public List<CommentWithUserViewModel> Comments { get; set; }
    }
}
