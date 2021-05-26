namespace Game
{
    public class MovementArgs : CustomEventArgs
    {
        public readonly int DX, DY;
        public MovementArgs(int dX, int dY)
        {
            DX = dX;
            DY = dY;
        }
    }
}