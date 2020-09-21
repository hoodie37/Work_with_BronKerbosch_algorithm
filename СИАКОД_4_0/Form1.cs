using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace СИАКОД_4_0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Node> nodes = new List<Node>();
        List<Rebro> rebra = new List<Rebro>();
        //public List<Node> nodes = new List<Node>();
        int globalflag = 0;
        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            Pen pen;
            pen = new Pen(Brushes.Blue);
            pen.Width = 4;
            if (globalflag == 1)
            {
                foreach (Node v in maxR)
                {
                    for (int i = 0; i < maxR.Count; i++)
                    {
                        e.Graphics.DrawLine(pen, v.x, v.y, maxR[i].x, maxR[i].y);
                    }
                }
            }
            foreach (Rebro r in rebra)
            {
                r.draw(e.Graphics);
            }
            foreach (Node n in nodes)
            {
               n.draw(e.Graphics);
            }
            
            
            pBox.Select();
        }

        void link(Node n1,Node n2)
        {
            Rebro r = new Rebro(n1, n2);
            rebra.Add(r);
            n1.linkedN.Add(n2);
            n2.linkedN.Add(n1);
            foreach (Node no in nodes)
             {
                if (no.CheckSelect())
                { 
                    no.Select();
                }
             }
        }

        int next = 0;
        Node n1;
        Node n2;
        int count = 0;
        private void pBox_MouseClick(object sender, MouseEventArgs e)
        {
            int flag = 0;

            if (next == 0)
            {
                foreach (Node n in nodes)
                {
                    if (n.mouseInShape(e.X, e.Y) == true)
                    {
                        next++;
                        flag = 1;
                        n.Select();
                        n1 = n;
                        this.Refresh();
                        pBox.Select();
                        return;
                    }
                }
            }

            if (next == 1)
            {
                foreach (Node n in nodes)
                {
                    if (n.mouseInShape(e.X, e.Y) == true)
                    {
                        flag = 1;
                        if (n.CheckSelect() != true)
                        {
                            next = 0;
                            n.Select();
                            n2 = n;
                           
                            link(n1, n2);
                            this.Refresh();
                            pBox.Select();
                            return;
                           
                        }
                    }
                }
            }

            if (flag == 0)
            {
                Node n = new Node(e.X, e.Y, count);
                nodes.Add(n);
                count++;
            }
            this.Refresh();
            pBox.Select();
        }

        public void Wait(double seconds)
        {
            int ticks = System.Environment.TickCount + (int)Math.Round(seconds * 1000.0);
            while (System.Environment.TickCount < ticks)
            {
                Application.DoEvents();
            }
        }



        /////////////РГР//////////////// 

         Node[] P; //все вершины
         List<Node> R;  //множество, содержащее на каждом шаге рекурсии полный подграф для данного шага. Строится рекурсивно.
        List<Node> X; //множество вершин, которые уже использовались для расширения
        List<Node> maxR; // максимальная клика


        public void bralgo(Graphics g)
        {
            globalflag = 0;
            // Инициализация
            P = new Node[nodes.Count];
            nodes.CopyTo(P);
            R = new List<Node>();
            X = new List<Node>();
            maxR = new List<Node>();
            bron(P, X); // Запускаем алгоритм
            // Выводим вершины клики
            foreach (Node v in maxR)
            {
                label1.Text += v.num.ToString() + " ";
            }
            // Отрисовываем вершины клики
            foreach (Node v in maxR)
            {
                v.fillEllipse();
            }
            globalflag = 1;
        }

         void bron(Node[] Pa, List<Node> Xa)
        {
            if (Xa.Count == 0 && check(Pa))
            {
                if (R.Count > maxR.Count)
                {
                    maxR = R; // Если множества X и P пустые и найдена большая клика,то сохраняем её
                }
                R = new List<Node>();
                return;
            }
            // Для каждой вершины запускаем рекурсию, пока в P не закончатся вершины
            for (int i = 0; i < Pa.Length; ++i)
            {               if (Pa[i] != null)
                {
                    R.Add(Pa[i]);
                    bron(conj(Pa[i], Pa), conj(Pa[i], Xa)); // Запуск рекурсии с изменёнными множествами P и X
                    Pa[i] = null;
                    Xa.Add(Pa[i]);
                }
            }
        }
        // Изменение множества X
        private List<Node> conj(Node v1, List<Node> list)
        {
            List<Node> res = new List<Node>();
            foreach (Node ch in v1.linkedN)
            {
                if (list.Contains(ch))
                    res.Add(ch);
            }
            list = res;
            return list;
        }
        // Изменение множества P
        private Node[] conj(Node v1, Node[] list)
        {
            Node[] res = new Node[list.Length];
            for (int i = 0; i < res.Length; ++i)
            {
                res[i] = null;
            }
            foreach (Node ch in v1.linkedN)
            {
                if (list.Contains(ch))
                {
                    int i = Array.IndexOf(list, ch);
                    res[i] = ch;
                }
            }
            list = res;
            return list;
        }
        // Проверка множества P на наличие ненулевых вершин
        private bool check(Node[] A)
        {
            bool ch = true;
            foreach (Node v in A)
                if (v != null)
                {
                    ch = false;
                    break;
                }
            return ch;
        }

        //private List<Node> haha(List<Node> a, Node v)
        //{
        //    List<Node> list = a;
        //    if (!a.Contains(v))
        //        list.Add(v);
        //    a = list;
        //    return a;
        //}
        ///////////////////КОНЕЦ РГР//////////////////////


        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = pBox.CreateGraphics(); ;
            bralgo(g);
            this.Refresh();
            pBox.Select();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            count = 0;
            rebra.Clear();
            nodes.Clear();
            label1.Text = "";
            globalflag = 0;
            this.Refresh();
        }

      
    }
}
