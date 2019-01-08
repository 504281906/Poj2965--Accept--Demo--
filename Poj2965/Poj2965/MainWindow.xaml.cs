using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Poj2965
{
    public class selPanel : WrapPanel
    {
        private double initW = 0;//初始宽度
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (initW > 0 && Math.Abs(initW - arrangeBounds.Width) < 30) return base.ArrangeOverride(arrangeBounds);//宽度变化明显重新计算
            initW = arrangeBounds.Width;
            double halfW = arrangeBounds.Width / 2;
            bool halfflag = true, fourflag = true;
            Size teSize = new Size(halfW, arrangeBounds.Height);//一半宽
            Size fSize = new Size(halfW / 2, arrangeBounds.Height);//1/4宽
            double minh = 0, maxh = 0;
            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(fSize);
                Size fourSize = element.DesiredSize;//宽度1/4时需要的高度

                element.Measure(teSize);
                Size halfSize = element.DesiredSize;//宽度一半时需要的高度

                element.Measure(arrangeBounds);
                Size acSize = element.DesiredSize;//完整大小时需要的高度


                minh = minh > 5 ? Math.Min(minh, acSize.Height) : acSize.Height;//5是随意给的
                maxh = Math.Max(maxh, acSize.Height);

                if (fourSize.Height > acSize.Height + 1)//+1给点冗余
                {
                    fourflag = false;
                }
                if (halfSize.Height > acSize.Height + 1)
                {
                    halfflag = false;
                }
            }
            if (maxh - minh < 15)//高度差太大，不美观，不在一行显示
            {
                if (fourflag)//可以显示四项
                {
                    this.ItemWidth = halfW / 2;
                    return new Size(0, 0);//增加性能 后面就不用算了
                }
                if (halfflag)//可以显示两项
                {
                    this.ItemWidth = halfW;
                    return new Size(0, 0);//增加性能 后面就不用算了
                }
            }

            return base.ArrangeOverride(arrangeBounds);
        }
    }

    public class Info: INotifyPropertyChanged
    {
        private bool _color;
        public bool color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("color"));
                }
            }
        }

        private int _index;

        public event PropertyChangedEventHandler PropertyChanged;

        public int index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("index"));
                }
            }
        }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Info> _gridList = new ObservableCollection<Info>();
        public ObservableCollection<Info> GridList
        {
            get
            {
                return _gridList ?? (_gridList = new ObservableCollection<Info>());
            }
            set
            {
                _gridList = value;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Random r = new Random();
            for (int i = 0; i < 16; i++)
            {
                Info temp = new Info();
                temp.index = i + 1;
                var va = r.Next(1, 3);
                if (va <= 1)
                    temp.color = true;
                else
                    temp.color = false;
                GridList.Add(temp);
            }
        }

        private void BtnGrid_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            Info temp = btn.DataContext as Info;
            if (temp == null) return;
            temp.color = !temp.color;
            for (int i = 0; i < 16; i++)
            {
                if (temp.index == i + 1) continue;
                if ((i+1)%4 == temp.index % 4)
                    GridList[i].color = !GridList[i].color;
                if (i/4 == (temp.index - 1) / 4)
                    GridList[i].color = !GridList[i].color;
                //btn.Background = System.Windows.Media.Brushes.Transparent;
            }
            GridList = GridList;
        }
    }
}