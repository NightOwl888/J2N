using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Projection;
using J2N.Text.CodeGen.Roslyn;

string source =
    File.ReadAllText(
        @"F:\Projects\J2N\src\J2N\Text\MutableTextBuffer.cs");

var extractor = new RoslynTypeExtractor();

var model = extractor.Extract(source);

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
        backingFieldName: "buffer");

File.WriteAllText(
    @"F:\Projects\J2N\src\J2N\Text\TextBuilder.g.cs",
    code);

Console.WriteLine("Generation complete.");

//using J2N.Text.CodeGen.Metadata;
//using J2N.Text.CodeGen.Generation;
//using J2N.Text.CodeGen.Projection;

//TypeModel model =
//    MetadataLoader.Load("Metadata/MutableTextBuffer.json");

//var profile = new ProjectionProfile
//{
//    Name = "TextBuilder",
//    ExcludedMembers = new HashSet<string>()
//};

//ProjectedTypeModel projected =
//    ProjectionEngine.Project(model, profile);

//var emitter = new CSharpFacadeEmitter();

//string output =
//    emitter.EmitFacade(
//        projected,
//        "buffer");

//File.WriteAllText("TextBuilder.g.cs", output);

//Console.WriteLine("Generated TextBuilder.g.cs");
