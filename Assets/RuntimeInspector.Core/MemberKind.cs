using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Describes a specific 'kind' of member.
    /// </summary>
    public enum MemberKind : byte
    {
        Field,
        Property,
        Method,
        Event
    }
}