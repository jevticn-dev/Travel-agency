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
    public partial class FormaIzmenaPaketa : Form
    {
        private TextBox txtNaziv, txtCena;
        private ComboBox cmbTip;
        private Panel dynamicPanel;

        // kontrole za različite tipove
        private TextBox txtDestinacija, txtTipSmestaja, txtTipPrevoza, txtDodatneAkt, txtVodic, txtBrod, txtRuta;
        private DateTimePicker dtpDatumPolaska;

        private Button btnSacuvaj, btnOdustani;

        // Svojstva (čitaju se spolja kad se klikne Sačuvaj)
        public string Naziv { get; private set; }
        public string Tip { get; private set; }
        public string Cena { get; private set; }
        public string Destinacija { get; private set; }
        public string TipSmestaja { get; private set; }
        public string TipPrevoza { get; private set; }
        public string DodatneAktivnosti { get; private set; }
        public string Vodic { get; private set; }
        public string Brod { get; private set; }
        public string Ruta { get; private set; }
        public DateTime? DatumPolaska { get; private set; }

        public FormaIzmenaPaketa(
            string naziv, string tip, string cena,
            string destinacija = null, string tipSmestaja = null, string tipPrevoza = null,
            string dodatneAkt = null, string vodic = null,
            string brod = null, string ruta = null, DateTime? datumPolaska = null)
        {
            Text = "Izmena paketa";
            Size = new Size(520, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            // zajednička polja
            var lblNaziv = MakeLabel("Naziv paketa:", 20, 20);
            txtNaziv = MakeTextBox(160, 18, 320, naziv);

            var lblTip = MakeLabel("Tip paketa:", 20, 60);
            cmbTip = new ComboBox { Left = 160, Top = 58, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTip.Items.AddRange(new[] { "Aranžman za more", "Aranžman za planine", "Ekskurzije", "Krstarenja" });
            cmbTip.SelectedIndexChanged += (s, e) => RenderDynamicFields();

            var lblCena = MakeLabel("Cena:", 20, 100);
            txtCena = MakeTextBox(160, 98, 320, cena);

            dynamicPanel = new Panel { Left = 20, Top = 140, Width = 460, Height = 260 };

            btnSacuvaj = MakeButton("Sačuvaj", 160, 420);
            btnOdustani = MakeButton("Odustani", 300, 420);

            Controls.AddRange(new Control[] { lblNaziv, txtNaziv, lblTip, cmbTip, lblCena, txtCena, dynamicPanel, btnSacuvaj, btnOdustani });

            // preset tip
            int idx = cmbTip.Items.IndexOf(tip);
            cmbTip.SelectedIndex = idx >= 0 ? idx : 0;

            // render dinamičkih polja
            RenderDynamicFields();

            // popuni vrednosti koje su stigle iz grida
            txtNaziv.Text = naziv ?? "";
            txtCena.Text = cena ?? "";

            if (tip == "Aranžman za more")
            {
                txtDestinacija.Text = destinacija ?? "";
                txtTipSmestaja.Text = tipSmestaja ?? "";
                txtTipPrevoza.Text = tipPrevoza ?? "";
            }
            else if (tip == "Aranžman za planine")
            {
                txtDestinacija.Text = destinacija ?? "";
                txtTipSmestaja.Text = tipSmestaja ?? "";
                txtDodatneAkt.Text = dodatneAkt ?? "";
            }
            else if (tip == "Ekskurzije")
            {
                txtDestinacija.Text = destinacija ?? "";
                txtTipPrevoza.Text = tipPrevoza ?? "";
                txtVodic.Text = vodic ?? "";
            }
            else if (tip == "Krstarenja")
            {
                txtBrod.Text = brod ?? "";
                txtRuta.Text = ruta ?? "";
                if (datumPolaska.HasValue) dtpDatumPolaska.Value = datumPolaska.Value;
            }

            // Dugmad
            btnSacuvaj.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNaziv.Text) || cmbTip.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtCena.Text))
                {
                    MessageBox.Show("Naziv, Tip i Cena su obavezni.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Naziv = txtNaziv.Text.Trim();
                Tip = cmbTip.SelectedItem.ToString();
                Cena = txtCena.Text.Trim();

                if (Tip == "Aranžman za more")
                {
                    Destinacija = txtDestinacija.Text.Trim();
                    TipSmestaja = txtTipSmestaja.Text.Trim();
                    TipPrevoza = txtTipPrevoza.Text.Trim();
                }
                else if (Tip == "Aranžman za planine")
                {
                    Destinacija = txtDestinacija.Text.Trim();
                    TipSmestaja = txtTipSmestaja.Text.Trim();
                    DodatneAktivnosti = txtDodatneAkt.Text.Trim();
                }
                else if (Tip == "Ekskurzije")
                {
                    Destinacija = txtDestinacija.Text.Trim();
                    TipPrevoza = txtTipPrevoza.Text.Trim();
                    Vodic = txtVodic.Text.Trim();
                }
                else if (Tip == "Krstarenja")
                {
                    Brod = txtBrod.Text.Trim();
                    Ruta = txtRuta.Text.Trim();
                    DatumPolaska = dtpDatumPolaska.Value.Date;
                }

                DialogResult = DialogResult.OK;
                Close();
            };

            btnOdustani.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        private void RenderDynamicFields()
        {
            dynamicPanel.Controls.Clear();
            int y = 0;
            string tip = cmbTip.SelectedItem?.ToString();

            if (tip == "Aranžman za more")
            {
                dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Destinacija:", 0, y),      txtDestinacija = MakeTextBox(150, y, 280),
                MakeLabel("Tip smeštaja:", 0, y+=40), txtTipSmestaja = MakeTextBox(150, y, 280),
                MakeLabel("Tip prevoza:", 0, y+=40),  txtTipPrevoza = MakeTextBox(150, y, 280),
            });
            }
            else if (tip == "Aranžman za planine")
            {
                dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Destinacija:", 0, y),       txtDestinacija = MakeTextBox(150, y, 280),
                MakeLabel("Tip smeštaja:", 0, y+=40),  txtTipSmestaja = MakeTextBox(150, y, 280),
                MakeLabel("Dodatne aktivnosti:", 0, y+=40), txtDodatneAkt = MakeTextBox(150, y, 280),
            });
            }
            else if (tip == "Ekskurzije")
            {
                dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Destinacija:", 0, y),      txtDestinacija = MakeTextBox(150, y, 280),
                MakeLabel("Tip prevoza:", 0, y+=40),  txtTipPrevoza = MakeTextBox(150, y, 280),
                MakeLabel("Vodič:", 0, y+=40),        txtVodic = MakeTextBox(150, y, 280),
            });
            }
            else if (tip == "Krstarenja")
            {
                dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Brod:", 0, y),             txtBrod = MakeTextBox(150, y, 280),
                MakeLabel("Ruta:", 0, y+=40),         txtRuta = MakeTextBox(150, y, 280),
                MakeLabel("Datum polaska:", 0, y+=40), dtpDatumPolaska = new DateTimePicker { Left=150, Top=y, Width=180, Format=DateTimePickerFormat.Short }
            });
            }
        }

        // helpers
        private Label MakeLabel(string t, int x, int y) => new Label { Text = t, Left = x, Top = y, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        private TextBox MakeTextBox(int x, int y, int w, string v = "") => new TextBox { Left = x, Top = y, Width = w, Height = 25, Text = v ?? "" };
        private Button MakeButton(string t, int x, int y) => new Button { Text = t, Left = x, Top = y, Width = 120, Height = 35, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    }
}