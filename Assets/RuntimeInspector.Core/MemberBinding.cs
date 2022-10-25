using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Groups the metadata and binding together.
    /// </summary>
    public struct MemberBinding
    {
        /// <summary>
        /// Contains additional instance-agnostic informaiton about the member.
        /// </summary>
        public MemberMetadata Metadata { get; set; }

        /// <summary>
        /// Can return the bound object and instance-dependent information.
        /// </summary>
        public IObjectBinding Binding { get; set; }
    }
}