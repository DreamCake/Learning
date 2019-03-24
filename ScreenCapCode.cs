using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System;

namespace ScreenCap
{
    public partial class Form1 : Form
    {
        PictureBox pictureBox = new PictureBox();
        //These variables control the mouse position
        int selectX;
        int selectY;
        int selectWidth;
        int selectHeight;
        public Pen selectPen;
        
        //This variable control when you start the right click
        bool start = false;

        public Form1()
        {
            pictureBox.Location = new Point(0,0);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            Opacity = 0.1;
            InitializeComponent();
        }
        void Form1_Shown(object sender, System.EventArgs e)
        {
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            //Hide the Form
            this.Hide();
            //Create the Bitmap
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //Create the Graphic Variable with screen Dimensions
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            //Copy Image from the screen
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            //Create a temporal memory stream for the image
            using (MemoryStream s = new MemoryStream())
            {
                //save graphic variable into memory
                printscreen.Save(s, ImageFormat.Jpeg);
                pictureBox.Size = new Size(Width, Height);
                //set the picture box with temporary stream
                pictureBox.Image = Image.FromStream(s);
            }
            //Show Form
            this.Show();
            //Cross Cursor
            Cursor = Cursors.Cross;
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //validate if there is an image
            if (pictureBox.Image == null)
                return;
            //validate if right-click was trigger
            if (start)
            {
                //refresh picture box
                pictureBox.Refresh();
                //set corner square to mouse coordinates
                selectWidth = e.X - selectX;
                selectHeight = e.Y - selectY;
                //draw dotted rectangle
                pictureBox.CreateGraphics().DrawRectangle(
                    selectPen, selectX, selectY, selectWidth, selectHeight);
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //validate when user right-click
            if (!start)
            {
                if (e.Button == MouseButtons.Left)
                {
                    //starts coordinates for rectangle
                    selectX = e.X;
                    selectY = e.Y;
                    selectPen = new Pen(Color.Red, 1);
                    selectPen.DashStyle = DashStyle.DashDotDot;
                }
                //refresh picture box
                pictureBox.Refresh();
                //start control variable for draw rectangle
                start = true;
            }
            else
            {
                //validate if there is image
                if (pictureBox.Image == null)
                    return;
                //same functionality when mouse is over
                if (e.Button == MouseButtons.Left)
                {
                    pictureBox.Refresh();
                    selectWidth = e.X - selectX;
                    selectHeight = e.Y - selectY;
                    pictureBox.CreateGraphics().DrawRectangle(
                        selectPen, selectX, selectY, selectWidth, selectHeight);

                }
                start = false;
                //function save image to clipboard
                SaveToClipboard();
            }
        }
        private void SaveToClipboard()
        {
            //validate if something selected
            if (selectWidth > 0)
            {

                Rectangle rect = new Rectangle(selectX, selectY, selectWidth, selectHeight);
                //create bitmap with original dimensions
                Bitmap OriginalImage = new Bitmap(pictureBox.Image, pictureBox.Width, pictureBox.Height);
                //create bitmap with selected dimensions
                Bitmap _img = new Bitmap(selectWidth, selectHeight);
                //create graphic variable
                Graphics g = Graphics.FromImage(_img);
                //set graphic attributes
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);
                //insert image stream into clipboard
                Clipboard.SetImage(_img);
            }
            //End application
            Application.Exit();
        }
    }
}
