using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PathFinders.Patterns.Facade;
using PathFinders.Services;
using PathFinders.Models;
using PathFinders.Patterns.Builder;
using System.Xml.Linq;
using System.Text.Json;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;


namespace PathFinders.GUI
{
    public partial class MainScreen : Form
    {
        private Panel sidebar;
        private Panel mainPanel;
        private Label lblLogo;
        private Button btnKlijenti, btnPaketi, btnRezervacije, btnIzlaz;
        private Button aktivnoDugme = null;
        private Panel indikator;

        // Panel za klijente
        private Panel klijentiPanel;
        private DataGridView dgvKlijenti;
        private Button btnDodaj, btnIzmeni, btnUndo, btnRedo;
        private Button btnUsluge;
        private TextBox txtPretraga;
        private readonly Size _baseClientSize;

        
        private readonly string _agencyName;
        private readonly IDatabaseService _dbService;
        private readonly TravelAgencyFacade _facade;

        public MainScreen(string agencyName, IDatabaseService dbService)
        {
            
            _agencyName = agencyName;
            _dbService = dbService;
            _facade = new TravelAgencyFacade(_dbService);
            
            this.Text = _agencyName;
            
       
            // Forma
            this.Text = "Moja Turistička Agencija";
            this.Size = new Size(1400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            _baseClientSize = this.ClientSize;
            this.MinimumSize = this.Size;

            // Sidebar
            sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 220;
            sidebar.BackColor = Color.FromArgb(45, 62, 80);

            // Logo
            lblLogo = new Label();
            lblLogo.Text = _agencyName; 
            lblLogo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Dock = DockStyle.Top;
            lblLogo.Height = 80;

            // Indikator
            indikator = new Panel();
            indikator.Size = new Size(5, 50);
            indikator.BackColor = Color.White;
            indikator.Visible = false;
            sidebar.Controls.Add(indikator);

            // Dugmad u sidebaru
            btnKlijenti = NapraviDugme("👥 Klijenti", 100);
            btnPaketi = NapraviDugme("📦 Paketi", 160);
            btnRezervacije = NapraviDugme("📅 Rezervacije", 220);
            btnUsluge = NapraviDugme("🛠️ Usluge", 280 );
            btnIzlaz = NapraviDugme("⏻ Izlaz", 340);
            

            sidebar.Controls.Add(lblLogo);
            

            sidebar.Controls.AddRange(new Control[] { btnKlijenti, btnPaketi, btnRezervacije, btnUsluge,btnIzlaz });
            btnUsluge.Click += (s, e) =>
            {
                PrikaziTabelu("Usluge");
                OznaciDugme(btnUsluge);
            };

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
            btnIzlaz.Click += (s, e) =>
            {
                this.Hide();
                var configScreen = new FormaIzborBaze();
                configScreen.Show();
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

        private async Task LoadClientsAsync()
        {
            //dgvKlijenti.Rows.Clear();
            //var clients = await _facade.GetClientsAsync();

            //foreach (var c in clients)
            //{

            //    dgvKlijenti.Rows.Add(
            //        c.Id,
            //        c.FirstName,
            //        c.LastName,
            //        c.PassportNumber,
            //        c.DateOfBirth.ToShortDateString(),
            //        c.Email,
            //        c.PhoneNumber
            //    );
            //}

            dgvKlijenti.Rows.Clear();

            var clients = await _facade.GetClientsAsync();
            foreach (var c in clients)
            {
                int rowIndex = dgvKlijenti.Rows.Add();
                var row = dgvKlijenti.Rows[rowIndex];

                row.Cells["ID"].Value = c.Id; // <- ključno
                row.Cells["Ime"].Value = c.FirstName;
                row.Cells["Prezime"].Value = c.LastName;
                row.Cells["BrojPasosa"].Value = c.PassportNumber;
                row.Cells["DatumRodjenja"].Value = c.DateOfBirth.ToShortDateString();
                row.Cells["Email"].Value = c.Email;
                row.Cells["Telefon"].Value = c.PhoneNumber;
            }

        }

        private void PopulateClientsGrid(IEnumerable<Client> clients)
        {
            dgvKlijenti.Rows.Clear();

            foreach (var c in clients)
            {
                int rowIndex = dgvKlijenti.Rows.Add();
                var row = dgvKlijenti.Rows[rowIndex];

                row.Cells["ID"].Value = c.Id;
                row.Cells["Ime"].Value = c.FirstName;
                row.Cells["Prezime"].Value = c.LastName;
                row.Cells["BrojPasosa"].Value = c.PassportNumber;
                row.Cells["DatumRodjenja"].Value = c.DateOfBirth.ToShortDateString();
                row.Cells["Email"].Value = c.Email;
                row.Cells["Telefon"].Value = c.PhoneNumber;
            }

            dgvKlijenti.ClearSelection();
            dgvKlijenti.CurrentCell = null;
        }

        private static string FormatPrice(decimal? price)
             => price.HasValue ? $"{price.Value:0.##}€" : "";

        private static string ToDisplayDate(DateTime? dt)
            => dt.HasValue ? dt.Value.ToString("dd.MM.yyyy") : "";

        private void FillGrid(DataGridView grid, string type)
        {
            grid.Rows.Clear();
            var items = _facade.GetPackagesByType(type);
            switch (type)
            {
                
                case "More": // u bazi

                    string tipSmestaja = "";
                    string tipPrevoza = "";
                    string dest = "";
                    foreach (var p in items)
                    {
                        using var doc = JsonDocument.Parse(p.Details);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("TipSmestaja", out var ts)) tipSmestaja = ts.GetString() ?? "";
                        if (root.TryGetProperty("TipPrevoza", out var tp)) tipPrevoza = tp.GetString() ?? "";
                        if (root.TryGetProperty("Destinacija", out var ds)) dest = ds.GetString() ?? "";


                        grid.Rows.Add(
                            p.Id,
                            p.Name,
                            $"{p.Price:0.##}€",
                            "Aranžman za more",      // u UI
                            dest,
                            tipSmestaja,
                            tipPrevoza
                        );
                    }
                    break;

                case "Planine":
                    tipSmestaja = "";
                    tipPrevoza = "";
                    dest = "";
                    string dodatneAktivnosti = "";
                    List<string> aktivnosti = new();

                    foreach (var p in items)
                    {
                        using var doc = JsonDocument.Parse(p.Details);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("TipSmestaja", out var ts)) tipSmestaja = ts.GetString() ?? "";
                        if (root.TryGetProperty("TipPrevoza", out var tp)) tipPrevoza = tp.GetString() ?? "";
                        if (root.TryGetProperty("Destinacija", out var ds)) dest = ds.GetString() ?? "";
                        root.TryGetProperty("DodatneAktivnosti", out var da);
                        aktivnosti.Clear();
                        foreach (var el in da.EnumerateArray())
                        {
                            if (el.ValueKind == JsonValueKind.String)
                            {
                                var s = el.GetString();
                                if (!string.IsNullOrWhiteSpace(s)) aktivnosti.Add(s);
                            }
                        }
                        string spojeno = string.Join("-", aktivnosti);
                        grid.Rows.Add(
                            p.Id,
                            p.Name,
                            $"{p.Price:0.##}€",
                            "Aranžman za planine",
                            dest,
                            tipSmestaja,
                            tipPrevoza,
                            spojeno
                        );
                    }
                    break;

                case "Ekskurzije":
                    dest = "";
                    tipPrevoza = "";
                    string vodic = "";
                    int trajanje = 0;

                    
                    foreach (var p in items)
                    {
                        using var doc = JsonDocument.Parse(p.Details);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("TipPrevoza", out var tp)) tipPrevoza = tp.GetString() ?? "";
                        if (root.TryGetProperty("Destinacija", out var ds)) dest = ds.GetString() ?? "";
                        if (root.TryGetProperty("TrajanjeUDanima", out var tr) && tr.TryGetInt32(out var t))
                        {
                            trajanje = t;
                        }
                        if (root.TryGetProperty("Vodic", out var vo)) vodic = vo.GetString() ?? "";
                        grid.Rows.Add(
                            p.Id,
                            p.Name,
                            $"{p.Price:0.##}€",
                            "Ekskurzije",
                            dest,
                            tipPrevoza,
                            vodic,
                            trajanje
                        );
                    }
                    break;

                case "Krstarenja":
                    string brod = "";
                    string ruta = "";
                    DateTime polazak = new DateTime();
                    string tipKabine = "";

                    foreach (var p in items)
                    {
                        using var doc = JsonDocument.Parse(p.Details);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("Brod", out var tp)) brod = tp.GetString() ?? "";
                        if (root.TryGetProperty("Ruta", out var ds)) ruta = ds.GetString() ?? "";
                        if (root.TryGetProperty("TipKabine", out var tk)) tipKabine = ds.GetString() ?? "";

                        root.TryGetProperty("DatumPolaska", out var dp);
                        var s = dp.GetString();

                        if (!DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal,
                                       out var d))
                        {
                            string[] formats = {
                        "dd.MM.yyyy", "dd.MM.yyyy HH:mm",
                        "yyyy-MM-dd", "yyyy-MM-dd HH:mm",
                        "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ"
                    };
                            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
                                                       DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal,
                                                       out var dex))
                                d = dex;
                        }
                        grid.Rows.Add(
                            p.Id,
                            p.Name,
                            $"{p.Price:0.##}€",
                            "Krstarenja",
                            brod,
                            ruta,
                            (d.Date).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                            tipKabine
                        );
                    }
                    break;
            }
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

            
            
            Button btnOtkazi = null;
            if (tip == "Rezervacije")
            {
                btnOtkazi = NapraviAkcijskoDugme("✖ Otkaži", 450, 10);
                Button btnUndo = NapraviAkcijskoDugme("↶ Undo", 230, 10);
                Button btnRedo = NapraviAkcijskoDugme("↷ Redo", 340, 10);
                topBar.Controls.Add(btnUndo);
                topBar.Controls.Add(btnRedo);
                topBar.Controls.Add(btnOtkazi);
            }


            this.txtPretraga = new TextBox
            {
                PlaceholderText = $"Pretraga ({tip})...",
                Width = 320,
                Top = 13,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            topBar.Controls.AddRange(new Control[] { btnDodaj, btnIzmeni, btnUndo, btnRedo, txtPretraga });

            if (tip == "Paketi")
            {
                // nema pretrage na paketima
                txtPretraga.Visible = false;
                // i ne treba da se računa layout prema txtPretraga
                topBar.Resize -= (s, e) => { txtPretraga.Left = topBar.ClientSize.Width - txtPretraga.Width - 10; };
            }

            topBar.Resize += (s, e) =>
            {
                txtPretraga.Left = topBar.ClientSize.Width - txtPretraga.Width - 10;
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            };


            frame.Controls.Add(dgv);
            frame.Controls.Add(topBar);
            wrapper.Controls.Add(frame);
            mainPanel.Controls.Add(wrapper);


            if (tip == "Klijenti")
            {
                this.dgvKlijenti = dgv;
                var colId = new DataGridViewTextBoxColumn
                {
                    Name = "ID",
                    HeaderText = "ID",
                    Visible = false,
                    ValueType = typeof(int) // ili typeof(string) ako je GUID/tekst
                };
                dgv.Columns.Add(colId);
                dgv.Columns.Add("Ime", "Ime");
                dgv.Columns.Add("Prezime", "Prezime");
                dgv.Columns.Add("BrojPasosa", "Broj pasoša");
                dgv.Columns.Add("DatumRodjenja", "Datum rođenja");
                dgv.Columns.Add("Email", "Email");
                dgv.Columns.Add("Telefon", "Telefon");
                dgv.Columns.Add("ID", "ID");

                dgv.Columns["Ime"].Width = 160;
                dgv.Columns["Prezime"].Width = 170;
                dgv.Columns["BrojPasosa"].Width = 120;
                dgv.Columns["DatumRodjenja"].Width = 120;
               

                dgv.Columns["Email"].MinimumWidth = 220;
                dgv.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dgv.Columns["Telefon"].Width = 140;
                dgv.Columns["Telefon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns["ID"].Visible = false;


                dgv.CellClick += (s2, e2) =>
                {
                    if (e2.RowIndex >= 0)
                    {
                        dgv.ClearSelection();
                        dgv.Rows[e2.RowIndex].Selected = true;
                    }
                };


                LoadClientsAsync();
                


                btnDodaj.Click += (s, e) =>
                {
                    var forma = new FormaNoviKlijent();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        
                        Client client = new Client
                        {

                            FirstName = forma.Ime,
                            LastName = forma.Prezime,
                            PassportNumber = forma.BrojPasosa,
                            DateOfBirth = forma.DatumRodjenja,
                            Email = forma.Email,
                            PhoneNumber = forma.Telefon
                        };
                        _facade.AddClientAsync(client);
                        LoadClientsAsync();
                    }
                };


                btnIzmeni.Click += async (s, e) =>
                {
                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberi klijenta u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var row = dgv.SelectedRows[0];

                    // 1) Bezbedno čitanje ID-a iz skrivene kolone
                    var rawId = row.Cells["ID"]?.Value;
                    int clientId;
                    if (rawId is int i) clientId = i;
                    else if (!int.TryParse(Convert.ToString(rawId), out clientId))
                    {
                        MessageBox.Show("ID klijenta nije validan.", "Greška",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    
                    string ime = row.Cells["Ime"]?.Value?.ToString() ?? "";
                    string prezime = row.Cells["Prezime"]?.Value?.ToString() ?? "";
                    string brojPasosa = row.Cells["BrojPasosa"]?.Value?.ToString() ?? "";
                    string email = row.Cells["Email"]?.Value?.ToString() ?? "";
                    string telefon = row.Cells["Telefon"]?.Value?.ToString() ?? "";

                   
                    DateTime datumRodjenja;
                    var rawDob = row.Cells["DatumRodjenja"]?.Value;
                    if (rawDob is DateTime dt) datumRodjenja = dt.Date;
                    else if (!DateTime.TryParse(Convert.ToString(rawDob), out datumRodjenja))
                        datumRodjenja = DateTime.Today;

                    
                    var forma = new FormaIzmenaKlijenta(ime, prezime, brojPasosa, email, telefon, datumRodjenja);

                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        var client = new Client
                        {
                            Id = clientId,
                            FirstName = forma.Ime,
                            LastName = forma.Prezime,
                            PassportNumber = forma.BrojPasosa,
                            DateOfBirth = forma.DatumRodjenja,
                            Email = forma.Email,
                            PhoneNumber = forma.Telefon
                        };

                        try
                        {
                            await _facade.UpdateClientAsync(client);
                            await LoadClientsAsync(); 
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Greška pri izmeni klijenta:\n" + ex.Message, "Greška",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };
                txtPretraga.TextChanged += async (s, e) =>
                {
                    string q = (txtPretraga.Text ?? "").Trim();

                    if (string.IsNullOrEmpty(q))
                    {
                        LoadClientsAsync();
                    }
                    else if (q.IndexOf(' ') < 0)
                    {
                        Client byPassport = await _facade.GetClientByPassportNumberAsync(q);
                        if (byPassport != null)
                        {
                            
                            PopulateClientsGrid(new List<Client> { byPassport });
                        }
                        else
                        {
                            var byName = await _facade.GetClientsByName(q, ""); // samo ime
                            PopulateClientsGrid(byName);
                        }
                    }
                    else
                    {
                        var parts = q.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string first = parts[0];
                        string last = string.Join(" ", parts.Skip(1));

                        var byName = await _facade.GetClientsByName(first, last);
                        PopulateClientsGrid(byName);
                    }
                };

                //int ukupnaSirinaKolona = 0;
                //foreach (DataGridViewColumn col in dgv.Columns)
                //{
                //    ukupnaSirinaKolona += col.Width;
                //}


                //ukupnaSirinaKolona += 60;


                //this.FormBorderStyle = FormBorderStyle.FixedSingle;
                //this.MaximizeBox = false;
                //this.ClientSize = new Size(ukupnaSirinaKolona + sidebar.Width, this.ClientSize.Height);
            }
            else if (tip == "Paketi")
            {

                var paketiHost = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };
                frame.Controls.Remove(dgv);
                frame.Controls.Add(paketiHost);
                paketiHost.BringToFront();

                var btnObrisi = NapraviAkcijskoDugme("🗑 Obriši", 230, 10);
                topBar.Controls.Add(btnObrisi);



                var cboTip = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 220,
                    Top = 12
                };
                topBar.Controls.Add(cboTip);

                topBar.Resize += (s, e2) =>
                {
                    // bez txtPretraga – veži combobox na desni rub
                    cboTip.Left = topBar.ClientSize.Width - cboTip.Width - 10;
                };
                cboTip.Left = topBar.ClientSize.Width - cboTip.Width - 10;

                //topBar.Resize += (s, e2) =>
                //{
                //    txtPretraga.Left = topBar.ClientSize.Width - txtPretraga.Width - 10;
                //    cboTip.Left = txtPretraga.Left - cboTip.Width - 10;
                //};

                //txtPretraga.Left = topBar.ClientSize.Width - txtPretraga.Width - 10;
                //cboTip.Left = txtPretraga.Left - cboTip.Width - 10;


                var tips = new[] { "Aranžman za more", "Aranžman za planine", "Ekskurzije", "Krstarenja" };


                var columnsByType = new Dictionary<string, (string name, int width, DataGridViewAutoSizeColumnMode mode, bool visible)[]>
                {
                    ["Aranžman za more"] = new[]
                    {
                        ("ID", 0, DataGridViewAutoSizeColumnMode.None, false),
                        ("Naziv", 200, DataGridViewAutoSizeColumnMode.None,true),
                        ("Cena", 100, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Destinacija", 180, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip smeštaja", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip prevoza", 160, DataGridViewAutoSizeColumnMode.Fill,true)
                    },
                    ["Aranžman za planine"] = new[]
                    {
                        ("ID", 0, DataGridViewAutoSizeColumnMode.None, false),
                        ("Naziv", 200, DataGridViewAutoSizeColumnMode.None,true),
                        ("Cena", 80, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Destinacija", 180, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip smeštaja", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip prevoza", 130, DataGridViewAutoSizeColumnMode.None,true),
                        ("Dodatne aktivnosti", 200, DataGridViewAutoSizeColumnMode.Fill,true)
                    },
                    ["Ekskurzije"] = new[]
                    {

                        ("ID", 0, DataGridViewAutoSizeColumnMode.None, false),
                        ("Naziv", 220, DataGridViewAutoSizeColumnMode.None,true),
                        ("Cena", 100, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Destinacija", 200, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip prevoza", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Vodič", 200, DataGridViewAutoSizeColumnMode.Fill,true),
                        ("Trajanje", 100, DataGridViewAutoSizeColumnMode.None,true)
                    },
                    ["Krstarenja"] = new[]
                    {
                        ("ID", 0, DataGridViewAutoSizeColumnMode.None, false),
                        ("Naziv", 240, DataGridViewAutoSizeColumnMode.None,true),
                        ("Cena", 100, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip", 160, DataGridViewAutoSizeColumnMode.None,true),
                        ("Brod", 180, DataGridViewAutoSizeColumnMode.None,true),
                        ("Ruta", 260, DataGridViewAutoSizeColumnMode.Fill,true),
                        ("Polazak", 130, DataGridViewAutoSizeColumnMode.None,true),
                        ("Tip kabine", 100, DataGridViewAutoSizeColumnMode.None,true)
                    }
                };
                var grids = new Dictionary<string, DataGridView>();

                DataGridView MakeGrid((string name, int width, DataGridViewAutoSizeColumnMode mode, bool visible)[] cols)
                {
                    var g = new DataGridView
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
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                        Visible = false
                    };

                    foreach (var c in cols)
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = c.name,
                            HeaderText = c.name,
                            AutoSizeMode = c.mode,
                            Visible = c.visible
                        };
                        if (c.mode == DataGridViewAutoSizeColumnMode.None) col.Width = c.width;

                        // safety – dodatno “zakucaj” ID kolonu
                        if (string.Equals(c.name, "ID", StringComparison.OrdinalIgnoreCase))
                        {
                            col.Visible = false;
                            col.Width = 0;
                            col.ReadOnly = true;
                        }

                        g.Columns.Add(col);
                    }


                    g.CellClick += (ss, ee) =>
                    {
                        if (ee.RowIndex >= 0)
                        {
                            foreach (var other in grids.Values)
                                if (!ReferenceEquals(other, g)) other.ClearSelection();
                            g.ClearSelection();
                            g.Rows[ee.RowIndex].Selected = true;
                        }
                    };
                    return g;
                }



                foreach (var t in tips)
                {
                    var g = MakeGrid(columnsByType[t]);
                    grids[t] = g;
                    paketiHost.Controls.Add(g);
                    g.BringToFront();
                }


                cboTip.Items.AddRange(tips);
                if (cboTip.Items.Count > 0) cboTip.SelectedIndex = 0;


                void ShowType(string type)
                {
                    foreach (var kv in grids)
                        kv.Value.Visible = (kv.Key == type);
                }


                ShowType((string)cboTip.SelectedItem);


                cboTip.SelectedIndexChanged += (s, e2) =>
                {
                    var tsel = (string)cboTip.SelectedItem;
                    ShowType(tsel);
                    txtPretraga.Text = "";

                    foreach (var g in grids.Values)
                    {
                        g.ClearSelection();
                        g.CurrentCell = null;
                    }
                };

                // --- DEMO podaci (upis direktno u odgovarajuće kolone) ---

                //var more = _facade.GetPackagesByType("More");
                //var planine = _facade.GetPackagesByType("Planine");
                //var ekskurzije = _facade.GetPackagesByType("Ekskurzije");
                //var krstarenja = _facade.GetPackagesByType("Krstarenja");


                FillGrid(grids["Aranžman za more"], "More");
                FillGrid(grids["Aranžman za planine"], "Planine" );
                FillGrid(grids["Ekskurzije"], "Ekskurzije");
                FillGrid(grids["Krstarenja"], "Krstarenja");





                DataGridView CurrentGrid() => grids[(string)cboTip.SelectedItem];


                btnDodaj.Click += (s, e2) =>
                {
                    
                    var forma = new FormaNoviPaket();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {

                        var tipPaketa = forma.Tip;

                        if (tipPaketa == "Aranžman za more")
                        {
                            SeaTravelPackageBuilder sea = new SeaTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.TipSmestaja);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Destinacija, sea);
                            FillGrid(grids["Aranžman za more"], "More");
                        }
                        else if (tipPaketa == "Aranžman za planine")
                        {
                            List<string> lista = (forma.DodatneAktivnosti).Split('-').ToList();
                            MountainTravelPackageBuilder mountain = new MountainTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.TipSmestaja, lista);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Destinacija, mountain);
                            FillGrid(grids["Aranžman za planine"], "Planine");
                        }
                        else if (tipPaketa == "Ekskurzije")
                        {
                            ExcursionTravelPackageBuilder ex = new ExcursionTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.Vodic, forma.Trajanje);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Destinacija, ex);
                            FillGrid(grids["Ekskurzije"], "Ekskurzije");
                        }
                        else if (tipPaketa == "Krstarenja")
                        {
                            CruiseTravelPackageBuilder cruise = new CruiseTravelPackageBuilder(forma.Brod, forma.Ruta, forma.DatumPolaska, forma.TipKabine);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Ruta, cruise);
                            FillGrid(grids["Krstarenja"], "Krstarenja");
                        }

                        cboTip.SelectedItem = tipPaketa;
                    }
                };


                btnIzmeni.Click += (s, e2) =>
                {

                    DataGridView fromGrid = null;
                    DataGridViewRow row = null;
                    foreach (var kv in grids)
                    {
                        if (kv.Value.SelectedRows.Count > 0)
                        {
                            fromGrid = kv.Value;
                            row = kv.Value.SelectedRows[0];
                            break;
                        }
                    }
                    if (row == null)
                    {
                        MessageBox.Show("Prvo izaberi paket u tabeli.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string GetCell(string name) => fromGrid.Columns.Contains(name)
                    ? (row.Cells[name].Value?.ToString() ?? "")
                    : "";

                    DateTime? ParseDate(string s)
                    {
                        if (DateTime.TryParse(s, out var d)) return d;
                        return null;
                    }

                    string naziv = GetCell("Naziv");
                    string tipPaketa = GetCell("Tip");
                    string cena = GetCell("Cena");
                    string id = GetCell("ID");

                    FormaIzmenaPaketa forma = null;


                    if (tipPaketa == "Aranžman za more")
                    {
                        var destinacija = GetCell("Destinacija");
                        var tipSmestaja = GetCell("Tip smeštaja");
                        var tipPrevoza = GetCell("Tip prevoza");

                        forma = new FormaIzmenaPaketa(
                            naziv, tipPaketa, cena,
                            destinacija: destinacija,
                            tipSmestaja: tipSmestaja,
                            tipPrevoza: tipPrevoza
                        );
                    }
                    else if (tipPaketa == "Aranžman za planine")
                    {
                        var destinacija = GetCell("Destinacija");
                        var tipSmestaja = GetCell("Tip smeštaja");
                        var akt = GetCell("Dodatne aktivnosti");
                        var tipPrevoza = GetCell("TipPrevoza");

                        forma = new FormaIzmenaPaketa(
                            naziv, tipPaketa, cena,
                            destinacija: destinacija,
                            tipSmestaja: tipSmestaja,
                            dodatneAkt: akt,
                            tipPrevoza: tipPrevoza
                        );
                    }
                    else if (tipPaketa == "Ekskurzije")
                    {
                        var destinacija = GetCell("Destinacija");
                        var tipPrevoza = GetCell("Tip prevoza");
                        var vodic = GetCell("Vodič");
                        var trajanje = GetCell("Trajanje");

                        forma = new FormaIzmenaPaketa(
                            naziv, tipPaketa, cena,
                            destinacija: destinacija,
                            tipPrevoza: tipPrevoza,
                            vodic: vodic,
                            trajanjeUDanima: int.Parse(trajanje)
                        );
                    }
                    else if (tipPaketa == "Krstarenja")
                    {
                        var brod = GetCell("Brod");
                        var ruta = GetCell("Ruta");
                        var polStr = GetCell("Polazak");
                        var pol = ParseDate(polStr);
                        var tipKabine = GetCell("TipKabine");

                        forma = new FormaIzmenaPaketa(
                            naziv, tipPaketa, cena,
                            brod: brod,
                            ruta: ruta,
                            datumPolaska: pol,
                            tipKabine: tipKabine
                        );
                    }
                    else
                    {
                        // fallback (ne bi trebalo da se desi)
                        forma = new FormaIzmenaPaketa(naziv, tipPaketa, cena);
                    }


                    if (forma.ShowDialog() != DialogResult.OK)
                    {
                        tipPaketa = forma.Tip;

                        if (tipPaketa == "Aranžman za more")
                        {
                            SeaTravelPackageBuilder sea = new SeaTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.TipSmestaja);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Destinacija, sea);
                            FillGrid(grids["Aranžman za more"], "More");
                        }
                        else if (tipPaketa == "Aranžman za planine")
                        {
                            List<string> lista = (forma.DodatneAktivnosti).Split('-').ToList();
                            MountainTravelPackageBuilder mountain = new MountainTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.TipSmestaja, lista);
                            //_facade.UpdatePackageAsync(mountain);
                            FillGrid(grids["Aranžman za planine"], "Planine");
                        }
                        else if (tipPaketa == "Ekskurzije")
                        {
                            ExcursionTravelPackageBuilder ex = new ExcursionTravelPackageBuilder(forma.Destinacija, forma.TipPrevoza, forma.Vodic, forma.TrajanjeUDanima);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Destinacija, ex);
                            FillGrid(grids["Ekskurzije"], "Ekskurzije");
                        }
                        else if (tipPaketa == "Krstarenja")
                        {
                            CruiseTravelPackageBuilder cruise = new CruiseTravelPackageBuilder(forma.Brod, forma.Ruta, forma.DatumPolaska, forma.TipKabine);
                            _facade.AddPackageAsync(forma.Naziv, decimal.Parse(forma.Cena), forma.Ruta, cruise);
                            FillGrid(grids["Krstarenja"], "Krstarenja");
                        }

                        cboTip.SelectedItem = tipPaketa;
                    }




                };

                btnObrisi.Click += async (s, e2) =>
                {
                    
                    DataGridView fromGrid = null;
                    DataGridViewRow row = null;
                    foreach (var kv in grids)
                    {
                        if (kv.Value.Visible && kv.Value.SelectedRows.Count > 0)
                        {
                            fromGrid = kv.Value;
                            row = kv.Value.SelectedRows[0];
                            break;
                        }
                    }

                    if (row == null)
                    {
                        MessageBox.Show("Prvo izaberi paket u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    
                    int id;
                    var rawId = row.Cells["ID"]?.Value;
                    if (rawId is int i) id = i;
                    else if (!int.TryParse(Convert.ToString(rawId), out id))
                    {
                        MessageBox.Show("ID paketa nije validan.", "Greška",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var naziv = row.Cells["Naziv"]?.Value?.ToString() ?? "(bez naziva)";

                    var confirm = MessageBox.Show(
                        $"Obrisati paket „{naziv}” (ID: {id})?",
                        "Potvrda brisanja",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirm != DialogResult.Yes) return;

                    try
                    {
                        await _facade.DeletePackageAsync(id);

                        
                        var tsel = (string)cboTip.SelectedItem;
                        switch (tsel)
                        {
                            case "Aranžman za more":
                                FillGrid(grids["Aranžman za more"], "More");
                                break;
                            case "Aranžman za planine":
                                FillGrid(grids["Aranžman za planine"], "Planine");
                                break;
                            case "Ekskurzije":
                                FillGrid(grids["Ekskurzije"], "Ekskurzije");
                                break;
                            case "Krstarenja":
                                FillGrid(grids["Krstarenja"], "Krstarenja");
                                break;
                        }

                        
                        foreach (var g in grids.Values) { g.ClearSelection(); g.CurrentCell = null; }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Greška pri brisanju paketa:\n" + ex.Message,
                            "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                //txtPretraga.TextChanged += (s, e2) =>
                //{
                //    var g = CurrentGrid();
                //    string q = (txtPretraga.Text ?? "").Trim().ToLowerInvariant();
                //    foreach (DataGridViewRow r in g.Rows)
                //    {
                //        bool match = string.IsNullOrEmpty(q);
                //        if (!match)
                //        {
                //            foreach (DataGridViewCell c in r.Cells)
                //            {
                //                var val = c.Value?.ToString()?.ToLowerInvariant() ?? "";
                //                if (val.Contains(q)) { match = true; break; }
                //            }
                //        }
                //        r.Visible = match;
                //    }
                //};
            }
            else if (tip == "Rezervacije")
            {

                txtPretraga.Visible = false;


                var cboKlijent = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 260,
                    Top = 12,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                cboKlijent.Items.AddRange(new object[] { "Bojan Kovarbasic", "Bojana Kovarbasic" });
                cboKlijent.SelectedIndex = 0;
                topBar.Controls.Add(cboKlijent);


                topBar.Resize += (s, e2) =>
                {
                    cboKlijent.Left = topBar.ClientSize.Width - cboKlijent.Width - 10;
                };

                cboKlijent.Left = topBar.ClientSize.Width - cboKlijent.Width - 10;

                dgv.Columns.Add("Paket", "Paket");
                dgv.Columns.Add("BrojOsoba", "Broj osoba");
                dgv.Columns.Add("DatumRez", "Datum rezervacije");
                dgv.Columns.Add("Destinacija", "Destinacija");
                dgv.Columns.Add("Usluge", "Usluge");
                dgv.Columns["Paket"].Width = 240;
                dgv.Columns["BrojOsoba"].Width = 80;         // smanjeno
                dgv.Columns["DatumRez"].Width = 120;         // smanjeno
                dgv.Columns["Destinacija"].Width = 160;
                dgv.Columns["Usluge"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                var colKlijent = new DataGridViewTextBoxColumn
                {
                    Name = "Klijent",
                    HeaderText = "Klijent",
                    Visible = false
                };
                dgv.Columns.Add(colKlijent);


                dgv.Columns["Paket"].Width = 260;
                dgv.Columns["BrojOsoba"].Width = 140;
                dgv.Columns["DatumRez"].Width = 220;
                dgv.Columns["Destinacija"].MinimumWidth = 260;
                dgv.Columns["Destinacija"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                dgv.Rows.Add("Grčka - Letovanje", 2, "12.07.2025", "Krf", "Transfer, Osiguranje");
                dgv.Rows.Add("Kopaonik - Zimovanje", 4, "15.01.2026", "Kopaonik", "Ski pass, Sauna");



                dgv.CellClick += (s2, e2) =>
                {
                    if (e2.RowIndex >= 0)
                    {

                        dgv.ClearSelection();
                        dgv.Rows[e2.RowIndex].Selected = true;
                    }
                };

                void ApplyFilter()
                {
                    var sel = cboKlijent.SelectedItem?.ToString() ?? "";
                    foreach (DataGridViewRow r in dgv.Rows)
                    {
                        var klijent = r.Cells["Klijent"].Value?.ToString() ?? "";
                        bool show = string.Equals(klijent, sel, StringComparison.OrdinalIgnoreCase);
                        r.Visible = show;
                    }
                }

                cboKlijent.SelectedIndexChanged += (s, e2) => ApplyFilter();

                ApplyFilter();

                cboKlijent.SelectedIndexChanged += (s, e2) =>
                {
                    string sel = cboKlijent.SelectedItem?.ToString() ?? "";
                    foreach (DataGridViewRow r in dgv.Rows)
                    {
                        var klijent = r.Cells["Klijent"].Value?.ToString() ?? "";
                        r.Visible = string.Equals(klijent, sel, StringComparison.OrdinalIgnoreCase);
                    }


                    dgv.ClearSelection();
                    dgv.CurrentCell = null;
                };

                btnOtkazi.Click += (s, e) =>
                {
                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberite rezervaciju u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var result = MessageBox.Show(
                        "Da li ste sigurni da želite da otkažete rezervaciju?",
                        "Potvrda otkazivanja",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        dgv.Rows.Remove(dgv.SelectedRows[0]);
                    }
                };

                btnDodaj.Click += (s, e2) =>
                {
                    using (var dlg = new FormaNovaRezervacija())
                    {
                        dlg.ShowDialog(this); // samo otvara formu i čeka da se zatvori
                    }
                };

                btnIzmeni.Click += (s, e) =>
                {
                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberite rezervaciju u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var row = dgv.SelectedRows[0];

                    // Bezbedno parsiraj postojeći broj osoba
                    int stariBroj = 1;
                    var cellVal = row.Cells["BrojOsoba"].Value?.ToString() ?? "1";
                    int.TryParse(cellVal, out stariBroj);
                    if (stariBroj < 1) stariBroj = 1;

                    using (var dlg = new FormaIzmenaRezervacije(stariBroj))
                    {
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            row.Cells["BrojOsoba"].Value = dlg.NoviBrojOsoba;

                            // (opciono) ostavi selekciju na istom redu
                            dgv.ClearSelection();
                            row.Selected = true;
                            dgv.CurrentCell = row.Cells["BrojOsoba"];
                        }
                    }
                };

            }
            else if (tip == "Usluge")
            {
                var uslugeHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
                frame.Controls.Remove(dgv);
                frame.Controls.Add(uslugeHost);
                uslugeHost.BringToFront();

                var g = new DataGridView
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
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
                };
                uslugeHost.Controls.Add(g);

                var cNaziv = new DataGridViewTextBoxColumn { Name = "Naziv", HeaderText = "Naziv", Width = 420 };
                var cCena = new DataGridViewTextBoxColumn { Name = "Cena", HeaderText = "Cena", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                g.Columns.AddRange(new DataGridViewColumn[] { cNaziv, cCena });

                g.CellClick += (ss, ee) =>
                {
                    if (ee.RowIndex >= 0)
                    {
                        g.ClearSelection();
                        g.Rows[ee.RowIndex].Selected = true;
                    }
                };

                g.Rows.Add("Transfer do aerodroma", "25€");
                g.Rows.Add("Dodatno osiguranje", "15€");
                g.Rows.Add("Iznajmljivanje opreme", "30€");
                g.Rows.Add("Ulaznica za muzej", "12€");
                g.Rows.Add("Spa paket na brodu", "59€");

                txtPretraga.TextChanged += (s, e2) =>
                {
                    string q = (txtPretraga.Text ?? "").Trim().ToLowerInvariant();
                    foreach (DataGridViewRow r in g.Rows)
                    {
                        bool match = string.IsNullOrEmpty(q);
                        if (!match)
                        {
                            foreach (DataGridViewCell c in r.Cells)
                            {
                                var val = c.Value?.ToString()?.ToLowerInvariant() ?? "";
                                if (val.Contains(q)) { match = true; break; }
                            }
                        }
                        r.Visible = match;
                    }
                };

                btnDodaj.Click += (s, e2) =>
                {
                    using (var dlg = new FormaNovaUsluga())
                    {
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            g.Rows.Add(dlg.Naziv, dlg.Cena);
                            g.ClearSelection();
                            var last = g.Rows[g.Rows.Count - 1];
                            if (last.Visible)
                            {
                                last.Selected = true;
                                g.CurrentCell = last.Cells["Naziv"];
                            }
                        }
                    }
                };

                btnIzmeni.Click += (s, e2) =>
                {
                    if (g.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Prvo izaberite uslugu u tabeli.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var row = g.SelectedRows[0];
                    string naziv = row.Cells["Naziv"].Value?.ToString() ?? "";
                    string cena = row.Cells["Cena"].Value?.ToString() ?? "";

                    using (var dlg = new FormaIzmenaUsluge(naziv, cena))
                    {
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            row.Cells["Naziv"].Value = dlg.Naziv;
                            row.Cells["Cena"].Value = dlg.Cena;
                            g.ClearSelection();
                            row.Selected = true;
                            g.CurrentCell = row.Cells["Naziv"];
                        }
                    }
                };
            }

        }
    }

}