using Org.BouncyCastle.Pqc.Crypto.Lms;
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
    public partial class FormaIzborPaketa : Form
    {
        // Vraćamo samo ovo spolja (nema nove globalne klase):
        public class SelectedPackage
        {
            public string Tip { get; set; }
            public string Naziv { get; set; }
            public string Display { get; set; }     // npr "599€ – Krf – Avion" ili "1299€ – MSC Opera – Split–… – 15.06.2026"
            public string Destinacija { get; set; } // za kolonu "Destinacija" u Rezervacijama
        }

        public SelectedPackage Izabrani { get; private set; }

        private ComboBox cboTip;
        private DataGridView dgv;
        private Button btnIzaberi, btnOdustani;

        // Čuvamo tabele po tipu
        private readonly string[] _tipovi = { "Aranžman za more", "Aranžman za planine", "Ekskurzije", "Krstarenja" };
        private readonly DataSet _ds = new DataSet();

        public FormaIzborPaketa()
        {
            Text = "Izbor paketa";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 520);
            BackColor = Color.White;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(1200, 520);

            // UI
            cboTip = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = 12,
                Top = 12,
                Width = 280
            };
            Controls.Add(cboTip);

            dgv = new DataGridView
            {
                Left = 12,
                Top = 48,
                Width = ClientSize.Width - 24,
                Height = ClientSize.Height - 110,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
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
            Controls.Add(dgv);

            btnIzaberi = new Button
            {
                Text = "Izaberi paket",
                Left = ClientSize.Width - 240,
                Top = ClientSize.Height - 50,
                Width = 110,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Enabled = false
            };
            btnIzaberi.Click += (s, e) => OnIzaberi();
            Controls.Add(btnIzaberi);

            btnOdustani = new Button
            {
                Text = "Otkaži",
                Left = ClientSize.Width - 120,
                Top = ClientSize.Height - 50,
                Width = 110,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnOdustani.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnOdustani);

            // Podaci
            NapraviTabeleSaDemoPodacima();

            // Tipovi
            cboTip.Items.AddRange(_tipovi);
            if (cboTip.Items.Count > 0) cboTip.SelectedIndex = 0;
            cboTip.SelectedIndexChanged += (s, e) => PrikaziTabeluZaTip((string)cboTip.SelectedItem);

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.ClearSelection();
                    dgv.Rows[e.RowIndex].Selected = true;
                    btnIzaberi.Enabled = true;
                }
            };

            // init
            PrikaziTabeluZaTip(_tipovi[0]);
        }

        private void NapraviTabeleSaDemoPodacima()
        {
            // Aranžman za more
            var more = new DataTable("Aranžman za more");
            more.Columns.Add("Naziv");
            more.Columns.Add("Cena");
            more.Columns.Add("Tip");
            more.Columns.Add("Destinacija");
            more.Columns.Add("Tip smeštaja");
            more.Columns.Add("Tip prevoza");
            more.Rows.Add("Krf - leto 2026", "599€", "Aranžman za more", "Krf", "Hotel 4*", "Avion");
            _ds.Tables.Add(more);

            // Aranžman za planine
            var planine = new DataTable("Aranžman za planine");
            planine.Columns.Add("Naziv");
            planine.Columns.Add("Cena");
            planine.Columns.Add("Tip");
            planine.Columns.Add("Destinacija");
            planine.Columns.Add("Tip smeštaja");
            planine.Columns.Add("Dodatne aktivnosti");
            planine.Rows.Add("Kopaonik vikend", "199€", "Aranžman za planine", "Kopaonik", "Apartman", "Ski pass");
            _ds.Tables.Add(planine);

            // Ekskurzije
            var ekskurzije = new DataTable("Ekskurzije");
            ekskurzije.Columns.Add("Naziv");
            ekskurzije.Columns.Add("Cena");
            ekskurzije.Columns.Add("Tip");
            ekskurzije.Columns.Add("Destinacija");
            ekskurzije.Columns.Add("Tip prevoza");
            ekskurzije.Columns.Add("Vodič");
            ekskurzije.Rows.Add("Beč jednodnevna", "89€", "Ekskurzije", "Beč", "Autobus", "Licencirani vodič");
            _ds.Tables.Add(ekskurzije);

            // Krstarenja
            var krstarenja = new DataTable("Krstarenja");
            krstarenja.Columns.Add("Naziv");
            krstarenja.Columns.Add("Cena");
            krstarenja.Columns.Add("Tip");
            krstarenja.Columns.Add("Brod");
            krstarenja.Columns.Add("Ruta");
            krstarenja.Columns.Add("Polazak"); // string "dd.MM.yyyy"
            krstarenja.Rows.Add("Jadransko krstarenje", "1299€", "Krstarenja", "MSC Opera", "Split–Kotor–Krf–Bari", "15.06.2026");
            _ds.Tables.Add(krstarenja);
        }

        private void PrikaziTabeluZaTip(string tip)
        {
            var dt = _ds.Tables[tip];
            dgv.Columns.Clear();
            dgv.DataSource = null;

            // Kreiraj kolone istim redom
            foreach (DataColumn c in dt.Columns)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = c.ColumnName,
                    HeaderText = c.ColumnName,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };

                // širine slične tvojim
                switch (c.ColumnName)
                {
                    case "Naziv": col.Width = tip == "Krstarenja" ? 240 : (tip == "Ekskurzije" ? 220 : 200); break;
                    case "Cena": col.Width = 100; break;
                    case "Tip": col.Width = 160; break;
                    case "Destinacija": col.Width = tip == "Ekskurzije" ? 200 : 180; break;
                    case "Tip smeštaja": col.Width = 160; break;
                    case "Tip prevoza": col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "Dodatne aktivnosti": col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "Vodič": col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "Brod": col.Width = 180; break;
                    case "Ruta": col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "Polazak": col.Width = 130; break;
                }

                dgv.Columns.Add(col);
            }

            // Ručno ubaci redove
            dgv.Rows.Clear();
            foreach (DataRow r in dt.Rows)
            {
                dgv.Rows.Add(r.ItemArray);
            }

            dgv.ClearSelection();
            btnIzaberi.Enabled = false;
        }

        private void OnIzaberi()
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Moraš barem jedan red da selektuješ.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var tip = (string)cboTip.SelectedItem;
            var row = dgv.SelectedRows[0];

            // Čitaj polja po tipu i napravi Display + Destinacija
            string naziv = row.Cells["Naziv"].Value?.ToString() ?? "";
            string cena = row.Cells["Cena"].Value?.ToString() ?? "";

            string display, destinacija;

            if (tip == "Krstarenja")
            {
                var brod = row.Cells["Brod"].Value?.ToString() ?? "";
                var ruta = row.Cells["Ruta"].Value?.ToString() ?? "";
                var polazak = row.Cells["Polazak"].Value?.ToString() ?? "";
                display = $"{cena} – {brod} – {ruta} – {polazak}";
                destinacija = !string.IsNullOrWhiteSpace(ruta) ? ruta : (!string.IsNullOrWhiteSpace(brod) ? brod : "-");
            }
            else if (tip == "Ekskurzije")
            {
                var dest = row.Cells["Destinacija"].Value?.ToString() ?? "";
                var tpv = row.Cells["Tip prevoza"].Value?.ToString() ?? "";
                display = $"{cena} – {dest} – {tpv}";
                destinacija = string.IsNullOrWhiteSpace(dest) ? "-" : dest;
            }
            else if (tip == "Aranžman za planine")
            {
                var dest = row.Cells["Destinacija"].Value?.ToString() ?? "";
                var akt = row.Cells["Dodatne aktivnosti"].Value?.ToString() ?? "";
                display = $"{cena} – {dest} – {akt}";
                destinacija = string.IsNullOrWhiteSpace(dest) ? "-" : dest;
            }
            else // Aranžman za more
            {
                var dest = row.Cells["Destinacija"].Value?.ToString() ?? "";
                var tpv = row.Cells["Tip prevoza"].Value?.ToString() ?? "";
                display = $"{cena} – {dest} – {tpv}";
                destinacija = string.IsNullOrWhiteSpace(dest) ? "-" : dest;
            }

            Izabrani = new SelectedPackage
            {
                Tip = tip,
                Naziv = naziv,
                Display = display,
                Destinacija = destinacija
            };

            DialogResult = DialogResult.OK;
        }
    }
}