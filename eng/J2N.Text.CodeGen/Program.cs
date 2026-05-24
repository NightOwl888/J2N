using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;
using Microsoft.CodeAnalysis;

namespace J2N.Text.CodGen
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string? sourceDirectory = null;
            string? infrastructureDirectory = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--source-directory":
                        sourceDirectory = args[++i];
                        break;

                    case "--infrastructure-directory":
                        infrastructureDirectory = args[++i];
                        break;
                }
            }

            if (sourceDirectory is null || infrastructureDirectory is null)
            {
                Console.Error.WriteLine(
                    "Usage: J2N.Text.CodeGen " +
                    "--source-directory <path> " +
                    "--infrastructure-directory <path>");

                return 1;
            }

            sourceDirectory =
                Path.GetFullPath(sourceDirectory);

            infrastructureDirectory =
                Path.GetFullPath(infrastructureDirectory);

            Console.WriteLine($"Source directory: {sourceDirectory}");
            Console.WriteLine($"Infrastructure directory: {infrastructureDirectory}");

            List<string> sourceTexts =
                Directory.GetFiles(sourceDirectory, "MutableTextBuffer*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
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

            // ---------------------------------------------------------------------
            // TextMemoryExtensions
            // ---------------------------------------------------------------------

            List<string> extensionSourceTexts =
                Directory.GetFiles(sourceDirectory, "TextMemoryExtensions*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

            TypeModel extensionModel =
                extractor.Extract(
                    extensionSourceTexts,
                    infrastructureTexts,
                    "J2N.Text.TextMemoryExtensions");

            // ---------------------------------------------------------------------
            // Generate TextBuilder
            // ---------------------------------------------------------------------

            GenerateFacade(
                sourceDirectory,
                facadeName: "TextBuilder",
                synchronized: false,
                emitSynchronizationNotes: false,
                model);

            GenerateExtensions(
                sourceDirectory,
                facadeName: "TextBuilder",
                emitSynchronizationNotes: false,
                extensionModel);

            // ---------------------------------------------------------------------
            // Generate PooledTextBuilder
            // ---------------------------------------------------------------------

            GenerateFacade(
                sourceDirectory,
                facadeName: "PooledTextBuilder",
                synchronized: false,
                emitSynchronizationNotes: false,
                model);

            GenerateExtensions(
                sourceDirectory,
                facadeName: "PooledTextBuilder",
                emitSynchronizationNotes: false,
                extensionModel);

            // ---------------------------------------------------------------------
            // Generate SynchronizedTextBuilder
            // ---------------------------------------------------------------------

            GenerateFacade(
                sourceDirectory,
                facadeName: "SynchronizedTextBuilder",
                synchronized: true,
                emitSynchronizationNotes: true,
                model);

            GenerateExtensions(
                sourceDirectory,
                facadeName: "SynchronizedTextBuilder",
                emitSynchronizationNotes: true,
                extensionModel);

            // ---------------------------------------------------------------------
            // Report completion
            // ---------------------------------------------------------------------

            Console.WriteLine("Generation complete.");

            return 0;
        }

        static void GenerateFacade(
            string sourceDirectory,
            string facadeName,
            bool synchronized,
            bool emitSynchronizationNotes,
            TypeModel model)
        {
            var projection = new BuilderProjection();
            var facadeEmitter = new CSharpFacadeEmitter();

            ProjectedTypeModel projected =
                projection.Project(
                    model,
                    facadeNamespace: "J2N.Text",
                    facadeName: facadeName,
                    options: new ProjectionOptions
                    {
                        EmitSynchronizationNotes = emitSynchronizationNotes,
                    });

            string facadeCode =
                facadeEmitter.EmitFacade(
                    projected,
                    backingFieldName: "buffer",
                    options: new FacadeEmitterOptions
                    {
                        SuppressMissingDocumentationWarnings = true,
                        WrapMembersInLock = synchronized,
                        EmitSynchronizationNotes = emitSynchronizationNotes,
                    });

            string facadePath =
                Path.Combine(
                    sourceDirectory,
                    $"{facadeName}.generated.cs");

            File.WriteAllText(
                facadePath,
                facadeCode);
        }

        static void GenerateExtensions(
            string sourceDirectory,
            string facadeName,
            bool emitSynchronizationNotes,
            TypeModel extensionModel)
        {
            var extensionProjection = new ExtensionMethodProjection();
            var extensionEmitter = new CSharpExtensionEmitter();

            ProjectedTypeModel projectedExtensions =
                extensionProjection.Project(
                    extensionModel,
                    "MutableTextBuffer",
                    facadeNamespace: "J2N.Text",
                    facadeType: facadeName,
                    options: new ProjectionOptions
                    {
                        EmitSynchronizationNotes = emitSynchronizationNotes,
                    });

            string extensionCode =
                extensionEmitter.Emit(projectedExtensions);

            string extensionPath =
                Path.Combine(
                    sourceDirectory,
                    $"TextMemoryExtensions.{facadeName}.generated.cs");

            File.WriteAllText(
                extensionPath,
                extensionCode);
        }
    }
}

