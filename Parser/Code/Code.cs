namespace Wall_E
{
    #region BaseCommandClass

    public interface ICode
    {
        int Line { get; set; }
        void Execute(Executor executor); // Ya implementado para lógicas
    }


    #endregion
}