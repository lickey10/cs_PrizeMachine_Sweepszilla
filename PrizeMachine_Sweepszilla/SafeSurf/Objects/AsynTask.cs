using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace SCTVObjects
{
    public static class ThreadSync
    {
        public static void InvokeDelegate(Delegate del, params object[] args)
        {
            if (del != null)
            {
                ISynchronizeInvoke synchronizer = del.Target as ISynchronizeInvoke;
                if (synchronizer != null && synchronizer.InvokeRequired)
                {
                    synchronizer.Invoke(del, args);
                    return;
                }
                del.DynamicInvoke(args);
            }
        }
    }

    public abstract class AsynTask : IDisposable
    {
        private delegate object RecurringTaskHandler();
        private delegate void RecurringTaskCompletedHandler(object state, DateTime timeStamp);
        
        #region Private Fields
        protected const int ONE_SECOND = 1000;
        private System.Threading.Timer _timer = null;
        private int _interval = 0;
        private int _timeOut = 0;
        private object _syncLock = new object();
        private bool _stateProcessing = false;
        private DateTime ? _lastUpdated = null;
        #endregion
        
        public event AsynTaskCompletedEventHandler Completed;
        
        #region Constructors 
        
        private AsynTask()
        {   } 
        
        public AsynTask( int interval )
        {
            _interval = interval;
        }

        public AsynTask(int interval, int timeout)
        {
            _interval = interval;
            _timeOut = timeout;
        }

        #endregion
        
        #region Properties
                
        public int Interval
        {
            get{ return _interval; }
        }
        
        public int Timeout
        {
            get{ return _timeOut; }
        }
        
        public DateTime ? LastUpdated
        {
            get{ return _lastUpdated; }
            internal set{ _lastUpdated = value; }
        }
        #endregion
             
        public void Start()
        {
            int timeInterval = this.Interval * ONE_SECOND;
            _timer = new System.Threading.Timer( OnTimer, null, 0, timeInterval );  
        }
        public void Kill()
        {
            if( !_stateProcessing )
            {
                _timer.Dispose();
                return;    
            } 
        }
        private void OnTimer( object state )
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;          
            if( _stateProcessing )
            {
                //Tools.WriteToFile(Tools.errorFile, "OnTimer() - Still Processing, Thread Id: {0}.", threadId );
                return;
            }
            lock( _syncLock )
            {
                if( !_stateProcessing )
                {
                    _stateProcessing = true;
                    RecurringTaskHandler recurringTaskHandler = new RecurringTaskHandler( OnRecurringTask );
                    IAsyncResult asyncResult = recurringTaskHandler.BeginInvoke( null, null );
                    int timeOut = this.Timeout * ONE_SECOND; 
                    if( asyncResult.AsyncWaitHandle.WaitOne( timeOut, false ) )
                    {
                        this.LastUpdated = DateTime.Now;                
                        object result = recurringTaskHandler.EndInvoke( asyncResult );
                        RecurringTaskCompletedHandler recurringTaskCompletedHandler = new RecurringTaskCompletedHandler( OnRecurringTaskCompleted );
                        recurringTaskCompletedHandler.BeginInvoke( result, this.LastUpdated.Value, null, null );
                    }                   
                    _stateProcessing = false;                    
                }
                else
                    return;
            }            
        }        
        protected virtual object OnRecurringTask()
        {
            object state = null;
            try
            {
                state = RecurringTask();
            }
            catch( SystemException e )
            {
                throw e;
            }
            catch( Exception e )
            {
                Debug.Write(e.Message.ToString());
            }
            return state;        
        }
        protected virtual void OnRecurringTaskCompleted(object state, DateTime timeStamp)
        {
            try
            {            
                RecurringTaskCompleted( state );
                DateTime lastUpdated = this.LastUpdated ?? DateTime.MinValue;                
                if( timeStamp >= lastUpdated )
                    ThreadSync.InvokeDelegate(Completed, this, new AsynTaskCompletedEventArgs( state, lastUpdated ));                                               
            }
            catch( SystemException e )
            {
                throw e;
            }
            catch( Exception e )
            {
                Debug.Write(e.Message.ToString());
            }        
        }       
        public abstract object RecurringTask();
        public abstract void RecurringTaskCompleted( object state );
        public void Dispose()
        {
            Kill();    
        }

    }
    
    #region Event Args & Delegates
    
    public delegate void AsynTaskCompletedEventHandler( object sender, AsynTaskCompletedEventArgs e );
    
    public class AsynTaskCompletedEventArgs : EventArgs
    {
        private readonly object _result;
        private readonly DateTime _lastUpdated;
        private AsynTaskCompletedEventArgs() : base()
        {   }

        public AsynTaskCompletedEventArgs(object result, DateTime lastUpdated)
            : this()
        {
            _result = result;
            _lastUpdated = lastUpdated;
        }
        
        public object Result
        {
            get{ return _result; }
        }

        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
        }

    }
    
    #endregion




    public class PlayerTask : AsynTask
    {
        public PlayerTask(int interval) : base(interval)
        {
        }

        public PlayerTask(int interval, int timeout, VLC vlcLib, string[] options, string file)
            : base(interval, timeout)
        {
            vlcLib.playlistClear();
            vlcLib.addTarget("vlc:quit");
            vlcLib.play();
            vlcLib.playlistClear();
            vlcLib.addTarget(file, options);
            vlcLib.play();
        }

        public PlayerTask(int interval, int timeout, VLC vlcLib, string[] options, ArrayList files)
            : base(interval, timeout)
        {
            vlcLib.playlistClear();
            vlcLib.addTarget("vlc:quit");
            vlcLib.play();
            vlcLib.playlistClear();

            foreach (string file in files)
            {
                vlcLib.addTarget(file, options);
            }

            try
            {
                vlcLib.play();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        
        public override object RecurringTask()
        {
            object[] state = new object[] { "success!" };
            return state;
        }

        public override void RecurringTaskCompleted(object state)
        {

        }
    }


}
