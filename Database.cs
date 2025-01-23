using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passgen
{
    public class Database
    {
        public Database() 
        {
            if (Sites == null)
            {
                Sites = new List<Site>();
            }
            if (Users == null)
            {
                Users = new List<User>();
            }
        }
        public List<Site> Sites { get; set; }
        public string MasterPassword { get; set; }
        public List<User> Users { get; set; }
    }
}
