# Directory-Representation
This C# program generates a static class representing the contents of a user-selected root directory. The directory structure can be viewed in two formats:

1. **Flat Structure**: All files and subdirectories are listed without hierarchy.
2. **Tree-Structured Hierarchy**: The directory structure is represented hierarchically, with nested subdirectories.

---

## Usage

1. **Set Input and Output Paths**:
   - Replace `WRITE YOUR INPUT PATH HERE` with the full path to the root directory you want to analyze.
   - Replace `WRITE YOUR OUTPUT PATH HERE` with the path where you want the generated `.cs` file to be saved.

2. **Choose a View Mode**:
   - **Tree Structure**:
     ```csharp
     string result = findingPathV1.findPath(inputPath, baseUrl);
     ```
   - **Flat Structure**:
     ```csharp
     string result = findingPathV2.findPath(inputPath, baseUrl);
     ```

3. **Run the Program**:
   - Build and execute the program in your C# IDE (e.g., Visual Studio).
   - The generated class will be saved to the specified output path.

4. **Output**:
   - The `.cs` file contains a static class with properties corresponding to the directory structure and file paths.
