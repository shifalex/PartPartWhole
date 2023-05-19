namespace PartPartWhole.Models
{
    public class PPWObject
    {
        public PPWObject(int addent1, int addent2, int sum)
        {
            Addent1 = addent1;
            Addent2 = addent2;
            Sum = sum;
        }

        public int Addent1 { get; set; }
        public int Addent2 { get; set; }
        public int Sum { get; set; }

    }
}
