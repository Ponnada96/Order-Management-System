namespace Orders.Domain.DbContext
{
    /// <summary>
    /// User
    /// </summary>
    public partial class User
    {
        public bool isBuyer { get; set; }
        public bool isAdmin { get; set; }
    }
}
