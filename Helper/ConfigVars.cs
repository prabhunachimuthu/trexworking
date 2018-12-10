using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public sealed class ConfigVars
    {
        public string AdminUserName = string.Empty;
        public string AdminPassword = string.Empty;
        public string apikey = string.Empty;
        public string secretkey = string.Empty;
        public string hangfireConnectionString = string.Empty;
        public string ConnectionString = string.Empty;
        public string url = string.Empty;
        public string copyrightmsg = string.Empty;
        public string version = string.Empty;
        public string clienttimeout = string.Empty;
        public string servertimeout = string.Empty;
        public string rollbarAccessToken = string.Empty;
        public string rollbarEnvironment = string.Empty;


        public string Host { set; get; }
        public int Port { set; get; }
        public string From { set; get; }
        public string DisplayName { set; get; }
        public string Password { set; get; }
        public bool EnabelSSL { set; get; }
        public string sendgridapi { get; set; }
        public int OpenTokAPIKey { set; get; }
        public string OpenTokAPISecretKey { set; get; }

        public TimeSlot slots = null;

        private ConfigVars()
        {
            Host = Environment.GetEnvironmentVariable("Host");
            Port = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_Port"));
            From = Environment.GetEnvironmentVariable("From");
            DisplayName = Environment.GetEnvironmentVariable("DisplayName");
            Password = Environment.GetEnvironmentVariable("Password");
            EnabelSSL = Convert.ToBoolean(Environment.GetEnvironmentVariable("EnabelSSL"));
            url = Environment.GetEnvironmentVariable("url");

            sendgridapi = Environment.GetEnvironmentVariable("SendGridAPI");

            AdminUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
            AdminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            apikey = Environment.GetEnvironmentVariable("VSEE_APIKEY");
            secretkey = Environment.GetEnvironmentVariable("VSEE_SECRETKEY");
            ConnectionString = Utilities.GetPGConnectionString(Environment.GetEnvironmentVariable("HEROKU_POSTGRESQL_GREEN_URL"));
            hangfireConnectionString = Utilities.GetPGConnectionString(Environment.GetEnvironmentVariable("DATABASE_URL"));
            copyrightmsg = Environment.GetEnvironmentVariable("COPYRIGHT_MSG");
            version = Environment.GetEnvironmentVariable("VERSION");
            clienttimeout = Environment.GetEnvironmentVariable("Client_Timeout");
            servertimeout = Environment.GetEnvironmentVariable("Server_Timeout");
            rollbarAccessToken = Environment.GetEnvironmentVariable("Rollbar_Access_Token");
            rollbarEnvironment = Environment.GetEnvironmentVariable("Rollbar_Environment");
            OpenTokAPIKey = Convert.ToInt32(Environment.GetEnvironmentVariable("OpenTokAPIKey"));
            OpenTokAPISecretKey = Environment.GetEnvironmentVariable("OpenTokAPISecretKey");

            ////For local testing
            //Host = "smtp.netfirms.com";
            //Port = 587;
            //From = "test@softtrends.co.in";
            //DisplayName = "Softtrends";
            //Password = "Test@123";
            //EnabelSSL = true;
            //url = "http://localhost:51799";
            //sendgridapi = "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";

            //AdminUserName = "admin";
            //AdminPassword = "a";
            //apikey = "fghnoxeqrdecu9xyzw0lydeu86izx7824hzp4jlc3awfhvyigk1vhtiqimqknkuv";
            //secretkey = "gtsf6ygej8bqngnjvffqrrowxh6yk0yoqsvrmxavbtxeh3p2izjouikp6y6olgl3";
            ////stagingtest
            //hangfireConnectionString = Utilities.GetPGConnectionString("postgres://mfzbiaqynwxeku:b17bd82e4a21dfbce6da53f85dda64c4a7ad34eeed0deaefb0cb19e94df8bbd7@ec2-54-83-8-246.compute-1.amazonaws.com:5432/dflt2qp5n44ru");
            ////dev
            ////hangfireConnectionString = Utilities.GetPGConnectionString("postgres://jqktseplvdccuf:44604313a37eed840601c7c48fb23062aff767b40eba5951e00bf18a26c9f5b2@ec2-54-235-196-250.compute-1.amazonaws.com:5432/dgusdacgtj17v");
            ////trex test
            ////hangfireConnectionString = Utilities.GetPGConnectionString("postgres://jnyedhsuzbqyse:6396f4559a8fdda5f4abc0fb1b78512801d677d35139173e12a75bbe4ed09849@ec2-54-227-240-7.compute-1.amazonaws.com:5432/d8949qcnu79632");

            ////stagingtest
            //ConnectionString = Utilities.GetPGConnectionString("postgres://jmbezzzwpaicso:3c70e292d3c61592c6f5512bfe9bfe3cc66fdde9d05d4b618492b4b5df5ac896@ec2-54-83-8-246.compute-1.amazonaws.com:5432/ddh2qscke6khal");
            ////dev
            ////ConnectionString = Utilities.GetPGConnectionString("postgres://jehhygholtuwbe:d6b07399e24a0da664e77016d2e726f25c225f317bb27b8664df7f08a7106880@ec2-23-23-247-222.compute-1.amazonaws.com:5432/dc0b6auvvnshv8");
            ////staging
            ////ConnectionString = Utilities.GetPGConnectionString("postgres://robunemucinrwo:4429505784f839dc9b2a22d16c3ba464e72e657058f3bcfdfa46615e7a55793a@ec2-107-21-255-2.compute-1.amazonaws.com:5432/d60oduivhro875");
            ////trextest- yellow
            ////ConnectionString = Utilities.GetPGConnectionString("postgres://ksvodnionusxxp:ae72c26b204dcb575a492221b6fa8616371cb2a831786c4bc2ea2f7dcbf2994a@ec2-54-204-14-96.compute-1.amazonaws.com:5432/d9b0rq8f3ht3fu");
            ////MC demo
            ////ConnectionString = Utilities.GetPGConnectionString("postgres://tbozyrjnirtved:3d035cb1f5cb6e92f0913d12e439ea362d5f3cd831e1d2b7b63b537385d1d6d5@ec2-50-19-105-113.compute-1.amazonaws.com:5432/dc71s40e83mmsu");
            //copyrightmsg = "2017 - 2018 TREX INVESTMENT, INC.";
            //version = "2.4.0";
            //clienttimeout = "50";
            //servertimeout = "60";
            //rollbarAccessToken = "5d1dac699c424641a33943e87a2e216c";
            //rollbarEnvironment = "Trex-Rehab-Test";
            //OpenTokAPIKey = 46201082;
            //OpenTokAPISecretKey = "fbef52398143449c9b269e1357d797c64ae945be";
        }
        public static ConfigVars NewInstance
        {
            get
            {
                return ConfigvarInstance.Instance;
            }
        }
        private class ConfigvarInstance
        {
            static ConfigvarInstance()
            {

            }
            internal static readonly ConfigVars Instance = new ConfigVars();
        }
    }
}
