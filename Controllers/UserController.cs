using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPITEST.Database;
using WebAPITEST.Models;

namespace WebAPITEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="user">使用者基本資料</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("電子郵件已存在");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// 查找用戶——用戶輸入搜索條件，列出符合條件的用戶記錄（需要分頁顯示），可選擇的條件如下：
        /// </summary>
        /// <param name="name">姓名（可模糊查詢，包含關鍵字的須列出）</param>
        /// <param name="minAge">年齡範圍</param>
        /// <param name="maxAge">年齡範圍</param>
        /// <param name="gender">性別</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string? name, int? minAge, int? maxAge, string? gender)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.Name.Contains(name));

            if (minAge.HasValue)
                query = query.Where(u => u.Age >= minAge);

            if (maxAge.HasValue)
                query = query.Where(u => u.Age <= maxAge);

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(u => u.Gender == gender);

            return Ok(await query.ToListAsync());
        }

        /// <summary>
        /// 回傳使用者基本資料
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// 統計各城市不同性別的用戶數量
        /// </summary>
        /// <param name="gender">性別</param>
        /// <returns></returns>
        [HttpGet("summary")]
        public async Task<IActionResult> summary(string? gender)
        {
            var summary = _context.Users.AsQueryable()
                .Where(x => x.Gender == gender)
                .GroupBy(u => new { u.City, u.Gender })
                .Select(g => new { g.Key.City, g.Key.Gender, Count = g.Count() });
            return Ok(await summary.ToListAsync());
        }
    }
}
