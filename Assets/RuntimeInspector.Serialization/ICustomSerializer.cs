using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Serialization
{
    public interface ICustomSerializer
    {
        public JToken WriteJson();
        public void PopulateJson( JToken json );
    }
}
