using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hydra.CommandCenter
{
    public partial class AppSummaryView : UserControl
    {
        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue( RemoveCommandProperty ); }
            set { SetValue( RemoveCommandProperty, value ); }
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register( "RemoveCommand", typeof( ICommand ), typeof( AppSummaryView ), new PropertyMetadata( null ) );

        public AppSummaryView()
        {
            InitializeComponent();
        }
    }
}
