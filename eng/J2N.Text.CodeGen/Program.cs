using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Generation.Migration;
using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;
using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string? j2nSourceDirectory = null;
            string? sourceDirectory = null;
            string? infrastructureDirectory = null;
            bool migration = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--j2n-source-directory":
                        j2nSourceDirectory = args[++i];
                        break;

                    case "--source-directory":
                        sourceDirectory = args[++i];
                        break;

                    case "--infrastructure-directory":
                        infrastructureDirectory = args[++i];
                        break;

                    case "--migration":
                        migration = true;
                        break;
                }
            }

            if (j2nSourceDirectory is null || sourceDirectory is null || infrastructureDirectory is null)
            {
                Console.Error.WriteLine(
                    "Usage: J2N.Text.CodeGen " +
                    "--j2n-source-directory <path> " +
                    "--source-directory <path> " +
                    "--infrastructure-directory <path>");

                return 1;
            }

            j2nSourceDirectory =
                Path.GetFullPath(j2nSourceDirectory);

            sourceDirectory =
                Path.GetFullPath(sourceDirectory);

            infrastructureDirectory =
                Path.GetFullPath(infrastructureDirectory);

            Console.WriteLine($"J2N Source directory: {sourceDirectory}");
            Console.WriteLine($"J2N.Text Source directory: {sourceDirectory}");
            Console.WriteLine($"Infrastructure directory: {infrastructureDirectory}");

            if (migration)
            {
                MutableTextBufferExtensionMigrationGenerator.Generate(
                    sourceDirectory,
                    infrastructureDirectory);

                return 0;
            }

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
            // MutableTextBufferExtensions
            // ---------------------------------------------------------------------

            List<string> j2nExtensionSourceTexts =
                Directory.GetFiles(j2nSourceDirectory, "MutableTextBufferExtensions*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

            List<string> extensionSourceTexts =
                Directory.GetFiles(sourceDirectory, "MutableTextBufferExtensions*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

            TypeModel j2nExtensionModel =
                extractor.Extract(
                    j2nExtensionSourceTexts,
                    infrastructureTexts,
                    "J2N.MutableTextBufferExtensions");

            TypeModel extensionModel =
                extractor.Extract(
                    extensionSourceTexts,
                    infrastructureTexts,
                    "J2N.Text.MutableTextBufferExtensions");

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

            // ---------------------------------------------------------------------
            // Generate TextBuilder
            // ---------------------------------------------------------------------

            FacadeGenerationOptions textBuilderOptions = new()
            {
                FacadeName = "TextBuilder",
                ClassAccessibility = Accessibility.Public,
                IsSynchronized = false,
                EmitSynchronizationNotes = false,
                IsSealed = false,
            };

            GenerateFacade(
                sourceDirectory,
                textBuilderOptions,
                model);

            GenerateExtensions(
                j2nSourceDirectory,
                facadeNamespace: "J2N",
                emitClassDocumentation: true,
                textBuilderOptions,
                j2nExtensionModel,
                implementationModel);

            GenerateExtensions(
                sourceDirectory,
                facadeNamespace: "J2N.Text",
                emitClassDocumentation: false,
                textBuilderOptions,
                extensionModel,
                implementationModel);

            // ---------------------------------------------------------------------
            // Generate SynchronizedTextBuilder
            // ---------------------------------------------------------------------

            FacadeGenerationOptions synchronizedTextBuilderOptions = new()
            {
                FacadeName = "SynchronizedTextBuilder",
                ClassAccessibility = Accessibility.Internal,
                IsSynchronized = true,
                EmitSynchronizationNotes = true,
                IsSealed = false,
            };

            GenerateFacade(
                sourceDirectory,
                synchronizedTextBuilderOptions,
                model);

            GenerateExtensions(
                j2nSourceDirectory,
                facadeNamespace: "J2N",
                emitClassDocumentation: true,
                synchronizedTextBuilderOptions,
                j2nExtensionModel,
                implementationModel);

            GenerateExtensions(
                sourceDirectory,
                facadeNamespace: "J2N.Text",
                emitClassDocumentation: false,
                synchronizedTextBuilderOptions,
                extensionModel,
                implementationModel);

            // ---------------------------------------------------------------------
            // Report completion
            // ---------------------------------------------------------------------

            Console.WriteLine("Generation complete.");

            return 0;
        }

        static void GenerateFacade(
            string outputDirectory,
            FacadeGenerationOptions generationOptions,
            TypeModel model)
        {
            var projection = new BuilderProjection();
            var facadeEmitter = new CSharpFacadeEmitter();

            ProjectedTypeModel projected =
                projection.Project(
                    model,
                    facadeNamespace: "J2N.Text",
                    facadeName: generationOptions.FacadeName,
                    options: new ProjectionOptions
                    {
                        EmitSynchronizationNotes = generationOptions.EmitSynchronizationNotes,
                    });

            string facadeCode =
                facadeEmitter.EmitFacade(
                    projected,
                    backingFieldName: "buffer",
                    options: new FacadeEmitterOptions
                    {
                        SuppressMissingDocumentationWarnings = true,
                        WrapMembersInLock = generationOptions.IsSynchronized,
                        EmitSynchronizationNotes = generationOptions.EmitSynchronizationNotes,
                        IsSealed = generationOptions.IsSealed,
                        ClassAccessibility = generationOptions.ClassAccessibility,
                    });

            string facadePath =
                Path.Combine(
                    outputDirectory,
                    $"{generationOptions.FacadeName}.generated.cs");

            File.WriteAllText(
                facadePath,
                facadeCode);
        }

        static void GenerateExtensions(
            string outputDirectory,
            string facadeNamespace,
            bool emitClassDocumentation,
            FacadeGenerationOptions generationOptions,
            TypeModel extensionModel,
            TypeModel implementationModel)
        {
            var extensionProjection = new ExtensionMethodProjection();
            var extensionEmitter = new CSharpExtensionEmitter();

            ProjectedTypeModel projectedExtensions =
                extensionProjection.Project(
                    extensionModel,
                    implementationModel,
                    "MutableTextBuffer",
                    facadeNamespace,
                    projectedBuilderType: generationOptions.FacadeName,
                    projectedTypeName: generationOptions.FacadeName + "Extensions",
                    options: new ProjectionOptions
                    {
                        EmitSynchronizationNotes = generationOptions.EmitSynchronizationNotes,
                        PreserveSelfTypeGenerics = !generationOptions.IsSealed,
                    });

            string extensionCode =
                extensionEmitter.Emit(
                    projectedExtensions,
                    options: new ExtensionEmitterOptions
                    {
                        WrapMembersInLock = generationOptions.IsSynchronized,
                        EmitClassDocumentation = emitClassDocumentation,
                        FacadeName = generationOptions.FacadeName,
                        ClassAccessibility = generationOptions.ClassAccessibility,
                    });

            string extensionPath =
                Path.Combine(
                    outputDirectory,
                    $"{generationOptions.FacadeName}Extensions.generated.cs");

            File.WriteAllText(
                extensionPath,
                extensionCode);
        }
    }
}

