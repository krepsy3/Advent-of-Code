namespace Common
{
    public class InputLoader
    {
        public enum State { Init, Loading, Finished }

        public int Day { get; init; }

        public InputLoader() { }
        public InputLoader(int day) { Day = day; }

        private StreamReader reader = null;
        private string currentLine = null;
        private State state = State.Init;

        public bool BeginLoading()
        {
            if (state != State.Init)
            {
                return false;
            }

            FileInfo fi = new FileInfo(Path.Combine("Inputs", Day.ToString("00") + ".txt"));
            if (!fi.Exists)
            {
                return false;
            }

            state = State.Loading;
            reader = new StreamReader(fi.OpenRead());
            return true;
        }

        public bool NextLine()
        {
            if (state != State.Loading)
            {
                return false;
            }

            var line = reader.ReadLine();
            if (line is null)
            {
                state = State.Finished;
                try
                {
                    reader.Dispose();
                }

                catch { }
            }

            else
            {
                currentLine = line;
            }

            return true;
        }

        public string GetLine() => currentLine;

        public string GetNextLine()
        {
            NextLine();
            if (state == State.Loading)
                return GetLine();

            return null;
        }

        public bool Reset()
        {
            if (state == State.Init)
            {
                return false;
            }

            if (reader is not null)
            {
                try
                {
                    reader.Dispose();
                }

                catch { }
            }

            currentLine = null;
            state = State.Init;
            return BeginLoading();
        }

        public bool EndLoading()
        {
            if (state == State.Init)
            {
                return false;
            }

            if (reader is not null)
            {
                try
                {
                    reader.Dispose();
                }

                catch { }
            }

            currentLine = null;
            state = State.Finished;
            return true;
        }
    }
}