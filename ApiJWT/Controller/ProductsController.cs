using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiJWT.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiJWT.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    // Si solo se tiene un sistema de autentiación
    [Authorize]
    // Si se tienen varios sistemas de autenticación
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController(ProductosJwtContext productosJwtContext) : ControllerBase
    {
        private readonly ProductosJwtContext _dbContext = productosJwtContext;

        [HttpGet]
        [Route("GetProducts")]
        public async Task<ActionResult> GetProducts()
        {
            var productL = await _dbContext.Products.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { products = productL});
        }
    } 
}