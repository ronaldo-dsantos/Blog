using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly BlogDataContext _context;
        private readonly EmailService _emailService;

        public AccountController(TokenService tokenService, BlogDataContext context, EmailService emailService)
        {
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
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

                _emailService.Send(user.Name, user.Email, "Bem vindo ao Blog", $"Sua senha é <strong>{password}</ strong>");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password // Senha enviada via e-mail, retornando aqui apenas para fins de teste
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

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage(UploadImageViewModel uploadImageViewModel)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"^data:image\/[a-zA-Z]+;base64,").Replace(uploadImageViewModel.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna do servidor"));
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            user.Image = $"https://localhost:7159/images/{fileName}";

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();                
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna do servidor"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso", null));
        }
    }
}
