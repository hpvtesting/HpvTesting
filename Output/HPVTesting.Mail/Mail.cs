using HPVTesting.Business.Helpers;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HPVTesting.Mail
{
    public class Mail
    {
        private static readonly IRazorEngineService RazorEngine;

        static Mail()
        {
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedTemplateManager(typeof(Mail).Namespace + ".Templates"),
                Namespaces = { "HPVTesting.Mail", "HPVTesting.Mail.Models" },
                CachingProvider = new DefaultCachingProvider()
            };
            RazorEngine = RazorEngineService.Create(config);
        }

        public Mail(string templateName)
        {
            TemplateName = templateName;
            ViewBag = new DynamicViewBag();
        }

        public string TemplateName { get; set; }

        public object Model { get; set; }

        public DynamicViewBag ViewBag { get; set; }

        public string GenerateBody()
        {
            var layout = RazorEngine.RunCompile("_Layout");
            var body = RazorEngine.RunCompile(TemplateName, Model.GetType(), Model);
            return layout.Replace("{{BODY}}", body);
        }

        public MailMessage Send(string to, string subject, string cc = null)
        {
            MailSettings appSettings = new MailSettings();
            var email = new MailMessage
            {
                From = new MailAddress(appSettings.MailFrom),
                Body = GenerateBody(),
                IsBodyHtml = true,
                Subject = subject,
                BodyEncoding = Encoding.UTF8
            };

            if (!appSettings.EnableMail)
            {
                return email;
            }

            email.Headers.Add("X-MC-Metadata", "{ \"key\": \"" + Guid.NewGuid().ToString("N") + "\" }");

            try
            {
                foreach (var sendTo in to.Split(' ', ',', ';'))
                {
                    email.To.Add(sendTo);
                }

                if (cc != null)
                {
                    foreach (var sendCc in cc.Split(' ', ',', ';'))
                    {
                        email.CC.Add(sendCc);
                    }
                }

                foreach (var sendTo in to.Split(' ', ',', ';'))
                {
                    if (sendTo != "" && CommonHelper.IsValidEmail(sendTo))
                    {
                        email.To.Add(sendTo);
                    }
                }

                if (cc != null)
                {
                    foreach (var sendCc in cc.Split(' ', ',', ';'))
                    {
                        if (sendCc != "" && CommonHelper.IsValidEmail(sendCc))
                        {
                            email.CC.Add(sendCc);
                        }
                    }
                }

                var smtp = new SmtpClient(appSettings.MailHost)
                {
                    Port = Convert.ToInt16(appSettings.MailPort),
                    Credentials = new NetworkCredential
                    {
                        UserName = appSettings.MailFrom,
                        Password = appSettings.MailPassword
                    }
                };
                smtp.EnableSsl = true;
                smtp.Send(email);
            }
            catch (Exception e)
            {
                return null;
            }
            return email;
        }
    }

    public class Mail<TModel> : Mail where TModel : class
    {
        public Mail(string templateName, TModel mailModel) : base(templateName)
        {
            Model = mailModel;
        }
    }
}