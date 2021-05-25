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
using System.Data.SqlClient;
using ParbolicMotionGame.Data;
using System.Collections.ObjectModel;

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

        public int time_interval = 5;
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
        public bool[] wall_pn_sub = new bool[LEVEL_MAX];
        public bool pos_y_ceiling_touch = false;
        public int pos_y_ceiling_touchIncrease = pos_y_Increase;
        public float xpos_mirror = 0;
        public int[] wall_pn_Level4_main = new int[2];
        public int[] wall_pn_Level4_sub = new int[2];
        public bool TouchOnOff = true;
        public float[] LEVEL_4_Wall_y_pos_main = new float[2] { 0.1f, 0.8f };
        public float[] LEVEL_4_Wall_y_pos_sub = new float[2] { 0.15f, 0.9f };
        public float[] x_rendering = new float[10000];
        public float[] y_rendering = new float[10000];
        int cnt_rendering = 0;
        int cnt_gameplayrender = 0;
        bool enable_rendering = false;
        float Vo_test = 0f;
        int Vo_test_Power = 1000;

        /*
         *  Update : Wall Motion Rendering
         */
        int cnt_defencewallrendering_1 = 0;
        int cnt_defencewallrendering_2 = 0;
        int cnt_defencewallrendering_3 = 0;
        int cnt_defencewallrendering_4 = 0;
        int cnt_defencewallrendering_5 = 0;
        float[] defencewall_1_x = new float[2] { 0.6f, 0.61f };
        float[] defencewall_1_y = new float[2] { 0.2f, 0.8f };
        float[] defencewall_2_x = new float[2] { 0.55f, 0.56f };
        float[] defencewall_2_y = new float[2] { 0.15f, 0.7f };
        float[] defencewall_3_x = new float[2] { 0.5f, 0.7f };
        float[] defencewall_3_y = new float[2] { 0.15f, 0.75f };
        float[] defencewall_4_x_1 = new float[2] { 0.6f, 0.61f };
        float[] defencewall_4_y_1 = new float[2] { 0.1f, 0.55f };
        float[] defencewall_4_x_2 = new float[2] { 0.75f, 0.76f };
        float[] defencewall_4_y_2 = new float[2] { 0.15f, 0.6f };
        float[] defencewall_5_x_1 = new float[2] { 0.75f, 0.76f };
        float[] defencewall_5_x_2 = new float[2] { 0.75f, 0.76f };
        float[] defencewall_5_y_1 = new float[2] { 0.15f, 0.4f };
        float[] defencewall_5_y_2 = new float[2] { 0.6f, 0.8f };

        float ballsize = 0.01f;
        bool wallupdown = true;
        
        /*
         *  SKPaint : Paint List 
         */
        SKPaint paint = new SKPaint();
        SKPaint paint_restrictcircle = new SKPaint();
        SKPaint paintInitBall = new SKPaint();
        SKPaint paint_GoalBlock = new SKPaint();
        SKPaint paint_PredictArrow = new SKPaint();

        /*
         *  Game State
         */
        int GameState = 0;

        /*
         *  Binding Text
         */
        int game_power = 0;
        int game_level = 0;
        int game_score = 0;
        int game_rad = 0;
        bool game_Btn_Visible = false;
        bool game_Btn_Enable = false;
        bool game_touch_enable = false;
        bool game_start_touch_enable = false;
        bool gameDebugLevelUp_Visble = true; //Debug -> true, default : false
        bool gameDebugLevelDown_Visble = true;
        bool gameDebugLevelUp_Enable = true;
        bool gameDebugLevelDown_Enable = true;
        bool gameStartTitle_Visible = false;
        bool gameStartTitle_Enable = false;
        bool GameInitOnOff = true;
        bool GameInitDeviceTimerOnOff = true;
        public int Game_power { get => game_power; set { game_power = value; NotifyPropertyChanged("Game_power"); } }
        public int Game_level { get => game_level; set { game_level = value; NotifyPropertyChanged("Game_level"); } }
        public int Game_score { get => game_score; set { game_score = value; NotifyPropertyChanged("Game_score"); } }
        public int Game_rad { get => game_rad; set { game_rad = value; NotifyPropertyChanged("Game_rad"); } }
        public bool Game_Btn_Visible { get => game_Btn_Visible; set { game_Btn_Visible = value; NotifyPropertyChanged("Game_Btn_Visible"); } }
        public bool Game_Btn_Enable { get => game_Btn_Enable; set { game_Btn_Enable = value; NotifyPropertyChanged("Game_Btn_Enable"); } }
        public bool Game_touch_enable { get => game_touch_enable; set { game_touch_enable = value; NotifyPropertyChanged("Game_touch_enable"); } }
        public bool Game_start_touch_enable { get => game_start_touch_enable; set { game_start_touch_enable = value; NotifyPropertyChanged("Game_start_touch_enable"); } }
        public bool GameDebugLevelUp_Enable { get => gameDebugLevelUp_Enable; set { gameDebugLevelUp_Enable = value; NotifyPropertyChanged("GameDebugLevelUp_Enable"); } }
        public bool GameDebugLevelUp_Visble { get => gameDebugLevelUp_Visble; set { gameDebugLevelUp_Visble = value; NotifyPropertyChanged("GameDebugLevelUp_Visble"); } }
        public bool GameDebugLevelDown_Enable { get => gameDebugLevelDown_Enable; set { gameDebugLevelDown_Enable = value; NotifyPropertyChanged("GameDebugLevelDown_Enable"); } }
        public bool GameDebugLevelDown_Visble { get => gameDebugLevelDown_Visble; set { gameDebugLevelDown_Visble = value; NotifyPropertyChanged("GameDebugLevelDown_Visble"); } }

        public bool GameStartTitle_Visible { get => gameStartTitle_Visible; set { gameStartTitle_Visible = value; NotifyPropertyChanged("GameStartTitle_Visible"); } }
        public bool GameStartTitle_Enable { get => gameStartTitle_Enable; set { gameStartTitle_Enable = value; NotifyPropertyChanged("GameStartTitle_Enable"); } }

        /*
         *  Enum 
         */
        public enum GameStartCondition { GameTitle, GameInit, GamePlay ,GameEnd, GameClear }
        
        /*
         *  Timer
         */
        System.Threading.Timer timer_;
        System.Threading.Timer timer_wall;

        public CanvasView1()
        {

            Game_level = 1;
            Game_power = 0;
            Game_score = 0;
            Game_rad = 0;
            Game_Btn_Visible = false;
            //game_Btn_Enable = false;
            game_Btn_Enable = false;
            game_touch_enable = true;
            //DeviceTimer_Wall();
            Paint_Collection(paint, paint_restrictcircle, paintInitBall, paint_GoalBlock, paint_PredictArrow);
        }

        public void PaintSurface_StartTitle(object sender, SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas;
            if (GameState == (int)GameStartCondition.GameTitle)
            {
                //SKCanvas canvas = surface.Canvas;
                SKPaint textPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.Red.ToSKColor(),
                    FakeBoldText = true,
                    StrokeWidth = 1
                };
                string game_Title_text = "START TOUCH!";
            
                //Adjust TextSize property so text is 95% of screen width
                float textWidth = textPaint.MeasureText(game_Title_text);
                textPaint.TextSize = 0.2f * info.Width * textPaint.TextSize / textWidth;
            
                //Find the text bounds
                SKRect textBounds = new SKRect();
                textPaint.MeasureText(game_Title_text, ref textBounds);
            
                // Calculate offsets to Low center the text on the screen
                float xText = info.Width / 2 - textBounds.MidX;
                float yText = info.Height * 0.8f;
            
                //And draw the text
                canvas.DrawText(game_Title_text, xText, yText, textPaint);
                EndWaitTime(1000);
            }
            return;
        }

        public void PaintSurface_main(object sender, SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.
            
            if (GameState == (int)GameStartCondition.GameEnd)
            {
                canvas.Clear();
                EndWaitTime(1000);
                Game_Btn_Visible = false;
                Game_Btn_Enable = false;
                Gameover_textdraw(sender, surface, info);
                return;
            }
            else if (GameState == (int)GameStartCondition.GameInit )
            {
                canvas.Clear();
                if(game_level == 6)
                {
                    GameClearText(sender, surface, info);
                    return;
                }
                GameLevelTextCanvas(Game_level, sender, surface, info);

                /*
                 * Game Level 1,2,3,4,5
                 * Switch - case ?
                 * rect position cal : GameLevel 1
                 */
                GoalBlockRectPosition(info, canvas, paint_GoalBlock);

                // /*
                //  *  LEVEL 1~5 Wall Make
                //  */
                // DefenceWallMake(Game_level, canvas, info);

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
                    canvas_initcircle.DrawCircle(Init_x, Init_y, info.Width * ballsize, paintInitBall);
                }

                //TEST ACTION
                float Vo_test = ((float)Math.Sqrt((Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2)))
                    / default_radius_move_allow) * 350; //300은 조금 약하고,, 400은 너무 쌔다..500 -> 600 ->

                //Binding Text Collection
                Game_power = (int)Vo_test;
                Game_rad = (int)(control_rcos_abs * 100);

                /*
                 *  Start Direction Prediction Arrow Graphics
                 */
                if (TouchOnOff)
                {
                    PredictArrowDirection(canvas, info, paint_PredictArrow);
                }
                else
                {
                   /*
                    * x,y Position Pre-Calculate
                    */
                    if (!enable_rendering)
                    {
                        //PredictPostionCalculate(canvas, info, Vo_test);
                    }
#if false
                if (!TouchOnOff)
                {
                    //Add : for skcanvas stop and timer stop only use for (i)
                    //timer_stop();
                    for (int i = 0; i < info.Width * 2; i++)
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
                            float t_y = (float)pos_y_ceiling_touchIncrease / 25;
                            float g_y = 6 * t_y;
                            y = t_y * g_y * 5;
                        }
                        /*
                         * Ball Touch DefenceWall True or False
                         */
                        DefenceWallTouchJudge(Game_level, x, y, info);

                        switch (Game_level)
                        {
                            case 1:
                                if (wall_pn[0])
                                {
                                    x = (float)0.6 * info.Width + ((float)0.6 * info.Width - x);
                                }
                                break;
                            case 2:
                                if (wall_pn[1])
                                {
                                    x = (float)0.55 * info.Width + ((float)0.55 * info.Width - x);
                                }
                                break;
                            case 3:
                                if (wall_pn[2])
                                {
                                    x = (float)xpos_mirror * info.Width + ((float)xpos_mirror * info.Width - x);
                                }
                                break;
                            case 4:
                                if (wall_pn[3])
                                {
                                    x = (float)0.6 * info.Width + ((float)0.6 * info.Width - x);
                                }
                                if (wall_pn_sub[3] != true && x >= (float)0.75 * info.Width &&
                                    x <= (float)0.755 * info.Width && y >= (float)0.15 * info.Height
                                    && y <= (float)0.6 * info.Height)
                                {
                                    wall_pn_sub[3] = true;
                                    wall_pn[3] = false;
                                }
                                if (wall_pn_sub[3])
                                {
                                    x = (float)0.75 * info.Width + ((float)0.75 * info.Width - x);

                                    if (wall_pn_Level4_main[1] != 1 && x >= (float)0.6 * info.Width && x <= (float)0.605 * info.Width
                                        && y >= (float)0.1 * info.Height && y <= (float)0.55 * info.Height)
                                    {
                                        wall_pn_Level4_main[1] = 1;
                                    }
                                }
                                //공이 A B 벽이 있을 경우,
                                //B벽을 맞고 A벽을 맞을때 공의 위치 계산
                                // A<-->B , A와B 벽사이의 거리 *2 만큼 x위치에서 빼준다.
                                if (wall_pn_Level4_main[1] == 1)
                                {
                                    x = (float)(Vo_test * control_rcos_abs) * t + Init_x - 0.3f * info.Width;
                                    wall_pn_sub[3] = false;

                                    if (x >= (float)0.75 * info.Width && x <= (float)0.755 * info.Width
                                        && y >= (float)0.15 * info.Height && y <= (float)0.6 * info.Height)
                                    {
                                        wall_pn_sub[3] = true;
                                        wall_pn[3] = false;
                                        wall_pn_Level4_main[1] = 0;
                                    }

                                }
                                break;
                            case 5:
                                if (wall_pn[4])
                                {
                                    x = (float)0.75 * info.Width + ((float)0.75 * info.Width - x);
                                }
                                break;
                            default:
                                break;
                        }

                        //GAME END
                        if (y > info.Height || x < 0 || x > info.Width)
                        {
                            //for (int j = 0; j < cnt_rendering; j += 2)
                            //{
                            //    canvas.DrawCircle(x_rendering[j], y_rendering[j], (float)0.01 * info.Width, paint);
                            //}

                            //Game End Setting, Variable initialization
                            enable_rendering = true;
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

                        //Add : for rendering
                        cnt_rendering = i;
                        x_rendering[i] = x;
                        y_rendering[i] = y;

                        // //공 위치 갱신 Draw
                        canvas.DrawCircle(x, y, (float)0.01 * info.Width, paint);
                    }
                }
#endif
                    if (cnt_rendering >= parabolic_cnt)
                    {
                        canvas.DrawCircle(x_rendering[parabolic_cnt], y_rendering[parabolic_cnt], (float)0.01 * info.Width, paint);
                        if (x_rendering[parabolic_cnt] > (float)(info.Width * 0.75))
                        {
                            GoalBlockDamageCount(Game_level, x_rendering[parabolic_cnt], y_rendering[parabolic_cnt], info);
                        }
                    }
                    //Force quit
                    else
                    {
                        //Game End Setting, Variable initialization
                        GameEndandInitSetting(sender, Game_level, surface, info);
                    }
#if false
                    for (int i = parabolic_cnt - 1; i < parabolic_cnt; i++)
                    {
                        if (i == -1) continue;
                        if (cnt_rendering >= i)
                        {
                            canvas.DrawCircle(x_rendering[i], y_rendering[i], (float)0.01 * info.Width, paint);
                        }
                        //Force quit
                        else
                        {
                            //Game End Setting, Variable initialization
                            GameEndandInitSetting(sender, Game_level, surface, info);
                            break;
                        }

                        //if (y_rendering[i] > info.Height || x_rendering[i] < 0 || x_rendering[i] > info.Width)
                        //{
                        //    //Game End Setting, Variable initialization
                        //    GameEndandInitSetting(sender, Game_level, surface, info);
                        //    break;
                        //}
                        if (x_rendering[i] > (float)(info.Width * 0.75))
                        {
                            GoalBlockDamageCount(Game_level, x_rendering[i], y_rendering[i], info);
                        }
                    }
#endif
                }
                //canvasView1에서 canvasView2의 Surface 중복해서 쓰는거 될까??
                PaintSurface_sub(sender, surface, info);
                
            }

            else if (GameState == (int)GameStartCondition.GamePlay)
            {

            }
            else
            {
                GameStartTitle_textdraw(sender, surface, info);
            }

            /*
             *  LEVEL 1~5 Wall Make
             */
            DefenceWallMake(Game_level, canvas, info);

            switch (GameState)
            {
                case (int)GameStartCondition.GameTitle:
                    {
                        GameStartTitle_textdraw(sender, surface, info);
                    }
                    break;

                case (int)GameStartCondition.GameInit:
                    {
                        GameLevelTextCanvas(Game_level, sender, surface, info);

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
                            canvas_initcircle.DrawCircle(Init_x, Init_y, info.Width * ballsize, paintInitBall);
                        }

                        //TEST ACTION
                        Vo_test = ((float)Math.Sqrt((Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2)))
                            / default_radius_move_allow) * 350; //300은 조금 약하고,, 400은 너무 쌔다..500 -> 600 ->

                        //Binding Text Collection
                        Game_power = (int)Vo_test;
                        Game_rad = (int)(control_rcos_abs * 100);

                        /*
                         *  Start Direction Prediction Arrow Graphics
                         */
                        PredictArrowDirection(canvas, info, paint_PredictArrow);

                        /*
                         * x,y Position Pre-Calculate
                         */
                        //PredictPostionCalculate(canvas, info, Vo_test);
                    }
                    break;

                case (int)GameStartCondition.GamePlay:
                    {
                        cnt_gameplayrender++;
                        GameLevelTextCanvas(Game_level, sender, surface, info);

                        /*
                         * Game Level 1,2,3,4,5
                         * Switch - case ?
                         * rect position cal : GameLevel 1
                         */
                        GoalBlockRectPosition(info, canvas, paint_GoalBlock);

                        if (cnt_rendering >= cnt_gameplayrender)
                        {
                            canvas.DrawCircle(x_rendering[parabolic_cnt], y_rendering[parabolic_cnt], (float)0.01 * info.Width, paint);
                            if (x_rendering[parabolic_cnt] > (float)(info.Width * 0.75))
                            {
                                GoalBlockDamageCount(Game_level, x_rendering[parabolic_cnt], y_rendering[parabolic_cnt], info);
                            }
                        }
                        //Force quit
                        else
                        {
                            //Game End Setting, Variable initialization
                            GameEndandInitSetting(sender, Game_level, surface, info);
                        }

                    }
                    break;

                case (int)GameStartCondition.GameEnd:
                    {
                        canvas.Clear();
                        EndWaitTime(1000);
                        Game_Btn_Visible = false;
                        Game_Btn_Enable = false;
                        Gameover_textdraw(sender, surface, info);
                    }
                    break;

                case (int)GameStartCondition.GameClear:
                    {
                        GameClearText(sender, surface, info);
                    }
                    break;

                default:
                    break;
            }

        }

        public void PaintSurface_main_Ver2(object sender, SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.
            canvas.Clear();
            switch (GameState)
            {
                case (int)GameStartCondition.GameTitle:
                    {
                        /*
                         *  LEVEL 1~5 Wall Make
                         */
                        DefenceWallMake(Game_level, canvas, info);

                        /*
                         *  Game Level Text Make
                         */
                        GameLevelTextCanvas(Game_level, sender, surface, info);

                        /*
                         *  Init Ball Position Draw Make
                         */
                        InitBallPostion(Game_level, info);

                        /*
                         *  Goal Block Position Calculate
                         */
                        GoalBlockRectPosition(info, canvas, paint_GoalBlock);

                        /*
                         *  Goal Block DrawOnly Make
                         */
                        GoalBlockDrawOnly(Game_level, canvas);

                        //Init Restrict Circle Location Paint
                        SKCanvas canvas_restrictcircle = surface.Canvas;
                        SKCanvas canvas_initcircle = surface.Canvas;

                        // 공 날리기전에는 무조건 초기 원 설정 범위 내부에 들어오게 한다.
                        if (timer_stop_PN != true)
                        {
                            canvas_restrictcircle.DrawCircle(InitConstant_X, InitConstant_Y, (float)(info.Width * 0.07), paint_restrictcircle);
                            canvas_initcircle.DrawCircle(Init_x, Init_y, (float)(info.Width * 0.01), paintInitBall);
                        }

                        /*
                         *  App Start -> Device Timer(Wall Motion Calculate),
                         *               Thread Timer(SKcanvas ReDraw (Invalidate))
                         */
                        if (GameInitOnOff)
                        {
                            StartTimer(sender);
                            if (GameInitDeviceTimerOnOff)
                            {
                                DeviceTimer_Wall(sender);
                            }
                            GameInitOnOff = false;
                            GameInitDeviceTimerOnOff = false;
                        }
                    }
                    break;

                case (int)GameStartCondition.GameInit:
                    {
                        /*
                         *  LEVEL 1~5 Wall Make
                         */
                        DefenceWallMake(Game_level, canvas, info);

                        /*
                         *  Game Level Text Make
                         */
                        GameLevelTextCanvas(Game_level, sender, surface, info);

                        /*
                         *  Goal Block DrawOnly Make
                         */
                        GoalBlockDrawOnly(Game_level, canvas);

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

                        /*
                         *  Ball Radius and Degree Calculate
                         */
                        Init_Rad_cal();

                        // 공 날리기전에는 무조건 초기 원 설정 범위 내부에 들어오게 한다.
                        if (timer_stop_PN != true)
                        {
                            canvas_initcircle.DrawCircle(Init_x, Init_y, info.Width * ballsize, paintInitBall);
                        }

                        //TEST ACTION
                        Vo_test = ((float)Math.Sqrt((Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2)))
                            / default_radius_move_allow) * Vo_test_Power; //300은 조금 약하고,, 400은 너무 쌔다..500 -> 600 -> 1000 ( Thread 동작 개선했더니 느려짐 )

                        //Binding Text Collection
                        Game_power = (int)Vo_test;
                        Game_rad = (int)(control_rcos_abs * 100);

                        /*
                         *  Start Direction Prediction Arrow Graphics
                         */
                        PredictArrowDirection(canvas, info, paint_PredictArrow);

                    }
                    break;

                case (int)GameStartCondition.GamePlay:
                    {
                        /*
                         *  LEVEL 1~5 Wall Make
                         */
                        DefenceWallMake(Game_level, canvas, info);

                        /*
                         * x,y Position Pre-Calculate
                         */
                        if (cnt_gameplayrender == 0)
                        {
                            PredictPostionCalculate(canvas, info, Vo_test, control_rcos_abs, control_rsin_abs);
                        }
                        cnt_gameplayrender++;
                        GameLevelTextCanvas(Game_level, sender, surface, info);

                        if (cnt_rendering >= cnt_gameplayrender)
                        {
                            canvas.DrawCircle(x_rendering[cnt_gameplayrender], y_rendering[cnt_gameplayrender], (float)0.01 * info.Width, paint);
                            if (x_rendering[cnt_gameplayrender] > (float)(info.Width * 0.75))
                            {
                                GoalBlockDamageCount(Game_level, x_rendering[cnt_gameplayrender], y_rendering[cnt_gameplayrender], info);
                            }
                        }
                        //Ball Rendering End and Finish Condition Check
                        else
                        {
                            //Game End Setting, Variable initialization
                            GameEndandInitSetting(sender, Game_level, surface, info);
                        }

                        /*
                         *  Goal Block Destroy Effect
                         */
                        GoalBlockDestroyEffect(Game_level, canvas, info, surface);

                    }
                    break;

                case (int)GameStartCondition.GameEnd:
                    {
                        canvas.Clear();
                        EndWaitTime(1000);
                        Game_Btn_Visible = false;
                        Game_Btn_Enable = false;
                        Gameover_textdraw(sender, surface, info);
                    }
                    break;

                case (int)GameStartCondition.GameClear:
                    {
                        GameClearText(sender, surface, info);
                    }
                    break;

                default:
                    break;
            }
        }

        private void PredictPostionCalculate(SKCanvas canvas, SKImageInfo info, float Vo_test, float control_rcos_abs, float control_rsin_abs)
        {
            pos_y_ceiling_touchIncrease = 0;

            pos_y_ceiling_touch = false;
            for (int i = 0; i < info.Width * 2; i++)
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
                    float t_y = (float)pos_y_ceiling_touchIncrease / 25;
                    //float g_y = 6 * t_y;
                    //y = t_y * g_y * 5;

                    float g_y = 7 * t_y;
                    y = t_y * g_y * 5;
                }
                /*
                 * Ball Touch DefenceWall True or False
                 */
                DefenceWallTouchJudge(Game_level, x, y, info);
                
                switch (Game_level)
                {
                    case 1:
                        if (wall_pn[0])
                        {
                            x = (defencewall_1_x[0]-ballsize) * info.Width + ((defencewall_1_x[0] - ballsize) * info.Width - x);
                        }
                        break;
                    case 2:
                        if (wall_pn[1])
                        {
                            x = (defencewall_2_x[0] - ballsize) * info.Width + ((defencewall_2_x[0] - ballsize) * info.Width - x);
                        }
                        break;
                    case 3:
                        if (wall_pn[2])
                        {
                            x = (float)xpos_mirror * info.Width + ((float)xpos_mirror * info.Width - x);
                        }
                        break;
                    case 4:
                        if (wall_pn[3])
                        {
                            x = (defencewall_4_x_1[0] - ballsize) * info.Width + ((defencewall_4_x_1[0] - ballsize) * info.Width - x);
                        }
                        if (wall_pn_sub[3] != true && x >= (defencewall_4_x_2[0]-ballsize) * info.Width &&
                            x <= (defencewall_4_x_2[0] + ballsize) * info.Width && y >= defencewall_4_y_2[0] * info.Height
                            && y <= defencewall_4_y_2[1] * info.Height)
                        {
                            wall_pn_sub[3] = true;
                            wall_pn[3] = false;
                        }
                        if (wall_pn_sub[3])
                        {
                            x = (defencewall_4_x_2[0] - ballsize) * info.Width + ((defencewall_4_x_2[0] - ballsize) * info.Width - x);

                            if (wall_pn_Level4_main[1] != 1 && x >= (defencewall_4_x_1[0]+ballsize) * info.Width && x <= (defencewall_4_x_1[0] + ballsize*2) * info.Width
                                && y >= defencewall_4_y_1[0] * info.Height && y <= defencewall_4_y_1[1] * info.Height)
                            {
                                wall_pn_Level4_main[1] = 1;
                            }
                        }
                        //공이 A B 벽이 있을 경우,
                        //B벽을 맞고 A벽을 맞을때 공의 위치 계산
                        // A<-->B , A와B 벽사이의 거리 *2 만큼 x위치에서 빼준다.
                        if (wall_pn_Level4_main[1] == 1)
                        {
                            x = (float)(Vo_test * control_rcos_abs) * t + Init_x - 0.3f * info.Width;
                            wall_pn_sub[3] = false;

                            if (x >= (defencewall_4_x_2[0] - ballsize) * info.Width && x <= (defencewall_4_x_2[0]) * info.Width
                                && y >= defencewall_4_y_2[0] * info.Height && y <= defencewall_4_y_2[1] * info.Height)
                            {
                                wall_pn_sub[3] = true;
                                wall_pn[3] = false;
                                wall_pn_Level4_main[1] = 0;
                            }

                        }
                        break;
                    case 5:
                        if (wall_pn[4])
                        {
                            x = (defencewall_5_x_1[0]-ballsize) * info.Width + ((defencewall_5_x_1[0] - ballsize) * info.Width - x);
                        }
                        break;
                    default:
                        break;
                }
                //Add : for pre-rendering
                cnt_rendering = i;
                x_rendering[i] = x;
                y_rendering[i] = y;

                // GAME END
                // End rendering point save
                if (y > info.Height || x < 0 || x > info.Width)
                {
                    enable_rendering = true;
                    cnt_rendering = i;
                    break;
                }
            }

        }

        private void PredictArrowDirection(SKCanvas canvas, SKImageInfo info, SKPaint paint_PredictArrow)
        {
            if (TouchOnOff)
            {
                //paintInitBall, 예측 공 Paint
                for (int i = 1; i < Vo_test_Power/100; i++)
                {
                    float Predict_t = 0.125f * i/2;
                    float Predict_g = 5 * Predict_t;
                    float Predict_x = (float)(Game_power * control_rcos_abs) * Predict_t + Init_x;
                    float Predict_y = ((float)(Game_power * control_rsin_abs) * Predict_t - Predict_g * Predict_t * 4);
                    Predict_y = Init_y - Predict_y;

                    canvas.DrawCircle(Predict_x, Predict_y, 
                        (float)(0.01-0.0005*i) * info.Width , paint);
                }
            }
        }

        private void GameLevelTextCanvas(int game_level, object sender, SKSurface surface, SKImageInfo info)
        {
            //SKImageInfo info = e.Info; //그리기 화면에 대한 정보 (너비, 높이 픽셀)
            //SKSurface surface = e.Surface; // 그리기 화면 자체
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.

            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Black.ToSKColor(),
                FakeBoldText = true,
                StrokeWidth = 1
            };

            string game_over_text = "LEVEL"+game_level;

            //Adjust TextSize property so text is 95% of screen width
            float textWidth = textPaint.MeasureText(game_over_text);
            textPaint.TextSize = 0.1f * info.Width * textPaint.TextSize / textWidth;

            //Find the text bounds
            SKRect textBounds = new SKRect();
            textPaint.MeasureText(game_over_text, ref textBounds);

            // Calculate offsets to center the text on the screen
            float xText = (float)0.1*info.Width;
            float yText = (float)0.1 * info.Height;

            //And draw the text
            canvas.DrawText(game_over_text, xText, yText, textPaint);
            return;
        }

        private void GameClearText(object sender, SKSurface surface, SKImageInfo info)
        {
            //SKImageInfo info = e.Info; //그리기 화면에 대한 정보 (너비, 높이 픽셀)
            //SKSurface surface = e.Surface; // 그리기 화면 자체
            SKCanvas canvas = surface.Canvas; //그래픽그리기 컨텍스트
                                              //개체는 그래픽 SKCanvan 변환과 클리핑을 포함 하는 그래픽 상태를 캡슐화 합니다.

            canvas.Save();
            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Red.ToSKColor(),
                FakeBoldText = true,
                StrokeWidth = 1
            };

            string game_over_text = "CLEAR";

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
        }

        private void DefenceWallTouchJudge(int LEVEL, float x, float y, SKImageInfo info)
        {
            switch (LEVEL)
            {
                case 1:
                    if (wall_pn[0] != true && x >= (defencewall_1_x[0]-ballsize) * info.Width &&
                        x <= (defencewall_1_x[1] - ballsize) * info.Width && y >= defencewall_1_y[0] * info.Height
                        && y <= defencewall_1_y[1] * info.Height)
                    {
                        wall_pn[0] = true;
                    }
                    break;
                case 2:
                    if (wall_pn[1] != true && x >= (defencewall_1_x[0] - ballsize) * info.Width &&
                        x <= (defencewall_1_x[0] + ballsize*2) * info.Width && y >= defencewall_2_y[0] * info.Height
                        && y <= defencewall_2_y[1] * info.Height)
                    {
                        wall_pn[1] = true;
                    }
                    break;
                case 3:
                    if (wall_pn[2] != true && x >= (defencewall_3_x[0]-ballsize) * info.Width &&
                        x <= (defencewall_3_x[0] + ballsize*2) * info.Width && y >= defencewall_3_y[0] * info.Height
                        && y <= (defencewall_3_y[0] / 10 + defencewall_3_y[0]) * info.Height)
                    {
                        wall_pn[2] = true;
                        xpos_mirror = (defencewall_3_x[0] - ballsize);
                    }
                    else if (wall_pn[2] != true && x >= (defencewall_3_x[0] + ballsize) * info.Width &&
                        x <= (defencewall_3_x[0] + ballsize * 3) * info.Width && y >= defencewall_3_y[0] * info.Height
                        && y <= (defencewall_3_y[0] / 10 + defencewall_3_y[0]) * info.Height)
                    {
                        wall_pn[2] = true;
                        xpos_mirror = (defencewall_3_x[0] + ballsize);
                    }
                    else if (wall_pn[2] != true && x >= (defencewall_3_x[0] + ballsize * 4) * info.Width &&
                        x <= (defencewall_3_x[0] + ballsize * 6) * info.Width && y >= (defencewall_3_y[0] / 10 + defencewall_3_y[0]) * info.Height
                        && y <= ((defencewall_3_y[0]*2) / 10 + defencewall_3_y[0]) * info.Height)

                    {
                        wall_pn[2] = true;
                        xpos_mirror = (defencewall_3_x[0] + ballsize * 4);
                    }
                    else if (wall_pn[2] != true && x >= (defencewall_3_x[0] + ballsize * 7) * info.Width &&
                        x <= (defencewall_3_x[0] + ballsize * 10) * info.Width && y >= ((defencewall_3_y[0]*2) / 10 + defencewall_3_y[0]) * info.Height
                        && y <= ((defencewall_3_y[0] * 3) / 10 + defencewall_3_y[0]) * info.Height)
                    {
                        wall_pn[2] = true;
                        xpos_mirror = (defencewall_3_x[0] + ballsize * 7);
                    }
                    else if (wall_pn[2] != true && x >= (defencewall_3_x[0] + ballsize * 10) * info.Width &&
                        x <= (defencewall_3_x[0] + ballsize * 12) * info.Width && y >= ((defencewall_3_y[0] * 3) / 10 + defencewall_3_y[0]) * info.Height
                        && y <= ((defencewall_3_y[0] * 4) / 10 + defencewall_3_y[0]) * info.Height)
                    {
                        wall_pn[2] = true;
                        xpos_mirror = (defencewall_3_x[0] + ballsize * 10);
                    }
                    break;

                case 4:
                    if (wall_pn[3] != true && x >= (defencewall_4_x_1[0]-ballsize) * info.Width &&
                        x <= (defencewall_4_x_1[0] + ballsize) * info.Width && y >= defencewall_4_y_1[0] * info.Height
                        && y <= defencewall_4_y_1[1] * info.Height)
                    {
                        wall_pn[3] = true;
                    }
                    break;
                case 5:
                    if (wall_pn[4] != true && x >= (defencewall_5_x_1[0]-ballsize) * info.Width 
                        && x <= (defencewall_5_x_1[0] + ballsize) * info.Width) 
                    {
                        //y좌표가 0~0.2info.height, 0.4~0.6만 통과한다.
                        if ( y >= defencewall_5_y_2[0] * info.Height && y <= defencewall_5_y_2[1] * info.Height )
                        {
                            wall_pn[4] = true;
                        }
                        if( y >= defencewall_5_y_1[0] * info.Height && y <= defencewall_5_y_1[1] * info.Height )
                        {
                            wall_pn[4] = true;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void Paint_Collection(SKPaint paint, SKPaint paint_restrictcircle, SKPaint paintInitBall, SKPaint paint_GoalBlock, SKPaint paint_PredictArrow)
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

            //Start Direction Prediction Arrow
            paint_PredictArrow.IsAntialias = true;
            paint_PredictArrow.Style = SKPaintStyle.StrokeAndFill;
            paint_PredictArrow.Color = Color.Red.ToSKColor();
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

                case 4:
                    for (int row = 2; row < 4; row++)
                    {
                        //row 2~3, 3~4
                        if (rect_pn[row] == true && rect_point[row] < x && rect_point[row + 1] > x)
                        {
                            for (int col = 0; col < 1; col++)
                            {
                                //col 0~1
                                if (rect_pn[4 * col + row] == true && rect_point[col + 5] < y && rect_point[col + 6] > y)
                                {
                                    rect_pn[4 * col + row] = false;
                                    Game_score++;
                                }
                            }
                        }
                    }
                    break;
                case 5:
                    for (int row = 3; row < 4; row++)
                    {
                        //row 2~3, 3~4
                        if (rect_pn[row] == true && rect_point[row] < x && rect_point[row + 1] > x)
                        {
                            for (int col = 0; col < 1; col++)
                            {
                                //col 0~1
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
                Game_Btn_Visible = false;
                Game_Btn_Enable = false;
                return;
            }
            else
            {
                // /*
                //  *  LEVEL 1~5 Wall Make
                //  */
                // DefenceWallMake(Game_level, canvas, info);

                /*
                 *  Goal Block Destroy Effect
                 */
                SKCanvas canvas_GoalBlock = surface.Canvas;
                GoalBlockDestroyEffect(Game_level, canvas_GoalBlock, info, surface);
            }
        }

        private void GoalBlockDrawOnly(int LEVEL, SKCanvas canvas)
        {
            SKPaint paintGoalBlock = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Blue.ToSKColor()
            };
            switch (LEVEL)
            {
                case 1:
                    for (int i = 0; i < 16; i++)
                            canvas.DrawRect(rect[i], paintGoalBlock);
                    break;
                case 2:
                    for (int i = 1; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int position = i + j * 4;
                            canvas.DrawRect(rect[position], paintGoalBlock);
                        }
                    }
                    break;
                case 3:
                    for (int i = 2; i < 4; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            int position = i + j * 4;
                            canvas.DrawRect(rect[position], paintGoalBlock);
                        }
                    }
                    break;
                case 4:
                    for (int i = 2; i < 4; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            int position = i + j * 4;
                            canvas.DrawRect(rect[position], paintGoalBlock);
                        }
                    }
                    break;
                case 5:
                    for (int i = 3; i < 4; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            int position = i + j * 4;
                            canvas.DrawRect(rect[position], paintGoalBlock);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void GoalBlockDestroyEffect(int LEVEL, SKCanvas canvas, SKImageInfo info, SKSurface surface)
        {
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
                        {
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
                case 4:
                    for (int i = 2; i < 4; i++)
                    {
                        for (int j = 0; j < 1; j++)
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
                case 5:
                    for (int i = 3; i < 4; i++)
                    {
                        for (int j = 0; j < 1; j++)
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
            //New : for block effect rendering
            BlockRotationCnt++;

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
            switch (LEVEL)
            {
                case 1:
                    SKPoint[] DefenceWallLevel_1 = new SKPoint[4];
                    DefenceWallLevel_1[0] = new SKPoint(defencewall_1_x[0] * info.Width, defencewall_1_y[0] * info.Height);
                    //DefenceWallLevel_1[1] = new SKPoint(defencewall_1_x[1] * info.Width, defencewall_1_y[0] * info.Height);
                    DefenceWallLevel_1[2] = new SKPoint(defencewall_1_x[0] * info.Width, defencewall_1_y[1] * info.Height);
                    //DefenceWallLevel_1[3] = new SKPoint(defencewall_1_x[1] * info.Width, defencewall_1_y[1] * info.Height);

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
                    // DefenceWallLevel_2[0] = new SKPoint((float)0.55 * info.Width, (float)0.15 * info.Height);
                    // DefenceWallLevel_2[1] = new SKPoint((float)0.56 * info.Width, (float)0.15 * info.Height);
                    // DefenceWallLevel_2[2] = new SKPoint((float)0.55 * info.Width, (float)0.7 * info.Height);
                    // DefenceWallLevel_2[3] = new SKPoint((float)0.56 * info.Width, (float)0.7 * info.Height);

                    DefenceWallLevel_2[0] = new SKPoint(defencewall_2_x[0] * info.Width, defencewall_2_y[0] * info.Height);
                    DefenceWallLevel_2[1] = new SKPoint(defencewall_2_x[1] * info.Width, defencewall_2_y[0] * info.Height);
                    DefenceWallLevel_2[2] = new SKPoint(defencewall_2_x[0] * info.Width, defencewall_2_y[1] * info.Height);
                    DefenceWallLevel_2[3] = new SKPoint(defencewall_2_x[1] * info.Width, defencewall_2_y[1] * info.Height);
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
                    // DefenceWallLevel_3[0] = new SKPoint((float)0.5 * info.Width,  (float)0.15 * info.Height);
                    // DefenceWallLevel_3[3] = new SKPoint((float)0.7 * info.Width,  (float)0.75 * info.Height);
                    DefenceWallLevel_3[0] = new SKPoint(defencewall_3_x[0] * info.Width, defencewall_3_y[0] * info.Height);
                    DefenceWallLevel_3[3] = new SKPoint(defencewall_3_x[1] * info.Width, defencewall_3_y[1] * info.Height);

                    SKPaint paintDefenceWallLevel_3 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Black.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.015)
                    };

                    canvas.DrawLine(DefenceWallLevel_3[0], DefenceWallLevel_3[3], paintDefenceWallLevel_3);
                    break;
                case 4:
                    SKPoint[] DefenceWallLevel_4 = new SKPoint[4];
                    DefenceWallLevel_4[0] = new SKPoint(defencewall_4_x_1[0] * info.Width, defencewall_4_y_1[0] * info.Height);
                    DefenceWallLevel_4[1] = new SKPoint(defencewall_4_x_1[0] * info.Width, defencewall_4_y_1[1] * info.Height);
                    DefenceWallLevel_4[2] = new SKPoint(defencewall_4_x_2[0] * info.Width, defencewall_4_y_2[0] * info.Height);
                    DefenceWallLevel_4[3] = new SKPoint(defencewall_4_x_2[0] * info.Width, defencewall_4_y_2[1] * info.Height);

                    SKPaint paintDefenceWallLevel_4 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Purple.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.01)
                    };
                    SKPaint paintDefenceWallLevel_4_sub = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Purple.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.01)
                    };

                    canvas.DrawLine(DefenceWallLevel_4[0], DefenceWallLevel_4[1], paintDefenceWallLevel_4);
                    canvas.DrawLine(DefenceWallLevel_4[2], DefenceWallLevel_4[3], paintDefenceWallLevel_4_sub);
                    break;

                case 5:
                    SKPoint[] DefenceWallLevel_5 = new SKPoint[4];
                    DefenceWallLevel_5[0] = new SKPoint(defencewall_5_x_1[0] * info.Width, defencewall_5_y_1[0] * info.Height);
                    DefenceWallLevel_5[1] = new SKPoint(defencewall_5_x_1[0] * info.Width, defencewall_5_y_1[1] * info.Height);
                    DefenceWallLevel_5[2] = new SKPoint(defencewall_5_x_2[0] * info.Width, defencewall_5_y_2[0] * info.Height);
                    DefenceWallLevel_5[3] = new SKPoint(defencewall_5_x_2[0] * info.Width, defencewall_5_y_2[1] * info.Height);

                    SKPaint paintDefenceWallLevel_5 = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Cyan.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.01)
                    };
                    SKPaint paintDefenceWallLevel_5_sub = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = Color.Cyan.ToSKColor(),
                        StrokeWidth = (float)(info.Width * 0.01)
                    };

                    canvas.DrawLine(DefenceWallLevel_5[0], DefenceWallLevel_5[1], paintDefenceWallLevel_5);
                    canvas.DrawLine(DefenceWallLevel_5[2], DefenceWallLevel_5[3], paintDefenceWallLevel_5_sub);
                    break;
                default:
                    break;
            }
        }

        public void TouchTitle(object sender, SKTouchEventArgs e)
        {
             if(GameState == (int)GameStartCondition.GameTitle)
             {
                game_start_touch_enable = false;
                //DeviceTimer_Wall(sender);
                //GameState Change
                GameState = (int)GameStartCondition.GameInit;

                ((SKCanvasView)sender).InvalidateSurface();
                //timer_stop_PN = true;
                StartTimer(sender);
                game_touch_enable = true;
                //copy git
                e.Handled = true;
                //TouchMotion Wait?
                EndWaitTime(5000);
            }
        }

        public void TouchMotion(object sender, SKTouchEventArgs e)
        {
            //if (!game_over && TouchOnOff && !game_start_touch_enable)
            //if (GameState == (int)GameStartCondition.GameTitle )
            if (game_touch_enable)
            {
                drag_onoff = false;

                // GameState Change
                GameState = (int)GameStartCondition.GameInit;

                //StartTimer_Wall(sender);
                switch (e.ActionType)
                {
                    case SKTouchAction.Moved:
                        Init_x = e.Location.X;
                        Init_y = e.Location.Y;

                        dis = (float)(Math.Pow(Init_x - default_Init_x, 2) + Math.Pow(Init_y - default_Init_y, 2));
                        dis_init_circle = (float)Math.Sqrt(dis);
                        
                        control_rcos = (Init_x - default_Init_x) / dis_init_circle;
                        control_rsin = (Init_y - default_Init_y) / dis_init_circle;
                        
                        if (dis_init_circle >= default_radius_move_allow)
                        {
                            Init_x = control_rcos * default_radius_move_allow + default_Init_x;
                            Init_y = control_rsin * default_radius_move_allow + default_Init_y;
                        }
                        
                        if (e.Location.X >= InitConstant_X)
                        {
                            Init_x = InitConstant_X;
                        }

                        if (e.InContact)
                        {
                            EndWaitTime(1000);
                            ((SKCanvasView)sender).InvalidateSurface();
                        }
                        break;

                    case SKTouchAction.Released:
                        timer_stop_PN = true;
                        parabolic_clear = false;
                        parabolic_cnt = 0;
                        parabolic_pre_cnt = 0;
                        BlockRotationCnt = 0;
                        cnt_gameplayrender = 0;

                        game_touch_enable = false;
                        TouchOnOff = false;

                        // GameState Change
                        GameState = (int)GameStartCondition.GamePlay;
                        break;
                }

                //  if (timer_stop_PN)
                //  {
                //      game_touch_enable = false;
                //      TouchOnOff = false;
                //      //timer_stop_wall();
                //      //time_interval = 5;  //skcanvas redraw time 
                //      //StartTimer(sender); //Make SK Graphics
                //  
                //      // // GameState Change
                //      // GameState = (int)GameStartCondition.GamePlay;
                //  }
                //copy git
                e.Handled = true;
            }

            //if (GameState == (int)GameStartCondition.GameTitle)
            //{
            //    game_start_touch_enable = false;
            //
            //    //GameState Change
            //    GameState = (int)GameStartCondition.GameInit;
            //
            //    StartTimer(sender);
            //    game_touch_enable = true;
            //
            //    //copy git
            //    e.Handled = true;
            //    //TouchMotion Wait?
            //    EndWaitTime(5000);
            //}
        }

        private void GoalBlockRectPosition(SKImageInfo info, SKCanvas canvas, SKPaint paint_GoalBlock)
        {
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
                case 4:
                    //LEVEL4 Init Location Constant
                    InitConstant_X = (float)(info.Width * 0.2);
                    InitConstant_Y = (float)(info.Height * 0.5);
                    default_radius_move_allow = (float)(info.Width * 0.07);
                    default_Init_x = (float)(info.Width * 0.2);
                    default_Init_y = (float)(info.Height * 0.5);

                    //LEVEL4 Init Ball Position
                    if (drag_onoff)
                    {
                        Init_x = (float)(info.Width * 0.2);
                        Init_y = (float)(info.Height * 0.5);

                        //Init rect_pn (2x1)
                        for (int i = 2; i < 4; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                rect_pn[i + j * 4] = true;
                            }
                        }
                    }
                    break;
                case 5:
                    //LEVEL5 Init Location Constant
                    InitConstant_X = (float)(info.Width * 0.2);
                    InitConstant_Y = (float)(info.Height * 0.6);
                    default_radius_move_allow = (float)(info.Width * 0.07);
                    default_Init_x = (float)(info.Width * 0.2);
                    default_Init_y = (float)(info.Height * 0.6);

                    //LEVEL5 Init Ball Position
                    if (drag_onoff)
                    {
                        Init_x = (float)(info.Width * 0.2);
                        Init_y = (float)(info.Height * 0.6);

                        //Init rect_pn (1x1)
                        for (int i = 3; i < 4; i++)
                        {
                            for (int j = 0; j < 1; j++)
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
            
            if (dis_init_circle >= default_radius_move_allow)
            {
                Init_x = control_rcos * default_radius_move_allow + default_Init_x;
                Init_y = control_rsin * default_radius_move_allow + default_Init_y;
            }
            control_rcos_abs = ((default_Init_x - Init_x) / dis_init_circle);
            control_rsin_abs = ((Init_y - default_Init_y) / dis_init_circle);

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
                        
            //enable_rendering = true;
            for (int i = 0; i <= cnt_rendering; i++)
            {
                x_rendering[i] = 0f;
                y_rendering[i] = 0f;
            }

            /*
             *  reset Setting
             */
            cnt_gameplayrender = 0;
            //BlockRotationCnt = 0;
            //parabolic_cnt = 0;
            //time_interval = 0;
            //BlockRotationCnt = 0;
            //game_over = true;

            pos_y_ceiling_touch = false;
            pos_y_ceiling_touchIncrease = pos_y_Increase;
            xpos_mirror = 0;
            TouchOnOff = true;

            for (int i = 0; i < 2; i++)
            {
                wall_pn_Level4_main[i] = 0;
                wall_pn_Level4_sub[i] = 0;
            }

            for (int i = 0; i < LEVEL_MAX; i++)
            {
                wall_pn[i] = false;
                wall_pn_sub[i] = false;
            }

            if (timer_stop_PN)
            {
                /*
                 * Timer Stop
                 */

                //timer_stop_wall();
                //timer_stop();
                timer_stop_PN = false;
                timer_stop_PN_BlockRotation = false;
                drag_onoff = true; //Initial x,y 
                Init_x = 0;
                Init_y = 0;
                GameState = (int)GameStartCondition.GameEnd;
                //Init rect_pn
                switch(LEVEL)
                {
                    case 1:
                        for (int j = 0; j < 16; j++)
                        {
                            //rect_pn = false -> block destory
                            if (!rect_pn[j])
                            {
                                GameState = (int)GameStartCondition.GameInit;
                            }
                            rect_pn[j] = true;
                        }
                        break;
                    case 2:
                        for(int i = 1; i < 4; i++)
                        {
                            for(int j = 0; j < 3; j++)
                            {
                                if (!rect_pn[i + j * 4])
                                {
                                    GameState = (int)GameStartCondition.GameInit;
                                }
                                rect_pn[i + j * 4] = true;
                            }
                        }
                        break;
                    case 3:
                        for (int i = 2; i < 4; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (!rect_pn[i + j * 4])
                                {
                                    GameState = (int)GameStartCondition.GameInit;
                                }
                                rect_pn[i + j * 4] = true;
                            }
                        }
                        break;
                    case 4:
                        for (int i = 2; i < 4; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                if (!rect_pn[i + j * 4])
                                {
                                    GameState = (int)GameStartCondition.GameInit;
                                }
                                rect_pn[i + j * 4] = true;
                            }
                        }
                        break;
                    case 5:
                        for (int i = 3; i < 4; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                if (!rect_pn[i + j * 4])
                                {
                                    GameState = (int)GameStartCondition.GameInit;
                                }
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

            cnt_rendering = 0;
            enable_rendering = false;

            if (GameState == (int)GameStartCondition.GameEnd)
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear();

                EndWaitTime(1000); // Add for End Btn Image 
                Game_Btn_Visible = false;
                Game_Btn_Enable = false;
                //gameStartTitle_Visible = true;
                //gameStartTitle_Enable = true;
                EndWaitTime(1000); // Add for End Btn Image
                Gameover_textdraw(sender, surface, info);
                timer_stop();
            }
            else
            {
                Game_level++;
                wallupdown = true;
                game_touch_enable = true;
                ((SKCanvasView)sender).InvalidateSurface();
            }

        }

        private void GameStartTitle_textdraw(object sender, SKSurface surface, SKImageInfo info)
        {
             SKCanvas canvas = surface.Canvas;
             SKPaint textPaint = new SKPaint
             {
                 Style = SKPaintStyle.Fill,
                 Color = Color.Red.ToSKColor(),
                 FakeBoldText = true,
                 StrokeWidth = 1
             };
             string game_Title_text = "START TOUCH!";
             
             //Adjust TextSize property so text is 95% of screen width
             float textWidth = textPaint.MeasureText(game_Title_text);
             textPaint.TextSize = 0.2f * info.Width * textPaint.TextSize / textWidth;
             
             //Find the text bounds
             SKRect textBounds = new SKRect();
             textPaint.MeasureText(game_Title_text, ref textBounds);
             
             // Calculate offsets to Low center the text on the screen
             float xText = info.Width / 2 - textBounds.MidX;
             float yText = info.Height * 0.8f;
             
             //And draw the text
             canvas.DrawText(game_Title_text, xText, yText, textPaint);
             //canvas.Restore();
             EndWaitTime(1000);
             return;
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
                Style = SKPaintStyle.Fill,
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

            EndWaitTime(1000);
            Game_Btn_Visible = false;
            Game_Btn_Enable = false;
        }

        public void BTN_game_continue(object sender, EventArgs e, Button gamecontinue_btn, bool answer_, ref ParabolicDB parabolics)
        {
            //Add for Operation Safety
            EndWaitTime(1000);

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
            parabolics.LevelPrabolic_class = Game_level;
            parabolics.ScoreParabolic_class = Game_score;
            Game_score = 0;
            Game_level = 1;
            Game_power = 0;
            Game_rad = 0;
            Game_Btn_Visible = false;
            Game_Btn_Enable = true;
        }

        public void GameResetAndUpdateDB(object sender, bool answer_, ref ParabolicDB parabolics)
        {
            //Add for Operation Safety
            EndWaitTime(1000);

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
            parabolics.LevelPrabolic_class = Game_level;
            parabolics.ScoreParabolic_class = Game_score;
            Game_score = 0;
            Game_level = 1;
            Game_power = 0;
            Game_rad = 0;
            Game_Btn_Visible = false;
            Game_Btn_Enable = true;
        }

        public void OnTapGestureRecognizerTapped_GameReset(object sender, EventArgs e, ref ParabolicDB parabolics)
        {
            //Add for Operation Safety
            EndWaitTime(2000);

            parabolic_cnt = 0;
            game_over = false;

            timer_stop_PN = false;
            TouchOnOff = true; 
            drag_onoff = true; //Initial x,y 
            Init_x = 0;
            Init_y = 0;

            //Init rect_pn
            for (int j = 0; j < 16; j++)
            {
                rect_pn[j] = true;
            }
            parabolics.LevelPrabolic_class = Game_level;
            parabolics.ScoreParabolic_class = Game_score;
            Game_score = 0;
            Game_level = 1;
            Game_power = 0;
            Game_rad = 0;
            game_touch_enable = true;
            GameInitOnOff = true;
            Game_Btn_Visible = false;
            Game_Btn_Enable = false;
            cnt_defencewallrendering_1 = 0;
            cnt_defencewallrendering_2 = 0;
            cnt_defencewallrendering_3 = 0;
            cnt_defencewallrendering_4 = 0;
            cnt_defencewallrendering_5 = 0;
            GameState = (int)GameStartCondition.GameTitle;
            //((MainPage)sender).CanvasView3_Invalidate();
        }

        public void Debug_GameLevelUp(object sender, EventArgs e, Button gamecontinue_btn)
        {
            //Add for Operation Safety
            EndWaitTime(1000);
            Game_level++;
            wallupdown = true;
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

            Game_Btn_Visible = false;
            Game_Btn_Enable = true;
        }
        public void Debug_GameLevelDown(object sender, EventArgs e, Button gamecontinue_btn)
        {
            //Add for Operation Safety
            EndWaitTime(1000);
            Game_level--;
            parabolic_cnt = 0;
            game_over = false;
            wallupdown = true;
            timer_stop_PN = false;
            drag_onoff = true; //Initial x,y 
            Init_x = 0;
            Init_y = 0;

            //Init rect_pn
            for (int j = 0; j < 16; j++)
            {
                rect_pn[j] = true;
            }

            Game_Btn_Visible = false;
            Game_Btn_Enable = true;
        }

        public void Btn_StartTitle(object sender, EventArgs e)
        {
            gameStartTitle_Visible = false;
            gameStartTitle_Enable = false;
            game_touch_enable = true;
            //StartTimer(sender);
        }

        private void StartTimer_BlockRotation()
        {
            if (timer_stop_PN)
            {
                timer_stop_PN_BlockRotation = true;
            }
        }

        public void DeviceTimer_Wall(object sender)
        {
            //Only-One use
            Game_level = 1;
            Device.StartTimer(TimeSpan.FromMilliseconds(5), () =>
            {
                switch (Game_level)
                {
                    case 1: //Up-down
                        cnt_defencewallrendering_1++;
                        if (wallupdown) // up
                        {
                            defencewall_1_y[1] = 0.8f - 0.002f * (cnt_defencewallrendering_1);
                            defencewall_1_y[0] = 0.2f - 0.002f * (cnt_defencewallrendering_1);
                            if (cnt_defencewallrendering_1 >= 99)
                            {
                                wallupdown = false;
                                cnt_defencewallrendering_1 = 0;
                            }
                        }
                        else //down
                        {
                            defencewall_1_y[1] = 0.602f + 0.002f * (cnt_defencewallrendering_1);
                            defencewall_1_y[0] = 0.002f + 0.002f * (cnt_defencewallrendering_1);
                            if (cnt_defencewallrendering_1 >= 99)
                            {
                                wallupdown = true;
                                cnt_defencewallrendering_1 = 0;
                            }
                        }
                        break;
                    case 2: //Up-down
                        cnt_defencewallrendering_2++;
                        if (wallupdown) // up
                        {
                            defencewall_2_y[0] = 0.15f - 0.0015f * (cnt_defencewallrendering_2);
                            defencewall_2_y[1] = 0.7f - 0.0015f * (cnt_defencewallrendering_2);
                            if (cnt_defencewallrendering_2 >= 99)
                            {
                                wallupdown = false;
                                cnt_defencewallrendering_2 = 0;
                            }
                        }
                        else //down
                        {
                            defencewall_2_y[0] = 0.0015f * (cnt_defencewallrendering_2);
                            defencewall_2_y[1] = 0.55f + 0.0015f * (cnt_defencewallrendering_2);
                            if (cnt_defencewallrendering_2 >= 99)
                            {
                                wallupdown = true;
                                cnt_defencewallrendering_2 = 0;
                            }
                        }
                        break;
                    case 3: //LeftUp, RightDown
                        cnt_defencewallrendering_3++;
                        if (wallupdown) // up
                        {
                            defencewall_3_x[0] = 0.5f - 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_x[1] = 0.7f - 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_y[0] = 0.15f - 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_y[1] = 0.75f - 0.0015f * (cnt_defencewallrendering_3);
                            if (cnt_defencewallrendering_3 >= 99)
                            {
                                wallupdown = false;
                                cnt_defencewallrendering_3 = 0;
                            }
                        }
                        else //down
                        {
                            defencewall_3_x[0] = 0.35f + 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_x[1] = 0.55f + 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_y[0] = 0.0015f * (cnt_defencewallrendering_3);
                            defencewall_3_y[1] = 0.6f + 0.0015f * (cnt_defencewallrendering_3);
                            if (cnt_defencewallrendering_3 >= 99)
                            {
                                wallupdown = true;
                                cnt_defencewallrendering_3 = 0;
                            }
                        }
                        break;

                    case 4: //Main, Sub : Up-Down
                        cnt_defencewallrendering_4++;
                        if (wallupdown) // up
                        {
                            //main wall
                            defencewall_4_y_1[0] = 0.1f - 0.001f * (cnt_defencewallrendering_4);
                            defencewall_4_y_1[1] = 0.55f - 0.001f * (cnt_defencewallrendering_4);

                            //sub wall
                            defencewall_4_y_2[0] = 0.15f + 0.001f * (cnt_defencewallrendering_4);
                            defencewall_4_y_2[1] = 0.6f + 0.001f * (cnt_defencewallrendering_4);

                            if (cnt_defencewallrendering_4 >= 99)
                            {
                                wallupdown = false;
                                cnt_defencewallrendering_4 = 0;
                            }
                        }
                        else //down
                        {
                            //main
                            defencewall_4_y_1[0] = 0.001f * (cnt_defencewallrendering_4);
                            defencewall_4_y_1[1] = 0.45f + 0.001f * (cnt_defencewallrendering_4);

                            //sub wall
                            defencewall_4_y_2[0] = 0.25f - 0.001f * (cnt_defencewallrendering_4);
                            defencewall_4_y_2[1] = 0.7f - 0.001f * (cnt_defencewallrendering_4);

                            if (cnt_defencewallrendering_4 >= 99)
                            {
                                wallupdown = true;
                                cnt_defencewallrendering_4 = 0;
                            }
                        }
                        break;

                    case 5:
                        cnt_defencewallrendering_5++;
                        if (wallupdown)
                        {
                            //Main Wall : Left Motion
                            //Sub Wall : Right Motion
                            if (cnt_defencewallrendering_5 <= 49)
                            {
                                defencewall_5_x_1[0] = 0.75f - 0.005f * (cnt_defencewallrendering_5);
                                defencewall_5_x_2[0] = 0.75f + 0.002f * (cnt_defencewallrendering_5);
                            }
                            else
                            //Main Wall : Down Motion
                            //Sub Wall : Down Motion
                            {
                                defencewall_5_y_1[0] = 0.15f + 0.006f * (cnt_defencewallrendering_5- 49);
                                defencewall_5_y_1[1] = 0.4f + 0.006f * (cnt_defencewallrendering_5 - 49);
                                defencewall_5_y_2[0] = 0.6f + 0.003f * (cnt_defencewallrendering_5 - 49);
                                defencewall_5_y_2[1] = 0.8f + 0.003f * (cnt_defencewallrendering_5 - 49);
                            }

                            if (cnt_defencewallrendering_5 >= 99)
                            {
                                wallupdown = false;
                                cnt_defencewallrendering_5 = 0;
                            }
                        }
                        else
                        {
                            if (cnt_defencewallrendering_5 <= 49)
                            //Main Wall : Right Motion
                            //Sub Wall : Left Motion
                            {
                                defencewall_5_x_1[0] = 0.5f + 0.005f * (cnt_defencewallrendering_5);
                                defencewall_5_x_2[0] = 0.85f - 0.002f * (cnt_defencewallrendering_5);
                            }
                            else
                            //Main Wall : Up Motion
                            //Sub Wall : Up Motion
                            {
                                defencewall_5_y_1[0] = 0.45f - 0.006f * (cnt_defencewallrendering_5 - 49);
                                defencewall_5_y_1[1] = 0.7f - 0.006f * (cnt_defencewallrendering_5 - 49);
                                defencewall_5_y_2[0] = 0.75f - 0.003f * (cnt_defencewallrendering_5 - 49);
                                defencewall_5_y_2[1] = 0.95f - 0.003f * (cnt_defencewallrendering_5 - 49);
                            }
                            if (cnt_defencewallrendering_5 >= 99)
                            {
                                wallupdown = true;
                                cnt_defencewallrendering_5 = 0;
                            }
                        }
                        break;

                    default:
                        break;
                }
                //((SKCanvasView)sender).InvalidateSurface();
                return true;
            });

        }

        public void StartTimer_Wall(object sender)
        {
            timer_start_wall(MY_WALL_TIMER_TICK_OBECT, sender, 0, time_interval);
        }

        private void timer_start_wall(TimerCallback callback, object sender, int start, int time_interval)
        {
            timer_wall = new System.Threading.Timer(callback, sender, start, 33);
        }

        private async void MY_WALL_TIMER_TICK_OBECT(object sender)
        {
            await Task.Run(() =>
            {
                //Draw DefenceWall Only
                ((SKCanvasView)sender).InvalidateSurface();
            });
        }

        public void StartTimer(object sender)
        {
            //if (timer_stop_PN)
            //{
                timer_start(MY_TIMER_TICK_OBJECT, sender, 0, time_interval);
            //}
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
                
                //if(timer_stop_PN_BlockRotation)
                //{
                //    BlockRotationCnt++;
                //}
            });
        }
        private async void EndWaitTime(int WaitTime)
        {
            await Task.Delay(WaitTime);
        }
        private void timer_stop()
        {
            timer_.Dispose();
        }

        private void timer_stop_wall()
        {
            timer_wall.Dispose();
            
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
