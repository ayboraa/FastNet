namespace FastNet
{
    /// <summary>
    /// Defines methods which every <see cref="Message"/> must implement.
    /// </summary>
    public interface IMessageReader
    {

        int ReadInt32();

        short ReadInt16();

        bool ReadBoolean();

        byte ReadByte();

        string ReadString();

        float ReadFloat();

        long ReadInt64();

        ulong ReadUInt64();

        byte[] ReadByteArray();

    }
}