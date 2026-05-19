using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen.Roslyn
{
    internal static class RoslynAttributeExtensions
    {
        public static bool HasAttribute(
            this ISymbol symbol,
            string fullyQualifiedMetadataName)
        {
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                INamedTypeSymbol? attributeClass =
                    attribute.AttributeClass;

                if (attributeClass is null)
                    continue;

                string fullName =
                    attributeClass.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat);

                //
                // Roslyn prepends "global::"
                //
                if (fullName.StartsWith("global::", StringComparison.Ordinal))
                {
                    fullName = fullName.Substring("global::".Length);
                }

                if (fullName == fullyQualifiedMetadataName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
