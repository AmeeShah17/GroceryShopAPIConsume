namespace GroceryShopAPIConsume.Models
{
    public class CustomerModel
    {
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string MobileNo { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
       
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
    public class CustomerRegisterModel
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string MobileNo { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
    }

    public class CustomerLoginModel
    {
        public string Email { set; get; }
        public string Password { set; get; }
    }
}
