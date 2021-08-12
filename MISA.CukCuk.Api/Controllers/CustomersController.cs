using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MISA.CukCuk.Api.Model;
using System.Data;
using MySqlConnector;
using Dapper;
using System.Text.RegularExpressions;

namespace MISA.CukCuk.Api.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        /// <summary>
        /// Lấy toàn bộ khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCustomers()
        {
            try
            {
                // Truy cập vào database
                // 1. Khai báo thông tin kết nối database
                var connectionString = "Host = 47.241.69.179;" +
                    "Database = MISA.CukCuk_Demo_NVMANH;" +
                    "User Id = dev;" +
                    "Password = 12345678;";

                // 2. Khởi tạo đối tượng kêt nối database
                IDbConnection dbConnection = new MySqlConnection(connectionString);

                // 3. Lẩy dữ liệu
                var sqlCommand = "SELECT * FROM Customer";
                var customers = dbConnection.Query<object>(sqlCommand);

                // 4. Trả về cho client
                var response = StatusCode(200, customers);
                return response;
            }
            catch(Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.Resource.Exception_ErroMsg,
                    errorCode = "misa-001"
                };
                var response = StatusCode(403, errorObj);
                return response;
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo Id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        public IActionResult GetById(Guid customerId)
        {
            try
            {
                // Truy cập vào database
                // 1. Khai báo thông tin kết nối database
                var connectionString = "Host = 47.241.69.179;" +
                    "Database = MISA.CukCuk_Demo_NVMANH;" +
                    "User Id = dev;" +
                    "Password = 12345678;";

                // 2. Khởi tạo đối tượng kêt nối database
                IDbConnection dbConnection = new MySqlConnection(connectionString);

                // 3. Lẩy dữ liệu
                var sqlCommand = "SELECT * FROM Customer WHERE CustomerId = @CustomerIdParam";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CustomerIdParam", customerId);
                var customer = dbConnection.Query<object>(sqlCommand, param: parameters);

                // 4. Trả về cho client
                if(customer == null)
                {
                    var response = StatusCode(204);
                    return response;
                }
                else
                {
                    var response = StatusCode(200, customer);
                    return response;
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.Resource.Exception_ErroMsg,
                    errorCode = "misa-001"
                };
                var response = StatusCode(403, errorObj);
                return response;
            }
        }

        /// <summary>
        /// Thêm khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Insertcustomer(Customer customer)
        {
            try
            {
                //Kiểm tra mã nhân viên
                if (customer.CustomerCode == "" || customer.CustomerCode == null)
                {
                    var exception = new
                    {
                        userMsg = Properties.Resource.EmptyCode,
                        errorCode = "misa-001",
                    };
                    return StatusCode(400, exception);
                }

                //Kiểm tra email

                bool isEmail = Regex.IsMatch(customer.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (isEmail == false)
                {
                    var error = new
                    {
                        devMsg = "CustomerEmail is not correct format",
                        userMsg = "Email " + Properties.Resource.Error,
                        errorCode = "misa-001",

                    };
                    return StatusCode(400, error);
                }

                customer.CustomerId = Guid.NewGuid();

                // Truy cập vào database
                // 1. Khai báo thông tin kết nối database
                var connectionString = "Host = 47.241.69.179;" +
                    "Database = MISA.CukCuk_Demo_NVMANH;" +
                    "User Id = dev;" +
                    "Password = 12345678;";

                // 2. Khởi tạo đối tượng kêt nối database
                IDbConnection dbConnection = new MySqlConnection(connectionString);

                // Khai báo dynamicParam
                var dynamicParam = new DynamicParameters();

                // 3. Thêm dữ liệu vào database
                var columnsName = string.Empty;
                var columnsParam = string.Empty;

                // Đọc từng property của object
                var properties = customer.GetType().GetProperties();

                // Duyệt từng property
                foreach (var prop in properties)
                {
                    // Lấy tên của prop
                    var propName = prop.Name;

                    // Lấy value của prop
                    var propValue = prop.GetValue(customer);

                    // Lấy kiểu của prop    
                    var propType = prop.PropertyType;

                    // Thêm param với mỗi property của đối tượng
                    dynamicParam.Add($"@{propName}", propValue);

                    columnsName += $"{propName},";
                    columnsParam += $"@{propName},";
                }

                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                columnsParam = columnsParam.Remove(columnsParam.Length - 1, 1);

                var sqlCommand = $"INSERT INTO Customer({columnsName}) VALUES({columnsParam})";

                var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParam);

                // 4. Trả về cho client
                var response = StatusCode(200, rowEffects);
                return response;
            }
            catch(Exception ex) {
                var error = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.Resource.Exception_ErroMsg,
                    errorCode = "misa-001",
                };
                return StatusCode(403, error);
            }
        }

        /// <summary>
        /// Sửa khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpdateCustomer(Customer customer)
        {
            try
            {
                //Kiểm tra mã nhân viên
                if (customer.CustomerCode == "" || customer.CustomerCode == null)
                {
                    var exception = new
                    {
                        userMsg = Properties.Resource.EmptyCode,
                        errorCode = "misa-001",
                    };
                    return StatusCode(400, exception);
                }

                //Kiểm tra email

                bool isEmail = Regex.IsMatch(customer.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (isEmail == false)
                {
                    var error = new
                    {
                        devMsg = "CustomerEmail is not correct format",
                        userMsg = "Email " + Properties.Resource.Error,
                        errorCode = "misa-001",

                    };
                    return StatusCode(400, error);
                }

                // Truy cập vào database
                // 1. Khai báo thông tin kết nối database
                var connectionString = "Host = 47.241.69.179;" +
                    "Database = MISA.CukCuk_Demo_NVMANH;" +
                    "User Id = dev;" +
                    "Password = 12345678;";

                // 2. Khởi tạo đối tượng kêt nối database
                IDbConnection dbConnection = new MySqlConnection(connectionString);

                // Khai báo dynamicParam
                var dynamicParam = new DynamicParameters();

                // 3. Sửa dữ liệu trong database
                var value = string.Empty;

                // Đọc từng property của object
                var properties = customer.GetType().GetProperties();

                // Duyệt từng property
                foreach (var prop in properties)
                {
                    // Lấy tên của prop
                    var propName = prop.Name;

                    // Lấy value của prop
                    var propValue = prop.GetValue(customer);

                    // Lấy kiểu của prop    
                    var propType = prop.PropertyType;

                    // Thêm param với mỗi property của đối tượng
                    dynamicParam.Add($"@{propName}", propValue);

                    value += $"{propName} = @{propName},";
                }
                value = value.Remove(value.Length - 1, 1);


                var sqlCommand = $"UPDATE Customer SET {value} WHERE CustomerId = @CustomerIdParam";
                dynamicParam.Add("@CustomerIdParam", customer.CustomerId);

                var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParam);

                // 4. Trả về cho client
                var response = StatusCode(200, rowEffects);
                return response;
            }
            catch(Exception ex)
            {
                var error = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.Resource.Exception_ErroMsg,
                    errorCode = "misa-001",
                };
                return StatusCode(403, error);
            }
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpDelete("{customerId}")]
        public IActionResult DeleteCustomer(Guid customerId)
        {
            try
            {
                // Truy cập vào database
                // 1. Khai báo thông tin kết nối database
                var connectionString = "Host = 47.241.69.179;" +
                    "Database = MISA.CukCuk_Demo_NVMANH;" +
                    "User Id = dev;" +
                    "Password = 12345678;";

                // 2. Khởi tạo đối tượng kêt nối database
                IDbConnection dbConnection = new MySqlConnection(connectionString);

                // 3. Thêm dữ liệu vào database
                var columnsName = string.Empty;
                var columnsParam = string.Empty;

                var sqlCommand = $"DELETE FROM Customer WHERE CustomerId = @CustomerIdParam";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CustomerIdParam", customerId);
                var rowEffects = dbConnection.Execute(sqlCommand, param: parameters);

                // 4. Trả về cho client
                var response = StatusCode(200, rowEffects);
                return response;
            }
            catch(Exception ex)
            {
                var error = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.Resource.Exception_ErroMsg,
                    errorCode = "misa-001",
                };
                return StatusCode(403, error);
            }
        }
    }
}
