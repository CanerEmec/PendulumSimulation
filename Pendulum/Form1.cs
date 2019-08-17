using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pendulum
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics gr, grV, grH, grA;
        Pendulum pdl;
        Pivot pvt;
        Rod rod;
        Bob bob;
        Color bgColor,pvtColor, bobColor;
        Size pvtSize, bobSize;
        double rodLength, bobMass, angle;
        int step, stoptime;
        Pendulum.CoordinatePoint coordinatePoint;

        private void btnClColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                lColor.BackColor = colorDialog1.Color;
                bgColor = colorDialog1.Color;
            }
        }

        private void nudPvtSize_ValueChanged(object sender, EventArgs e)
        {
            pvtSize.Width = (int)nudPvtSize.Value;
            pvtSize.Height = (int)nudPvtSize.Value;
        }

        private void cmbCoordP_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbCoordP.SelectedIndex)
            {
                case 0:
                    coordinatePoint = Pendulum.CoordinatePoint.Pivot;
                    break;
                case 1:
                    coordinatePoint = Pendulum.CoordinatePoint.Bob;
                    break;
            }
        }

        private void nudStep_ValueChanged(object sender, EventArgs e)
        {
            step = (int)nudStep.Value;
        }

        private void nudStop_ValueChanged(object sender, EventArgs e)
        {
            stoptime = (int)nudStop.Value;
        }

        private void nudbobMass_ValueChanged(object sender, EventArgs e)
        {
            bobMass = (double)nudbobMass.Value;
        }

        private void nudAngle_ValueChanged(object sender, EventArgs e)
        {
            angle = (double)nudAngle.Value;
        }

        private void nudBobSize_ValueChanged(object sender, EventArgs e)
        {
            bobSize.Width = (int)nudBobSize.Value;
            bobSize.Height = (int)nudBobSize.Value;
        }

        private void nudRodL_ValueChanged(object sender, EventArgs e)
        {
            rodLength = (double)nudRodL.Value;
        }

        private void btnBobColor_Click(object sender, EventArgs e)
        {

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                lBobColor.BackColor = colorDialog1.Color;
                bobColor = colorDialog1.Color;
            }
        }

        private void btnPvtColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                lPvtColor.BackColor = colorDialog1.Color;
                pvtColor = colorDialog1.Color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gr = pMain.CreateGraphics();
            grV = pVertical.CreateGraphics();
            grH = pHorizantal.CreateGraphics();
            grA = pAngle.CreateGraphics();

            bgColor = Color.GhostWhite;
            lColor.BackColor = bgColor;
            pvtColor = Color.Red;
            lPvtColor.BackColor = Color.Red;

            pvtSize = new Size(20, 20);
            rodLength = 200;

            bobColor = Color.CadetBlue;
            bobSize = new Size(50, 50);
            bobMass = 10;//kg

            angle = 65;
            step = 500;
            stoptime = 100;
            coordinatePoint = Pendulum.CoordinatePoint.Bob;
            cmbCoordP.SelectedIndex = 1;

            gr.Clear(bgColor);
            grV.Clear(bgColor);
            grH.Clear(bgColor);
            grA.Clear(bgColor);

            pvt = new Pivot(new Point(pMain.Width / 2, 20), pvtSize, pvtColor);
            rod = new Rod(rodLength);
            bob = new Bob(bobMass, bobSize, bobColor);

            pdl = new Pendulum(pvt, rod, bob);
            pdl.Set_CoordPoint(coordinatePoint, new Point(0, 0));

            pdl.SimulationUpdated += Pdl_SimulationUpdated;
        }

        private void Pdl_SimulationUpdated(object sender, Pendulum.SimulationUpdatedEventArg e)
        {
            // Update Charts
            
        //    throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pdl.Simulate(gr, grV, grH, grA, pMain.Size, pVertical.Size, pHorizantal.Size, pAngle.Size, bgColor,angle, step, stoptime);


        }
    }
}
