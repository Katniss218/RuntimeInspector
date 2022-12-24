using RuntimeEditor.Core;
using RuntimeEditor.UI.Inspector.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeEditor.UI.Inspector
{
    public static class Converter
    {
        private static Dictionary<(Type, Type), object> _cachedConverters = null;

        private static readonly Type BASE_CONVERTER_TYPE = typeof( IConverter<,> );

        /// <summary>
        /// The index of the generic parameter corresponding to the input type.
        /// </summary>
        public const int INPUT_GENERIC_PARAM_INDEX = 0;

        /// <summary>
        /// The index of the generic parameter corresponding to the output type.
        /// </summary>
        public const int OUTPUT_GENERIC_PARAM_INDEX = 1;

        /// <summary>
        /// Name of the conversion method for the Forward conversion path.
        /// </summary>
        public const string CONVERT_FORWARD_METHOD_NAME = "ConvertForward";

        /// <summary>
        /// Name of the conversion method for the Reverse conversion path.
        /// </summary>
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

        /// <summary>
        /// Converts a value from outputType to inputType (using the Forward conversion path).
        /// </summary>
        public static bool TryConvertForward( Type outputType, Type inputType, object incoming, out object convertedValue )
        {
            return TryConvertInternal( false, outputType, inputType, incoming, out convertedValue );
        }

        /// <summary>
        /// Converts a value from inputType to outputType (using the Reverse conversion path).
        /// </summary>
        public static bool TryConvertReverse( Type outputType, Type inputType, object incoming, out object convertedValue )
        {
            // converting from output to input.
            return TryConvertInternal( true, outputType, inputType, incoming, out convertedValue );
        }

        private static void ReloadConverters()
        {
            _cachedConverters = new Dictionary<(Type, Type), object>();

            List<Type> converters = GetAllTypesImplementingGenericInterface( BASE_CONVERTER_TYPE );

            foreach( var converterType in converters )
            {
                Type[] genericArguments = converterType.GetInterfaces().First().GenericTypeArguments;

                Type inType = genericArguments[INPUT_GENERIC_PARAM_INDEX];
                Type outType = genericArguments[OUTPUT_GENERIC_PARAM_INDEX];

                object converter = Activator.CreateInstance( converterType );

                _cachedConverters.Add( (inType, outType), converter );
            }
        }

        private static object GetConverter( Type outputType, Type inputType )
        {
            object foundConverter;
            Type currentOutputType = outputType;
            while( true )
            {
                _cachedConverters.TryGetValue( (inputType, currentOutputType), out foundConverter );
                if( foundConverter != null )
                {
                    return foundConverter;
                }

                if( currentOutputType.BaseType == null && foundConverter == null )
                {
                    return null;
                }
                currentOutputType = currentOutputType.BaseType;
            }
        }

        private static bool TryConvertInternal( bool backwards, Type outputType, Type inputType, object incoming, out object convertedValue )
        {
            if( outputType == inputType )
            {
                convertedValue = incoming;
                return true;
            }

            if( _cachedConverters == null )
            {
                ReloadConverters();
            }

            string methodName;
            object[] parameters ;
            if( backwards )
            {
                // reversed check because we're assigning the other way.
                if( inputType.IsAssignableFrom( outputType ) )
                {
                    convertedValue = incoming;
                    return true;
                }
                methodName = CONVERT_REVERSE_METHOD_NAME;
                parameters = new[] { incoming };
            }
            else
            {
                if( outputType.IsAssignableFrom( inputType ) )
                {
                    convertedValue = incoming;
                    return true;
                }
                methodName = CONVERT_FORWARD_METHOD_NAME;
                parameters = new[] { outputType, incoming };
            }

            object converter = GetConverter( outputType, inputType );
            if( converter == null )
            {
                Debug.LogWarning( $"Converter for '{inputType}' -> '{outputType}' not found." );
                convertedValue = default;
                return false;
            }

            try
            {
                MethodInfo method = converter.GetType().GetMethod( methodName );

                convertedValue = method.Invoke( converter, parameters );
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