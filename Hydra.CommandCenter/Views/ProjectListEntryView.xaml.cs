using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hydra.CommandCenter
{
    public partial class ProjectListEntryView : UserControl
    {
        public ICommand RemoveCommand
        {
            get { return (ICommand)this.GetValue( RemoveCommandProperty ); }
            set { this.SetValue( RemoveCommandProperty, value ); }
        }

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            "RemoveCommand", typeof( ICommand ), typeof( ProjectListEntryView ), new PropertyMetadata( null ) );

        public ProjectListEntryView()
        {
            InitializeComponent();
        }
    }
}
