using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;

ApiModel api = MutableTextBufferApi.Create();

TypeModel mutableTextBuffer =
    api.Types.Single(t => t.Name == "MutableTextBuffer");

var emitter = new CSharpFacadeEmitter();

string code =
    emitter.EmitFacade(
        mutableTextBuffer,
        "J2N.Text",
        "TextBuilder",
        "buffer");

string outputPath =
    Path.Combine(
        AppContext.BaseDirectory,
        "TextBuilder.g.cs");

File.WriteAllText(outputPath, code);

Console.WriteLine("Generated:");
Console.WriteLine(outputPath);