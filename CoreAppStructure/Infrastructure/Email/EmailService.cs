﻿using System.Net.Mail;
using System.Net;
using RazorEngine;
using RazorEngine.Templating;

namespace CoreAppStructure.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailModel _emailConfig;

        public EmailService(EmailModel emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailConfig.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            using var smtpClient = new SmtpClient(_emailConfig.SmtpServer)
            {
                Port = _emailConfig.Port,
                Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailWithTemplateAsync(string to, string subject, string templatePath, object model)
        {
            string templateContent = File.ReadAllText(templatePath);

            string body = Engine.Razor.RunCompile(templateContent, templatePath, null, model);

            await SendEmailAsync(to, subject, body);
        }
    }
}
