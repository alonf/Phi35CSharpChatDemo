using System.Diagnostics;
using System.IO;
using Microsoft.ML.OnnxRuntimeGenAI;

var modelDirectory = @"C:\Dev\PhiModels\Phi-3.5-mini-instruct-onnx";
var gitRepoUrl = "https://huggingface.co/microsoft/Phi-3.5-mini-instruct-onnx";

// Clone the repository if the directory does not exist
if (!Directory.Exists(modelDirectory))
{
    Console.WriteLine("Model directory not found. Cloning the repository...");
    if (CloneGitRepository(gitRepoUrl, modelDirectory))
    {
        Console.WriteLine("Repository cloned successfully.");
    }
    else
    {
        Console.WriteLine("Failed to clone the repository.");
        return;
    }
}
else
{
    Console.WriteLine("Model directory already exists. Skipping clone.");
}

// Load the model and tokenizer
var modelPath = Path.Combine(modelDirectory, "cpu_and_mobile", "cpu-int4-awq-block-128-acc-level-4");
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

    Console.Write("Phi3.5: ");
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

static bool CloneGitRepository(string repoUrl, string destinationPath)
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {repoUrl} \"{destinationPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine(output);
            return true;
        }
        else
        {
            Console.WriteLine($"Error during cloning: {error}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred while cloning: {ex.Message}");
        return false;
    }
}
