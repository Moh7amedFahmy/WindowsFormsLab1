using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsLab1
{
    public partial class Form1 : Form
    {
        private readonly int[] years = { 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997 };
        private readonly int[] revenues = { 150, 170, 180, 175, 200, 250, 210, 240, 280, 140 };

        private Panel chartPanel;
        private DataGridView revenueGrid;
        private Color lineColor = Color.Blue;
        
        private int chartLeft = 60;
        private int chartTop = 30;
        private int chartRight;
        private int chartBottom;

        private Label companyNameLabel;
        private MenuStrip menuStrip;

        public Form1()
        {
            InitializeComponent();
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            Text = "ABC Annual Revenue";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 550);
            KeyPreview = true;

            menuStrip = new MenuStrip();
            var formatMenu = new ToolStripMenuItem("Format");
            var companyNameMenuItem = new ToolStripMenuItem("Company Name");
            companyNameMenuItem.Click += CompanyNameMenuItem_Click;
            formatMenu.DropDownItems.Add(companyNameMenuItem);
            menuStrip.Items.Add(formatMenu);

            companyNameLabel = new Label
            {
                Text = "ABC Company",
                Dock = DockStyle.Top,
                Height = 45,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };

            var reportTitleLabel = new Label
            {
                Text = "Annual Revenue",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 560,
                IsSplitterFixed = false
            };

            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            chartPanel.Paint += ChartPanel_Paint;
            chartPanel.MouseClick += ChartPanel_MouseClick;

            revenueGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            splitContainer.Panel1.Controls.Add(chartPanel);
            splitContainer.Panel2.Controls.Add(revenueGrid);

            Controls.Add(splitContainer);
            Controls.Add(reportTitleLabel);
            Controls.Add(companyNameLabel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
        }

        private void CompanyNameMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new CompanyNameFormatDialog())
            {
                dialog.CompanyName = companyNameLabel.Text;
                dialog.FontName = companyNameLabel.Font.Name;
                dialog.FontSize = (int)companyNameLabel.Font.Size;
                dialog.TextColor = companyNameLabel.ForeColor;

                dialog.LoadCurrentSettings();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    companyNameLabel.Text = dialog.CompanyName;
                    companyNameLabel.Font = new Font(dialog.FontName, dialog.FontSize, FontStyle.Bold);
                    companyNameLabel.ForeColor = dialog.TextColor;
                }
            }
        }

        private void LoadData()
        {
            var table = new DataTable();
            table.Columns.Add("Year", typeof(int));
            table.Columns.Add("Revenue", typeof(int));

            for (int i = 0; i < years.Length; i++)
            {
                table.Rows.Add(years[i], revenues[i]);
            }

            revenueGrid.DataSource = table;
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var left = 60;
            var top = 30;
            var right = chartPanel.ClientSize.Width - 30;
            var bottom = chartPanel.ClientSize.Height - 60;

            chartLeft = left;
            chartTop = top;
            chartRight = right;
            chartBottom = bottom;

            if (right <= left || bottom <= top)
            {
                return;
            }

            e.Graphics.DrawLine(Pens.Black, left, bottom, right, bottom);
            e.Graphics.DrawLine(Pens.Black, left, bottom, left, top);

            using (var axisFont = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString("Year", axisFont, Brushes.Black, (left + right) / 2 - 15, bottom + 25);
                e.Graphics.DrawString("Revenue", axisFont, Brushes.Black, 10, top - 5);
            }

            int maxRevenue = 300;
            int minRevenue = 0;
            float chartWidth = right - left;
            float chartHeight = bottom - top;
            float sectionWidth = chartWidth / years.Length;
            var linePoints = new PointF[years.Length];

            using (var barBrush = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Red, Color.White))
            using (var yearFont = new Font("Segoe UI", 8))
            {
                for (int i = 0; i < years.Length; i++)
                {
                    float valueRatio = (float)(revenues[i] - minRevenue) / (maxRevenue - minRevenue);
                    float barHeight = valueRatio * chartHeight;
                    float x = left + i * sectionWidth + sectionWidth * 0.15f;
                    float y = bottom - barHeight;
                    float width = sectionWidth * 0.7f;

                    e.Graphics.FillRectangle(barBrush, x, y, width, barHeight);
                    e.Graphics.DrawRectangle(Pens.DarkRed, x, y, width, barHeight);

                    linePoints[i] = new PointF(x + width / 2f, y);

                    var yearText = years[i].ToString();
                    var textSize = e.Graphics.MeasureString(yearText, yearFont);
                    e.Graphics.DrawString(yearText, yearFont, Brushes.Black, x + (width - textSize.Width) / 2f, bottom + 5);
                }
            }

            using (var linePen = new Pen(lineColor, 2f))
            using (var markerBrush = new SolidBrush(lineColor))
            {
                if (linePoints.Length > 1)
                {
                    e.Graphics.DrawLines(linePen, linePoints);
                }

                foreach (var point in linePoints)
                {
                    e.Graphics.FillEllipse(markerBrush, point.X - 3, point.Y - 3, 6, 6);
                }
            }
        }

        private void ChartPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (chartRight <= chartLeft || chartBottom <= chartTop)
            {
                return;
            }

            float chartWidth = chartRight - chartLeft;
            float chartHeight = chartBottom - chartTop;
            float sectionWidth = chartWidth / years.Length;

            int maxRevenue = 300;
            int minRevenue = 0;

            for (int i = 0; i < years.Length; i++)
            {
                float valueRatio = (float)(revenues[i] - minRevenue) / (maxRevenue - minRevenue);
                float barHeight = valueRatio * chartHeight;
                float x = chartLeft + i * sectionWidth + sectionWidth * 0.15f;
                float y = chartBottom - barHeight;
                float width = sectionWidth * 0.7f;

                if (e.X >= x && e.X <= x + width && e.Y >= y && e.Y <= chartBottom)
                {
                    MessageBox.Show(
                        $"Year: {years[i]}\nRevenue: {revenues[i]}",
                        "Revenue Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.R))
            {
                lineColor = Color.Red;
                chartPanel.Invalidate();
                return true;
            }

            if (keyData == (Keys.Control | Keys.G))
            {
                lineColor = Color.Green;
                chartPanel.Invalidate();
                return true;
            }

            if (keyData == (Keys.Control | Keys.B))
            {
                lineColor = Color.Blue;
                chartPanel.Invalidate();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
