using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Stock? Stock { get; set; } 
        [ForeignKey(nameof(StockId))]
        public int StockId { get; set; } //FK = 5
        public AppUser? AppUser { get; set; }
        [ForeignKey(nameof(AppUserId))]
        public int AppUserId { get; set; }
    }
}
