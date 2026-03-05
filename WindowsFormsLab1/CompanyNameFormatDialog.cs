using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsLab1
{
    public partial class CompanyNameFormatDialog : Form
    {
        private TabControl tabControl;
        private TabPage fontTab;
        private TabPage sizeTab;
        private TabPage colorTab;
        private TabPage textTab;

        private RadioButton rbTimesNewRoman;
        private RadioButton rbArial;
        private RadioButton rbCourier;

        private RadioButton rbSize16;
        private RadioButton rbSize20;
        private RadioButton rbSize24;

        private Button btnChooseColor;
        private Panel colorPreviewPanel;
        private Color selectedColor;

        private TextBox txtOldValue;
        private TextBox txtNewValue;

        private Button btnOk;
        private Button btnCancel;

        public string CompanyName { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public Color TextColor { get; set; }

        public CompanyNameFormatDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Company Name Format";
            Size = new Size(450, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(415, 250)
            };

            fontTab = new TabPage("Font");
            sizeTab = new TabPage("Size");
            colorTab = new TabPage("Color");
            textTab = new TabPage("Text");

            tabControl.TabPages.Add(fontTab);
            tabControl.TabPages.Add(sizeTab);
            tabControl.TabPages.Add(colorTab);
            tabControl.TabPages.Add(textTab);

            rbTimesNewRoman = new RadioButton
            {
                Text = "Times New Roman",
                Location = new Point(20, 30),
                AutoSize = true
            };

            rbArial = new RadioButton
            {
                Text = "Arial",
                Location = new Point(20, 60),
                AutoSize = true
            };

            rbCourier = new RadioButton
            {
                Text = "Courier",
                Location = new Point(20, 90),
                AutoSize = true
            };

            fontTab.Controls.AddRange(new Control[] { rbTimesNewRoman, rbArial, rbCourier });

            rbSize16 = new RadioButton
            {
                Text = "16",
                Location = new Point(20, 30),
                AutoSize = true
            };

            rbSize20 = new RadioButton
            {
                Text = "20",
                Location = new Point(20, 60),
                AutoSize = true
            };

            rbSize24 = new RadioButton
            {
                Text = "24",
                Location = new Point(20, 90),
                AutoSize = true
            };

            sizeTab.Controls.AddRange(new Control[] { rbSize16, rbSize20, rbSize24 });

            var lblColor = new Label
            {
                Text = "Choose Color:",
                Location = new Point(20, 30),
                AutoSize = true
            };

            btnChooseColor = new Button
            {
                Text = "Select Color...",
                Location = new Point(20, 60),
                Size = new Size(120, 30)
            };
            btnChooseColor.Click += BtnChooseColor_Click;

            colorPreviewPanel = new Panel
            {
                Location = new Point(150, 60),
                Size = new Size(100, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            colorTab.Controls.AddRange(new Control[] { lblColor, btnChooseColor, colorPreviewPanel });

            var lblOldValue = new Label
            {
                Text = "Old Value:",
                Location = new Point(20, 30),
                AutoSize = true
            };

            txtOldValue = new TextBox
            {
                Location = new Point(20, 55),
                Size = new Size(350, 25),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            var lblNewValue = new Label
            {
                Text = "New Value:",
                Location = new Point(20, 95),
                AutoSize = true
            };

            txtNewValue = new TextBox
            {
                Location = new Point(20, 120),
                Size = new Size(350, 25)
            };

            textTab.Controls.AddRange(new Control[] { lblOldValue, txtOldValue, lblNewValue, txtNewValue });

            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(250, 270),
                Size = new Size(80, 30)
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(340, 270),
                Size = new Size(80, 30)
            };

            Controls.AddRange(new Control[] { tabControl, btnOk, btnCancel });

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        public void LoadCurrentSettings()
        {
            txtOldValue.Text = CompanyName;
            txtNewValue.Text = CompanyName;

            switch (FontName)
            {
                case "Times New Roman":
                    rbTimesNewRoman.Checked = true;
                    break;
                case "Arial":
                    rbArial.Checked = true;
                    break;
                case "Courier New":
                    rbCourier.Checked = true;
                    break;
                default:
                    rbTimesNewRoman.Checked = true;
                    break;
            }

            if (FontSize == 16)
                rbSize16.Checked = true;
            else if (FontSize == 20)
                rbSize20.Checked = true;
            else if (FontSize == 24)
                rbSize24.Checked = true;
            else
                rbSize16.Checked = true;

            selectedColor = TextColor;
            colorPreviewPanel.BackColor = selectedColor;
        }

        private void BtnChooseColor_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = selectedColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    colorPreviewPanel.BackColor = selectedColor;
                }
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (rbTimesNewRoman.Checked)
                FontName = "Times New Roman";
            else if (rbArial.Checked)
                FontName = "Arial";
            else if (rbCourier.Checked)
                FontName = "Courier New";

            if (rbSize16.Checked)
                FontSize = 16;
            else if (rbSize20.Checked)
                FontSize = 20;
            else if (rbSize24.Checked)
                FontSize = 24;

            TextColor = selectedColor;

            if (!string.IsNullOrWhiteSpace(txtNewValue.Text))
                CompanyName = txtNewValue.Text;
        }
    }
}
