using System.Xml;

namespace SubtitleViewer
{
    public partial class Form1 : Form
    {
        public string subtitle_file = "";
        public string image_file = "";

        bool IsMaximized = false;

        float font_size = 12f;
        int page = 0;
        int pages = 0;
        string[] lines;

        Bitmap? img = null;

        Font font;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
            this.WindowState = FormWindowState.Maximized;
            IsMaximized = true;

            if (!string.IsNullOrEmpty(subtitle_file))
            {
                try
                {
                    lines = File.ReadAllLines(subtitle_file);
                    pages = lines.Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading subtitle file: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(image_file))
            {
                try
                {
                    img = new Bitmap(image_file);
                    pages++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
             font = new Font("Arial", font_size);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //
            if (e.KeyCode == Keys.F11)
            {
                if (IsMaximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    IsMaximized = false;
                    Redraw();
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                    IsMaximized = true;
                    Redraw();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Right)
            {
                font_size += 2;
                font.Dispose();
                font = new Font("Arial", font_size);
                Redraw();
            }
            else if ((e.KeyCode == Keys.Subtract || e.KeyCode == Keys.Left)&& font_size > 8) 
            {
                font_size -= 2;
                font.Dispose();
                font = new Font("Arial", font_size);
                Redraw();
            }
            else if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Down)
            {
                if (page < (pages-1))
                {
                    page++;
                    Redraw();
                }
            }
            else if (e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Up)
            {
                if (page > 0)
                {
                    page--;
                    Redraw();
                }
            }
            else if (e.KeyCode == Keys.A)
            {
                page = 0;
                Redraw();
            }
            else if (e.KeyCode == Keys.Z)
            {
                page = pages-1;
                Redraw();
            }

            else if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("F11: Toggle Fullscreen\nEscape: Close\n+/- or Left/Right: Increase/Decrease Font Size\nUp/Down or PageUp/Page Down: Previous/next line\nA/Z Beginning/The End");
            }
            
        }

        private void Redraw()
        {
            panel1.Invalidate();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (string.IsNullOrEmpty(subtitle_file))
            {
                g.DrawString("Please provide a subtitle file and optional image file", font, Brushes.Orange, new PointF(10, 10));
            }
            else
            {
                if (img!=null && page == 0)
                {
                    g.DrawImage(img, 0, 0, panel1.Width, panel1.Height);
                }
                else
                {
                    int idx = page;
                    if (img != null)
                    {
                        idx--;
                    }
                    string str = lines[idx];

                    int colon_index = str.IndexOf(':');
                    if (colon_index > 0)
                    {
                        string actor = str.Substring(0, colon_index+1);
                        int actor_string_length_in_pixels = (int)g.MeasureString(actor, font).Width;
                        g.DrawString(actor, font, Brushes.Yellow, new PointF(10, 10));

                        str = str.Substring(colon_index + 1);
                        DrawTextString(g, str, actor_string_length_in_pixels + 10, 10);
                    }
                    else
                    {
                        DrawTextString(g, str, 10, 10);
                    }

                }
            }
        }
        private void DrawTextString(Graphics g, string str, int x, int y)
        {
            SizeF size = g.MeasureString(str, font);
            int string_length_in_pixels = (int)size.Width;
            int string_height_in_pixels = (int)size.Height;

            int available_room = panel1.Width - x;

            if (string_length_in_pixels > available_room)
            {
                int last_space = FindLastIndexThatWoudlFit(str,g,available_room);
                if (last_space > 0)
                {
                    string first_line = str.Substring(0, last_space);
                    g.DrawString(first_line, font, Brushes.White, new PointF(x, y));
                    if (last_space < str.Length)
                    {
                        string second_line = " " + str.Substring(last_space + 1);
                        DrawTextString(g, second_line, x, y + string_height_in_pixels);
                    }
                }
                else
                {
                    g.DrawString(str, font, Brushes.White, new PointF(x, y));
                }
            }
            else
            {
                g.DrawString(str, font, Brushes.White, new PointF(x, y));
            }

        }

        private int FindLastIndexThatWoudlFit(string str, Graphics g, int available_room)
        {
            string current_str = str;
            while (true)
            {
                // measure current_str in pizels with font
                SizeF size = g.MeasureString(current_str, font);
                int string_length_in_pixels = (int)size.Width;
                if (string_length_in_pixels < available_room)
                {
                    return current_str.Length;
                }
                else
                {
                    int last_space = current_str.LastIndexOf(' ');
                    if (last_space > 0)
                    {
                        current_str = current_str.Substring(0, last_space);
                    }
                    else
                    {
                        return current_str.Length;
                    }
                }
            }
        }
    }
}
