using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;
using System.Security.Cryptography;
using UltraPassCore;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

void main()
{
    PasswordManager mgr = new PasswordManager();
    menu(mgr);
}

/*menu function:
  Inputs: list of vaults, path to vault folder
  Outputs: none
  Function: acts as the core menu to tie all functions together
*/
void menu(PasswordManager mgr)
{
    bool KillProg = false;
    while (!KillProg)
    {
        Console.WriteLine("Available vaults: ");
        if(mgr.VaultLength != 0) for (int i = 0; i < mgr.VaultLength; i++) Console.WriteLine(i + ". " + mgr.vaults[i]);
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
            string[] VaultInfo = GetVaultInfo();
            mgr.CreateVault(VaultInfo[0], VaultInfo[1]);
            menu(mgr);
            KillProg = true;
        }
        else
        {
            int vault;
            bool success = int.TryParse(response, out vault);
            if (success && 0 <= vault && vault <= mgr.VaultLength - 1)
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

string[] GetVaultInfo()
{
    string name;
    string password;
    bool NameComplete = false;
    bool PasswordComplete = false;
    do
    {
        Console.WriteLine("Enter a name for your vault: ");
        name = Console.ReadLine();
        if (IsAlphaNumeric(name) && name != "") NameComplete = true;
        else Console.WriteLine("Please only use alphanumeric characters or underscores(_) in your vault name.");
    } while(!NameComplete);
    do
    {
        Console.WriteLine("Enter a password for your vault: ");
        password = Console.ReadLine();
        if (password != "") PasswordComplete = true;
        else Console.WriteLine("Please enter a password.");
    } while (!PasswordComplete);
    return [name, password];
}

/*IsAlphaNumeric function:
Input: string
Output: boolean
Function: determines if a given string is alphanumeric
*/
bool IsAlphaNumeric(string input)
{
    Regex rg = new Regex(@"^[^a-zA-Z0-9_,]*$");
    return rg.IsMatch(input);
}


main();