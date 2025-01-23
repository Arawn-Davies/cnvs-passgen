using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace passgen
{
    public class Site
    {
        public string _URL { get; set; }
        // A site can have multiple users, so we'll use a list to store them.
        public List<User> Users { get; set; }
        
        public Site(string URL)
        {
            if (Users == null)
            {
                Users = new List<User>();
            }
            _URL = URL;
        }

        public Site(string URL, User user)
        {
            if (Users == null)
            {
                Users = new List<User>();
            }
            _URL = URL;
            Users.Add(user);
        }
    }
}
