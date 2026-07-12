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
            string? sourceDirectory = null;
            string? infrastructureDirectory = null;
            bool migration = false;

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

                    case "--migration":
                        migration = true;
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

            List<string> extensionSourceTexts =
                Directory.GetFiles(sourceDirectory, "MutableTextBufferExtensions*.cs")
                    .Where(f => !f.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                    .Select(File.ReadAllText)
                    .ToList();

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
                IsSynchronized = false,
                EmitSynchronizationNotes = false,
                IsSealed = false,
            };

            GenerateFacade(
                sourceDirectory,
                textBuilderOptions,
                model);

            GenerateExtensions(
                sourceDirectory,
                textBuilderOptions,
                extensionModel,
                implementationModel);

            // ---------------------------------------------------------------------
            // Generate PooledTextBuilder
            // ---------------------------------------------------------------------

            FacadeGenerationOptions pooledTextBuilderOptions = new()
            {
                FacadeName = "PooledTextBuilder",
                IsSynchronized = false,
                EmitSynchronizationNotes = false,
                IsSealed = true,
            };

            GenerateFacade(
                sourceDirectory,
                pooledTextBuilderOptions,
                model);


            GenerateExtensions(
                sourceDirectory,
                pooledTextBuilderOptions,
                extensionModel,
                implementationModel);

            // ---------------------------------------------------------------------
            // Generate SynchronizedTextBuilder
            // ---------------------------------------------------------------------

            FacadeGenerationOptions synchronizedTextBuilderOptions = new()
            {
                FacadeName = "SynchronizedTextBuilder",
                IsSynchronized = true,
                EmitSynchronizationNotes = true,
                IsSealed = false,
            };

            GenerateFacade(
                sourceDirectory,
                synchronizedTextBuilderOptions,
                model);

            GenerateExtensions(
                sourceDirectory,
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
            string sourceDirectory,
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
                    });

            string facadePath =
                Path.Combine(
                    sourceDirectory,
                    $"{generationOptions.FacadeName}.generated.cs");

            File.WriteAllText(
                facadePath,
                facadeCode);
        }

        static void GenerateExtensions(
            string sourceDirectory,
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
                    facadeNamespace: "J2N.Text",
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
                    });

            string extensionPath =
                Path.Combine(
                    sourceDirectory,
                    $"{generationOptions.FacadeName}Extensions.generated.cs");

            File.WriteAllText(
                extensionPath,
                extensionCode);
        }
    }
}

