namespace DirectoryRepresentation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Input directory and base URL
            string inputPath = @"WRITE YOUR INPUT PATH HERE";
            string baseUrl = "https://www.example.com";

            // Generate and print the static class structure
            string result = findingPathV1.findPath(inputPath, baseUrl);
            //string result = findingPathV2.findPath(inputPath, baseUrl);
            //Console.WriteLine(result);

            string outputFilePath = Path.Combine(@"WRITE YOUR OUTPUT PATH HERE");

            try
            {
                // Save the output to a .cs file
                File.WriteAllText(outputFilePath, result);
                Console.WriteLine($"Output successfully saved to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
            }

        }
    }
}
