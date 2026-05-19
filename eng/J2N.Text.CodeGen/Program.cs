using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;

string sourceDirectory =
    @"F:\Projects\J2N\src\J2N\Text";
string infrastructureDirectory =
    @"F:\Projects\J2N\src\J2N\CodeGeneration";

// ---------------------------------------------------------------------
// MutableTextBuffer
// ---------------------------------------------------------------------

List<string> sourceTexts =
    Directory.GetFiles(sourceDirectory, "MutableTextBuffer*.cs")
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

var projection = new BuilderProjection();

ProjectedTypeModel projected =
    projection.Project(
        model,
        facadeNamespace: "J2N.Text",
        facadeName: "TextBuilder");

var facadeEmitter = new CSharpFacadeEmitter();

string builderCode =
    facadeEmitter.EmitFacade(
        projected,
        backingFieldName: "buffer",
        options: new FacadeEmitterOptions
        {
            // J2N TODO: remove this once docs are complete
            SuppressMissingDocumentationWarnings = true,
        });

File.WriteAllText(
    Path.Combine(sourceDirectory, "TextBuilder.g.cs"),
    builderCode);

// ---------------------------------------------------------------------
// Generate TextMemoryExtensions.TextBuilder.g.cs
// ---------------------------------------------------------------------

var extensionProjection = new ExtensionMethodProjection();

ProjectedTypeModel projectedExtensions =
    extensionProjection.Project(
        extensionModel,
        "MutableTextBuffer",
        facadeNamespace: "J2N.Text",
        facadeType: "TextBuilder");

var extensionEmitter = new CSharpExtensionEmitter();

string extensionCode =
    extensionEmitter.Emit(
        projectedExtensions);

File.WriteAllText(
    Path.Combine(sourceDirectory, "TextMemoryExtensions.TextBuilder.g.cs"),
    extensionCode);

Console.WriteLine("Generation complete.");