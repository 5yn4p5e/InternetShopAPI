using InternetShop.Interfaces;
using InternetShop.InternetShopModels;
using InternetShop.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository userRepository)
        {
            _accountRepository = userRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            User user = new() { Email = model.Email, UserName = model.Email };
            // Добавление нового пользователя
            var result = await _accountRepository.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Установка роли User
                await _accountRepository.AddToRoleAsync(user, "user");
                // Установка куки
                await _accountRepository.SignInAsync(user, false);
                return Ok(new { message = "Добавлен новый пользователь: " + user.UserName });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                var errorMsg = new
                {
                    message = "Пользователь не добавлен",
                    error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                };
                return Created("", errorMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _accountRepository.PasswordSignInAsync(model.Email, model.Password, model.RememberMe);
            if (result.Succeeded)
            {
                var user = await _accountRepository.FindByEmailAsync(model.Email);
                IList<string>? roles = await _accountRepository.GetRolesAsync(user);
                string? userRole = roles.FirstOrDefault();
                return Ok(new { message = "Выполнен вход", userName = model.Email, userRole });
            }
            else
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                var errorMsg = new
                {
                    message = "Вход не выполнен",
                    error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                };
                return Created("", errorMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logoff()
        {
            User usr = await _accountRepository.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new { message = "Сначала выполните вход" });
            }
            // Удаление куки
            await _accountRepository.SignOutAsync();
            return Ok(new { message = "Выполнен выход", userName = usr.UserName });
        }

        [HttpGet]
        public async Task<IActionResult> IsAuthenticated()
        {
            User usr = await _accountRepository.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });
            }
            IList<string> roles = await _accountRepository.GetRolesAsync(usr);
            string? userRole = roles.FirstOrDefault();
            return Ok(new { message = "Сессия активна", userName = usr.UserName, userRole });
        }
    }
}