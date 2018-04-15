using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWindow
{
    class MHash
    {
        //// Const Value ////
        const int HASHMAX = 1000;


        private UInt32 m_nCompare;
        public UInt32 MyHash(UInt32 sHashData)
        {
            UInt32 HashValue = 5381;

            UInt32 ParsingData = sHashData;
            do
            {
                m_nCompare = ParsingData % 10;
                HashValue = (((HashValue << 5) + HashValue) + m_nCompare) % HASHMAX;

                ParsingData = ParsingData / 10;
            } while (ParsingData == 0);
            return HashValue % HASHMAX;
        }

        private struct NODE
        {
            UInt32 NameData;
            int prev;
        }
    }
}
