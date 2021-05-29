using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Threading;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Shapes;

using ParbolicMotionGame.ViewModels;
using ParbolicMotionGame.Data;
using ParbolicMotionGame.RankingPage;
using System.Collections.ObjectModel;

namespace ParbolicMotionGame
{
    public partial class MainPage : ContentPage
    {
        readonly ParabolicDBManager parabolicDBManager = new ParabolicDBManager();
        IList<ParabolicDB> parabolicDBs = new ObservableCollection<ParabolicDB>();
        ParabolicDB parabolicDB = new ParabolicDB();

        public MainPage()
        {
            InitializeComponent();
            CanvasView3.InvalidateSurface();
            CanvasView.InvalidateSurface();
            BindingContext = new CanvasView1();
            //CanvasView2.InvalidateSurface();
            //CanvasView.InvalidateSurface();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            //canvasView1.PaintSurface_main(sender, e.Surface, e.Info);
            canvasView1.PaintSurface_main_Ver2(sender, e.Surface, e.Info);
        }

        private void CanvasView_PaintSurface3(object sender, SKPaintSurfaceEventArgs e)
        {
            //CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            //canvasView1.PaintSurface_StartTitle(sender, e.Surface, e.Info);
        }

        private void CanvasView_PaintSurface2(object sender, SKPaintSurfaceEventArgs e)
        {

        }

        //Game Start Title
        private void CanvasView_Touch_StartEvent(object sender, SKTouchEventArgs e)
        {
            //CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            //
            //canvasView1.TouchTitle(sender, e);
        }

        //Game Start
        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            
            canvasView1.TouchMotion(sender, e);
        }

        public void CanvasView1_Invalidate()
        {
            CanvasView.InvalidateSurface();
        }

        public void CanvasView3_Invalidate()
        {
            CanvasView3.InvalidateSurface();
        }

        async private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            bool answer = await DisplayAlert("Question?", "Would you save Record?", "Yes", "No");
            canvasView1.OnTapGestureRecognizerTapped_GameReset(sender, e, ref parabolicDB);

            if (answer)
                await Navigation.PushModalAsync(new AddGameDBPage(parabolicDBManager, parabolicDBs, parabolicDB));
            CanvasView1_Invalidate();
        }

        async private void OnTapRankingBoardPage(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            //canvasView1.timer_stop();
            await Navigation.PushModalAsync(new GameRankingPage(sender , e));
            canvasView1.EndWaitTime(1000);
            //canvasView1.StartTimer(sender);
        }

        //For Debug
        private void Button_Clicked_LevelUp(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.Debug_GameLevelUp(sender, e);

            CanvasView1_Invalidate();
        }

        //For Debug
        private void Button_Clicked_LevelDown(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.Debug_GameLevelDown(sender, e);

            CanvasView1_Invalidate();
        }


        private void Button_Clicked_StartTitle(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.Btn_StartTitle(sender, e);

            CanvasView1_Invalidate();
        }

    }
}
