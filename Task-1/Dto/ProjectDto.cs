using System.ComponentModel.DataAnnotations;

namespace Task_1.Dto
{
    public class ProjectDto
    {
        public int PROJ_ID { get; set; }
        public int SO_NUMBER { get; set; }
        public DateTime SO_DATE { get; set; }
        public string CUST_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string PROJECT_NAME { get; set; }
        public DateTime DUEDATE { get; set; }
        public string PROJECT_STATUS { get; set; }
        public string? IsBillable { get; set; }
        public DateTime CREATED_ON { get; set; }
        public DateTime CREATED_DATETIME { get; set; }
        public DateTime UPDATED_ON { get; set; }
        public DateTime UPDATED_DATETIME { get; set; }
    }
}
