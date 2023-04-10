using CruidApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace CruidApplication.ViewModel
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Title { set; get; }
        public int Year { set; get; }
        [Range(1, 10)]
        public double Rate { set; get; }
        [Required, StringLength(2500)]
        public string StoreLine { set; get; }
        [Display(Name = "Select Poster...")]
        public byte[] Poster { set; get; }
        [Display(Name = "Genre")]
        public byte GenerId { set; get; }
        public IEnumerable<Genre> Genres { set; get; }
    }
}
