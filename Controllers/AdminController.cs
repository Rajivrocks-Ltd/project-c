using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projectC.model;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using projectC.JWT;

namespace projectC.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {


        ProjectContext _context;
        public AdminController(ProjectContext context)
        {
            this._context = context;
        }
        // GET Users
        [HttpGet]
        public IQueryable<User> GetUsers()
        {
            var result = from m in this._context.users orderby m.Id select m;

            return result;
        }
        //Get Products
        [HttpGet]
        public IQueryable<Product> GetProducts()
        {
            var result = from m in this._context.products orderby m.Id select m;

            return result;
        }

        //Create user
        [HttpPost("User/Add")]
        public IActionResult CreateUser(string token, [FromBody]User u, string name, string lastname, int age, string password, string gender, string streetname, string email, string housenumber, string addition, string postalcode, string city, string phonenumber)
        {

            bool RoleId = JWTValidator.RoleIDTokenValidation(token);

            if (RoleId)
            {
                var UserData = from user in _context.users
                               where (name == u.Name &&
                               lastname == u.LastName &&
                               age == u.Age &&
                               password == u.Password &&
                               gender == u.Gender &&
                               streetname == u.Street_Name &&
                               email == u.email &&
                               housenumber == u.House_Number &&
                               addition == u.Addition &&
                               postalcode == u.Postalcode &&
                               city == u.City &&
                               phonenumber == u.Telephone_Number)
                               select u;

                _context.users.Add(u);
                _context.SaveChanges();

                return Ok("Account Created.");
            }
            else
            {

                return Unauthorized();

            }

        }

        //Edit User
        [HttpPut("User/Edit")]
        public IActionResult Update(string token, [FromBody]User user)
        {


            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            int id = JWTValidator.IDTokenValidation(token);
            var edit = _context.users.Find(id);
            if (RoleId)
            {
                edit.Name = user.Name;
                edit.LastName = user.LastName;
                edit.Age = user.Age;
                edit.Gender = user.Gender;
                edit.Password = user.Password;
                edit.Street_Name = user.Street_Name;
                edit.email = user.email;
                edit.House_Number = user.House_Number;
                edit.Addition = user.Addition;
                edit.Postalcode = user.Postalcode;
                edit.City = user.City;
                edit.Telephone_Number = user.Telephone_Number;

                _context.users.Update(edit);
                _context.SaveChanges();

                return Ok();
            }
            else
            {

                return Unauthorized();

            }

        }
        //Add product
        [HttpPost("Product/Add")]
        public IActionResult ProductAdd(string token, [FromBody]Product p, User u, string name, string Description, float price, string FirstIMG, int stock)
        {


            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var ProductData = from Product in _context.products
                                  where (name == p.Name &&
                                  Description == p.Description &&
                                  price == p.Price &&
                                  FirstIMG == p.FirstImg &&
                                  stock == p.Stock)
                                  select p;

                _context.products.Add(p);
                _context.SaveChanges();

                return Ok("Product added");
            }
            else
            {

                return Unauthorized();

            }

        }

        [HttpPut("Product/Edit/{productid}")]
        public IActionResult ProductEdit(string token, int productid, [FromBody]Product p)
        { 
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            var edit = _context.products.Find(productid);
            if (RoleId)
            {
                if(p.Name != null){
                edit.Name = p.Name;
                Console.WriteLine(p.Name);
                } else {
                    edit.Name = edit.Name;
                    Console.WriteLine("dombo");
                }
                if(p.Description != null){
                edit.Description = p.Description;
                } else {
                    edit.Description = edit.Description;
                }
                edit.Price = p.Price;
                if(p.FirstImg != null){
                edit.FirstImg = p.FirstImg;
                } else{
                    edit.FirstImg = edit.FirstImg;
                }
                edit.Stock = p.Stock;

                _context.products.Update(edit);
                _context.SaveChanges();

                return Ok();
            }
            
                return Unauthorized();

        }

        //Delete user
        [HttpDelete("User/Delete/{userid}")]
        public IActionResult DeleteUser(string token, int userid, [FromBody]User user)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var remove = (from u in _context.users
                              where u.Id == userid
                              select u).FirstOrDefault();

                if (remove != null)
                {
                    _context.users.Remove(remove);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {

                    return NotFound();

                }
                
            }
            
            return Unauthorized();
        
        }

        //Delete product
        [HttpDelete("Product/Delete/{productid}")]
        public IActionResult DeleteProduct(string token, int productid, [FromBody]User u)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var remove = (from p in _context.products
                              where productid == p.Id
                              select p).FirstOrDefault();

                if (remove != null)
                {
                    _context.products.Remove(remove);
                    _context.SaveChanges();
                    return Ok("Product Deleted");
                }
                else
                {

                    return NotFound();

                }
                
            }
            
            return Unauthorized();
        
        }

        [HttpGet("Status")]
        public Boolean CheckAdminStatus(string token)
        {
            bool Validation = JWTValidator.RoleIDTokenValidation(token);
            return Validation;
        }
    }
}





