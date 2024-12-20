//Application Developed With the help of ChatGPT.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Speech.Synthesis;

namespace AdvancedTextEditor
{
    public partial class Form1 : Form
    {
        private string savesFolderPath;
        private string filePath;

        // Timer to track the key hold duration
        private Timer keyHoldTimer;
        private DateTime keyPressTime;
        private bool isKeyPressed;

        // Create an instance of SpeechSynthesizer
        private SpeechSynthesizer synthesizer;
        private ColorDialog colorDialogBackground;
        private ColorDialog colorDialogText;

        public Form1()
        {
            InitializeComponent();

            // Initialize the SpeechSynthesizer object
            synthesizer = new SpeechSynthesizer();

            // Populate the ComboBox with available voices
            PopulateVoices();

            // Initialize the ColorDialog control
            colorDialogBackground = new ColorDialog();

            // Initialize the ColorDialog control
            colorDialogText = new ColorDialog();

            // Set the form to capture key events
            this.KeyPreview = true; // This ensures that the form can capture key events

            // Populate the ComboBox with available fonts
            PopulateFontComboBox();

            PopulateFontSizeComboBox();

            // Get the path of the folder where the executable is located
            string exeFolderPath = Application.StartupPath;

            // Define the "saves" folder path
            savesFolderPath = Path.Combine(exeFolderPath, "saves");

            // Define the file path where the TextBox content will be saved
            filePath = Path.Combine(savesFolderPath, "textfile.txt");

            // Ensure the "saves" folder exists
            if (!Directory.Exists(savesFolderPath))
            {
                Directory.CreateDirectory(savesFolderPath);
            }

            synthesizer = new SpeechSynthesizer();
        }

        private void PopulateVoices()
        {
            // Clear the ComboBox
            comboBoxVoices.Items.Clear();

            // Get available voices
            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                comboBoxVoices.Items.Add(voice.VoiceInfo.Name);
            }

            // Select the default voice (first item)
            if (comboBoxVoices.Items.Count > 0)
                comboBoxVoices.SelectedIndex = 0;
        }

        // Method to populate the ComboBox with font sizes
        private void PopulateFontSizeComboBox()
        {
            for (int i = 8; i <= 30; i++) // Populating with font sizes from 8 to 30
            {
                comboBoxFontSize.Items.Add(i);
            }

            // Set a default font size (e.g., 12)
            comboBoxFontSize.SelectedItem = 12;
        }

        private void PopulateFontComboBox()
        {
            foreach (FontFamily font in FontFamily.Families)
            {
                comboBox1.Items.Add(font.Name);
            }

            // Select the first font by default
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set the filter to only show text files
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                string filePath = openFileDialog.FileName;

                try
                {
                    // Read the entire text from the file
                    string fileContent = File.ReadAllText(filePath);

                    // Display the content in the TextBox
                    textBox1.Text = fileContent;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while reading the file: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set the filter to only show .txt files
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";

            // Show the dialog and check if the user selected a file
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the file path where the user wants to save the file
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Write the content of the TextBox to the selected file
                    File.WriteAllText(filePath, textBox1.Text);

                    // Optionally, show a success message
                    MessageBox.Show("File saved successfully!");
                }
                catch (Exception ex)
                {
                    // Handle any errors that might occur during the save process
                    MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string message = "About this Advanced Text Editor";
            string title = "Application Created by Providing Solutions LLC, Opening Settings.";

            MessageBox.Show(title, message);
            
            // Show the settings form
            Settings settingsForm = new Settings(this); // Pass Form1 to SettingsForm
            settingsForm.ShowDialog(); // Show the settings form as a modal dialog
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Loop through all controls on the form
            foreach (Control control in Controls)
            {
                // Check if the control is a Label or Button
                if (control is Label || control is System.Windows.Forms.Button)
                {
                    // Change the font to Verdana with size 10
                    control.Font = new Font("Verdana", 10);

                    // For buttons, automatically adjust the size
                    if (control is System.Windows.Forms.Button button)
                    {
                        // Set AutoSize to true for buttons
                        button.AutoSize = true;
                    }
                }
            }

            // Adjust form size after changing fonts and button sizes
            AdjustFormSize();
        }

        //Adjust textbox size to font
        private void AdjustTextBoxSize(System.Windows.Forms.TextBox textBox)
        {
            // Adjust the width based on the text content
            using (Graphics g = textBox.CreateGraphics())
            {
                // Calculate the width of the text content with the font
                SizeF textSize = g.MeasureString(textBox.Text, textBox.Font);
                textBox.Width = (int)textSize.Width + 10; // Adding some padding
            }

            // Adjust height based on font size
            textBox.Height = (int)(textBox.Font.Height * 2); // Example height adjustment
        }

        //Adjust form to font size
        private void AdjustFormSize()
        {
            // Calculate the required size for the form
            int formWidth = 0;
            int formHeight = 0;

            // Calculate the maximum width and height of controls
            foreach (Control control in Controls)
            {
                if (control.Visible)
                {
                    // Update form width to be large enough to fit all controls
                    formWidth = Math.Max(formWidth, control.Right);
                    // Update form height to be large enough to fit all controls
                    formHeight = Math.Max(formHeight, control.Bottom);
                }
            }

            // Adjust the form size based on controls' layout
            this.ClientSize = new Size(formWidth + 10, formHeight + 10); // Add some padding
        }

        public void ChangeBackgroundColor(Color newColor)
        {
            this.BackColor = newColor;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Checking, if version is outdated to the latest version

            string Version = "DISCONTINIUED (WAS 2.3REBORN)";

            WebClient version = new WebClient();
            string current = version.DownloadString("https://novowarecheats.github.io/authentication/version.html");

            if (current.Contains(Version))
            {
                try
                {
                    // Get all files matching the pattern "textfile_*.txt"
                    var files = Directory.GetFiles(savesFolderPath, "textfile_*.txt");

                    // If there are files, find the most recent one by creation time
                    if (files.Length > 0)
                    {
                        // Sort the files by creation time in descending order (most recent first)
                        var latestFile = files.OrderByDescending(file => File.GetCreationTime(file)).First();

                        // Read the content of the most recent file and set it to the TextBox
                        textBox1.Text = File.ReadAllText(latestFile);

                        // Extract the file name from the full path (without the directory)
                        string fileName = Path.GetFileName(latestFile);

                        // Set the label text to display the current file name
                        labelFile.Text = $"Currently open: {fileName}";  // Assuming you have a Label named label1
                    }
                    else
                    {
                        MessageBox.Show("No saved files found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading file: " + ex.Message);
                }

                //Correct, Launching Application Normally.

                // Enable drag-and-drop for the TextBox
                textBox1.AllowDrop = true;

                // Attach event handlers for drag and drop events
                textBox1.DragEnter += textBox1_DragEnter;
                textBox1.DragDrop += textBox1_DragDrop;
            }
            else
            {
                string message = "Outdated Version";
                string title = "This version is outdated, please consider downloading the latest version from " +
                    "https://providingsolutions.github.io/version.html " +
                    "(You can still use this version although it may be outdated and some features may not work.)";

                MessageBox.Show(title, message);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string message = "Warning";
            string title = "This sections of the application is still being developed, so currently its not functioning.";

            MessageBox.Show(title, message);

            //this.MinimizeBox = true;

            //DeveloperMode developerMode = new DeveloperMode();
            //developerMode.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the TextBox (txtContent) contains any text
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // Ask the user if they want to save the content
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?",
                                                      "Confirm Exit",
                                                      MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Call a method to save the file (you can implement this)
                    SaveFileClosing();
                }
                else if (result == DialogResult.Cancel)
                {
                    // Cancel the closing of the form
                    e.Cancel = true;
                }
            }
        }

        private void SaveFileClosing()
        {
            // Implement your file-saving logic here
            // For example, you could save the content of the TextBox to a file.
            // SaveFileDialog is one way to prompt the user to select a location.

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, textBox1.Text);
                }
            }
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            // Check if the TextBox has text and if the CheckBox is unchecked
            if (!checkBox1.Checked && !string.IsNullOrEmpty(textBox1.Text))
            {
                // Show a Yes/No MessageBox asking if they want to save the file
                DialogResult result = MessageBox.Show(
                    "Do you want to save the changes to the file?",
                    "Save File",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Save the text if the user clicked Yes
                    SaveFileClosing();
                }
                else if (result == DialogResult.Cancel)
                {
                    // Cancel the closing operation if the user clicked Cancel
                    e.Cancel = true;
                }
            }
            // Dispose the synthesizer when closing the form
            synthesizer.Dispose();
        }

        // This event handles what is printed when the user selects to print
        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Get the content from the TextBox (txtContent)
            string textToPrint = textBox1.Text;

            // Set up the font and brush to print the text
            Font font = new Font("Arial", 12);
            Brush brush = Brushes.Black;

            // Define the area where the text will be printed (the margins)
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;

            // Print the text to the page
            e.Graphics.DrawString(textToPrint, font, brush, leftMargin, topMargin);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            // Create a PrintDialog to allow the user to select a printer
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            // Assign the PrintPage event to specify what will be printed
            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);

            // Set the PrintDocument of the PrintDialog
            printDialog.Document = printDocument;

            // Show the print dialog and check if the user selected "Print"
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Start printing the document
                printDocument.Print();
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            // Get the files being dragged
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check if there is at least one file
            if (files.Length > 0)
            {
                string filePath = files[0]; // Get the first file path

                // Check if the file is a .txt file
                if (Path.GetExtension(filePath).ToLower() == ".txt")
                {
                    // Read the file content and display it in the TextBox
                    try
                    {
                        string fileContent = File.ReadAllText(filePath);
                        textBox1.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading the file: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Please drop a .txt file.");
                }
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the data being dragged is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Indicate that the data is acceptable to drop
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Deny drop if not a file
                e.Effect = DragDropEffects.None;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Check if the TextBox is empty
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter some content in the script box before saving.");
                return; // Exit the method if the TextBox is empty
            }

            // Get the content from the TextBox
            string scriptContent = textBox1.Text;

            // Specify the path for the .bat file
            string batFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "tempScript.bat");

            try
            {
                // Save the content into a .bat file
                File.WriteAllText(batFilePath, scriptContent);

                // Optional: Show a message about the file creation
                MessageBox.Show($"Batch script saved to: {batFilePath}", "Success");

                // Execute the .bat file
                Process.Start(batFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Change the font of the TextBox
            textBox1.Font = new Font("Tahoma", 14, FontStyle.Bold);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected font name from the ComboBox
            string selectedFont = comboBox1.SelectedItem.ToString();
            try
            {
                // Set the TextBox font to the selected font
                textBox1.Font = new Font(selectedFont, textBox1.Font.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTextBoxFont();
        }

        // Method to update TextBox font based on selected font and font size
        private void UpdateTextBoxFont()
        {
            // Get the selected font and font size
            string selectedFont = comboBox1.SelectedItem?.ToString();
            int selectedFontSize = (int)comboBoxFontSize.SelectedItem;

            try
            {
                // Set the TextBox font to the selected font and size
                textBox1.Font = new Font(selectedFont, selectedFontSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // Create a new ColorDialog
            ColorDialog colorDialog = new ColorDialog();

            // Optional: Set initial color if needed
            colorDialog.Color = textBox1.BackColor; // Set the initial color to the current BackColor of the TextBox

            // Show the color dialog and check if the user selected a color
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Change the BackColor of the TextBox (or any other control)
                textBox1.BackColor = colorDialog.Color;
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            // Create a ColorDialog instance
            ColorDialog colorDialog = new ColorDialog();

            // Optional: Set the initial color to the current BackColor of one of the buttons (e.g., button1)
            colorDialog.Color = button1.BackColor;

            // Show the color dialog and check if the user selected a color
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Loop through all controls on the form
                foreach (Control control in this.Controls)
                {
                    // Check if the control is a Button
                    if (control is System.Windows.Forms.Button)
                    {
                        // Set the BackColor of the button to the selected color
                        control.BackColor = colorDialog.Color;
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("TextBox is empty. Nothing to save.");
                checkBox1.Checked = false;
                return; // Skip saving if TextBox is empty
            }

            try
            {
                // Generate a unique file name using timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filePath = Path.Combine(savesFolderPath, $"textfile_{timestamp}.txt");

                // Write the text from the TextBox to the new file
                File.WriteAllText(filePath, textBox1.Text);
                MessageBox.Show($"Text saved successfully to: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // Show a Yes/No MessageBox to confirm deletion of all contents
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete everything inside the 'saves' folder?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // If the user clicks Yes, delete all contents inside the folder
                if (result == DialogResult.Yes)
                {
                    // Check if the folder exists
                    if (Directory.Exists(savesFolderPath))
                    {
                        // Delete all files in the folder
                        foreach (var file in Directory.GetFiles(savesFolderPath))
                        {
                            File.Delete(file);
                        }

                        // Delete all subdirectories in the folder
                        foreach (var dir in Directory.GetDirectories(savesFolderPath))
                        {
                            Directory.Delete(dir, true); // true to delete directories and their contents recursively
                        }

                        MessageBox.Show("All contents in the 'saves' folder have been deleted.");
                    }
                    else
                    {
                        MessageBox.Show("The 'saves' folder does not exist.");
                    }
                }
                else
                {
                    MessageBox.Show("Deletion canceled.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting contents: " + ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string Title = "Warning!";
            string Message = "Only Delete Files inside the Saves folder. deleting other files may cause corruption.";

            MessageBox.Show(Message, Title);


            // Open a file dialog to let the user select a file to delete
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file to delete";
            openFileDialog.Filter = "All Files (*.*)|*.*"; // You can change the file type filter if needed

            // Set the initial directory to the folder where the executable is located
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Ensure the file exists before deleting
                    if (File.Exists(filePath))
                    {

                        // Get the directory of the selected file
                        string fileDirectory = Path.GetDirectoryName(filePath);

                        // Delete the selected file
                        File.Delete(filePath);
                        MessageBox.Show($"File {filePath} deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Redirect to the folder where the file was located
                        Process.Start("explorer.exe", fileDirectory);
                    }
                    else
                    {
                        MessageBox.Show("File not found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Catch any errors that may occur during file deletion
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // This method automatically opens the folder where the executable is located
        private void OpenExecutableFolder()
        {
            try
            {
                string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Open the folder in Windows Explorer
                Process.Start("explorer.exe", executableDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Create a preset JSON structure
            var jsonPreset = new
            {
                name = "Sample Name",
                age = 30,
                address = new
                {
                    street = "123 Main St",
                    city = "Sample City",
                    postalCode = "12345"
                },
                isActive = true,
                items = new string[] { "item1", "item2", "item3" }
            };

            // Convert the object to a JSON string with indented formatting
            string jsonString = JsonConvert.SerializeObject(jsonPreset, Newtonsoft.Json.Formatting.Indented);

            // Display the JSON string in the TextBox
            textBox1.Text = jsonString;

            MessageBox.Show("Json Preset Created!");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Create a preset XML structure using XDocument (LINQ to XML)
            XElement xmlPreset = new XElement("Person",
                new XElement("Name", "John Doe"),
                new XElement("Age", 30),
                new XElement("Address",
                    new XElement("Street", "123 Elm St"),
                    new XElement("City", "Sample City"),
                    new XElement("PostalCode", "12345")
                ),
                new XElement("IsActive", true),
                new XElement("Items",
                    new XElement("Item", "item1"),
                    new XElement("Item", "item2"),
                    new XElement("Item", "item3")
                )
            );

            // Convert the XElement to a string (formatted XML)
            string xmlString = xmlPreset.ToString();

            // Display the XML string in the TextBox
            textBox1.Text = xmlString;

            MessageBox.Show("Xml Preset Created!");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Show the color dialog and check if the user selected a color
            if (colorDialogBackground.ShowDialog() == DialogResult.OK)
            {
                // Change the background color of the form to the selected color
                this.BackColor = colorDialogBackground.Color;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Ensure synthesizer is initialized
            if (synthesizer == null)
            {
                MessageBox.Show("Speech synthesizer is not initialized.");
                return;
            }

            // Ensure the TextBox is not empty
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter text to speak.");
                return;
            }

            // Get the selected voice name from the ComboBox
            string selectedVoiceName = comboBoxVoices.SelectedItem.ToString();

            // Set the selected voice
            synthesizer.SelectVoice(selectedVoiceName);

            // Speak the text entered in the TextBox using the selected voice
            synthesizer.SpeakAsync(textBox1.Text);
        }

        // Method to change the text color of all controls
        private void ChangeTextColor(Control.ControlCollection controls, Color color)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Label || ctrl is System.Windows.Forms.Button || ctrl is System.Windows.Forms.TextBox || ctrl is LinkLabel)
                {
                    // Set the ForeColor property for controls with text
                    ctrl.ForeColor = color;
                }

                // Recursively change the text color of child controls (for containers like panels, groupboxes, etc.)
                if (ctrl.HasChildren)
                {
                    ChangeTextColor(ctrl.Controls, color);
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // Show the color dialog and check if the user selected a color
            if (colorDialogText.ShowDialog() == DialogResult.OK)
            {
                // Get the selected color from the color dialog
                Color selectedColor = colorDialogText.Color;

                // Change the text color of all labels and buttons on the form
                ChangeTextColor(this.Controls, selectedColor);

                // Check if any button has red background and change its text color to white
                ChangeButtonTextColorForRedBackground(this.Controls);
            }
        }

        // Method to change the text color of all labels and buttons
        private void ChangeTextColor12(Control.ControlCollection controls, Color color)
        {
            foreach (Control ctrl in controls)
            {
                // Check if the control is a Label or Button
                if (ctrl is Label || ctrl is System.Windows.Forms.Button)
                {
                    // Set the ForeColor property for labels and buttons
                    ctrl.ForeColor = color;
                }

                // Recursively change the text color of child controls (for containers like panels, groupboxes, etc.)
                if (ctrl.HasChildren)
                {
                    ChangeTextColor(ctrl.Controls, color);
                }
            }
        }

        // Method to check if any button has red background and change its text color to white
        private void ChangeButtonTextColorForRedBackground(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                // Check if the control is a Button and its BackColor is red
                if (ctrl is System.Windows.Forms.Button button && button.BackColor == Color.Red)
                {
                    // Change the text color of buttons with red background to white
                    button.ForeColor = Color.White;
                }

                // Recursively check for child controls
                if (ctrl.HasChildren)
                {
                    ChangeButtonTextColorForRedBackground(ctrl.Controls);
                }
            }
        }
    }
}
