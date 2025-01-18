using System.IO.Compression;
using Microsoft.ML.OnnxRuntimeGenAI;

var modelPath = @"C:\Dev\PhiModels\Phi-3.5-mini-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32";
var modelUrl = "https://huggingface.co/microsoft/Phi-3.5-mini-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32.zip";

// Check if model exists locally
if (!Directory.Exists(modelPath))
{
    Console.WriteLine("Model not found locally. Downloading...");
    await DownloadAndExtractModel(modelUrl, modelPath);
}

var model = new Model(modelPath);
var tokenizer = new Tokenizer(model);

var systemPrompt = "You are an AI assistant that helps people find information. Answer questions using a direct style. Do not share more information than requested by the users.";

Console.WriteLine("Ask your question. Type an empty string to Exit.");

// Chat loop
while (true)
{
    Console.WriteLine();
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ)) break;

    Console.Write("Phi3: ");
    var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{userQ}<|end|><|assistant|>";
    var tokens = tokenizer.Encode(fullPrompt);

    var generatorParams = new GeneratorParams(model);
    generatorParams.SetSearchOption("max_length", 2048);
    generatorParams.SetSearchOption("past_present_share_buffer", false);
    generatorParams.SetInputSequences(tokens);

    var generator = new Generator(model, generatorParams);
    while (!generator.IsDone())
    {
        generator.ComputeLogits();
        generator.GenerateNextToken();
        var outputTokens = generator.GetSequence(0);
        var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
        var output = tokenizer.Decode(newToken);
        Console.Write(output);
    }
    Console.WriteLine();
}


async Task DownloadAndExtractModel(string url, string destinationPath)
{
    var tempFile = Path.Combine(Path.GetTempPath(), "model.zip");

    using (HttpClient client = new HttpClient())
    {
        using (var response = await client.GetAsync(url))
        {
            response.EnsureSuccessStatusCode();
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }

    // Extract the downloaded zip file
    ZipFile.ExtractToDirectory(tempFile, destinationPath);

    // Clean up the temporary file
    File.Delete(tempFile);

    Console.WriteLine($"Model downloaded and extracted to: {destinationPath}");
}
