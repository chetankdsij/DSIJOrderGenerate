using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YourCompany.Modules.DSJUserSubscription;

namespace DSIJOrderGenerate
{
    public class EmailSend
    {
        public string SendEmail(string Email, string MessageBody, string Subject, DataTable dt, Stream attachment,string AttachmentName)
        {
            try
            {
                writeprocessentry("SendEmail start");
                string mailstatus = "";
                string recipients = Email;
                EmailController controllerObj = new EmailController();
                EmailManagementInfo objCRRManagementInfo = new EmailManagementInfo();
                DataSet dsSMTP = new DataSet();
                string body = string.Empty;

                objCRRManagementInfo = GetManagementInfoObj(dt);
                if (objCRRManagementInfo == null)
                {
                    return false.ToString();
                }
                dsSMTP =new DSJUserSubscriptionController().GetSmtpServer(int.Parse(objCRRManagementInfo.smtpId));
                string recommender = string.Empty;
                body = HttpUtility.HtmlDecode(MessageBody);
                try
                {
                    var mail = new System.Net.Mail.MailAddress(recipients);
                    if (!string.IsNullOrEmpty(recipients.Trim()))
                    {
                        mailstatus = EmailController.SendMail(objCRRManagementInfo.mailfrom, recipients, objCRRManagementInfo.cc, objCRRManagementInfo.bcc, EmailController.MailPriority.Normal, Subject, EmailController.MailFormat.Html, System.Text.Encoding.UTF8, HttpUtility.HtmlDecode(body.ToString()), "",
                        dsSMTP.Tables[0].Rows[0]["SmtpIp"].ToString(), dsSMTP.Tables[0].Rows[0]["smtpAuthentication"].ToString(), dsSMTP.Tables[0].Rows[0]["SmtpUser"].ToString(), dsSMTP.Tables[0].Rows[0]["SmtpPassword"].ToString(), bool.Parse(dsSMTP.Tables[0].Rows[0]["EnableSSL"].ToString()), attachment,  AttachmentName);
                        writeprocessentry("mailstatus : " + mailstatus + "  , Email : " + recipients);
                    }
                }
                catch (Exception ex)
                {
                    writeprocessentry("SendEmail exception " + ex.Message);
                    return "False";
                }
                writeprocessentry("SendEmail status " + mailstatus);
                return mailstatus;
            }
            catch (Exception ex)
            {
                writeprocessentry("SendEmail exception " + ex.Message);
                return "False";
            }
        }
        public static EmailManagementInfo GetManagementInfoObj(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            EmailManagementInfo emailInfo = new EmailManagementInfo();
            emailInfo.attachmentpath = dt.Rows[0]["attachmentpath"].ToString();
            emailInfo.bcc = dt.Rows[0]["bcc"].ToString();
            emailInfo.body = dt.Rows[0]["body"].ToString();
            emailInfo.CategoryId = int.Parse(dt.Rows[0]["CategoryID"].ToString());
            emailInfo.cc = dt.Rows[0]["cc"].ToString();
            emailInfo.CreatedByUser = int.Parse(dt.Rows[0]["createdby"].ToString());
            emailInfo.mailfrom = dt.Rows[0]["mailfrom"].ToString();
            emailInfo.MailId = int.Parse(dt.Rows[0]["mailid"].ToString());
            emailInfo.mailto = dt.Rows[0]["mailto"].ToString();
            emailInfo.mFormat = (EmailController.MailFormat)Enum.Parse(typeof(EmailController.MailFormat), dt.Rows[0]["mformat"].ToString());
            emailInfo.priority = (EmailController.MailPriority)Enum.Parse(typeof(EmailController.MailPriority), dt.Rows[0]["priority"].ToString());
            emailInfo.smtpId = dt.Rows[0]["smtpid"].ToString();
            emailInfo.subject = dt.Rows[0]["subject"].ToString();
            return emailInfo;
        }

        public void writeprocessentry(string message)
        {
            try
            {
                string filePath = "C:\\logs\\PayUBizResponse\\";
                string filename = String.Format("{0:yyyy-MM-dd}.txt", DateTime.Now);
                string path = Path.Combine(filePath, filename);
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("");
                    }
                }
                message = message + " " + DateTime.Now.ToLongTimeString();
                using (FileStream fs = new FileStream(filePath + filename, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("\n");
                    sw.WriteLine(message);
                }
            }
            catch (Exception En)
            {
            }
        }
    }
    public class EmailController
    {
        public enum MailFormat
        {
            Text,
            Html
        }

        public enum MailPriority
        {
            Normal,
            Low,
            High
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body, object Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL, Stream attachment, string AttachmentName)
        {
            string functionReturnValue = null;
            SqlDataProvider obj = new SqlDataProvider();
            //string fromname = obj.getFromName(MailFrom);
            string fromname = "";


            // translate semi-colon delimiters to commas as ASP.NET 2.0 does not support semi-colons
            MailTo = MailTo.Replace(";", ",");
            Cc = Cc.Replace(";", ",");
            Bcc = Bcc.Replace(";", ",");

            System.Net.Mail.MailMessage objMail = new System.Net.Mail.MailMessage();
            objMail.From = new System.Net.Mail.MailAddress(MailFrom, fromname);

            if (!string.IsNullOrEmpty(MailTo))
            {
                objMail.To.Add(MailTo);
            }
            if (!string.IsNullOrEmpty(Cc))
            {
                objMail.CC.Add(Cc);
            }
            if (!string.IsNullOrEmpty(Bcc))
            {
                objMail.Bcc.Add(Bcc);
            }
            objMail.Priority = (System.Net.Mail.MailPriority)Priority;
            objMail.IsBodyHtml = Convert.ToBoolean((BodyFormat == MailFormat.Html ? true : false));
            string objtype = Attachment.GetType().FullName;
            if (objtype == "System.Collections.ArrayList")
            {
                foreach (object product in ((ArrayList)Attachment).ToArray())
                {
                    objMail.Attachments.Add(new System.Net.Mail.Attachment(product.ToString()));
                }
            }
            else if ((!string.IsNullOrEmpty(Convert.ToString(attachment))))
            {
                objMail.Attachments.Add(new System.Net.Mail.Attachment(attachment,AttachmentName));
            }

            // message
            objMail.SubjectEncoding = BodyEncoding;
            objMail.Subject = Subject;
            objMail.BodyEncoding = BodyEncoding;
            objMail.Body = Body;

            // external SMTP server alternate port
            int SmtpPort = 0;
            int portPos = SMTPServer.IndexOf(":");
            if (portPos > -1)
            {
                SmtpPort = Int32.Parse(SMTPServer.Substring(portPos + 1, SMTPServer.Length - portPos - 1));
                SMTPServer = SMTPServer.Substring(0, portPos);
            }

            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();

            if (!string.IsNullOrEmpty(SMTPServer))
            {
                smtpClient.Host = SMTPServer;
                if (SmtpPort > 0)
                {
                    smtpClient.Port = SmtpPort;
                }
                switch (SMTPAuthentication)
                {
                    case "":
                    case "0":
                        // anonymous
                        if (!string.IsNullOrEmpty(SMTPUsername) & !string.IsNullOrEmpty(SMTPPassword))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                        }
                        break;
                    case "1":
                        // basic
                        if (!string.IsNullOrEmpty(SMTPUsername) & !string.IsNullOrEmpty(SMTPPassword))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                        }
                        break;
                    case "2":
                        // NTLM
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                        break;
                }
            }
            smtpClient.EnableSsl = SMTPEnableSSL;

            try
            {
                smtpClient.Send(objMail);
                functionReturnValue = "True";
                objMail.To.RemoveAt(0);
            }
            catch (Exception objException)
            {
                // mail configuration problem
                if ((objException.InnerException != null))
                {
                    functionReturnValue = string.Concat(objException.Message, "\r\n", objException.InnerException.Message);
                    //LogException(objException.InnerException);
                }
                else
                {
                    functionReturnValue = objException.Message;
                    //LogException(objException);
                }
            }
            return functionReturnValue;

        }
    }
    public class EmailManagementInfo
    {
        public string attachmentpath { get; set; }
        public string bcc { get; set; }
        public string body { get; set; }
        public Encoding bodyEncoading { get; set; }
        public string bodytype { get; set; }
        public int CategoryId { get; set; }
        public string cc { get; set; }
        public int CreatedByUser { get; set; }
        public string mailfrom { get; set; }
        public int MailId { get; set; }
        public string mailto { get; set; }
        public EmailController.MailFormat mFormat { get; set; }
        public EmailController.MailPriority priority { get; set; }
        public string smtpId { get; set; }
        public string subject { get; set; }
        public string tagname { get; set; }
    }

}
