public enum TileView
{
    /// <summary>
    /// The viewer can see sea
    /// </summary>
    /// <remarks>
    /// May be masking a ship if viewed via a sea adapter
    /// </remarks>
    Sea,

    /// <summary>
    /// The viewer knows that site was attacked but nothing
    /// was hit
    /// </summary>
    Miss,

    /// <summary>
    /// The viewer can see a ship at this site
    /// </summary>
    Ship,

    /// <summary>
    /// The viewer knows that the site was attacked and
    /// something was hit
    /// </summary>
    Hit
}