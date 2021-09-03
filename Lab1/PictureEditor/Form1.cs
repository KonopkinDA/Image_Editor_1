using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PictureEditor
{
    public partial class Form1 : Form
    {
        private List<string> operations = new List<string> { "Нет", "Сумма", "Произведение", "Среднее", "Минимум", "Максимум" };
        private List<string> masks = new List<string> { "Нет", "Круг", "Квадрат", "Прямоугольник" };
        private Bitmap image = null;
        private List<Bitmap> originalPictures = new List<Bitmap>();
        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            panel1.AutoScroll = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                Bitmap imageT = new Bitmap(openFileDialog.FileName);
                image = new Bitmap(imageT, pictureBox1.Width, pictureBox1.Height);
                originalPictures.Add(new Bitmap(image));
                imageT.Dispose();

                GroupBox gb = new GroupBox();
                gb.Height = 240;
                gb.Width = 200;

                if (panel1.Controls.Count == 0)
                {
                    gb.Location = new Point(50, 0);
                }
                else
                {
                    gb.Location = new Point(panel1.Controls[panel1.Controls.Count - 1].Location.X,
                        panel1.Controls[panel1.Controls.Count - 1].Location.Y + 240);
                }

                PictureBox pb = new PictureBox();
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = image;
                pb.Location = new Point(25, 10);
                pb.Height = 100;
                pb.Width = 150;

                ComboBox cb = new ComboBox();
                cb.Location = new Point(10, 145);
                cb.Height = 20;
                cb.Width = 100;
                for (int j = 0; j < operations.Count; j++)
                {
                    cb.Items.Add(operations[j]);
                }
                cb.SelectedItem = "Нет";

                ComboBox cb2 = new ComboBox();
                cb2.Location = new Point(10, 170);
                cb2.Height = 20;
                cb2.Width = 100;
                for (int j = 0; j < masks.Count; j++)
                {
                    cb2.Items.Add(masks[j]);
                }
                cb2.SelectedItem = "Нет";
                cb2.SelectedIndexChanged += Masks;

                CheckBox cbR = new CheckBox();
                cbR.Text = "R";
                cbR.AutoSize = true;
                cbR.Location = new Point(40, 200);
                cbR.Checked = true;
                cbR.CheckedChanged += RGB;
                cbR.CheckedChanged += Masks;

                CheckBox cbG = new CheckBox();
                cbG.Text = "G";
                cbG.AutoSize = true;
                cbG.Location = new Point(90, 200);
                cbG.Checked = true;
                cbG.CheckedChanged += RGB;
                cbG.CheckedChanged += Masks;

                CheckBox cbB = new CheckBox();
                cbB.Text = "B";
                cbB.AutoSize = true;
                cbB.Location = new Point(140, 200);
                cbB.Checked = true;
                cbB.CheckedChanged += RGB;
                cbB.CheckedChanged += Masks;

                TrackBar trB = new TrackBar();
                trB.Location = new Point(10, 110);
                trB.Width = 180;
                trB.Maximum = 255;
                trB.Minimum = 0;
                trB.AutoSize = true;
                trB.ValueChanged += ChangeTrackBar;

                Button b = new Button();
                b.Text = "Удалить";
                b.Location = new Point(120, 145);
                b.Height = 20;
                b.Width = 70;
                b.Click += DeleteImage;

                gb.Controls.Add(b);
                gb.Controls.Add(cb);
                gb.Controls.Add(cb2);
                gb.Controls.Add(pb);
                gb.Controls.Add(cbR);
                gb.Controls.Add(cbG);
                gb.Controls.Add(cbB);
                gb.Controls.Add(trB);
                panel1.Controls.Add(gb);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDileFialog = new SaveFileDialog();
            saveDileFialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveDileFialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            saveDileFialog.RestoreDirectory = true;
            image = (Bitmap)pictureBox1.Image;

            if (saveDileFialog.ShowDialog() == DialogResult.OK)
            {
                if (image != null)
                {
                    image.Save(saveDileFialog.FileName);
                }
            }
        }

        private void ChangeTrackBar(object sender, EventArgs e)
        {
            TrackBar gr = (TrackBar)sender;
            int ind = panel1.Controls.GetChildIndex(gr.Parent);
            PictureBox pb = (PictureBox)panel1.Controls[ind].Controls[3];
            image = (Bitmap)pb.Image;
            for (int i = 0; i < pb.Image.Width; i++)
            {
                for (int j = 0; j < pb.Image.Height; j++)
                {
                    Color c = Color.FromArgb(255 - gr.Value, image.GetPixel(i, j).R, image.GetPixel(i, j).G, image.GetPixel(i, j).B);
                    image.SetPixel(i, j, c);
                }
            }
            pb.Image = image;
        }

        private void Functions()
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = image2;
            }
            Bitmap resultImage = new Bitmap((Bitmap)pictureBox1.Image);
            for (int i = 0; i < resultImage.Width; i++)
            {
                for (int j = 0; j < resultImage.Height; j++)
                {
                    for (int k = 0; k < panel1.Controls.Count; k++)
                    {
                        PictureBox p = (PictureBox)panel1.Controls[k].Controls[3];
                        Bitmap b = (Bitmap)p.Image;
                        int r1 = 0, g1 = 0, b1 = 0;
                        ComboBox cmb = (ComboBox)panel1.Controls[k].Controls[1];
                        if (k != 0)
                        {
                            switch (cmb.SelectedItem.ToString())
                            {
                                case "Произведение":
                                    r1 = (int)((resultImage.GetPixel(i, j).R * (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0))) / 255.0);
                                    g1 = (int)((resultImage.GetPixel(i, j).G * (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0))) / 255.0);
                                    b1 = (int)((resultImage.GetPixel(i, j).B * (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0))) / 255.0);
                                    break;
                                case "Сумма":
                                    r1 = resultImage.GetPixel(i, j).R + (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0));
                                    g1 = resultImage.GetPixel(i, j).G + (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0));
                                    b1 = resultImage.GetPixel(i, j).B + (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0));
                                    break;
                                case "Среднее":
                                    r1 = (int)((resultImage.GetPixel(i, j).R + (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0)))/2.0);
                                    g1 = (int)((resultImage.GetPixel(i, j).G + (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0)))/2.0);
                                    b1 = (int)((resultImage.GetPixel(i, j).B + (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0)))/2.0);
                                    break;
                                case "Минимум":
                                    if (resultImage.GetPixel(i, j).R < (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        r1 = resultImage.GetPixel(i, j).R;
                                    }
                                    else
                                    {
                                        r1 = (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    if (resultImage.GetPixel(i, j).G < (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        g1 = resultImage.GetPixel(i, j).G;
                                    }
                                    else
                                    {
                                        g1 = (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    if (resultImage.GetPixel(i, j).B < (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        b1 = resultImage.GetPixel(i, j).B;
                                    }
                                    else
                                    {
                                        b1 = (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    break;
                                case "Максимум":
                                    if (resultImage.GetPixel(i, j).R > (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        r1 = resultImage.GetPixel(i, j).R;
                                    }
                                    else
                                    {
                                        r1 = (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    if (resultImage.GetPixel(i, j).G > (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        g1 = resultImage.GetPixel(i, j).G;
                                    }
                                    else
                                    {
                                        g1 = (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    if (resultImage.GetPixel(i, j).B > (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0)))
                                    {
                                        b1 = resultImage.GetPixel(i, j).B;
                                    }
                                    else
                                    {
                                        b1 = (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0));
                                    }
                                    break;
                                case "Нет":
                                    r1 = resultImage.GetPixel(i, j).R;
                                    g1 = resultImage.GetPixel(i, j).G;
                                    b1 = resultImage.GetPixel(i, j).B;
                                    break;
                            }
                        }
                        else
                        {
                            r1 = (int)(b.GetPixel(i, j).R * (b.GetPixel(i, j).A / 255.0));
                            g1 = (int)(b.GetPixel(i, j).G * (b.GetPixel(i, j).A / 255.0));
                            b1 = (int)(b.GetPixel(i, j).B * (b.GetPixel(i, j).A / 255.0));
                        }
                        if (r1 > 255)
                        {
                            r1 = 255;
                        }
                        if (g1 > 255)
                        {
                            g1 = 255;
                        }
                        if (b1 > 255)
                        {
                            b1 = 255;
                        }
                        Color c = Color.FromArgb(r1, g1, b1);
                        resultImage.SetPixel(i, j, c);
                    }
                }
            }
            pictureBox1.Image = resultImage;
        }

        private void Masks(object sender, EventArgs e)
        {
            ComboBox gr1 = sender as ComboBox;
            CheckBox gr2 = null;
            ComboBox gr = null;
            if (gr1 == null)
            {
                gr2 = (CheckBox)sender;
                gr = (ComboBox)gr2.Parent.Controls[2];
            }
            else
            {
                gr = gr1;
            }
            int ind = panel1.Controls.GetChildIndex(gr.Parent);
            PictureBox pb = (PictureBox)panel1.Controls[ind].Controls[3];
            image = (Bitmap)pb.Image;
            //image = originalPictures[ind];
            switch (gr.SelectedItem.ToString())
            {
                case "Круг":
                    for(int i = 0; i < image.Width; i++)
                    {
                        for(int j = 0; j < image.Height; j++)
                        {
                            if(Math.Sqrt((i-image.Width/2)* (i - image.Width / 2) + (j - image.Height / 2)* (j - image.Height / 2)) > 200)
                            {
                                Color c = Color.FromArgb(0, 0, 0);
                                image.SetPixel(i, j, c);
                            }
                        }
                    }
                    pb.Image = image;
                    break;

                case "Квадрат":
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            if (i < 180 || i > (image.Width - 180) || j < 100 || j > (image.Height - 100))
                            {
                                Color c = Color.FromArgb(0, 0, 0);
                                image.SetPixel(i, j, c);
                            }
                        }
                    }
                    pb.Image = image;
                    break;

                case "Прямоугольник":
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            if (i < 100 || i > (image.Width - 100) || j < 100 || j > (image.Height - 100))
                            {
                                Color c = Color.FromArgb(0, 0, 0);
                                image.SetPixel(i, j, c);
                            }
                        }
                    }
                    pb.Image = image;
                    break;
            }
        }

        private void RGB(object sender, EventArgs e)
        {
            CheckBox gr = (CheckBox)sender;
            int ind = panel1.Controls.GetChildIndex(gr.Parent);
            PictureBox pb = (PictureBox)panel1.Controls[ind].Controls[3];
            image = (Bitmap)pb.Image;
            for (int i = 0; i < pb.Image.Width; i++)
            {
                for (int j = 0; j < pb.Image.Height; j++)
                {
                    if(!gr.Checked)
                    {
                        if (gr.Text == "R")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, 0, image.GetPixel(i, j).G, image.GetPixel(i, j).B);
                            image.SetPixel(i, j, c);
                        }
                        if (gr.Text == "G")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, image.GetPixel(i, j).R, 0, image.GetPixel(i, j).B);
                            image.SetPixel(i, j, c);
                        }
                        if (gr.Text == "B")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, image.GetPixel(i, j).R, image.GetPixel(i, j).G, 0);
                            image.SetPixel(i, j, c);
                        }
                    }
                    else
                    {
                        Bitmap temp = originalPictures[ind];
                        if (gr.Text == "R")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, temp.GetPixel(i, j).R, image.GetPixel(i, j).G, image.GetPixel(i, j).B);
                            image.SetPixel(i, j, c);
                        }
                        if (gr.Text == "G")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, image.GetPixel(i, j).R, temp.GetPixel(i, j).G, image.GetPixel(i, j).B);
                            image.SetPixel(i, j, c);
                        }
                        if (gr.Text == "B")
                        {
                            Color c = Color.FromArgb(image.GetPixel(i, j).A, image.GetPixel(i, j).R, image.GetPixel(i, j).G, temp.GetPixel(i, j).B);
                            image.SetPixel(i, j, c);
                        }
                    }
                }
            }
            pb.Image = image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Functions();
        }

        private void DeleteImage(object sender, EventArgs e)
        {
            Button gr = (Button)sender;
            int ind2 = panel1.Controls.GetChildIndex(gr.Parent);
            for (int ind = ind2; ind < panel1.Controls.Count; ind++)
            {
                panel1.Controls[ind].Location = new Point(panel1.Controls[ind].Location.X, panel1.Controls[ind].Location.Y - 240);
            }
            originalPictures.RemoveAt(ind2);
            gr.Parent.Dispose();
        }

    }
}
