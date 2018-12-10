using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public class Utilities
    {
        public static ILoggerFactory AppLoggerFactory { get; set; }
        public static IHostingEnvironment env { get; set; }
        public static string GetPGConnectionString(string pDatabaseUrl)
        {
            if (!string.IsNullOrEmpty(pDatabaseUrl))
            {
                string conStrParts = pDatabaseUrl.Replace("//", "");
                string[] strConn = conStrParts.Split(new char[] { '/', ':', '@', '?' });
                strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return string.Format("Host={0};Port={1};Database={2};User ID={3};Password={4};sslmode=Require;Trust Server Certificate=true;Pooling=false;", strConn[3], strConn[4], strConn[5], strConn[1], strConn[2]);
            }

            return string.Empty;
        }

        public static string GetEnvVarVal(string pKey)
        {
            return Environment.GetEnvironmentVariable(pKey);
        }

        public static string SHA1HashStringForUTF8String(string pStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(pStr);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalMilliseconds;
        }
        public static string getHighStockColor(int count)
        {
            string result = "";
            if (count == 0)
            {
                result = "#2f7ed8";
            }
            else if (count > 0 && count < 3)
            {
                result = "#0d233a";
            }
            else
            {
                result = "#8bbc21";
            }
            return result;
        }
        //public static List<ExcerciseProtocol> GetExcerciseProtocol()
        //{
        //    List<ExcerciseProtocol> _ExcerciseProtocol = new List<ExcerciseProtocol>();

        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Knee", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Flexion", ProtocolEnum = 1 });
        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Knee", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Extension", ProtocolEnum = 2 });
        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Knee", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Flexion-Extension", ProtocolEnum = 3 });

        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Ankle", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Flexion", ProtocolEnum = 1 });
        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Ankle", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Extension", ProtocolEnum = 2 });
        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Ankle", ExcerciseEnum = "Flexion-Extension", ProtocolName = "Flexion-Extension", ProtocolEnum = 3 });

        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Shoulder", ExcerciseEnum = "Forward Flexion", ProtocolName = "Forward Flexion", ProtocolEnum = 1 });

        //    _ExcerciseProtocol.Add(new ExcerciseProtocol { Limb = "Shoulder", ExcerciseEnum = "External Rotation", ProtocolName = "External Rotation", ProtocolEnum = 1 });



        //    return _ExcerciseProtocol;
        //}

        //public static List<EquipmentExcercise> GetEquipmentExcercise()
        //{
        //    List<EquipmentExcercise> _EquipmentExcercise = new List<EquipmentExcercise>();

        //    _EquipmentExcercise.Add(new EquipmentExcercise { Limb = "Knee", ExcerciseEnum = "Flexion-Extension", EquipmentName = "Stretch Bar", ExcerciseName = "Flexion-Extension" });

        //    _EquipmentExcercise.Add(new EquipmentExcercise { Limb = "Ankle", ExcerciseEnum = "Flexion-Extension", EquipmentName = "Stretch Bar", ExcerciseName = "Flexion-Extension" });

        //    _EquipmentExcercise.Add(new EquipmentExcercise { Limb = "Shoulder", ExcerciseEnum = "Forward Flexion", EquipmentName = "Straight Arm", ExcerciseName = "Straight Arm Forward Flexion" });

        //    _EquipmentExcercise.Add(new EquipmentExcercise { Limb = "Shoulder", ExcerciseEnum = "External Rotation", EquipmentName = "Rotation Arm", ExcerciseName = "External Rotation" });

        //    return _EquipmentExcercise;
        //}

        public static List<Sides> GetSide()
        {
            List<Sides> _Sides = new List<Sides>();
            _Sides.Add(new Sides { Side = "Left", value = 1 });
            _Sides.Add(new Sides { Side = "Right", value = 1 });
            return _Sides;
        }

        public static string GetTimeSlot(string id)
        {
            string result = "";
            switch (id)
            {
                case "0":
                    result = "12 AM - 1 AM";
                    break;
                case "1":
                    result = "1 AM - 2 AM";
                    break;
                case "2":
                    result = "2 AM - 3 AM";
                    break;
                case "3":
                    result = "3 AM - 4 AM";
                    break;
                case "4":
                    result = "4 AM - 5 AM";
                    break;
                case "5":
                    result = "5 AM - 6 AM";
                    break;
                case "6":
                    result = "6 AM - 7 AM";
                    break;
                case "7":
                    result = "7 AM - 8 AM";
                    break;
                case "8":
                    result = "8 AM - 9 AM";
                    break;
                case "9":
                    result = "9 AM - 10 AM";
                    break;
                case "10":
                    result = "10 AM - 11 AM";
                    break;
                case "11":
                    result = "11 AM - 12 PM";
                    break;
                case "12":
                    result = "12 PM - 1 PM";
                    break;
                case "13":
                    result = "1 PM - 2 PM";
                    break;
                case "14":
                    result = "2 PM - 3 PM";
                    break;
                case "15":
                    result = "3 PM - 4 PM";
                    break;
                case "16":
                    result = "4 PM - 5 PM";
                    break;
                case "17":
                    result = "5 PM - 6 PM";
                    break;
                case "18":
                    result = "6 PM - 7 PM";
                    break;
                case "19":
                    result = "7 PM - 8 PM";
                    break;
                case "20":
                    result = "8 PM - 9 PM";
                    break;
                case "21":
                    result = "9 PM - 10 PM";
                    break;
                case "22":
                    result = "10 PM - 11 PM";
                    break;
                case "23":
                    result = "11 PM - 12 AM";
                    break;

            }
            return result;
        }
        public static string GetLocalDateTime(DateTime? Convertdate, string ClientTimeZoneoffset)
        {
            try
            {
                if (Convertdate != null && ClientTimeZoneoffset != null)
                {
                    string Temp = ClientTimeZoneoffset.Trim();
                    if (!Temp.Contains("+") && !Temp.Contains("-"))
                    {
                        Temp = Temp.Insert(0, "+");
                    }
                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());
                    foreach (TimeZoneInfo timeZoneInfo in timeZones)
                    {
                        if (timeZoneInfo.ToString().Contains(Temp))
                        {
                            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                            _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                            break;
                        }
                    }
                    return _now.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }


        }
        public static string getUserType(string usertype)
        {
            string result = "";
            switch (usertype)
            {
                case "0":
                    result = "Admin";
                    break;
                case "1":
                    result = "Support";
                    break;
                case "2":
                    result = "Therapist";
                    break;
                case "3":
                    result = "Provider";
                    break;
                case "4":
                    result = "Installer";
                    break;
                case "5":
                    result = "Patient";
                    break;
                case "6":
                    result = "Patient Administrator";
                    break;
            }
            return result;
        }

        public static string GetUTCDateTimebyTimeZoneId(DateTime? Convertdate, string timeZoneId)
        {
            try
            {
                if (Convertdate.HasValue && timeZoneId != null)
                {

                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());

                    try
                    {


                        TimeZoneInfo tst = timeZones.FirstOrDefault(p => (p.SupportsDaylightSavingTime && p.DaylightName == timeZoneId) || p.StandardName == timeZoneId);
                        if (tst != null)
                        {
                            _now = TimeZoneInfo.ConvertTime(startTime, tst, TimeZoneInfo.Utc);
                        }
                    }
                    catch (Exception ex)
                    {


                    }

                    return _now.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }


        }

        public static string GetLocalDateTimebyTimeZoneId(DateTime? Convertdate, string timeZoneId)
        {
            try
            {
                if (Convertdate.HasValue && timeZoneId != null)
                {

                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());

                    //Heroku
                    try
                    {

                        TimeZoneInfo tst = timeZones.FirstOrDefault(p => (p.SupportsDaylightSavingTime && p.DaylightName == timeZoneId) || p.StandardName == timeZoneId);
                        if (tst != null)
                        {
                            _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                        }
                    }
                    catch (Exception ex)
                    {


                    }

                    return _now.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }


        }

        public static string GetUTCDateTime(DateTime? Convertdate, string ClientTimeZoneoffset)
        {
            try
            {
                if (Convertdate != null && ClientTimeZoneoffset != null)
                {
                    string Temp = ClientTimeZoneoffset.Trim();
                    if (!Temp.Contains("+") && !Temp.Contains("-"))
                    {
                        Temp = Temp.Insert(0, "+");
                    }
                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

                    Console.Write("System Time Zones :" + JsonConvert.SerializeObject(timeZones));
                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());
                    foreach (TimeZoneInfo timeZoneInfo in timeZones)
                    {
                        if (timeZoneInfo.SupportsDaylightSavingTime)
                            Console.Write("Prabhu TimeZoneInfo:" + JsonConvert.SerializeObject(timeZoneInfo));
                        if (timeZoneInfo.ToString().Contains(Temp))
                        {
                            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                            _now = TimeZoneInfo.ConvertTime(startTime, tst, TimeZoneInfo.Utc);
                            break;
                        }
                    }
                    return _now.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }


        }

        public static string ConverTimetoServerTimeZone(DateTime date, string timezone)
        {


            string localUtcTime = Utilities.GetUTCDateTimebyTimeZoneId(date, timezone);
            Console.Write("Prabhu Utc Date :" + localUtcTime);
            string timezoneid = TimeZoneInfo.Local.Id;// "Eastern Daylight Time"; // // TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            string serverDateTime = Utilities.GetLocalDateTimebyTimeZoneId(Convert.ToDateTime(localUtcTime), timezoneid);
            Console.Write("Prabhu Server Date :" + serverDateTime);
            return serverDateTime;
        }

        public static string ConverTimetoBrowserTimeZone(DateTime date, string timezone)
        {

            Console.Write("Prabhu Browser TimeZone :" + timezone);

            string timezoneid = TimeZoneInfo.Local.Id;// "Eastern Daylight Time"
            Console.Write("Prabhu Server TimeZone :" + timezoneid);
            string localUtcTime = Utilities.GetUTCDateTimebyTimeZoneId(date, timezoneid);
            Console.Write("Prabhu UTC Date :" + localUtcTime);
            string browserDateTime = Utilities.GetLocalDateTimebyTimeZoneId(Convert.ToDateTime(localUtcTime), timezone);

            Console.Write("Prabhu Bfowser Date :" + localUtcTime);
            return browserDateTime;

        }

        public static string ConverOneTimeZonetoAnotherTimeZone(DateTime date, string basetimezone, string destinationtimezone)
        {

            var basetoffset = Utilities.convert(Convert.ToDouble(basetimezone) / 60);

            var destinationoffset = Utilities.convert(Convert.ToDouble(destinationtimezone) / 60);

            string localUtcTime = Utilities.GetUTCDateTime(date, basetoffset);
            string serverDateTime = Utilities.GetLocalDateTime(Convert.ToDateTime(localUtcTime), destinationoffset);
            return serverDateTime;
        }

        public static string convert(double value)
        {

            var hours = (int)value;
            value -= (int)value;
            value *= 60;
            var mins = (int)value;
            value -= (int)value;
            value *= 60;
            var secs = (int)value;
            var display_hours = "";
            var minute = "";
            // handle GMT case (00:00)
            if (hours == 0)
            {
                display_hours = "00";
            }
            else if (hours > 0)
            {
                // add a plus sign and perhaps an extra 0
                display_hours = (hours < 10) ? "+0" + hours : "+" + hours;
            }
            else
            {
                // add an extra 0 if needed
                display_hours = (hours > -10) ? "-0" + Math.Abs(hours) : hours.ToString();
            }

            if (mins == 0)
            {
                minute = "00";
            }
            else if (mins > 0)
            {
                // add a plus sign and perhaps an extra 0
                minute = (mins < 10) ? "0" + mins : mins.ToString();
            }
            else
            {
                // add an extra 0 if needed
                minute = (mins > -10) ? "0" + Math.Abs(mins) : Math.Abs(mins).ToString();
            }

            return display_hours + ":" + minute;
        }

        public static string EncryptText(string stringToEncrypt)
        {
            string encryptedValue = string.Empty;
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = 0x80;
                    aesAlg.BlockSize = 0x80;
                    byte[] pwdBytes = Encoding.UTF8.GetBytes("TREX%18");
                    byte[] keyBytes = new byte[0x10];
                    int len = pwdBytes.Length;
                    if (len > keyBytes.Length)
                    {
                        len = keyBytes.Length;
                    }
                    Array.Copy(pwdBytes, keyBytes, len);
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = keyBytes;

                    ICryptoTransform transform = aesAlg.CreateEncryptor();
                    byte[] plainText = Encoding.UTF8.GetBytes(stringToEncrypt);
                    encryptedValue = Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return encryptedValue;
        }

        public static string DecryptText(string encryptedText)
        {
            string decryptedValue = string.Empty;
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = 0x80;
                    aesAlg.BlockSize = 0x80;
                    byte[] pwdBytes = Encoding.UTF8.GetBytes("TREX%18");
                    byte[] keyBytes = new byte[0x10];
                    int len = pwdBytes.Length;
                    if (len > keyBytes.Length)
                    {
                        len = keyBytes.Length;
                    }
                    Array.Copy(pwdBytes, keyBytes, len);
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = keyBytes;

                    byte[] encryptedData = Convert.FromBase64String(encryptedText);
                    byte[] plainText = aesAlg.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    decryptedValue = Encoding.UTF8.GetString(plainText);
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
            return decryptedValue;
        }

    }
}
