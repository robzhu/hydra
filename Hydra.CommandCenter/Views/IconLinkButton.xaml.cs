using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FontAwesome.WPF;

namespace Hydra.CommandCenter
{
    public partial class IconLinkButton : UserControl
    {
        public FontAwesomeIcon Icon
        {
            get { return (FontAwesomeIcon)GetValue( IconProperty ); }
            set { SetValue( IconProperty, value ); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register( "Icon", typeof( FontAwesomeIcon ), typeof( IconLinkButton ), new PropertyMetadata( FontAwesomeIcon.Stop ) );

        

        public ICommand Command
        {
            get { return (ICommand)this.GetValue( CommandProperty ); }
            set { this.SetValue( CommandProperty, value ); }
        }

        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register( "Command", typeof( ICommand ), typeof( IconLinkButton ), new PropertyMetadata( null ) );



        public string Text
        {
            get { return (string)GetValue( TextProperty ); }
            set { SetValue( TextProperty, value ); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register( "Text", typeof( string ), typeof( IconLinkButton ), new PropertyMetadata( null ) );

        

        public IconLinkButton()
        {
            InitializeComponent();
        }
    }
}
