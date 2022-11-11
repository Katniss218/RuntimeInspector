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

        public static bool TryConvertForward( Type outputType, Type inputType, object incoming, out object convertedValue )
        {
            // OutType equal to InType -> no conversion
            // OutType derived from InType -> no conversion
            // otherwise -> find a converter that best matches. direct inType->outType is the best, but inType.BaseType (recursive) will work too.
            if( _cachedConverters == null )
            {
                ReloadConverters();
            }

            // When the types are the same, there is no need to convert.
            if( outputType.IsAssignableFrom( inputType ) )
            {
                convertedValue = incoming;
                return true;
            }

            // TODO - Take into account base types if available??? Do we want that?
            object foundConverter;
            Type currentInputType = inputType;
            while( true )
            {
                _cachedConverters.TryGetValue( (currentInputType, outputType), out foundConverter );
                if( foundConverter != null )
                {
                    break;
                }

                if( currentInputType.BaseType == null && foundConverter == null )
                {
                    Debug.LogError( $"Converter for types '{currentInputType}' -> '{outputType}' not found." );

                    convertedValue = null;
                    return false;
                }
                currentInputType = currentInputType.BaseType;
            }

            try
            {
                convertedValue = foundConverter.GetType().GetMethod( CONVERT_FORWARD_METHOD_NAME ).Invoke( foundConverter, new[] { incoming } );
                return true;
            }
            catch
            {
                convertedValue = null;
                return false;
            }
        }
    }
}