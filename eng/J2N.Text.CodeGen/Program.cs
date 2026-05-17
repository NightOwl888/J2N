using J2N.Text.CodeGen.Metadata;
using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Projection;

TypeModel model =
    MetadataLoader.Load("Metadata/MutableTextBuffer.json");

var profile = new ProjectionProfile
{
    Name = "TextBuilder",
    ExcludedMembers = new HashSet<string>()
};

ProjectedTypeModel projected =
    ProjectionEngine.Project(model, profile);

var emitter = new CSharpFacadeEmitter();

string output =
    emitter.EmitFacade(
        projected,
        "buffer");

File.WriteAllText("TextBuilder.g.cs", output);

Console.WriteLine("Generated TextBuilder.g.cs");
