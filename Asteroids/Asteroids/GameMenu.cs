using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Asteroids
{
    public partial class GameMenu : Form
    {
        List<HighScore> saveData = new List<HighScore>();//list containing all the saved data from the binary file
        public delvoidbool _gameStart = null;//delegate for passing the game start signal
        public delvoidInt _returnScore = null;//delegate for retreiving the score from main
        public GameMenu()
        {
            InitializeComponent();
            //itializing the return score delegate
            _returnScore = new delvoidInt(Scoring);
            _BtnStart.Click += _BtnStart_Click;

            //Trying to read the binary file to the listview
            try
            {
                //opening the filestream and deserializing it to the list of high scores
                FileStream fs = new FileStream("Highscores.bin", FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                saveData = (List<HighScore>)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //sorting the list to descending order
            saveData.Sort((s1,s2)=> s2._score.CompareTo(s1._score));
            //displaying all the information to the user
            foreach (HighScore h in saveData)
            {
                ListViewItem lvi = new ListViewItem(h._score.ToString());
                lvi.SubItems.Add(h._name);
                LV_HighScores.Items.Add(lvi);
            }
        }
        /// <summary>
        /// call back method that does the scoring at the end of the game
        /// </summary>
        /// <param name="score"></param>
        private void Scoring(int score)
        {
            NameDialog dialog = new NameDialog();
            HighScore nhs = new HighScore();
            //Getting the name from the user via a modal dialog
            if(DialogResult.OK == dialog.ShowDialog())
            {
                //creating a new highscore
                nhs._name = dialog.GetName;
                nhs._score = score;
            }
            //adding the new entry 
            saveData.Add(nhs);
            //saveData.Sort();
            //trying to write the new list to the binary file
            try
            {
                FileStream fs = new FileStream("Highscores.bin", FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                //serializing the data
                bf.Serialize(fs, saveData);
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //sorting the data
            saveData.Sort((s1, s2) => s2._score.CompareTo(s1._score));
            //clearing the listview
            LV_HighScores.Items.Clear();
            //displaying the scores to the user
            foreach (HighScore h in saveData)
            {
                ListViewItem lvi = new ListViewItem(h._score.ToString());
                lvi.SubItems.Add(h._name);
                LV_HighScores.Items.Add(lvi);
            }

        }
        /// <summary>
        /// invoking the game start delegate to start the game when the start button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BtnStart_Click(object sender, EventArgs e)
        {
            _gameStart.Invoke(true);
        }

       
    }
}
