using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject
{
    class Group
    {
        public string name;
        public string domain;
        public int priority;

        public Group(string name, string domain)
        {
            this.name = name;
            this.domain = domain;
            this.priority = 1;
        }
    }
}
