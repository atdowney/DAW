using System;
using System.Drawing;
using System.Windows.Forms;

namespace DAW
{
    public class CustomButton
    {
        public Rectangle Rect { get; private set; }
        public string Text { get; set; }
        public Color DefaultColor { get; set; }
        public Color HoverColor { get; set; }
        public bool IsMouseOver { get; private set; }

        public event EventHandler Click;

        public CustomButton(string text, Rectangle rect, Color defaultColor, Color hoverColor)
        {
            Text = text;
            Rect = rect;
            DefaultColor = defaultColor;
            HoverColor = hoverColor;
            IsMouseOver = false;
        }

        public void Draw(Graphics g)
        {
            Color buttonColor = IsMouseOver ? HoverColor : DefaultColor;
            using (Brush brush = new SolidBrush(buttonColor))
            {
                g.FillRectangle(brush, Rect);
            }
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(Text, font);
                PointF location = new PointF(
                    Rect.X + (Rect.Width - textSize.Width) / 2,
                    Rect.Y + (Rect.Height - textSize.Height) / 2);
                g.DrawString(Text, font, Brushes.White, location);
            }
        }

        public void OnMouseMove(Point location)
        {
            bool wasMouseOver = IsMouseOver;
            IsMouseOver = Rect.Contains(location);
            if (IsMouseOver != wasMouseOver)
            {
                Application.OpenForms[0].Invalidate(Rect);
            }
        }

        public void OnMouseClick(Point location)
        {
            if (Rect.Contains(location))
            {
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        public void OnMouseLeave()
        {
            if (IsMouseOver)
            {
                IsMouseOver = false;
                Application.OpenForms[0].Invalidate(Rect);
            }
        }
    }
}
