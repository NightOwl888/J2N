using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;

TypeModel model =
    MetadataLoader.Load(
        Path.Combine(
            AppContext.BaseDirectory,
            "Metadata",
            "MutableTextBuffer.json"));

var emitter = new CSharpFacadeEmitter();

string output =
    emitter.EmitFacade(
        model,
        "J2N.Text",
        "TextBuilder",
        "buffer");

string outputPath =
    Path.Combine(
        Environment.CurrentDirectory,
        "TextBuilder.g.cs");

File.WriteAllText(outputPath, output);

Console.WriteLine($"Generated: {outputPath}");
