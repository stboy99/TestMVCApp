using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using TestMVCApp.Models;
using TestMVCApp.Models.Global;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserRepository _repo;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserRepository repo, ILogger<AccountController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        var model = new LoginFormViewModel
        {
            FormAction = "/Account/Login",
            ButtonText = "Login",
            Fields = new List<FormField>
            {
                new FormField { Name = "Username", Label = "Username", Type = "text", IsRequired = true },
                new FormField { Name = "Password", Label = "Password", Type = "password", IsRequired = true }
            }
        };
        return View(model);
    } 
    [HttpPost("Login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _repo.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("Index", "Person");
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        var model = new RegisterFormViewModel
        {
            FormAction = "/Account/Register",
            ButtonText = "Register",
            Register = new Register(),
            Fields = new List<FormField>
            {
                new FormField { Name = "Username", Label = "Username", Type = "text", IsRequired = true },
                new FormField { Name = "Password", Label = "Password", Type = "password", IsRequired = true }
            }
        };
        return View(model);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(Register model)
    {
       _logger.LogInformation("Registering user: {username}", model.Username);
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _repo.RegisterAsync(model.Username, model.Password);

            // Optional: Auto-login after registration
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Username)
        };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("Index", "Home");
        }
        catch (ApplicationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
}
