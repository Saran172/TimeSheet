using System.Reflection.Metadata;

namespace Task_1.Entities
{
    public class WorkApproval
    {
        public int Id { get; set; }
        public int Work_Id { get; set; }    
        public int Emp_Id { get; set; }
        public int Manager_Id { get; set; }
        public string? Approval_Status {  get; set; }
        public string? Comment {  get; set; }
        public string? Correction {  get; set; }
        public DateTime Approval_Date {  get; set; }=DateTime.Today;
    }
    public class WorkApprovalDTO
    {
        public int Work_Id { get; set; }
        public DateTime Work_Date { get; set; }
        public string Emp_Name { get; set; }
        public string Cust_Name { get; set; }
        public string Proj_Name { get; set; }
        public string? Work_Descriptions { get; set; }
        public string? Approval_Status { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public DateTime Approval_Date { get; set; }
    }
}
