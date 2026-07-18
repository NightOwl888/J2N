using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text.CodeGen.Generation
{
    public sealed class ExtensionEmitterOptions
    {
        public bool WrapMembersInLock { get; init; }
        public bool EmitClassDocumentation { get; init; }
        public string? FacadeName { get; init; }
        public Accessibility ClassAccessibility { get; init; } = Accessibility.Public;
    }
}
