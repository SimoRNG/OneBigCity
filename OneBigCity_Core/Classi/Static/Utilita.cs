namespace OneBigCity_Model.Classi.Static;

public static class Utilita
{
    private static readonly Random rng = new Random();
    public static int RNG(int lowerBound, int upperBound)
    {
        return rng.Next(lowerBound,upperBound);
    }
}
