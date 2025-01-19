using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntimeGenAI;

const int vkCtrl = 0x11;
const int vkCloseBracket = 0xDD;

var modelDirectory = @"C:\Dev\PhiModels\Phi-3.5-mini-instruct-onnx";
var gitRepoUrl = "https://huggingface.co/microsoft/Phi-3.5-mini-instruct-onnx";

// Check for GPU availability
bool isGpuAvailable = IsGpuAvailable();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(isGpuAvailable ? "GPU detected. Using GPU-optimized model." : "No GPU detected. Using CPU-optimized model.");
Console.ResetColor();

// Clone the repository if the directory does not exist
if (!Directory.Exists(modelDirectory))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Model directory for the selected version not found. Cloning the repository...");
    Console.ResetColor();
    if (CloneGitRepository(gitRepoUrl, modelDirectory))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Repository cloned successfully.");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Failed to clone the repository.");
        Console.ResetColor();
        return;
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Model directory for the selected version already exists. Skipping clone.");
    Console.ResetColor();
}

// Adjust modelPath based on GPU availability
var subdirectory = isGpuAvailable ? "gpu/gpu-int4-awq-block-128" : "cpu_and_mobile/cpu-int4-awq-block-128-acc-level-4";
var modelPath = Path.Combine(modelDirectory, subdirectory);

// Load the model and tokenizer
var model = new Model(modelPath);
var tokenizer = new Tokenizer(model);

var systemPrompt = "You are an AI assistant that helps people find information. Answer questions using a direct style. Do not share more information than requested by the users.";
var conversationHistory = $"<|system|>{systemPrompt}<|end|>";

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Ask your question. Type an empty string to Exit. Type 'CLH' to clear history and start a new session.");
Console.ResetColor();

// Chat loop
while (true)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("Q: ");
    Console.ResetColor();
    var userQ = Console.ReadLine();

    if (string.IsNullOrEmpty(userQ)) break;

    if (userQ.ToUpper() == "CLH")
    {
        conversationHistory = $"<|system|>{systemPrompt}<|end|>";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("History cleared. Starting a new session.");
        Console.ResetColor();
        continue;
    }

    conversationHistory += $"<|user|>{userQ}<|end|><|assistant|>";

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Phi3.5: ");
    Console.ResetColor();
    var tokens = tokenizer.Encode(conversationHistory);

    var generatorParams = new GeneratorParams(model);
    generatorParams.SetSearchOption("max_length", 2048);
    generatorParams.SetSearchOption("past_present_share_buffer", false);
    generatorParams.SetInputSequences(tokens);

    var generator = new Generator(model, generatorParams);

    while (!generator.IsDone())
    {
        if (IsUserInterrupted())
            break;

        generator.ComputeLogits();
        generator.GenerateNextToken();
        var outputTokens = generator.GetSequence(0);
        var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
        var output = tokenizer.Decode(newToken);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(output);

        Console.ResetColor();

        // Append the assistant's response to the conversation history
        conversationHistory += output;
    }
    Console.WriteLine();
}

static bool IsUserInterrupted()
{
    if ((GetAsyncKeyState(vkCtrl) & 0x8000) != 0 && (GetAsyncKeyState(vkCloseBracket) & 0x8000) != 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nPhi3.5 Answer interrupted by user.");
        Console.ResetColor();
        // Clear the input buffer
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true);
        }

        return true;
    }

    return false;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(output);
            Console.ResetColor();
            return true;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error during cloning: {error}");
        Console.ResetColor();
        return false;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Exception occurred while cloning: {ex.Message}");
        Console.ResetColor();
        return false;
    }
}

static bool IsGpuAvailable()
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "nvidia-smi",
                Arguments = "--query-gpu=name --format=csv,noheader",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return !string.IsNullOrWhiteSpace(output);
    }
    catch
    {
        return false;
    }
}

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);

