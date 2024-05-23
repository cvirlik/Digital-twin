namespace Digital_twin.Dataset.Support
{
    public class IdGenerator
    {
        private long _currentIdNode = 1010100000000000000;
        private long _currentIdWay = 1000000000000000000;

        public long GenerateIdNode()
        {
            return _currentIdNode++;
        }
        
        public long GenerateIdWay()
        {
            return _currentIdWay++;
        }
    }
}
