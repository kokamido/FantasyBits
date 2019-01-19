namespace ConsoleApplication2
{
    public interface ILogic
    {
        void Turn(GameState state);
    }
    
    public class SimpleAi : ILogic
    {
        public void Turn(GameState state)
        {
            throw new System.NotImplementedException();
        }
    }
}