using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Asteroids
{
    /// <summary>
    /// Enum that dictates the size of an asteroid for readability 
    /// </summary>
    public enum AsterSize
    {
        Small = 20,
        Medium = 40,
        Large = 60
    }
    /// <summary>
    /// Struct for holding the highscore info 
    /// </summary>
    [Serializable]
    struct HighScore
    {
        public string _name;
        public int _score;
    };
    //delegate for starting the game
    public delegate void delvoidbool( bool start);
    //delegate for updating the score
    public delegate void delvoidInt(int score);
    public partial class Form1 : Form
    {
        //creating the game menu
        GameMenu gm;
        //all assets in the game
        List<Asset> asteroids = new List<Asset>();
        List<Asset> playerAssets = new List<Asset>();
        List<Asset> bullets = new List<Asset>();
        //timer that controls animation 
        Timer gameTimer;
        //random object for placing asteroids
        Random rand = new Random();

        bool gameRunning = false;//whether the game is running or not
        int _score = 0; //players score
        int _extraGuy = 0;//point until the player gets another life
        int _lives = 3;//total lives the player has
        int _difficulty = 10;//current difficulty 
        int _tickCount = 0;//tick counter for the difficulty progression

        public Form1()
        {
            InitializeComponent();
            //setting the size of the window
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;

            //creating the game timer and starting it
            gameTimer = new Timer();
            gameTimer.Start();

            gameTimer.Tick += GameTimer_Tick;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            //adding the player assets to the player list
            playerAssets.Add(new UserInterface(new PointF((float)Size.Width / 20, (float)Size.Height / 20), _score, _lives));
            playerAssets.Add(new Player(new PointF(Size.Width / 2, Size.Height / 2)));

            //showing the game menue
            GameMenuScreen();
        }

        /// <summary>
        /// call back for starting the game from the modeless dialog
        /// </summary>
        /// <param name="start"></param>
        public void StartGameCallback(bool start)
        {
            gameRunning = start;
        }
        /// <summary>
        /// creates the modeless dialog if it doesn't exist and showing it
        /// </summary>
        public void GameMenuScreen()
        {
            if (gm == null)
            {
                gm = new GameMenu();
                gm.Location = new Point(Location.X + Width, Location.Y);
                gm._gameStart = new delvoidbool(StartGameCallback);
                gm.Show();
            }
        }
        /// <summary>
        /// every timer tick rendering the shapes to the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            //adding an extra life every 10000 points
            if (_extraGuy > 10000)
            {
                ++_lives;
                _extraGuy -= 10000;

                foreach (Asset a in playerAssets.ToList())
                {
                    if (a is UserInterface ui)
                        ui.Lives = _lives;
                }
            }
            //increasing the difficulty over time
            if(_tickCount == 50)
            {
                ++_difficulty;
                _tickCount = 0;
            }
            
            //creating a double buffer
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                //creating the graphics display
                using (BufferedGraphics bg = bgc.Allocate(CreateGraphics(), ClientRectangle))
                {
                    //applying some of that crips looking antialiasing to the shapes
                    bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    if (gameRunning)
                    {
                        //render metho that acts as the buffer
                        Render(bg.Graphics);
                    }
                    else
                        WaitScreen(bg.Graphics);

                    //swaping the the render to the main buffer
                    bg.Render();
                }
            }
            //incrementing the tick count every tick
            ++_tickCount;
        }
        /// <summary>
        /// rendering the waiting screen that displays the asteroid logo
        /// </summary>
        /// <param name="gr"></param>
        public void WaitScreen(Graphics gr)
        {
            //clearing the buffer
            gr.Clear(Color.Black);

            WaitingScreen waiting = new WaitingScreen(new PointF(Size.Width / 2, Size.Height / 2));

            waiting.Tick(Size);
            waiting.Render(gr);
        }
        /// <summary>
        /// work horse method that controls all the rendering and collision calls
        /// </summary>
        /// <param name="gr"></param>
        public void Render(Graphics gr)
        {
            //clearing the buffer
            gr.Clear(Color.Black);

            //adding asteroids up until the current difficulty
            if (asteroids.Count < _difficulty)
                GenerateAsteroid(gr);

            //checking to see if a bullet hit an asteroid
            BulletAsteroidCollisions(gr);
            //checking to see if an asteroid hit the player
            PlayerAsteroidCollision(gr);

            //removing all assets that are marked for death
            asteroids.RemoveAll(x => x.IsMarkedForDeath);
            bullets.RemoveAll(x => x.IsMarkedForDeath);

            //applying transforms to each shape
            playerAssets.ForEach(x => x.Tick(this.Size));
            bullets.ForEach(x => x.Tick(this.Size));
            asteroids.ForEach(x => x.Tick(this.Size));
            //rendering all shapes
            playerAssets.ForEach(x => x.Render(gr));
            bullets.ForEach(x => x.Render(gr));
            asteroids.ForEach(x => x.Render(gr));
        }
        /// <summary>
        /// Method for checking if an asteroid hit the player
        /// </summary>
        /// <param name="gr"></param>
        public void PlayerAsteroidCollision(Graphics gr)
        {
            for (int i = 0; i < asteroids.Count; i++)
            {
                //creating a dummy asteroid for an isAlive check
                Asteroid temp = (Asteroid)asteroids[i];
                if( temp.isAlive && playerAssets[1].Dist(asteroids[i]))
                {
                    //doing a region based intersection check on the asteroid and the player
                    Region player = new Region(playerAssets[1].GetPath());

                    Region aster = new Region(asteroids[i].GetPath());

                    Region intersect = player.Clone();

                    intersect.Intersect(aster);
                    //filling the player as white if they get hit
                    gr.FillRegion(new SolidBrush(Color.White), player);
                    //if there is an intersection
                    if (!intersect.IsEmpty(gr))
                    {
                        --_lives;//removing a life
                        //displaying that to the user
                        foreach (Asset a in playerAssets.ToList())
                        {
                            if (a is UserInterface ui)
                                ui.Lives = _lives;
                        }
                        //sleeping the game to give a hitch that alerts they player they took a hit
                        System.Threading.Thread.Sleep(1000);
                        //removing the asteroid
                        asteroids[i].IsMarkedForDeath = true;

                        //ending the game if the life counter = zero
                        if(_lives == 0)
                        {
                            gameRunning = false;
                            gm._returnScore.Invoke(_score);
                            //GameMenuScreen();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// method for region checking a bullet vs an asteroid
        /// </summary>
        /// <param name="gr"></param>
        public void BulletAsteroidCollisions(Graphics gr)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < asteroids.Count; j++)
                {
                    //regoin checking each asteroid against each bullet
                    if (bullets[i].Dist(asteroids[j]))
                    {
                        Region bullet = new Region(bullets[i].GetPath());

                        Region aster = new Region(asteroids[j].GetPath());

                        Region intersect = bullet.Clone();

                        intersect.Intersect(aster);

                        //if they intercept marking both for removal
                        if (!intersect.IsEmpty(gr))
                        {
                            asteroids[j].IsMarkedForDeath = true;
                            bullets[i].IsMarkedForDeath = true;
                        }
                        //if there is an intersection and the asteroid is marked for death splitting the asteroid 
                        if(asteroids[j].IsMarkedForDeath && !intersect.IsEmpty(gr))
                            asteroidSplit((Asteroid)asteroids[j]);
                    }
                }
            }
        }
        /// <summary>
        /// method for calculating the asteroid that split
        /// </summary>
        /// <param name="a"></param>
        public void asteroidSplit(Asteroid a)
        {
            //if it was a large asteroid it splits into two medium asteroid scoring the player 100 points
            if (a.aSize == AsterSize.Large)
            {
                for (int i = 0; i < 2; i++)
                {
                    asteroids.Add(new Asteroid(a.location, AsterSize.Medium));
                }
                _score += 100;
                _extraGuy += 100;
            }
            //if it was a medium asteroid it splits into threes small asteroids scoring the player 200 points
            if (a.aSize == AsterSize.Medium)
            {
                for (int i = 0; i < 3; i++)
                {
                    asteroids.Add(new Asteroid(a.location, AsterSize.Small));
                }
                _score += 200;
                _extraGuy += 200;
            }
            //for everysmall asteroid hit thats 300 points
            if (a.aSize == AsterSize.Small)
            {
                _score += 300;
                _extraGuy += 300;
            }
            foreach (Asset asset in playerAssets)
            {
                if (asset is UserInterface ui)
                    ui.Score = _score;

            }
        }
        /// <summary>
        /// generating an asteroid that won't just immediate kill the player 
        /// </summary>
        /// <param name="gr"></param>
        public void GenerateAsteroid(Graphics gr)
        {
            //bounding rectangle that covers the area where the player is
            Rectangle bounding = new Rectangle(new Point(Size.Width / 2, Size.Height / 2), new Size(200, 200));
            //making a region around it
            Region safespawn = new Region(bounding);
            //creating an asteroid at random ponts
            Asteroid aster = new Asteroid(new PointF(rand.Next(0, Size.Width), rand.Next(0, Size.Height)), AsterSize.Large);  
            //creating an intersection dummy
            Region intersects = safespawn.Clone();
            //while it intersects creating a new asteroid
            while(!intersects.IsEmpty(gr))
            {
                aster = new Asteroid(new PointF(rand.Next(0, Size.Width), rand.Next(0, Size.Height)), AsterSize.Large);
                Region checker = new Region(aster.GetPath());

                intersects.Intersect(checker);
            }
            //adding the asteroid when it finds a match
            asteroids.Add(aster);

        }
        /// <summary>
        /// whenever the keys are released the ship stop moving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                foreach (Asset a in playerAssets)
                {
                    if (a is Player p)
                    {
                        p.RotationDelta = 0;
                    }

                }
            }
        }
        /// <summary>
        /// controlling the rotation of the ship via the left and right keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //gameRunning = true;


                if (bullets.Count < 8)
                    bullets.Add(new Bullet(new PointF(0, 0), (Player)playerAssets[1]));
                else
                    bullets.RemoveAt(0);


            }
            else if(e.KeyCode == Keys.Right)
            {
                foreach(Asset a in playerAssets)
                {
                    if (a is Player p)
                    {
                        p.RotationDelta = 10;
                    }
                    
                }
            }
            else if(e.KeyCode == Keys.Left)
            {
                foreach (Asset a in playerAssets)
                {
                    if(a is Player p)
                        p.RotationDelta = -10;
                }
            }

        }
    }
}
