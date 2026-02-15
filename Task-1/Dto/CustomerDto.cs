using System.ComponentModel.DataAnnotations;

namespace Task_1.Dto
{
    public class CustomerDto
    {
        public int Cust_Id { get; set; }
        public string Cust_Code { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_ContactNo { get; set; }
        public string Cust_Email { get; set; }
        public DateTime Created_on { get; set; }
        public DateTime Created_DateTime { get; set; }
        public DateTime Updated_on { get; set; }
        public DateTime Updated_DateTime { get; set; }
    }
}
