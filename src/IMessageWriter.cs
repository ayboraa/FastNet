namespace FastNet
{
    /// <summary>
    /// Defines methods which every <see cref="Message"/> must implement.
    /// </summary>
    public interface IMessageWriter
    {
        void Write(string data);

        void Write(int data);

        void Write(short data);

        void Write(bool data);

        void Write(byte data);

        void Write(float data);

        void Write(long data);

        void Write(ulong data);

        void Write(byte[] data);
    }
}