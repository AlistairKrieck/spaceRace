using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Media;

namespace spaceRace
{
    //Alistair Krieck
    //Space Race :)
    //2024 / 01 / 08
    //I dont know what else goes here :/
    public partial class spaceRace : Form
    {
        Bitmap image = new Bitmap(@"C:\Users\aliskrie701\Downloads\Comp Sci\spaceRace\spaceRace\Resources\kaboom.png");
        Bitmap rocketUp = new Bitmap(@"C:\Users\aliskrie701\Downloads\Comp Sci\spaceRace\spaceRace\Resources\rocketUp.png");
        Bitmap rocketDown = new Bitmap(@"C:\Users\aliskrie701\Downloads\Comp Sci\spaceRace\spaceRace\Resources\rocketDown.png");
        Bitmap rocketFlameUp = new Bitmap(@"C:\Users\aliskrie701\Downloads\Comp Sci\spaceRace\spaceRace\Resources\rocketFlameUp.png");
        Bitmap rocketFlameDown = new Bitmap(@"C:\Users\aliskrie701\Downloads\Comp Sci\spaceRace\spaceRace\Resources\rocketFlameDown.png");

        SoundPlayer boomSound = new SoundPlayer(Properties.Resources.explosionSFX);
        SoundPlayer pointSound = new SoundPlayer(Properties.Resources.pointSFX);

        Stopwatch p1DeathTimer = new Stopwatch();
        Stopwatch p2DeathTimer = new Stopwatch();

        Rectangle p1 = new Rectangle(100, 370, 20, 50);
        Rectangle p2 = new Rectangle(490, 370, 20, 50);
        int playerSpeed = 8;

        List<Rectangle> asteroidsL = new List<Rectangle>();
        List<int> ballSizeL = new List<int>();
        List<int> ballSpeedL = new List<int>();

        int ballMaxSpeed = 5;
        int ballMinSpeed = 1;

        List<Rectangle> asteroidsR = new List<Rectangle>();
        List<int> ballSizeR = new List<int>();
        List<int> ballSpeedR = new List<int>();

        int p1score = 0;
        int p2score = 0;
        int time = 500;

        bool sDown = false;
        bool wDown = false;
        bool upDown = false;
        bool downDown = false;

        string p1PrevPos = "up";
        string p2PrevPos = "up";

        bool p1Dead = false;
        bool p2Dead = false;

        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Pen whitePen = new Pen(Color.White);
        Font winFont = new Font("Bauer Badoni", 50, FontStyle.Bold);
        Font failFont = new Font("Bauer Badoni", 25, FontStyle.Bold);

        Random randGen = new Random();
        int randValue = 0;

        string gameState = "waiting";

        public spaceRace()
        {
            InitializeComponent();
        }


        public void GameInit()
        {
            gameTimer.Enabled = true;

            titleLabel.Text = null;
            subtitleLabel.Text = null;

            p1.X = 100;
            p1.Y = this.Height - 100;

            p2.X = this.Width - 100;
            p2.Y = this.Height - 100;

            p1score = 0;
            p2score = 0;
            time = 1000;

            asteroidsL.Clear();
            asteroidsR.Clear();

            gameState = "running";
        }


        private void spaceRace_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;

                case Keys.S:
                    sDown = false;
                    break;

                case Keys.Up:
                    upDown = false;
                    break;

                case Keys.Down:
                    downDown = false;
                    break;

            }
        }

        private void spaceRace_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    p1PrevPos = "up";
                    break;

                case Keys.S:
                    sDown = true;
                    p1PrevPos = "down";
                    break;

                case Keys.Up:
                    upDown = true;
                    p2PrevPos = "up";
                    break;

                case Keys.Down:
                    downDown = true;
                    p2PrevPos = "down";
                    break;

                case Keys.Space:
                    if (gameState == "waiting")
                    {
                        GameInit();
                    }
                    break;

                case Keys.Escape:
                    if (gameState == "waiting")
                    {
                        Application.Exit();
                    }
                    break;

                case Keys.Enter:
                    if (gameState == "gameOver")
                    {
                        gameState = "waiting";
                    }
                    break;
            }
        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move hero
            if (p1Dead == false)
            {
                if (wDown == true && p1.Y > 0)
                {
                    p1.Y -= playerSpeed;
                }


                if (sDown == true && p1.Y < this.Height - 2 * p2.Height)
                {
                    p1.Y += playerSpeed;
                }
            }

            if (p2Dead == false)
            {
                if (upDown == true && p2.Y > 0)
                {
                    p2.Y -= playerSpeed;
                }

                if (downDown == true && p2.Y < this.Height - 2 * p2.Height)
                {
                    p2.Y += playerSpeed;
                }
            }

            //is it time to make a new ball
            randValue = randGen.Next(0, 100);
            if (randValue < 5) //5% chance of balls
            {
                ballSizeL.Add(randGen.Next(5, 10));
                Rectangle newBall = new Rectangle(-5, randGen.Next(0, this.Height - 2 * p1.Height - 30), ballSizeL[asteroidsL.Count], ballSizeL[asteroidsL.Count]);
                asteroidsL.Add(newBall);
                ballSpeedL.Add(randGen.Next(ballMinSpeed, ballMaxSpeed));
            }

            else if (randValue < 10)
            {
                ballSizeR.Add(randGen.Next(5, 10));
                Rectangle newBall = new Rectangle(this.Width + 5, randGen.Next(0, this.Height - 2 * p1.Height - 30), ballSizeR[asteroidsR.Count],
                    ballSizeR[asteroidsR.Count]);

                asteroidsR.Add(newBall);
                ballSpeedR.Add(randGen.Next(ballMinSpeed, ballMaxSpeed));
            }

            //move left balls
            for (int i = 0; i < asteroidsL.Count(); i++)
            {
                //get new position of x based on speed
                int x = asteroidsL[i].X + ballSpeedL[i];

                //replace the rect in the list w/ updated version
                asteroidsL[i] = new Rectangle(x, asteroidsL[i].Y, ballSizeL[i], ballSizeL[i]);

                //remove ball if too far
                if (asteroidsL[i].X > this.Width / 2 - 1)
                {
                    asteroidsL.RemoveAt(i);
                    ballSpeedL.RemoveAt(i);
                    ballSizeL.RemoveAt(i);
                }
            }

            //move right balls
            for (int i = 0; i < asteroidsR.Count(); i++)
            {
                //get new position of x based on speed
                int x = asteroidsR[i].X - ballSpeedR[i];

                //replace the rect in the list w/ updated version
                asteroidsR[i] = new Rectangle(x, asteroidsR[i].Y, ballSizeR[i], ballSizeR[i]);

                //remove ball if too far
                if (asteroidsR[i].X < this.Width / 2 + 1)
                {
                    asteroidsR.RemoveAt(i);
                    ballSpeedR.RemoveAt(i);
                    ballSizeR.RemoveAt(i);
                }

            }

            for (int i = 0; i < asteroidsL.Count(); i++)
            {
                //check for collision with player 1
                if (asteroidsL[i].IntersectsWith(p1))
                {
                    boomSound.Play();
                    asteroidsL.RemoveAt(i);
                    p1Dead = true;
                }
            }

            for (int i = 0; i < asteroidsR.Count(); i++)
            {
                //check for collision with player 2
                if (asteroidsR[i].IntersectsWith(p2))
                {
                    boomSound.Play();
                    asteroidsR.RemoveAt(i);
                    p2Dead = true;
                }
            }

            //check if player makes it to the top
            if (p1.Y < 10)
            {
                pointSound.Play();
                p1.Y = this.Height - 2 * p1.Height;
                p1score++;
            }

            if (p2.Y < 10)
            {
                pointSound.Play();
                p2.Y = this.Height - 2 * p1.Height;
                p2score++;
            }


            time--;

            Refresh();
        }

        private void spaceRace_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (gameState == "waiting")
            {
                p1ScoreLabel.Text = null;
                p2ScoreLabel.Text = null;
                titleLabel.Text = "Space Race";
                subtitleLabel.Text = "Press space to start, or esc to exit.";
            }

            else if (gameState == "running")
            {
                //update labels
                p1ScoreLabel.Text = p1score.ToString();
                p2ScoreLabel.Text = p2score.ToString();
                subtitleLabel.Text = time.ToString();

                //draw "arena"
                e.Graphics.DrawRectangle(whitePen, this.Width / 2 + 1, 0, this.Width / 2 - 1, this.Height);

                //draw p1
                if (p1PrevPos == "up" && wDown == false)
                {
                    g.DrawImage(rocketUp, new Rectangle(p1.X, p1.Y, p1.Width, p1.Height));
                }

                else if (p1PrevPos == "up" && wDown == true)
                {
                    g.DrawImage(rocketFlameUp, new Rectangle(p1.X, p1.Y, p1.Width, p1.Height));
                }

                else if (p1PrevPos == "down" && sDown == false)
                {
                    g.DrawImage(rocketDown, new Rectangle(p1.X, p1.Y, p1.Width, p1.Height));
                }

                else if (p1PrevPos == "down" && sDown == true)
                {
                    g.DrawImage(rocketFlameDown, new Rectangle(p1.X, p1.Y, p1.Width, p1.Height));
                }

                //draw p2
                if (p2PrevPos == "up" && upDown == false)
                {
                    g.DrawImage(rocketUp, new Rectangle(p2.X, p2.Y, p2.Width, p2.Height));
                }

                else if (p2PrevPos == "up" && upDown == true)
                {
                    g.DrawImage(rocketFlameUp, new Rectangle(p2.X, p2.Y, p2.Width, p2.Height));
                }

                else if (p2PrevPos == "down" && downDown == false)
                {
                    g.DrawImage(rocketDown, new Rectangle(p2.X, p2.Y, p2.Width, p2.Height));
                }

                else if (p2PrevPos == "down" && downDown == true)
                {
                    g.DrawImage(rocketFlameDown, new Rectangle(p2.X, p2.Y, p2.Width, p2.Height));
                }

                //draw asteroids
                for (int i = 0; i < asteroidsL.Count(); i++)
                {
                    e.Graphics.FillEllipse(whiteBrush, asteroidsL[i]);
                }

                for (int i = 0; i < asteroidsR.Count(); i++)
                {
                    e.Graphics.FillEllipse(whiteBrush, asteroidsR[i]);
                }

                if (p1Dead == true)
                {
                    p1DeathTimer.Start();
                    g.DrawImage(image, new Rectangle(p1.X - 50, p1.Y - 50, 100, 100));
                    if (p1DeathTimer.ElapsedMilliseconds > 300)
                    {
                        p1Dead = false;
                        p1.Y = this.Height - 2 * p1.Height;
                        p1DeathTimer.Stop();
                        p1DeathTimer.Reset();
                    }
                }

                if (p2Dead == true)
                {
                    p2DeathTimer.Start();
                    g.DrawImage(image, new Rectangle(p2.X - 50, p2.Y - 50, 100, 100));
                    if (p2DeathTimer.ElapsedMilliseconds > 300)
                    {
                        p2Dead = false;
                        p2.Y = this.Height - 2 * p2.Height;
                        p2DeathTimer.Stop();
                        p2DeathTimer.Reset();
                    }
                }

                //check if game is over
                if (p1score == 3 || p2score == 3 || time <= 0)
                {
                    gameState = "gameOver";
                }

                //check for winners
                if (gameState == "gameOver")
                {
                    if (p1score > p2score)
                    {
                        p2ScoreLabel.Text = null;
                        p1ScoreLabel.Text = null;
                        titleLabel.Text = "Player 1 Wins!";
                        subtitleLabel.Text = "Press Enter to Return to Title Screen";
                    }

                    else if (p1score < p2score)
                    {
                        p2ScoreLabel.Text = null;
                        p1ScoreLabel.Text = null;
                        titleLabel.Text = "Player 2 Wins!";
                        subtitleLabel.Text = "Press Enter to Return to Title Screen";
                    }

                    else
                    {
                        p2ScoreLabel.Text = null;
                        p1ScoreLabel.Text = null;
                        titleLabel.Text = "Draw!";
                        subtitleLabel.Text = "Press Enter to Return to Title Screen";
                    }
                }
            }
        }
    }
}
