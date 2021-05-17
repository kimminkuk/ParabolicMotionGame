using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using ParbolicMotionGame.Data;

namespace ParbolicMotionGame
{
    public class AddGameDBPage : ContentPage
    {
        readonly ParabolicDBManager parabolicDBManager;
        readonly IList<ParabolicDB> parabolicDBs;
        readonly ParabolicDB existingParabolicDB;
        readonly EntryCell NameCell, LevelCell, ScoreCell;
        public AddGameDBPage(ParabolicDBManager manager, IList<ParabolicDB> parabolicDBs, ParabolicDB existingPara)
        {
            this.parabolicDBManager = manager;
            this.parabolicDBs = parabolicDBs;
            this.existingParabolicDB = existingPara;
            var tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot(existingPara != null ? "Edit Test" : "New GameDB")
                {
                    new TableSection("Record Details")
                    {
                        (NameCell = new EntryCell {
                            Label = "Name",
                            Placeholder = "add Name",
                            Text = existingPara.NameParabolic_class,
                        }),
                        (LevelCell = new EntryCell {
                            Label = "Level",
                            Placeholder = existingPara.LevelPrabolic_class.ToString(),
                            IsEnabled = false,
                            Text = existingPara.LevelPrabolic_class.ToString(),
                        }),
                        (ScoreCell = new EntryCell {
                            Label = "Score",
                            Placeholder = existingPara.ScoreParabolic_class.ToString(),
                            IsEnabled = false,
                            Text = existingPara.ScoreParabolic_class.ToString(),
                        }),
                    },
                }
            };

            Button button = new Button()
            {
                BackgroundColor = Color.Green,
                TextColor = Color.White,
                Text =  "Add Your Records",
                CornerRadius = 0,
            };
            button.Clicked += OnDismiss;

            Content = new StackLayout
            {
                Spacing = 0,
                Children = { tableView, button },
            };
        }
        async void OnDismiss(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            try
            {
                string Name_ = NameCell.Text;
                string Level_string = LevelCell.Text;
                string Score_string = ScoreCell.Text;

                int Level_ = Int32.Parse(Level_string);
                int Score_ = Int32.Parse(Score_string);

                //Update: DB Add (Post)
                var test = await parabolicDBManager.Add(Level_, Score_, Name_);

                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error",
                    ex.Message,
                    "OK");
            }
            finally
            {
                IsBusy = false;
                button.IsEnabled = true;
            }
        }
    }
}