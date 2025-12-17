
namespace CMaurer.Common
{
    public class MksSafeData : MksItem
    {
        /// <summary>
        /// The SAFE summary
        /// </summary>
        public string _topic = "";

        /// <summary>
        /// The SAFE - 1st assigned user
        /// </summary>
        public MksUserData _assignedUser = new MksUserData();

        /// <summary>
        /// The SAFE - 1st assigned Tester
        /// </summary>
        public MksUserData _assignedTester = new MksUserData();

        public override string ToString()
        {
            return base.ToString() + ": " + _id + "," + _assignedUser.ToString();
        }
    }
}

