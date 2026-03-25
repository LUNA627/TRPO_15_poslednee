using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ElectronicStoreShop.Models;

namespace ElectronicStoreShop.Service
{
    public class DBService
    {
        private ElectronicStoreContext context;
        public ElectronicStoreContext Context => context;

        private static DBService? instance;
        public static DBService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBService();
                }
                return instance;
            }
        }
        private DBService()
        {
            context = new ElectronicStoreContext();
        }
    }
}
