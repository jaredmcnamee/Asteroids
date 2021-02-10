using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Asteroids
{
    /// <summary>
    /// Base class for all assets in the game has all basic class members and methods
    /// </summary>
    abstract public class Asset
    {
        protected float _fRadius;//the radius of the shape
        protected float _fRotation;//current rotation
        protected float _fRotationIncrement;//amount object rotates per tick
        protected float _fXSpeed;//x velocity in the drawer
        protected float _fYSpeed;//y velocity in the drawer
        protected static Random _rand = new Random();//Random object shared by all shapes
        protected Asset _ref;//the parent object if the asset requires it

        public float Rotation { get { return _fRotation; } }//the rotation of the object
        public float RotationDelta { get { return _fRotationIncrement; } set { _fRotationIncrement = value; } }//the change in rotation per tick of the object
        public Color baseColor { get; set; }//the color of the shape
        public bool IsMarkedForDeath { get; set; }//whether or not the shape will be destroyed next remove cycle
        public PointF location { get; set; }//location of the shape on screen
        public float radius { get { return _fRadius; } }//returns the radius of the shape

        /// <summary>
        /// Initializing the bas shape to all the common parameters
        /// </summary>
        /// <param name="pos"></param>
        public Asset(PointF pos)
        {
            location = pos;
            _fRotation = 0;

            IsMarkedForDeath = false;
        }
        /// <summary>
        /// method for finding the location of the graphics path after transforms
        /// </summary>
        /// <returns></returns>
        abstract public GraphicsPath GetPath();
        /// <summary>
        /// method that determines how the shape moves in the drawer
        /// </summary>
        /// <param name="s"></param>
        abstract public void Tick(Size s);
        /// <summary>
        /// rendering the provided path with the set color
        /// </summary>
        /// <param name="gr"></param>
        public virtual void Render(Graphics gr)
        {
            //gr.FillPath(new SolidBrush(baseColor), GetPath());
            gr.DrawPath(new Pen(baseColor), GetPath());
        }
        /// <summary>
        /// grabs the shapes region for boundary checking
        /// </summary>
        /// <returns></returns>
        abstract public Region GetRegion();
        
        /// <summary>
        /// Checks if the distance between two shapes is getting close to a collision
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Dist(Asset b)
        {
            double dist = Math.Sqrt(Math.Pow(location.X - b.location.X, 2) + Math.Pow(location.Y - b.location.Y, 2));

            double radii = b.radius + radius;

            if (dist <= radii)
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// Class that represents the player in the game holds the information about the ship and it position
    /// </summary>
    public class Player: Asset
    {
        //only one player should only make one model
        public static readonly GraphicsPath _Model;
        
        //contructor that generates the player model
        static Player()
        {
            _Model = new GraphicsPath();
            _Model.AddPolygon(new PointF[] { new PointF(0, -15), new PointF(-10, 10), new PointF(0, 7), new PointF(10, 10) });
        }

        /// <summary>
        /// initializing the player to their initial position and rotation
        /// </summary>
        /// <param name="pos"></param>
        public Player(PointF pos): base(pos)
        {
            baseColor = Color.Yellow;
            _fXSpeed = 0;
            _fYSpeed = 0;
            _fRotation = 0;
            _fRotationIncrement = 0;
            _fRadius = 20;
            
        }

        /// <summary>
        /// rendering the triangle after transforms
        /// </summary>
        /// <returns></returns>
        public override GraphicsPath GetPath()
        {
            //Matrix for containing the tranforms
            Matrix MatTrans = new Matrix();

            //translate first
            MatTrans.Translate(location.X, location.Y);
            //then rotate
            MatTrans.Rotate(_fRotation);
            //creating a working path
            GraphicsPath temp = (GraphicsPath)_Model.Clone();
            //applying transforms
            temp.Transform(MatTrans);
            //incrementing the rotation
            _fRotation += _fRotationIncrement;
            //creating a region for distance checking
            Region reg = new Region(temp);

            
            return temp;
        }
        /// <summary>
        /// assigning the location of the player to the center of the window
        /// </summary>
        /// <param name="s"></param>
        public override void Tick(Size s)
        {
            location = new PointF(s.Width / 2, s.Height / 2);
            _fXSpeed = 0;
            _fYSpeed = 0;

            
        }
        /// <summary>
        /// getting the region of the player character to detect collisions
        /// </summary>
        /// <returns></returns>
        public override Region GetRegion()
        {
            return new Region(_Model);
        }
    }
    /// <summary>
    /// Class that represents the bullets the player is firing at the asteroid 
    /// this class is a child of the player class
    /// </summary>
    public class Bullet: Asset
    {
        //model for the bullet
        public static readonly GraphicsPath _model;
        /// <summary>
        /// creating the bullet model for all bullets
        /// </summary>
        static Bullet()
        {
            _model = new GraphicsPath();
            _model.AddRectangle(new Rectangle(new Point(0,-10), new Size(1,2)));
        }
        /// <summary>
        /// creating the bullet it must have a player reference
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="player"></param>
        public Bullet(PointF pos, Player player) : base(pos)
        {
            _ref = player; // each bullet must have a player

            location = _ref.location;
            baseColor = Color.White;
            _fXSpeed = -6;
            _fYSpeed = -6;
            _fRotation = _ref.Rotation;
            _fRadius = 5;
        }  
        /// <summary>
        /// every tick moving the bullet along its path until it reaches the edge of the screen where it dies
        /// </summary>
        /// <param name="s"></param>
        public override void Tick(Size s)
        {
            float DeltaX = location.X;
            float DeltaY = location.Y;

            
            DeltaX += -(float)Math.Sin(_fRotation * Math.PI / 180) * _fXSpeed;
            DeltaY += (float)Math.Cos(_fRotation * Math.PI / 180) * _fYSpeed;
            location = new PointF(DeltaX, DeltaY);
             
            
            if(location.X > s.Width || location.X < 0 || location.Y > s.Height|| location.Y < 0)
                IsMarkedForDeath = true;
        }
        /// <summary>
        /// Applying all transforms to the bullet
        /// </summary>
        /// <returns></returns>
        public override GraphicsPath GetPath()
        {
            Matrix transforms = new Matrix();
            transforms.Translate(location.X, location.Y);
            transforms.Rotate(_fRotation);
            

            GraphicsPath temp = (GraphicsPath)_model.Clone();
            
            temp.Transform(transforms);
            
            _fRotation += _fRotationIncrement;

            Region reg = new Region(temp);
            return temp;
        }
        /// <summary>
        /// method that returns the region the bullet resides in
        /// </summary>
        /// <returns></returns>
        public override Region GetRegion()
        {
            return new Region(_model);
        }
    }
    /// <summary>
    /// Class that represents the asteroids that the player will be attempting to destroy
    /// creates random polygons with random rotations and velocities that fade into screen and fade out when they leave screen
    /// </summary>
    public class Asteroid : Asset
    {
        //every asteroid has it own model
        readonly GraphicsPath _Model = new GraphicsPath();
        //the int that represents the alpha of the asteroids color
        private int _alpha;
        //if true signifies a brand new asteroid that should be faded in
        private bool _bfadeIn;
        private bool _dying;

        public AsterSize aSize { get; set; }
        public bool isAlive { get { return !_bfadeIn; } }
        /// <summary>
        /// Generating the shape of the asteroid and setting its initial settings
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size">Enumeration that determines the size of the asteroid</param>
        public Asteroid(PointF pos, AsterSize size): base(pos)
        {
            _Model.AddPath(MakePolyPath(new PointF(0,0), (int)size,0,5, 7),true);
            _fRadius = (float)size;
            baseColor = Color.Red;
            _fRotation = 0;
            _fRotationIncrement = (float)(_rand.NextDouble() * 6.00 - 3.00);
            _fXSpeed = (float)(_rand.NextDouble() * 5 - 2.5);
            _fYSpeed = (float)(_rand.NextDouble() * 5 - 2.5);
            _alpha = 0;
            aSize = size;
            _bfadeIn = true;
            _dying = false;
        }
        /// <summary>
        /// Work horse function behind creating the random polygons for the asteroids
        /// </summary>
        /// <param name="pos"> location of the asteroid</param>
        /// <param name="radius">radius of the asteroid</param>
        /// <param name="variance"> radius variance in the asteroid</param>
        /// <param name="spikeyness">how acute the interior angles of the polygon are</param>
        /// <param name="vertices"> number of vertices in the polygon</param>
        /// <returns></returns>
        static protected GraphicsPath MakePolyPath(PointF pos, float radius, float variance, float spikeyness, int vertices)
        {
            //the return path 
            GraphicsPath Path = new GraphicsPath();

            //generating a variance value based on user input
            variance = (float)(Clamp(variance, 0, 1) * 2 * Math.PI / vertices);
            //generating the spikeyness based on user input
            spikeyness = (float)(Clamp(spikeyness, 0, 1) * radius);

            //the number angled steps in the polygon
            float[] steps = new float[vertices];
            //the lower limit
            float lower = (float)(2 * Math.PI / vertices) - variance;
            //the uper limit
            float upper = (float)(2 * Math.PI / vertices) + variance;
            //sum of the angles
            float sum = 0.0F;

            //generating the interior angles of the polygon
            for (int i = 0; i < vertices; i++)
            {
                float temp = (float)(_rand.NextDouble() * (upper + lower) + lower);
                steps[i] = temp;
                sum += temp;
            }
            //conforming the interior angles to a bounding circle 
            float k = (float)(sum / (2 * Math.PI));
            for (int i = 0; i < vertices; i++)
            {
                steps[i] = steps[i] / k;
            }

            //array to hold the points in the polygon
            PointF[] pts = new PointF[vertices];
            //the angle the point is at from the origin
            float angle = (float)(_rand.NextDouble() * (2 * Math.PI));

            //generating the points in the polygon
            for (int i = 0; i < vertices; i++)
            {
                float temp = (float)(Clamp((float)_rand.NextDouble() * (radius + spikeyness), 0, (int)(2 * radius)));
                float x = (float)(pos.X + (temp * Math.Cos(angle)));
                float y = (float)(pos.Y + (temp * Math.Sin(angle)));
                pts[i] = new PointF(x, y);

                //incrementing the angle
                angle += steps[i];
            }
            //adding the points to the path
            Path.AddPolygon(pts);
            //returning the finished graphics path
            return Path;
        }
        /// <summary>
        /// helper method to clamp the value to an inclusive range of min and max
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public override void Tick(Size s)
        {
            ////if the x velocity would take the object outside the boundary reversing its velocity
            //if (((location.X + _fRadius + _fXSpeed) >= s.Width - _fRadius) || ((location.X - _fRadius + _fXSpeed) <= 0))
            //{
            //    _fXSpeed *= -1;
            //}
            ////if the y velocity would take the object outside the boundary reversing its veolocity
            //if (((location.Y + _fRadius + _fYSpeed) >= s.Height - _fRadius) || ((location.Y - _fRadius + _fYSpeed) <= 0))
            //{
            //    _fYSpeed *= -1;
            //}


            //applying the changes in velocity
            location = new PointF(location.X + _fXSpeed, location.Y + _fYSpeed);

            //if the asteroid floats past the edge of the screen marking it for removal
            if (location.X > s.Width + 25 || location.X < 0 - 25 || location.Y > s.Height + 25 || location.Y < 0 - 25)
                _dying = true;

            //fading new asteroids in and 
            if(_bfadeIn)
            {
                if (_alpha < 255)
                    _alpha += 15;
                else
                    _bfadeIn = false;
                fade();
            }
            //fading dead asteroids out
            if (_dying && !_bfadeIn)
            {
                if (_alpha >= 15)
                    _alpha -= 15;
                if (_alpha == 0)
                    IsMarkedForDeath = true;
                fade();

            }
        }
        /// <summary>
        /// helper method that just creates a new base color based on its rgb parts
        /// </summary>
        public void fade()
        {
            Color temp = baseColor;
            int r = temp.R;
            int g = temp.G;
            int b = temp.B;

            baseColor = Color.FromArgb(_alpha, r, g, b);
        }

        /// <summary>
        /// rendering the rock after transforms
        /// </summary>
        /// <returns></returns>
        public override GraphicsPath GetPath()
        {
            //Matrix for containing the tranforms
            Matrix MatTrans = new Matrix();
            //translate first
            MatTrans.Translate(location.X, location.Y);
            //then rotate
            MatTrans.Rotate(_fRotation);
            //creating a working path
            GraphicsPath temp = (GraphicsPath)_Model.Clone();
            //applying transforms
            temp.Transform(MatTrans);
            //incrementing the rotation
            _fRotation += _fRotationIncrement;
            //creating a region for distance checking
            Region reg = new Region(temp);

            return temp;
        }
        /// <summary>
        /// gets the region the asteroid takes up
        /// </summary>
        /// <returns></returns>
        public override Region GetRegion()
        {
            return new Region(_Model);
        }
    }

    public class WaitingScreen: Asset
    {
        protected readonly GraphicsPath _Model;

        public  WaitingScreen(PointF pos):base(pos)
        {
            //Ui is white and doesn't move
            baseColor = Color.White;
            _fXSpeed = 0;
            _fYSpeed = 0;
            _fRotation = 0;
            _fRotationIncrement = 0;
            //itializing the graphics path
            _Model = new GraphicsPath();
        }

        /// <summary>
        /// positioning the interface to the top right corner
        /// </summary>
        /// <param name="s"></param>
        public override void Tick(Size s)
        {
            location = new PointF((s.Width / 2), (s.Height / 2));
        }

        public override GraphicsPath GetPath()
        {
            //Matrix for containing the tranforms
            Matrix MatTrans = new Matrix();

            //translate first
            MatTrans.Translate(location.X, location.Y);
            //then rotate
            MatTrans.Rotate(_fRotation);
            //creating a working path
            GraphicsPath temp = (GraphicsPath)_Model.Clone();

            //adding the string that shows the waiting screen
            temp.AddString("Asteroids", FontFamily.GenericSansSerif, (int)FontStyle.Regular, 70, new PointF(-155, -40), StringFormat.GenericTypographic);
            
            //applying transforms
            temp.Transform(MatTrans);
            //creating a region for distance checking
            Region reg = new Region(temp);

            return temp;
        }
        /// <summary>
        /// provides the region the UI takes up
        /// </summary>
        /// <returns></returns>
        public override Region GetRegion()
        {
            return new Region(_Model);
        }
    }
    /// <summary>
    /// Class for creating and displaying the User Interface ie score and life counter
    /// </summary>
    public class UserInterface: Asset
    {
        protected int _lives;//the number of lives left
        protected string _score;//the score
        protected Font UI;//the font for the score

        //the graphics path that contains the entire ui
        protected readonly GraphicsPath _Model;
        //property for getting and setting the life of the player
        public int Lives { get { return _lives; } set { _lives = value; } }
        //the property for getting and setting the score
        public int Score { get { return int.Parse(_score); } set { _score = value.ToString("000000"); } }

        /// <summary>
        /// itializing the ui to all the values
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="score"></param>
        /// <param name="lives"></param>
        public UserInterface(PointF pos, int score, int lives): base(pos)
        {
            Lives = lives;
            Score = score;
            //Ui is white and doesn't move
            baseColor = Color.White;
            _fXSpeed = 0;
            _fYSpeed = 0;
            _fRotation = 0;
            _fRotationIncrement = 0;
            //itializing the graphics path
            _Model = new GraphicsPath();
        }

        /// <summary>
        /// positioning the interface to the top right corner
        /// </summary>
        /// <param name="s"></param>
        public override void Tick(Size s)
        {
            location = new PointF(s.Width/20, s.Height/20);
        }

        public override GraphicsPath GetPath()
        {
            //Matrix for containing the tranforms
            Matrix MatTrans = new Matrix();

            //translate first
            MatTrans.Translate(location.X, location.Y);
            //then rotate
            MatTrans.Rotate(_fRotation);
            //creating a working path
            GraphicsPath temp = (GraphicsPath)_Model.Clone();

            //adding the string that shows the score
            temp.AddString(_score, FontFamily.GenericSansSerif, (int)FontStyle.Regular, 20, new PointF(0, 0), StringFormat.GenericTypographic);
            //adding a visual life counter for each life the player has
            for (int i = 0; i < _lives; i++)
            {
                int x = (i * 20);
                
                temp.AddPolygon(new PointF[] { new PointF(10 + x, 25), new PointF(0 + x, 50), new PointF(10 + x, 47), new PointF(20 + x, 50) });
                //_Model.AddPath(temp, false);
            }
            //applying transforms
            temp.Transform(MatTrans);
            //creating a region for distance checking
            Region reg = new Region(temp);

            return temp;
        }
        /// <summary>
        /// provides the region the UI takes up
        /// </summary>
        /// <returns></returns>
        public override Region GetRegion()
        {
            return new Region(_Model);
        }
    }
}
