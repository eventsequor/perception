namespace Gpm.Adapter.Internal
{
    public abstract class AdapterBase
    {
        protected abstract string Domain
        {
            get;
        }

        protected abstract string Version
        {
            get;
        }

        public AdapterBase()
        {
            LoggerMapper.Debug(string.Format("{0} ver.{1}", Domain, Version), GetType());
        }
    }
}