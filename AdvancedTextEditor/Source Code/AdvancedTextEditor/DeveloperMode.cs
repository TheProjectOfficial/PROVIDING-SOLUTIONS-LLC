using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedTextEditor
{
    public partial class DeveloperMode : Form
    {
        private void ApplyPythonSyntaxHighlighting()
        {
            string text = richTextBox1.Text;

            // Clear any previous formatting
            richTextBox1.Clear();

            // Regular expressions to match keywords, functions, strings, etc.
            HighlightSyntax(text, @"\b(def|class|if|else|elif|return|for|while|import|from|as|with|try|except|finally)\b", Color.Blue); // Python keywords
            HighlightSyntax(text, @"\b([A-Za-z_][A-Za-z0-9_]*)(?=\()", Color.Green); // Function names
            HighlightSyntax(text, @"'[^']*'|""[^""]*""", Color.Red); // Strings
            HighlightSyntax(text, @"#.*$", Color.Gray); // Comments

            // Apply the original text after all formatting
            richTextBox1.Text = text;
        }

        private void HighlightSyntax(string text, string pattern, Color color)
        {
            // Find all matches of the pattern in the text
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(text))
            {
                // Select the matching text and apply the specified color
                richTextBox1.SelectionStart = match.Index;
                richTextBox1.SelectionLength = match.Length;
                richTextBox1.SelectionColor = color;
            }
        }
        public DeveloperMode()
        {
            InitializeComponent();
        }

        private void DeveloperMode_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Will be added next update.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ApplyPythonSyntaxHighlighting();
        }
    }
}
