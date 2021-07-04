﻿using BookStoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreRepository.Admin
{
    public class AdminRepo : IAdminRepo
    {
        private readonly IConfiguration config;
        public AdminRepo(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public async Task<string> Login(Login login)
        {
            string conn = config["ConnectionString"];
            SqlConnection connection = new SqlConnection(conn);
            try
            {
                using (SqlCommand command = new SqlCommand("spLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Email", login.Email.ToLower());
                    command.Parameters.AddWithValue("Password", login.Password);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader != null)
                    {
                        string jwt =this.GenerateJWTtokens(login.Email);
                        connection.Close();
                        return await Task.Run(() => jwt);
                    }
                    connection.Close();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }
            finally
            {
                connection.Close();
            }
        }



        public string GenerateJWTtokens(string adminEmail)
        {
            string key = this.config["Jwt:Key"];
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Role, "Admin"),
                            new Claim("Email", adminEmail)
                        }),
                    Expires = DateTime.Now.AddDays(Convert.ToDouble(this.config["Jwt:JwtExpireDays"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
                };
                var securityTokenHandler = new JwtSecurityTokenHandler();
                var securityToken = securityTokenHandler.CreateToken(tokenDescriptor);
                var token = securityTokenHandler.WriteToken(securityToken);
                return token;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
