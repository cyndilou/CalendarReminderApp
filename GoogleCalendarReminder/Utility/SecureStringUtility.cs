using System;
using System.Security;

namespace GoogleCalendarReminder.Utility
{
    public static class SecureStringUtility
    {
        static readonly byte[] Entropy = System.Text.Encoding.Unicode.GetBytes("SuperSecretPasswordEncryption!");

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(SecureStringToString(input)),
                Entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    Entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);

                return StringToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString StringToSecureString(string input)
        {
            var secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            //secure.MakeReadOnly();

            return secure;
        }

        public static string SecureStringToString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }

    //public static class SecureStringUtility
    //{
    //    public static SecureString StringToSecureString(string input)
    //    {
    //        SecureString output = new SecureString();
    //        int l = input.Length;
    //        char[] s = input.ToCharArray(0, l);
    //        foreach (char c in s)
    //        {
    //            output.AppendChar(c);
    //        }
    //        return output;
    //    }

    //    public static String SecureStringToString(SecureString input)
    //    {
    //        string output = "";
    //        IntPtr ptr = SecureStringToBSTR(input);
    //        output = PtrToStringBSTR(ptr);
    //        return output;
    //    }

    //    private static IntPtr SecureStringToBSTR(SecureString ss)
    //    {
    //        IntPtr ptr = new IntPtr();
    //        ptr = Marshal.SecureStringToBSTR(ss);
    //        return ptr;
    //    }

    //    private static string PtrToStringBSTR(IntPtr ptr)
    //    {
    //        string s = Marshal.PtrToStringBSTR(ptr);
    //        Marshal.ZeroFreeBSTR(ptr);
    //        return s;
    //    }
    //}
}
