using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using n8n.Models;

namespace n8n.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ManageAiGetMsgController : Controller
    {
        private readonly AppDbContext _db;
        public ManageAiGetMsgController(AppDbContext db)
        {
            _db = db;
        }
        //由ai 来获取用户+管理员的消息记录, 只获取最近一天的
        [HttpGet("{telegramid}")]
        public async Task<IActionResult> GetHistory(string telegramid)
        {
            var logs = await _db.AiMessageLogs.Where(x => x.TelegramID == telegramid && (x.ResponseTime > DateTime.Now.AddDays(-1)))
                .OrderByDescending(x => x.ResponseTime)
                .Select(x => new
                {
                    x.ResponseTime,
                    x.ResponseMessage,
                    x.Rules
                }).ToListAsync();
            return Ok(logs);
        }
        public class CreateUserRulesRequest
        {
            public string TelegramID { get; set; }
            public string ResponseMessage { get; set; }
        }
        // 在暂停ai chat, 获取用户信息
        [HttpPost]
        public async Task<IActionResult> CreateUserRules([FromBody] CreateUserRulesRequest request)
        {
            _db.AiMessageLogs.Add(new AiMessageLog
            {
                TelegramID = request.TelegramID,
                Rules = "Customer",
                ResponseTime = DateTime.Now,
                ResponseMessage = request.ResponseMessage
            });
            await _db.SaveChangesAsync();
            return Ok();
        }
        //自动获取admin信息
        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserRulesRequest request)
        {
            _db.AiMessageLogs.Add(new AiMessageLog
            {
                TelegramID = request.TelegramID,
                Rules = "Admin",
                ResponseTime = DateTime.Now,
                ResponseMessage = request.ResponseMessage
            });
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
