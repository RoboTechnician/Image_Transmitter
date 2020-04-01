using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO.Ports;

namespace ImageTransmitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void InputButtonClick(object sender, EventArgs e)
        {
            if(inputDialog.ShowDialog() == DialogResult.OK)
            {
                if (inputDialog.CheckFileExists)
                {
                    image = (Bitmap)Image.FromFile(inputDialog.FileName);

                    pictureBox1.Size = new Size(image.Width, image.Height);
                    pictureBox1.Image = image;
                    ClientSize = new Size(image.Width, image.Height + 40);

                    pictureBox1.pen = new Pen(Color.White, 2)
                    {
                        DashStyle = DashStyle.Dash
                    };
                    pictureBox1.rectangle = new Rectangle();
                    pictureBox1.Invalidate();
                }
            }
        }

        private void CutButtonClick(object sender, EventArgs e)
        {
            if (pictureBox1.rectangle.Width != 0 && pictureBox1.rectangle.Height != 0)
            {
                image = CutImage(image, pictureBox1.rectangle);

                pictureBox1.Size = new Size(image.Width, image.Height);
                pictureBox1.Image = image;
                ClientSize = new Size(image.Width, image.Height + 40);

                pictureBox1.rectangle = new Rectangle();
                pictureBox1.Invalidate();
            }
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            if (image == null || portsBox.SelectedIndex < 0)
                return;

            Bitmap trImage;
            serialPort1.PortName = (string)portsBox.SelectedItem;
            if (image.Width > imageSizeLimit || image.Height > imageSizeLimit)
            {
                var resizeCoef = (float)imageSizeLimit / Math.Max(image.Width, image.Height);
                trImage = ResizeImage(image, (int)Math.Round(image.Width * resizeCoef), (int)Math.Round(image.Height * resizeCoef));
            }
            else
                trImage = image;

            var frames = CreateFrames(trImage, FrameLength.Large);

            serialPort1.Open();
            for (var i = 0; i < frames.Length; i++)
                serialPort1.Write(frames[i], 0, frames[i].Length);
            serialPort1.Close();
        }

        private void ImageMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseIsDown = true;
                clickPoint = e.Location;
                pictureBox1.rectangle = new Rectangle();
                pictureBox1.Invalidate();
            }
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                var start = new Point
                    (
                    Math.Max(Math.Min(e.Location.X, clickPoint.X), 0),
                    Math.Max(Math.Min(e.Location.Y, clickPoint.Y), 0)
                    );
                var size = new Size
                    (
                    Math.Min(Math.Max(e.Location.X, clickPoint.X), image.Width) - start.X,
                    Math.Min(Math.Max(e.Location.Y, clickPoint.Y), image.Height) - start.Y
                    );
                pictureBox1.rectangle = new Rectangle(start, size);
                pictureBox1.Invalidate();

            }
        }

        private void ImageMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseIsDown = false;
        }

        private Bitmap CutImage(Bitmap image, Rectangle rec)
        {
            var destRec = new Rectangle(0, 0, rec.Width, rec.Height);
            var destImage = new Bitmap(rec.Width, rec.Height);

            using (var gp = Graphics.FromImage(destImage))
            {
                gp.DrawImage(image, destRec, rec, GraphicsUnit.Pixel);
            }

            return destImage;
        }

        private byte[][] CreateFrames(Bitmap image, FrameLength len)
        {
            //| SFD = 42 | Len  | LastFrame | FrameNum |         Data          |    CS    |
            //     6        2         1         15      50, 100, 500, 1000 байт  4 байта

            byte[][] result = new byte[(int) Math.Ceiling(((image.Width * image.Height + 1) * 2 / (float)len.Value) )][];

            var pointer = 0;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new byte[7 + len.Value];
                result[i][0] = (42 << 2);
                result[i][0] |= (byte)len.Code;
                result[i][1] = (byte)((i == result.Length - 1 ? 1 : 0) << 7);
                result[i][1] |= (byte)(i >> 8);
                result[i][2] = (byte)(i & 0xFF);

                var j = 0;
                if (i == 0)
                {
                    result[i][3] = (byte)image.Width;
                    result[i][4] = (byte)image.Height;
                    j = 1;
                }

                for (; j < len.Value / 2; j++, pointer++)
                {
                    if (pointer >= image.Width * image.Height)
                    {
                        result[i][j * 2 + 3] = 0;
                        result[i][j * 2 + 4] = 0;
                    }
                    else
                    {
                        int y = pointer / image.Width;
                        int x = pointer % image.Width;
                        var color = image.GetPixel(x, y);
                        int lpcColor = ((color.R / 8) << 11) | ((color.G / 4) << 5) | (color.B / 8);

                        result[i][j * 2 + 3] = (byte)(lpcColor >> 8);
                        result[i][j * 2 + 4] = (byte)(lpcColor & 0xFF);
                    }
                }

                var crc = CRC32(result[i], 3 + len.Value);
                result[i][len.Value + 3] = (byte)(crc >> 24);
                result[i][len.Value + 4] = (byte)((crc >> 16) & 0xFF);
                result[i][len.Value + 5] = (byte)((crc >> 8) & 0xFF);
                result[i][len.Value + 6] = (byte)(crc & 0xFF);
            }

            return result;
        }

        private uint CRC32(byte[] bytes, int len)
        {
            uint[] crc_table = new uint[256];
            uint crc;

            for (uint i = 0; i < 256; i++)
            {
                crc = i;
                for (uint j = 0; j < 8; j++)
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;

                crc_table[i] = crc;
            };

            crc = 0xFFFFFFFF;

            for (var i = 0; i < len; i++)
                crc = crc_table[(crc ^ bytes[i]) & 0xFF] ^ (crc >> 8);

            return crc ^ 0xFFFFFFFF;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private struct FrameLength
        {
            public int Value { get; set; }
            public int Code { get; set; }
            public static FrameLength Short { get { return new FrameLength { Value = 50, Code = 0}; } }
            public static FrameLength Medium { get { return new FrameLength { Value = 100, Code = 1 }; } }
            public static FrameLength Large { get { return new FrameLength { Value = 500, Code = 2 }; } }
            public static FrameLength VaryLarge { get { return new FrameLength { Value = 1000, Code = 3 }; } }
        }

        //private Byte[] BitmapToLPCByte(Bitmap image)
        //{

        //}
    }
}
