using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.ValueSelection
{
    public interface IEntryProvider
    {
        Entry[] GetAllEntries( Type type );
    }
}