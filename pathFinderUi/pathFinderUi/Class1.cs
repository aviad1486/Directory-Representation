using System;

namespace DirectoryStructure
{
    public class temp
    {
        public class folder_001
        {
            public class folder_002
            {
                public readonly string _1_txt = "https://www.example.com/a1/b/1.txt";
            }
            public readonly folder_002 b = new folder_002();
        }
        public class folder_003
        {
            public class folder_004
            {
                public readonly string _2_txt = "https://www.example.com/a2/a2/2.txt";
            }
            public class folder_005
            {
                public readonly string _1_txt = "https://www.example.com/a2/b/1.txt";
            }
            public readonly folder_004 a2 = new folder_004();
            public readonly folder_005 b = new folder_005();
        }
        public readonly folder_001 a1 = new folder_001();
        public readonly folder_003 a2 = new folder_003();
        public readonly string a2_txt = "https://www.example.com/a2.txt";
    }
}
