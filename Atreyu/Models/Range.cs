namespace Atreyu.Models
{
    using ReactiveUI;

    /// <summary>
    /// The range.
    /// </summary>
    public class Range<T> : ReactiveObject
    {
        private T start;

        private T end;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public Range(T start, T end)
        {
            this.Start = start;
            this.End = end;
        }

        public T Start
        {
            get => this.start;
            set => this.RaiseAndSetIfChanged(ref this.start, value);
        }

        public T End
        {
            get => this.end;
            set => this.RaiseAndSetIfChanged(ref this.end, value);
        }

        #endregion
    }
}