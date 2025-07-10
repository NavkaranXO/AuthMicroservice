using System.ComponentModel.DataAnnotations;

namespace MyMicroservice.Models
{
    public class CashCard
    {
        public int Id { get; set; }

        [Required]
        public required string Owner { get; set; } 
        
        public decimal Balance{ get; set; }
    }
}