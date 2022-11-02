using RuntimeInspector.Core;
using RuntimeInspector.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI
{
    public static class InputConverterProvider
    {
        private static Dictionary<(Type, Type), object> availableConverters = new Dictionary<(Type, Type), object>();

        private static readonly Type baseConverterType = typeof( IConverter<,> );

        private const int INPUT_PARAM_INDEX = 0;
        private const int OUTPUT_PARAM_INDEX = 1;

        public static void ReloadConverters()
        { 
            availableConverters = new Dictionary<(Type, Type), object>();

            List<Type> converters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where(
                    dt => !dt.IsAbstract
                 && dt != baseConverterType
                 && dt.GetInterfaces().Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == baseConverterType ) )
                .ToList();

            foreach( var converterType in converters )
            {
                Type[] genericArguments = converterType.GetInterfaces().First().GenericTypeArguments;

                Type inType = genericArguments[INPUT_PARAM_INDEX];
                Type outType = genericArguments[OUTPUT_PARAM_INDEX];

                object converter = Activator.CreateInstance( converterType );

                availableConverters.Add( (inType, outType), converter );
            }
        }

        public static void AssignValue( MemberBinding binding, object incoming )
        {
            if( !availableConverters.Any() )
            {
                ReloadConverters();
            }

            Type inType = incoming.GetType();

            Type outType = binding.Metadata.Type;

            // When the types are the same, there is no need to convert.
            if( inType == outType )
            {
                binding.Binding.SetValue( incoming );
                return;
            }

            // TODO - Take into account base types if available??? Do we want that?

            availableConverters.TryGetValue( (inType, outType), out object converter );
            if( converter == null )
            {
                Debug.LogError( $"Converter for types '{inType}' -> '{outType}' not found." );
                return;
            }

            object outVal = converter.GetType().GetMethod( "Convert" ).Invoke( converter, new[] { incoming } );

            binding.Binding.SetValue( outVal );
        }
    }
}