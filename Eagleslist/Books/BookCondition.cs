
namespace Eagleslist
{
    public enum BookCondition : int
    {
        New = 1 << 0,
        Great = 1 << 1,
        Good = 1 << 2,
        Okay = 1 << 3,
        Poor = 1 << 4,
        Mixed = 1 << 5
    }

    public static class BookConditionMethods
    {
        public static string ToString(this BookCondition bookCondition)
        {
            switch (bookCondition)
            {
                case BookCondition.New:
                    return "New";
                case BookCondition.Great:
                    return "Great";
                case BookCondition.Good:
                    return "Good";
                case BookCondition.Okay:
                    return "Okay";
                case BookCondition.Poor:
                    return "Poor";
                case BookCondition.Mixed:
                    return "Mixed";
                default:
                    return null;
            }
        }

        public static BookCondition FromString(string str) {
            switch (str)
            {
                case "New":
                    return BookCondition.New;
                case "Great":
                    return BookCondition.Great;
                case "Good":
                    return BookCondition.Good;
                case "Okay":
                    return BookCondition.Okay;
                case "Poor":
                    return BookCondition.Poor;
                case "Mixed":
                    return BookCondition.Mixed;
                default:
                    return 0;
            }
        }

        public static BookCondition FromInt(int i)
        {
            return (BookCondition)(1 << i);
        }
    }
}
