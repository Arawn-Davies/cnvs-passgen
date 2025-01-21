using System.Security.Cryptography;
using System.Text;

namespace passgen
{
    internal class Program
    {
        private static Dictionary<string, string> sites = new Dictionary<string, string>();
        private static Dictionary<string, string> passwords = new Dictionary<string, string>();

        static void Main(string[] args)
        {
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
            // Capture entry info (website URL, username and password)
            Console.WriteLine("Enter the website URL:");
            string site = Console.ReadLine();
            Console.WriteLine("Enter the username:");
            string user = Console.ReadLine();
            Console.WriteLine("Enter the password:");
            string pass = Console.ReadLine();
            // Add the site and username to the sites dictionary, and the username and password to the passwords dictionary.
            sites.Add(site, user);
            passwords.Add(user, pass);
            Console.WriteLine("Entry added successfully!");
        }

        static void ShowEntries()
        {
            // Show every entry in the sites dictionary, along with the corresponding username and password from the passwords dictionary.
            foreach (var site in sites)
            {
                Console.WriteLine("Site: " + site.Key);
                Console.WriteLine("Username: " + site.Value);
                //Console.WriteLine("Password: " + passwords[site.Value]);
                Console.WriteLine("Password: " + stringRSA(passwords[site.Value]));
                Console.WriteLine();

            }
        }

        static void CheckMP()
        {
            // Prompt the user to enter the master password before allowing them to add an entry.
            Console.WriteLine("Enter your master password:");
            string mp = Console.ReadLine();
            // Hash the entered password using RSA4096, and compare it to the stored hashed password.
            string hashedMP = stringRSA(mp);
            string storedHashedMP = "YOUR_STORED_HASHED_MP";

        }

        static void SetMasterPassword()
        {
            // Capture all keypresses into an array until 'Enter' is pressed.
            Console.WriteLine("Enter your master password:");
            string pass = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    // Check if the password is strong enough (at least 8 characters long, with at least one uppercase letter, one lowercase letter, one number and one special character).
                    if (pass.Length < 8 || !pass.Any(char.IsLower) || !pass.Any(char.IsUpper) || !pass.Any(char.IsDigit) || !pass.Any(char.IsPunctuation))
                    {
                        Console.WriteLine("Password is not strong enough. Please try again.");
                        pass = "";
                        continue;
                    }
                    // Hash the password using RSA4096, and print to screen for user to copy manually.
                    string hashedPass = stringRSA(pass);
                    Console.WriteLine("Your hashed password is: " + hashedPass);

                    Console.WriteLine("Be sure to write it down or secure it safely for future use & recovery.");
                    Console.WriteLine("You will need it in order to decrypt entries.");

                    break;
                }
                pass += key.KeyChar;
            }

        }

        // Encrypt a string to RSA-4096
        private static string stringRSA(string input)
        {
            // Create a new instance of the RSACryptoServiceProvider class
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096))
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
