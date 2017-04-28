namespace TCP_Framework
{
    public static class ServerFactory
    {
        public static IServer Server { get; set; }

        public static IServer GenerateServer()
        {
            return Server ?? new Server();
        }
    }
}