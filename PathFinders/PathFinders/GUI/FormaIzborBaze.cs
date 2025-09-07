using System;
using System.IO;
using System.Windows.Forms;
using PathFinders.Patterns.Multiton; // Import Multiton namespace
using PathFinders.Services;
using PathFinders.Backup;
using System.Linq;

namespace PathFinders.GUI
{
    public partial class FormaIzborBaze : Form
    {
        private Label lblTitle;
        private ComboBox cboConfigs;
        private Button btnOpen;
        private BackupScheduler activeScheduler;

        public FormaIzborBaze()
        {
            // Windows Forms auto-generated code, no changes needed here
            Text = "Odabir baze";
            Size = new Size(480, 250);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblTitle = new Label
            {
                Text = "Turističke agencije",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var lblDrop = new Label
            {
                Text = "Izaberi konfiguraciju:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Left = 10,
                Top = 20,
                AutoSize = true
            };

            cboConfigs = new ComboBox
            {
                Left = 10,
                Top = 45,
                Width = 420,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Dynamically load config files from the application's startup path.
            LoadConfigFiles();

            btnOpen = new Button
            {
                Text = "Izaberi bazu",
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            btnOpen.FlatAppearance.BorderSize = 0;

            btnOpen.Left = (480 - btnOpen.Width) / 2;
            btnOpen.Top = 170 - btnOpen.Height - 40;

            btnOpen.Click += BtnOpen_Click;

            pnl.Controls.Add(lblDrop);
            pnl.Controls.Add(cboConfigs);
            pnl.Controls.Add(btnOpen);

            Controls.Add(pnl);
            Controls.Add(lblTitle);
        }

        private void LoadConfigFiles()
        {
            // We search for files named 'config*.txt'
            string[] configFiles = Directory.GetFiles(Application.StartupPath, "config*.txt");
            foreach (string file in configFiles)
            {
                // We add just the filename to the ComboBox
                cboConfigs.Items.Add(Path.GetFileName(file));
            }
            if (cboConfigs.Items.Count > 0)
            {
                cboConfigs.SelectedIndex = 0;
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (cboConfigs.SelectedItem == null)
            {
                MessageBox.Show("Izaberite konfiguraciju.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string configFilePath = Path.Combine(Application.StartupPath, cboConfigs.SelectedItem.ToString());
                string[] lines = File.ReadAllLines(configFilePath);
                string agencyName = lines[0];
                string connectionString = lines[1];

                if (activeScheduler != null)
                {
                    activeScheduler.Stop();
                }

                Backup.Backup backupModule = new Backup.Backup();
                string backupPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
                activeScheduler = backupModule.InitializeBackup(connectionString, backupPath);

                // Multiton: Get or create a single instance of the database service for this connection string
                IDatabaseService dbService = DatabaseManager.GetInstance(connectionString);

                // Pass the service and agency name to MainScreen
                var main = new MainScreen(agencyName, dbService);

                this.Hide();
                main.FormClosed += (s, _) => this.Close();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju konfiguracije: {ex.Message}", "Greška",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}