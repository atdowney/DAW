using System;
using System.Drawing;
using System.Windows.Forms;

namespace DAW
{
    public class CustomButton
    {
        // Rectangle defining the button's position and size
        public Rectangle Rect { get; private set; }

        // Text to be displayed on the button
        public string Text { get; set; }

        // Text to be displayed on the button
        public int TextSize { get; set; }

        // Default color of the button
        public Color DefaultColor { get; set; }

        // Color of the button when the mouse hovers over it
        public Color HoverColor { get; set; }

        // Flag indicating whether the mouse is over the button
        public bool IsMouseOver { get; private set; }

        // Event triggered when the button is clicked
        public event EventHandler Click;

        // Constructor to initialize the button with text, rectangle, default color, and hover color
        public CustomButton(string text, int textSize, Rectangle rect, Color defaultColor, Color hoverColor)
        {
            Text = text;
            TextSize = textSize;
            Rect = rect;
            DefaultColor = defaultColor;
            HoverColor = hoverColor;
            IsMouseOver = false;
        }

        // Method to draw the button
        public void Draw(Graphics g)
        {
            // Determine the button color based on mouse hover state
            Color buttonColor = IsMouseOver ? HoverColor : DefaultColor;

            // Draw the button rectangle
            using (Brush brush = new SolidBrush(buttonColor))
            {
                g.FillRectangle(brush, Rect);
            }

            // Draw the button text
            using (Font font = new Font("Arial", TextSize, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(Text, font);
                PointF location = new PointF(
                    Rect.X + (Rect.Width - textSize.Width) / 2,
                    Rect.Y + (Rect.Height - textSize.Height) / 2);
                g.DrawString(Text, font, Brushes.White, location);
            }
        }

        // Method to handle mouse move events
        public void OnMouseMove(Point location)
        {
            // Check if the mouse is over the button
            bool wasMouseOver = IsMouseOver;
            IsMouseOver = Rect.Contains(location);

            // Invalidate the button rectangle if the hover state changed
            if (IsMouseOver != wasMouseOver)
            {
                Application.OpenForms[0].Invalidate(Rect);
            }
        }

        // Method to handle mouse click events
        public void OnMouseClick(Point location)
        {
            // Trigger the click event if the button is clicked
            if (Rect.Contains(location))
            {
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        // Method to handle mouse leave events
        public void OnMouseLeave()
        {
            // Reset the hover state and invalidate the button rectangle if needed
            if (IsMouseOver)
            {
                IsMouseOver = false;
                Application.OpenForms[0].Invalidate(Rect);
            }
        }
    }
}
