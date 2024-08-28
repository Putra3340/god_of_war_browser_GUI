using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gow_Browser_GUI
{
    public partial class ControlPanel : Form
    {
        public string exec = "";
        public string filePath, port, gowver, playstation, encoding,method = "";
        bool noterror = true;
        private CancellationTokenSource cancellationTokenSource;



        public async Task LogCheck()
        {
            // Cancel the running operation
            Process.Start("taskkill", "/F /IM gow.exe");
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            comboBox1.Enabled = true;
            button1.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;
            AppendTextToTextBox("Process Stopped!");
            return;
        }

        public ControlPanel()
        {
            InitializeComponent();
            Process.Start("taskkill", "/F /IM gow.exe");
            textBox3.Text = "8000";
            radioButton1.Checked = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "God of War Browser";
            tabPage1.Text = "Controls";
            tabPage2.Text = "Console Log";
            tabPage3.Text = "Credits";
            label9.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #region Textbox Changed

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            filePath = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            port = textBox3.Text;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            gowver = textBox5.Text;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            playstation = textBox4.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            encoding = comboBox1.Text;
        }
        #endregion

        //Browse File
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                filePath = openFileDialog1.FileName; // store global
                textBox2.Text = filePath; // save it
            }
        }

        //START
        public async void button2_Click(object sender, EventArgs e)
        {
            #region Check Input

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("File Path is Empty");
                goto End;
            }
            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("Port is Empty");
                goto End;
            }
            if (string.IsNullOrEmpty(gowver))
            {
                MessageBox.Show("God of War Version is Empty");
                goto End;
            }
            if (string.IsNullOrEmpty(playstation))
            {
                MessageBox.Show("Playstation Version is Empty");
                goto End;
            }
            if (string.IsNullOrEmpty(encoding))
            {
                encoding = "Windows 1252";
            }

            #endregion
            #region Method Radio
            if (radioButton1.Checked)
            {
                method = "-psarc";
            }
            else if (radioButton2.Checked)
            {
                method = "-iso";
            }else if (radioButton3.Checked)
            {
                method = "-toc";
            }
            #endregion
            //await RunCommandAsync("taskkill /f /im gow.exe", cancellationTokenSource.Token);
            // Cancel any previous operation
            cancellationTokenSource?.Cancel();

            // Create a new CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();

            // Construct the command string
            string command = $"gow.exe {method} \"{filePath}\" -ps ps{playstation} -gowversion {gowver} -i :{port} -encoding \"{encoding}\"";
            #region restrict input
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            comboBox1.Enabled = false;
            button1.Enabled = false;
            radioButton1 .Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            #endregion
            AppendTextToTextBox(command);
            await RunCommandAsync(command, cancellationTokenSource.Token);
            //await LogCheck();
        End:
            return;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/Putra3340";
            System.Diagnostics.Process.Start(url);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/mogaika/god_of_war_browser";
            System.Diagnostics.Process.Start(url);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/mogaika";
            System.Diagnostics.Process.Start(url);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start($"http://127.0.0.1:{port}");
        }

        //STOP
        private void button3_Click(object sender, EventArgs e)
        {
            LogCheck();
            cancellationTokenSource?.Cancel();
        }



        private async Task RunCommandAsync(string command, CancellationToken cancellationToken)
        {
            try
            {
                // Run the command on a background thread
                await Task.Run(() => ExecuteCommand(command, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                textBox1.AppendText("Operation canceled.\n");
            }
            catch (Exception ex)
            {
                textBox1.AppendText("An error occurred: " + ex.Message + "\n");
            }
        }

        private void ExecuteCommand(string command, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new process
                using (Process process = new Process())
                {
                    // Set the process start information
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/C {command}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    // Start the process
                    process.Start();

                    // Read the output from the command asynchronously
                    using (var outputReader = process.StandardOutput)
                    using (var errorReader = process.StandardError)
                    {
                        // Continuously read the output
                        Task.Run(() =>
                        {
                            while (!outputReader.EndOfStream)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    process.Kill();
                                    break;
                                }

                                string line = outputReader.ReadLine();
                                AppendTextToTextBox(line);
                            }
                        });

                        // Continuously read the error output
                        Task.Run(() =>
                        {
                            while (!errorReader.EndOfStream)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    process.Kill();
                                    break;
                                }

                                string line = errorReader.ReadLine();
                                AppendTextToTextBox(line);
                            }
                        });

                        // Wait for the process to exit
                        process.WaitForExit();
                        LogCheck();

                    }
                }
            }
            catch (Exception ex)
            {
                AppendTextToTextBox("An error occurred: " + ex.Message);
            }
        }

        private void AppendTextToTextBox(string text)
        {
            if (InvokeRequired)
            {
                // Use Invoke to update the TextBox on the main thread
                Invoke(new Action(() => textBox1.AppendText(text + Environment.NewLine)));
            }
            else
            {
                textBox1.AppendText(text + Environment.NewLine);
            }
        }
    }
}
