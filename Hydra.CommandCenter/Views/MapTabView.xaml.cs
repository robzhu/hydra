using System.Windows;
using System.Windows.Controls;

namespace Hydra.CommandCenter
{
    public partial class MapTabView : UserControl
    {
        public MapTabView()
        {
            InitializeComponent();
            //DataContextChanged += MapTabView_DataContextChanged;
        }

        //void MapTabView_DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
        //{
        //    MapPageViewModel vm = e.NewValue as MapPageViewModel;
        //    if( vm != null )
        //    {
        //        vm.View = this;
        //    }
        //}
    }
}
