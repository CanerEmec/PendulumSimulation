using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Pendulum
{
    class Pendulum
    {
        public Pendulum(Pivot pivot, Rod rod, Bob bob)
        {
            this._pivot = pivot;
            this._rod = rod;
            this._bob = bob;

            pointP.X = _pivot.Location.X + _pivot.Size_.Width / 2;
            pointP.Y = Convert.ToInt32(_rod.Length) + _pivot.Size_.Height / 2;

            Vector3 GravitationalForce = new Vector3(0, -9.81 * _bob.Mass, 0);

            Forces.Add(GravitationalForce);
        }

        #region Events
        public class SimulationUpdatedEventArg : EventArgs
        {
            public SimulationParameters Parameter;
            public Vector3 Value;
            public double Time;
        }

        public event EventHandler<SimulationUpdatedEventArg> SimulationUpdated;
        #endregion

        public CoordinatePoint CoordPoint { get; set; }
        private Point CoordinateP = new Point();
        private Point pointP = new Point();
        private SimParameters simParameters;
        private Matrix3x3 TransformationMatrix = new Matrix3x3(0, 0, 0, 0, 0, 0, 0, 0, 1);
        private Vector3 NetForce = new Vector3(), NetForce2;
        private List<Vector3> Forces = new List<Vector3>();

        private Pivot _pivot;
        private Rod _rod;
        private Bob _bob;

        public void ConstructPendulum(double angle, Graphics gr, Size size0)
        {
            pointP.X = _pivot.Location.X + _pivot.Size_.Width / 2;
            pointP.Y = Convert.ToInt32(_rod.Length) + _pivot.Size_.Height / 2;
            Forces[0] = new Vector3(0, -9.81 * _bob.Mass, 0);

            InitScene(gr, pointP);


            _pivot.Draw(gr);
            Point loc = new Point();
            loc.X = _pivot.Location.X + _pivot.Size_.Width / 2;
            loc.Y = _pivot.Location.Y + _pivot.Size_.Height / 2;

            _rod.Angle = angle;
            _rod.Draw(gr, loc);

            Point loc1 = new Point();
            Size sizeBob = _bob.Size_;// new Size(100, 100); 
            loc1.X = _rod.Point2.X - sizeBob.Width / 2;
            loc1.Y = _rod.Point2.Y - sizeBob.Height / 2;
            _bob.CreateRect(loc1, sizeBob);

            _bob.Draw(gr, CoordinateP, size0);

            
        }

        private void InitScene(Graphics gr, Point point)
        {
            Point loc = new Point();
            loc.X = _pivot.Location.X + _pivot.Size_.Width / 2;
            loc.Y = _pivot.Location.Y + _pivot.Size_.Height / 2;

            Pen pen = new Pen(Color.DimGray);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            

            gr.DrawLine(pen, loc, point);

            
        }

        public void CreateVerticalRuler(Graphics gr, Size size0)
        {
            Size size = new Size(20, 20);
            Rectangle rect = new Rectangle(new Point(size0.Width / 2 - size.Width / 2, _bob.Location.Y + (_bob.Size_.Height - size.Height) / 2), size);
            Pen pen = new Pen(Color.DimGray);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            gr.DrawLine(pen, new Point(size0.Width / 2, 0), new Point(size0.Width / 2, size0.Height));
            gr.DrawLine(pen, new Point(0, rect.Y + size.Height / 2), new Point(size0.Width, rect.Y + size.Height / 2));

            gr.FillEllipse(Brushes.Red, rect);
            gr.DrawEllipse(Pens.Black, rect);

            pen.Dispose();
        }

        public void CreateHorizantalRuler(Graphics gr, Size size0)
        {
            Size size = new Size(20, 20);
            Rectangle rect = new Rectangle(new Point( _bob.Location.X + (_bob.Size_.Width - size.Width) / 2, size0.Height / 2 - size.Height / 2), size);
            Pen pen = new Pen(Color.DimGray);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            gr.DrawLine(pen, new Point(0, size0.Height / 2), new Point(size0.Width, size0.Height / 2));
            gr.DrawLine(pen, new Point(rect.X + size.Width / 2, 0), new Point(rect.X + size.Width / 2, size0.Height));

            gr.FillEllipse(Brushes.Red, rect);
            gr.DrawEllipse(Pens.Black, rect);

            pen.Dispose();
        }

        public void DrawAnglePie(Graphics gr, Size size0, Double angle)
        {
            Size size = new Size(size0.Width - 20, size0.Height - 20);
            Rectangle rect = new Rectangle(new Point(10, 10), size);
            Rectangle rect2 = new Rectangle(new Point(10, 10), new Size(size.Width / 2, size.Height / 2));
            Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Italic);

            gr.FillPie(Brushes.Red, rect, 90, (float)angle);
            gr.DrawPie(Pens.Black, rect, 90, (float)(angle));

            gr.DrawString("A: " + Math.Abs(angle), font, Brushes.Black, rect2.Location);
        }

        public void Set_CoordPoint(CoordinatePoint coordinate, Point point)
        {
            switch (coordinate)
            {
                case CoordinatePoint.Pivot:
                    CoordinateP.X = _pivot.Location.X + _pivot.Size_.Width / 2;
                    CoordinateP.Y = _pivot.Location.Y + _pivot.Size_.Height / 2;
                    break;

                case CoordinatePoint.Bob:
                    CoordinateP.X = _pivot.Location.X + _pivot.Size_.Width / 2;
                    CoordinateP.Y = Convert.ToInt32(_rod.Length) + _pivot.Size_.Height / 2 + _pivot.Location.Y;
                    break;

                case CoordinatePoint.Custom:
                    CoordinateP = point;
                    break;

                default:
                    CoordinateP.X = _pivot.Location.X + _pivot.Size_.Width / 2;
                    CoordinateP.Y = Convert.ToInt32(_rod.Length) + _pivot.Size_.Height / 2; ;
                    break;
            }
        }

        public async void Simulate(Graphics grMain, Graphics grVertical, Graphics grHorizantal, Graphics grAngle, Size sMain, Size sVertical, Size sHorizantal, Size sAngle, Color bgColor, double angle_, int step, int time)
        {
            simParameters = new SimParameters();
            simParameters.Time = 0;
            simParameters.dT = (step/1000.0);
            simParameters.StopTime = time;
            simParameters.AngularPosition.x = angle_;

            CalcNetForce();

            await Task.Run(() => 
            {
                while (simParameters.Time < simParameters.StopTime)
                {
                    UpdateTransformationMatrix(simParameters.AngularPosition.x);
                    NetForce2 = CrossProduct(TransformationMatrix, NetForce);
                    NetForce2.y = 0;
                    UpdateAngAcceleration();
                    UpdateAngVelocity();
                    UpdateAngPosition();

                    simParameters.Time += simParameters.dT;

                    DrawAll(grMain, grVertical, grHorizantal, grAngle, sMain, sVertical, sHorizantal, sAngle, simParameters.AngularPosition.x, bgColor);

                }
            });
        }

        public void DrawAll(Graphics grMain, Graphics grVertical, Graphics grHorizantal, Graphics grAngle, Size sMain, Size sVertical, Size sHorizantal, Size sAngle, double angle, Color color)
        {
            grMain.Clear(color);
            grVertical.Clear(color);
            grHorizantal.Clear(color);
            grAngle.Clear(color);

            ConstructPendulum(angle, grMain, sMain);
            CreateVerticalRuler(grVertical, sVertical);
            CreateHorizantalRuler(grHorizantal, sHorizantal);
            DrawAnglePie(grAngle, sAngle, angle);
            Task.Delay(50).Wait();
        }

        public void AddForce(Vector3 force)
        {
            Forces.Add(force);
        }

        private void CalcNetForce()
        {
            foreach (Vector3 item in Forces)
            {
                NetForce = SumForces(NetForce, item);
            }
        }

        private void UpdateTransformationMatrix(double angle)
        {
            TransformationMatrix.Values[0, 0] = Math.Cos(angle * Math.PI / 180);
            TransformationMatrix.Values[1, 1] = Math.Cos(angle * Math.PI / 180);
            TransformationMatrix.Values[0, 1] = Math.Sin(angle * Math.PI / 180);
            TransformationMatrix.Values[1, 0] = -Math.Sin(angle * Math.PI / 180);
        }

        private void UpdateAngAcceleration()
        {
            simParameters.AngularAcc.x = NetForce2.x / (_bob.Mass * _rod.Length/10);
            simParameters.AngularAcc.y = NetForce2.y / (_bob.Mass * _rod.Length/10);
            simParameters.AngularAcc.z = NetForce2.z / (_bob.Mass * _rod.Length/10);

            SimulationUpdatedEventArg arg = new SimulationUpdatedEventArg();
            arg.Parameter = SimulationParameters.AngularAcceleration;
            arg.Value = simParameters.AngularAcc;
            arg.Time = simParameters.Time;


            SimulationUpdated?.Invoke(this, arg);
        }

        private void UpdateAngVelocity()
        {
            simParameters.AngularVelocity.x += simParameters.AngularAcc.x * simParameters.dT;
            simParameters.AngularVelocity.y += simParameters.AngularAcc.y * simParameters.dT;
            simParameters.AngularVelocity.z += simParameters.AngularAcc.z * simParameters.dT;

            SimulationUpdatedEventArg arg = new SimulationUpdatedEventArg();
            arg.Parameter = SimulationParameters.AngularVelocity;
            arg.Value = simParameters.AngularVelocity;
            arg.Time = simParameters.Time;

            SimulationUpdated?.Invoke(this, arg);
        }

        private void UpdateAngPosition()
        {
            simParameters.AngularPosition.x += simParameters.AngularVelocity.x * simParameters.dT + 0.5 * simParameters.AngularAcc.x * simParameters.dT * simParameters.dT;
            simParameters.AngularPosition.y += simParameters.AngularVelocity.y * simParameters.dT + 0.5 * simParameters.AngularAcc.y * simParameters.dT * simParameters.dT;
            simParameters.AngularPosition.z += simParameters.AngularVelocity.z * simParameters.dT + 0.5 * simParameters.AngularAcc.z * simParameters.dT * simParameters.dT;

            simParameters.AngularPosition.x %= 360;
            simParameters.AngularPosition.y %= 360;
            simParameters.AngularPosition.z %= 360;


            SimulationUpdatedEventArg arg = new SimulationUpdatedEventArg();
            arg.Parameter = SimulationParameters.AngularPosition;
            arg.Value = simParameters.AngularPosition;
            arg.Time = simParameters.Time;


            SimulationUpdated?.Invoke(this, arg);
        }

        private Vector3 SumForces(Vector3 f1, Vector3 f2)
        {
            return new Vector3
            {
                x = f1.x + f2.x,
                y = f1.y + f2.y,
                z = f1.z + f2.z
            };
        }

        private Vector3 CrossProduct(Matrix3x3 mat3x3, Vector3 vec3)
        {
            Vector3 result = new Vector3();

            result.x = vec3.x * mat3x3.Values[0, 0] + vec3.y * mat3x3.Values[0, 1] + vec3.z * mat3x3.Values[0, 2];
            result.y = vec3.x * mat3x3.Values[1, 0] + vec3.y * mat3x3.Values[1, 1] + vec3.z * mat3x3.Values[1, 2];
            result.z = vec3.x * mat3x3.Values[2, 0] + vec3.y * mat3x3.Values[2, 1] + vec3.z * mat3x3.Values[2, 2];

            return result;
        }

        public struct Vector3
        {
            public Vector3(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public double x, y, z;
        }

        public struct Matrix3x3
        {
            public Matrix3x3(double idx11, double idx12, double idx13,
                             double idx21, double idx22, double idx23,
                             double idx31, double idx32, double idx33)
            {
                Values = new double[,] { {idx11, idx12, idx13 },
                                         {idx21, idx22, idx23 },
                                         {idx31, idx32, idx33 } };
            }

            public double[,] Values; 
        }

        public struct SimParameters
        {
            public double dT, Time, StopTime;
            public Vector3 AngularAcc, AngularVelocity, AngularPosition;
            //public Vector3 GravitationalForce;
        }

        public enum CoordinatePoint
        {
            Pivot,
            Bob,
            Custom
        }

        public enum SimulationParameters
        {
            Time,
            AngularAcceleration,
            AngularVelocity,
            AngularPosition
        }

    }

    class Bob
    {
        public Bob(double mass, Size size, Color color)
        {
            this.Mass = mass;
            _rectangle = new Rectangle();
            _rectangle.Size = size;
            this.Color_ = color;
        }

        public Bob(double mass, Point loc, Size size, Color color) : this(mass, size, color)
        {
            this._rectangle = new Rectangle(loc, size);
            this._pointText = new Point(loc.X + size.Width + 10, loc.Y);
            //this.Mass = mass;
        }

        public double Mass { get; set; }    // kg

        public Color Color_;
        private Rectangle _rectangle;
        private Point _pointText;

        public Point Location
        {
            get
            {
                return _rectangle.Location;
            }

            set
            {
                _rectangle.Location = value;
            }
        }

        public Size Size_
        {
            get
            {
                return _rectangle.Size;
            }
            set
            {
                _rectangle.Size = value;
            }
        }

        public void CreateRect(Point loc,Size size)
        {
            _rectangle = new Rectangle(loc, size);
            this._pointText = new Point(loc.X + size.Width + 10, loc.Y);
        }

        public void Draw(Graphics gr, Point coordPoint, Size sizecontainer)
        {
            Brush br = new SolidBrush(Color_);

            gr.FillEllipse(br, _rectangle);
            gr.DrawEllipse(Pens.Black, _rectangle);

            Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Italic);

            string info = "Mass: " + Mass +
                " kg\nX: " + (Location.X - coordPoint.X + Size_.Width / 2) + "\nY: " + (-Location.Y + coordPoint.Y - Size_.Height / 2);

            gr.DrawString(info, font, Brushes.Black, _pointText);


            Pen pen = new Pen(Color.DimGray);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            Point locH1 = new Point();
            locH1.X = 0;
            locH1.Y = _rectangle.Y + _rectangle.Size.Height / 2;

            Point locH2 = new Point(x: sizecontainer.Width, y: locH1.Y);

            Point locV1 = new Point(x: _rectangle.X + _rectangle.Size.Width / 2, y: 0);
            Point locV2 = new Point(x: _rectangle.X + _rectangle.Size.Width / 2, y: sizecontainer.Height);

            gr.DrawLine(pen, locH1, locH2);
            gr.DrawLine(pen, locV1, locV2);
            pen.Dispose();
        }

    }

    class Rod
    {
        public Rod(double length)
        {
            this.Length = length;
        }

        public Rod(double length, double angle):this(length)
        {
            this.Angle = angle;
        }

        public double Length { get; set; }
        public double Angle { get; set; }
        public Point Point2 { get; private set; }
        private Rectangle _rectangle = new Rectangle();

        public void Draw(Graphics gr, Point point1)
        {
            Point secondP = new Point(), TPoint = new Point();
            secondP.X = point1.X + Convert.ToInt32(Length * Math.Sin(Angle * Math.PI / 180));
            secondP.Y = point1.Y + Convert.ToInt32(Length * Math.Cos(Angle * Math.PI / 180));
            Point2 = secondP;

            TPoint.X = point1.X + Convert.ToInt32(Length/3 * Math.Sin(Angle * Math.PI / 180));
            TPoint.Y = point1.Y + Convert.ToInt32(Length/3 * Math.Cos(Angle * Math.PI / 180));

            Pen pen = new Pen(Color.Brown);
            pen.Width = 2;

            gr.DrawLine(pen, point1, secondP);

            Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Italic);

            gr.DrawString("L: " + Length, font, Brushes.Black, TPoint);
        }
    }

    class Pivot
    {
        public Pivot(Point loc, Size size, Color color)
        {
            _rectangle = new Rectangle(loc, size);
            this.Color_ = color;
        }

        public Color Color_;

        private Rectangle _rectangle;

        public Point Location
        {
            get
            {
                return _rectangle.Location;
            }

            set
            {
                _rectangle.Location = value;
            }
        }

        public Size Size_
        {
            get
            {
                return _rectangle.Size;
            }
            set
            {
                _rectangle.Size = value;
            }
        }


        public void Draw(Graphics gr)
        {
            Brush br = new SolidBrush(Color_);
            
            gr.FillEllipse(br, _rectangle);
            gr.DrawEllipse(Pens.Black, _rectangle);
        }
    }
}
