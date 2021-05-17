using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParbolicMotionGame.Data;
using System.Collections.ObjectModel;

namespace ParbolicMotionGame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameBoardPage : ContentPage
    {
        readonly ParabolicDBManager parabolicDBManager = new ParabolicDBManager();
        readonly IList<ParabolicDB> parabolicDBs = new ObservableCollection<ParabolicDB>();
        public GameBoardPage()
        {
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
        async void OnRefreshTest(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var GameDBCollection = await parabolicDBManager.GetAll();
                foreach (ParabolicDB ItemDB in GameDBCollection)
                {
                    if (parabolicDBs.All(a => a.LevelPrabolic_class != ItemDB.ScoreParabolic_class))
                    {
                        parabolicDBs.Add(ItemDB);
                    }
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void Recoredfresh()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var GameDBCollection = await parabolicDBManager.GetAll();
                foreach(ParabolicDB ItemDB in GameDBCollection)
                {
                    if(parabolicDBs.All(a => a.LevelPrabolic_class != ItemDB.ScoreParabolic_class))
                    {
                        parabolicDBs.Add(ItemDB);
                    }
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}