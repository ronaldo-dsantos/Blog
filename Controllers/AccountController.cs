﻿using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers
{    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly BlogDataContext _context;

        public AccountController(TokenService tokenService, BlogDataContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> Post(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = registerViewModel.Name,
                Email = registerViewModel.Email,
                Slug = registerViewModel.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05x99 - Este e-mail já está cadastrado"));                
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna do servidor"));
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await _context.Users.AsNoTracking()
                                           .Include(u =>u.Roles)
                                           .FirstOrDefaultAsync(u => u.Email == loginViewModel.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, loginViewModel.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna do servidor"));
            }
        }
    }
}
