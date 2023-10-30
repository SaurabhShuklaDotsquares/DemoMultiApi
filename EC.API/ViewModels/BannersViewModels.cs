using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class BannersViewModels
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int type { get; set; }
    }
}
