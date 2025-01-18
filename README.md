Here is a detailed **README.md** for your project:

---

# SLM Demo for Alon Fliess's Lecture

This project is a demonstration for **Small Language Models (SLMs)**, part of the lecture by **Alon Fliess**. The demo showcases how to load, run, and interact with the **Phi-3.5-mini-instruct-onnx** model locally using C#. It highlights the capabilities of running a fine-tuned model for AI assistance tasks directly on a local machine without relying on cloud infrastructure.

---

## Features

- Automatically downloads the **Phi-3.5-mini-instruct-onnx** model if it's not already available locally.
- Loads the ONNX-based small language model for local inference.
- Tokenizes user queries and generates responses using a simple chat loop.
- Demonstrates inference with quantized models for optimal performance on CPU.
- Fully written in C# using the **Microsoft.ML.OnnxRuntimeGenAI** library.

---

## Project Workflow

### 1. **Setup Phase (Before the Loop)**
Before entering the chat loop, the program performs the following steps:

1. **Model Path Check**:
   - The program checks if the specified model directory exists.
   - If the model is not found, it downloads the necessary files from a Hugging Face repository and extracts them locally.

2. **Model Initialization**:
   - The ONNX model is loaded using the `Model` class, enabling the runtime to process queries efficiently.

3. **Tokenizer Initialization**:
   - A tokenizer is instantiated to convert textual prompts into tokenized input that the model can process.

4. **System Prompt Setup**:
   - A system-level prompt is defined to instruct the model about its role as an AI assistant.
   - This prompt ensures the model maintains a consistent tone and behavior.

---

### 2. **Main Chat Loop**
Inside the main loop, the program handles user queries and generates AI responses:

1. **Prompt Collection**:
   - The user provides input via the console.
   - If the input is empty, the program exits.

2. **Tokenization**:
   - The program combines the system prompt and user query into a single text string.
   - This string is tokenized into numerical representations that the model can understand.

3. **Response Generation**:
   - A `Generator` object is created and configured with parameters, including:
     - `max_length`: Maximum length of the response.
     - `past_present_share_buffer`: Optimization setting for memory usage.
   - The model processes the tokenized input and predicts the next token(s) iteratively.

4. **Decoding and Display**:
   - The generated tokens are decoded back into human-readable text.
   - The response is displayed to the user.

5. **Loop Continuation**:
   - The process repeats, allowing the user to ask additional questions until they exit.

---

## Functions and Methods Explained

### **Setup Phase**
- **`DownloadAndExtractModel(string url, string destinationPath)`**:
  - Downloads the model from the specified URL if it is not found locally.
  - Extracts the model files and prepares them for use.

- **`new Model(modelPath)`**:
  - Initializes the ONNX runtime model, enabling it to handle inference tasks.

- **`new Tokenizer(model)`**:
  - Creates a tokenizer for converting text into token sequences compatible with the model.

---

### **Chat Loop Functions**
- **`tokenizer.Encode(fullPrompt)`**:
  - Converts the combined system prompt and user query into numerical tokens.

- **`new GeneratorParams(model)`**:
  - Configures the generation parameters, such as:
    - Maximum length of the response.
    - Optimization settings for memory usage.

- **`generator.ComputeLogits()`**:
  - Processes the current token sequence to predict the next possible token.

- **`generator.GenerateNextToken()`**:
  - Advances the token generation by selecting the most probable next token.

- **`tokenizer.Decode(newToken)`**:
  - Converts the generated token back into human-readable text for display.

---

## Prerequisites

- **.NET SDK**: Ensure you have the latest .NET SDK installed.
- **Hugging Face Model Access**: The program downloads the Phi-3.5-mini-instruct-onnx model from Hugging Face. Ensure internet access is available during the initial run.
- **ONNX Runtime**: The `Microsoft.ML.OnnxRuntimeGenAI` library is required for model inference.

---

## How to Run

1. Clone this repository:
   ```bash
   git clone <repository_url>
   cd <repository_folder>
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build and run the project:
   ```bash
   dotnet run
   ```

4. Ask questions via the console, and the model will generate responses in real-time.

---

## Example Interaction

**Console Input**:
```
Q: What is the capital of France?
```

**Model Output**:
```
Phi3.5: The capital of France is Paris.
```

---

This project demonstrates the power and efficiency of Small Language Models for local AI tasks. Feel free to explore, modify, and extend the code! Let me know if you'd like further assistance.
