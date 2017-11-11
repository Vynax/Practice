using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NTU
{
    public partial class frmOOP : Form
    {
        #region 參數宣告
        Land landscape;
        rockMan hero;
        List<bullet> listBullet = new List<bullet>();
        List<monster> listMonster = new List<monster>();
        Random r = new Random();
        int hp = 100;
        bool heroMove = false;

        #endregion

        public frmOOP()
        {
            InitializeComponent();
            NewLand();
            NewHero();
        }

        //創建地圖
        private void NewLand()
        {
            landscape = new Land();
            landscape.role = Properties.Resources.angryBird;
        }

        //創建英雄
        private void NewHero()
        {
            hero = new rockMan();
            hero.X = Paramater.W / 2;
            hero.Y = (288 - 80) * 2;
        }


        private void frmOOP_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(Paramater.W * Paramater.rate, Paramater.H * Paramater.rate);
            this.DoubleBuffered = true;
            // this.KeyPreview = true;
            timer1.Interval = 40;
        }

        private void frmOOP_Paint(object sender, PaintEventArgs e)
        {
            landscape.Draw(e.Graphics);
            hero.Draw(e.Graphics);
            foreach (bullet tmp in listBullet)
                tmp.Draw(e.Graphics);
            foreach (monster tmp in listMonster)
                tmp.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.Red, 0, 0, hp * (this.Width / 100), 50);
        }

        private void frmOOP_KeyDown(object sender, KeyEventArgs e)
        {
            timer1.Enabled = true;
            if (e.KeyData == Keys.Space)
                shoot();

            if (e.KeyData == Keys.Right)
            {
                hero.dir = 0;
                heroMove = true;
                landscape.dir = hero.dir;
            }

            if (e.KeyData == Keys.Left)
            {
                hero.dir = 1;
                heroMove = true;
                landscape.dir = hero.dir;
            }
        }

        private void shoot()
        {
            bullet tmp = new bullet();
            tmp.X = hero.X;
            tmp.Y = hero.Y;
            tmp.dir = hero.dir;
            listBullet.Add(tmp);
        }

        private void frmOOP_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Left)
            {
                hero.step = 0;
                heroMove = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            #region 背景移動
            if (heroMove)
            {
                hero.Move();
                landscape.Move();
            }
            #endregion

            #region 子彈
            this.Text = "發射子彈數目:" + listBullet.Count.ToString();
            for (int i = 0; i < listBullet.Count; i++)
            {
                if (listBullet[i].X > this.Width || listBullet[i].X < 0)
                    listBullet.Remove(listBullet[i]);
                else
                {
                    listBullet[i].Move();
                }
            }
            #endregion

            #region 怪獸工廠
            monsterRaise();
            #endregion

            #region 碰撞偵測
            for (int i = 0; i < listMonster.Count; i++)
            {
                Rectangle recMonster = new Rectangle(listMonster[i].X, listMonster[i].Y, listMonster[i].sRole.Width, listMonster[i].sRole.Height);//好的方式
                Rectangle recRockman = new Rectangle(hero.X, hero.Y, 84, 80);//不好的設計方式
                if (recMonster.IntersectsWith(recRockman))
                {
                    hp--;
                    listMonster.Remove(listMonster[i]);
                    continue;
                }

                for (int j = 0; j < listBullet.Count; j++)
                {
                    Rectangle recBullet = new Rectangle(listBullet[j].X, listBullet[j].Y, 60, 60);//0.0
                    if (recBullet.IntersectsWith(recMonster))
                    {
                        listMonster.Remove(listMonster[i]);
                        listBullet.Remove(listBullet[j]);
                        break;
                    }
                }
            }
            #endregion

            this.Invalidate();
        }

        private void monsterFactory(int style)
        {
            monster tmp = new monster(Properties.Resources.monster1, Color.Black, 3, new Size(76, 117));
            tmp.X = this.Width;
            tmp.Y = hero.Y;
            listMonster.Add(tmp);
        }

        private void monsterRaise()
        {
            this.Text += "" + listMonster.Count.ToString();
            if (r.Next(100) % 9 == 0)
                monsterFactory(1);
            for (int i = 0; i < listMonster.Count; i++)
            {
                if (listMonster[i].X < 0)
                    listMonster.Remove(listMonster[i]);
                else
                    listMonster[i].Move();
            }
        }
    }

    //抽象類別
    abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Image role { get; set; }
        public Bitmap roleBmp { get; set; }
        public int step;//動畫分解圖步驟
        public int dir;//動畫方向
        public int R;//距離
        public Rectangle[,] rect { get; set; }//動畫分解圖
        public abstract void Draw(Graphics G);
        public abstract void Move();
    }

    //***靜態參數類別
    class Paramater
    {
        public static int W = Properties.Resources.angryBird.Width;
        public static int H = Properties.Resources.angryBird.Height;
        //放大倍率
        public static int rate = 2;
    }

    //地圖類別
    class Land : GameObject
    {
        Rectangle rectDest, rectSrc;
        public Land()
        {
            X = 0;
            dir = 0;
        }

        public override void Draw(Graphics G)
        {
            //throw new NotImplementedException();
            rectDest = new Rectangle(0, 0, Paramater.W * Paramater.rate, Paramater.H * Paramater.rate);
            for (int i = -1; i <= 1; i++)
            {
                rectSrc = new Rectangle(i * role.Width + (X % role.Width), 0, Paramater.W, Paramater.H);
                G.DrawImage(role, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        public override void Move()
        {
            //throw new NotImplementedException();
            if (dir == 0)
                X += 5;
            else if (dir == 1)
                X -= 5;
        }
    }

    //洛克人類別
    class rockMan : GameObject
    {
        public rockMan()
        {
            role = Properties.Resources.run84X80;
            rect = new Rectangle[2, 11];//人物圖的22張小圖矩形區域
            //計算出人物圖的22張小凸的矩形區域
            for (int i = 0; i < rect.GetLength(0); i++)
                for (int j = 0; j < rect.GetLength(1); j++)
                    rect[i, j] = new Rectangle(j * 84, i * 80, 84, 80);
        }

        public override void Draw(Graphics e)
        {
            //throw new NotImplementedException();
            if (step > 10)
                step = 0;
            e.DrawImage(role, X, Y, rect[dir, step], GraphicsUnit.Pixel);
        }

        public override void Move()
        {
            //throw new NotImplementedException();
            step++;
        }
    }

    //子彈類別
    class bullet : GameObject
    {
        public bullet()
        {
            R = -20;
            roleBmp = Properties.Resources.bullet;
            roleBmp.MakeTransparent(Color.FromArgb(0, 0, 0));
            rect = new Rectangle[1, 2];

            for (int j = 0; j < rect.GetLength(1); j++)
                rect[0, j] = new Rectangle(j * 60, 0, 60, 60);
            step = 0;
            dir = 0;
        }

        public override void Draw(Graphics e)
        {
            Rectangle r = new Rectangle(0, 0, 0, 0);
            if (dir == 0)
                r = new Rectangle(X + 84, Y + 22, 60, 60);
            else if (dir == 1)
                r = new Rectangle(X + 30, Y + 22, -60, 60);
            //throw new NotImplementedException();
            if (step > rect.GetLength(1) - 1)
                step = 0;
            e.DrawImage(roleBmp, r, rect[0, step], GraphicsUnit.Pixel);
        }

        public override void Move()
        {
            //throw new NotImplementedException();
            step++;
            if (dir == 0)
            {
                X += 30;
            }
            if (dir == 1)
                X -= 30;
        }
    }

    //怪獸類別
    class monster : GameObject
    {
        public Size sRole;

        /// <summary>
        /// 怪獸工廠
        /// </summary>
        /// <param name="bmp">圖檔</param>
        /// <param name="c">去背</param>
        /// <param name="col">數量</param>
        /// <param name="s">單圖大小</param>

        public monster(Bitmap bmp, Color c, int col, Size s)
        {
            roleBmp = bmp;
            roleBmp.MakeTransparent(c);
            sRole = s;
            rect = new Rectangle[1, col];
            for (int j = 0; j < rect.GetLength(1); j++)
                rect[0, j] = new Rectangle(j * s.Width, 0, sRole.Width, sRole.Height);
        }

        public override void Draw(Graphics e)
        {
            //throw new NotImplementedException();]
            if (step > rect.GetLength(1) - 1)
                step = 0;
            Rectangle r = new Rectangle(X, Y, sRole.Width, sRole.Height);
            e.DrawImage(roleBmp, r, rect[0, step], GraphicsUnit.Pixel);
        }

        public override void Move()
        {
            //throw new NotImplementedException();
            step++;
            X -= 20;
        }
    }
}
