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

    // Event
    btnKlijenti.Click += (s, e) => PrikaziKlijente();
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
        btn.Cursor = Cursors.Hand;

        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
        btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 73, 94);

        return btn;
    }

    private void PrikaziKlijente()
    {
        mainPanel.Controls.Clear();

        // Panel za klijente
        klijentiPanel = new Panel();
        klijentiPanel.Dock = DockStyle.Fill;
        klijentiPanel.BackColor = Color.White;

        // Dugmad i pretraga
        btnDodaj = new Button { Text = "+ Dodaj", BackColor = Color.SeaGreen, ForeColor = Color.White, Location = new Point(10, 10), Size = new Size(100, 35) };
        btnIzmeni = new Button { Text = "✎ Izmeni", BackColor = Color.SteelBlue, ForeColor = Color.White, Location = new Point(120, 10), Size = new Size(100, 35) };
        btnUndo = new Button { Text = "↶ Undo", BackColor = Color.DarkOrange, ForeColor = Color.White, Location = new Point(230, 10), Size = new Size(100, 35) };
        btnRedo = new Button { Text = "↷ Redo", BackColor = Color.MediumVioletRed, ForeColor = Color.White, Location = new Point(340, 10), Size = new Size(100, 35) };

        txtPretraga = new TextBox { PlaceholderText = "Pretraga po imenu, prezimenu ili broju pasoša...", Location = new Point(460, 13), Width = 400 };

        // Tabela
        dgvKlijenti = new DataGridView();
        dgvKlijenti.Location = new Point(10, 60);
        dgvKlijenti.Size = new Size(930, 450);
        dgvKlijenti.BackgroundColor = Color.White;
        dgvKlijenti.AllowUserToAddRows = false;
        dgvKlijenti.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal;
        dgvKlijenti.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvKlijenti.EnableHeadersVisualStyles = false;
        dgvKlijenti.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        // Kolone
        dgvKlijenti.Columns.Add("Ime", "Ime");
        dgvKlijenti.Columns.Add("Prezime", "Prezime");
        dgvKlijenti.Columns.Add("BrojPasosa", "Broj pasoša");
        dgvKlijenti.Columns.Add("DatumRodjenja", "Datum rođenja");
        dgvKlijenti.Columns.Add("Email", "Email");
        dgvKlijenti.Columns.Add("Telefon", "Telefon");

        // Test podaci
        dgvKlijenti.Rows.Add("Bojan", "Kovarbasic", "123456789", "26.8.2024", "mail1@gmail.com", "062123456");
        dgvKlijenti.Rows.Add("Pera", "Pesić", "123456/89", "6.1.2004", "mail2@gmail.com", "061234567");
        dgvKlijenti.Rows.Add("Djura", "Djurić", "123456/89", "11.8.2025", "mail3@gmail.com", "065123456");

        // Dodaj u panel
        klijentiPanel.Controls.Add(btnDodaj);
        klijentiPanel.Controls.Add(btnIzmeni);
        klijentiPanel.Controls.Add(btnUndo);
        klijentiPanel.Controls.Add(btnRedo);
        klijentiPanel.Controls.Add(txtPretraga);
        klijentiPanel.Controls.Add(dgvKlijenti);

        mainPanel.Controls.Add(klijentiPanel);
    }

}
}
