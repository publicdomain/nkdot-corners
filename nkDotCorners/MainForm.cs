﻿// <copyright file="MainForm.cs" company="PublicDomain.is">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace nkDotCorners
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using PublicDomain;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The settings data.
        /// </summary>
        private SettingsData settingsData = null;

        /// <summary>
        /// The settings data path.
        /// </summary>
        private string settingsDataPath = $"nkDot-SettingsData.txt";

        /// <summary>
        /// The dot brush.
        /// </summary>
        private Brush dotBrush = null;

        /// <summary>
        /// The dot rectangle.
        /// </summary>
        private Rectangle dotRectangle;

        /// <summary>
        /// The corner count.
        /// </summary>
        private int cornerCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:nkDot.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            /* Settings data */

            // Check for settings file
            if (!File.Exists(this.settingsDataPath))
            {
                // Create new settings file
                this.SaveSettingsFile(this.settingsDataPath, new SettingsData());
            }

            // Load settings from disk
            this.settingsData = this.LoadSettingsFile(this.settingsDataPath);

            // Set background
            this.dotPictureBox.BackColor = this.settingsData.BackgroundColor;

            // Set dot brush
            this.dotBrush = new SolidBrush(this.settingsData.DotColor);

            // Set initial dot rectangle
            this.SetDotRectangle();

            // Set size
            this.ClientSize = this.settingsData.Size;

            // Set location
            this.Location = this.settingsData.Location;

            // Set dot timer interval
            this.dotTimer.Interval = this.settingsData.RedrawInterval;

            // Set run time
            this.runTimer.Interval = this.settingsData.RunningTime;

            // Min size
            this.MinimumSize = new Size(this.settingsData.DotSize + 3, this.settingsData.DotSize + 3);

            // Maximized
            this.WindowState = this.settingsData.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        /// <summary>
        /// Sets the dot rectangle.
        /// </summary>
        private void SetDotRectangle()
        {
            // Raise or reset corner count
            this.cornerCount++;

            try
            {
                // Act upon current corner count
                switch (this.cornerCount)
                {
                    // North-West
                    case 1:
                        // New dot rectangle using settings data
                        this.dotRectangle = new Rectangle(0, 0, this.settingsData.DotSize, this.settingsData.DotSize);

                        break;

                    // South-East
                    case 2:
                        // New dot rectangle using settings data
                        this.dotRectangle = new Rectangle(this.dotPictureBox.Width - this.settingsData.DotSize, this.dotPictureBox.Height - this.settingsData.DotSize, this.settingsData.DotSize, this.settingsData.DotSize);

                        break;

                    // North-East
                    case 3:
                        // New dot rectangle using settings data
                        this.dotRectangle = new Rectangle(this.dotPictureBox.Width - this.settingsData.DotSize, 0, this.settingsData.DotSize, this.settingsData.DotSize);

                        break;

                    // South - West
                    case 4:
                        // New dot rectangle using settings data
                        this.dotRectangle = new Rectangle(0, this.dotPictureBox.Height - this.settingsData.DotSize, this.settingsData.DotSize, this.settingsData.DotSize);

                        // Reset corner count
                        this.cornerCount = 0;

                        break;
                }
            }
            catch (Exception ex)
            {
                // Let it fall through
                ;
            }
        }

        /// <summary>
        /// Handles the dot picture box paint.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnDotPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Set smoothing mode
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the dot
            e.Graphics.FillEllipse(this.dotBrush, this.dotRectangle);
        }

        /// <summary>
        /// Handles the main form load.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            // Set topmost
            this.TopMost = this.settingsData.AlwaysOnTop;

            // Activate timers
            this.dotTimer.Enabled = true;
            this.runTimer.Enabled = true;
        }

        /// <summary>
        /// Handles the dot timer tick.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDotTimerTick(object sender, EventArgs e)
        {
            // Set dot rectangle
            this.SetDotRectangle();

            // INvalidate
            this.dotPictureBox.Invalidate();
        }

        /// <summary>
        /// Handles the run timer tick.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnRunTimerTick(object sender, EventArgs e)
        {
            // Disable
            this.runTimer.Enabled = false;

            // Exit program
            this.Close();
        }

        /// <summary>
        /// Loads the settings file.
        /// </summary>
        /// <returns>The settings file.</returns>
        /// <param name="settingsFilePath">Settings file path.</param>
        private SettingsData LoadSettingsFile(string settingsFilePath)
        {
            // Use file stream
            using (FileStream fileStream = File.OpenRead(settingsFilePath))
            {
                // Set xml serialzer
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                // Return populated settings data
                return xmlSerializer.Deserialize(fileStream) as SettingsData;
            }
        }

        /// <summary>
        /// Saves the settings file.
        /// </summary>
        /// <param name="settingsFilePath">Settings file path.</param>
        /// <param name="settingsDataParam">Settings data parameter.</param>
        private void SaveSettingsFile(string settingsFilePath, SettingsData settingsDataParam)
        {
            try
            {
                // Use stream writer
                using (StreamWriter streamWriter = new StreamWriter(settingsFilePath, false))
                {
                    // Set xml serialzer
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                    // Serialize settings data
                    xmlSerializer.Serialize(streamWriter, settingsDataParam);
                }
            }
            catch (Exception exception)
            {
                // Advise user
                MessageBox.Show($"Error saving settings file.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{exception.Message}", "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
