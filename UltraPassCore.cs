using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Paddings;
using System.Diagnostics;

namespace UltraPassCore
{
    class PasswordManager
    {
        public string[] vaults = [];
        public int VaultLength;
        string VaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"UltraPass\");
        public PasswordManager() 
        {
            ReadVaults();
            VaultLength = vaults.Length;
        }

        /*ReadVaults function:
        Input: none
        Output: array of vaults within vault directory
        Function: gets vaults within directory, determines whether to create a new vault or read from existing list
        */
        public void ReadVaults()
        {
            string[] VaultFiles;
            try
            {
                VaultFiles = Directory.GetFiles(VaultPath, "*.upw", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(VaultPath);
                VaultFiles = [];
            }
            if (VaultFiles.Length != 0)
            {
                vaults = GetVaultNames(VaultFiles);
            }
            else vaults = [];
        }

        /*CreateVault function:
        Input: file path of vaults directory
        Output: array with vault name
        Function: creates a new .upw file in vaults directory with name prompted from CLI
        */
        public void CreateVault(string VaultName, string VaultPassword)
        {
            if(VaultPassword == "")
            {
                throw new Exception("Vault password is blank", e);
            }
            SHA256 hash = SHA256.Create();
            /*IMPORTANT: CHANGE THIS TO TAKE USERNAME AND PASSWORD AS FUNCTION INPUT*/
            string HashedPassword = HashPassword(VaultPassword);
            string path = VaultPath + @"\" + VaultName + ".upw";
            try
            {
                File.Create(path);
            }
            catch(IOException)
            {
                Debug.Write("IOException caught in method CreateVault");
                return;
            }
            File.AppendAllText(path, HashedPassword + Environment.NewLine);
            vaults.Append(VaultName);
            return;

        }

        /*GetVaultNames function:
        Input: array of vault files
        Output: array of vault names
        Function: turns the file paths into just the vault names
        */
        string[] GetVaultNames(string[] VaultFiles)
        {
            for (int i = 0; i < VaultFiles.Length; i++)
            {
                string file = VaultFiles[i];
                string[] name = file.Split(@"\");
                string[] filename = name[name.Length - 1].Split(".");
                VaultFiles[i] = filename[0];
            }
            return VaultFiles;
        }

        /*IsAlphaNumeric function:
        Input: string
        Output: boolean
        Function: determines if a given string is alphanumeric
        */
        public bool IsAlphaNumeric(string input)
        {
            Regex rg = new Regex(@"^[^a-zA-Z0-9_,]*$");
            return rg.IsMatch(input);
        }
        /*Personal note on regexes:
         Regexes contain a series of characters that will be checked against, in this case all lowercase letters (a-z), uppercase letters (A-Z), numbers (0-9), and underscores(_)*/

        /*HashPassword function:
        Input: password string
        Output: Base64-encoded string
        Function: hashes a string with SHA-256 before converting it into a Base64 string
        */
        public string HashPassword(string input)
        {
            SHA256 hash = SHA256.Create();
            byte[] bytes = Encoding.Default.GetBytes(input);
            bytes = hash.ComputeHash(bytes);
            return System.Convert.ToBase64String(bytes);
        }
    }
    class Vault
    {
        string VaultName;
        string VaultPassword;
        Vault(string VaultName, string VaultPassword)
        {
            this.VaultName = VaultName;
            this.VaultPassword = VaultPassword;
        }
    }
    class Password
    {
        string name;
        string username;
        string password;
        public Password()
        {
            name = "name";
            username = "username";
            password = "password";
        }
    }
}
