using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;

string sourceDirectory =
    @"F:\Projects\J2N\src\J2N\Text";

List<string> sourceTexts =
    Directory.GetFiles(sourceDirectory, "MutableTextBuffer*.cs")
        .Select(File.ReadAllText)
        .ToList();

var extractor = new RoslynTypeExtractor();

TypeModel model =
    extractor.Extract(
        sourceTexts,
        "J2N.Text.MutableTextBuffer");


var projection = new BuilderProjection();

ProjectedTypeModel projected =
    projection.Project(
        model,
        facadeNamespace: "J2N.Text",
        facadeName: "TextBuilder");

var emitter = new CSharpFacadeEmitter();

string code =
    emitter.EmitFacade(
        projected,
        backingFieldName: "buffer",
        options: new FacadeEmitterOptions
        {
            // J2N TODO: remove this once we have all of the docs
            SuppressMissingDocumentationWarnings = true,
        });

File.WriteAllText(
    @"F:\Projects\J2N\src\J2N\Text\TextBuilder.g.cs",
    code);

Console.WriteLine("Generation complete.");
