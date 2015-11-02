using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTClient
{
    public class GloveDataPacket
    {

        public static int INDEX_TYPE = 0;
        public static int INDEX_GESTURE = 1;
        public static int INDEX_FINGERS = 2;

        public static int INDEX_HEADING = 4;
        public static int INDEX_PITCH = 8;
        public static int INDEX_ROLL = 12;

        public static int TYPE_DATA = 0;
        public static int TYPE_ACK = 1;
        public static int TYPE_CALIBRATION = 2;

        public int type;
        public int gesture;
        public int finger0;
        public int finger1;
        public int finger2;
        public double heading;
        public double pitch;
        public double roll;

        //[FIXME: exception in case of error?]
        public GloveDataPacket(byte[] data, int len)
        {
            foreach (var item in data)
            {
                Console.Write(item.ToString() + ", ");
            }
            Console.WriteLine("");
            //Log.i("GLOVE", Arrays.toString(data));
            type = data[INDEX_TYPE];
            if (type == TYPE_DATA)
            {
                gesture = data[INDEX_GESTURE];

                int fingers = BitConverter.ToUInt16(data, INDEX_FINGERS);
                finger0 = (int)(fingers / 100);
                finger1 = (int)((fingers / 10) % 10);
                finger2 = (int)(fingers % 10);

                heading = BitConverter.ToDouble(data, INDEX_HEADING);
                heading = ToDegrees(heading);

                pitch = BitConverter.ToDouble(data, INDEX_PITCH);
                pitch = ToDegrees(pitch);

                roll = BitConverter.ToDouble(data, INDEX_ROLL);
                roll = ToDegrees(roll);

            }
        }
        private double ToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

    }
}
