using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiJWT.Custom;
using ApiJWT.Models;
using ApiJWT.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiJWT.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AccessController(ProductosJwtContext productosJwtContext, Utilities utilities) : ControllerBase
    {
        private readonly ProductosJwtContext _dbContext = productosJwtContext;
        private readonly Utilities _utilities = utilities;

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(UserDto userDto) {
            var newUser = new User 
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = _utilities.EncryptSha256(userDto.Password)
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            if (newUser.Id != 0) {
                return StatusCode(StatusCodes.Status200OK, new {isSucces = true , user = newUser.Name});
            } else {
                return StatusCode(StatusCodes.Status200OK, new {isSucces = false});
            }
        }

        [HttpPost]
        [Route("LogIn")]
        public async Task<IActionResult> LogIn(LoginDto loginDto)
        {
            var findUser = await _dbContext.Users
                            .Where(u => 
                            u.Email == loginDto.Email &&
                            u.Password == _utilities.EncryptSha256(loginDto.Password)
                            ).FirstOrDefaultAsync();
            
            if (findUser == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = ""});
            else 
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilities.GenerateToken(findUser)});
        }
    }
}