using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace RestartIt
{
    /// <summary>
    /// Manages secure credential storage using Windows Data Protection API (DPAPI).
    /// Passwords are encrypted for the current user and cannot be decrypted by other users.
    /// </summary>
    public static class CredentialManager
    {
        /// <summary>
        /// Encrypts a plain text password using DPAPI.
        /// </summary>
        /// <param name="plainText">The password to encrypt</param>
        /// <returns>Base64-encoded encrypted password, or empty string if input is null/empty</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = ProtectedData.Protect(
                    dataToEncrypt,
                    null, // No additional entropy
                    DataProtectionScope.CurrentUser); // Only current user can decrypt

                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error encrypting credential: {ex.Message}");
                // Return empty string on error - caller should handle
                return string.Empty;
            }
        }

        /// <summary>
        /// Encrypts a SecureString password using DPAPI.
        /// </summary>
        /// <param name="securePassword">The SecureString password to encrypt</param>
        /// <returns>Base64-encoded encrypted password, or empty string if input is null/empty</returns>
        public static string EncryptSecureString(SecureString securePassword)
        {
            if (securePassword == null || securePassword.Length == 0)
                return string.Empty;

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                string plainText = Marshal.PtrToStringUni(ptr);
                return Encrypt(plainText);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }

        /// <summary>
        /// Decrypts a DPAPI-encrypted password.
        /// </summary>
        /// <param name="encryptedText">Base64-encoded encrypted password</param>
        /// <returns>Decrypted plain text password, or empty string if input is null/empty or decryption fails</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            try
            {
                byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = ProtectedData.Unprotect(
                    dataToDecrypt,
                    null, // No additional entropy
                    DataProtectionScope.CurrentUser); // Only current user can decrypt

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting credential: {ex.Message}");
                // Return empty string on error - could be old plain text password or corrupted data
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypts a DPAPI-encrypted password and returns it as a SecureString.
        /// </summary>
        /// <param name="encryptedText">Base64-encoded encrypted password</param>
        /// <returns>Decrypted password as SecureString, or empty SecureString if input is null/empty or decryption fails</returns>
        public static SecureString DecryptToSecureString(string encryptedText)
        {
            SecureString securePassword = new SecureString();
            
            if (string.IsNullOrEmpty(encryptedText))
                return securePassword;

            string plainText = Decrypt(encryptedText);
            
            if (string.IsNullOrEmpty(plainText))
                return securePassword;

            try
            {
                foreach (char c in plainText)
                {
                    securePassword.AppendChar(c);
                }
                return securePassword;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating SecureString: {ex.Message}");
                securePassword.Dispose();
                return new SecureString();
            }
        }

        /// <summary>
        /// Converts a SecureString to a plain string. Use with caution and clear the string after use.
        /// </summary>
        /// <param name="securePassword">The SecureString to convert</param>
        /// <returns>Plain text string representation</returns>
        public static string SecureStringToString(SecureString securePassword)
        {
            if (securePassword == null || securePassword.Length == 0)
                return string.Empty;

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }

        /// <summary>
        /// Checks if a string appears to be encrypted (Base64 format).
        /// This is a heuristic check and not 100% accurate.
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text appears to be Base64-encoded</returns>
        public static bool IsEncrypted(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            try
            {
                // Try to decode as Base64
                Convert.FromBase64String(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
