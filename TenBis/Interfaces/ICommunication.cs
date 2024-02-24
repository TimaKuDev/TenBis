namespace TenBis.Interfaces
{
    internal interface ICommunication
    {
        public void Notify(string message);
        bool? ValidateRunningScript();
    }
}
