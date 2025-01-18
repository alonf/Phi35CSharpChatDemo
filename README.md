Here's the updated **README.md** incorporating the latest changes to the project, including GPU detection and the combined repository structure:

---

# SLM Demo for Alon Fliess's Lecture

This project is a demonstration for **Small Language Models (SLMs)**, presented as part of a lecture by **Alon Fliess**. It highlights how to load and interact with the **Phi-3.5-mini-instruct-onnx** model locally using C#. The demo dynamically determines whether to use GPU or CPU for model inference, ensuring optimal performance on the available hardware.

---

## Features

- Automatically clones the Hugging Face repository for **Phi-3.5-mini-instruct-onnx** if not found locally.
- Dynamically selects GPU or CPU models based on hardware availability.
- Demonstrates ONNX-based inference for efficient AI tasks.
- Maintains conversational history and allows users to clear history during the session.
- Fully implemented in C# using the **Microsoft.ML.OnnxRuntimeGenAI** library.

---

## Project Workflow

### 1. **Setup Phase (Before the Loop)**

1. **Model Directory Check**:
   - The program checks if the model repository exists locally.
   - If not found, it clones the **Phi-3.5-mini-instruct-onnx** repository from Hugging Face.

2. **Hardware Detection**:
   - Detects whether a GPU is available using the `nvidia-smi` command.
   - Selects the GPU-optimized or CPU-optimized model path accordingly.

3. **Model and Tokenizer Initialization**:
   - Loads the ONNX model for either GPU or CPU.
   - Initializes a tokenizer to process user input and model responses.

4. **System Prompt Setup**:
   - A system-level prompt is defined to instruct the model on its role as an AI assistant.
   - This ensures consistent and concise responses.

---

### 2. **Main Chat Loop**

1. **User Input**:
   - The user inputs queries through the console.
   - Special commands:
     - `CLH`: Clears the conversation history and starts a new session.
     - Empty input: Exits the program.

2. **History Management**:
   - Keeps a running history of the conversation, allowing the model to generate context-aware responses.

3. **Response Generation**:
   - The program tokenizes the combined system prompt, user input, and history.
   - Uses the ONNX model to generate responses token-by-token.

4. **Output**:
   - Decodes the model's output tokens into human-readable text and displays the response.

---

## Functions and Methods Explained

### **Setup Phase**
- **`CloneGitRepository(string repoUrl, string destinationPath)`**:
  - Clones the model repository from Hugging Face if not already present locally.
  - Handles errors during the cloning process.

- **`IsGpuAvailable()`**:
  - Checks for GPU availability using the `nvidia-smi` command.
  - Returns `true` if a GPU is detected.

---

### **Chat Loop Functions**
- **`tokenizer.Encode(fullPrompt)`**:
  - Tokenizes the combined system prompt, user query, and conversation history.

- **`new GeneratorParams(model)`**:
  - Configures the generation parameters for the model, including:
    - Maximum response length (`max_length`).
    - Optimization for memory usage.

- **`generator.ComputeLogits()`**:
  - Processes the tokenized input to calculate the next possible tokens.

- **`generator.GenerateNextToken()`**:
  - Advances the generation process by selecting the next most probable token.

- **`tokenizer.Decode(newToken)`**:
  - Converts the generated token into human-readable text.

---

## Prerequisites

- **.NET SDK**: Ensure the latest .NET SDK is installed.
- **ONNX Runtime**: Required for running the models locally.
- **Hugging Face Repository Access**: Internet access is needed for the initial repository cloning.
- **GPU Support**: A compatible NVIDIA GPU is recommended but not required.

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

4. Interact with the AI assistant:
   - Ask questions via the console.
   - Use the `CLH` command to clear history.

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

## Technical Details

### Repository Structure
The **Phi-3.5-mini-instruct-onnx** repository contains both GPU and CPU versions:

- **GPU**: Located in `gpu/gpu-int4-awq-block-128`.
- **CPU**: Located in `cpu_and_mobile/cpu-int4-awq-block-128-acc-level-4`.

### Dynamic Path Selection
- The program detects the hardware and selects the appropriate subdirectory.
- GPU model path: `gpu/gpu-int4-awq-block-128`.
- CPU model path: `cpu_and_mobile/cpu-int4-awq-block-128-acc-level-4`.

---

This project is designed to showcase the capabilities of Small Language Models and their efficiency in local AI inference tasks. It demonstrates how lightweight models can be effectively utilized in real-world scenarios. Feel free to explore and adapt the code for your own use cases.
