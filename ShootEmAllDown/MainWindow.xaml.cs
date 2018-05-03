﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private static DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {

            InitializeComponent();
            DataContext = this;
            InitializeGame();

        }

        private void InitializeGame()
        {
            AddCountries();
            AddPlanets();
            Enemies = new ObservableCollection<Enemy>();
            RandomizedEnemies = new ObservableCollection<Enemy>();


            var random = new Random();

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
                    var jet = new JetFighter(lvl, country);
                    jet.MovingLeft = xDirection;
                    jet.MovingUp = yDirection;
                    Enemies.Add(jet);
                }
                else
                {
                    var planet = planets[random.Next(countries.Count() - 1)];
                    var ufo = new UFO(lvl, planet);
                    ufo.MovingLeft = !xDirection;
                    ufo.MovingUp = !yDirection;
                    Enemies.Add(ufo);
                }
                xDirection = !xDirection;
                yDirection = !yDirection;
            }
            RandomizedEnemies = UnsortEnemies(Enemies);

            foreach (var enemy in RandomizedEnemies)
            {
                //var rndY = random.Next((int)(PlayField.Height - enemy.Rectangle.Height));
                //var rndX = random.Next((int)(PlayField.Width - enemy.Rectangle.Width));

                Canvas.SetTop(enemy.Rectangle, PlayField.Height / 2);
                Canvas.SetLeft(enemy.Rectangle, PlayField.Width / 2);
                PlayField.Children.Add(enemy.Rectangle);
            }

            //Sets up the timer for moving objects
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += timer_Tick;
            timer.Start();
        }


        private void timer_Tick(object sender, object e)
        {
            MoveEnemies();
        }

        private void MoveEnemies()
        {
            var enemies = PlayField.Children.OfType<Rectangle>();
            foreach (var enemy in enemies)
            {
                //Set speed and direction of the movement
                var baseSpeed = 5;
                var speedX = baseSpeed;
                var speedY = baseSpeed;
                var collidedWithWall = false;
                var nme = RandomizedEnemies.Where(r => r.Rectangle.Uid == enemy.Uid).FirstOrDefault();

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

                //Bounce back if it hits the walls.
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
            var rnd = new Random();
            return rnd.Next(list.Count());
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

        private void PlayField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            status.Content = "Missed!";

            for (var i = 0; i < PlayField.Children.OfType<Rectangle>().Count(); i++)
            {
                var enemy = PlayField.Children[i] as Rectangle;
                var nme = RandomizedEnemies.Where(r => r.Rectangle.Uid == enemy.Uid).FirstOrDefault();
                var pos = Mouse.GetPosition(enemy);
                if ((pos.X <= enemy.Width && pos.X > 0) && (pos.Y <= enemy.Height && pos.Y > 0))
                {
                    UpdateEnemy(nme);
                    if (nme.Energy <= 0)
                    {
                        PlayField.Children.Remove(enemy);
                    }
                    if (PlayField.Children.OfType<Rectangle>().Count() == 0)
                    {
                        status.Content = "You win!";
                        timer.Stop();
                    }
                    else status.Content = nme.Energy.ToString();
                }
            }
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

        private void SetDirection(UIElement uIElement)
        {

        }
    }
}
