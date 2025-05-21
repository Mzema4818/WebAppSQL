using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Employees
    {
        [Key]
        public int ID { get; set; }
        public required string email { get; set; }
        public required string hashPassword { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public string? sessionID { get; set; }
    }
}
