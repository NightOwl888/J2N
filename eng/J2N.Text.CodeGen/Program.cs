using J2N.Text.CodeGen.Generation;
using J2N.Text.CodeGen.Metadata;

ApiModel api = MutableTextBufferApi.Create();

TypeModel mutableTextBuffer =
    api.Types.Single(t => t.Name == "MutableTextBuffer");

var emitter = new CSharpFacadeEmitter();

string code =
    emitter.EmitFacade(
        mutableTextBuffer,
        "Lucene.Net.Analysis.Common",
        "OpenStringBuilder",
        "_buffer");

string outputPath =
    Path.Combine(
        AppContext.BaseDirectory,
        "OpenStringBuilder.g.cs");

File.WriteAllText(outputPath, code);

Console.WriteLine("Generated:");
Console.WriteLine(outputPath);