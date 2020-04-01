namespace XJK.Converters
{
    public interface IConverter<TIn, TOut>
    {
        TOut Convert(TIn input);

        TIn ConvertBack(TOut output);
    }
}
