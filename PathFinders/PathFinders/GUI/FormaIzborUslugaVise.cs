using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PathFinders.GUI
{
    public partial class FormaIzborUslugaVise : Form
    {
        public class SelectedService
        {
            public string Naziv { get; set; }
            public string Cena { get; set; }
            public string Display => string.IsNullOrWhiteSpace(Cena) ? Naziv : $"{Naziv} – {Cena}";
        }

        public List<SelectedService> Izabrane { get; private set; } = new List<SelectedService>();

        private TextBox txtPretraga;
        private DataGridView dgv;
        private Button btnIzaberi, btnOdustani;

        public FormaIzborUslugaVise()
        {
            Text = "Izbor usluga";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(760, 520);

            txtPretraga = new TextBox { Left = 12, Top = 12, Width = 360 };
            Controls.Add(txtPretraga);

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
                EditMode = DataGridViewEditMode.EditOnEnter,
                ReadOnly = false,
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

            var cChk = new DataGridViewCheckBoxColumn
            {
                Name = "Selekcija",
                HeaderText = "Selekcija",
                Width = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };
            var cNaziv = new DataGridViewTextBoxColumn { Name = "Naziv", HeaderText = "Naziv", Width = 420, ReadOnly = true };
            var cCena = new DataGridViewTextBoxColumn { Name = "Cena", HeaderText = "Cena", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true };
            dgv.Columns.AddRange(new DataGridViewColumn[] { cChk, cNaziv, cCena });

            btnIzaberi = new Button
            {
                Text = "Izaberi",
                Left = ClientSize.Width - 240,
                Top = ClientSize.Height - 50,
                Width = 110,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnIzaberi.Click += (s, e) => PotvrdiIzbor();
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

            txtPretraga.TextChanged += (s, e) => PrimeniFilter(txtPretraga.Text);

            dgv.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name != "Selekcija")
                {
                    var cell = dgv.Rows[e.RowIndex].Cells["Selekcija"];
                    bool current = Convert.ToBoolean(cell.Value ?? false);
                    cell.Value = !current;
                }
            };

            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "Selekcija")
                    dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { PotvrdiIzbor(); e.Handled = true; }
                else if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; e.Handled = true; }
            };

            UcitajDemo();
        }

        public void LoadServices(IEnumerable<(string Naziv, string Cena)> services)
        {
            dgv.Rows.Clear();
            if (services != null)
                foreach (var (naziv, cena) in services)
                    dgv.Rows.Add(false, naziv ?? "", cena ?? "");
            dgv.ClearSelection();
        }

        private void UcitajDemo()
        {
            dgv.Rows.Clear();
            dgv.Rows.Add(false, "Transfer do aerodroma", "25€");
            dgv.Rows.Add(false, "Dodatno osiguranje", "15€");
            dgv.Rows.Add(false, "Iznajmljivanje opreme", "30€");
            dgv.Rows.Add(false, "Ulaznica za muzej", "12€");
            dgv.Rows.Add(false, "Spa paket na brodu", "59€");
            dgv.Rows.Add(false, "Rani check-in", "20€");
            dgv.Rows.Add(false, "Kasni check-out", "25€");
            dgv.ClearSelection();
        }

        private void PrimeniFilter(string q)
        {
            string needle = (q ?? "").Trim().ToLowerInvariant();
            foreach (DataGridViewRow r in dgv.Rows)
            {
                bool match = string.IsNullOrEmpty(needle);
                if (!match)
                {
                    foreach (DataGridViewCell c in r.Cells.Cast<DataGridViewCell>().Where(c => c.OwningColumn.Name != "Selekcija"))
                    {
                        var val = c.Value?.ToString()?.ToLowerInvariant() ?? "";
                        if (val.Contains(needle)) { match = true; break; }
                    }
                }
                r.Visible = match;
            }
        }

        private void PotvrdiIzbor()
        {
            dgv.EndEdit();
            var picked = dgv.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Visible && Convert.ToBoolean(r.Cells["Selekcija"].Value ?? false))
                .Select(r => new SelectedService
                {
                    Naziv = r.Cells["Naziv"].Value?.ToString() ?? "",
                    Cena = r.Cells["Cena"].Value?.ToString() ?? ""
                })
                .Where(s => !string.IsNullOrWhiteSpace(s.Naziv))
                .ToList();

            if (picked.Count == 0)
            {
                MessageBox.Show("Izaberite bar jednu uslugu.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Izabrane = picked;
            DialogResult = DialogResult.OK;
        }
    }
}
