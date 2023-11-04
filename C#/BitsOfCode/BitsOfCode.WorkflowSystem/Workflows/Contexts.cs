using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsOfCode.WorkflowSystem.Workflows
{
    internal class GlobalContext
    {
        public string GlobalString { get; set; }
        public SubContext SubContext { get; set; }

    }

    internal class SubContext
    {
        public string SubString { get; set; }
    }
}
