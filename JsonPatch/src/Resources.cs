﻿// <auto-generated>
using System.Reflection;


namespace Microsoft.AspNetCore.JsonPatch
{
    internal static partial class Resources
    {
        private static global::System.Resources.ResourceManager s_resourceManager;
        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(Resources)));
        internal static global::System.Globalization.CultureInfo Culture { get; set; }
#if !NET20
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal static string GetResourceString(string resourceKey, string defaultValue = null) => ResourceManager.GetString(resourceKey, Culture);

        private static string GetResourceString(string resourceKey, string[] formatterNames)
        {
            var value = GetResourceString(resourceKey);
            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }
            return value;
        }

        /// <summary>The property at '{0}' could not be copied.</summary>
        internal static string @CannotCopyProperty => GetResourceString("CannotCopyProperty");
        /// <summary>The property at '{0}' could not be copied.</summary>
        internal static string FormatCannotCopyProperty(object p0)
           => string.Format(Culture, GetResourceString("CannotCopyProperty"), p0);

        /// <summary>The type of the property at path '{0}' could not be determined.</summary>
        internal static string @CannotDeterminePropertyType => GetResourceString("CannotDeterminePropertyType");
        /// <summary>The type of the property at path '{0}' could not be determined.</summary>
        internal static string FormatCannotDeterminePropertyType(object p0)
           => string.Format(Culture, GetResourceString("CannotDeterminePropertyType"), p0);

        /// <summary>The '{0}' operation at path '{1}' could not be performed.</summary>
        internal static string @CannotPerformOperation => GetResourceString("CannotPerformOperation");
        /// <summary>The '{0}' operation at path '{1}' could not be performed.</summary>
        internal static string FormatCannotPerformOperation(object p0, object p1)
           => string.Format(Culture, GetResourceString("CannotPerformOperation"), p0, p1);

        /// <summary>The property at '{0}' could not be read.</summary>
        internal static string @CannotReadProperty => GetResourceString("CannotReadProperty");
        /// <summary>The property at '{0}' could not be read.</summary>
        internal static string FormatCannotReadProperty(object p0)
           => string.Format(Culture, GetResourceString("CannotReadProperty"), p0);

        /// <summary>The property at path '{0}' could not be updated.</summary>
        internal static string @CannotUpdateProperty => GetResourceString("CannotUpdateProperty");
        /// <summary>The property at path '{0}' could not be updated.</summary>
        internal static string FormatCannotUpdateProperty(object p0)
           => string.Format(Culture, GetResourceString("CannotUpdateProperty"), p0);

        /// <summary>The expression '{0}' is not supported. Supported expressions include member access and indexer expressions.</summary>
        internal static string @ExpressionTypeNotSupported => GetResourceString("ExpressionTypeNotSupported");
        /// <summary>The expression '{0}' is not supported. Supported expressions include member access and indexer expressions.</summary>
        internal static string FormatExpressionTypeNotSupported(object p0)
           => string.Format(Culture, GetResourceString("ExpressionTypeNotSupported"), p0);

        /// <summary>The index value provided by path segment '{0}' is out of bounds of the array size.</summary>
        internal static string @IndexOutOfBounds => GetResourceString("IndexOutOfBounds");
        /// <summary>The index value provided by path segment '{0}' is out of bounds of the array size.</summary>
        internal static string FormatIndexOutOfBounds(object p0)
           => string.Format(Culture, GetResourceString("IndexOutOfBounds"), p0);

        /// <summary>The path segment '{0}' is invalid for an array index.</summary>
        internal static string @InvalidIndexValue => GetResourceString("InvalidIndexValue");
        /// <summary>The path segment '{0}' is invalid for an array index.</summary>
        internal static string FormatInvalidIndexValue(object p0)
           => string.Format(Culture, GetResourceString("InvalidIndexValue"), p0);

        /// <summary>The JSON patch document was malformed and could not be parsed.</summary>
        internal static string @InvalidJsonPatchDocument => GetResourceString("InvalidJsonPatchDocument");
        /// <summary>Invalid JsonPatch operation '{0}'.</summary>
        internal static string @InvalidJsonPatchOperation => GetResourceString("InvalidJsonPatchOperation");
        /// <summary>Invalid JsonPatch operation '{0}'.</summary>
        internal static string FormatInvalidJsonPatchOperation(object p0)
           => string.Format(Culture, GetResourceString("InvalidJsonPatchOperation"), p0);

        /// <summary>The provided path segment '{0}' cannot be converted to the target type.</summary>
        internal static string @InvalidPathSegment => GetResourceString("InvalidPathSegment");
        /// <summary>The provided path segment '{0}' cannot be converted to the target type.</summary>
        internal static string FormatInvalidPathSegment(object p0)
           => string.Format(Culture, GetResourceString("InvalidPathSegment"), p0);

        /// <summary>The provided string '{0}' is an invalid path.</summary>
        internal static string @InvalidValueForPath => GetResourceString("InvalidValueForPath");
        /// <summary>The provided string '{0}' is an invalid path.</summary>
        internal static string FormatInvalidValueForPath(object p0)
           => string.Format(Culture, GetResourceString("InvalidValueForPath"), p0);

        /// <summary>The value '{0}' is invalid for target location.</summary>
        internal static string @InvalidValueForProperty => GetResourceString("InvalidValueForProperty");
        /// <summary>The value '{0}' is invalid for target location.</summary>
        internal static string FormatInvalidValueForProperty(object p0)
           => string.Format(Culture, GetResourceString("InvalidValueForProperty"), p0);

        /// <summary>'{0}' must be of type '{1}'.</summary>
        internal static string @ParameterMustMatchType => GetResourceString("ParameterMustMatchType");
        /// <summary>'{0}' must be of type '{1}'.</summary>
        internal static string FormatParameterMustMatchType(object p0, object p1)
           => string.Format(Culture, GetResourceString("ParameterMustMatchType"), p0, p1);

        /// <summary>The type '{0}' which is an array is not supported for json patch operations as it has a fixed size.</summary>
        internal static string @PatchNotSupportedForArrays => GetResourceString("PatchNotSupportedForArrays");
        /// <summary>The type '{0}' which is an array is not supported for json patch operations as it has a fixed size.</summary>
        internal static string FormatPatchNotSupportedForArrays(object p0)
           => string.Format(Culture, GetResourceString("PatchNotSupportedForArrays"), p0);

        /// <summary>The type '{0}' which is a non generic list is not supported for json patch operations. Only generic list types are supported.</summary>
        internal static string @PatchNotSupportedForNonGenericLists => GetResourceString("PatchNotSupportedForNonGenericLists");
        /// <summary>The type '{0}' which is a non generic list is not supported for json patch operations. Only generic list types are supported.</summary>
        internal static string FormatPatchNotSupportedForNonGenericLists(object p0)
           => string.Format(Culture, GetResourceString("PatchNotSupportedForNonGenericLists"), p0);

        /// <summary>The target location specified by path segment '{0}' was not found.</summary>
        internal static string @TargetLocationAtPathSegmentNotFound => GetResourceString("TargetLocationAtPathSegmentNotFound");
        /// <summary>The target location specified by path segment '{0}' was not found.</summary>
        internal static string FormatTargetLocationAtPathSegmentNotFound(object p0)
           => string.Format(Culture, GetResourceString("TargetLocationAtPathSegmentNotFound"), p0);

        /// <summary>For operation '{0}', the target location specified by path '{1}' was not found.</summary>
        internal static string @TargetLocationNotFound => GetResourceString("TargetLocationNotFound");
        /// <summary>For operation '{0}', the target location specified by path '{1}' was not found.</summary>
        internal static string FormatTargetLocationNotFound(object p0, object p1)
           => string.Format(Culture, GetResourceString("TargetLocationNotFound"), p0, p1);

        /// <summary>The test operation is not supported.</summary>
        internal static string @TestOperationNotSupported => GetResourceString("TestOperationNotSupported");
        /// <summary>The current value '{0}' at position '{2}' is not equal to the test value '{1}'.</summary>
        internal static string @ValueAtListPositionNotEqualToTestValue => GetResourceString("ValueAtListPositionNotEqualToTestValue");
        /// <summary>The current value '{0}' at position '{2}' is not equal to the test value '{1}'.</summary>
        internal static string FormatValueAtListPositionNotEqualToTestValue(object p0, object p1, object p2)
           => string.Format(Culture, GetResourceString("ValueAtListPositionNotEqualToTestValue"), p0, p1, p2);

        /// <summary>The value at '{0}' cannot be null or empty to perform the test operation.</summary>
        internal static string @ValueForTargetSegmentCannotBeNullOrEmpty => GetResourceString("ValueForTargetSegmentCannotBeNullOrEmpty");
        /// <summary>The value at '{0}' cannot be null or empty to perform the test operation.</summary>
        internal static string FormatValueForTargetSegmentCannotBeNullOrEmpty(object p0)
           => string.Format(Culture, GetResourceString("ValueForTargetSegmentCannotBeNullOrEmpty"), p0);

        /// <summary>The current value '{0}' at path '{2}' is not equal to the test value '{1}'.</summary>
        internal static string @ValueNotEqualToTestValue => GetResourceString("ValueNotEqualToTestValue");
        /// <summary>The current value '{0}' at path '{2}' is not equal to the test value '{1}'.</summary>
        internal static string FormatValueNotEqualToTestValue(object p0, object p1, object p2)
           => string.Format(Culture, GetResourceString("ValueNotEqualToTestValue"), p0, p1, p2);


    }
}
