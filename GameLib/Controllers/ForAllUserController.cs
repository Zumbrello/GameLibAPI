using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using GameLib.Context;
using GameLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameLib.Controllers;
[ApiController]
[Route("api/[controller]")]

public class ForAllUserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly GtcymkznContext _context;
    private int TokenTimeMinute = 5;
    private DateTime _tokenCreationTime;

    public ForAllUserController(GtcymkznContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    
    //создание списка игр
    private static IEnumerable<Game> GetGame()
    {
        using (var context = new GtcymkznContext())
        {
            return context.Games.ToList();
        }
    }
    //запрос на вывод всех игр
    [HttpGet("getgame")]
    [Authorize]
    public ActionResult GetDataApi()
    {
        var gameData = GetGame();
        return Ok(gameData);
    }
    
    //создание списка издателей
    private static IEnumerable<Publisher> GetPublishers()
    {
        using (var context = new GtcymkznContext())
        {
            return context.Publishers.ToList();
        }
    }
    //запрос на вывод всех издателей
    [HttpGet("getpublisher")]
    [Authorize]
    public ActionResult GetDataPublishers()
    {
        var gamePublishersData = GetPublishers();
        return Ok(gamePublishersData);
    }
    
    //создние списка жанров
    private static IEnumerable<Genere> GetGeners() //подключение к базе данных
    {
        using (var context = new GtcymkznContext())
        {
            return context.Generes.ToList();
        }
    }
    //вывод жанров
    [HttpGet("getgeners")]
    [Authorize]
    public ActionResult GetData()
    {
        var genersData = GetGeners(); //вывод данных в api
        return Ok(genersData);
    }
    
    //добавление игры в избранное
    [HttpPost("addgameforfavorite")]
    [Authorize]
    public IActionResult AddBookForFavorite(int idGame)
    {

        int idUser = GetUserIdFromToken(); //получаем id пользователя из токена
        Favorite favoriteModel = new Favorite //экземпляр класса избранного
        {
            IdUser = idUser,
            IdGame = idGame
        };

        _context.Favorites.Add(favoriteModel); //добавляем 
        _context.SaveChanges(); //сохраняем

        return Ok("Game add to favorite.");
    }
    
    [HttpGet("Favorite")]
    [Authorize]
    public OkObjectResult GetFavoriteBooksByUserId()
    {
        int userId = GetUserIdFromToken();
        var favoriteBooks = _context.Favorites
            .Where(f => f.IdUser == userId) // Фильтрация по IdUser
            .Include(f => f.IdGameNavigation) // Предзагрузка связанной сущности Game
            .Select(f => new
            {
                IdGame = f.IdGameNavigation.IdGame,
                Name = f.IdGameNavigation.GameName,
                Publisher = f.IdGameNavigation.IdPublisherNavigation.Publisher1,
                Developer = f.IdGameNavigation.IdDeveloperNavigation.Developer1
            })
            .ToList();

        return Ok(favoriteBooks);
    }
    //запрос на информацию о конкретной игре
    [HttpGet("getinformationaboutgame")]
    [Authorize]
    public async Task<ActionResult<ModelGameInformation>> GetInformationAboutGame(int gameId)
    {
        int userId = GetUserIdFromToken(); //из токена получаем id пользователя
        
        //запрос к бд для нахождения книги соотвествующей введенному id
        var favoriteGame = await _context.GameGenres
            .Include(f => f.IdGameNavigation)
            .ThenInclude(game => game.IdPublisherNavigation)
            .Include(f =>f.IdGameNavigation)
            .ThenInclude(game => game.IdDeveloperNavigation)
            .Include(f => f.IdGameNavigation)
            .ThenInclude(game => game.GameGenres)
            .ThenInclude(gg => gg.IdGenreNavigation)
            .FirstOrDefaultAsync(f => f.IdGame == gameId);

        if (favoriteGame == null)
        {
            return NotFound("Game not found"); // если игра не найдена в избранном пользователя, возвращаем 404 Not Found
        }
        
        var gameInformation = new ModelGameInformation()
        {
            IdGame = favoriteGame.IdGameNavigation.IdGame,
            Name = favoriteGame.IdGameNavigation.GameName,
            Publisher = favoriteGame.IdGameNavigation.IdPublisherNavigation.Publisher1,
            Developer = favoriteGame.IdGameNavigation.IdDeveloperNavigation.Developer1,
            Geners = favoriteGame.IdGameNavigation.GameGenres.Select(gg => gg.IdGenreNavigation.Gener).ToArray(),
            Description = favoriteGame.IdGameNavigation.Description,
            SystemRequestMin = favoriteGame.IdGameNavigation.SystemRequestMin,
            SystemRequestRec = favoriteGame.IdGameNavigation.SystemRequestRec,
            ReleaseDate = favoriteGame.IdGameNavigation.ReleaseDate,
            MainImage = favoriteGame.IdGameNavigation.MainImage
        };

        return Ok(gameInformation); // возвращаем информацию о игре
    }
    //запрос на изменения пароля
    [HttpPut("changepassword")]
    public IActionResult ChangePassword(string password)
    {

        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = _context.Users.FirstOrDefault(u => u.IdUser == id);
        if (user == null)
        {
            return Unauthorized(); // Пользователь не найден
        }
        
        // Шифрование пароля
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        user.Password = hashedPassword;
        _context.SaveChanges(); //сохранения нового пароля
        return Ok("Password changed");
    }
    
    //изменения email
    [HttpPut("changeemail")]
    public IActionResult ChangeEmail(string email)
    {
        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = _context.Users.FirstOrDefault(u => u.IdUser == id);
        if (user == null)
        {
            return Unauthorized(); // Пользователь не найден
        }

        user.Email = email;

        _context.SaveChanges(); //сохранение нового мыла

        return Ok("Email changed");
    }
    
    //изменение логина
    [HttpPut("changelogin")]
    public IActionResult ChangeLogin(string login)
    {
        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = _context.Users.FirstOrDefault(u => u.IdUser == id);
        if (user == null)
        {
            return Unauthorized(); // Пользователь не найден
        }
        
        // Проверяем, что новый логин не совпадает ни с одним из существующих логинов
        var existingUser = _context.Users.FirstOrDefault(u => u.Login == login);
        if (existingUser != null)
        {
            return BadRequest("Login already exists"); // Логин уже существует
        }

        user.Login = login;
        _context.SaveChanges(); //сохранение нового логина
        
        Console.WriteLine(id);
        return Ok("Login changed");
        
    }

    //удаляем игру из избранного
    [HttpDelete("removegamefromfavorites")]
    public async Task<IActionResult> DeleteBookFromFavarite(int idGame)
    {
        var game =  _context.Favorites.FirstOrDefault(b => b.IdGame == idGame); //находим игру по id

        if (game == null)
        {
            return NotFound("Book not found"); // Если пользователь с указанным id не найден, возвращаем 404 Not Found
        }

        _context.Favorites.Remove(game); //удаляем
        await _context.SaveChangesAsync(); //сохраняем

        return Ok("Game delited"); 
    }

    //авторизация
    [HttpPost("login")]
    public IActionResult Authenticate(string login, string password, bool isDesktop)
    {
        var user = _context.Users.FirstOrDefault(u => u.Login == login);
        if (user == null)
        {
            return Unauthorized();
        }
        
        if (isDesktop)
        {
            // Проводим проверку на администратора только для запросов с ПК
            if (user.UserRole != 1)
            {
                return BadRequest();
            }
        }
        
        var loginResponse = new LoginResponse();
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (isPasswordValid)
        {
            string token = CreateToken(user.IdUser);

            loginResponse.Token = token;
            loginResponse.ResponseMsg = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };

            // Возвращаем токен
            return Ok(new { loginResponse });
            
        }
        else
        {
            // Если имя пользователя или пароль недействительны, отправляем статус-код "BadRequest" в ответе
            return BadRequest("Username or Password Invalid!");
        }
    }
    
    //регистрация
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterUser(string login, string email, string password)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Проверяем, существует ли пользователь с таким же именем пользователя или email'ом
        if (await _context.Users.AnyAsync(u => u.Login == login || u.Email == email))
        {
            return Conflict("A user with the same username or email address already exists");
        }
            
        // Шифрование пароля
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        // Создаем нового пользователя
        var user = new User
        {
            Login = login,
            Email = email,
            Password = hashedPassword, 
            UserRole = 2
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User successfully registered");
    }
    
    
    //все для токенов
    private string CreateToken(int userId)
    {
        var claims = new List<Claim>()
        {
            // Список претензий (claims) - мы проверяем только id пользователя, можно добавить больше претензий.
            new Claim("userId", Convert.ToString(userId)),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(TokenTimeMinute),
            signingCredentials: cred
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
    
    //получение id пользователя из токена
    private int GetUserIdFromToken()
    {
        var token = GetTokenFromAuthorizationHeader(); //получаем токен
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //полчение срока действия токена
        var now = DateTime.UtcNow;
        if (jwtToken.ValidTo < now)
        {
            // Токен истек, выполните необходимые действия, например, вызовите исключение
            throw new Exception("Expired token.");
        }
        // Извлечение идентификатора пользователя из полезной нагрузки токена
        var userId = int.Parse(jwtToken.Claims.First(c => c.Type == "userId").Value);

        return userId;
    }

    //получение токена из запроса
    private string GetTokenFromAuthorizationHeader()
    {
        var autorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (autorizationHeader != null && autorizationHeader.StartsWith("Bearer "))
        {
            var token = autorizationHeader.Substring("Bearer ".Length).Trim();
            return token;
        }

        return null;
    }
    private bool IsRequestFromDesktop()
    {
        // Пример анализа заголовка User-Agent для определения типа устройства
        string userAgent = Request.Headers["User-Agent"].ToString();
    
        // Предположим, что запрос с ПК будет содержать "Windows" в User-Agent
        return userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase);
    }
  
    
}