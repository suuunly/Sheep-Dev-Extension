namespace SDE.Mesh
{
    public enum EPolygonResult
    {
        SUCCESSFUL = 0,
        NORMAL_INCORRECT_FACING,
        LESS_THAN_MIN_VERTEX,

        COLLIDED_WITH_OBJECT,
        BELOW_MINIMUM_BOUNDARY_SIZE
    }
}

