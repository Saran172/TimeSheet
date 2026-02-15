using System.ComponentModel.DataAnnotations;

namespace Task_1.Entities
{
    public class Work
    {
        [Key]
        public int Work_Id { get; set; }
        public DateTime Work_Date { get; set; } = DateTime.Today;
        public string Emp_Name {  get; set; }
        public int Emp_Id { get; set; }
        public int? IsTicket { get; set; }
        public string? Cust_Name { get; set; }
        public string? Proj_Name {  get; set; }
        public string Work_Descriptions { get; set; }
        public string? Ticket_Number { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Total_Hours { get; set; } 
        public int Manager_Id { get; set; }
        public string? Manager_Name { get; set; }
        public string? Manager_Approval {  get; set; }
        public string? Comment { get; set; }
        public string? Correction {  get; set; }
        public DateTime Created_On { get; set; } = DateTime.Now;
        public DateTime Created_DateTime { get; set; } = DateTime.Now;
        public DateTime Updated_On { get; set; } = DateTime.Now;
        public DateTime Updated_DateTime { get; set; } = DateTime.Now;

    
    
    }
    public class Workdto
    {
        public int Work_Id { get; set; }
        public DateTime WORK_DATE { get; set; }
        public string EMP_NAME { get; set; }
        public int EMP_ID { get; set; }
        public string? CUST_NAME { get; set; }
        public string? PROJ_NAME { get; set; }
        public string WORK_DESCRIPTIONS { get; set; }
        public string? Ticket_Number { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public string TOTAL_HOURS { get; set; }
        public int MANAGER_ID { get; set; }
        public string MANAGER_NAME { get; set; }
        public string? MANAGER_APPROVAL { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public DateTime CREATED_ON { get; set; } = DateTime.Now;
        public DateTime CREATED_DATETIME { get; set; } = DateTime.Now;
        public DateTime UPDATED_ON { get; set; } = DateTime.Now;
        public DateTime UPDATED_DATETIME { get; set; } = DateTime.Now;


    }

    public class WorkProjdto
    {
        public int WORK_ID { get; set; }
        public int proj_Id { get; set; }
        public DateTime WORK_DATE { get; set; }
        public string EMP_NAME { get; set; }
        public int EMP_ID { get; set; }
        public string? CUST_NAME { get; set; }
        public string? PROJ_NAME { get;set; }
        public string WORK_DESCRIPTIONS { get; set; }
        public string? TICKET_NUMBER { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public string TOTAL_HOURS { get; set; }
        public int MANAGER_ID { get; set; }
        public string MANAGER_NAME {  get; set; }
        public string? MANAGER_APPROVAL { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public DateTime CREATED_ON { get; set; } 
        public DateTime CREATED_DATETIME { get; set; } 
        public DateTime UPDATED_ON { get; set; } = DateTime.Now;
        public DateTime UPDATED_DATETIME { get; set; } = DateTime.Now;

        public int? So_Number { get; set; }
        public string? CUST_CODE { get; set; }
        public string? IsBillable { get; set; }
        public string? TaskStatus { get; set; }

    }
    public class Prodetaildto
    {
        public int WORK_ID { get; set; }
        public DateTime WORK_DATE { get; set; }
        public string EMP_NAME { get; set; }
        public int EMP_ID { get; set; }
        public string? CUST_NAME { get; set; }
        public string? PROJ_NAME { get;set; }
        public string WORK_DESCRIPTIONS { get; set; }
        public string? Ticket_Number { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public string TOTAL_HOURS { get; set; }
        public int MANAGER_ID { get; set; }
        public string MANAGER_NAME {  get; set; }
        public string? MANAGER_APPROVAL { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public DateTime CREATED_ON { get; set; } = DateTime.Now;
        public DateTime CREATED_DATETIME { get; set; } = DateTime.Now;
        public DateTime UPDATED_ON { get; set; } = DateTime.Now;
        public DateTime UPDATED_DATETIME { get; set; } = DateTime.Now;

        public int So_Number { get; set; }
        public string CUST_CODE { get; set; }
        public string? IsBillable { get; set; }

    }
}
