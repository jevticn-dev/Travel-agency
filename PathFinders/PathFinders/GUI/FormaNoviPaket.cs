using System;
using System.Drawing;
using System.Windows.Forms;

public class FormaNoviPaket : Form
{
    
    private TextBox txtNaziv, txtCena;
    private ComboBox cmbTip;

    // panel koji menja sadržaj (dinamička polja)
    private Panel dynamicPanel;

    // kontrole za različite tipove
    private TextBox txtDestinacija, txtTipSmestaja, txtTipPrevoza, txtDodatneAkt, txtVodic, txtBrod, txtRuta;
    private DateTimePicker dtpDatumPolaska;

    private Button btnSacuvaj, btnOdustani;

    public string Naziv { get; private set; }
    public string Tip { get; private set; }
    public string Cena { get; private set; }

    // opcioni povratni detalji (popunjavani u zavisnosti od tipa)
    public string Destinacija { get; private set; }
    public string TipSmestaja { get; private set; }
    public string TipPrevoza { get; private set; }
    public string DodatneAktivnosti { get; private set; }
    public string Vodic { get; private set; }
    public string Brod { get; private set; }
    public string Ruta { get; private set; }
    public DateTime? DatumPolaska { get; private set; }

    public FormaNoviPaket()
    {
        Text = "Novi paket";
        Size = new Size(520, 520);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false;

        var lblNaziv = MakeLabel("Naziv paketa:", 20, 20);
        txtNaziv = MakeTextBox(160, 18, 320);

        var lblTip = MakeLabel("Tip paketa:", 20, 60);
        cmbTip = new ComboBox { Left = 160, Top = 58, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTip.Items.AddRange(new[] {
            "Aranžman za more",
            "Aranžman za planine",
            "Ekskurzije",
            "Krstarenja"
        });
        cmbTip.SelectedIndexChanged += (s, e) => RenderDynamicFields();

        var lblCena = MakeLabel("Cena:", 20, 100);
        txtCena = MakeTextBox(160, 98, 320);

        dynamicPanel = new Panel { Left = 20, Top = 140, Width = 460, Height = 260, BorderStyle = BorderStyle.None };

        btnSacuvaj = MakeButton("Sačuvaj", 160, 420);
        btnOdustani = MakeButton("Odustani", 300, 420);

        Controls.AddRange(new Control[] {
            lblNaziv, txtNaziv, lblTip, cmbTip, lblCena, txtCena, dynamicPanel, btnSacuvaj, btnOdustani
        });

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
                if (AnyEmpty(txtDestinacija, txtTipSmestaja, txtTipPrevoza))
                {
                    Error("Destinacija, Tip smeštaja i Tip prevoza su obavezni.");
                    return;
                }
                Destinacija = txtDestinacija.Text.Trim();
                TipSmestaja = txtTipSmestaja.Text.Trim();
                TipPrevoza = txtTipPrevoza.Text.Trim();
            }
            else if (Tip == "Aranžman za planine")
            {
                if (AnyEmpty(txtDestinacija, txtTipSmestaja, txtDodatneAkt))
                {
                    Error("Destinacija, Tip smeštaja i Dodatne aktivnosti su obavezni.");
                    return;
                }
                Destinacija = txtDestinacija.Text.Trim();
                TipSmestaja = txtTipSmestaja.Text.Trim();
                DodatneAktivnosti = txtDodatneAkt.Text.Trim();
            }
            else if (Tip == "Ekskurzije")
            {
                if (AnyEmpty(txtDestinacija, txtTipPrevoza, txtVodic))
                {
                    Error("Destinacija, Tip prevoza i Vodič su obavezni.");
                    return;
                }
                Destinacija = txtDestinacija.Text.Trim();
                TipPrevoza = txtTipPrevoza.Text.Trim();
                Vodic = txtVodic.Text.Trim();
            }
            else if (Tip == "Krstarenja")
            {
                if (AnyEmpty(txtBrod, txtRuta) || dtpDatumPolaska == null)
                {
                    Error("Brod, Ruta i Datum polaska su obavezni.");
                    return;
                }
                Brod = txtBrod.Text.Trim();
                Ruta = txtRuta.Text.Trim();
                DatumPolaska = dtpDatumPolaska.Value.Date;
            }

            DialogResult = DialogResult.OK;
            Close();
        };

        btnOdustani.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        // podrazumevani tip (po želji)
        cmbTip.SelectedIndex = 0;
    }

    private void RenderDynamicFields()
    {
        dynamicPanel.Controls.Clear();
        int y = 0;

        string tip = cmbTip.SelectedItem?.ToString();

        if (tip == "Aranžman za more")
        {
            dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Destinacija:", 0, y),       txtDestinacija = MakeTextBox(140, y, 320),
                MakeLabel("Tip smeštaja:", 0, y+=40),  txtTipSmestaja = MakeTextBox(140, y, 320),
                MakeLabel("Tip prevoza:", 0, y+=40),   txtTipPrevoza = MakeTextBox(140, y, 320),
            });
        }
        else if (tip == "Aranžman za planine")
        {
            dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Destinacija:", 0, y),        txtDestinacija = MakeTextBox(140, y, 320),
                MakeLabel("Tip smeštaja:", 0, y+=40),   txtTipSmestaja = MakeTextBox(140, y, 320),
                MakeLabel("Dodatne aktivnosti:", 0, y+=40), txtDodatneAkt = MakeTextBox(140, y, 320),
            });
        }
        else if (tip == "Ekskurzije")
        {
            dynamicPanel.Controls.AddRange(new Control[] {
               MakeLabel("Destinacija:", 0, y),        txtDestinacija = MakeTextBox(140, y, 320),
                MakeLabel("Tip prevoza:", 0, y+=40),    txtTipPrevoza = MakeTextBox(140, y, 320),
                MakeLabel("Vodič:", 0, y+=40),          txtVodic = MakeTextBox(140, y, 320),
            });
        }
        else if (tip == "Krstarenja")
        {
            dynamicPanel.Controls.AddRange(new Control[] {
                MakeLabel("Brod:", 0, y),               txtBrod = MakeTextBox(140, y, 320),
                MakeLabel("Ruta:", 0, y+=40),           txtRuta = MakeTextBox(140, y, 320),
                MakeLabel("Datum polaska:", 0, y+=40),  dtpDatumPolaska = new DateTimePicker { Left=140, Top=y, Width=320, Format=DateTimePickerFormat.Short }
            });
        }
    }

    // helpers
    private Label MakeLabel(string t, int x, int y) => new Label { Text = t, Left = x, Top = y, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
    private TextBox MakeTextBox(int x, int y, int w) => new TextBox { Left = x, Top = y, Width = w, Height = 25 };
    private Button MakeButton(string t, int x, int y) => new Button { Text = t, Left = x, Top = y, Width = 120, Height = 35, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    private static bool AnyEmpty(params TextBox[] boxes)
    {
        foreach (var b in boxes) if (b == null || string.IsNullOrWhiteSpace(b.Text)) return true;
        return false;
    }
    private static void Error(string msg) => MessageBox.Show(msg, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
