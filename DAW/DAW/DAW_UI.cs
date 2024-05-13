using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAW
{
    public partial class DAW_UI : Form
    {

        private Rectangle closeButtonRect;
        private Rectangle minimizeButtonRect;
        private Rectangle maximizeButtonRect;
        private bool isMaximized = false;
        private Point lastLocation;
        private bool isMouseOverClose = false;
        private bool isMouseOverMinimize = false;
        private bool isMouseOverMaximize = false;

        public DAW_UI()
        {
           FormBorderStyle = FormBorderStyle.None;
           DoubleBuffered = true;
           StartPosition = FormStartPosition.CenterScreen;
           MinimumSize = new Size(1000, 600);

           BackgroundImage = Properties.Resources.DAWspace;
           BackgroundImageLayout = ImageLayout.Stretch;

           MouseDown += MainForm_MouseDown;
           MouseMove += MainForm_MouseMove;
           MouseMove += MainForm_MouseMoveButtons;
           MouseLeave += MainForm_MouseLeaveButtons;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastLocation = e.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
               Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

               Update();
            }
        }

        private void MainForm_MouseMoveButtons(object sender, MouseEventArgs e)
        {
            bool needsRedraw = false;

            if (closeButtonRect.Contains(e.Location))
            {
                if (!isMouseOverClose)
                {
                    isMouseOverClose = true;
                    needsRedraw = true;
                }
            }
            else
            {
                if (isMouseOverClose)
                {
                    isMouseOverClose = false;
                    needsRedraw = true;
                }
            }

            if (minimizeButtonRect.Contains(e.Location))
            {
                if (!isMouseOverMinimize)
                {
                    isMouseOverMinimize = true;
                    needsRedraw = true;
                }
            }
            else
            {
                if (isMouseOverMinimize)
                {
                    isMouseOverMinimize = false;
                    needsRedraw = true;
                }
            }

            if (maximizeButtonRect.Contains(e.Location))
            {
                if (!isMouseOverMaximize)
                {
                    isMouseOverMaximize = true;
                    needsRedraw = true;
                }
            }
            else
            {
                if (isMouseOverMaximize)
                {
                    isMouseOverMaximize = false;
                    needsRedraw = true;
                }
            }

            if (needsRedraw)
            {
                Invalidate();
            }
        }

        private void MainForm_MouseLeaveButtons(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            if (isMouseOverClose || isMouseOverMinimize || isMouseOverMaximize)
            {
                isMouseOverClose = false;
                isMouseOverMinimize = false;
                isMouseOverMaximize = false;
                needsRedraw = true;
            }

            if (needsRedraw)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int buttonWidth = 35;
            int buttonHeight = 20;

            minimizeButtonRect = new Rectangle(this.ClientSize.Width - 3 * buttonWidth, 0, buttonWidth, buttonHeight);
            maximizeButtonRect = new Rectangle(this.ClientSize.Width - 2 * buttonWidth, 0, buttonWidth, buttonHeight);
            closeButtonRect = new Rectangle(this.ClientSize.Width - buttonWidth, 0, buttonWidth, buttonHeight);

            // Draw title bar background
            //e.Graphics.FillRectangle(Brushes.DarkBlue, 0, 0,ClientSize.Width, buttonHeight);

            // Draw minimize button
            DrawButton(e.Graphics, minimizeButtonRect, "_", isMouseOverMinimize, Color.Black);
            // Draw maximize/restore button
            DrawButton(e.Graphics, maximizeButtonRect, isMaximized ? "❐" : "▢", isMouseOverMaximize, Color.Black);
            // Draw close button
            DrawButton(e.Graphics, closeButtonRect, "X", isMouseOverClose, Color.Red);
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, bool isMouseOver, Color userColor)
        {
            Color buttonColor = isMouseOver ? Color.FromArgb(225, userColor) : Color.FromArgb(0, Color.Black);
            using (Brush brush = new SolidBrush(buttonColor))
            {
                g.FillRectangle(brush, rect);
            }
            //g.DrawRectangle(Pens.Black, rect);
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(text, font);
                PointF location = new PointF(
                    rect.X + (rect.Width - textSize.Width) / 2,
                    rect.Y + (rect.Height - textSize.Height) / 2);
                g.DrawString(text, font, Brushes.White, location);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (closeButtonRect.Contains(e.Location))
            {
               Close();
            }
            else if (minimizeButtonRect.Contains(e.Location))
            {
               WindowState = FormWindowState.Minimized;
            }
            else if (maximizeButtonRect.Contains(e.Location))
            {
                if (isMaximized)
                {
                   WindowState = FormWindowState.Normal;
                    isMaximized = false;
                }
                else
                {
                   WindowState = FormWindowState.Maximized;
                    isMaximized = true;
                }
                Invalidate();
            }
        }
    }
}
