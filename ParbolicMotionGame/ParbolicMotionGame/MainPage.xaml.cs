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

namespace ParbolicMotionGame
{
    public partial class MainPage : ContentPage
    {
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
            //new CanvasView1().PaintSurface_main(e.Surface, e.Info);
        }

        private void CanvasView_PaintSurface2(object sender, SKPaintSurfaceEventArgs e)
        {
            //CanvasView1 canvasView2 = (CanvasView1)BindingContext;
            //canvasView2.PaintSurface_sub(sender, e.Surface, e.Info);
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            CanvasView1 canvasView1 = (CanvasView1)BindingContext;
            canvasView1.BTN_game_continue(sender, e, gamecontinue_btn);

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
