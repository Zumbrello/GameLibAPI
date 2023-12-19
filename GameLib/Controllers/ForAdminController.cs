using System.IdentityModel.Tokens.Jwt;
using GameLib.Context;
using GameLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLib.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ForAdminController : ControllerBase
{
    private GtcymkznContext Context = new GtcymkznContext(); //подключение к бд

    public ForAdminController() //конструктор контроллера
    {}
    //создание списка пользователей
    private List<User> GetUsers() //подключение к базе данных
    {
            return Context.Users.ToList();
    }
    //вывод всех пользователей
    [HttpGet("getusers")]
    public ActionResult GetDataApi()
    {
        if (!IsAdmin())
        {
            return BadRequest("Доступно только администраторам");
        }
        var usersData = GetUsers(); //вывод данных в api
        return Ok(usersData);
    }
    //удаление пользователя
    [HttpDelete("deliteuser")]
    public async Task<IActionResult> DeleteUser(int id_user)
    {
        if (!IsAdmin())
        {
            return BadRequest("Доступно только администраторам");
        }
        var user = await Context.Users.FindAsync(id_user); //находим пользователя по id

        if (user == null)
        {
            return NotFound("User not found"); // Если пользователь с указанным id не найден, возвращаем 404 Not Found
        }

        
        if (user.UserRole == 1)
        {
            return BadRequest("Ты не можешь удалать других администраторов!");
        }

        Context.Users.Remove(user); //удаляем пользователя
        await Context.SaveChangesAsync(); //сохраняем

        return Ok("User delited"); 
    }
    
    //удаление игры
    [HttpDelete("delitegame")]
    public async Task<IActionResult> DeliteGame(int id_game)
    {
        if (!IsAdmin())
        {
            return BadRequest("Доступно только администраторам");
        }
        var game = await Context.Games.FindAsync(id_game); //находим игру по id
        if (game == null)
        {
            return NotFound("Игра не найдена");
        }

        Context.Games.Remove(game);
        await Context.SaveChangesAsync();
        return Ok("Игра удалена");
    }
    //Добавление игры
    [HttpPost("addgame")]
    [Authorize]
    public async Task<ActionResult> AddGame([FromBody] Gameadd newGameadd)
    {
        if (!IsAdmin())
        {
            return BadRequest("Доступно только администраторам");
        }
        int idUser = GetUserIdFromToken();

        User user = Context.Users.Where(u => u.IdUser == idUser).FirstOrDefault();
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }
        bool isAdmin = user.UserRole == 1;

        if (isAdmin)
        {
            
        }
        else
        {
            
        }
        try
        {
            // Создаем новый объект игры
            var newGame = new Game
            {
                GameName = newGameadd.Name,
                Description = newGameadd.Description,
                IdDeveloper = newGameadd.IdDeveloper,
                IdPublisher = newGameadd.IdPublisher,
                SystemRequestMin = newGameadd.SystemRequestMin,
                SystemRequestRec = newGameadd.SystemRequestRec,
                ReleaseDate = newGameadd.ReleaseDate,
                MainImage = newGameadd.MainImage
                
            };

            // Добавляем новую игру в базу данных
            Context.Games.Add(newGame);
            await Context.SaveChangesAsync();

            return Ok("Game added successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class Gameadd
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int IdDeveloper { get; set; }
        public int IdPublisher { get; set; }
        public string SystemRequestMin { get; set; }
        public string SystemRequestRec { get; set; }
        public string ReleaseDate { get; set; }
        public string MainImage { get; set; }
    }
    [HttpPost("editgame")]
    [Authorize]
    
    public async Task<ActionResult> EditGame([FromBody] Gameedit editGame)
    {
        if (!IsAdmin())
        {
            return BadRequest("Доступно только администраторам");
        }
        var game = Context.Games.Where(g => g.IdGame == editGame.IdGame).FirstOrDefault();
        
        //Проверяем, что игра существует
        if (game == null)
        {
            return NotFound("Игра не найдена");
        }

        try
        {
            //Изменяем данны об игре
            //game.GameGenres = editGame.GameGenres;
            game.Description = editGame.Description;
            game.IdPublisher = editGame.IdPublisher;
            game.IdDeveloper = editGame.IdDeveloper;
            //game.Favorites = editGame.Favorites;
            game.GameName = editGame.GameName;
            game.MainImage = editGame.MainImage;
            game.ReleaseDate = editGame.ReleaseDate;
            game.SystemRequestMin = editGame.SystemRequestMin;
            game.SystemRequestRec = editGame.SystemRequestRec;
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка при выполнении запроса");
        }

        //Удали, если без этого данные обновятся, иначе раскомментируй
        //Context.Games.Update(game);
        await Context.SaveChangesAsync();

        return Ok("Game edited successfully");
        
    }
    
    public class Gameedit
    {
        public int IdGame { get; set; }

        public string? GameName { get; set; }

        public int? IdDeveloper { get; set; }

        public int? IdPublisher { get; set; }

        public string? Description { get; set; }

        public string? SystemRequestMin { get; set; }

        public string? SystemRequestRec { get; set; }

        public string? ReleaseDate { get; set; }

        public string? MainImage { get; set; }
       // public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

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
    private bool IsAdmin()
    {
        int idUser = GetUserIdFromToken();
        User user = Context.Users.Where(u => u.IdUser == idUser).FirstOrDefault();
        if (user == null)
        {
            return false;
        }
        return user.UserRole == 1;
    }
}