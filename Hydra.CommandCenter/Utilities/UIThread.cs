using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Hydra.CommandCenter
{
    public static class DispatcherObjectExtensions
    {
        public static void CheckedInvoke( this DispatcherObject dispatcherObject, Action action )
        {
            dispatcherObject.Dispatcher.CheckedInvoke( action );
        }
    }

    public static class DispatcherExtensions
    {
        public static void CheckedInvoke( this Dispatcher dispatcher, Action action )
        {
            if( action == null ) return;

            if( dispatcher.CheckAccess() )
            {
                action();
            }
            else
            {
                dispatcher.Invoke( (ThreadStart)( () =>
                {
                    action();
                } ) );
            }
        }
    }

    public static class UIThread
    {
        private static Dispatcher _applicationDispatcher = null;

        public static void Marshall( Action action )
        {
            if( action == null ) return;

            if( ( _applicationDispatcher == null ) && ( Application.Current != null ) )
            {
                _applicationDispatcher = Application.Current.Dispatcher;
            }

            if( _applicationDispatcher != null )
            {
                _applicationDispatcher.CheckedInvoke( action );
            }
            else
            {
                action();
            }
        }
    }
}
