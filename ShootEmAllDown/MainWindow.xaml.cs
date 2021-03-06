﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WMPLib;

namespace ShootEmAllDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> countries = new List<string>();
        private List<string> planets = new List<string>();

        public ObservableCollection<Enemy> Enemies { get; set; }
        public ObservableCollection<Enemy> RandomizedEnemies { get; set; }

        private Random random = new Random();

        private string FX_PATH_HIT = Environment.CurrentDirectory + new Uri(@"pack://siteoforigin:,,,/Resources/hit.wav").AbsolutePath;
        private string FX_PATH_SHOOT = Environment.CurrentDirectory + new Uri(@"pack://siteoforigin:,,,/Resources/shoot.wav").AbsolutePath;
        private string FX_PATH_EXPLOSION = Environment.CurrentDirectory + new Uri(@"pack://siteoforigin:,,,/Resources/explosion.wav").AbsolutePath;

        private WindowsMediaPlayer fx_hit;
        private WindowsMediaPlayer fx_shoot;
        private WindowsMediaPlayer fx_explosion;


        private bool inGame = false;
        private static DispatcherTimer timer = new DispatcherTimer();


        public MainWindow()
        {

            InitializeComponent();
            DataContext = this;

            RandomizedEnemies = new ObservableCollection<Enemy>();
            Enemies = new ObservableCollection<Enemy>();

            //Sets up the timer for moving objects
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += timer_Tick;

            //Set a custom the cursor for the play area.
            string path = Environment.CurrentDirectory + new Uri(@"pack://siteoforigin:,,,/Resources/crosshair.cur").AbsolutePath;
            PlayField.Cursor = new Cursor(path);
        }

        private void InitializeSound()
        {
            fx_hit = new WindowsMediaPlayer();
            fx_shoot = new WindowsMediaPlayer();
            fx_explosion = new WindowsMediaPlayer();
        }

        private void InitializeGame()
        {
            InitializeSound();
            AddCountries();
            AddPlanets();
            Enemies.Clear();
            RandomizedEnemies.Clear();
            // Create same amount of enemy types, ten in total.
            // TODO: Maybe randomize so that it's not always the same amount of each type.

            var xDirection = true;
            var yDirection = true;

            for (var i = 0; i < 10; i++)
            {
                var lvl = random.Next(1, 3);
                if (i % 2 == 0)
                {
                    var country = countries[random.Next(countries.Count() - 1)];
                    Enemy jet = new JetFighter(lvl, random.Next(1, 3), country);
                    jet.MovingLeft = xDirection;
                    jet.MovingUp = yDirection;
                    Enemies.Add(jet);
                }
                else
                {
                    var planet = planets[random.Next(planets.Count() - 1)];
                    var ufo = new UFO(lvl, random.Next(1, 4), planet)
                    {
                        MovingLeft = !xDirection,
                        MovingUp = !yDirection
                    };
                    Enemies.Add(ufo);
                }
                xDirection = !xDirection;
                yDirection = !yDirection;
            }

            RandomizedEnemies = UnsortEnemies(Enemies);
            PlayField.Children.Clear();
            foreach (var enemy in RandomizedEnemies)
            {
                //var rndY = random.Next((int)(PlayField.Height - enemy.Rectangle.Height));
                //var rndX = random.Next((int)(PlayField.Width - enemy.Rectangle.Width));

                Canvas.SetTop(enemy.Rectangle, random.Next((int)(PlayField.Height - enemy.Rectangle.Height)));
                Canvas.SetLeft(enemy.Rectangle, random.Next((int)(PlayField.Width - enemy.Rectangle.Width)));
                PlayField.Children.Add(enemy.Rectangle);
            }
            StartTiming();
        }

        private void timer_Tick(object sender, object e)
        {
            MoveEnemies();
            UpdateTime();
            UpdateStatistics();
        }

        private void MoveEnemies()
        {
            var enemies = PlayField.Children.OfType<Rectangle>();
            foreach (var enemy in enemies)
            {
                //Set speed and direction of the movement
                var collidedWithWall = false;
                var nme = RandomizedEnemies.Where(r => r.Rectangle.Uid == enemy.Uid).FirstOrDefault();

                var baseSpeed = nme.Speed;
                var speedX = baseSpeed;
                var speedY = baseSpeed;

                if (nme.MovingLeft)
                {
                    speedX = -baseSpeed;
                }
                if (nme.MovingUp)
                {
                    speedY = -baseSpeed;
                }

                //Get current position of the enemy
                var posX = Canvas.GetLeft(enemy);
                var posY = Canvas.GetTop(enemy);

                //Bounce back/turn around if it hits the walls.
                if (posX >= PlayField.Width - enemy.Width)
                {
                    speedX = -speedX;
                    nme.MovingLeft = true;
                    collidedWithWall = true;
                }
                else if (posX <= 0)
                {
                    nme.MovingLeft = false;
                    collidedWithWall = true;
                }
                if (posY >= PlayField.Height - enemy.Height)
                {
                    speedY = -speedY;
                    nme.MovingUp = true;
                    collidedWithWall = true;
                }
                else if (posY <= 0)
                {
                    nme.MovingUp = false;
                    collidedWithWall = true;
                }

                //Bounce if it hits one of the other enemies
                if (!collidedWithWall)
                {
                    var rect1 = new Rect() { Height = enemy.Height, Width = enemy.Width, Location = new Point(posX, posY) };
                    foreach (var rival in enemies)
                    {

                        var rivalX = Canvas.GetLeft(rival);
                        var rivalY = Canvas.GetTop(rival);
                        var rect2 = new Rect() { Height = rival.Height, Width = rival.Width, Location = new Point(rivalX, rivalY) };
                        var riv = RandomizedEnemies.Where(r => r.Rectangle.Uid == rival.Uid).FirstOrDefault();

                        if (rect1.IntersectsWith(rect2))
                        {
                            if (rect1.Left < rect2.Left)
                            {
                                speedX = baseSpeed;
                                riv.MovingLeft = true;
                                nme.MovingLeft = false;
                            }
                            if (rect1.Right > rect2.Right)
                            {
                                speedX = -baseSpeed;
                                riv.MovingLeft = false;
                                nme.MovingLeft = true;
                            }
                            if (rect1.Top > rect2.Top)
                            {
                                speedY = baseSpeed;
                                riv.MovingUp = true;
                                nme.MovingUp = false;
                            }
                            if (rect1.Bottom < rect2.Bottom)
                            {
                                speedY = -baseSpeed;
                                riv.MovingUp = false;
                                nme.MovingUp = true;
                            }
                        }
                    }
                }
                Canvas.SetLeft(enemy, posX + speedX);
                Canvas.SetTop(enemy, posY + speedY);


                //Resets if something unforseen happens and the enemy unintentionally leaves the play area.
                if (Math.Abs(posX) > 1.3 * PlayField.Width || Math.Abs(posY) > 1.3 * PlayField.Height)
                {
                    Canvas.SetLeft(enemy, random.Next((int)enemy.Width, (int)PlayField.Width - (int)enemy.Width));
                    Canvas.SetTop(enemy, random.Next((int)enemy.Height, (int)PlayField.Height - (int)enemy.Height));
                }
                SetNewBrushIfJet(nme);
            }
        }

        // Jet fighters can't fly backwards obviously, so this changes the imagesource dynamically.
        private void SetNewBrushIfJet(Enemy enemy)
        {
            if (enemy is JetFighter)
            {
                var jet = enemy as JetFighter;
                enemy.Rectangle.Fill = jet.RightBrush;

                if (jet.MovingLeft)
                {
                    enemy.Rectangle.Fill = jet.LeftBrush;
                }
            }
        }

        private void AddCountries()
        {
            countries.Add("Russia");
            countries.Add("Germany");
            countries.Add("Spain");
            countries.Add("USA");
            countries.Add("Japan");
            countries.Add("North Korea");
            countries.Add("China");
        }

        private void AddPlanets()
        {
            planets.Add("Mars");
            planets.Add("Jupiter");
            planets.Add("Mercury");
            planets.Add("Krypton");
            planets.Add("Venus");
            planets.Add("Saturn");
        }

        private int RandomIndexInList<T>(List<T> list)
        {
            return random.Next(list.Count());
        }

        private ObservableCollection<Enemy> UnsortEnemies(ObservableCollection<Enemy> enemies)
        {
            if (enemies.Count() != 0)
            {

                var index = RandomIndexInList(enemies.ToList());
                RandomizedEnemies.Add(enemies[index]);
                enemies.RemoveAt(index);

                UnsortEnemies(enemies);

            }
            return RandomizedEnemies;
        }


        // Need to keep track of hits and misses to calculate a final score.
        private int totalHits = 0;
        private int totalMisses = 0;
        private int todaysBest = 0;

        private void PlayField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (inGame)
            {
                fx_shoot.URL = FX_PATH_SHOOT;
                status.Content = "Missed!";
                totalMisses += 1;
                for (var i = 0; i < PlayField.Children.OfType<Rectangle>().Count(); i++)
                {
                    var enemy = PlayField.Children[i] as Rectangle;
                    var nme = RandomizedEnemies.Where(r => r.Rectangle.Uid == enemy.Uid).FirstOrDefault();
                    var pos = Mouse.GetPosition(enemy);
                    if ((pos.X <= enemy.Width && pos.X > 0) && (pos.Y <= enemy.Height && pos.Y > 0))
                    {
                        status.Content = "That's a hit!";
                        totalHits += 1;
                        totalMisses -= 1;
                        fx_hit.URL = FX_PATH_HIT;
                        UpdateEnemy(nme);
                        if (nme.Energy <= 0)
                        {
                            PlayField.Children.Remove(enemy);
                            status.Content = nme.BattleCry;
                            fx_explosion.URL = FX_PATH_EXPLOSION;
                            RandomizedEnemies.Remove(nme);
                        }
                        if (PlayField.Children.OfType<Rectangle>().Count() == 0)
                        {
                            inGame = false;
                            GameOver();
                        }
                    }
                    
                }
                
            }
        }

        private void UpdateStatistics()
        {
            total_hits.Content = totalHits;
            total_misses.Content = totalMisses;
        }
            

        private void UpdateEnemy(Enemy enemy, string weapon = "AA")
        {
            var energyLoss = 0.0;
            switch (weapon)
            {
                case "AA":
                    energyLoss = 0.5;
                    break;
            }
            enemy.Energy -= energyLoss;
        }

        //Keep track of how long it takes to eradicate the enemies

        private Stopwatch stopWatch = new Stopwatch();

        private TimeSpan fastest = new TimeSpan();
        private TimeSpan time = new TimeSpan();

        private void StartTiming()
        {
            timer.Start();
            stopWatch.Restart();
            inGame = true;
        }

        private void UpdateTime()
        {
            time = stopWatch.Elapsed;
            current.Content = time.Minutes + "m " + time.Seconds + "s " + time.Milliseconds + "ms";
        }


        //When the last enemy is down, the game is over.
        public void GameOver()
        {
            stopWatch.Stop();
            timer.Stop();
            status.Content = "Victory!";
            //if (time < fastest || fastest.Milliseconds == 0)
            //{
            //    fastest = time;
            //}
            CalculateTotalScore();
            btn_new_game.IsEnabled = true;
            
        }

        private void CalculateTotalScore()
        {
            var bonus = (float) totalHits / totalMisses;
            var totalScore = (int)(time.TotalMilliseconds * bonus);
            total_score.Content = totalScore;
            if(todaysBest < totalScore || todaysBest == 0)
            {
                todaysBest = totalScore;
                todays_best.Content = todaysBest;
            }
        }

        private void btn_new_game_Click(object sender, RoutedEventArgs e)
        {
            totalHits = 0;
            totalMisses = 0;
            UpdateStatistics();
            InitializeGame();
            
            btn_new_game.IsEnabled = false;
        }
    }
}
