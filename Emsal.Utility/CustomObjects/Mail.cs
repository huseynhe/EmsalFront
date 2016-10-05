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
            //string fromPassword = "e1701895";

            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = "smtp.gmail.com";
            //smtp.Port = 587;
            //smtp.EnableSsl = true;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
            //smtp.Timeout = 20000;
            //smtp.Send(msg);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
