namespace Game
{
    public class EntityMovementArgs
    {
        public readonly int DX, DY;
        public EntityMovementArgs(int dX, int dY)
        {
            DX = dX;
            DY = dY;
        }
    }
}