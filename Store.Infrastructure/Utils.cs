using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    [SuppressMessage("ReSharper","InconsisentNaming")]
    public class Utils
    {

        #region Private Fields


        /// <summary>
        /// 向指定的邮件地址发送邮件
        /// </summary>
        /// <param name="to">需要发送邮件的邮件地址。</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容</param>
        public static void SendEmail(string to, string subject, string content)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(to));
            msg.Subject = subject;
            msg.Body = content;

            var smtpClient = new SmtpClient();
            smtpClient.Send(msg);
        }
        #endregion
    }
}
