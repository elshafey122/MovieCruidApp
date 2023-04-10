using System.ComponentModel.DataAnnotations;

namespace CruidApplication.Models
{
    public class Film
    {
        public int Id { set; get; }
        [Required, MaxLength(250)]
        public string Title { set; get; }
        public int Year { set; get; }
        public double Rate { set; get; }
        [Required, MaxLength(2500)]
        public string StoreLine { set; get; }
        [Required]
        public byte GenreId { set; get; }
        public byte[] Poster { set; get; }
        public Genre Genre { set; get; }
    }
}
