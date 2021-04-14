using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

using ParbolicMotionGame;

namespace ParbolicMotionGame.ViewModels
{
    public class CanvasView1 : INotifyPropertyChanged
    {
        public float[] rect_point = new float[4 * 4];
        public bool[] rect_pn = new bool[4 * 4];
        SKRect[] rect = new SKRect[4 * 4];

        public float InitConstant_X = 0;
        public float InitConstant_Y = 0;
        public float default_Init_x = 0;
        public float default_Init_y = 0;
        public float default_radius = 0;
        public float default_radius_move_allow = 0;
        public float Init_x = 0;
        public float Init_y = 0;
        public float control_rcos = 0;
        public float control_rsin = 0;
        public float control_rcos_abs = 0;
        public float control_rsin_abs = 0;
        public bool drag_onoff = true;
        public bool game_over = false;
        public float dis = 0;
        public float dis_init_circle = 0;

        public int time_interval = 0;
        public bool timer_stop_PN = false;
        public bool parabolic_clear = false;
        public int parabolic_cnt = 0;
        public int parabolic_pre_cnt = 0;
        public bool wall_1_pn = false;
        public bool timer_stop_PN_BlockRotation = false;
        public int BlockRotationCnt = 0;

        //New
        const int LEVEL_MAX = 5;
        const int pos_y_Increase = 25;
        public bool[] wall_pn = new bool[LEVEL_MAX];
        public bool pos_y_ceiling_touch = false;
        public int pos_y_ceiling_touchIncrease = pos_y_Increase;
        public float xpos_mirror = 0; 

        //Binding Text
        int game_power = 0;
        int game_level = 0;
        int game_score = 0;
        int game_rad = 0;
        bool game_Btn_Visible = false;
        bool game_Btn_Enable = false;
        public int Game_power { get => game_power; set { game_power = value; NotifyPropertyChanged("Game_power"); } }
        public int Game_level { get => game_level; set { game_level = value; NotifyPropertyChanged("Game_level"); } }
        public int Game_score { get => game_score; set { game_score = value; NotifyPropertyChanged("Game_score"); } }
        public int Game_rad { get => game_rad; set { game_rad = value; NotifyPropertyChanged("Game_rad"); } }
        public bool Game_Btn_Visible { get => game_Btn_Visible; set { game_Btn_Visible = value; NotifyPropertyChanged("Game_Btn_Visible"); } }
        public bool Game_Btn_Enable { get => game_Btn_Enable; set { game_Btn_Enable = value; NotifyPropertyChanged("Game_Btn_Enable"); } }

        System.Threading.Timer timer_;

        public CanvasView1()
        {
            Game_level = 1;
            Game_power = 0;
            Game_score = 0;
            Game_rad = 0;
            Game_Btn_Visible = false;
            game_Btn_Enable = false;
        }

        public void PaintSurface_main(object sender, SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.

            if (game_over)
            {
                canvas.Clear();
                Gameover_textdraw(sender, surface, info);
                return;
            }
            else
            {
                canvas.Clear();

                /*
                 * Main Canvas Paint Collection
                 * Ball, Init Ball, Restrict Bound, Goal Block 
                 */
                SKPaint paint = new SKPaint();
                SKPaint paint_restrictcircle = new SKPaint();
                SKPaint paintInitBall = new SKPaint();
                SKPaint paint_GoalBlock = new SKPaint();
                Paint_Collection(paint, paint_restrictcircle, paintInitBall, paint_GoalBlock);

                //Switch Case.. Level 1,Level2...Level5
                /*
                 * Game Level 1,2,3,4,5
                 * Switch - case ?
                 * rect position cal : GameLevel 1
                 */
                GoalBlockRectPosition(Game_level, info, canvas, paint_GoalBlock);

                /*
                 * BALL: Start Position
                 */
                InitBallPostion(Game_level, info);

                //Init Restrict Circle Location Paint
                SKCanvas canvas_restrictcircle = surface.Canvas;
                SKCanvas canvas_initcircle = surface.Canvas;

                // 공 날리기전에는 무조건 초기 원 설정 범위 내부에 들어오게 한다.
                if (timer_stop_PN != true)
                {
                    canvas_restrictcircle.DrawCircle(InitConstant_X, InitConstant_Y, (float)(info.Width * 0.07), paint_restrictcircle);
                    canvas_initcircle.DrawCircle(Init_x, Init_y, (float)(info.Width * 0.01), paintInitBall);
                }

                //Init Rad Cal
                Init_Rad_cal();

                // 공 날리기전에는 무조건 초기 원 설정 범위 내부에 들어오게 한다.
                if (timer_stop_PN != true)
                {
                    canvas_initcircle.DrawCircle(Init_x, Init_y, (float)(info.Width * 0.01), paintInitBall);
                }

                //TEST ACTION
                float Vo_test = ((float)Math.Sqrt((Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2)))
                    / default_radius_move_allow) * 300; //500 -> 600 ->

                //Binding Text Collection
                Game_power = (int)Vo_test;
                Game_rad = (int)(control_rcos_abs * 100);

                //TEST PARABOLIC MOTION
                //int i_2 = 0;
                for (int i = parabolic_cnt - 1; i < parabolic_cnt; i++)
                {
                    if (i == -1) continue;
                    
                    float t = (float)i / 80;
                    float g = 5 * t;

                    float x = (float)(Vo_test * control_rcos_abs) * t + Init_x;
                    float y = ((float)(Vo_test * control_rsin_abs) * t - g * t * 4); //info.Height ( ex) 640 )
                    //float y_Upside = 0;
                    
                    y = Init_y - y;

                    //처음 천장 맞은 경우
                    //한번 실행하면서 순간 Y좌표를 저장한다.
                    if (!pos_y_ceiling_touch && y < 0)                    
                    {
                        pos_y_ceiling_touch = true;
                    }

                    //천장 맞으면 자유 낙하 운동 개념으로 떨어지게..
                    if (pos_y_ceiling_touch)
                    {
                        pos_y_ceiling_touchIncrease++;
                        float t_y = (float)pos_y_ceiling_touchIncrease / 30;
                        float g_y = 6 * t_y;
                        y = t_y * g_y * 5;
                    }

                    switch (Game_level)
                    {
                        case 1:
                            if (wall_pn[0] != true && x >= (float)0.6 * info.Width &&
                                x <= (float)0.62 * info.Width && y >= (float)0.2 * info.Height 
                                && y <= (float)0.8 * info.Height)
                            {
                                wall_pn[0] = true;
                            }
                            if (wall_pn[0])
                            {
                                x = (float)0.6 * info.Width + ((float)0.6 * info.Width - x);
                            }
                            break;
                        case 2:
                            if (wall_pn[1] != true && x >= (float)0.55 * info.Width &&
                                x <= (float)0.57 * info.Width && y >= (float)0.15 * info.Height
                                && y <= (float)0.7 * info.Height)
                            {
                                wall_pn[1] = true;
                            }
                            if (wall_pn[1])
                            {
                                x = (float)0.55 * info.Width + ((float)0.55 * info.Width - x);
                            }
                            break;
                        case 3:
                            if (wall_pn[2] != true && x >= (float)0.5 * info.Width &&
                                x <= (float)0.52* info.Width && y >= (float)0.15 * info.Height
                                && y <= (float)0.20 * info.Height)
                            {
                                wall_pn[2] = true;
                                xpos_mirror = (float)0.51;
                            }
                            else if (wall_pn[2] != true && x >= (float)0.52 * info.Width &&
                                x <= (float)0.54 * info.Width && y >= (float)0.20 * info.Height
                                && y <= (float)0.25 * info.Height)
                            {
                                wall_pn[2] = true;
                                xpos_mirror = (float)0.53;
                            }
                            else if (wall_pn[2] != true && x >= (float)0.54 * info.Width &&
                                x <= (float)0.56 * info.Width && y >= (float)0.25 * info.Height
                                && y <= (float)0.4 * info.Height)

                            {
                                wall_pn[2] = true;
                                xpos_mirror = (float)0.55;
                            }
                            else if (wall_pn[2] != true && x >= (float)0.58 * info.Width &&
                                x <= (float)0.60 * info.Width && y >= (float)0.4 * info.Height
                                && y <= (float)0.55 * info.Height)
                            {
                                wall_pn[2] = true;
                                xpos_mirror = (float)0.57;
                            }
                            else if (wall_pn[2] != true && x >= (float)0.60 * info.Width &&
                                x <= (float)0.62 * info.Width && y >= (float)0.55 * info.Height
                                && y <= (float)0.7 * info.Height)
                            {
                                wall_pn[2] = true;
                                xpos_mirror = (float)0.59;
                            }

                            if (wall_pn[2])
                            {
                                x = (float)xpos_mirror * info.Width + ((float)xpos_mirror * info.Width - x);
                            }
                            break;
                        
                        default:
                            break;
                    }

                    //GAME END
                    if (y > info.Height || x < 0 || x > info.Width)
                    {
                        //Game End Setting, Variable initialization
                        GameEndandInitSetting(sender, Game_level, surface, info);
                        break;
                    }

                    // End Point Switch On
                    // First 1~16 check
                    // 0  1  2  3  4
                    // |  |  |  |  |
                    // |  |  |  |  |
                    // |  |  |  |  |
                    // |  |  |  |  |
                    //yes-> Level Up, No-> Game End
                    if (x > (float)(info.Width * 0.75))
                    {
                        GoalBlockDamageCount(Game_level, x, y, info);
                    }
                    //공 위치 갱신 Draw
                    canvas.DrawCircle(x, y, (float)0.01 * info.Width, paint);
                }

                //canvasView1에서 canvasView2의 Surface 중복해서 쓰는거 될까??
                PaintSurface_sub(sender, surface, info);
                
            }
        }

        private void Paint_Collection(SKPaint paint, SKPaint paint_restrictcircle, SKPaint paintInitBall, SKPaint paint_GoalBlock)
        {
            //Ball Paint
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.Color = Color.Red.ToSKColor();
            paint.StrokeWidth = 5;

            //Init Ball restrict Location Paint
            paint_restrictcircle.IsAntialias = true;
            paint_restrictcircle.Style = SKPaintStyle.Stroke;
            paint_restrictcircle.Color = Color.Blue.ToSKColor();
            paint_restrictcircle.StrokeWidth = 2;

            //Init Ball Location Paint
            paintInitBall.IsAntialias = true;
            paintInitBall.Style = SKPaintStyle.Fill;
            paintInitBall.Color = Color.Red.ToSKColor();
            paintInitBall.StrokeWidth = 5;

            //Goal Block Paint
            paint_GoalBlock.IsAntialias = true;
            paint_GoalBlock.Style = SKPaintStyle.Fill;
            paint_GoalBlock.Color = Color.Blue.ToSKColor();
        }

        private void GoalBlockDamageCount(int LEVEL, float x, float y, SKImageInfo info)
        {
            switch (LEVEL)
            {
                case 1:
                    for (int row = 0; row < 4; row++)
                    {
                        if (rect_pn[row] == true && rect_point[row] < x && rect_point[row + 1] > x)
                        {
                            for (int col = 0; col < 4; col++)
                            {
                                //col 0~1, 1~2, 2~3, 3~4
                                if (rect_pn[4 * col + row] == true && rect_point[col + 5] < y && rect_point[col + 6] > y)
                                {
                                    rect_pn[4 * col + row] = false;
                                    Game_score++;
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    for (int row = 1; row < 4; row++)
                    {
                        //row 1~2, 2~3, 3~4
                        if (rect_pn[row] == true && rect_point[row] < x && rect_point[row + 1] > x)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                //col 0~1, 1~2, 2~3
                                if (rect_pn[4 * col + row] == true && rect_point[col + 5] < y && rect_point[col + 6] > y)
                                {
                                    rect_pn[4 * col + row] = false;
                                    Game_score++;
                                }
                            }
                        }
                    }
                    break;

                case 3:
                    for (int row = 2; row < 4; row++)
                    {
                        //row 2~3, 3~4
                        if (rect_pn[row] == true && rect_point[row] < x && rect_point[row + 1] > x)
                        {
                            for (int col = 0; col < 2; col++)
                            {
                                //col 0~1, 1~2
                                if (rect_pn[4 * col + row] == true && rect_point[col + 5] < y && rect_point[col + 6] > y)
                                {
                                    rect_pn[4 * col + row] = false;
                                    Game_score++;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void PaintSurface_sub(object sender, SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
            //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.

            if (game_over)
            {
                //canvas.Clear();
                //Gameover_textdraw(sender, surface, info);
                return;
            }
            else
            {
                /*
                 * LEVEL 1~5 Wall Make
                 */
                DefenceWallMake(Game_level, canvas, info);

                /*
                 * Goal Block Destroy Effect
                 */
                SKCanvas canvas_GoalBlock = surface.Canvas;
                GoalBlockDestroyEffect(Game_level, canvas_GoalBlock, info, surface);
            }
        }

        private void GoalBlockDestroyEffect(int LEVEL, SKCanvas canvas, SKImageInfo info, SKSurface surface)
        {
            //  for (int i = 0; i < 5; i++)
            //  {
            //      rect_point[i] = (float)(info.Width * 0.75 + info.Width * (0.25 / 4) * i);
            //  }
            //  
            //  rect_point[5] = (float)(info.Height * 0.2);
            //  rect_point[6] = (float)(info.Height * 0.4);
            //  rect_point[7] = (float)(info.Height * 0.6);
            //  rect_point[8] = (float)(info.Height * 0.8);
            //  rect_point[9] = (float)(info.Height);
            //  
            //  for (int i = 0; i < 16; i++)
            //  {
            //      rect[i] = new SKRect(rect_point[i % 4], rect_point[i / 4 + 5],
            //          rect_point[(i % 4) + 1], rect_point[i / 4 + 6]);
            //  }
            // 
            SKPaint paintGoalBlock = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Blue.ToSKColor()
            };
            switch(LEVEL)
            {
                case 1:
                    for (int i = 0; i < 16; i++)
                    {
                        if (rect_pn[i])
                            canvas.DrawRect(rect[i], paintGoalBlock);
                        else //Rotation Block or Delete Block
                             //if (!rect_pn[i]) //Goal Block Destroy case
                        {
                            //Thread?
                            //StartTimer_BlockRotation();
                            timer_stop_PN_BlockRotation = true;

                            float rotation_x = (rect_point[i % 4] + rect_point[(i % 4) + 1]) / 2;
                            float rotation_y = (rect_point[i / 4 + 5] + rect_point[i / 4 + 6]) / 2;
                            OnDrawRoation(LEVEL, rotation_x, rotation_y, info, surface);
                        }
                    }
                    break;
                case 2:
                    for (int i = 1; i < 4; i++)
                    {
                        for(int j = 0; j < 3; j++)
                        {
                            int position = i + j * 4;
                            if (rect_pn[position])
                                canvas.DrawRect(rect[position], paintGoalBlock);
                            else 
                            {
                                timer_stop_PN_BlockRotation = true;

                                float rotation_x = (rect_point[position % 4] + rect_point[(position % 4) + 1]) / 2;
                                float rotation_y = (rect_point[position / 4 + 5] + rect_point[position / 4 + 6]) / 2;
                                OnDrawRoation(LEVEL, rotation_x, rotation_y, info, surface);
                            }
                        }
                    }
                    break;
                case 3:
                    for (int i = 2; i < 4; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            int position = i + j * 4;
                            if (rect_pn[position])
                                canvas.DrawRect(rect[position], paintGoalBlock);
                            else
                            {
                                timer_stop_PN_BlockRotation = true;

                                float rotation_x = (rect_point[position % 4] + rect_point[(position % 4) + 1]) / 2;
                                float rotation_y = (rect_point[position / 4 + 5] + rect_point[position / 4 + 6]) / 2;
                                OnDrawRoation(LEVEL, rotation_x, rotation_y, info, surface);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnDrawRoation(int LEVEL, float rotation_x, float rotation_y, SKImageInfo info, SKSurface surface)
        {
            //Block Color Changed
            SKPaint painttest_false = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,

                Color = Color.Red.ToSKColor()
            };
            //Block Color Changed
            SKPaint painttest_false2 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,

                Color = Color.Green.ToSKColor()
            };
            //Block Color Changed
            SKPaint painttest_false3 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,

                Color = Color.Yellow.ToSKColor()
            };
            //Block Color Changed
            SKPaint painttest_false4 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,

                Color = Color.Purple.ToSKColor()
            };

            //LEVEL 1~5
            float _squareWidth = (float)(info.Width * 0.25) / 5;
            float Vo_Rotation = 300;
            float Rad_const = 0.7071f; // sqrt(2)/2;
            float t = BlockRotationCnt / 80.0f;
            float g = 5 * t;
            float[] x2 = new float[8];
            float[] x2_reverse = new float[8];
            float[] y2 = new float[4];
            for (int i = 0; i < 8; i++)
            {
                x2[i] = rotation_x - (Vo_Rotation * Rad_const) * t / (1.7f + 0.1f * i);
            }
            for (int i = 0; i < 8; i++)
            {
                x2_reverse[i] = rotation_x + (Vo_Rotation * Rad_const) * t / (1.7f + 0.1f * i);
            }
            float y = (Vo_Rotation * Rad_const) * t - g * t * 14;

            y2[0] = (Vo_Rotation * Rad_const) * t - g * t * 14f;
            y2[1] = (Vo_Rotation * Rad_const) * t - g * t * 12f;

            y2[2] = (Vo_Rotation * Rad_const) * t - g * t * 14.5f;
            y2[3] = (Vo_Rotation * Rad_const) * t - g * t * 13f;

            y = rotation_y - y;
            for (int i = 0; i < 4; i++)
                y2[i] = rotation_y - y2[i];

            SKCanvas[] canvas_divide = new SKCanvas[8];
            for (int i = 0; i < 8; i++)
            {
                canvas_divide[i] = surface.Canvas;
                canvas_divide[i].Save();
                SKPath path_divide = new SKPath();
                path_divide.MoveTo(x2[i], y2[i / 2]);
                path_divide.LineTo(x2[i] + _squareWidth / 4, y2[i / 2]);
                path_divide.LineTo(x2[i] + _squareWidth / 4, y2[i / 2] - _squareWidth / 4);
                path_divide.LineTo(x2[i], y2[i / 2] - _squareWidth / 4);
                path_divide.LineTo(x2[i], y2[i / 2]);

                if (i > 4)
                    canvas_divide[i].DrawPath(path_divide, painttest_false);
                else
                    canvas_divide[i].DrawPath(path_divide, painttest_false2);

                canvas_divide[i].Restore();
            }

            SKCanvas[] canvas_divide_reverse = new SKCanvas[8];
            for (int i = 0; i < 8; i++)
            {
                canvas_divide_reverse[i] = surface.Canvas;
                canvas_divide_reverse[i].Save();
                SKPath path_divide = new SKPath();
                path_divide.MoveTo(x2_reverse[i], y2[i / 2]);
                path_divide.LineTo(x2_reverse[i] + _squareWidth / 4, y2[i / 2]);
                path_divide.LineTo(x2_reverse[i] + _squareWidth / 4, y2[i / 2] - _squareWidth / 4);
                path_divide.LineTo(x2_reverse[i], y2[i / 2] - _squareWidth / 4);
                path_divide.LineTo(x2_reverse[i], y2[i / 2]);
                if (i > 4)
                    canvas_divide_reverse[i].DrawPath(path_divide, painttest_false3);
                else
                    canvas_divide_reverse[i].DrawPath(path_divide, painttest_false4);
                canvas_divide_reverse[i].Restore();
            }
        }

        private void DefenceWallMake(int LEVEL, SKCanvas canvas, SKImageInfo info)
        {
            //LEVEL 1 Example
            switch(LEVEL)
            {
                case 1:
                    SKPoint[] DefenceWallLevel_1 = new SKPoint[4];
                    DefenceWallLevel_1[0] = new SKPoint((float)0.6 * info.Width, (float)0.2 * info.Height);
                    DefenceWallLevel_1[1] = new SKPoint((float)0.61 * info.Width, (float)0.2 * info.Height);
                    DefenceWallLevel_1[2] = new SKPoint((float)0.6 * info.Width, (float)0.8 * info.Height);
                    DefenceWallLevel_1[3] = new SKPoint((float)0.61 * info.Width, (float)0.8 * info.Height);

                    SKPaint paintDefenceWallLevel_1 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Blue.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.015)
                    };

                    canvas.DrawLine(DefenceWallLevel_1[0], DefenceWallLevel_1[2], paintDefenceWallLevel_1);
                    break;
                case 2:
                    SKPoint[] DefenceWallLevel_2 = new SKPoint[4];
                    DefenceWallLevel_2[0] = new SKPoint((float)0.55 * info.Width, (float)0.15 * info.Height);
                    DefenceWallLevel_2[1] = new SKPoint((float)0.56 * info.Width, (float)0.15 * info.Height);
                    DefenceWallLevel_2[2] = new SKPoint((float)0.55 * info.Width, (float)0.7 * info.Height);
                    DefenceWallLevel_2[3] = new SKPoint((float)0.56 * info.Width, (float)0.7 * info.Height);

                    SKPaint paintDefenceWallLevel_2 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.DarkGreen.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.015)
                    };

                    canvas.DrawLine(DefenceWallLevel_2[0], DefenceWallLevel_2[2], paintDefenceWallLevel_2);
                    break;
                case 3:
                    SKPoint[] DefenceWallLevel_3 = new SKPoint[4];
                    DefenceWallLevel_3[0] = new SKPoint((float)0.5 * info.Width,  (float)0.15 * info.Height);
                    //DefenceWallLevel_3[1] = new SKPoint((float)0.55 * info.Width, (float)0.1 * info.Height);
                    //DefenceWallLevel_3[2] = new SKPoint((float)0.75 * info.Width, (float)0.7 * info.Height);
                    DefenceWallLevel_3[3] = new SKPoint((float)0.7 * info.Width,  (float)0.75 * info.Height);

                    SKPaint paintDefenceWallLevel_3 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Black.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.015)
                    };

                    canvas.DrawLine(DefenceWallLevel_3[0], DefenceWallLevel_3[3], paintDefenceWallLevel_3);
                    break;

                default:
                    break;
            }
           // SKPoint[] points2 = new SKPoint[4];
           // points2[0] = new SKPoint((float)0.6 * info.Width, (float)0.2 * info.Height);
           // points2[1] = new SKPoint((float)0.61 * info.Width, (float)0.2 * info.Height);
           // points2[2] = new SKPoint((float)0.6 * info.Width, (float)0.8 * info.Height);
           // points2[3] = new SKPoint((float)0.61 * info.Width, (float)0.8 * info.Height);
           //
           // SKPaint paintWall = new SKPaint
           // {
           //     Style = SKPaintStyle.Fill,
           //     Color = Color.Blue.ToSKColor(),
           //     StrokeWidth = (float)(info.Width * 0.015)
           // };
           //
           // canvas.DrawLine(points2[0], points2[2], paintWall);
        }

        public void TouchMotion(object sender, SKTouchEventArgs e)
        {
            if (!game_over)
            {
                drag_onoff = false;
                switch (e.ActionType)
                {
                    case SKTouchAction.Moved:
                        Init_x = e.Location.X;
                        Init_y = e.Location.Y;
                        if (e.InContact)
                        {
                            ((SKCanvasView)sender).InvalidateSurface();
                        }
                        break;
                    case SKTouchAction.Released:
                        timer_stop_PN = true;
                        timer_stop_PN = true;
                        parabolic_clear = false;
                        parabolic_cnt = 0;
                        parabolic_pre_cnt = 0;
                        for (int i = 0; i < LEVEL_MAX; i++)
                        {
                            wall_pn[i] = false;
                        }
                        break;
                }

                if (timer_stop_PN)
                {
                    StartTimer(sender); //Make SK Graphics
                }

                //copy git
                e.Handled = true;
            }
        }

        private void GoalBlockRectPosition(int LEVEL, SKImageInfo info, SKCanvas canvas, SKPaint paint_GoalBlock)
        {
            switch(LEVEL)
            {
                case 1:
                    //LEVEL1 Goal Point Block Location Setting
                    for (int i = 0; i < 5; i++)
                    {
                        rect_point[i] = (float)(info.Width * 0.75 + info.Width * (0.25 / 4) * i);
                    }
                    rect_point[5] = (float)(info.Height * 0.2);
                    rect_point[6] = (float)(info.Height * 0.4);
                    rect_point[7] = (float)(info.Height * 0.6);
                    rect_point[8] = (float)(info.Height * 0.8);
                    rect_point[9] = (float)(info.Height);
                    for (int i = 0; i < 16; i++)
                    {
                        rect[i] = new SKRect(rect_point[i % 4], rect_point[i / 4 + 5],
                            rect_point[(i % 4) + 1], rect_point[i / 4 + 6]);

                        // canvas.DrawRect(rect[i], paint_GoalBlock);
                    }
                    break;
                case 2:
                    // // //LEVEL2 Goal Point Block Location Setting
                    // for (int i = 1; i < 4; i++)
                    // {
                    //     for (int j = 0; j < 3; j++)
                    //     {
                    //         canvas.DrawRect(rect[i + j * 4], paint_GoalBlock);
                    //     }
                    // }
                    break;

                default:
                    break;
            }
        }
        private void InitBallPostion(int LEVEL, SKImageInfo info)
        {
            switch(LEVEL)
            {
                case 1:
                    //LEVEL1 Init Location Constant
                    InitConstant_X = (float)(info.Width * 0.075);
                    InitConstant_Y = (float)(info.Height * 0.7);
                    default_radius_move_allow = (float)(info.Width * 0.07);
                    default_Init_x = (float)(info.Width * 0.075);
                    default_Init_y = (float)(info.Height * 0.7);

                    //LEVEL1 Init Ball Position
                    if (drag_onoff)
                    {
                        Init_x = (float)(info.Width * 0.075);
                        Init_y = (float)(info.Height * 0.7);

                        //Init rect_pn
                        for (int i = 0; i < 16; i++) rect_pn[i] = true;
                    }
                    break;
                case 2:
                    //LEVEL2 Init Location Constant
                    InitConstant_X = (float)(info.Width * 0.075);
                    InitConstant_Y = (float)(info.Height * 0.5);
                    default_radius_move_allow = (float)(info.Width * 0.07);
                    default_Init_x = (float)(info.Width * 0.075);
                    default_Init_y = (float)(info.Height * 0.5);

                    //LEVEL2 Init Ball Position
                    if (drag_onoff)
                    {
                        Init_x = (float)(info.Width * 0.075);
                        Init_y = (float)(info.Height * 0.5);

                        //Init rect_pn
                        for (int i = 1; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                rect_pn[i + j * 4] = true;
                            }
                        }
                    }
                    break;
                case 3:
                    //LEVEL3 Init Location Constant
                    InitConstant_X = (float)(info.Width * 0.1);
                    InitConstant_Y = (float)(info.Height * 0.4);
                    default_radius_move_allow = (float)(info.Width * 0.07);
                    default_Init_x = (float)(info.Width * 0.1);
                    default_Init_y = (float)(info.Height * 0.4);

                    //LEVEL3 Init Ball Position
                    if (drag_onoff)
                    {
                        Init_x = (float)(info.Width * 0.1);
                        Init_y = (float)(info.Height * 0.4);

                        //Init rect_pn
                        for (int i = 2; i < 4; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                rect_pn[i + j * 4] = true;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        private void Init_Rad_cal()
        {
            dis = (float)(Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2));
            dis_init_circle = (float)Math.Sqrt(dis);

            control_rcos = (Init_x - default_Init_x) / dis_init_circle;
            control_rsin = (Init_y - default_Init_y) / dis_init_circle;
            control_rcos_abs = Math.Abs((Init_x - default_Init_x) / dis_init_circle); //각도 절대값 괜찮나?
            control_rsin_abs = Math.Abs((Init_y - default_Init_y) / dis_init_circle); //각도 절대값 괜찮나?
            Init_x = control_rcos * default_radius_move_allow + default_Init_x;
            Init_y = control_rsin * default_radius_move_allow + default_Init_y;
        }
        private void Init_Location_set_sub(int LEVEL, SKImageInfo info)
        {
            default_radius = (float)(info.Width * 0.01);
            default_radius_move_allow = (float)(info.Width * 0.07);
            default_Init_x = (float)(info.Width * 0.075);
            default_Init_y = (float)(info.Height * 0.7);
        }

        private void GameEndandInitSetting(object sender, int LEVEL, SKSurface surface, SKImageInfo info)
        {
            parabolic_cnt = 0;
            time_interval = 0;
            BlockRotationCnt = 0;
            game_over = true;
            pos_y_ceiling_touch = false;
            pos_y_ceiling_touchIncrease = pos_y_Increase;
            xpos_mirror = 0;
            if (timer_stop_PN)
            {
                timer_stop();
                timer_stop_PN = false;
                timer_stop_PN_BlockRotation = false;
                drag_onoff = true; //Initial x,y 
                Init_x = 0;
                Init_y = 0;

                //Init rect_pn
                switch(LEVEL)
                {
                    case 1:
                        for (int j = 0; j < 16; j++)
                        {
                            //rect_pn = false -> block destory
                            if (!rect_pn[j]) game_over = false;
                            rect_pn[j] = true;
                        }
                        break;
                    case 2:
                        for(int i = 1; i < 4; i++)
                        {
                            for(int j = 0; j < 3; j++)
                            {
                                if (!rect_pn[i + j * 4]) game_over = false;
                                rect_pn[i + j * 4] = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            Game_power = 0;
            Game_rad = 0;

            if (game_over)
            {
                Game_score = 0;
                Game_level = 1;
                Gameover_textdraw(sender, surface, info);
            }
            else
            {
                Game_level++;
                ((SKCanvasView)sender).InvalidateSurface();
            }

        }

        private void Gameover_textdraw(object sender, SKSurface surface, SKImageInfo info)
        {
            //SKImageInfo info = e.Info; //그리기 화면에 대한 정보 (너비, 높이 픽셀)
            //SKSurface surface = e.Surface; // 그리기 화면 자체
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.

            canvas.Save();
            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Red.ToSKColor(),
                FakeBoldText = true,
                StrokeWidth = 1
            };

            string game_over_text = "END";

            //Adjust TextSize property so text is 95% of screen width
            float textWidth = textPaint.MeasureText(game_over_text);
            textPaint.TextSize = 0.95f * info.Width * textPaint.TextSize / textWidth;

            //Find the text bounds
            SKRect textBounds = new SKRect();
            textPaint.MeasureText(game_over_text, ref textBounds);

            // Calculate offsets to center the text on the screen
            float xText = info.Width / 2 - textBounds.MidX;
            float yText = info.Height / 2 - textBounds.MidY;

            //And draw the text
            canvas.DrawText(game_over_text, xText, yText, textPaint);
            canvas.Restore();

            Game_Btn_Visible = true;
            Game_Btn_Enable = true;
        }

        public void BTN_game_continue(object sender, EventArgs e, Button gamecontinue_btn)
        {
            parabolic_cnt = 0;
            game_over = false;

            timer_stop_PN = false;
            drag_onoff = true; //Initial x,y 
            Init_x = 0;
            Init_y = 0;

            //Init rect_pn
            for (int j = 0; j < 16; j++)
            {
                rect_pn[j] = true;
            }

            Game_score = 0;
            Game_level = 1;
            Game_power = 0;
            Game_rad = 0;
            Game_Btn_Visible = false;
            Game_Btn_Enable = false;
        }
        private void StartTimer_BlockRotation()
        {
            if (timer_stop_PN)
            {
                timer_stop_PN_BlockRotation = true;
            }
        }

        public void StartTimer(object sender)
        {
            if (timer_stop_PN)
            {
                time_interval = 5;
                timer_start(MY_TIMER_TICK_OBJECT, sender, 0, time_interval);
            }
        }
        private void timer_start(TimerCallback callback, object sender, int start, int time_interval)
        {
            timer_ = new System.Threading.Timer(callback, sender, start, time_interval);
        }

        private async void MY_TIMER_TICK_OBJECT(object state_sender)
        {
            await Task.Run(() =>
            {
                
                parabolic_cnt++;
                ((SKCanvasView)state_sender).InvalidateSurface();
                
                if(timer_stop_PN_BlockRotation)
                {
                    BlockRotationCnt++;
                }
            });
        }
        private void timer_stop()
        {
            timer_.Dispose();
        }

        #region notifyproperty
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
