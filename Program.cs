using System.Security.Cryptography;
using System.Text;

namespace passgen
{
    internal class Program
    {
        private static Database _dB;
        static void Main(string[] args)
        {
            _dB = new Database();
            Console.WriteLine("Welcome to PassGen!");
            Console.WriteLine("1 - Add a site, user & password");
            Console.WriteLine("2 - Show all sites, users & passwords");
            Console.WriteLine("3 - Create master password");
            Console.WriteLine("4 - Exit");

            string option = Console.ReadLine();

            int opt = 0;
            while (true)
            {
                try
                {
                    opt = Convert.ToInt32(option);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    option = Console.ReadLine();
                }
            }


            if (opt == 1)
            {
                // Check that the master password has been entered before allowing the user to add an entry.
                CheckMP();
                AddEntry();
                ShowEntries();
            }
            else if (opt == 2)
            {
                ShowEntries();
            }
            else if (opt == 3)
            {
                SetMasterPassword();
            }
            Environment.Exit(0);
        }

        static void AddEntry()
        {
            string? url, username, pass = "";
            // Capture entry info (website URL, username and password)
            while (true)
            {
                Console.WriteLine("Enter the website URL:");
                url = Console.ReadLine();
                if (string.IsNullOrEmpty(url))
                {
                    Console.WriteLine("URL cannot be empty. Please try again.");
                    continue;
                }
                break;
            }
            Console.WriteLine("Enter the username:");
            while (true)
            {
                username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Username cannot be empty. Please try again.");
                    continue;
                }
                break;
            }
            Console.WriteLine("Enter the password:");
            while (true)
            {
                pass = Console.ReadLine();
                if (string.IsNullOrEmpty(pass))
                {
                    Console.WriteLine("Password cannot be empty. Please try again.");
                    continue;
                }
                break;
            }

            // Create a new User and Site object based on the user input.
            User user = new User(username, pass);
            Site site = new Site(url, user);
            // Add the new objects to the database.
            _dB.Sites.Add(site);
            _dB.Users.Add(user);
            Console.WriteLine("Entry added successfully!");
        }

        static void ShowEntries()
        {
            // Show every entry in the sites dictionary, along with the corresponding username and password from the passwords dictionary.
            foreach (var site in _dB.Sites)
            {
                if (site._URL != null)
                {
                    Console.WriteLine("URL: " + site._URL);
                }
                else
                {
                    Console.WriteLine("No URL found for this site.");
                }
                if (site.Users != null)
                {
                    foreach (var user in site.Users)
                    {
                        Console.WriteLine("Username: " + user.Username);
                    }
                }
                else
                {
                    Console.WriteLine("No users found for this site.");
                }
                if (site.Users != null)
                {
                    foreach (var user in site.Users)
                    {
                        Console.WriteLine("Password: " + user.Password);
                    }
                }
                else
                {
                    Console.WriteLine("No passwords found for this site.");
                }
                Console.WriteLine();

            }
            foreach (var user in _dB.Users)
            {
                if (user.Username != null)
                {
                    Console.WriteLine("Username: " + user.Username);
                }
                else
                {
                    Console.WriteLine("No username found for this user.");
                }
                if (user.Password != null)
                {
                    Console.WriteLine("Password: " + user.Password);
                }
                else
                {
                    Console.WriteLine("No password found for this user.");
                }
                Console.WriteLine();
            }
        }

        static void CheckMP()
        {
            // Check if the master password isn't empty, and if it is, prompt the user to set it.
            if (string.IsNullOrEmpty(_dB.MasterPassword))
            {
                Console.WriteLine("Master password not set. Please set it before adding an entry.");
                SetMasterPassword();
                return;
            }
            // Prompt the user to enter the master password before allowing them to add an entry.
            Console.WriteLine("Enter your master password:");
            string? mp = "";
            mp = Console.ReadLine();
            // Hash the entered password using RSA4096, and compare it to the stored hashed password.
            string hashedMP = stringRSA(mp);
            if (hashedMP != _dB.MasterPassword)
            {
                Console.WriteLine("Incorrect master password. Please try again.");
                CheckMP();
            }

        }

        static void SetMasterPassword()
        {
            // Capture all keypresses into an array until 'Enter' is pressed.
            Console.WriteLine("Enter your master password:");
            string pass = "";
            string hashedPass = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                
                // Handle backspace keypresses
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (pass.Length > 0)
                    {
                        //Console.Write("\x1B[1D");
                        //Console.Write("\x1B[1P");
                        pass = pass.Remove(pass.Length  - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    // Check if the password is strong enough (at least 8 characters long, with at least one uppercase letter, one lowercase letter, one number and one special character).
                    if (pass.Length < 8 || !pass.Any(char.IsLower) || !pass.Any(char.IsUpper) || !pass.Any(char.IsDigit) || !pass.Any(char.IsPunctuation))
                    {
                        Console.WriteLine("Password is not strong enough. Please try again.");
                        pass = "";
                        continue;
                    }
                    // Hash the password using RSA-512, and print to screen for user to copy manually.
                    hashedPass = stringRSA(pass);
                    Console.WriteLine("Your hashed password is: \n\n" + hashedPass);

                    Console.WriteLine("Be sure to write it down or secure it safely for future use & recovery.");
                    Console.WriteLine("You will need it in order to decrypt entries.");

                    break;
                }
                else
                {
                    Console.Write("*");
                }
                pass += key.KeyChar;
            }
            _dB.MasterPassword = hashedPass;
        }

        // Encrypt a string to RSA-512
        private static string stringRSA(string input)
        {
            // Create a new instance of the RSACryptoServiceProvider class
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512))
            {
                // Convert the string to a byte array
                byte[] data = Encoding.UTF8.GetBytes(input);
                // Encrypt the data
                byte[] encryptedData = rsa.Encrypt(data, true);
                // Create a new StringBuilder to collect the bytes
                StringBuilder sb = new StringBuilder();
                // Loop through each byte of the encrypted data and format each one as a hexadecimal string
                foreach (byte b in encryptedData)
                {
                    sb.Append(b.ToString("x2"));
                }
                // Return the hexadecimal string
                return sb.ToString();
            }
        }

    }
}
