using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            for (var i = 0; i < 10; i++)
            {
                var lvl = random.Next(1, 5);
                if (i % 2 == 0)
                {
                    var country = countries[random.Next(countries.Count() - 1)];
                    Enemies.Add(new JetFighter(lvl, country));
                }
                else
                {
                    var planet = planets[random.Next(countries.Count() - 1)];
                    Enemies.Add(new UFO(lvl, planet));
                }
            }

            RandomizedEnemies = UnsortEnemies(Enemies);
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
    }
}
