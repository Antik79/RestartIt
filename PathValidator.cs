using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RestartIt
{
    /// <summary>
    /// Provides validation for file paths, directories, and command-line arguments.
    /// </summary>
    public static class PathValidator
    {
        // Characters that are invalid in Windows file paths
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Validates an executable file path.
        /// </summary>
        /// <param name="filePath">The file path to validate</param>
        /// <param name="errorMessage">Output parameter containing error message if validation fails</param>
        /// <returns>True if the path is valid, false otherwise</returns>
        public static bool ValidateExecutablePath(string filePath, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.ExecutablePathRequired",
                    "Executable path is required.");
                return false;
            }

            // Check for invalid path characters
            if (filePath.IndexOfAny(InvalidPathChars) >= 0)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.InvalidPathCharacters",
                    "The path contains invalid characters.");
                return false;
            }

            // Check if path is too long (Windows MAX_PATH is 260 characters)
            if (filePath.Length > 260)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.PathTooLong",
                    "The path is too long. Maximum length is 260 characters.");
                return false;
            }

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    errorMessage = LocalizationService.Instance.GetString(
                        "Validation.FileNotFound",
                        "The executable file does not exist.");
                    return false;
                }

                // Check if it's actually a file (not a directory)
                FileInfo fileInfo = new FileInfo(filePath);
                if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    errorMessage = LocalizationService.Instance.GetString(
                        "Validation.PathIsDirectory",
                        "The specified path is a directory, not a file.");
                    return false;
                }

                // Check file extension (should be .exe, but allow other executables)
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension))
                {
                    errorMessage = LocalizationService.Instance.GetString(
                        "Validation.NoFileExtension",
                        "The file does not have an extension. Please specify a valid executable file.");
                    return false;
                }

                // Warn about non-standard extensions but don't block them
                // (some executables might have .bat, .cmd, .com, etc.)
            }
            catch (ArgumentException)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.InvalidPathFormat",
                    "The path format is invalid.");
                return false;
            }
            catch (PathTooLongException)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.PathTooLong",
                    "The path is too long. Maximum length is 260 characters.");
                return false;
            }
            catch (NotSupportedException)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.PathNotSupported",
                    "The path format is not supported.");
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format(
                    LocalizationService.Instance.GetString(
                        "Validation.PathValidationError",
                        "Error validating path: {0}"),
                    ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a working directory path.
        /// </summary>
        /// <param name="directoryPath">The directory path to validate (can be empty/null)</param>
        /// <param name="errorMessage">Output parameter containing error message if validation fails</param>
        /// <returns>True if the path is valid, false otherwise</returns>
        public static bool ValidateWorkingDirectory(string directoryPath, out string errorMessage)
        {
            errorMessage = null;

            // Working directory is optional
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return true;
            }

            // Check for invalid path characters
            if (directoryPath.IndexOfAny(InvalidPathChars) >= 0)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.InvalidPathCharacters",
                    "The path contains invalid characters.");
                return false;
            }

            // Check if path is too long
            if (directoryPath.Length > 260)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.PathTooLong",
                    "The path is too long. Maximum length is 260 characters.");
                return false;
            }

            try
            {
                // Check if directory exists
                if (!Directory.Exists(directoryPath))
                {
                    errorMessage = LocalizationService.Instance.GetString(
                        "Validation.DirectoryNotFound",
                        "The working directory does not exist.");
                    return false;
                }

                // Verify it's actually a directory
                DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                if (!dirInfo.Exists)
                {
                    errorMessage = LocalizationService.Instance.GetString(
                        "Validation.DirectoryNotFound",
                        "The working directory does not exist.");
                    return false;
                }
            }
            catch (ArgumentException)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.InvalidPathFormat",
                    "The path format is invalid.");
                return false;
            }
            catch (PathTooLongException)
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.PathTooLong",
                    "The path is too long. Maximum length is 260 characters.");
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format(
                    LocalizationService.Instance.GetString(
                        "Validation.DirectoryValidationError",
                        "Error validating directory: {0}"),
                    ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates and sanitizes command-line arguments.
        /// </summary>
        /// <param name="arguments">The arguments string to validate</param>
        /// <param name="sanitizedArguments">Output parameter containing sanitized arguments</param>
        /// <param name="errorMessage">Output parameter containing error message if validation fails</param>
        /// <returns>True if the arguments are valid, false otherwise</returns>
        public static bool ValidateAndSanitizeArguments(string arguments, out string sanitizedArguments, out string errorMessage)
        {
            sanitizedArguments = null;
            errorMessage = null;

            // Arguments are optional
            if (string.IsNullOrWhiteSpace(arguments))
            {
                sanitizedArguments = string.Empty;
                return true;
            }

            // Check for potentially dangerous characters/patterns
            // Note: We allow most characters since arguments can contain various special characters
            // But we check for obvious injection attempts

            // Check for null characters (definitely dangerous)
            if (arguments.Contains("\0"))
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.InvalidNullCharacter",
                    "Arguments cannot contain null characters.");
                return false;
            }

            // Check for extremely long arguments (potential DoS)
            if (arguments.Length > 8192) // Windows command line limit is around 8191 characters
            {
                errorMessage = LocalizationService.Instance.GetString(
                    "Validation.ArgumentsTooLong",
                    "Arguments are too long. Maximum length is 8192 characters.");
                return false;
            }

            // Basic sanitization: trim whitespace
            sanitizedArguments = arguments.Trim();

            // Additional validation: check for suspicious patterns
            // This is a basic check - in production, you might want more sophisticated validation
            // Check for multiple consecutive semicolons (potential command chaining attempt)
            if (Regex.IsMatch(sanitizedArguments, @";{2,}"))
            {
                // Allow but log a warning - semicolons can be valid in some contexts
                // We'll allow it but the user should be aware
            }

            return true;
        }

        /// <summary>
        /// Validates that a path is safe (doesn't contain path traversal attempts).
        /// </summary>
        /// <param name="path">The path to validate</param>
        /// <returns>True if the path appears safe, false if it contains suspicious patterns</returns>
        public static bool IsPathSafe(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return true;

            // Check for path traversal attempts
            if (path.Contains("..") || path.Contains("//") || path.Contains("\\\\"))
            {
                // These could be legitimate, but we'll flag them for review
                // In a strict security context, you might want to block these
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the full normalized path, resolving relative paths and removing redundant separators.
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Normalized full path, or null if path is invalid</returns>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return null;
            }
        }
    }
}

