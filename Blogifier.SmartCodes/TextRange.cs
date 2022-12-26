namespace Framework.Providers.Wiki.Interprete;

public class TextRange
{
    public int From { get; set; }
    public int To { get { return From + Length - 1; } }
    public int Length { get; set; }

    public override string ToString()
    {
        return $"[{From}, {Length}]";
    }

    public TextRange()
        : base()
    {

    }

    public TextRange(int inicio, int largo)
    {
        this.From = inicio;
        this.Length = largo;
    }
}
