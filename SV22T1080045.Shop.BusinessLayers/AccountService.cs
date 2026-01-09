//using SV22T1080045.Shop.DataLayers;
//using SV22T1080045.Shop.DomainModels;
//using System.Security.Cryptography;
//using System.Text;

//namespace SV22T1080045.Shop.BusinessLayers
//{
//    public class AccountService
//    {
//        private readonly AccountDAL _accountDAL;

//        public AccountService(AccountDAL accountDAL)
//        {
//            _accountDAL = accountDAL;
//        }

//        private string HashPassword(string password)
//        {
//            using (MD5 md5 = MD5.Create())
//            {
//                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
//                byte[] hashBytes = md5.ComputeHash(inputBytes);
//                return Convert.ToHexString(hashBytes);
//            }
//        }

//        public Customer? Login(string email, string password)
//        {
//            string hashedPassword = HashPassword(password);
//            return _accountDAL.Login(email, hashedPassword);
//        }

//        public string Register(Customer data)
//        {
//            if (_accountDAL.EmailExists(data.Email))
//                return "Email này đã được sử dụng.";

//            data.Password = HashPassword(data.Password);

//            bool result = _accountDAL.Register(data);
//            return result ? "" : "Đăng ký thất bại. Vui lòng thử lại.";
//        }

//        public Customer? GetCustomer(int id)
//        {
//            return _accountDAL.GetCustomerById(id);
//        }

//        public bool UpdateProfile(int id, string name, string phone, string address)
//        {
//            var customer = new Customer
//            {
//                CustomerID = id,
//                CustomerName = name,
//                Phone = phone,
//                Address = address
//            };
//            return _accountDAL.UpdateProfile(customer);
//        }

//        public bool ChangePassword(int id, string oldPass, string newPass)
//        {
//            var customer = _accountDAL.GetCustomerById(id);
//            if (customer == null) return false;

//            string hashedOld = HashPassword(oldPass);
//            if (customer.Password != hashedOld) return false;

//            string hashedNew = HashPassword(newPass);
//            return _accountDAL.ChangePassword(id, hashedNew);
//        }
//    }
//}