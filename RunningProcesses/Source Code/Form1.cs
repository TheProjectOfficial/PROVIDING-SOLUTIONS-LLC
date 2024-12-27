//application developed with the help of chatgpt. for more information
//visit https://theprojectofficial.github.io/providingsolutionsllc

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunningProcesses
{
    public partial class Form1 : Form
    {
        private Process selectedProcess;  // Store the selected process
        
        // Predefined list of system processes (you can add more here)
        private readonly string[] systemProcesses = new string[]
        {
            "explorer", "svchost", "winlogon", "csrss", "smss", "lsass", "taskhostw", "System", "Services", "Dwm",
            "Idle", "SearchIndexer", "wininit"
        };


        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void listBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected process name (extract the PID from the selected item)
            if (listBoxTasks.SelectedItem != null)
            {
                string selectedItem = listBoxTasks.SelectedItem.ToString();
                int pid = int.Parse(selectedItem.Split(new[] { "PID: " }, StringSplitOptions.None)[1].TrimEnd(')'));

                // Find the process by ID
                selectedProcess = Process.GetProcesses().FirstOrDefault(p => p.Id == pid);
            }
        }

        private void btnCloseApplication_Click(object sender, EventArgs e)
        {
            // Check if a process is selected
            if (selectedProcess != null)
            {
                try
                {
                    selectedProcess.Kill();  // Kill the selected process
                    MessageBox.Show($"Application {selectedProcess.ProcessName} closed successfully.");
                    UpdateRunningTasks();  // Refresh the task list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing application: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No application selected.");
            }
        }

        // Log errors to a file or handle silently
        private void CreateDirectory()
        {
            // Get the directory where the executable is located
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Specify the name of the new directory
            string newDirectoryName = "logs";

            // Combine the executable directory path with the new directory name
            string newDirectoryPath = Path.Combine(executableDirectory, newDirectoryName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(newDirectoryPath))
            {
                Directory.CreateDirectory(newDirectoryPath);
                Console.WriteLine($"Directory '{newDirectoryName}' created at {newDirectoryPath}");
            }
            else
            {
                Console.WriteLine($"Directory '{newDirectoryName}' already exists.");
            }
        }

        // Log errors to a file or handle silently
        private void LogError(Exception ex)
        {
            try
            {
                // Get the path of the executable folder
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Define the path for the 'logs' folder inside the executable folder
                string logFolder = Path.Combine(exeDirectory, "logs");

                // Ensure the logs folder exists
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                // Define the path for the log file (e.g., log.txt)
                string logFilePath = Path.Combine(logFolder, "log.txt");

                // Log the exception details
                using (var sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now}: {ex.Message}");
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine();
                }
            }
            catch (Exception logEx)
            {
                // If logging fails, you can also log to a secondary logging mechanism (e.g., Event Viewer or silent debug logging)
            }
        }

        // Method to update and filter the list of running tasks
        private void UpdateRunningTasks()
        {
            listBoxTasks.Items.Clear();

            try
            {
                // Add category titles
                listBoxTasks.Items.Add("Windows System Files:");
                listBoxTasks.Items.Add(""); // Empty line for separation

                // Get all running processes
                Process[] processes = Process.GetProcesses();

                // Filter and display system processes
                foreach (var process in processes)
                {
                    try
                    {
                        if (IsSystemProcess(process))
                        {
                            listBoxTasks.Items.Add($"{process.ProcessName} (PID: {process.Id})");
                        }
                    }
                    catch (AcessDeniedException)
                    {
                        // Skip system processes that throw access denied errors
                    }
                    catch (Exception ex)
                    {
                        // Log other unexpected exceptions but continue processing
                        LogError(ex);
                    }
                }

                // Add a space between categories
                listBoxTasks.Items.Add(""); // Empty line for separation

                listBoxTasks.Items.Add("Normal Processes:");
                listBoxTasks.Items.Add(""); // Empty line for separation

                // Filter and display normal processes
                foreach (var process in processes)
                {
                    try
                    {
                        if (!IsSystemProcess(process))
                        {
                            // Only show normal processes that match the search filter
                            if (string.IsNullOrEmpty(txtSearch.Text) || process.ProcessName.ToLower().Contains(txtSearch.Text.ToLower()))
                            {
                                listBoxTasks.Items.Add($"{process.ProcessName} (PID: {process.Id})");
                            }
                        }
                    }
                    catch (AcessDeniedException)
                    {
                        // Skip processes that throw access denied errors
                    }
                    catch (Exception ex)
                    {
                        // Log other unexpected exceptions but continue processing
                        LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception to a log file or handle silently without showing a message
                LogError(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateRunningTasks();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This application is still in testing phase" +
                " meaning, some features may not work as wanted, and you might get many exceptions when pressing random stuff.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            UpdateRunningTasks();
            CreateDirectory();

            this.MaximizeBox = false;
        }

        // Method to check if the process is a Windows system process
        private bool IsSystemProcess(Process process)
        {
            // Check if the process name is in the list of system processes
            return systemProcesses.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase);
        }

        // Helper function to close the selected process
        private void CloseSelectedProcess()
        {
            if (selectedProcess != null && !selectedProcess.HasExited)
            {
                try
                {
                    selectedProcess.Kill(); // Try to kill the process
                    MessageBox.Show($"Process {selectedProcess.ProcessName} (PID: {selectedProcess.Id}) has been closed.");
                    UpdateRunningTasks(); // Refresh the list after closing
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing the process: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No valid process selected or the process has already exited.");
            }
        }

        private void KillProcess()
        {
            try
            {
                selectedProcess.Kill();  // Kill the selected process
                MessageBox.Show($"Application {selectedProcess.ProcessName} closed.");
                UpdateRunningTasks();  // Refresh the task list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing application: {ex.Message}");
            }
        }


        private void btnCloseApplication_Click_1(object sender, EventArgs e)
        {
            CloseSelectedProcess();
        }

        private void listBoxTasks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBoxTasks.SelectedItem != null)
            {
                string selectedItem = listBoxTasks.SelectedItem.ToString();
                int pid = int.Parse(selectedItem.Split(new[] { "PID: " }, StringSplitOptions.None)[1].TrimEnd(')'));

                selectedProcess = Process.GetProcesses().FirstOrDefault(p => p.Id == pid);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateRunningTasks();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateRunningTasks(); // Refresh and filter the process list dynamically
        }

        private void btnOpenInExplorer_Click(object sender, EventArgs e)
        {
            OpenProcessInExplorer();
        }

        private void OpenProcessInExplorer()
        {
            if (selectedProcess != null)
            {
                try
                {
                    string processPath = GetProcessPath(selectedProcess);

                    if (!string.IsNullOrEmpty(processPath))
                    {
                        string directoryPath = Path.GetDirectoryName(processPath); // Get the directory containing the executable

                        // Open the directory in File Explorer
                        System.Diagnostics.Process.Start("explorer.exe", directoryPath);
                    }
                    else
                    {
                        MessageBox.Show("Could not retrieve the process path.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening process path: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No application selected.");
            }
        }

        // Method to safely get the process path
        private string GetProcessPath(Process process)
        {
            try
            {
                // Ensure the process is running and accessible
                if (process.HasExited)
                {
                    MessageBox.Show("The selected process has already exited.");
                    return null;
                }

                // Some processes (especially system processes) may throw exceptions
                if (IsSystemProcess(process))
                {
                    MessageBox.Show($"Cannot access system process path for: {process.ProcessName}");
                    return null;  // Skip system processes
                }

                // Attempt to access the process's main module (path)
                return process.MainModule.FileName;
            }
            catch (AcessDeniedException)
            {
                // Handle access denied error: this happens for protected system processes
                MessageBox.Show("Access denied to retrieve process path. Try running as Administrator.");
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                // Handle another access-related exception
                MessageBox.Show("Unauthorized access while retrieving process path.");
                return null;
            }
            catch (ArgumentException ex)
            {
                // This can happen if the process path is invalid (could be an argument issue with FileName)
                MessageBox.Show($"Invalid process path: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                MessageBox.Show($"Error retrieving process path: {ex.Message}");
                return null;
            }
        }

        [Serializable]
        private class AcessDeniedException : Exception
        {
            public AcessDeniedException()
            {
            }

            public AcessDeniedException(string message) : base(message)
            {
            }

            public AcessDeniedException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected AcessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedProcess != null)
            {
                // Get the process name
                string processName = selectedProcess.ProcessName;

                // Open the default browser to search for the process online (Google search)
                string searchUrl = $"https://www.google.com/search?q={processName}";
                System.Diagnostics.Process.Start(searchUrl); // Open the web browser
            }
            else
            {
                MessageBox.Show("No process selected to search online.");
            }
        }
    }
}
