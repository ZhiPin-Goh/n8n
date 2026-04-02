using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using n8n.Models;

namespace n8n.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ManageAppointmentController : Controller
    {
        private readonly AppDbContext _db;
        public ManageAppointmentController(AppDbContext db)
        {
            _db = db;
        }
        // 检查售后服务, 检查昨天的预约而已
        [HttpGet]
        public async Task<IActionResult> CheckAfterSales()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var appointments = await _db.Appointments.Where(x => x.Date.Date == yesterday.Date).ToListAsync();
            List<object> result = new List<object>();
            foreach (var appointment in appointments)
            {
                result.Add(new
                {
                    appointment.CustomerName,
                    appointment.Email,
                    appointment.Services,
                    appointment.Date
                });
            }
            return Ok(result);
        }
        //检查明天预约
        [HttpGet]
        public async Task<IActionResult> CheckTomorrom()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var appointments = await _db.Appointments.Where(x => x.Date.Date == tomorrow.Date).ToListAsync();
            List<object> result = new List<object>();
            foreach (var appointment in appointments)
            {
                result.Add(new
                {
                    appointment.CustomerName,
                    appointment.Email,
                    appointment.Services,
                    appointment.Date
                });
            }
            return Ok(result);
        }
        //检查空位
        [HttpGet("{datetime}")]
        public async Task<IActionResult> CheckAvailable(string datetime)
        {
            var date = DateTime.Parse(datetime);
            var count = await _db.Appointments.CountAsync(x => x.Date.Date == date.Date);
            return Ok(new { AvailableSlots = Math.Max(0, 10 - count) });
        }
        //检查预约时间
        [HttpGet("{telegramid}")]
        public async Task<IActionResult> CheckUpcoming(string telegramid)
        {
            var upcoming = await _db.Appointments.Where(x => x.TelegramID == telegramid && x.Date > DateTime.Now)
                .OrderBy(x => x.Date)
                .Select(x => new
                {
                    x.CustomerName,
                    x.Email,
                    x.Services,
                    x.Date
                }).ToListAsync();
            return Ok(upcoming);
        }
        //检查历史预约
        [HttpGet("{telegramid}")]
        public async Task<IActionResult> CheckHistory(string telegramid)
        {
            var history = await _db.Appointments.Where(x => x.TelegramID == telegramid && x.Date <= DateTime.Now)
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    x.CustomerName,
                    x.Email,
                    x.Services,
                    x.Date
                }).Take(1)
                .ToListAsync();
            return Ok(history);
        }
        public class CreateAppointmentRequest
        {
            public string TelegramID { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public string Services { get; set; }
            public DateTime Date { get; set; }
            public string CalendarEventId { get; set; }
        }
        // 创建预约
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            _db.Appointments.Add(new Appointment
            {
                TelegramID = request.TelegramID,
                CustomerName = request.CustomerName,
                Email = request.Email,
                Services = request.Services,
                Date = request.Date,
                CalendarEventID = request.CalendarEventId
            });
            await _db.SaveChangesAsync();
            return Ok();
        }
        // 更改预约
        public class UpdateAppointmentRequest
        {
            public string TelegramID { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public string Services { get; set; }
            public DateTime Date { get; set; }
            public string CalendarEventId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentRequest request)
        {
            var appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.TelegramID == request.TelegramID && x.Date > DateTime.Now);
            if (appointment == null)
            {
                return NotFound();
            }
            appointment.CustomerName = request.CustomerName;
            appointment.Email = request.Email;
            appointment.Services = request.Services;
            appointment.Date = request.Date;
            appointment.CalendarEventID = request.CalendarEventId;
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return Ok();
        }
        // 取消预约
        [HttpPost("{telegramid}")]
        public async Task<IActionResult> CencelAppointment(string telegramid)
        {
            var appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.TelegramID == telegramid && x.Date > DateTime.Now);
            if (appointment == null)
            {
                return NotFound();
            }
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return Ok();
        }

    }
}
