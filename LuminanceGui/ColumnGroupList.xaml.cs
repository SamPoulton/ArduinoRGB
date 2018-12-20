using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ArduinoRGBGui
{
    /// <summary>
    /// Interaction logic for ColumnGroupList.xaml
    /// </summary>
    public partial class ColumnGroupList : UserControl
    {
        public List<string[]> Items = new List<string[]>();
        public List<string> ColumnHeaders;
        public ColumnGroupList()
        {
            InitializeComponent();
        }

        public void SetColumnHeaders(params string[] items)
        {
            ColumnHeaders = new List<string>();
            foreach (string item in items) ColumnHeaders.Add(item);
            while (HeaderGrid.ColumnDefinitions.Count != ColumnHeaders.Count)
                HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100)});
            for (int i = 0; i <= ColumnHeaders.Count - 1; i++)
            {
                Label tempLabel = new Label();
                tempLabel.FontSize = 18;
                tempLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
                tempLabel.SetValue(Grid.ColumnProperty, i);
                tempLabel.Content = ColumnHeaders[i];
                HeaderGrid.Children.Add(tempLabel);
            }
        }

        public void AddItem(params string[] items)
        {
            Items.Add(items);
            RefreshList();
        }

        public void RefreshList()
        {
            ItemList.Items.Clear();
            foreach (string[] item in Items)
            {
                Grid tempGrid = new Grid();
                tempGrid.Height = 30;
                foreach (ColumnDefinition def in HeaderGrid.ColumnDefinitions)
                    tempGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = def.Width});
                for (int count = 0; count <= item.Length - 1; count++)
                {
                    Label tempLabel = new Label();
                    tempLabel.Foreground = new SolidColorBrush(Colors.White);
                    tempLabel.Content = item[count];
                    tempLabel.SetValue(Grid.ColumnProperty, count);
                }

                ItemList.Items.Add(tempGrid);
            }
        } 
    }
}
