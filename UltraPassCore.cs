using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace UltraPassCore
{
    public static class UltraPassUtils
    {
        /*IsAlphaNumeric function:
        Input: string
        Output: boolean
        Function: determines if a given string is alphanumeric
        */
        public static bool IsAlphaNumeric(string input)
        {
            Regex rg = new Regex(@"^[^a-zA-Z0-9_,]*$");
            return rg.IsMatch(input);
        }
        /*Personal note on regexes:
         Regexes contain a series of characters that will be checked against, in this case all lowercase letters (a-z), uppercase letters (A-Z), numbers (0-9), and underscores(_)*/

        /*HashPassword function:
        Input: password string
        Output: Base64-encoded string of the hash
        Function: hashes a string with SHA-256 before converting it into a Base64 string
        */
        public static string HashPassword(string input)
        {
            SHA256 hash = SHA256.Create();
            byte[] bytes = Encoding.Default.GetBytes(input);
            bytes = hash.ComputeHash(bytes);
            return System.Convert.ToBase64String(bytes);
        }
        public static string GenerateKey(string input)
        {
            SHA1 hash = SHA1.Create();
            byte[] bytes = Encoding.Default.GetBytes(input);
            bytes = hash.ComputeHash(bytes);
            return System.Convert.ToBase64String(bytes);
        }
    }
    class PasswordManager
    {
        public List<string> vaults = [];
        public int VaultLength;
        string VaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"UltraPass\");
        public PasswordManager() 
        {
            ReadVaults();
            VaultLength = vaults.Count;
        }

        /*ReadVaults function:
        Input: none
        Output: array of vaults within vault directory
        Function: gets vaults within directory, determines whether to create a new vault or read from existing list
        */
        public void ReadVaults()
        {
            List<string> VaultFiles;
            try
            {
                VaultFiles = Directory.GetFiles(VaultPath, "*.upw", SearchOption.AllDirectories).ToList<string>();
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(VaultPath);
                VaultFiles = [];
            }
            if (VaultFiles.Count != 0)
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
                throw new Exception("Vault password is blank");
            }
            SHA256 hash = SHA256.Create();
            string HashedPassword = UltraPassUtils.HashPassword(VaultPassword);
            string path = VaultPath + @"\" + VaultName + ".upw";
            try
            {
                File.AppendAllText(path, HashedPassword + Environment.NewLine);
                vaults.Add(VaultName);
            }
            catch
            {
                Debug.Write("Exception caught in method CreateVault");
                throw;
            }
            return;

        }

        /*GetVaultNames function:
        Input: array of vault files
        Output: array of vault names
        Function: turns the file paths into just the vault names
        */
        List<String> GetVaultNames(List<string> VaultFiles)
        {
            for (int i = 0; i < VaultFiles.Count; i++)
            {
                string file = VaultFiles[i];
                string[] name = file.Split(@"\");
                string[] filename = name[name.Length - 1].Split(".");
                VaultFiles[i] = filename[0];
            }
            return VaultFiles;
        }

        

        public Vault OpenVault(int vault, string password)
        {
            string key;
            
            string path = VaultPath + @"\" + vaults[vault] + ".upw";
            Vault v = new Vault(VaultPath, vaults[vault]);
            if(v.CheckPassword(password))
            {
                key = password;
                return v;
            }
            Debug.WriteLine("Password incorrect; throwing exception");
            throw new Exception("Incorrect password.");

        }
        public void CloseVault()
        {

        }
    }
    class Vault
    {
        List<Password> passwords = new List<Password>();
        string VaultName;
        private string HashedPassword;
        private string key;
        private int changes;
        private string FilePath;
        public Vault(string FilePath, string FileName)
        {
            this.changes = 0;
            this.VaultName = FileName;
            this.FilePath = FilePath;
            string path = FilePath + @"\" + FileName + ".upw";
            string[] file = File.ReadAllLines(path);
            this.key = "";
            if (file.Length > 0)
            {
                this.HashedPassword = file[0];
                for (int i = 1; i < file.Length; i++)
                {
                    passwords.Add(new Password(this, file[i]));
                }
            }
            else
            {
                throw new Exception("No password set for vault " + VaultName);
            }
        }
        public string GetVaultPassword()
        {
            return HashedPassword;
        }
        public void FlagChange()
        {
            changes++;
            return;
        }
        public string GetKey()
        {
            return key;
        }
        public bool CheckPassword(string p)
        {
            SHA256 hash = SHA256.Create();
            string hp = UltraPassUtils.HashPassword(p);
            if (hp == HashedPassword)
            {
                this.key = p;
                return true;
            }
            else return false;
        }
        public void AddPassword(string VaultPassword, string name, string username, string password)
        {
            Random rnd = new Random();
            Byte[] EncryptedUsername;
            Byte[] EncryptedPassword;
            int uiv = rnd.Next(1, 999999999);
            int piv = rnd.Next(1, 999999999);
            using(Aes encoder = Aes.Create())
            {
                encoder.Key = Convert.FromBase64String(UltraPassUtils.GenerateKey(key));

                EncryptedUsername = encoder.EncryptCbc(Encoding.ASCII.GetBytes(username), BitConverter.GetBytes(uiv));
                EncryptedPassword = encoder.EncryptCbc(Encoding.ASCII.GetBytes(username), BitConverter.GetBytes(piv));
            }
            Password p = new Password(this, name, EncryptedUsername, EncryptedPassword, uiv.ToString(), piv.ToString());
            passwords.Add(p);
        }
        public void CloseVault()
        {
            if (changes > 0)
            {
                //Update the file with new information
                string path = FilePath + @"\" + VaultName + ".upw";
                var arrLine = new List<string>();
                arrLine.Add(HashedPassword);
                foreach (Password p in passwords) 
                {
                    arrLine.Add(p.ExportLine());
                }
                File.WriteAllLines(path, arrLine);

            }
            return;
        }
    }
    class Password
    {
        string name;
        Byte[] username;
        string UIV;
        Byte[] password;
        string PIV;
        private Vault _parent;
        public Password(Vault parent, string n)
        {
            string[] data = n.Split(":");
            if (data.Length != 5)
            {
                throw new Exception("Not a valid line type");
            }
            this.name = data[0];
            this.username = Convert.FromBase64String(data[1]);
            this.UIV = data[2];
            this.password = Convert.FromBase64String(data[3]);
            this.PIV = data[4];
            this._parent = parent;

        }
        public Password(Vault parent, string name, Byte[] username, Byte[] password, string UIV, string PIV)
        {
            this._parent = parent;
            this.name = name;
            this.username = username;
            this.UIV = UIV;
            this.password = password;
            this.PIV = PIV;
        }
        public string GetName()
        {
            return name;
        }

        public (string, string) DecryptCredentials(string key)
        {
            string DecryptedUsername;
            string DecryptedPassword;
            using (Aes encoder = Aes.Create())
            {
                encoder.Key = Convert.FromBase64String(UltraPassUtils.GenerateKey(key));

                DecryptedUsername = Encoding.ASCII.GetString(encoder.DecryptCbc(this.username, BitConverter.GetBytes(Int32.Parse(this.UIV))));
                DecryptedPassword = Encoding.ASCII.GetString(encoder.DecryptCbc(this.password, BitConverter.GetBytes(Int32.Parse(this.PIV))));
            }
            return (DecryptedUsername, DecryptedPassword);
        }
        public void ChangeCredentials(string username, string password)
        {
            Byte[] EncryptedUsername;
            Byte[] EncryptedPassword;
            using (Aes encoder = Aes.Create())
            {
                encoder.Key = Convert.FromBase64String(_parent.GetKey());

                EncryptedUsername = encoder.EncryptCbc(Encoding.ASCII.GetBytes(username), BitConverter.GetBytes(Int32.Parse(this.UIV)));
                EncryptedPassword = encoder.EncryptCbc(Encoding.ASCII.GetBytes(username), BitConverter.GetBytes(Int32.Parse(this.PIV)));
            }
            this.username = EncryptedUsername;
            this.password = EncryptedPassword;
            _parent.FlagChange();
            return;
        }
        public string ExportLine()
        {
            string line = name + ":" + Convert.ToBase64String(username) + ":" + UIV + ":" + Convert.ToBase64String(password) + ":" + PIV;
            return line;
        }
    }
}
