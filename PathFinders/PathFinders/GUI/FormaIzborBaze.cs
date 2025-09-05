using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathFinders.GUI
{
    public partial class FormaIzborBaze : Form
    {
        private Label lblTitle;
        private ComboBox cboConfigs;
        private Button btnOpen;

        public FormaIzborBaze()
        {
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

            
            cboConfigs.Items.AddRange(new object[]
            {
                "config1.txt",
                "config2.txt",
                "config3.txt",
                "config4.txt"
            });
            if (cboConfigs.Items.Count > 0) cboConfigs.SelectedIndex = 0;

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

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (cboConfigs.SelectedItem == null)
            {
                MessageBox.Show("Izaberite konfiguraciju.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            
            var main = new MainScreen();

            // sakrij ovu formu i zatvori je kad se MainScreen zatvori
            this.Hide();
            main.FormClosed += (s, _) => this.Close();
            main.Show();
        }
    }
}
