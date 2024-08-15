using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;

void main()
{
    string VaultPath = @"C:\ProgramData\UltraPass";
    string[] vaults = ReadVaults(VaultPath);
    menu(vaults, VaultPath);
}

/*menu function:
  Inputs: list of vaults, path to vault folder
  Outputs: none
  Function: acts as the core menu to tie all functions together
*/
void menu(string[] vaults, string VaultPath)
{
    int VaultLength = vaults.Length;
    bool KillProg = false;
    while (!KillProg)
    {
        Console.WriteLine("Available vaults: ");
        for (int i = 0; i < VaultLength; i++) Console.WriteLine(i + ". " + vaults[i]);
        Console.WriteLine("Available options: \nType the vault's number to open it. \nType n to create a new vault. \nType q to quit.");
        string response = Console.ReadLine();
        if (response == "q")
        {
            Console.WriteLine("Thanks for using the UltraPass password vault!");
            KillProg = true;
        }
        else if (response == "n")
        {
            Console.WriteLine("Creating a new vault...");
            CreateVault(VaultPath);
            string[] NewVaults = ReadVaults(VaultPath);
            menu(NewVaults, VaultPath);
            KillProg = true;
        }
        else
        {
            int vault;
            bool success = int.TryParse(response, out vault);
            if (success && 0 <= vault && vault <= VaultLength - 1)
            {
                OpenVault(vault);
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
}

/*OpenVault function:
  TO DO
*/
void OpenVault(int vault)
{
    Console.WriteLine("Opening vault...");
    return;
}

/*ReadVaults function:
  Input: none
  Output: array of vaults within vault directory
  Function: gets vaults within directory, determines whether to create a new vault or read from existing list
*/
string[] ReadVaults(string VaultPath)
{
    string[] vaults;
    string[] VaultFiles;
    try
    {
        VaultFiles = Directory.GetFiles(VaultPath, "*.upw", SearchOption.AllDirectories);
    }
    catch(DirectoryNotFoundException)
    {
        Directory.CreateDirectory(VaultPath);
        VaultFiles = [];
    }
    if (VaultFiles.Length == 0)
    {
        Console.WriteLine("No vaults found! Creating a new vault...");
        CreateVault(VaultPath);
    }
        vaults = GetVaultNames(VaultFiles);
    
    return vaults;
}

/*InitializeVault function:
  Input: file path of vaults directory
  Output: array with vault name
  Function: creates a new .upw file in vaults directory with name prompted from CLI
 */
void CreateVault(string FilePath)
{
    string VaultName;
    bool complete = false;
    do
    {
        Console.WriteLine("Enter your new vault name here:");
        VaultName = Console.ReadLine();
        if (VaultName == "") Console.WriteLine("Put in a valid vault name.");
        else if (!InputChecker(VaultName)) Console.WriteLine("Your name has a disallowed character. Please try again.");
        else complete = true;
    } while (!complete);
    File.Create(FilePath + @"\" + VaultName + ".upw");
    return;
    
}

/*InputChecker function:
  Input: string
  Output: bool
  Function: determines if an arbitrary input contains any disallowed characters or not.
*/
bool InputChecker(string input)
{
    char[] DisallowedCharacters = ['/', '\\', '.', '*'];
    for(int i = 0; i < DisallowedCharacters.Length; i++)
    {
        if (input.Contains(DisallowedCharacters[i])) return false;
    }
    return true;
}

/*GetVaultName function:
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

main();