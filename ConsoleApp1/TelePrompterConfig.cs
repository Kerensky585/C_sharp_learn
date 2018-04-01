using static System.Math;

namespace TeleprompterConsole
{
    internal class TelePrompterConfig
    {

        public bool Done => Done;
        private bool done;
        
        private object lockHandle = new object();
        public int DelayInMilliSeconds { get; private set; } = 200;

        public void UpdateDelay(int increment)
        {
            var newDelay = Min(DelayInMilliSeconds + increment, 1000);
            newDelay = Max(newDelay, 20);

            lock (lockHandle)
            {
                DelayInMilliSeconds = newDelay;
            }
        }

        public void SetDone()
        {
            done = true;
        }
    }
}