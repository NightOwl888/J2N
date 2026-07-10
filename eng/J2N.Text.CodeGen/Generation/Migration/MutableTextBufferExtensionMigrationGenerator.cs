using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text.CodeGen.Generation.Migration
{
    internal static class MutableTextBufferExtensionMigrationGenerator
    {
        public static void Generate(
            string sourceDirectory,
            string infrastructureDirectory)
        {
            List<string> sourceTexts =
                Directory.GetFiles(sourceDirectory, "MutableTextBuffer*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Where(f => !f.StartsWith("MutableTextBufferExtensions", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

            List<string> infrastructureTexts =
                Directory.GetFiles(infrastructureDirectory, "*.cs")
                    .Select(File.ReadAllText)
                    .ToList();

            var extractor = new RoslynTypeExtractor();

            TypeModel model =
                extractor.Extract(
                    sourceTexts,
                    infrastructureTexts,
                    "J2N.Text.MutableTextBuffer");

            List<string> implementationSourceTexts =
                Directory.GetFiles(sourceDirectory, "MutableTextBuffer*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Where(f => !f.StartsWith("MutableTextBufferExtensions", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

            TypeModel implementationModel =
                extractor.Extract(
                    implementationSourceTexts,
                    infrastructureTexts,
                    "J2N.Text.MutableTextBuffer");


            var extensionProjection = new MutableTextBufferMigrationProjection();
            var extensionEmitter = new MutableTextBufferMigrationEmitter();

            ProjectedTypeModel projectedExtensions =
                extensionProjection.Project(
                    model,
                    implementationModel,
                    "MutableTextBuffer",
                    facadeNamespace: "J2N.Text",
                    projectedBuilderType: "MutableTextBuffer",
                    projectedTypeName: "MutableTextBufferExtensions",
                    options: new ProjectionOptions
                    {
                        EmitSynchronizationNotes = false,
                        PreserveSelfTypeGenerics = true,
                    });

            string extensionCode =
                extensionEmitter.Emit(
                    projectedExtensions,
                    options: new ExtensionEmitterOptions
                    {
                        WrapMembersInLock = false,
                    });

            string extensionPath =
                Path.Combine(
                    sourceDirectory,
                    $"MutableTextBufferExtensions.migration.generated.cs");

            File.WriteAllText(
                extensionPath,
                extensionCode);
        }
    }
}
