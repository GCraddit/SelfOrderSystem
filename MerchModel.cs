using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giles_Chen_test_1
{
    public class MerchModel
    {
        private readonly CafeContext _dbContext;

        public MerchModel(CafeContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<Merch> GetMerchItems()
        {
            return _dbContext.Merches.ToList();
        }
    }
}

