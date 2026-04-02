using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using n8n.Models;

namespace n8n.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ManageAiStatusController : Controller
    {
        private readonly AppDbContext _db;
        public ManageAiStatusController(AppDbContext db)
        {
            _db = db;
        }
        // 管理员手动更新状态, 主要是为了在用户投诉后, 可以暂停用户的ai chat功能
        [HttpPost("{telegramid}")]
        public async Task<IActionResult> UpdateStatusManual(string telegramid)
        {
            var user = await _db.AgentStatuses.FirstOrDefaultAsync(x => x.TelegramID == telegramid);
            if (user == null)
            {
                _db.AgentStatuses.Add(new AgentStatus
                {
                    TelegramID = telegramid,
                    IsActive = false
                });
            }
            else
            {
                user.IsActive = false;
                _db.AgentStatuses.Update(user);
            }
            await _db.SaveChangesAsync();
            return Ok();
        }
        // 用户自动恢复状态, 主要是为了在用户被暂停后, 可以通过某些条件自动恢复用户的ai chat功能
        [HttpPost("{telegramid}")]
        public async Task<IActionResult> UpdateStatusAuto(string telegramid)
        {
            var user = await _db.AgentStatuses.FirstOrDefaultAsync(x => x.TelegramID == telegramid);
            if (user == null)
            {
                _db.AgentStatuses.Add(new AgentStatus
                {
                    TelegramID = telegramid,
                    IsActive = true
                });
            }
            else
            {
                user.IsActive = true;
                _db.AgentStatuses.Update(user);
            }
            await _db.SaveChangesAsync();
            return Ok();
        }
        //检查ai chat状态
        [HttpGet("{telegramid}")]
        public async Task<IActionResult> CheckStatus(string telegramid)
        {
            var user = await _db.AgentStatuses.FirstOrDefaultAsync(x => x.TelegramID == telegramid);
            if (user == null)
            {
                return Ok(new { IsActive = true });
            }
            else
            {
                return Ok(new { user.IsActive });
            }
        }
    }
}
