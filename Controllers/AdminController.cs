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
using System.Text.RegularExpressions;

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
        [HttpGet("User")]
        public IActionResult GetUsers(string token, [FromQuery]User u)
        {

            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            int UserId = JWTValidator.IDTokenValidation(token);
            if (RoleId)
            {
                var query = _context.users.Where(f => f.Id != UserId).OrderBy(m => u.Id);
                return Ok(query);
            }
            else
            {

                return Unauthorized();

            }
        }
        [HttpGet("Images")]
        public IActionResult GetImages(string token, [FromQuery]ImageURL i)
        {

            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var query = _context.imageURLs.OrderBy(m => i.Id);
                return Ok(query);
            }
            else
            {

                return Unauthorized();

            }
        }
        //Get Products
        [HttpGet("Product")]
        public IActionResult GetProducts(string token, [FromQuery]Product p)
        {

            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var query = from pr in _context.products
                            join i in this._context.imageURLs
                            on pr.Id equals i.ProductId into imageURLsGroup
                            let NoI = imageURLsGroup.Count()
                            select new {
                                Product = pr,
                                NumberOfImages = NoI
                            };
                return Ok(query);
            }
            else
            {

                return Unauthorized();

            }
        }

        //Create user
        [HttpPost("User/Add")]
        public IActionResult CreateUser(string token, [FromBody]User u, string name, string lastname, string birthday, string password, string gender, string streetname, string email, string housenumber, string addition, string postalcode, string city, string phonenumber)
        {

            bool RoleId = JWTValidator.RoleIDTokenValidation(token);

            if (RoleId)
            {
                var UserData = from user in _context.users
                               where (name == u.Name &&
                               lastname == u.LastName &&
                               birthday == u.Birthday &&
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

                    if (u.Name == null || u.LastName == null || u.Birthday == null || u.Password == null || u.Gender == null || u.Street_Name == null || u.email == null || u.House_Number == null || u.Addition == null || u.Postalcode == null || u.City == null || u.Telephone_Number == null)
                        {
                            return BadRequest("A.U.B Alle velden invullen");
                        }

                    //Check for potential errors
                    bool DupeMail = _context.users.Any(Dupe => Dupe.email == u.email);
                    bool PhoneCheck = _context.users.Any(CheckPhone => CheckPhone.Telephone_Number == u.Telephone_Number);


                    //Criteria check
                    if (DupeMail)
                    {
                        return BadRequest("Email bestaat niet of is al in gebruik");
                    }
                    if (PhoneCheck)
                    {
                        return BadRequest("Telefoon nummer bestaat niet of is al in gebruik");
                    }
                    if (DupeMail == false && PhoneCheck == false)
                    {
                        if(ModelState.IsValid)
                                    {
                                        _context.users.Add(u);
                                        _context.SaveChanges();
                                        return Ok("Account Created");
                                    } else {
                                                var errors = ModelState.Select(x => x.Value.Errors)
                                                    .Where(y=>y.Count>0)
                                                    .ToList();
                                                    return BadRequest(errors);
                                            }
                    }

                return Ok("Account Created.");
            }
            else
            {

                return Unauthorized();

            }

        }

        //Edit User
        [HttpPut("User/Edit/{userid}")]
        public IActionResult Update(string token, int userid, [FromBody]User user)
        {


            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            var edit = _context.users.Find(userid);
            if (RoleId)
            {
                if (user.Name != null)
                {
                    edit.Name = user.Name;
                }
                if (user.LastName != null)
                {
                    edit.LastName = user.LastName;
                }
                if (user.Birthday != null)
                {
                    edit.Birthday = user.Birthday;
                }
                if (user.Gender != null)
                {
                    edit.Gender = user.Gender;
                }
                if (user.Password != null)
                {
                    edit.Password = user.Password;
                }
                if (user.Street_Name != null)
                {
                    edit.Street_Name = user.Street_Name;
                }
                if (user.email != null)
                {
                    edit.email = user.email;
                }
                if (user.House_Number != null)
                {
                    edit.House_Number = user.House_Number;
                }
                if (user.Addition != null)
                {
                    edit.Addition = user.Addition;
                }
                if (user.Postalcode != null)
                {
                    edit.Postalcode = user.Postalcode;
                }
                if (user.City != null)
                {
                    edit.City = user.City;
                }

                if (user.Telephone_Number != null)
                {
                    edit.Telephone_Number = user.Telephone_Number;
                }

                //Check for potential errors
                bool DupeMail = _context.users.Any(Dupe => Dupe.email == user.email);
                bool PhoneCheck = _context.users.Any(CheckPhone => CheckPhone.Telephone_Number == user.Telephone_Number);


                //Criteria check
                if (DupeMail)
                {
                    return BadRequest("Email bestaat niet of is al in gebruik");
                }
                if (PhoneCheck)
                {
                    return BadRequest("Telefoon nummer bestaat niet of is al in gebruik");
                }
                if (DupeMail == false && PhoneCheck == false)
                {
                    if (ModelState.IsValid)
                    {
                        _context.users.Update(edit);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                    }
                }

                return Ok("Account edited");

            }
            else
            {

                return Unauthorized();

            }
        }
        //Add product
        [HttpPost("Product/Add")]
        public IActionResult ProductAdd(string token, [FromBody]Product p, string name, string Description, float price, string FirstIMG, int stock, int category, int subcategory)
        {


            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {
                var a = this._context.products.OrderByDescending(pr => pr.Id).FirstOrDefault();
                var ProductData = from product in _context.products
                                  where (name == p.Name &&
                                  Description == p.Description &&
                                  price == p.Price &&
                                  FirstIMG == p.FirstImg &&
                                  stock == p.Stock &&
                                  category == p.CategoryId &&
                                  subcategory == p.SubCategoryId)
                                  select p;
                                p.Id = a.Id + 1;

                if (p.Name == null || p.Description == null || p.Price == default(float) || p.Price <= 0 || p.FirstImg == null || p.CategoryId <= 0 || p.SubCategoryId <= 0)
                {
                    return BadRequest("A.U.B vul alle velden in met geldige waarde (CategoryId, SubCategory en Price moeten groter zijn dan 0)");
                }

                if (ModelState.IsValid)
                {
                    _context.products.Add(p);
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("Product added");
            }
            else
            {

                return Unauthorized();

            }

        }

        //Add images
        [HttpPost("Product/Add/Images/{productid}")]
        public IActionResult AddImage(string token, [FromBody]List<ImageURL> imageURLs, int productid)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {

                if (ModelState.IsValid)
                {
                    foreach (var item in imageURLs)
                    {
                        ImageURL imageURL = new ImageURL();
                        imageURL.url = item.url;
                        imageURL.ProductId = productid;
                        _context.Add(imageURL);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("Image(s) Added");
            }

            return Unauthorized();
        }

        [HttpPut("Product/Edit/{productid}")]
        public IActionResult ProductEdit(string token, int productid, [FromBody]Product p)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            var edit = _context.products.Find(productid);
            if (RoleId)
            {
                if (p.Name != null)
                {
                    edit.Name = p.Name;
                }
                if (p.Description != null)
                {
                    edit.Description = p.Description;
                }
                if(p.Price != default(float)){
                edit.Price = p.Price;
                } 
                if (p.FirstImg != null)
                {
                    edit.FirstImg = p.FirstImg;
                }
                if(p.Stock != default(int)){
                edit.Stock = p.Stock;
                }

                if (ModelState.IsValid)
                {
                    _context.products.Update(edit);
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("Product updated");
            }

            return Unauthorized();

        }

        //Delete user
        [HttpDelete("User/Delete")]
        public IActionResult DeleteProduct(string token, [FromBody]List<User> users)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {

                if (ModelState.IsValid)
                {
                    foreach (var item in users)
                    {
                        User user = new User();
                        user.Id = item.Id;
                        _context.Remove(user);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("User(s) Deleted");
            }

            return Unauthorized();
        }

        //Delete product
        [HttpDelete("Product/Delete")]
        public IActionResult DeleteProduct(string token, [FromBody]List<Product> products)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {

                if (ModelState.IsValid)
                {
                    foreach (var item in products)
                    {
                        Product product = new Product();
                        product.Id = item.Id;
                        _context.Remove(product);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("Product(s) Deleted");
            }

            return Unauthorized();
        }

        //Delete images
        [HttpDelete("Product/Delete/Images")]
        public IActionResult DeleteImage(string token, [FromBody]List<ImageURL> imageURLs)
        {
            bool RoleId = JWTValidator.RoleIDTokenValidation(token);
            if (RoleId)
            {

                if (ModelState.IsValid)
                {
                    foreach (var item in imageURLs)
                    {
                        ImageURL imageURL = new ImageURL();
                        imageURL.Id = item.Id;
                        _context.Remove(imageURL);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y=>y.Count>0)
                                        .ToList();
                                        return BadRequest(errors);
                }

                return Ok("Images Deleted");
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