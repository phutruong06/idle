using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public partial class AppCore
{
    public static Texture2D GenerateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}

public static class AppData
{
    public class Server
    {
        public static string ip = "localhost:3000";

        public static string LoginRoute = $"{ip}/users/login";
        public static string GetTaskRoute = $"{ip}/tasks";

        // Admin
        public static string Admin_CreateTaskRoute = $"{ip}/tasks/create";
        public static string Admin_CancelTaskRoute = $"{ip}/tasks/cancel";

        // Users
        public static string User_SubmitTaskRoute = $"{ip}/tasks/submit";
    }

    public static string HardwareId { get => SystemInfo.deviceUniqueIdentifier; }
    public static string AppId { get => UnityEngine.Application.identifier; }
    public static string Version { get => UnityEngine.Application.version; }

    public static bool isFirstOpen { 
        get => PlayerPrefs.GetInt(nameof(isFirstOpen)) == 1 ? true : false;
        set => PlayerPrefs.SetInt(nameof(isFirstOpen), value == true ? 1 : 0);
    }

    public static bool isLoggedIn { 
        get => PlayerPrefs.GetInt(nameof(isLoggedIn)) == 1 ? true : false;
        set => PlayerPrefs.SetInt(nameof(isLoggedIn), value == true ? 1 : 0);
    }

    public static string token { 
        get => PlayerPrefs.GetString(nameof(token)); 
        set => PlayerPrefs.SetString(nameof(token), value);
    }

    [System.Serializable]
    public static class NativeUser
    {
        public static class CompanyDetail
        {
            public static string companyName = "";
            public static int companySize = 1;
        }

        public static string uid { get => PlayerPrefs.GetString(nameof(uid)); set => PlayerPrefs.SetString(nameof(uid), value); }
        public static int auid { get => PlayerPrefs.GetInt(nameof(auid)); set => PlayerPrefs.SetInt(nameof(auid), value); }
        public static DateTime firstCreateDate
        {
            get => DateTime.ParseExact(PlayerPrefs.GetString(nameof(firstCreateDate)), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            set => PlayerPrefs.SetString(nameof(firstCreateDate), value.ToString());
        }
        public static string username { get => PlayerPrefs.GetString(nameof(username)); set => PlayerPrefs.SetString(nameof(username), value); }
        public static int phoneNumber { get => PlayerPrefs.GetInt(nameof(phoneNumber)); set => PlayerPrefs.SetInt(nameof(phoneNumber), value); }
        public static string email { get => PlayerPrefs.GetString(nameof(email)); set => PlayerPrefs.SetString(nameof(email), value); }
        public static string avatarUri { get => PlayerPrefs.GetString(nameof(avatarUri)); set => PlayerPrefs.SetString(nameof(avatarUri), value); }
        public static string fullName { get => PlayerPrefs.GetString(nameof(fullName)); set => PlayerPrefs.SetString(nameof(fullName), value); }
        public static bool isAdminRole { get => PlayerPrefs.GetInt(nameof(isAdminRole)) == 1 ? true : false; set => PlayerPrefs.SetInt(nameof(isAdminRole), value == true ? 1 : 0); }
        public static string workplaceRole { get => PlayerPrefs.GetString(nameof(workplaceRole)); set => PlayerPrefs.SetString(nameof(workplaceRole), value); }

        public static DateTime dateOfBirth
        {
            get => DateTime.ParseExact(PlayerPrefs.GetString(nameof(dateOfBirth)), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            set => PlayerPrefs.SetString(nameof(dateOfBirth), value.ToString());
        }
    }

    public class AssignUser
    {
        public class CompanyDetail
        {
            public string companyName = "";
            public int companySize = 1;
        }

        public string uid;
        public int auid;
        public string username;
        public int phoneNumber;
        public string email;
        public string avatarUri;
        public string fullName;
        public bool isAdminRole;
        public string workplaceRole;

        public static DateTime dateOfBirth;
    }
}

public static class MemberInfoGetting
{
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
}