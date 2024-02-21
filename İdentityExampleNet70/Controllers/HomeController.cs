using İdentityExampleNet70.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Diagnostics;

namespace İdentityExampleNet70.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {


        private readonly ILogger<HomeController> _logger;
        private readonly SmtpSettings _smtpSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<SmtpSettings> smtpSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginCheck()
        {
            return View();
        }


        [HttpGet]

        public IActionResult SendEmail()
        {
            return View();
        }

        //EmailSend

        [HttpPost]
        public IActionResult SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Connect(_smtpSettings.SmtpServer, _smtpSettings.Port, _smtpSettings.UseSsl);
            smtpClient.Authenticate(_smtpSettings.Username, _smtpSettings.Password);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpSettings.DisplayName, _smtpSettings.Username));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = body
            };

            smtpClient.Send(emailMessage);
            smtpClient.Disconnect(true);

            return RedirectToAction("Index");
        }




















        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}