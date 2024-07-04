using System;
using System.Text;

namespace FastNet
{

    /// <summary>
    /// Provides methods to write to or read from the buffer.
    /// </summary>
    public class Message : IMessageReader, IMessageWriter, IDisposable
    {

        private byte[] _buf;
        private int cursor = 0;
        private int defaultBufferSize = 1024;

        public Message(int size, byte[]? buf)
        {
            if (buf != null)
                _buf = buf;
            else
                _buf = new byte[size];

        }

        public Message(int size)
        {
            _buf = new byte[size];
        }

        public Message()
        {
            _buf = new byte[defaultBufferSize];
        }

        public byte[] GetBuffer() {


            return _buf;

        }

        public int ReadInt32()
        {
            int int32 = BitConverter.ToInt32(this._buf, this.cursor);
            this.cursor += 4;
            return int32;
        }

        public short ReadInt16()
        {
            int int16 = (int)BitConverter.ToInt16(this._buf, this.cursor);
            this.cursor += 2;
            return (short)int16;
        }

        public bool ReadBoolean()
        {
            int num = BitConverter.ToBoolean(this._buf, this.cursor) ? 1 : 0;
            ++this.cursor;
            return num != 0;
        }

        public byte ReadByte()
        {
            int num = (int)this._buf[this.cursor];
            ++this.cursor;
            return (byte)num;
        }

        public string ReadString()
        {
            int count = this.ReadInt32();
            string str = (string)null;
            if (count >= 0)
            {
                str = Encoding.UTF8.GetString(this._buf, this.cursor, count);
                this.cursor += count;
            }
            return str;
        }

        public float ReadFloat()
        {
            double single = (double)BitConverter.ToSingle(this._buf, this.cursor);
            this.cursor += 4;
            return (float)single;
        }

        public long ReadInt64()
        {
            long int64 = BitConverter.ToInt64(this._buf, this.cursor);
            this.cursor += 8;
            return int64;
        }

        public ulong ReadUInt64()
        {
            long uint64 = (long)BitConverter.ToUInt64(this._buf, this.cursor);
            this.cursor += 8;
            return (ulong)uint64;
        }

        public byte[] ReadByteArray()
        {
            byte[] numArray = new byte[this.ReadInt32()];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = this._buf[this.cursor + index];
            this.cursor += numArray.Length;
            return numArray;
        }

        public void Write(string data)
        {
            int writeCursor = this.cursor;
            this.cursor += 4;
            int num;
            if (data != null)
            {
                num = Encoding.UTF8.GetBytes(data, 0, data.Length, this._buf, this.cursor);
                this.cursor += num;
            }
            else
                num = -1;
            byte[] bytes = BitConverter.GetBytes(num);
            this._buf[writeCursor] = bytes[0];
            this._buf[writeCursor + 1] = bytes[1];
            this._buf[writeCursor + 2] = bytes[2];
            this._buf[writeCursor + 3] = bytes[3];
        }

        public void Write(int data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this._buf[this.cursor] = bytes[0];
            this._buf[this.cursor + 1] = bytes[1];
            this._buf[this.cursor + 2] = bytes[2];
            this._buf[this.cursor + 3] = bytes[3];
            this.cursor += 4;
        }

        public void Write(short data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this._buf[this.cursor] = bytes[0];
            this._buf[this.cursor + 1] = bytes[1];
            this.cursor += 2;
        }

        public void Write(bool data)
        {
            this._buf[this.cursor] = BitConverter.GetBytes(data)[0];
            ++this.cursor;
        }

        public void Write(byte data)
        {
            this._buf[this.cursor] = data;
            ++this.cursor;
        }

        public void Write(float data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this._buf[this.cursor] = bytes[0];
            this._buf[this.cursor + 1] = bytes[1];
            this._buf[this.cursor + 2] = bytes[2];
            this._buf[this.cursor + 3] = bytes[3];
            this.cursor += 4;
        }

        public void Write(long data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this._buf[this.cursor] = bytes[0];
            this._buf[this.cursor + 1] = bytes[1];
            this._buf[this.cursor + 2] = bytes[2];
            this._buf[this.cursor + 3] = bytes[3];
            this._buf[this.cursor + 4] = bytes[4];
            this._buf[this.cursor + 5] = bytes[5];
            this._buf[this.cursor + 6] = bytes[6];
            this._buf[this.cursor + 7] = bytes[7];
            this.cursor += 8;
        }

        public void Write(ulong data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this._buf[this.cursor] = bytes[0];
            this._buf[this.cursor + 1] = bytes[1];
            this._buf[this.cursor + 2] = bytes[2];
            this._buf[this.cursor + 3] = bytes[3];
            this._buf[this.cursor + 4] = bytes[4];
            this._buf[this.cursor + 5] = bytes[5];
            this._buf[this.cursor + 6] = bytes[6];
            this._buf[this.cursor + 7] = bytes[7];
            this.cursor += 8;
        }

        public void Write(Guid data)
        {
            byte[] byteArray = data.ToByteArray();
            for (int index = 0; index < byteArray.Length; ++index)
                this._buf[this.cursor + index] = byteArray[index];
            this.cursor += byteArray.Length;
        }

        public void Write(byte[] data)
        {
            this.Write(data.Length);
            for (int index = 0; index < data.Length; ++index)
                this._buf[this.cursor + index] = data[index];
            this.cursor += data.Length;
        }

        public void Dispose()
        {
            _buf = null;
        }
    }
}
