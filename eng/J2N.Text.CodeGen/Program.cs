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
            if (args.Length != 2)
            {
                Console.Error.WriteLine(
                    "Usage: J2N.Text.CodeGen <SourceDirectory> <InfrastructureDirectory>");

                return 1;
            }

            string sourceDirectory = Path.GetFullPath(args[0]);
            string infrastructureDirectory = Path.GetFullPath(args[1]);

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
                model);

            GenerateExtensions(
                sourceDirectory,
                facadeName: "TextBuilder",
                extensionModel);

            // ---------------------------------------------------------------------
            // Generate PooledTextBuilder
            // ---------------------------------------------------------------------

            GenerateFacade(
                sourceDirectory,
                facadeName: "PooledTextBuilder",
                synchronized: false,
                model);

            GenerateExtensions(
                sourceDirectory,
                facadeName: "PooledTextBuilder",
                extensionModel);

            // ---------------------------------------------------------------------
            // Generate SynchronizedTextBuilder
            // ---------------------------------------------------------------------

            GenerateFacade(
                sourceDirectory,
                facadeName: "SynchronizedTextBuilder",
                synchronized: true,
                model);

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
            TypeModel model)
        {
            var projection = new BuilderProjection();
            var facadeEmitter = new CSharpFacadeEmitter();

            ProjectedTypeModel projected =
                projection.Project(
                    model,
                    facadeNamespace: "J2N.Text",
                    facadeName: facadeName);

            string facadeCode =
                facadeEmitter.EmitFacade(
                    projected,
                    backingFieldName: "buffer",
                    options: new FacadeEmitterOptions
                    {
                        SuppressMissingDocumentationWarnings = true,
                        WrapMembersInLock = synchronized
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
            TypeModel extensionModel)
        {
            var extensionProjection = new ExtensionMethodProjection();
            var extensionEmitter = new CSharpExtensionEmitter();

            ProjectedTypeModel projectedExtensions =
                extensionProjection.Project(
                    extensionModel,
                    "MutableTextBuffer",
                    facadeNamespace: "J2N.Text",
                    facadeType: facadeName);

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

