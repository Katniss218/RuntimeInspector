using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Holds instance-agnostic information about a member.
    /// <summary>
    public struct MemberMetadata : IEquatable<MemberMetadata>
    {
        /// <summary>
        /// Reflected name of the member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type that declared the member.
        /// </summary>
        /// <remarks>
        /// Useful when processing types with inheritance chains.
        /// </remarks>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// The type of the member.
        /// </summary>
        /// <remarks>
        /// When reflecting an 'int' field, the <see cref="Type"/> will be equal to 'typeof( int )'.
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        /// The kind of the member. See <see cref="MemberKind"></see> for more information.
        /// </summary>
        public MemberKind Kind { get; set; }

        /// <summary>
        /// If true, this member can be read from (false for write-only).
        /// </summary>
        public bool CanRead { get; set; }

        /// <summary>
        /// If true, this member can be written to (false for read-only).
        /// </summary>
        public bool CanWrite { get; set; }

        public bool Equals( MemberMetadata other )
        {
            return this.Name == other.Name
                && this.Type == other.Type
                && this.DeclaringType == other.DeclaringType;
        }

        /// <summary>
        /// Creates a MemberMetadata from a reflected field.
        /// </summary>
        public static MemberMetadata FromField( FieldInfo field )
        {
            return new MemberMetadata()
            {
                Name = field.Name,
                Type = field.FieldType,
                DeclaringType = field.DeclaringType,
                Kind = MemberKind.Field,
                CanRead = true,
                CanWrite = true
            };
        }

        /// <summary>
        /// Creates a MemberMetadata from a reflected property.
        /// </summary>
        public static MemberMetadata FromProperty( PropertyInfo property )
        {
            return new MemberMetadata()
            {
                Name = property.Name,
                Type = property.PropertyType,
                DeclaringType = property.DeclaringType,
                Kind = MemberKind.Property,
                CanRead = property.CanRead,
                CanWrite = property.CanWrite
            };
        }

        /// <summary>
        /// Creates a MemberMetadata from a reflected method.
        /// </summary>
        public static MemberMetadata FromMethod( MethodInfo method )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a MemberMetadata from a reflected event.
        /// </summary>
        public static MemberMetadata FromEvent( EventInfo @event )
        {
            throw new NotImplementedException();
        }
    }
}