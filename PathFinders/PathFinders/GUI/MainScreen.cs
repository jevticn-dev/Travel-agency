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
    public partial class MainScreen : Form
    {
        private Panel sidebar;
        private Panel mainPanel;
        private Label lblLogo;
        private Button btnKlijenti, btnPaketi, btnRezervacije, btnBackup;
        private Button aktivnoDugme = null;
        private Panel indikator;

        // Panel za klijente
        private Panel klijentiPanel;
        private DataGridView dgvKlijenti;
        private Button btnDodaj, btnIzmeni, btnUndo, btnRedo;
        private TextBox txtPretraga;

        public MainScreen()
        {
            // Forma
            this.Text = "Moja Turistička Agencija";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Sidebar
            sidebar = new Panel();
        sidebar.Dock = DockStyle.Left;
            sidebar.Width = 220;
            sidebar.BackColor = Color.FromArgb(45, 62, 80);

            // Logo
            lblLogo = new Label();
        lblLogo.Text = "Agencija";
            lblLogo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        lblLogo.ForeColor = Color.White;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Dock = DockStyle.Top;
            lblLogo.Height = 80;

            // Indikator
            indikator = new Panel();
            indikator.Size = new Size(5, 50);
            indikator.BackColor = Color.White;
            indikator.Visible = false; // biće prikazan tek kada kliknemo na dugme
            sidebar.Controls.Add(indikator);

            // Dugmad u sidebaru
            btnKlijenti = NapraviDugme("👥 Klijenti", 100);
        btnPaketi = NapraviDugme("📦 Paketi", 160);
        btnRezervacije = NapraviDugme("📅 Rezervacije", 220);
        btnBackup = NapraviDugme("💾 Backup", 280);

        sidebar.Controls.Add(lblLogo);
            sidebar.Controls.AddRange(new Control[] { btnKlijenti, btnPaketi, btnRezervacije, btnBackup
    });

            // Glavni panel
            mainPanel = new Panel();
    mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;

            // Dodaj panele
            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebar);

            // Event handlers
            btnKlijenti.Click += (s, e) => {
                PrikaziTabelu("Klijenti");
                OznaciDugme(btnKlijenti);
            };
            btnPaketi.Click += (s, e) => {
                PrikaziTabelu("Paketi");
                OznaciDugme(btnPaketi);
            };
            btnRezervacije.Click += (s, e) => {
                PrikaziTabelu("Rezervacije");
                OznaciDugme(btnRezervacije);
            };
            btnBackup.Click += (s, e) => {
                mainPanel.Controls.Clear(); // Obriši sadržaj glavnog panela
                OznaciDugme(btnBackup);
            };

            // Event
            //btnKlijenti.Click += (s, e) => PrikaziKlijente();
        }

        private Button NapraviDugme(string text, int top)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.Size = new Size(200, 50);
            btn.Location = new Point(10, top);
            btn.BackColor = Color.FromArgb(52, 73, 94);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.UseVisualStyleBackColor = false; // važno da koristi naš BackColor

            btn.Cursor = Cursors.Hand;

            return btn;
        }

        private void OznaciDugme(Button btn)
        {
            // reset svih dugmića u sidebaru
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button dugme)
                {
                    dugme.BackColor = Color.FromArgb(52, 73, 94); // osnovna tamna boja
                    dugme.ForeColor = Color.White;
                }
            }

            // ofarbaj trenutno selektovano
            btn.BackColor = Color.FromArgb(41, 128, 185); // plava
            btn.ForeColor = Color.White;
            aktivnoDugme = btn;

            indikator.Visible = true;
            indikator.Location = new Point(0, btn.Top);
        }


        private Button NapraviAkcijskoDugme(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Size = new Size(100, 35);
            btn.Location = new Point(x, y);
            btn.BackColor = Color.White;
            btn.ForeColor = Color.DimGray;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, btn.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 2, ButtonBorderStyle.Solid,
                    Color.LightGray, 2, ButtonBorderStyle.Solid);
            };
            return btn;
        }

        private void PrikaziTabelu(string tip)
        {
            mainPanel.Controls.Clear();

            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;

            // Dugmad
            Button btnDodaj = NapraviAkcijskoDugme("+ Dodaj", 10, 10);
            Button btnIzmeni = NapraviAkcijskoDugme("✎ Izmeni", 120, 10);
            Button btnUndo = NapraviAkcijskoDugme("↶ Undo", 230, 10);
            Button btnRedo = NapraviAkcijskoDugme("↷ Redo", 340, 10);
            Button btnOtkazi = null;

            if (tip == "Rezervacije")
            {
                btnOtkazi = NapraviAkcijskoDugme("✖ Otkazi", 450, 10);
                panel.Controls.Add(btnOtkazi);
            }

            TextBox txtPretraga = new TextBox { PlaceholderText = $"Pretraga ({tip})...", Location = new Point(570, 13), Width = 400 };

            // DataGridView
            DataGridView dgv = new DataGridView();
            dgv.Location = new Point(10, 60);
            dgv.Size = new Size(1000, 500);
            dgv.BackgroundColor = Color.White;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245); // svetlosivi redovi
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            dgv.GridColor = Color.LightGray;
            dgv.AllowUserToAddRows = false;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Kolone i test podaci
            dgv.Columns.Clear();
            if (tip == "Klijenti")
            {
                this.dgvKlijenti = dgv;
                dgv.Columns.Add("Ime", "Ime");
                dgv.Columns.Add("Prezime", "Prezime");
                dgv.Columns.Add("BrojPasosa", "Broj pasoša");
                dgv.Columns.Add("DatumRodjenja", "Datum rođenja");
                dgv.Columns.Add("Email", "Email");
                dgv.Columns.Add("Telefon", "Telefon");

                dgv.Rows.Add("Bojan", "Kovarbasic", "123456789", "26.8.2024", "mail1@gmail.com", "062123456");

                btnDodaj.Click += (s, e) => {
                    FormaNoviKlijent forma = new FormaNoviKlijent();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows.Add(
                            forma.Ime, forma.Prezime, forma.BrojPasosa,
                            forma.DatumRodjenja.ToShortDateString(), forma.Email, forma.Telefon
                        );
                    }
                };
            }
            else if (tip == "Paketi")
            {
                dgv.Columns.Add("Naziv", "Naziv");
                dgv.Columns.Add("Destinacija", "Destinacija");
                dgv.Columns.Add("Cena", "Cena");
                dgv.Columns.Add("Trajanje", "Trajanje");
                dgv.Columns.Add("Opis", "Opis");

                dgv.Rows.Add("Letovanje", "Grčka", "500€", "7 dana", "All inclusive");

                btnDodaj.Click += (s, e) => {
                    FormaNoviPaket forma = new FormaNoviPaket();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows.Add(
                            forma.Naziv, forma.Destinacija, forma.Cena,
                            forma.Trajanje, forma.Opis
                        );
                    }
                };
            }
            else if (tip == "Rezervacije")
            {
                dgv.Columns.Add("Ime", "Ime");
                dgv.Columns.Add("Prezime", "Prezime");
                dgv.Columns.Add("Paket", "Paket");
                dgv.Columns.Add("Datum", "Datum");
                dgv.Columns.Add("Status", "Status");

                dgv.Rows.Add("Pera", "Perić", "Grčka - Letovanje", "12.7.2025", "Potvrđeno");

                btnDodaj.Click += (s, e) => {
                    FormaNovaRezervacija forma = new FormaNovaRezervacija();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows.Add(
                            forma.Ime, forma.Prezime, forma.Paket,
                            forma.Datum.ToShortDateString(), forma.Status
                        );
                    }
                };
            }

            // Dodaj kontrole
            panel.Controls.Add(btnDodaj);
            panel.Controls.Add(btnIzmeni);
            panel.Controls.Add(btnUndo);
            panel.Controls.Add(btnRedo);
            panel.Controls.Add(txtPretraga);
            panel.Controls.Add(dgv);

            mainPanel.Controls.Add(panel);
        }

    }

}

