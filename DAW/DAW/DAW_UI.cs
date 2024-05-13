using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace DAW
{
    public partial class DAW_UI : Form
    {
        PrivateFontCollection privateFonts = new PrivateFontCollection();
        Font sevSegsFont;

        private DateTime recordingStartTime;

        int colorCounter = 0;
        private bool toggleColor = false;

        private List<CustomButton> buttons;
        private Audio newTrack;
        private Point lastLocation; // Declare lastLocation variable here
        private Timer uiUpdateTimer;
        private Timer recordingTimer;
        private bool Recording;

        Color buttonDefault = Color.FromArgb(50, Color.Black);
        Color buttonActionDefault = Color.FromArgb(150, Color.Black);

        public DAW_UI()
        {
            InitializeComponent();
            LoadCustomFont();
            InitializeCustomComponents();
            SetupEventHandlers();
        }

        private void InitializeCustomComponents()
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

            // Create window control buttons
            buttons.Add(new CustomButton("X", 10, new Rectangle(ClientSize.Width - buttonWidth, 0, buttonWidth, buttonHeight), buttonDefault, Color.Red));
            buttons.Add(new CustomButton("_", 10, new Rectangle(ClientSize.Width - 3 * buttonWidth, 0, buttonWidth, buttonHeight), buttonDefault, Color.Gray));
            buttons.Add(new CustomButton("▢", 10, new Rectangle(ClientSize.Width - 2 * buttonWidth, 0, buttonWidth, buttonHeight), buttonDefault, Color.Gray));

            // Define button properties
            int actionButtonWidth = 65;
            int actionButtonHeight = 50;
            int buttonSpacing = 10;

            // Create audio control buttons
            buttons.Add(new CustomButton("■", 20, new Rectangle((ClientSize.Width / 2) - (actionButtonWidth + buttonSpacing), 0, actionButtonWidth, actionButtonHeight), buttonActionDefault, Color.FromArgb(150, Color.Red)));
            buttons.Add(new CustomButton("▶", 20, new Rectangle((ClientSize.Width / 2), 0, actionButtonWidth, actionButtonHeight), buttonActionDefault, Color.FromArgb(150, Color.Green)));
            buttons.Add(new CustomButton("●", 20, new Rectangle((ClientSize.Width / 2) + (actionButtonWidth + buttonSpacing), 0, actionButtonWidth, actionButtonHeight), buttonActionDefault, Color.FromArgb(150, Color.Red)));
        }

        private void SetupEventHandlers()
        {
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

            recordingTimer = new Timer();
            recordingTimer.Interval = 10;
            recordingTimer.Tick += RecordingTimer_Tick;

            Shown += DAW_UI_Shown;
        }

        private void DAW_UI_Shown(object sender, EventArgs e)
        {

            _recordingTimerTBX.Location = new Point((ClientSize.Width / 2) - 75, (buttons[3].Rect.Height + 5));
            _recordingTimerTBX.Font = sevSegsFont;
            _recordingTimerTBX.ReadOnly = true;
            _recordingTimerTBX.Enabled = false;
            _recordingTimerTBX.Size = new System.Drawing.Size(215, 20);
            _recordingTimerTBX.BackColor = Color.Black;
            _recordingTimerTBX.ForeColor = Color.White;

        }

        private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - recordingStartTime;
            _recordingTimerTBX.Text = elapsedTime.ToString("h\\:mm\\:ss\\.ffff");
           

            // Update the color counter every tick (assuming 10ms interval)
    colorCounter += recordingTimer.Interval;

    // Check if 250 milliseconds have passed
    if (colorCounter >= 250)
    {
        // Reset the counter
        colorCounter = 0;
        // Toggle the color
        toggleColor = !toggleColor;

        // Set the background color based on the toggle state
        _recordingTimerTBX.BackColor = toggleColor ? Color.Red : Color.Black;
    }
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {


            Invalidate();

        }

        private void StopButton()
        {
            buttons[5].DefaultColor = buttonActionDefault;
            buttons[5].HoverColor = Color.FromArgb(150, Color.Red);
            _recordingTimerTBX.BackColor = Color.Black;
            newTrack?.StopRecording();
            Recording = false;
            recordingTimer.Stop(); // Stop the recording timer

        }

        private void PlayButton()
        {
            Console.WriteLine("Inside Play button");
        }

        private void RecordButton()
        {
            if (Recording)
            {
                StopButton();

            }
            else
            {
                _recordingTimerTBX.Text = "0:00:00.0000";
                buttons[5].DefaultColor = Color.FromArgb(150, Color.Red);
                buttons[5].HoverColor = Color.Red;
                newTrack = new Audio();
                newTrack.Record();
                Recording = true;
                recordingStartTime = DateTime.Now; // Store the start time
                recordingTimer.Start(); // Start the recording timer
            }

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
            foreach (CustomButton button in buttons)
            {
                button.OnMouseMove(e.Location);
            }
        }

        private void MainForm_MouseLeaveButtons(object sender, EventArgs e)
        {
            foreach (CustomButton button in buttons)
            {
                button.OnMouseLeave();
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (CustomButton button in buttons)
            {
                button.OnMouseClick(e.Location);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (CustomButton button in buttons)
            {
                button.Draw(e.Graphics);
            }
        }

        private void LoadCustomFont()
        {
            privateFonts = new PrivateFontCollection();
            string fontPath = Path.Combine(Application.StartupPath, "Fonts", "SevenSegment.ttf");
            privateFonts.AddFontFile(fontPath);

            FontFamily customFontFamily = privateFonts.Families[0];
            sevSegsFont = new Font(customFontFamily, 30, FontStyle.Regular);

        }
    }
}
