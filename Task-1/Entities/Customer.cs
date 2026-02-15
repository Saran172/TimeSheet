using System.ComponentModel.DataAnnotations;

namespace Task_1.Entities
{
    public class Customer
    {
        [Key] public int Cust_Id { get; set; }
        [Required]
        public string Cust_Code { get; set; }
        [Required]
        public string Cust_Name { get; set; }
        [Required]
        public string Cust_ContactNo { get; set; }
        [Required]
        public string Cust_Email { get; set; }
        [Required]
        public DateTime Created_on { get; set; } = DateTime.Now;
        [Required]
        public DateTime Created_DateTime { get; set; } = DateTime.Now;
        [Required]
        public DateTime Updated_on { get; set; } = DateTime.Now;
        [Required]
        public DateTime Updated_DateTime { get; set; } = DateTime .Now;
    }
}
