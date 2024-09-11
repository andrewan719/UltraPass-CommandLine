using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;
using System.Security.Cryptography;
using UltraPassCore;

void main()
{
    string VaultPath = @"C:\ProgramData\UltraPass";
    PasswordManager mgr = new PasswordManager();
    menu(mgr, VaultPath);
}

/*menu function:
  Inputs: list of vaults, path to vault folder
  Outputs: none
  Function: acts as the core menu to tie all functions together
*/
void menu(PasswordManager mgr)
{
    int VaultLength = mgr.vaults.Length;
    bool KillProg = false;
    while (!KillProg)
    {
        Console.WriteLine("Available vaults: ");
        for (int i = 0; i < VaultLength; i++) Console.WriteLine(i + ". " + mgr.vaults[i]);
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
            mgr.CreateVault();
            menu(mgr);
            KillProg = true;
        }
        else
        {
            int vault;
            bool success = int.TryParse(response, out vault);
            if (success && 0 <= vault && vault <= VaultLength - 1)
            {
                Console.WriteLine("Opening vault" + vault);
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
}



main();