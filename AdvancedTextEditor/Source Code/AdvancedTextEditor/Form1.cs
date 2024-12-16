//Application Developed With The Help of ChatGPT.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AdvancedTextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            string title = "Application Created by Providing Solutions LLC";

            MessageBox.Show(title, message);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //Checking, if version is outdated to the latest version

            string Version = "DISCONTINIUED (WAS 2.3REBORN)";

            WebClient version = new WebClient();
            string current = version.DownloadString("https://novowarecheats.github.io/authentication/version.html");

            if (current.Contains(Version))
            {
                //Correct, Launching Application Normally.
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
    }
}
