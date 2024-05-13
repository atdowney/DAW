using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DAW
{
    public partial class DAW_UI : Form
    {
        private List<CustomButton> buttons;
        private Audio newTrack;
        private Point lastLocation; // Declare lastLocation variable here
        private Timer uiUpdateTimer;

        public DAW_UI()
        {
            // Set form properties
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);

            // Set background image
            BackgroundImage = Properties.Resources.DAWspace;
            BackgroundImageLayout = ImageLayout.Stretch;

            // Initialize custom buttons
            buttons = new List<CustomButton>();

            // Define button properties
            int buttonWidth = 35;
            int buttonHeight = 20;
            int buttonSpacing = 10;

            // Create window control buttons
            buttons.Add(new CustomButton("X", new Rectangle(ClientSize.Width - buttonWidth, 0, buttonWidth, buttonHeight), Color.Black, Color.Red));
            buttons.Add(new CustomButton("_", new Rectangle(ClientSize.Width - 3 * buttonWidth, 0, buttonWidth, buttonHeight), Color.Black, Color.Gray));
            buttons.Add(new CustomButton("▢", new Rectangle(ClientSize.Width - 2 * buttonWidth, 0, buttonWidth, buttonHeight), Color.Black, Color.Gray));

            // Create audio control buttons
            buttons.Add(new CustomButton("■", new Rectangle((ClientSize.Width / 2) - (buttonWidth + buttonSpacing), 0, buttonWidth, buttonHeight), Color.Black, Color.Red));
            buttons.Add(new CustomButton("▶", new Rectangle((ClientSize.Width / 2), 0, buttonWidth, buttonHeight), Color.Black, Color.Green));
            buttons.Add(new CustomButton("●", new Rectangle((ClientSize.Width / 2) + (buttonWidth + buttonSpacing), 0, buttonWidth, buttonHeight), Color.Black, Color.Red));

            // Add button click events
            buttons[0].Click += (s, e) => Close();
            buttons[1].Click += (s, e) => WindowState = FormWindowState.Minimized;
            buttons[2].Click += (s, e) =>
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                }
                Invalidate();
            };

            buttons[3].Click += (s, e) => { Console.WriteLine("Stop button clicked"); StopButton(); };
            buttons[4].Click += (s, e) => { Console.WriteLine("Play button clicked"); PlayButton(); };
            buttons[5].Click += (s, e) => { Console.WriteLine("Record button clicked"); RecordButton(); };

            // Event handlers for mouse actions
            MouseDown += MainForm_MouseDown;
            MouseMove += MainForm_MouseMove;
            MouseMove += MainForm_MouseMoveButtons;
            MouseLeave += MainForm_MouseLeaveButtons;
            MouseClick += MainForm_MouseClick;

            // Initialize and start the timer
            uiUpdateTimer = new Timer();
            uiUpdateTimer.Interval = 100; // 100 milliseconds
            uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            uiUpdateTimer.Start();
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Place code here to update the UI as needed every 100ms
            // For example, you might want to refresh the form:
            Invalidate();
        }

        private void StopButton()
        {
            buttons[5].DefaultColor = Color.Black;
            newTrack?.StopRecording();

            
        }

        private void PlayButton()
        {
            Console.WriteLine("Inside Play button");  
        }

        private void RecordButton()
        {
            buttons[5].DefaultColor = Color.Red;
            newTrack = new Audio();
            newTrack.Record();
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
                    (Location.X - lastLocation.X) + e.X,
                    (Location.Y - lastLocation.Y) + e.Y);
                Update();
            }
        }

        private void MainForm_MouseMoveButtons(object sender, MouseEventArgs e)
        {
            foreach (var button in buttons)
            {
                button.OnMouseMove(e.Location);
            }
        }

        private void MainForm_MouseLeaveButtons(object sender, EventArgs e)
        {
            foreach (var button in buttons)
            {
                button.OnMouseLeave();
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (var button in buttons)
            {
                button.OnMouseClick(e.Location);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var button in buttons)
            {
                button.Draw(e.Graphics);
            }
        }
    }
}
