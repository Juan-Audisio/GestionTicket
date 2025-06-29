using GestionTicket.Models.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



[Route("api/[controller]")]
[ApiController]

public class HomeController : ControllerBase 
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public HomeController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]

    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        //ARMAMOS EL OBJETO COMPLETANDO LOS ATRIBUTOS COMPLETADOS POR EL USUARIO
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NombreCompleto = model.NombreCompleto
        };

        //HACEMOS USO DEL MÉTODO REGISTRAR USUARIO
        var result = await _userManager.CreateAsync(user, "Ezpeleta2025");

        if (result.Succeeded)
            return Ok("Usuario registrado");

        return BadRequest(result.Errors);
    }

    [HttpGet("usuario-logueado")]
public async Task<IActionResult> ObtenerUsuarioLogueado()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
        return Unauthorized("Usuario no identificado.");

    var user = await _userManager.FindByIdAsync(userId);

    if (user == null)
        return NotFound("Usuario no encontrado.");

    return Ok(new
    {
        email = user.Email,
        nombreCompleto = user.NombreCompleto
    });
}

[HttpPut("editar-usuario")]
public async Task<IActionResult> EditarUsuario([FromBody] EditarUsuarioModel model)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var usuario = await _userManager.FindByIdAsync(userId);

    if (usuario == null)
        return NotFound("Usuario no encontrado");

    // Actualiza el nombre completo
    usuario.NombreCompleto = model.NombreCompleto;

    // Si se quiere cambiar la contraseña
    if (!string.IsNullOrEmpty(model.PasswordActual) && !string.IsNullOrEmpty(model.PasswordNueva))
    {
        // Validar que las contraseñas nuevas coincidan
        if (model.PasswordNueva != model.RepetirPassword)
        {
            return BadRequest("La nueva contraseña y su confirmación no coinciden");
        }

        // Verificar la contraseña actual
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(usuario, model.PasswordActual);
        if (!isCurrentPasswordValid)
        {
            return BadRequest("La contraseña actual es incorrecta");
        }

        // Cambiar la contraseña
        var changePasswordResult = await _userManager.ChangePasswordAsync(usuario, model.PasswordActual, model.PasswordNueva);
        if (!changePasswordResult.Succeeded)
        {
            return BadRequest(changePasswordResult.Errors);
        }
    }
    else
    {
        // Solo actualizar el usuario sin cambiar contraseña
        var updateResult = await _userManager.UpdateAsync(usuario);
        if (!updateResult.Succeeded)
            return BadRequest(updateResult.Errors);
    }

    return Ok("Usuario actualizado correctamente");
}
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        //BUSCAMOS EL USUARIO POR MEDIO DE EMAIL EN LA BASE DE DATOS
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            //SI EL USUARIO ES ENCONTRADO Y LA CONTRASEÑA ES CORRECTA
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "ADMIN"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //RECUPERAMOS LA KEY SETEADA EN EL APPSETTING
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //ARMAMOS EL OBJETO CON LOS ATRIBUTOS PARA GENERAR EL TOKEN
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // GENERAMOS EL REFRESH TOKEN
            var refreshToken = GenerarRefreshToken();
            //GUARDAMOS EN BASE DE DATOS EL REFRESH TOKEN
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);

            return Ok(new
            {
                token = jwt,
                refreshToken = refreshToken
            });
        }

        return Unauthorized("Credenciales inválidas");
    }

    private string GenerarRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
    {
        //BUSCAMOS EL USUARIO POR EMAIL EN BASE DE DATOS
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized();

        //BUSCAMOS EL TOKENREFRESH GUARDADO
        var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");

        //COMPARAMOS EL REFRESH TOKEN DE BD CON EL GUARDADO EN EL DISPOSITIVO DEL USUARIO PARA UNA MAYOR SEGURIDAD
        if (savedToken != model.RefreshToken)
            return Unauthorized("Refresh token inválido");

        //GENERAMOS EL NUEVO TOKEN DE ACCESO PRINCIPAL
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var newToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(newToken);

        //GENERAMOS UN NUEVO REFRESH TOCKEN
        var newRefreshToken = GenerarRefreshToken();
        //VOLVEMOS A GUARDAR ESE REGISTRO
        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);

        return Ok(new
        {
            token = jwt,
            refreshToken = newRefreshToken
        });
    }

}