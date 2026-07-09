namespace J2N.Text.CodeGen.Generation
{
    public sealed class FacadeGenerationOptions
    {
        public required string FacadeName { get; init; }

        public bool IsSynchronized { get; init; }

        public bool EmitSynchronizationNotes { get; init; }

        public bool IsSealed { get; init; }
    }
}
