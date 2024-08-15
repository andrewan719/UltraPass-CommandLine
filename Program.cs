using System.IO;
using System.Collections.Generic;

void main()
{
    string[] vaults = ReadVaults();
    

}

/*ReadVaults function:
  Input: none
  Output: array of vaults within vault directory
  Function: gets vaults within directory, determines whether to create a new vault or read from existing list
*/
string[] ReadVaults()
{
    string[] vaults;
    string VaultPath = @"C:\ProgramData\UltraPass";
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
        vaults = CreateVault(VaultPath);
    }
    else
    {
        vaults = GetVaultNames(VaultFiles);
    }
    return vaults;
}

/*InitializeVault function:
  Input: file path of vaults directory
  Output: array with vault name
  Function: creates a new .upw file in vaults directory with name prompted from CLI
 */
string[] CreateVault(string FilePath)
{
    string VaultName;
    bool complete = false;
    do
    {
        Console.WriteLine("Enter your new vault name here:");
        VaultName = Console.ReadLine();
        if (VaultName == "")
        {
            Console.WriteLine("Put in a valid vault name.");
        }
        else complete = true;
    } while (!complete);
    File.Create(FilePath + @"\" + VaultName + ".upw");
    return [VaultName];
    
}

/*GetVaultName function:
  Input: array of vault files
  Output: array of vault names
  Function: turns the file paths into just the vault names
*/
string[] GetVaultNames(string[] VaultFiles)
{
    List<string> vaults = new List<string>();
    foreach (var file in VaultFiles)
    {
        string[] name = file.Split(@"\");
        string[] filename = name[name.Length - 1].Split(".");
        Console.WriteLine(filename.First());
        vaults.Append(filename.First());
    }
    return vaults.ToArray();
}

main();