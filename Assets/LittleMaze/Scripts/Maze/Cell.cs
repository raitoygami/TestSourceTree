
public struct P
{
    public int x;
    public int y;
    public P(int x, int y) { this.x = x; this.y = y; }
    public static P zero { get { return new P(0, 0); } }
    public static P one { get { return new P(1, 1); } }

    public static bool operator ==(P lhs, P rhs) { return lhs.x == rhs.x && lhs.y == rhs.y; }
    public static bool operator !=(P lhs, P rhs) { return lhs.x != rhs.x || lhs.y != rhs.y; }

    public static P operator +(P lhs, P rhs) { return new P(lhs.x + rhs.x, lhs.y + rhs.y); }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public struct Cell
{
    public int x;
    public int y;
    public bool bBlocked;
    public bool isPass;
    public int step;
    public bool bVisited;
}
