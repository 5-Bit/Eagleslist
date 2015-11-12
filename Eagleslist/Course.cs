using System.Collections.Generic;

namespace Eagleslist
{
    public class Course
    {

        public int CourseID { get; private set; }
        public string CRN { get; private set; }
        public List<Book> Books { get; private set; }
        public string Professor { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        
    }
}
