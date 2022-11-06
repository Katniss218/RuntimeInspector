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
        private static Dictionary<(Type, Type), object> _cachedConverters = null;

        private static readonly Type BASE_CONVERTER_TYPE = typeof( IConverter<,> );

        public const int INPUT_PARAM_INDEX = 0;
        public const int OUTPUT_PARAM_INDEX = 1;

        public const string CONVERT_FORWARD_METHOD_NAME = "ConvertForward";
        public const string CONVERT_REVERSE_METHOD_NAME = "ConvertReverse";

        private static List<Type> GetAllTypesImplementingGenericInterface( Type genericInterfaceType )
        {
            List<Type> converters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where(
                    dt => !dt.IsAbstract
                 && dt != genericInterfaceType
                 && dt.GetInterfaces().Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType ) )
                .ToList();

            return converters;
        }

        public static void ReloadConverters()
        {
            _cachedConverters = new Dictionary<(Type, Type), object>();

            List<Type> converters = GetAllTypesImplementingGenericInterface( BASE_CONVERTER_TYPE );

            foreach( var converterType in converters )
            {
                Type[] genericArguments = converterType.GetInterfaces().First().GenericTypeArguments;

                Type inType = genericArguments[INPUT_PARAM_INDEX];
                Type outType = genericArguments[OUTPUT_PARAM_INDEX];

                object converter = Activator.CreateInstance( converterType );

                _cachedConverters.Add( (inType, outType), converter );
            }
        }

        public static bool TryConvertForward( Type outType, object incoming, out object converted )
        {
            if( _cachedConverters == null )
            {
                ReloadConverters();
            }

            Type inType = incoming.GetType();

            // When the types are the same, there is no need to convert.
            if( outType.IsAssignableFrom( inType ) )
            {
                converted = incoming;
                return true;
            }

            // TODO - Take into account base types if available??? Do we want that?

            _cachedConverters.TryGetValue( (inType, outType), out object converter );
            if( converter == null )
            {
                Debug.LogError( $"Converter for types '{inType}' -> '{outType}' not found." );

                converted = null;
                return false;
            }

            try
            {
                converted = converter.GetType().GetMethod( CONVERT_FORWARD_METHOD_NAME ).Invoke( converter, new[] { incoming } );
            }
            catch
            {
                converted = null;
                return false;
            }

            return true;
        }

        public static bool TryConvertForward<T>( object incoming, out T converted )
        {
            if( TryConvertForward( typeof( T ), incoming, out object convertedInternal ) )
            {
                converted = (T)convertedInternal;
                return true;
            }

            converted = default;
            return false;
        }
    }
}