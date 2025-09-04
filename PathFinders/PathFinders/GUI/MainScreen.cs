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
            sidebar.Controls.AddRange(new Control[] { btnKlijenti, btnPaketi, btnRezervacije, btnBackup });

            // Glavni panel
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;

            // Dodaj panele
            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebar);

            // Event handlers
            btnKlijenti.Click += (s, e) =>
            {
                PrikaziTabelu("Klijenti");
                OznaciDugme(btnKlijenti);
            };
            btnPaketi.Click += (s, e) =>
            {
                PrikaziTabelu("Paketi");
                OznaciDugme(btnPaketi);
            };
            btnRezervacije.Click += (s, e) =>
            {
                PrikaziTabelu("Rezervacije");
                OznaciDugme(btnRezervacije);
            };
            btnBackup.Click += (s, e) =>
            {
                mainPanel.Controls.Clear(); 
                OznaciDugme(btnBackup);
            };

            
            PrikaziTabelu("Klijenti");
            OznaciDugme(btnKlijenti);
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
            btn.UseVisualStyleBackColor = false; 

            btn.Cursor = Cursors.Hand;

            return btn;
        }

        private void OznaciDugme(Button btn)
        {
            
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button dugme)
                {
                    dugme.BackColor = Color.FromArgb(52, 73, 94); 
                    dugme.ForeColor = Color.White;
                }
            }

            
            btn.BackColor = Color.FromArgb(41, 128, 185); 
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

            
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            
            var frame = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            
            var topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.White
            };

           
            Button btnDodaj = NapraviAkcijskoDugme("+ Dodaj", 10, 10);
            Button btnIzmeni = NapraviAkcijskoDugme("✎ Izmeni", 120, 10);
            Button btnUndo = NapraviAkcijskoDugme("↶ Undo", 230, 10);
            Button btnRedo = NapraviAkcijskoDugme("↷ Redo", 340, 10);
            Button btnOtkazi = null;
            if (tip == "Rezervacije")
            {
                btnOtkazi = NapraviAkcijskoDugme("✖ Otkaži", 450, 10);
                topBar.Controls.Add(btnOtkazi);
            }

            
            TextBox txtPretraga = new TextBox
            {
                PlaceholderText = $"Pretraga ({tip})...",
                Width = 320,
                Top = 13,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            topBar.Controls.AddRange(new Control[] { btnDodaj, btnIzmeni, btnUndo, btnRedo, txtPretraga });
            topBar.Resize += (s, e) =>
            {
                txtPretraga.Left = topBar.ClientSize.Width - txtPretraga.Width - 10; // 10px od desnog ruba
            };

            
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray,
                AllowUserToAddRows = false,
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                ReadOnly = true,
                ColumnHeadersDefaultCellStyle = { ForeColor = Color.Black, BackColor = Color.White },
                DefaultCellStyle =
                {
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.LightGray,
                    SelectionForeColor = Color.Black
                },
                        AlternatingRowsDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(245,245,245),
                    ForeColor = Color.Black
                },
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None // koristimo fiksne širine po koloni
            };

            
            frame.Controls.Add(dgv);
            frame.Controls.Add(topBar);
            wrapper.Controls.Add(frame);
            mainPanel.Controls.Add(wrapper);

            
            if (tip == "Klijenti")
            {
                this.dgvKlijenti = dgv;

                dgv.Columns.Add("Ime", "Ime");
                dgv.Columns.Add("Prezime", "Prezime");
                dgv.Columns.Add("BrojPasosa", "Broj pasoša");
                dgv.Columns.Add("DatumRodjenja", "Datum rođenja");
                dgv.Columns.Add("Email", "Email");
                dgv.Columns.Add("Telefon", "Telefon");

                
                dgv.Columns["Ime"].Width = 160;
                dgv.Columns["Prezime"].Width = 170;
                dgv.Columns["BrojPasosa"].Width = 120;
                dgv.Columns["DatumRodjenja"].Width = 120;

                dgv.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["Email"].Width = 320;  

                dgv.Columns["Telefon"].Width = 140;

                
                dgv.CellClick += (s2, e2) =>
                {
                    if (e2.RowIndex >= 0)
                    {
                        dgv.ClearSelection();
                        dgv.Rows[e2.RowIndex].Selected = true;
                    }
                };

                
                dgv.Rows.Add("Bojan", "Kovarbasic", "123456789", "26.8.2024", "dugimejl.koji.je.dug@primerdomena.rs", "062123456");
                dgv.Rows.Add("Bojana", "Kovarbasic", "123456789", "26-Aug-24", "mail1@gmail.com", "062123456");

               
                btnDodaj.Click += (s, e) =>
                {
                    var forma = new FormaNoviKlijent();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows.Add(
                            forma.Ime, forma.Prezime, forma.BrojPasosa,
                            forma.DatumRodjenja.ToShortDateString(), forma.Email, forma.Telefon
                        );
                    }
                };

                
                btnIzmeni.Click += (s, e) =>
                {
                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberi klijenta u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var row = dgv.SelectedRows[0];
                    string ime = row.Cells["Ime"].Value?.ToString() ?? "";
                    string prezime = row.Cells["Prezime"].Value?.ToString() ?? "";
                    string brojPasosa = row.Cells["BrojPasosa"].Value?.ToString() ?? "";
                    string email = row.Cells["Email"].Value?.ToString() ?? "";
                    string telefon = row.Cells["Telefon"].Value?.ToString() ?? "";
                    DateTime datumRodjenja;
                    if (!DateTime.TryParse(row.Cells["DatumRodjenja"].Value?.ToString(), out datumRodjenja))
                        datumRodjenja = DateTime.Today;

                    var forma = new FormaIzmenaKlijenta(ime, prezime, brojPasosa, email, telefon, datumRodjenja);
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        row.Cells["Ime"].Value = forma.Ime;
                        row.Cells["Prezime"].Value = forma.Prezime;
                        row.Cells["BrojPasosa"].Value = forma.BrojPasosa;
                        row.Cells["Email"].Value = forma.Email;
                        row.Cells["Telefon"].Value = forma.Telefon;
                        row.Cells["DatumRodjenja"].Value = forma.DatumRodjenja.ToShortDateString();
                    }
                };

                int ukupnaSirinaKolona = 0;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    ukupnaSirinaKolona += col.Width;
                }

                
                ukupnaSirinaKolona += 60;

                
                this.FormBorderStyle = FormBorderStyle.FixedSingle; 
                this.MaximizeBox = false;                           
                this.ClientSize = new Size(ukupnaSirinaKolona + sidebar.Width, this.ClientSize.Height);
            }
            else if (tip == "Paketi")
            {
                // Kolone: Naziv | Cena | Tip | Destinacija | Detalji
                dgv.Columns.Add("Naziv", "Naziv");
                dgv.Columns.Add("Cena", "Cena");
                dgv.Columns.Add("Tip", "Tip");
                dgv.Columns.Add("Destinacija", "Destinacija");
                dgv.Columns.Add("Detalji", "Detalji");

                // Širine kolona (Detalji se širi)
                dgv.Columns["Naziv"].Width = 200;
                dgv.Columns["Cena"].Width = 100;
                dgv.Columns["Tip"].Width = 160;
                dgv.Columns["Destinacija"].Width = 180;
                dgv.Columns["Detalji"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Pomocna lokalna funkcija za formatiranje "Detalji"
                string DetaljiText(
                    string tipPaketa,
                    string tipSmestaja = null,
                    string tipPrevoza = null,
                    string dodatneAkt = null,
                    string vodic = null,
                    string brod = null,
                    string ruta = null,
                    DateTime? datumPolaska = null)
                {
                    if (tipPaketa == "Aranžman za more")
                        return $"Smeštaj: {tipSmestaja ?? "-"}, Prevoz: {tipPrevoza ?? "-"}";
                    if (tipPaketa == "Aranžman za planine")
                        return $"Smeštaj: {tipSmestaja ?? "-"}, Akt.: {dodatneAkt ?? "-"}";
                    if (tipPaketa == "Ekskurzije")
                        return $"Prevoz: {tipPrevoza ?? "-"}, Vodič: {vodic ?? "-"}";
                    if (tipPaketa == "Krstarenja")
                        return $"Brod: {brod ?? "-"}, Ruta: {ruta ?? "-"}{(datumPolaska.HasValue ? $", Polazak: {datumPolaska.Value:dd.MM.yyyy}" : "")}";
                    return "";
                }

                // Demo redovi (primeri različitih tipova)
                dgv.Rows.Add(
                    "Krf - leto 2026",
                    "599€",
                    "Aranžman za more",
                    "Krf",
                    DetaljiText("Aranžman za more", tipSmestaja: "Hotel 4*", tipPrevoza: "Avion")
                );
                dgv.Rows.Add(
                    "Kopaonik vikend",
                    "199€",
                    "Aranžman za planine",
                    "Kopaonik",
                    DetaljiText("Aranžman za planine", tipSmestaja: "Apartman", dodatneAkt: "Ski pass")
                );
                dgv.Rows.Add(
                    "Beč jednodnevna",
                    "89€",
                    "Ekskurzije",
                    "Beč",
                    DetaljiText("Ekskurzije", tipPrevoza: "Autobus", vodic: "Licencirani vodič")
                );
                dgv.Rows.Add(
                    "Jadransko krstarenje",
                    "1299€",
                    "Krstarenja",
                    "-",
                    DetaljiText("Krstarenja", brod: "MSC Opera", ruta: "Split–Kotor–KrF–Bari", datumPolaska: new DateTime(2026, 6, 15))
                );

                // Klik selektuje red
                dgv.CellClick += (s2, e2) =>
                {
                    if (e2.RowIndex >= 0)
                    {
                        dgv.ClearSelection();
                        dgv.Rows[e2.RowIndex].Selected = true;
                    }
                };

                // + Dodaj
                btnDodaj.Click += (s, e) =>
                {
                    var forma = new FormaNoviPaket();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        // Destinacija zavisi od tipa; za krstarenja nema glavne destinacije -> "-"
                        string destinacija =
                            forma.Tip == "Krstarenja" ? "-" : (forma.Destinacija ?? "-");

                        string detalji = DetaljiText(
                            forma.Tip,
                            tipSmestaja: forma.TipSmestaja,
                            tipPrevoza: forma.TipPrevoza,
                            dodatneAkt: forma.DodatneAktivnosti,
                            vodic: forma.Vodic,
                            brod: forma.Brod,
                            ruta: forma.Ruta,
                            datumPolaska: forma.DatumPolaska
                        );

                        dgv.Rows.Add(
                            forma.Naziv,
                            forma.Cena,
                            forma.Tip,
                            destinacija,
                            detalji
                        );
                    }
                };

                // ✎ Izmeni
                btnIzmeni.Click += (s, e) =>
                {
                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberi paket u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var row = dgv.SelectedRows[0];

                    string naziv = row.Cells["Naziv"].Value?.ToString() ?? "";
                    string tipPaketa = row.Cells["Tip"].Value?.ToString() ?? "";
                    string cena = row.Cells["Cena"].Value?.ToString() ?? "";
                    string destinacija = row.Cells["Destinacija"].Value?.ToString() ?? "";

                    // Napomena: iz kolone "Detalji" ne možemo pouzdano rekonstruisati sva pojedinačna polja,
                    // pa u formu za izmenu šaljemo ono što imamo (tip + osnovna polja).
                    // Korisnik može popuniti/korigovati specifična polja po tipu.
                    var forma = new FormaIzmenaPaketa(
                        naziv,
                        tipPaketa,
                        cena,
                        destinacija // za more/planine/ekskurzije; za krstarenja ostaje "-"
                                    // Ostala specifična polja ostavljamo prazna,
                                    // pošto "Detalji" je formatiran tekst (nije struktuirano).
                    );

                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        // Destinacija opet zavisi od tipa
                        string novaDest =
                            forma.Tip == "Krstarenja" ? "-" : (forma.Destinacija ?? "-");

                        string detalji = DetaljiText(
                            forma.Tip,
                            tipSmestaja: forma.TipSmestaja,
                            tipPrevoza: forma.TipPrevoza,
                            dodatneAkt: forma.DodatneAktivnosti,
                            vodic: forma.Vodic,
                            brod: forma.Brod,
                            ruta: forma.Ruta,
                            datumPolaska: forma.DatumPolaska
                        );

                        row.Cells["Naziv"].Value = forma.Naziv;
                        row.Cells["Cena"].Value = forma.Cena;
                        row.Cells["Tip"].Value = forma.Tip;
                        row.Cells["Destinacija"].Value = novaDest;
                        row.Cells["Detalji"].Value = detalji;
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

                
                dgv.Columns["Ime"].Width = 140;
                dgv.Columns["Prezime"].Width = 160;
                dgv.Columns["Paket"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns["Datum"].Width = 120;
                dgv.Columns["Status"].Width = 120;

                
                dgv.Rows.Add("Pera", "Perić", "Grčka - Letovanje", "12.7.2025", "Potvrđeno");

                btnDodaj.Click += (s, e) =>
                {
                    var forma = new FormaNovaRezervacija();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows.Add(
                            forma.Ime, forma.Prezime, forma.Paket,
                            forma.Datum.ToShortDateString(), forma.Status
                        );
                    }
                };
            }
        }
    }

}

