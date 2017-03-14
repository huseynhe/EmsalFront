using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
    public static class Mail
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMail(MailMessage msg)
        {
            if(CheckForInternetConnection())
            {
                    msg.From = new MailAddress("tedaruk@agro.gov.az", "tedaruk.az");
                    string fromPassword = "123456";
                    //string fromPassword = "e3WQ@Y2d9&r!";

                    SmtpClient smtp = new SmtpClient();
                    //smtp.Host = "smtp.mail.gov.az";
                    smtp.Host = "85.132.122.243";
                    //smtp.Port = 587;
                    smtp.Port = 465;
                    smtp.EnableSsl = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
                    smtp.Timeout = 20000;
                    smtp.Send(msg);

                    return true;
            }
            else
            {
                return false;
            }
        }
    }
}
