using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleSoccer.Net
{
    public partial class SimpleSoccer : Form
    {
        private bool _simulationStarted = false;
        private bool _simulationPaused = false;

        private SoccerPitch _soccerPitch = null;
        private AutoResetEvent _stopEvent = new AutoResetEvent(false);
        Thread _gameThread;

        public SimpleSoccer()
        {
            InitializeComponent();

            this.ClientSize = new Size(640, 370);
        }

        private void gameLoop()
        {
            _soccerPitch = new SoccerPitch(pictureBox1.ClientRectangle.Width, pictureBox1.ClientRectangle.Height);

            _simulationStarted = true;

            PrecisionTimer timer = new PrecisionTimer(ParameterManager.Instance.FrameRate);
            while (!_stopEvent.WaitOne(0, true))
            {
                if (timer.ReadyForNextFrame() && !_simulationPaused)
                {
                    _soccerPitch.Update();
                }

                pictureBox1.Invalidate();
                Thread.Sleep(1);
            }

            _soccerPitch = null;
            _simulationStarted = false;

            // a final call to repaint our current state
            pictureBox1.Invalidate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopSimulation();
        }

        private void stopSimulation()
        {
            _stopEvent.Set();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.DarkGreen);

            if (_soccerPitch != null)
            {
                _soccerPitch.Render(e.Graphics);
            }
        }

        private void noAidsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowHighlightIfThreatened = false;
            ParameterManager.Instance.ShowIDs = false;
            ParameterManager.Instance.ShowRegions = false;
            ParameterManager.Instance.ShowControllingTeam = false;
            ParameterManager.Instance.ShowStates = false;
            ParameterManager.Instance.ShowSupportSpots = false;
            ParameterManager.Instance.ShowViewTargets = false;
        }

        private void debugAidsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            showIDsToolStripMenuItem.Checked = ParameterManager.Instance.ShowIDs;
            highlightIfThreatenedToolStripMenuItem.Checked = ParameterManager.Instance.ShowHighlightIfThreatened;
            showTargetsToolStripMenuItem.Checked = ParameterManager.Instance.ShowViewTargets;
            showSupportSpotsToolStripMenuItem.Checked = ParameterManager.Instance.ShowSupportSpots;
            showRegionsToolStripMenuItem.Checked = ParameterManager.Instance.ShowRegions;
            showStatesToolStripMenuItem.Checked = ParameterManager.Instance.ShowStates;
            showControllingTeamToolStripMenuItem.Checked = ParameterManager.Instance.ShowControllingTeam;
        }

        private void showIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowIDs = !showIDsToolStripMenuItem.Checked;
        }

        private void showStatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowStates = !showStatesToolStripMenuItem.Checked;
        }

        private void showRegionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowRegions = !showRegionsToolStripMenuItem.Checked;
        }

        private void showSupportSpotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowSupportSpots = !showSupportSpotsToolStripMenuItem.Checked;
        }

        private void showTargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowViewTargets = !showTargetsToolStripMenuItem.Checked;
        }

        private void showControllingTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowControllingTeam = !showControllingTeamToolStripMenuItem.Checked;
        }

        private void highlightIfThreatenedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterManager.Instance.ShowHighlightIfThreatened = !highlightIfThreatenedToolStripMenuItem.Checked;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_simulationStarted)
            {
                _simulationPaused = false;
                stopSimulation();
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_simulationStarted)
            {
                startSimulation();
            }
        }

        private void startSimulation()
        {
            _gameThread = new Thread(new ThreadStart(gameLoop));
            _gameThread.Start();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _simulationPaused = !pauseToolStripMenuItem.Checked;
        }

        private void simulationToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            stopToolStripMenuItem.Enabled = _simulationStarted;
            startToolStripMenuItem.Enabled = !_simulationStarted;
            pauseToolStripMenuItem.Enabled = _simulationStarted;
            pauseToolStripMenuItem.Checked = _simulationPaused;
        }

    }
}