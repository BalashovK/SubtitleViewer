namespace SubtitleViewer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Form1 f = new Form1();
            if(args.Length > 0)
            {
                f.subtitle_file = args[0];
                if (args.Length > 1)
                {
                    f.image_file = args[1];
                }
            }

            Application.Run(f);
        }
    }
}