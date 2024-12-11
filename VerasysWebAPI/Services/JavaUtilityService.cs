using System.Diagnostics;

namespace VerasysWebAPI.Services
{
    public class JavaUtilityService
    {
        public void StartJavaService()
        {
            try
            {
                // Configure the process start information
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Java\jre1.8.0_431\bin\java.exe", // Java executable
                    Arguments = @"-jar ""D:\Verasys\VerasyseSign2.1UtilityService4.1.5\VerasyseSign2.1UtilityService4.1.5\Verasays-eSign-Web-4.1.5.jar""",
                    RedirectStandardOutput = true, // Capture standard output
                    RedirectStandardError = true,  // Capture error output
                    UseShellExecute = false,       // Required for redirection
                    CreateNoWindow = true          // Run without a command prompt window
                };

                // Start the process and manage resources
                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    // Read the output asynchronously to avoid blocking
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Wait for the process to exit
                    process.WaitForExit();

                    // Log the output and error
                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Java Output: " + output);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Java Error: " + error);
                    }

                    // Check the exit code for success/failure
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Java process exited with code {process.ExitCode}. Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting Java service: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }
    }
}
