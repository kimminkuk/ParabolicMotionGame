using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParbolicMotionGame.Data;

using System.Collections.ObjectModel;
namespace ParbolicMotionGame.RankingPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameRankingPage : ContentPage
    {
        readonly ParabolicDBManager parabolicDBManager = new ParabolicDBManager();

        // ObservableCollection ?
        readonly ObservableCollection<ParabolicDB> parabolicDBs_react = new ObservableCollection<ParabolicDB>();
        readonly IList<ParabolicDB> parabolicDBs = new ObservableCollection<ParabolicDB>();
        public GameRankingPage(object sender, EventArgs e)
        {
            //BindingContext = parabolicDBs;
            BindingContext = parabolicDBs_react;
            InitializeComponent();

            //RankingView(sender , e);
            RankingView();
        }

        async void RankingView()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var GameDBCollection = await parabolicDBManager.GetAll();
                foreach (ParabolicDB ItemDB in GameDBCollection)
                {
                    if (parabolicDBs_react.All(a => a.LevelParabolic_class != ItemDB.ScoreParabolic_class))
                    {
                        parabolicDBs_react.Add(ItemDB);
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

        async void RankingView(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var GameDBCollection = await parabolicDBManager.GetAll();
                foreach (ParabolicDB ItemDB in GameDBCollection)
                {
                    if (parabolicDBs.All(a => a.LevelParabolic_class != ItemDB.ScoreParabolic_class))
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

        private void ListView_ItemTapped(object sender, EventArgs e)
        {

        }

        async private void RankingUpdate_Clicked(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var GameDBCollection = await parabolicDBManager.GetAll();
                foreach (ParabolicDB ItemDB in GameDBCollection)
                {
                    if (parabolicDBs.All(a => a.IdParabolic_class != ItemDB.IdParabolic_class))
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
                    if (parabolicDBs.All(a => a.IdParabolic_class != ItemDB.IdParabolic_class))
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