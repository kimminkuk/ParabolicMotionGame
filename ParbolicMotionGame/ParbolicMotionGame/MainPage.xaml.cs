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
            BindingContext = new CanvasView1();
            CanvasView2.InvalidateSurface();
            CanvasView.InvalidateSurface();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.PaintSurface_main(sender, e.Surface, e.Info);
        }

        private void CanvasView_PaintSurface2(object sender, SKPaintSurfaceEventArgs e)
        {

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

        public void CanvasView2_Invalidate()
        {
            CanvasView2.InvalidateSurface();
        }

        async public void Button_Clicked(object sender, EventArgs e)
        {
            
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            bool answer = await DisplayAlert("Question?", "Would you save Record?", "Yes", "No");

            canvasView1.BTN_game_continue(sender, e, gamecontinue_btn, answer, ref parabolicDB);
            if(answer)
              await Navigation.PushModalAsync(new AddGameDBPage(parabolicDBManager, parabolicDBs, parabolicDB));
            CanvasView1_Invalidate();
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

        private void Button_Clicked_LevelUp(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.Debug_GameLevelUp(sender, e, gamecontinue_btn);

            CanvasView1_Invalidate();
        }


        private void Button_Clicked_LevelDown(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.Debug_GameLevelDown(sender, e, gamecontinue_btn);

            CanvasView1_Invalidate();
        }


        public void Button_View(bool onoff)
        {
            if (onoff)
                gamecontinue_btn.IsVisible = true;
            else
                gamecontinue_btn.IsVisible = false;
        }
    }
}
