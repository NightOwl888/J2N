using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen.Generation
{
    public sealed class FacadeEmitterOptions
    {
        public bool SuppressMissingDocumentationWarnings { get; init; }
        public bool WrapMembersInLock { get; init; }
        public bool EmitSynchronizationNotes { get; init; }
        public bool IsSealed { get; init; } = true;
        public Accessibility ClassAccessibility { get; init; } = Accessibility.Public;
    }
}
