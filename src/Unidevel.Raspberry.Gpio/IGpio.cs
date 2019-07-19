namespace Unidevel.Raspberry
{
    public interface IGpio
    {
        /// <summary>
        /// Sets PIN to high (true) or low (false).
        /// </summary>
        /// <param name="gpioPinNumber">Pin to set or get value from. Must be between 1 and 32, inclusive.</param>
        /// <returns>Pin value.</returns>
        bool this[int gpioPinNumber] { get;set; }
    }
}
