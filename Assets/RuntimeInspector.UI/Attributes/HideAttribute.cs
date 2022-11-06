using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Attributes
{
    /// <summary>
    /// Hides a member from the Runtime Inspector.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field
      | AttributeTargets.Property
      | AttributeTargets.Event
      | AttributeTargets.Method
      | AttributeTargets.Constructor, AllowMultiple = false )]
    public class HideAttribute : Attribute
    {
        // TODO - Figure out why having hidden field and serialized property makes it flicker.
        /*
            [SerializeField]
            [field: Hide]
            public MeshRenderer Renderer { get; set; }
        */
        public HideAttribute()
        {

        }
    }
}
