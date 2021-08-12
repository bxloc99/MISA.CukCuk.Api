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

namespace MISA.CukCuk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEmployees()
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
            var sqlCommand = "SELECT * FROM Employee";
            var employees = dbConnection.Query<object>(sqlCommand);

            // 4. Trả về cho client
            var response = StatusCode(200, employees);
            return response;
        }

        [HttpGet("{employeeId}")]
        public IActionResult GetById(Guid employeeId)
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
            var sqlCommand = "SELECT * FROM Employee WHERE EmployeeId = @EmployeeIdParam";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeIdParam", employeeId);
            var employee = dbConnection.Query<object>(sqlCommand, param: parameters);

            // 4. Trả về cho client
            var response = StatusCode(200, employee);
            return response;
        }

        [HttpPost]
        public IActionResult Insertemployee(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid();

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
            var properties = employee.GetType().GetProperties();

            // Duyệt từng property
            foreach (var prop in properties)
            {
                // Lấy tên của prop
                var propName = prop.Name;

                // Lấy value của prop
                var propValue = prop.GetValue(employee);

                // Lấy kiểu của prop    
                var propType = prop.PropertyType;

                // Thêm param với mỗi property của đối tượng
                dynamicParam.Add($"@{propName}", propValue);

                columnsName += $"{propName},";
                columnsParam += $"@{propName},";
            }

            columnsName = columnsName.Remove(columnsName.Length - 1, 1);
            columnsParam = columnsParam.Remove(columnsParam.Length - 1, 1);

            var sqlCommand = $"INSERT INTO Employee({columnsName}) VALUES({columnsParam})";

            var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParam);

            // 4. Trả về cho client
            var response = StatusCode(200, rowEffects);
            return response;
        }

        [HttpPut]
        public IActionResult UpdateEmployee(Employee employee)
        {
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
            var properties = employee.GetType().GetProperties();

            // Duyệt từng property
            foreach (var prop in properties)
            {
                // Lấy tên của prop
                var propName = prop.Name;

                // Lấy value của prop
                var propValue = prop.GetValue(employee);

                // Lấy kiểu của prop    
                var propType = prop.PropertyType;

                // Thêm param với mỗi property của đối tượng
                dynamicParam.Add($"@{propName}", propValue);

                value += $"{propName} = @{propName},";
            }
            value = value.Remove(value.Length - 1, 1);


            var sqlCommand = $"UPDATE Employee SET {value} WHERE EmployeeId = @EmployeeIdParam";
            dynamicParam.Add("@EmployeeIdParam", employee.EmployeeId);

            var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParam);

            // 4. Trả về cho client
            var response = StatusCode(200, rowEffects);
            return response;
        }

        [HttpDelete("{employeeId}")]
        public IActionResult DeleteEmployee(Guid employeeId)
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

            var sqlCommand = $"DELETE FROM Employee WHERE EmployeeId = @EmployeeIdParam";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeIdParam", employeeId);
            var rowEffects = dbConnection.Execute(sqlCommand, param: parameters);

            // 4. Trả về cho client
            var response = StatusCode(200, rowEffects);
            return response;
        }
    }
}
