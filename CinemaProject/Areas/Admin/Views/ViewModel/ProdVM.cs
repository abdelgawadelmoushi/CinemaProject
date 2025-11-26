namespace CinemaProject.Areas.Admin.Views.ViewModel
{
    public class ProdVM
    {
        public List<Movie> movie { get; set; }=new List<Movie>();
        public List<Category> categories { get; set; } = new List<Category>();
        public List<Cinema> cinemas { get; set; } = new List<Cinema>();

        public string? Name { get; set; }

       public int Page { get; set; }
       public decimal MinPrice { get; set; }


    }
}
