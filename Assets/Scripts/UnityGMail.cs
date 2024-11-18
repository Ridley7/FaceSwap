/*
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;



public class UnityGMail
{
    public void SendMailFromGoogle()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("metadridley7@gmail.com");
        mail.To.Add("metadridley7@gmail.com");
        mail.Subject = "Plantilla FaceSwap";
        mail.Body = "Message from unity";

        //Anexamos alguna foto
        //var attachment = new Attachment("C:\\path\\file.ext");
        var attachment = new Attachment("/Users/blackdata/Desktop/Repositorios/FaceSwap/Assets/Resources/API_Configuration/tribu_mesoamericana.jpg");
        ///Users/blackdata/Desktop/Repositorios/FaceSwap/Assets/Resources/API_Configuration
        mail.Attachments.Add(attachment);

        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
        smtp.Port = 587;

        // Account Username: Usually your "from" email.
        // App Password: If your account has a 2 step verification, you must follow these instructions 
        //  to generate the password: https://support.google.com/accounts/answer/185833?hl=en.
        //mama ynvh sqvc hldq
        smtp.Credentials = new NetworkCredential("metadridley7@gmail.com", "mama ynvh sqvc hldq") as ICredentialsByHost;
        smtp.EnableSsl = true;
        

        ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                    return true;
                };

        smtp.Send(mail);
    }
}
*/



using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class UnityGMail
{
    public void SendMailFromGoogle()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("metadridley7@gmail.com");
        mail.To.Add("metadridley7@gmail.com");
        mail.Subject = "Plantilla FaceSwap";

        // Leer la plantilla HTML desde un archivo
        string htmlBody = File.ReadAllText("/Users/blackdata/Desktop/Repositorios/FaceSwap/Assets/Resources/API_Configuration/plantilla.html");

        // Usar el HTML como cuerpo del correo
        mail.Body = htmlBody;
        mail.IsBodyHtml = true;  // Indicamos que el cuerpo es HTML

        // Anexar alguna foto si es necesario
        var attachment = new Attachment("/Users/blackdata/Desktop/Repositorios/FaceSwap/Assets/Resources/API_Configuration/tribu_mesoamericana.jpg");
        mail.Attachments.Add(attachment);

        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
        smtp.Port = 587;
        //Lo de mama ynvh sqvc hldq es un App Password, si tu cuenta tiene verificación en dos pasos, 
        //debes seguir estas instrucciones para generar la contraseña: https://support.google.com/accounts/answer/185833?hl=en.
        smtp.Credentials = new NetworkCredential("metadridley7@gmail.com", "mama ynvh sqvc hldq") as ICredentialsByHost;
        smtp.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                return true;
            };

        smtp.Send(mail);
    }
}

