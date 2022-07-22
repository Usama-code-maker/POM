using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Authentication;
using Authentication.Model;
using Authentication.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Authenication.Models.DataManager
{
    public class EmployeeManager : IDataRepository<Employee>
    {
        readonly EmployeeManager employeeManager;
        readonly EmployeeContext _employeeContext;
        string key = "1prt56";
        private SecurityToken tokeOptions;

        public EmployeeManager(EmployeeContext context)
        {
            _employeeContext = context;
        }
        public IEnumerable<Employee> GetAll()
        {
            return _employeeContext.Employees.ToList();
        }
        public Employee Get(long id)
        {
            return _employeeContext.Employees
                  .FirstOrDefault(e => e.EmployeeId == id);
        }
        public void Add(Employee entity)
        {
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, entity.Password);
            }
            entity.Password = hash;
            _employeeContext.Employees.Add(entity);
            _employeeContext.SaveChanges();
        }
        public void Update(Employee employee, Employee entity)
        {
            employee.FirstName = entity.FirstName;
            employee.LastName = entity.LastName;
            employee.Email = entity.Email;
            employee.DateOfBirth = entity.DateOfBirth;
            employee.PhoneNumber = entity.PhoneNumber;
            _employeeContext.SaveChanges();
        }
        public void Delete(Employee employee)
        {
            _employeeContext.Employees.Remove(employee);
            _employeeContext.SaveChanges();
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }


       public Response Verification(Login login)
        {
            try
            {
                if (login.Email == null && login.Password == null)
                {
                    return null;
                }
                string hash = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    hash = GetMd5Hash(md5Hash, login.Password);
                }

                var data = _employeeContext.Employees.Where(
                                    x =>
                                        x.Email.Equals(login.Email)
                                        && x.Password.Equals(hash)
                                   ).FirstOrDefault();
                if (data is not null)
                {
                  var accesstoken = token(data.Email, data.Password);
                  return new Response
                    {
                        Id = (int)data.EmployeeId,
                        StatusCode = "200",
                        Message = "SuccesFully Login",
                        status = true,
                        Token = accesstoken.Token
                    };
                }
                else
                {
                    return new Response
                    {
                        StatusCode = "404",
                        Message = "Incorrect Email and Password",
                        status = false
                    };
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Response token(object email, object password)
        {
            var response = new AuthenticatedResponse();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new Response { Token = tokenString };

        }
    }
}