using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_1.Entities
{
    public class Project
    {
        [Key]
        public int Proj_Id { get; set; }
        [Required]
        public int So_Number { get; set; }
        [Required]
        public DateTime So_Date { get; set; }
        [Required]
        public string Cust_Code {  get; set; }
        [NotMapped]
        public string? Cust_Name { get; set; }
        [Required]
        public string Project_Name { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public string Project_Status { get; set; }
        public string? IsBillable { get; set; }
        [Required]
        public DateTime Created_On { get; set; }= DateTime.Now;
        [Required]
        public DateTime Created_DateTime { get; set; }=DateTime.Now;
        [Required]
        public DateTime Updated_On { get; set; } = DateTime.Now;
        [Required]
        public DateTime Updated_DateTime { get; set; } = DateTime.Now;

    }
}
