using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApi.Domain.Blogs;

namespace CatalogApi.Domain.Employees
{
    public class Employee : Entity
    {
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; }

        public Department Department { get; set; }
    }

    public class Department : Entity
    {
        public string Name { get; set; }
    }
}
