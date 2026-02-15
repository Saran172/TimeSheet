using System.ComponentModel.DataAnnotations;

namespace Task_1.Entities
{
    public class Employee
    {
        [Key]
        public int EMP_ID { get; set; }
        public string EMP_CODE { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_CONTACTNO { get; set; }
        public string? EMP_CONTACTMAIL { get; set; }
        public string Role { get; set; }
        public string Reporting_Manager { get; set; }
        public string EMP_DESIGNATION { get; set; }
        public string? EMP_STATUS { get; set; }
        public string EMP_PASSWORD { get; set; }
        public DateTime? CREATED_ON { get; set; } = DateTime.Now;
        public DateTime? CREATED_DATETIME { get; set; } = DateTime.Now;
        public DateTime? UPDATED_ON { get; set; } = DateTime.Now;
        public DateTime? UPDATED_DATETIME { get; set; } = DateTime.Now;
    }
}
